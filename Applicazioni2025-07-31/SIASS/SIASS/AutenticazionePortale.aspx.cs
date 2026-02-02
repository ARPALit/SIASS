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
using SIASS.BLL;
using SIASS.Model;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Security;
using ApiAnagraficheArpal;
using static ApiAnagraficheArpal.ConfigurazioneOperatore;
using System.Text.Json;

namespace SIASS
{
	public partial class AutenticazionePortale : System.Web.UI.Page
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		protected void Page_Load(object sender, EventArgs e)
		{
			LogHeaders();

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

			// Se l'header è vuoto, mostra errore
			if (valoriCodiceFiscale == null || !valoriCodiceFiscale.Any())
			{
				logger.Warn("Header non trovato o vuoto, login rifiutato");
				AvvisoLabel.Text = "Accesso non consentito: non è stato possibile identificare l'utente, in quanto mancano le informazioni di autenticazione fornite dal portale regionale.";
				return;
			}

			// Se ci sono più header con codice fiscale, mostra errore
			if (valoriCodiceFiscale.Count() > 1)
			{
				logger.Warn($"Header multipli");
				AvvisoLabel.Text = "Accesso non consentito: non è stato possibile identificare l'utente, in quanto negli header è presente più di un codice fiscale.";
				return;
			}

			logger.Info($"Login per Codice fiscale ricevuto: {valoriCodiceFiscale[0]}");

			// Il valore del codice fiscale ha un prefisso di sei caratteri che viene rimosso (tot lunghezza = 22)
			if (valoriCodiceFiscale[0].Length != 22)
			{
				logger.Error($"La lunghezza del codice fiscale ricevuto è diversa da 22 caratteri");
				AvvisoLabel.Text = "Accesso non consentito: il codice fiscale fornito dal portale regionale non è valido.";
				return;
			}

			// Rimuove i primi sei caratteri
			string codiceFiscale = valoriCodiceFiscale[0].Substring(6, valoriCodiceFiscale[0].Length - 6);

			var apiAnagrafiche = Global.ApiAnagrafiche;
			string identificativoApplicazione = ConfigurationManager.AppSettings.Get("IdentificativoApplicativo");

			// Ricava la configurazione operatore dal codice fiscale
			ConfigurazioneOperatore configurazioneOperatore = apiAnagrafiche.CaricaConfigurazioneOperatoreDaCodiceFiscale(identificativoApplicazione, codiceFiscale);
			logger.Debug($"Configurazione operatore:{JsonSerializer.Serialize(configurazioneOperatore)}");

			if (configurazioneOperatore == null)
			{
				logger.Warn($"Configurazione operatore non disponibile. CF:{codiceFiscale}");
				AvvisoLabel.Text = "Accesso non consentito: configurazione operatore non trovata.";
				return;
			}
			logger.Debug($"CF: {configurazioneOperatore.CF}");

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
			}
			FormsAuthentication.SetAuthCookie($"{configurazioneOperatoreSiass.Nome} {configurazioneOperatoreSiass.Cognome}", false);
			if (String.IsNullOrEmpty(paginaRedirect))
				FormsAuthentication.RedirectFromLoginPage($"{configurazioneOperatoreSiass.Nome} {configurazioneOperatoreSiass.Cognome}", false);
			else
				Response.Redirect(paginaRedirect, false);
		}

		private void LogHeaders()
		{
			int loop1, loop2;
			NameValueCollection coll;
			// Load Header collection into NameValueCollection object. 
			coll = Request.Headers;
			// Put the names of all keys into a string array. 
			String[] arr1 = coll.AllKeys;
			for (loop1 = 0; loop1 < arr1.Length; loop1++)
			{
				logger.Info("Key: " + arr1[loop1]);
				// Get all values under this key. 
				String[] arr2 = coll.GetValues(arr1[loop1]);
				for (loop2 = 0; loop2 < arr2.Length; loop2++)
				{
					logger.Info("   Value " + loop2 + ": " + Server.HtmlEncode(arr2[loop2]));
				}
			}
		}
	}
}