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

using DocumentFormat.OpenXml.Drawing.Charts;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SIASSImportOrganoclorurati
{
    internal class DAL
    {
        #region Stati importazione

        public const string IMPORTAZIONE_IN_CORSO = "In corso";
        public const string IMPORTAZIONE_RIUSCITA = "Riuscita";
        public const string IMPORTAZIONE_FALLITA = "Fallita";

        #endregion
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Elenco importazioni
        /// </summary>
        /// <returns></returns>
        internal static List<Importazione> ElencoImportazioniDaElaborare()
        {
            logger.Debug($"Call {MethodBase.GetCurrentMethod().Name}");

            string testoComando = $"select * from v_import_organoclorurati where stato = '{IMPORTAZIONE_IN_CORSO}' order by data_ricezione_file";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            OracleDataReader dr = SQLHelper.CreaReader(cmd, Properties.Settings.Default.StringaConnessioneDB);

            List<Importazione> importazioni = new List<Importazione>();
            while (dr.Read())
            {
                var importazione = new Importazione
                {
                    IdImportazione = dr.GetDecimal(dr.GetOrdinal("id_importazione")),
                    NomeFile = dr.GetString(dr.GetOrdinal("nome_file")),
                    DataRicezioneFile = dr.GetDateTime(dr.GetOrdinal("data_ricezione_file")),
                    Operatore = dr.GetString(dr.GetOrdinal("operatore"))
                };
                importazioni.Add(importazione);
            }
            logger.Info($"Trovate n° {importazioni.Count} importazioni da elaborare");
            return importazioni;
        }

        internal static void InserimentoMisurazione(MisurazioneOrganoclorurati misurazione, decimal idImportazione)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Parametro: {misurazione.Parametro} Valore: {misurazione.ConcVal} N° riga file: {misurazione.NumeroRigaFile}");

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true
            };

            string testoComando = "insert into sias_organoclorurati " +
                "(" +
                    "sito, " +
                    "piezometro, " +
                    "data_campionamento, " +
                    "fonte, " +
                    "parametro, " +
                    "conc_sign, " +
                    "conc_val, " +
                    "loq, " +
                    "incertezza, " +
                    "conc_udm, " +
                    "id_importazione, " +
                    "data_aggiornamento " +
                ") " +
                "values " +
                "( " +
                    ":sito, " +
                    ":piezometro, " +
                    ":data_campionamento, " +
                    ":fonte, " +
                    ":parametro, " +
                    ":conc_sign, " +
                    ":conc_val, " +
                    ":loq, " +
                    ":incertezza, " +
                    ":conc_udm, " +
                    ":id_importazione, " +
                    "sysdate " +
                ") " +
                "returning id_misurazione into :id_misurazione";

            cmd.CommandText = testoComando;

            cmd.Parameters.Add(new OracleParameter("sito", misurazione.Sito));
            cmd.Parameters.Add(new OracleParameter("piezometro", misurazione.Piezometro));
            cmd.Parameters.Add(new OracleParameter("data_campionamento", misurazione.DataCampionamento.Date));
            cmd.Parameters.Add(new OracleParameter("fonte", misurazione.Fonte));
            cmd.Parameters.Add(new OracleParameter("parametro", misurazione.Parametro));
            cmd.Parameters.Add(new OracleParameter("conc_sign", misurazione.ConcSign));
            cmd.Parameters.Add(new OracleParameter("conc_val", misurazione.ConcVal));
            cmd.Parameters.Add(new OracleParameter("loq", misurazione.Loq));
            cmd.Parameters.Add(new OracleParameter("incertezza", misurazione.Incertezza));
            cmd.Parameters.Add(new OracleParameter("conc_udm", misurazione.ConcUdm));
            cmd.Parameters.Add(new OracleParameter("id_importazione", idImportazione));

            // parametro di ritorno id_misurazione
            OracleParameter parametroIdMisurazione = new OracleParameter("id_misurazione", OracleDbType.Decimal, ParameterDirection.ReturnValue);
            cmd.Parameters.Add(parametroIdMisurazione);

            cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            logger.Trace($"Inserita misurazione con id_misurazione: {decimal.Parse(parametroIdMisurazione.Value.ToString())}");
        }

        /// <summary>
        /// Aggiorna il record identificato dai campi chiave
        /// </summary>
        /// <param name="misurazione"></param>
        /// <param name="idImportazione"></param>
        /// <returns>true se il record è aggiornato</returns>
        internal static bool AggiornamentoMisurazione(MisurazioneOrganoclorurati misurazione, decimal idImportazione)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Parametro: {misurazione.Parametro} Valore: {misurazione.ConcVal} N° riga file: {misurazione.NumeroRigaFile}");

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true
            };

            string testoComando = "update sias_organoclorurati set " +
                    "fonte = :fonte, " +
                    "parametro = :parametro, " +
                    "conc_sign = :conc_sign, " +
                    "conc_val = :conc_val, " +
                    "loq = :loq, " +
                    "incertezza = :incertezza, " +
                    "conc_udm = :conc_udm, " +
                    "id_importazione = :id_importazione, " +
                    "data_aggiornamento = sysdate " +
                "where " +
                    "upper(sito) = upper(:sito) and " +
                    "upper(piezometro) = upper(:piezometro) and " +
                    "data_campionamento = :data_campionamento and " +
                    "upper(parametro) = upper(:parametro) and " +
			        "upper(fonte) = upper(:fonte)";

			cmd.CommandText = testoComando;

            cmd.Parameters.Add(new OracleParameter("sito", misurazione.Sito));
            cmd.Parameters.Add(new OracleParameter("piezometro", misurazione.Piezometro));
            cmd.Parameters.Add(new OracleParameter("data_campionamento", misurazione.DataCampionamento.Date));
            cmd.Parameters.Add(new OracleParameter("fonte", misurazione.Fonte));
            cmd.Parameters.Add(new OracleParameter("parametro", misurazione.Parametro));
            cmd.Parameters.Add(new OracleParameter("conc_sign", misurazione.ConcSign));
            cmd.Parameters.Add(new OracleParameter("conc_val", misurazione.ConcVal));
            cmd.Parameters.Add(new OracleParameter("loq", misurazione.Loq));
            cmd.Parameters.Add(new OracleParameter("incertezza", misurazione.Incertezza));
            cmd.Parameters.Add(new OracleParameter("conc_udm", misurazione.ConcUdm));
            cmd.Parameters.Add(new OracleParameter("id_importazione", idImportazione));

            cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
            cmd.Connection.Open();
            int numeroRecordAggiornati = cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            if (numeroRecordAggiornati == 1)
                return true;
            
            // Errore nell'aggiornamento 
            if (numeroRecordAggiornati > 1)
                throw new ApplicationException($"Parametro: {misurazione.Parametro} Valore: {misurazione.ConcVal} N° riga file: {misurazione.NumeroRigaFile} - Errore: aggiornati {numeroRecordAggiornati} record");

            return false;
        }

        internal static void AggiornamentoStatoImportazione(decimal idImportazione, string stato)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Id importazione: {idImportazione} Stato: {stato}");

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true
            };

            string testoComando = "update sias_import_organoclorurati set " +
                    "stato = :stato " +
                "where " +
                    "id_importazione = :id_importazione";

            cmd.CommandText = testoComando;

            cmd.Parameters.Add(new OracleParameter("stato", stato));
            cmd.Parameters.Add(new OracleParameter("id_importazione", idImportazione));

            cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// Verifica se eisiste la stazione associata ai codici indicati
        /// </summary>
        /// <param name="codiceIdentificativoStazione"></param>
        /// <param name="codiceIdentificativoSito"></param>
        /// <returns></returns>
        internal static bool EsisteStazione(string codiceIdentificativoStazione, string codiceIdentificativoSito)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Codice identificativo stazione: {codiceIdentificativoStazione} Codice identificativo sito: {codiceIdentificativoSito}");

            string testoComando = "select 's' from sias_siti " +
                "inner join siass_stazioni on sias_siti.id_sito = siass_stazioni.id_sito " +
                "where " +
                "upper(sias_siti.codice_identificativo) = :codice_identificativo_sito " +
                "and " +
                "upper(siass_stazioni.codice_identificativo) = :codice_identificativo_stazione";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("codice_identificativo_sito", codiceIdentificativoSito));
            cmd.Parameters.Add(new OracleParameter("codice_identificativo_stazione", codiceIdentificativoStazione));

            OracleDataReader dr = SQLHelper.CreaReader(cmd, Properties.Settings.Default.StringaConnessioneDB);

            return dr.Read();
        }

        internal static List<string> ElencoUnitaMisuraAccettate()
        {
			logger.Debug($"Call {MethodBase.GetCurrentMethod().Name}");

			string testoComando = $"select distinct conc_udm from sias_organo_anag_param where conc_udm is not NULL order by conc_udm";

			OracleCommand cmd = new OracleCommand
			{
				BindByName = true,
				CommandText = testoComando
			};

			OracleDataReader dr = SQLHelper.CreaReader(cmd, Properties.Settings.Default.StringaConnessioneDB);

			List<string> elenco = new List<string>();
			while (dr.Read())
			{
                elenco.Add(dr.GetString(dr.GetOrdinal("conc_udm")));
			}
			logger.Info($"Trovate n° {elenco.Count} unità di misura accettate.");
			return elenco;
		}
	}
}
