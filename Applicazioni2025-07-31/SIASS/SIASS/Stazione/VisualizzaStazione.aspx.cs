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

using Microsoft.Reporting.WebForms;
using NLog;
using SIASS.BLL;
using SIASS.Model;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SIASS
{
	public partial class VisualizzaStazione : System.Web.UI.Page
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
			if (!(oper.SeAmministrazione || oper.SeGestione))
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
			{
				if (!decimal.TryParse(Request.QueryString["IdStazione"], out decimal idStazione))
				{
					logger.Warn("Parametro IdStazione mancante o errato.");
					Response.Write("Parametro IdStazione mancante o errato.");
					Response.End();
					return;
				}
				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
				if (infoStazione == null)
				{
					logger.Warn($"Stazione non trovata: IdStazione={idStazione}");
					Response.Write($"Stazione non trovata: IdStazione={idStazione}");
					Response.End();
					return;
				}
				logger.Debug($"Visualizza stazione: IdStazione={idStazione}");

				PopolaDatiStazione(infoStazione);
				GeneraScriptMappa(infoStazione);
				ViewState.Add("IdStazione", infoStazione.IdStazione);
			}
		}

		private void PopolaDatiStazione(InfoStazione infoStazione)
		{
			EsclusaMonitoraggioLabel.Text = infoStazione.EsclusaMonitoraggio ? "Sì" : "No";
			ControlloAnomalieLabel.Text = infoStazione.ControlloAnomalie ? "Sì" : "No";
			DescrizioneTipoStazioneLabel.Text = HttpUtility.HtmlEncode(infoStazione.DescrizioneTipoStazione);
			ReteAppartenenzaLabel.Text = HttpUtility.HtmlEncode(infoStazione.ReteAppartenenza);
			AllestimentoLabel.Text = HttpUtility.HtmlEncode(infoStazione.Allestimento);
			TeletrasmissioneLabel.Text = infoStazione.Teletrasmissione ? "Sì" : "No";
			PuntoConformitaLabel.Text = infoStazione.PuntoConformita ? "Sì" : "No";
			CodiceIdentificativoDescrizioneComuneProvinciaSitoLabel.Text = HttpUtility.HtmlEncode(infoStazione.CodiceIdentificativoDescrizioneComuneProvinciaSito);

			AnnotazioniLabel.Text = HttpUtility.HtmlEncode(infoStazione.Annotazioni);
			ModificaStazioneHyperLink.Text = $"{HttpUtility.HtmlEncode(infoStazione.CodiceIdentificativo)} - {HttpUtility.HtmlEncode(infoStazione.Descrizione)}  &rarr;";
			ModificaStazioneHyperLink.NavigateUrl = $"~/Stazione/ModificaStazione.aspx?IdStazione={infoStazione.IdStazione}";

			// Localizzazione
			if (infoStazione.Localizzazione != null)
			{
				DenominazioneComuneProvinciaLabel.Text = HttpUtility.HtmlEncode(infoStazione.Localizzazione.DenominazioneComune) + " (" + HttpUtility.HtmlEncode(infoStazione.Localizzazione.DenominazioneProvincia) + ")";
				LocalitaLabel.Text = HttpUtility.HtmlEncode(infoStazione.Localizzazione.Localita);
				DescrizioneBacinoLabel.Text = HttpUtility.HtmlEncode(infoStazione.Localizzazione.DescrizioneBacino);
				DescrizioneCorpoIdricoLabel.Text = HttpUtility.HtmlEncode(infoStazione.Localizzazione.DescrizioneCorpoIdrico);
				CTRLabel.Text = HttpUtility.HtmlEncode(infoStazione.Localizzazione.CTR);
				LatitudineLabel.Text = infoStazione.Localizzazione.Latitudine.ToString();
				LongitudineLabel.Text = infoStazione.Localizzazione.Longitudine.ToString();
				LatitudineGaussBoagaLabel.Text = infoStazione.Localizzazione.LatitudineGaussBoaga.ToString();
				LongitudineGaussBoagaLabel.Text = infoStazione.Localizzazione.LongitudineGaussBoaga.ToString();
				if (infoStazione.Localizzazione.QuotaPianoCampagna.HasValue)
					QuotaPianoCampagnaLabel.Text = infoStazione.Localizzazione.QuotaPianoCampagna.Value.ToString();
				CodiceSIRALLabel.Text = HttpUtility.HtmlEncode(infoStazione.Localizzazione.CodiceSIRAL);
			}
			ModificaLocalizzazioneHyperLink.NavigateUrl = $"~/Stazione/Localizzazione/ElencoLocalizzazioni.aspx?IdStazione={infoStazione.IdStazione}";

			// Caratteristiche tecniche pozzo
			if (infoStazione.CaratteristicheTecnichePozzo != null)
			{
				if (infoStazione.CaratteristicheTecnichePozzo.Profondita.HasValue)
					ProfonditaLabel.Text = infoStazione.CaratteristicheTecnichePozzo.Profondita.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.Diametro.HasValue)
					DiametroLabel.Text = infoStazione.CaratteristicheTecnichePozzo.Diametro.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaDa.HasValue)
					RangeSoggiacenzaDaLabel.Text = infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaDa.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaA.HasValue)
					RangeSoggiacenzaALabel.Text = infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaA.Value.ToString();
				DescrizioneTipoChiusuraLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheTecnichePozzo.DescrizioneTipoChiusura);
				if (infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoPc.HasValue)
					QuotaBoccapozzoPcLabel.Text = infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoPc.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoSlm.HasValue)
					QuotaBoccapozzoSlmLabel.Text = infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoSlm.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm.HasValue)
					QuotaPianoRiferimentoSlmLabel.Text = infoStazione.CaratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.DifferenzaPrpc.HasValue)
					DifferenzaPrpcLabel.Text = infoStazione.CaratteristicheTecnichePozzo.DifferenzaPrpc.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.ProfonditaEmungimento.HasValue)
					ProfonditaEmungimentoLabel.Text = infoStazione.CaratteristicheTecnichePozzo.ProfonditaEmungimento.Value.ToString();
				if (infoStazione.CaratteristicheTecnichePozzo.PortataMassimaEsercizio.HasValue)
					PortataMassimaEsercizioLabel.Text = infoStazione.CaratteristicheTecnichePozzo.PortataMassimaEsercizio.Value.ToString();
				PresenzaForoSondaLabel.Text = infoStazione.CaratteristicheTecnichePozzo.PresenzaForoSonda ? "Sì" : "No";
				DescrizioneTipoDestinazioneusoLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheTecnichePozzo.DescrizioneTipoDestinazioneuso);
				DescrizioneTipoFrequenzaUtilizzoLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheTecnichePozzo.DescrizioneTipoFrequenzaUtilizzo);
				CaptataLabel.Text = infoStazione.CaratteristicheTecnichePozzo.Captata ? "Sì" : "No";
			}
			ModificaCaratteristicheTecnichePozzoHyperLink.NavigateUrl = $"~/Stazione/CaratteristicheTecniche/ElencoCaratteristicheTecniche.aspx?IdStazione={infoStazione.IdStazione}";



			// Caratteristiche installazione
			if (infoStazione.CaratteristicheInstallazione != null)
			{
				DescrizioneTipoFissaggioTrasmettitoreLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheInstallazione.DescrizioneTipoFissaggioTrasmettitore);
				CavoEsternoInGuainaLabel.Text = infoStazione.CaratteristicheInstallazione.CavoEsternoInGuaina ? "Sì" : "No";
				CavoSottotracciaLabel.Text = infoStazione.CaratteristicheInstallazione.CavoSottotraccia ? "Sì" : "No";
				ProtezioneAreaLabel.Text = infoStazione.CaratteristicheInstallazione.ProtezioneArea ? "Sì" : "No";
				DescrizioneTipoAccessoLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheInstallazione.DescrizioneTipoAccesso);
				OsservazioniLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheInstallazione.Osservazioni);
				SicurezzaLabel.Text = HttpUtility.HtmlEncode(infoStazione.CaratteristicheInstallazione.Sicurezza);
				ProfonditaSensoreLabel.Text = infoStazione.CaratteristicheInstallazione.ProfonditaSensore.HasValue ? infoStazione.CaratteristicheInstallazione.ProfonditaSensore.ToString() : null;
			}
			ModificaCaratteristicheInstallazioneHyperLink.NavigateUrl = $"~/Stazione/CaratteristicheInstallazione/ElencoCaratteristicheInstallazione.aspx?IdStazione={infoStazione.IdStazione}";

			// Dati amministrativi
			if (infoStazione.DatiAmministrativi != null)
			{
				GestoreLabel.Text = HttpUtility.HtmlEncode(infoStazione.DatiAmministrativi.Gestore);
				IndirizzoGestoreLabel.Text = HttpUtility.HtmlEncode(infoStazione.DatiAmministrativi.IndirizzoGestore);
				TelefonoGestoreLabel.Text = HttpUtility.HtmlEncode(infoStazione.DatiAmministrativi.TelefonoGestore);
				PartitaIVAGestoreLabel.Text = HttpUtility.HtmlEncode(infoStazione.DatiAmministrativi.PartitaIVAGestore);
				RiferimentoGestoreLabel.Text = HttpUtility.HtmlEncode(infoStazione.DatiAmministrativi.RiferimentoGestore);
			}
			ModificaDatiAmministrativiHyperLink.NavigateUrl = $"~/Stazione/DatiAmministrativi/ElencoDatiAmministrativi.aspx?IdStazione={infoStazione.IdStazione}";

			// Ultimo intervento
			if (infoStazione.UltimoIntervento != null)
			{
				DataInterventoLabel.Text = infoStazione.UltimoIntervento.DataIntervento.ToString(CostantiGenerali.FORMATO_DATA);
				TipoInterventoLabel.Text = infoStazione.UltimoIntervento.DescrizioneTipoIntervento;
				OperatoriLabel.Text = infoStazione.UltimoIntervento.DescrizioneOperatoriIntervento;
			}

			// Strumenti stazione
			var strumentiStazione = StrumentoManager.ElencoStrumenti(infoStazione.IdStazione)
				.Where(i => i.FineValidita == null)
				.OrderBy(i => i.Marca)
				.ThenBy(i => i.Modello);

			StrumentiStazioneRepeater.DataSource = strumentiStazione;
			StrumentiStazioneRepeater.DataBind();
			ModificaStrumentiHyperLink.NavigateUrl = $"~/Stazione/Strumenti/ElencoStrumenti.aspx?IdStazione={infoStazione.IdStazione}";

			GrandezzeStazioneRepeater.DataSource = SensoreManager.ElencoGrandezzeStazione(infoStazione.IdStazione);
			GrandezzeStazioneRepeater.DataBind();
			GrandezzeMisurazioniHyperLink.NavigateUrl = $"~/Stazione/Grandezze/ElencoGrandezze.aspx?IdStazione={infoStazione.IdStazione}";

			InterventiHyperLink.NavigateUrl = $"~/Stazione/Interventi/ElencoInterventi.aspx?IdStazione={infoStazione.IdStazione}";

			AllegatiStazioneHyperLink.NavigateUrl = $"~/Stazione/AllegatiStazione/ElencoAllegatiStazione.aspx?IdStazione={infoStazione.IdStazione}";

			MisurazioniOrganocloruratiHyperLink.NavigateUrl = $"~/Stazione/ElencoMisurazioniOrganoclorurati.aspx?IdStazione={infoStazione.IdStazione}";
		}

		protected void PDFStazioneLinkButton_Click(object sender, EventArgs e)
		{
			decimal idStazione = decimal.Parse(ViewState["IdStazione"].ToString());

			ReportViewer rv = new ReportViewer();

			StazioneManager.GeneraReportStazione(rv, idStazione);
			byte[] bytes = rv.LocalReport.Render("PDF", null,
				out string mimeType, out _, out string extension, out _, out _);
			Response.Buffer = true;
			Response.Clear();
			Response.ContentType = mimeType;
			Response.AddHeader("content-disposition", "attachment; filename=SchedaStazione." + extension);
			Response.BinaryWrite(bytes);
			Response.Flush();
			Response.End();

		}

		protected void PDFInterventiLinkButton_Click(object sender, EventArgs e)
		{
			decimal idStazione = decimal.Parse(ViewState["IdStazione"].ToString());
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

			byte[] bytes = InterventoManager.GeneraTuttiPDFInterventiStazione(idStazione, oper.OrganizzazioneAttiva().Codice);
			Response.Buffer = true;
			Response.Clear();
			Response.ContentType = "application/pdf";
			Response.AddHeader("content-disposition", "attachment; filename=Interventi.pdf");
			Response.BinaryWrite(bytes);
			Response.Flush();
			Response.End();
		}

		private void GeneraScriptMappa(InfoStazione infoStazione)
		{
			// Aggiunge marker singola stazione
			if (infoStazione.Localizzazione != null)
			{
				NoLocalizzazionePanel.Visible = false;

				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

				string cartellaImmaginiTipiStazione = Utils.ApplicationUrlRoot() + "/img/TipiStazione/";

				string script = "<script type=\"text/javascript\">";

				// Inizializzazione mappa
				script += "function initMap() {" +
				"var myLatLng = { lat: " + infoStazione.Localizzazione.Latitudine.ToString().Replace(",", ".") + ", lng: " + infoStazione.Localizzazione.Longitudine.ToString().Replace(",", ".") + " }; " +
				"var map = new google.maps.Map(document.getElementById('map'), {" +
					"center: myLatLng," +
					"scrollwheel: true," +
					"zoom: 17, " +
					"mapTypeId: google.maps.MapTypeId.ROADMAP " +
				"}); " +
				"var rndLatLng; ";

				// Genera tooltip con codice e descrizione stazione
				string tooltipStazione = infoStazione.CodiceIdentificativo + " - " + infoStazione.Descrizione;
				// Escape di eventuali apici
				tooltipStazione = tooltipStazione.Replace("'", @"\'");

				//  costruisce il maker della stazione
				script += "stazioneLatLng = { lat: " + infoStazione.Localizzazione.Latitudine.ToString().Replace(",", ".") + ", lng: " + infoStazione.Localizzazione.Longitudine.ToString().Replace(",", ".") + " }; ";
				// Immagine
				script += "var image = { " +
						"url: '" + cartellaImmaginiTipiStazione + infoStazione.IdTipoStazione + ".png' " +
					"}; ";
				// Marker
				script += "var marker" + infoStazione.IdStazione.ToString() + " = new google.maps.Marker({ " +
					"position: stazioneLatLng, " +
					"map: map, " +
					"title: '" + tooltipStazione + "', " +
					"url: '/Stazione/VisualizzaStazione.aspx?IdStazione=" + infoStazione.IdStazione.ToString() + "', " +
					"icon: image" +
				"}); ";

				// Aggiunge listener per evento di clic solo per utente gestore
				if (oper.SeAmministrazione || oper.SeGestione)
				{
					script += "google.maps.event.addListener(marker" + infoStazione.IdStazione.ToString() + ", 'click', function () { " +
						"window.location.href = marker" + infoStazione.IdStazione.ToString() + ".url; " +
					"}); ";
				}

				script += "}";
				script += "</script>";

				if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleAPIKey"]))
					script += @"<script async defer src='https://maps.googleapis.com/maps/api/js?callback=initMap'></script>";
				else
					script += @"<script async defer src='https://maps.googleapis.com/maps/api/js?key=" + ConfigurationManager.AppSettings["GoogleAPIKey"] + "&callback=initMap'></script>";

				ClientScript.RegisterStartupScript(
					Type.GetType("System.String"),
					"Mappa",
					script
					);
			}
		}
	}
}