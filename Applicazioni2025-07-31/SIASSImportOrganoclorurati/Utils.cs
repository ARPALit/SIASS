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
using System.Text;

namespace SIASSImportOrganoclorurati
{
    public static class Utils
    {
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
                exceptionDetails.Add(string.Format("Source: {0} - Message: {1}", ex.Source, ex.Message));
                ex = ex.InnerException;
            }

            exceptionDetails.Add("Stack trace: " + stackTrace);

            StringBuilder sb = new StringBuilder();
            foreach (var exceptionDetail in exceptionDetails) sb.AppendLine(exceptionDetail);

            return (Environment.NewLine + sb.ToString());
        }
    }
}
