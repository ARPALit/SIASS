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

using ApiAnagraficheArpal;
using NLog;
using SIASS.BLL;
using SIASS.Model;
using System;
using System.Configuration;
using System.Linq;
using System.Text.Json;
using System.Web.Security;
using System.Web.UI;

namespace SIASS
{
    public partial class Login : System.Web.UI.Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Lettura dell'elenco dei nomi degli header che possono contenere il codice fiscale
                // dal parametro di configurazione che contiene la lista separata da virgole
                string stringaNomiHeaderCodiceFiscale = ConfigurationManager.AppSettings.Get("NomiHeaderCodiceFiscale");
                string[] elencoHeaderCodiceFiscale = stringaNomiHeaderCodiceFiscale.Split(',');

                // Prova a leggere tutti gli header contenuti in elencoHeaderCodiceFiscale
                // interrompendo appena ne trova uno
                string[] valoriCodiceFiscale = null;
                foreach (var nomeHeader in elencoHeaderCodiceFiscale)
                {
                    logger.Info($"Ricerca del codice fiscale nell'header {nomeHeader}");
                    valoriCodiceFiscale = Request.Headers.GetValues(nomeHeader);
                    if (valoriCodiceFiscale != null)
                        break;
                }

                // Se l'header contiene codice fiscale, rimanda alla pagina di login esterno
                if (valoriCodiceFiscale != null)
                {
                    logger.Warn($"Trovato codice fiscale nell'header, redirezione alla pagina di login esterno.");
                    Response.Redirect("~/AutenticazionePortale.aspx");
                    return;
                }

                logger.Info($"Header non trovato, utilizzo autenticazione interna.");

                // nasconde richiesta credenziali
                CredenzialiPanel.Visible = false;

                // impostazione pagina uscita
                IntranetHyperLink.NavigateUrl = ConfigurationManager.AppSettings.Get("URLUscita");

                // lettura dominio\user name dell'operatore connesso
                string loginOperatore = Request.LogonUserIdentity.Name;

                if (loginOperatore != string.Empty)
                {
                    // estrazione del solo user
                    int i = loginOperatore.IndexOf("\\") + 1;
                    loginOperatore = loginOperatore.Substring(i, loginOperatore.Length - i);

                    // logon
                    Logon(loginOperatore);
                }
                else
                {
                    MessaggioErroreLabel.Text = "Proprietà LogonUserIdentity.Name non disponibile.";
                }
            }

        }

        private void Logon(string loginOperatore)
        {
            // creazione operatore
			var apiAnagrafiche = Global.ApiAnagrafiche;
			string identificativoApplicazione = ConfigurationManager.AppSettings.Get("IdentificativoApplicativo");
			// Ricava la configurazione operatore dal codice operatore
			ConfigurazioneOperatore configurazioneOperatore = apiAnagrafiche.CaricaConfigurazioneOperatoreDaUsername(identificativoApplicazione, loginOperatore);
			logger.Debug($"Configurazione operatore:{JsonSerializer.Serialize(configurazioneOperatore)}");

            // Se l'operatore non è autorizzato
			if (!configurazioneOperatore.SeAutorizzato)
			{
				logger.Warn($"Configurazione operatore non disponibile. loginOperatore:{loginOperatore}");
                MessaggioErroreLabel.Text = "Accesso non consentito: configurazione operatore non trovata.";
				CredenzialiPanel.Visible = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("RichiestaCredenziali"));
				return;
			}
			logger.Debug($"loginOperatore: {loginOperatore}");

			// La configurazione operatore SIASS è creata a partire da quella generica
			ConfigurazioneOperatoreSiass configurazioneOperatoreSiass = new ConfigurazioneOperatoreSiass(configurazioneOperatore);
			logger.Debug($"Configurazione operatore SIASS:{JsonSerializer.Serialize(configurazioneOperatoreSiass)}");

			OperatoreSiassManager.SalvaOperatoreInSessione(configurazioneOperatoreSiass);

			string paginaRedirect;
			if (configurazioneOperatoreSiass.SePossibileSelezioneProfilo)
			{
				// L'operatore ha più di un profilo, manda alla pagina di selezione
				paginaRedirect = "~/SelezioneProfilo.aspx";
			}
			else
			{
				// L'operatore ha un solo profilo, va alla pagina dell'elenco stazioni
				paginaRedirect = "~/Stazione/ElencoStazioni.aspx";
				if (!String.IsNullOrEmpty(Session["Dispositivo"] as string) && (Session["Dispositivo"].ToString() == "Mobile"))
					paginaRedirect = "~/Mobile/Default.aspx";
			}
			FormsAuthentication.SetAuthCookie($"{configurazioneOperatoreSiass.Nome} {configurazioneOperatoreSiass.Cognome}", false);
			if (String.IsNullOrEmpty(paginaRedirect))
				FormsAuthentication.RedirectFromLoginPage($"{configurazioneOperatoreSiass.Nome} {configurazioneOperatoreSiass.Cognome}", false);
			else
				Response.Redirect(paginaRedirect, false);

		}
		protected void OKButton_Click(object sender, EventArgs e)
        {
            Logon(UsernameTextBox.Text.Trim());
        }

    }
}