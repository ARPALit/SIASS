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
using System.Configuration;
using ApiAnagraficheArpal;

namespace SIASS
{
    public class Global : System.Web.HttpApplication
    {

		public static ApiAnagraficheArpal.ApiAnagrafiche ApiAnagrafiche;
		protected void Application_Start(object sender, EventArgs e)
        {
			/*
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-3.6.0.min.js"
                });
            */
			string baseUrl = ConfigurationManager.AppSettings["ApiAnagraficheBaseUrl"];
			string apiKey = ConfigurationManager.AppSettings["ApiAnagraficheApiKey"];
			string configurazioneOperatore = ConfigurationManager.AppSettings["ApiAnagraficheConfigurazioneOperatore"];
			string analiti = ConfigurationManager.AppSettings["ApiAnagraficheAnaliti"];
			string pacchetti = ConfigurationManager.AppSettings["ApiAnagrafichePacchetti"];
			string contenitori = ConfigurationManager.AppSettings["ApiAnagraficheContenitori"];

			ConfigurazioneEndpoint configurazioneEndpoint = new ConfigurazioneEndpoint()
			{
				BaseUrl = baseUrl,
				ApiKey = apiKey,
				ConfigurazioneOperatore = configurazioneOperatore,
				Analiti = analiti,
				Pacchetti = pacchetti,
                Contenitori = contenitori
			};
			ApiAnagrafiche = new ApiAnagraficheArpal.ApiAnagrafiche(configurazioneEndpoint);
		}

		protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Server.Transfer("~/PaginaErrore.aspx", true);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}