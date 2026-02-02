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

using Oracle.ManagedDataAccess.Client;
using System.Data;


namespace SIASS
{
    /// <summary>
    /// Classe di supporto per le operazioni sulla base dati
    /// </summary>
    public static class SQLHelper
    {
        /// <summary>
        /// Popolamento e restituzione datatable sulla base del command
        /// passato come argomento
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="stringaConnessione"></param>
        /// <returns></returns>
        public static DataTable PopolaDataTable(OracleCommand cmd, string stringaConnessione)
        {
            cmd.Connection = new OracleConnection(stringaConnessione);
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            cmd.Connection.Open();
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmd.Connection.Close();
            return dt;
        }

        /// <summary>
        /// Popolamento e restituzione datatable sulla base della stringa SQL
        /// passato come argomento
        /// </summary>
        /// <param name="testoComando">stringa SQL da eseguire</param>
        /// <param name="stringaConnessione"></param>
        /// <returns></returns>
        public static DataTable PopolaDataTable(string testoComando, string stringaConnessione)
        {
            // creazione comando
            OracleCommand cmd = new OracleCommand(testoComando);
            // costruzione datatable risultati
            return PopolaDataTable(cmd, stringaConnessione);
        }

        /// <summary>
        /// Esecuzione di comando di modifica dati mediante ExecuteNonQuery
        /// </summary>
        /// <param name="cmd">Comando da eseguire</param>
        /// <param name="stringaConnessione"></param>
        /// <returns>Numero di righe interessata</returns>
        public static int ComandoAggiornamentoDati(OracleCommand cmd, string stringaConnessione)
        {
            int recordModificati;
            cmd.Connection = new OracleConnection(stringaConnessione);
            cmd.Connection.Open();
            recordModificati = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return recordModificati;
        }

        /// <summary>
        /// Esecuzione di comando di lettura valore mediante ExecuteScalar
        /// </summary>
        /// <param name="cmd">Comando da eseguire</param>
        /// <param name="stringaConnessione"></param>
        /// <returns>oggetto letto</returns>
        public static object ComandoLetturaValore(OracleCommand cmd, string stringaConnessione)
        {
            object oggettoLetto;
            cmd.Connection = new OracleConnection(stringaConnessione);
            cmd.Connection.Open();
            oggettoLetto = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return oggettoLetto;
        }

        /// <summary>
        /// Utility per l'aggiunta di una condizione AND a una stringa.
        /// Se la stringa non è vuota aggiunge l'AND con gli spazi opportuni.
        /// Racchiude la condizione tra parentesi
        /// </summary>
        /// <param name="condizioneOriginale">la condizione esistente cui aggiungere la nuova condizione</param>
        /// <param name="nuovaCondizione">la condizione da aggiungere</param>
        /// <returns>la condizione data dall'unione delle due</returns>
        public static string AggiuntaCondizioneAnd(string condizioneOriginale, string nuovaCondizione)
        {
            if (!string.IsNullOrEmpty(condizioneOriginale))
                return condizioneOriginale += " AND (" + nuovaCondizione + ")";
            else
                return "(" + nuovaCondizione + ")";
        }

        /// <summary>
        /// Creazione oracle datareader (con chiusura connessione alla chiusura del reader)
        /// </summary>
        /// <param name="cmd">Comando da eseguire</param>
        /// <param name="stringaConnessione"></param>
        /// <returns></returns>
        public static OracleDataReader CreaReader(OracleCommand cmd, string stringaConnessione)
        {
            cmd.Connection = new OracleConnection(stringaConnessione);            
            cmd.Connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

    }
}
