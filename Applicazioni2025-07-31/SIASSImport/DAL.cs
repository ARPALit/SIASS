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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SIASSImport
{
    public static class DAL
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static InfoSensore LetturaDatiSensore(string codiceIdentificativoSensore)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - CodiceIdentificativoSensore: {codiceIdentificativoSensore}");

            string testoComando = "select " +
                    "id_grandezza_stazione, " +
                    "coeff_conver_unita_misura " +
                "from sias_sensori " +
                "where codice_identificativo = :codice_identificativo";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("codice_identificativo", codiceIdentificativoSensore));

            cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
            cmd.Connection.Open();
            OracleDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (dr.Read())
            {
                decimal? coefficienteConversioneUnitaMisura = null;
                if (!dr.IsDBNull(dr.GetOrdinal("coeff_conver_unita_misura")))
                    coefficienteConversioneUnitaMisura = dr.GetDecimal(dr.GetOrdinal("coeff_conver_unita_misura"));

                InfoSensore i = new InfoSensore(codiceIdentificativoSensore,
                    dr.GetDecimal(dr.GetOrdinal("id_grandezza_stazione")),
                    coefficienteConversioneUnitaMisura);

                cmd.Connection.Close(); // Aggiunta per evitare problema di connessioni esaurite

                return i;
            }
            else
            {
                cmd.Connection.Close(); // Aggiunta per evitare problema di connessioni esaurite
                return null;
            }
        }

        public static MisurazioneSuDB LetturaMisurazioneDaDB(MisurazioneDaImportare misurazione)
        {
            string testoComando = "select " +
                "id_misurazione, " +
                "validata " +
                "from sias_misurazioni " +
                "where id_grandezza_stazione = :id_grandezza_stazione " +
                "and data_misurazione = :data_misurazione " +
                "and id_intervento is null " + // Sono ignorate le misuraioni derivanti da interventi
                "and rownum = 1";

            OracleCommand cmd = new OracleCommand
            {
                BindByName = true,
                CommandText = testoComando
            };

            cmd.Parameters.Add(new OracleParameter("id_grandezza_stazione", misurazione.IdGrandezzaStazione.Value));
            cmd.Parameters.Add(new OracleParameter("data_misurazione", misurazione.DataOraMisurazione));

            cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
            cmd.Connection.Open();
            OracleDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (dr.Read())
            {
                MisurazioneSuDB msdb = new MisurazioneSuDB()
                {
                    IdMisurazione = dr.GetDecimal(dr.GetOrdinal("id_misurazione")),
                    Validata = dr.GetDecimal(dr.GetOrdinal("validata"))
                };
                cmd.Connection.Close(); // Aggiunta per evitare problema di connessioni esaurite

                return msdb;
            }
            else
            {
                cmd.Connection.Close(); // Aggiunta per evitare problema di connessioni esaurite
                return null;
            }
        }

        public static void InserimentoMisurazione(MisurazioneDaImportare misurazione, RapportoElaborazioneFile rapportoFile)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Id grandezza stazione: {misurazione.IdGrandezzaStazione} Data ora misurazione: {misurazione.DataOraMisurazione} Valore: {misurazione.ValoreMisurazioneUnitaMisuraGrandezza}");

            try
            {
                OracleCommand cmd = new OracleCommand
                {
                    BindByName = true
                };

                string testoComando = "insert into sias_misurazioni " +
                    "(" +
                        "data_misurazione, " +
                        "valore, " +
                        "id_grandezza_stazione, " +
                        "ultimo_aggiornamento, " +
                        "autore_ultimo_aggiornamento, " +
                        "codice_identificativo_sensore, " +
                        "valore_sensore " +
                    ") " +
                    "values " +
                    "( " +
                        ":data_misurazione, " +
                        ":valore, " +
                        ":id_grandezza_stazione, " +
                        "sysdate, " +
                        "'Importazione', " +
                        ":codice_identificativo_sensore, " +
                        ":valore_sensore " +
                    ") " +
                    "returning id_misurazione into :id_misurazione";

                cmd.CommandText = testoComando;

                cmd.Parameters.Add(new OracleParameter("data_misurazione", misurazione.DataOraMisurazione));
                cmd.Parameters.Add(new OracleParameter("valore", misurazione.ValoreMisurazioneUnitaMisuraGrandezza));
                cmd.Parameters.Add(new OracleParameter("id_grandezza_stazione", misurazione.IdGrandezzaStazione));
                cmd.Parameters.Add(new OracleParameter("codice_identificativo_sensore", misurazione.CodiceIdentificativoSensore));
                cmd.Parameters.Add(new OracleParameter("valore_sensore", misurazione.ValoreMisurazioneUnitaMisuraSensore));

                // parametro di ritorno id_misurazione
                OracleParameter parametroIdMisurazione = new OracleParameter("id_misurazione", OracleDbType.Decimal, ParameterDirection.ReturnValue);
                cmd.Parameters.Add(parametroIdMisurazione);

                cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                logger.Trace($"Inserita misurazione con id_misurazione: {decimal.Parse(parametroIdMisurazione.Value.ToString())}");
            }
            catch (Exception ex)
            {
                logger.Error(Utils.GetExceptionDetails(ex));
                rapportoFile.SeErroreInserimentoDatiDB = true;
            }
        }

        public static void AggiornamentoMisurazione(MisurazioneDaImportare misurazione, MisurazioneSuDB misurazioneSuDB, RapportoElaborazioneFile rapportoFile)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Id misurazione: {misurazioneSuDB.IdMisurazione}");

            try
            {
                OracleCommand cmd = new OracleCommand
                {
                    BindByName = true
                };

                string testoComando = "update sias_misurazioni " +
                    "set " +
                        "data_misurazione = :data_misurazione, " +
                        "valore = :valore, " +
                        "ultimo_aggiornamento = sysdate, " +
                        "autore_ultimo_aggiornamento = 'Importazione', " +
                        "valore_sensore = :valore_sensore " +
                    "where id_misurazione = :id_misurazione";

                cmd.CommandText = testoComando;

                cmd.Parameters.Add(new OracleParameter("data_misurazione", misurazione.DataOraMisurazione));
                cmd.Parameters.Add(new OracleParameter("valore", misurazione.ValoreMisurazioneUnitaMisuraGrandezza));
                cmd.Parameters.Add(new OracleParameter("valore_sensore", misurazione.ValoreMisurazioneUnitaMisuraSensore));
                cmd.Parameters.Add(new OracleParameter("id_misurazione", misurazioneSuDB.IdMisurazione));

                cmd.Connection = new OracleConnection(Properties.Settings.Default.StringaConnessioneDB);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                logger.Error(Utils.GetExceptionDetails(ex));
                rapportoFile.SeErroreInserimentoDatiDB = true;
            }
        }
    }
}
