/*
 * Nome del progetto: SIASS
 * Copyright (C) 2025 Agenzia regionale per la protezione dell'ambiente ligure
 *
 * Questo programma è software libero: puoi ridistribuirlo e/o modificarlo
 * secondo i termini della GNU Affero General Public License pubblicata dalla
 * Free Software Foundation, sia la versione 3 della licenza, sia (a tua scelta)
 * qualsiasi versione successiva.
 *
 * Questo programma è distribuito nella speranza che possa essere utile,
 * ma SENZA ALCUNA GARANZIA; senza nemmeno la garanzia implicita di
 * COMMERCIABILITÀ o IDONEITÀ PER UNO SCOPO PARTICOLARE. Vedi la
 * GNU Affero General Public License per ulteriori dettagli.
 *
 * Dovresti aver ricevuto una copia della GNU Affero General Public License
 * insieme a questo programma. In caso contrario, vedi <https://www.gnu.org/licenses/>.
*/

using NLog;
using Oracle.ManagedDataAccess.Client;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SIASS
{
    internal static class ALIMSManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Seleziona le date di cui visualizzare i campionamenti per la stazione indicata
        /// e nell'intorno della data intervento; due date precedenti e due successive
        /// </summary>
        /// <param name="codiceStazione"></param>
        /// <param name="dataIntervento"></param>
        /// <returns></returns>
        private static List<DateTime> DateCampionamentiDaSelezionare(string codiceStazione, DateTime dataIntervento)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - CodiceStazione: {codiceStazione} DataIntervento: {dataIntervento}");

            string testoComando = "select distinct " +
                "trunc(data_campionamento) data " +
                "from alims_dati " +
                "where stazione = :codice_stazione " +
                "order by data";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("codice_stazione", codiceStazione));

            SIASSEntities context = new SIASSEntities();
            string stringaConnessioneDB = context.Database.Connection.ConnectionString;

            cmd.Connection = new OracleConnection(stringaConnessioneDB);

            cmd.Connection.Open();

            DataSet dataSet = new DataSet();

            using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
            {
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataSet);
            }

            cmd.Connection.Close();

            var dateCampionamentiPerStazione =
                (from DataRow dr in dataSet.Tables[0].Rows
                 select Convert.ToDateTime(dr["data"]))
                 .ToList();

            var dueDateCampionamentiAntecedentiIntervento = dateCampionamentiPerStazione
                .Where(i => i < dataIntervento.Date)
                .OrderByDescending(i => i)
                .Take(2)
                .ToList();
            var dueDateCampionamentiSuccessiveIntervento = dateCampionamentiPerStazione
                .Where(i => i > dataIntervento.Date)
                .Take(2)
                .ToList();

            var elenco = new List<DateTime>() { dataIntervento.Date };

            elenco.AddRange(dueDateCampionamentiAntecedentiIntervento);
            elenco.AddRange(dueDateCampionamentiSuccessiveIntervento);

            elenco.Sort();

            return elenco;
        }

        /// <summary>
        /// Costruzione dell'argomento da usare nella clausola PIVOT compresi in nomi di colonna
        /// </summary>
        /// <param name="dateCampionamentiDaSelezionare"></param>
        /// <returns></returns>
        private static string ArgomentoClausolaINPivot(List<DateTime> dateCampionamentiDaSelezionare)
        {
            var sb = new StringBuilder();
            foreach (var data in dateCampionamentiDaSelezionare)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.Append($"date '{data:yyyy-MM-dd}' D{data:dd_MM_yyyy}");
            }

            return sb.ToString();
        }

        public static DataTable ParametriPerStazioneEDataIntervento(string codiceStazione, DateTime dataIntervento)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - CodiceStazione:{codiceStazione} DataIntervento:{dataIntervento}");

            var dateCampionamentiDaSelezionare = DateCampionamentiDaSelezionare(codiceStazione, dataIntervento);

            string testoComando = "select * from " +
                "(" +
                    "select trunc(data_campionamento) data, parametro || ' (' || unita_misura || ')' parametro, valore " +
                    "from alims_dati " +
                    "where stazione = :codice_stazione " +
                    "and trunc(data_campionamento) between :data_primo_campionamento and :data_ultimo_campionamento" +
                ") " +
                "pivot " +
                "(" +
                    "max(valore) " +
                    $"for data in ({ArgomentoClausolaINPivot(dateCampionamentiDaSelezionare)}) " + // Il bind di parametri non è supportato dentro una PIVOT
                 ") " +
                 "order by parametro";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("codice_stazione", codiceStazione));
            cmd.Parameters.Add(new OracleParameter("data_primo_campionamento", dateCampionamentiDaSelezionare.First()));
            cmd.Parameters.Add(new OracleParameter("data_ultimo_campionamento", dateCampionamentiDaSelezionare.Last()));

            SIASSEntities context = new SIASSEntities();
            string stringaConnessioneDB = context.Database.Connection.ConnectionString;

            cmd.Connection = new OracleConnection(stringaConnessioneDB);
            cmd.Connection.Open();

            DataSet dataSet = new DataSet();

            using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
            {
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataSet);
            }

            cmd.Connection.Close();

            return dataSet.Tables[0];
        }

        /// <summary>
        /// Data una stazione e un anno (opzionale) ritorna i relativi parametri alims 
        /// </summary>
        /// <param name="codiceStazione"></param>
        /// <param name="anno"></param>
        /// <returns></returns>
        public static DataTable ParametriPerStazioneEAnno(string codiceStazione, decimal? anno = null)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - CodiceStazione:{codiceStazione} Anno:{anno}");

            string testoComando = "SELECT TRUNC(data_campionamento) data, parametro || ' (' || unita_misura || ')' parametro, valore " +
                    "FROM alims_dati " +
                    "WHERE stazione = :codice_stazione ";
            if (anno != null)
                testoComando += $"AND EXTRACT(YEAR FROM data_campionamento) = :anno ";
            testoComando += "ORDER BY data_campionamento desc, parametro";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("codice_stazione", codiceStazione));
            if (anno != null)
                cmd.Parameters.Add(new OracleParameter("anno", anno));

            SIASSEntities context = new SIASSEntities();
            string stringaConnessioneDB = context.Database.Connection.ConnectionString;

            cmd.Connection = new OracleConnection(stringaConnessioneDB);
            cmd.Connection.Open();

            DataSet dataSet = new DataSet();

            using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
            {
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataSet);
            }

            cmd.Connection.Close();

            return dataSet.Tables[0];
        }

        /// <summary>
        /// Data una stazione ritorna gli anni per cui esistono parametri alims
        /// </summary>
        /// <param name="codiceStazione"></param>
        /// <returns></returns>
        public static DataTable AnniConDatiPerStazione(string codiceStazione)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - CodiceStazione:{codiceStazione}");

            string testoComando = "SELECT DISTINCT(EXTRACT(YEAR FROM data_campionamento)) anno " +
                "FROM alims_dati " +
                "WHERE stazione = :codice_stazione " +
                "ORDER BY anno DESC";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("codice_stazione", codiceStazione));

            SIASSEntities context = new SIASSEntities();
            string stringaConnessioneDB = context.Database.Connection.ConnectionString;

            cmd.Connection = new OracleConnection(stringaConnessioneDB);
            cmd.Connection.Open();

            DataSet dataSet = new DataSet();

            using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
            {
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataSet);
            }

            cmd.Connection.Close();

            return dataSet.Tables[0];
        }

        #region Pacchetti ALIMS

        /// <summary>
        /// Elenco dei pacchetti forniti da ALIMS
        /// </summary>
        /// <returns></returns>
        public static List<InfoPacchettoAlims> ElencoPacchettiAlims()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select distinct sigla_pacchetto, sigla_pacchetto || ' - ' || nome_pacchetto as sigla_e_nome_pacchetto " +
                    "from alims_pacchetti_contenitori order by sigla_pacchetto";
                return context.Database.SqlQuery<InfoPacchettoAlims>(query).ToList();
            }
        }

        /// <summary>
        /// Elenco dei parametri forniti da ALIMS per un dato pacchetto
        /// </summary>
        /// <param name="siglaPacchetto"></param>
        /// <returns></returns>
        public static List<InfoParametroAlims> ParametriAlimsPacchetto(string siglaPacchetto)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select distinct parametro, metodo, udm, ldq, " +
                    "limite_indic, limite_minimo, limite_massimo, " +
                    "laboratorio_analisi " +
                    "from " +
                    "alims_pacchetti_contenitori " +
                    "where sigla_pacchetto = :sigla_pacchetto " +
                    "order by parametro, metodo";
                return context.Database.SqlQuery<InfoParametroAlims>(query, new OracleParameter("@sigla_pacchetto", siglaPacchetto)).ToList();
            }
        }

        /// <summary>
        /// Elenco dei contenitori forniti da ALIMS per un dato pacchetto
        /// </summary>
        /// <param name="siglaPacchetto"></param>
        /// <returns></returns>
        public static List<InfoContenitoreAlims> ContenitoriAlimsPacchetto(string siglaPacchetto)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select distinct " +
                    "tipo_contenitore, numero_contenitori " +
                    "from alims_pacchetti_contenitori " +
                    "where sigla_pacchetto = :sigla_pacchetto and (tipo_contenitore is not null and numero_contenitori is not null) " +
                    "order by tipo_contenitore";
                return context.Database.SqlQuery<InfoContenitoreAlims>(query, new OracleParameter("@sigla_pacchetto", siglaPacchetto)).ToList();
            }
        }

        #endregion Pacchetti ALIMS
    }
}
