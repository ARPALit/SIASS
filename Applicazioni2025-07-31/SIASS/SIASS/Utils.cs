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

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace SIASS
{
    public class Utils
    {
        /// <summary>
        /// Url radice dell'applicazione
        /// </summary>
        /// <returns></returns>
        public static string ApplicationUrlRoot()
        {
            string urlRoot = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string appDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
            if (!appDomainAppVirtualPath.Equals("/")) urlRoot += appDomainAppVirtualPath;
            return urlRoot;
        }

        /// <summary>
        /// Estrazione di tutti i dettagli di un'eccezione con la gestione delle eccezioni SQL
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionDetails(Exception ex)
        {
            List<string> exceptionDetails = new List<string>();
            string stackTrace = ex.StackTrace;
            while (ex != null)
            {
                if (ex is SqlException exception)
                {
                    exceptionDetails.Add(string.Format("Source: {0} - Number: {1} - Message: {2}", ex.Source, exception.Number, ex.Message));
                }
                else
                    exceptionDetails.Add(string.Format("Source: {0} - Message: {1}", ex.Source, ex.Message));
                ex = ex.InnerException;
            }

            exceptionDetails.Add("Stack trace: " + stackTrace);

            StringBuilder sb = new StringBuilder();
            foreach (var exceptionDetail in exceptionDetails) sb.AppendLine(exceptionDetail);

            return (Environment.NewLine + sb.ToString());
        }

        /// <summary>
        /// Funzione di sostituzione di segnaposto in un testo
        /// </summary>
        /// <param name="sourceText">Testo in cui sostituire i segnaposto</param>
        /// <param name="placeHoldersList">Elenco di segnaposto e valore da sostituire. 
        /// I segnaposto devono avere un formato tale da non essere presenti nel testo.</param>
        /// <returns></returns>
        public static string ReplacePlaceHolder(string sourceText, List<Tuple<string, string>> placeHoldersList)
        {
            if (string.IsNullOrEmpty(sourceText)) return sourceText;

            string output = sourceText;
            foreach (Tuple<string, string> t in placeHoldersList) output = output.Replace(t.Item1, t.Item2);

            return output;
        }

        /// <summary>
        /// Tronca una stringa ai primi N caratteri dopo aver rimosso gli spazi in testa e coda.
        /// </summary>
        public static string TrimAndTruncate(string source, int length)
        {
            if (source == null) return null;
            string trimmed = source.Trim();
            if (trimmed.Length > length) trimmed = trimmed.Substring(0, length);
            return trimmed;
        }
    }
}