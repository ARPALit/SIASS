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
using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace SIASS
{
    public partial class PaginaErrore : System.Web.UI.Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            string descrizioneErrore;

            // Se non è abilitata la visualizzazione dettagliata dell'errore
            // visualizza un messaggio standard
            if (!ConfigurationManager.AppSettings["MostraErroreDettagliato"].Equals("S"))
                descrizioneErrore = ConfigurationManager.AppSettings["MessaggioErroreOperatore"];
            else
                // Lettura dell'ultima eccezione e costruzione del testo
                descrizioneErrore = DettagliEccezione(Page);

            // Visualizzazione del messaggio d'errore leggibile in HTML
            MessaggioErroreLabel.Text = Regex.Replace(descrizioneErrore, @"\r\n?|\n", "<br />");
        }

        /// <summary>
        /// Costruzione dei dettagli dell'eccezione verificatasi nella pagina e salvataggio su log
        /// </summary>
        /// <param name="pagina"></param>
        /// <returns></returns>
        private static string DettagliEccezione(Page pagina)
        {
            Exception ex = pagina.Server.GetLastError();
            StringBuilder sb = new StringBuilder();

            // La HttpUnhandledException non è visualizzata
            if (ex is HttpUnhandledException)
                CostruzioneDettagliEccezione(ex.InnerException, sb);
            else
                CostruzioneDettagliEccezione(ex, sb);

            string dettagliEccezione = sb.ToString();

            // Scrittura dell'eccezione nel log            
            logger.Error(dettagliEccezione);

            // Cancellazione dell'errore dal server
            pagina.Server.ClearError();
            return dettagliEccezione;
        }

        /// <summary>
        /// Costruzione del testo con i dettagli dell'eccezione
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        private static StringBuilder CostruzioneDettagliEccezione(Exception ex, StringBuilder sb)
        {
            if (ex == null)
            {
                sb.AppendLine("Message: no exception logged.");
            }
            else
            {
                sb.AppendLine("Message: " + ex.Message);
                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine("TargetSite: " + ex.TargetSite);
                sb.AppendLine("StackTrace: " + ex.StackTrace);

                // Legge ricorsivamente l'eccezione interna.
                if (ex.InnerException != null)
                {
                    sb.AppendLine("InnerException: ");
                    CostruzioneDettagliEccezione(ex.InnerException, sb);
                }
            }
            return sb;
        }
    }
}