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

using DocumentFormat.OpenXml.Office2019.Excel.ThreadedComments;
using NLog;
using SIASS.BLL;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SIASS.BLL.OrganocloruratiManager;

namespace SIASS
{
	public partial class NuovaImportazioneOrganoclorurati : System.Web.UI.Page
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		protected void Page_Load(object sender, EventArgs e)
		{
			// recupero e verifica presenza informazioni operatore
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

			if (oper == null || String.IsNullOrEmpty(oper.ProfiloAttivo()))
			{
				OperatoreManager.Logout();
				Response.End();
				return;
			}

			// verifica autorizzazioni
			if (!(oper.SeAmministrazione || oper.SeGestione || oper.SeGestoreDitta))
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
			{
				logger.Debug($"NuovaImportazioneOrganoclorurati");
				CaricaNuovoFileMultiView.SetActiveView(CaricaNuovoFileView);
				CaricaNuovoFileHyperLink.NavigateUrl = "NuovaImportazioneOrganoclorurati.aspx";
				ElencoImportazioniHyperLink.NavigateUrl = "ImportazioniOrganoclorurati.aspx";
				FileUpload.Attributes["accept"] = ".xlsx";
			}
		}

		protected void CaricaButton_Click(object sender, EventArgs e)
		{
			logger.Debug($"Caricamento file");
			if (!FileUpload.HasFile)
			{
				logger.Debug($"Nessun file specificato");
				Response.Redirect("NuovaImportazioneOrganoclorurati.aspx");
				Response.End();
				return;
			}
			string ora = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			string nomeFileDaElaborare = $"{ora}_{FileUpload.FileName}";

			string estensioneFileAllegato = Path.GetExtension(FileUpload.FileName).ToLower();
			if (estensioneFileAllegato != ".xlsx")
			{
				// Estensione non accettata, avvisa l'utente
				EsitoLabel.Text = $"Errore nell'importazione: tipo di file non accettato.";
				CaricaNuovoFileMultiView.SetActiveView(EsitoView);
				return;
			}

			// Verifica dimensione
			int i = FileUpload.PostedFile.ContentLength;
			// Verifica massimo limite consentito
			if (i > int.Parse(ConfigurationManager.AppSettings["DimensioneMassimaFileDaCaricare"]))
			{
				// Se il file è troppo grande, avvisa l'utente
				EsitoLabel.Text = $"Errore nell'importazione: File troppo grande - dimensione massima: {(int.Parse(ConfigurationManager.AppSettings["DimensioneMassimaFileDaCaricare"]) / 1048576)} MByte.";
				CaricaNuovoFileMultiView.SetActiveView(EsitoView);
				return;
			}

			if (!Regex.IsMatch(FileUpload.FileName, @"^[0-9a-zA-Z.]{1,100}$", RegexOptions.IgnoreCase))
			{
				// Verifica nome file
				EsitoLabel.Text = $"Errore nell'importazione: Il nome del file deve essere composto solo da lettere, cifre e punti";
				CaricaNuovoFileMultiView.SetActiveView(EsitoView);
				return;
			}

			logger.Debug($"Upload file {nomeFileDaElaborare}");

			if (CaricaFile(FileUpload, nomeFileDaElaborare))
			{
				logger.Debug($"File caricato: {nomeFileDaElaborare}");

				// Se l'operatore è un gestore ditta, si usa la partita IVA per verificare che possa caricare i dati per le stazioni del file
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				string partitaIvaOperatore = null;
				if (oper.SeGestoreDitta)
				{
					partitaIvaOperatore = oper.OrganizzazioneAttiva().PIVA;
				}
				if (VerificaImportazione(nomeFileDaElaborare, partitaIvaOperatore, out string errore))
				{
					EsitoLabel.Text = $"File caricato: {nomeFileDaElaborare}. L'importazione verrà elaborata. Il risultato sarà disponibile a breve nell'elenco importazioni.";
					logger.Debug($"File caricato: {nomeFileDaElaborare}");
					OrganocloruratiManager.InserisciImportazione(
						$"{oper.Nome} {oper.Cognome}",
						nomeFileDaElaborare,
						partitaIvaOperatore
						);
				}
				else
				{
					logger.Debug($"Errore nell'importazione: {errore}");
					EsitoLabel.Text = $"Errore nell'importazione: {errore}";
				}
			}
			CaricaNuovoFileMultiView.SetActiveView(EsitoView);
		}

		private bool CaricaFile(FileUpload fileUpload, string nomeFile)
		{
			// Verifica dimensione
			int dimensioneFile = fileUpload.PostedFile.ContentLength;
			// Verifica massimo limite consentito
			if (dimensioneFile > int.Parse(ConfigurationManager.AppSettings["DimensioneMassimaFileDaCaricare"]))
			{
				// Se il file è troppo grande, avvisa l'utente
				EsitoLabel.Text = $"File troppo grande {fileUpload.FileName} - dimensione massima {int.Parse(ConfigurationManager.AppSettings["DimensioneMassimaFileDaCaricare"]) / 1048576} MByte";
				return false;
			}
			// Salva il file creando eventualmente la cartella
			string cartella = Server.MapPath("~/File/ImportazioniOrganoclorurati");
			if (!Directory.Exists(cartella))
				Directory.CreateDirectory(cartella);
			fileUpload.SaveAs($"{cartella}/{nomeFile}");
			logger.Debug($"Caricato file {fileUpload.FileName} come {nomeFile} nella cartella {cartella}");

			return true;
		}

		private bool VerificaImportazione(string nomeFile, string partitaIvaOperatore, out string errore)
		{
			errore = null;
			// Vengono validati subito sito e piezometro per verificare che l'operatore abbia accesso alla relativa stazione
			// Il resto della validazione viene effettuato dal sw ImportOrganoclorurati

			// Legge il file ricavando tutti i codice stazione e sito

			var esitoParsing = OrganocloruratiManager.ParsingFile(Server.MapPath($"~/File/ImportazioniOrganoclorurati/{nomeFile}"));
			if (!esitoParsing.Riuscito)
			{
				if (esitoParsing.ErroreFile != null)
				{
					errore = esitoParsing.ErroreFile;
				}
				else
				{
					errore = string.Join(", ", esitoParsing.ErroriParsing.Select(i => i));
				}
				return false;
			}

			// Verifica che l'elenco non sia vuoto
			if (esitoParsing.StazioniSiti.Count == 0)
			{
				errore = "Nessuna stazione trovata nel file.";
				return false;
			}

			// Fa una distinct dell'elenco
			var elencoStazioni = esitoParsing.StazioniSiti
				 .GroupBy(s => new { s.Stazione, s.Sito })
				 .Select(i => i.First()).ToList();

			// Per ogni stazione+sito verifica che esistano su db
			foreach (var stazione in elencoStazioni)
			{
				// Se non esistono si ferma e segnala errore
				if (!OrganocloruratiManager.EsisteStazione(stazione.Stazione, stazione.Sito))
				{
					errore = $"Stazione non trovata. Stazione:{stazione.Stazione} - Sito:{stazione.Sito}";
					return false;
				}
			}

			// Se non è specificata la apritta IVA operatore, non la verifica
			if (String.IsNullOrEmpty(partitaIvaOperatore))
			{
				return true;
			}

			// Finito il giro lo rifa ricavando la partita iva per ogni stazione
			foreach (var stazione in elencoStazioni)
			{
				string partitaIvaStazione = OrganocloruratiManager.PartitaIVAStazione(stazione.Stazione, stazione.Sito);
				// Se non la trova si ferma e segnala errore
				if (String.IsNullOrEmpty(partitaIvaStazione))
				{
					errore = $"Partita IVA non trovata in archivio. Stazione:{stazione.Stazione} - Sito:{stazione.Sito}";
					return false;
				}
				else
				{
					stazione.PartitaIVAStazione = partitaIvaStazione;
				}
			}

			// Confronta ogni partita iva con quella dell'operatore
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			foreach (var stazione in elencoStazioni)
			{
				if (!oper.SeCreazioneInterventoStazione(stazione.PartitaIVAStazione, null))
				{
					// Se non corrispondono si ferma e segnala errore
					errore = $"L'operatore non ha accesso alla stazione. Stazione:{stazione.Stazione} - Sito:{stazione.Sito}";
					return false;
				}
			}

			return true;
		}
	}
}