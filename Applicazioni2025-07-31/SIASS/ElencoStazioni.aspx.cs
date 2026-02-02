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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
	public partial class ElencoStazioni : System.Web.UI.Page
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		protected void Page_Load(object sender, EventArgs e)
		{
			Session.Add("Dispositivo", "Desktop");

			// recupero e verifica presenza informazioni operatore
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

			if (oper == null || String.IsNullOrEmpty(oper.ProfiloAttivo()))
			{
				OperatoreManager.Logout();
				Response.End();
				return;
			}

			// verifica autorizzazioni

			if (!oper.SeAutorizzato)
			{
				OperatoreManager.Logout();
				return;
			}

			// Se l'operatore ha solo consultazione dataset, viene rediretto sulla relativa pagina
			if (oper.SeConsultazioneDataset)
			{
				Response.Redirect("~/ConsultazioneDataset/Default.aspx");
				return;
			}

			if (!Page.IsPostBack)
			{
				if (oper.SeGestoreDitta)
				{
					logger.Debug($"Operatore gestore ditta, mostra solo casella ricerca");
					UpdatePanel2.Visible = false;
					MappaPanel.Visible = false;
				}
				else
				{
					logger.Debug($"Operatore gestore ditta, mostra filtri ricerca");
					GestorePanel.Visible = false;
					using (SIASSEntities context = new SIASSEntities())
					{
						logger.Debug($"Caricamento reti appartenenza");
						var elencoRetiAppartenenza = context.TipiReteAppartenenza.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
						TipiReteAppartenenzaDropDownList.DataSource = elencoRetiAppartenenza.ToList();
						TipiReteAppartenenzaDropDownList.DataBind();
						TipiReteAppartenenzaDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));

						logger.Debug($"Caricamento elenco bacini");
						var elencoBacini = context.Bacini.OrderBy(i => i.DescrizioneBacino);
						BaciniDropDownList.DataSource = elencoBacini.ToList();
						BaciniDropDownList.DataBind();
						BaciniDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

						logger.Debug($"Caricamento tipi allestimento");
						var elencoTipiAllestimento = context.TipiAllestimento.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
						TipiAllestimentoDropDownList.DataSource = elencoTipiAllestimento.ToList();
						TipiAllestimentoDropDownList.DataBind();
						TipiAllestimentoDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

						logger.Debug($"Caricamento tipi grandezza");
						var elencoGrandezze = context.TipiGrandezza.OrderBy(i => i.ORDINE).ThenBy(i => i.NOME_GRANDEZZA);
						TipiGrandezzaDropDownList.DataSource = elencoGrandezze.ToList();
						TipiGrandezzaDropDownList.DataBind();
						TipiGrandezzaDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));

						logger.Debug($"Caricamento corpi idrici");
						var elencoCorpiIdrici = context.CorpiIdrici.OrderBy(i => i.DescrizioneCorpoIdrico);
						CorpiIdriciDropDownList.DataSource = elencoCorpiIdrici.ToList();
						CorpiIdriciDropDownList.DataBind();
						CorpiIdriciDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

						logger.Debug($"Caricamento tipi stazione");
						var elencoTipiStazione = context.TipiStazione.OrderBy(i => i.DescrizioneTipoStazione);
						TipiStazioneDropDownList.DataSource = elencoTipiStazione.ToList();
						TipiStazioneDropDownList.DataBind();
						TipiStazioneDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

						logger.Debug($"Caricamento elenco siti");
						var elencoSiti = context.Siti.OrderBy(i => i.DESCRIZIONE);
						SitiDropDownList.DataSource = elencoSiti.ToList();
						SitiDropDownList.DataBind();
						SitiDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

						logger.Debug($"Caricamento elenco gestori");
						var elencoGestori = context.DatiAmministrativi.Where(i => i.Gestore != null).Select(i => i.Gestore).Distinct().OrderBy(i => i);
						GestoriDropDownList.DataSource = elencoGestori.ToList();
						GestoriDropDownList.DataBind();
						GestoriDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

						if (oper.SeGestoreRete)
						{
							TipiReteAppartenenzaDropDownList.SelectedValue = oper.Rete;
							TipiReteAppartenenzaDropDownList.Enabled = false;
						}
					}
				}
				EsclusaMonitoraggioCheckBox.Checked = true;

				AggiornaElencoProvince();
				AggiornaElencoComuni();

				// impostazione pagina versione
				VersioneLabel.Text = "Versione: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

				// impostazione link privacy
				string urlPrivacy = ConfigurationManager.AppSettings["URLPrivacy"];
				if (String.IsNullOrEmpty(urlPrivacy))
				{
					PrivacyHyperLink.Visible = false;
				}
				else
				{
					PrivacyHyperLink.NavigateUrl = urlPrivacy;
				}

				NuovaStazioneButton.Visible = oper.SeAmministrazione;

			}
		}

		protected void ProvinceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			AggiornaElencoComuni();
		}

		private void AggiornaElencoProvince()
		{
			logger.Debug($"Caricamento elenco province");
			using (SIASSEntities context = new SIASSEntities())
			{
				var elencoProvince = context.Province.OrderBy(i => i.DenominazioneProvincia);
				ProvinceDropDownList.DataSource = elencoProvince.ToList();
				ProvinceDropDownList.DataBind();
				ProvinceDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));
			}
		}

		private void AggiornaElencoComuni()
		{
			logger.Debug($"Caricamento elenco comuni");
			using (SIASSEntities context = new SIASSEntities())
			{
				IEnumerable<Comune> elencoComuni = context.Comuni;
				if (ProvinceDropDownList.SelectedIndex > 0)
					elencoComuni = elencoComuni.Where(i => i.CodiceProvincia.Equals(ProvinceDropDownList.SelectedValue));

				ComuniDropDownList.DataSource = elencoComuni.OrderBy(i => i.DenominazioneComune).ToList();
				ComuniDropDownList.DataBind();
				ComuniDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));
			}
		}

		protected void CercaStazione(int numeroPagina)
		{
			logger.Debug($"Ricerca stazione");
			var parametriRicerca = PopolaParametriRicerca(numeroPagina, StazioniGridView.PageSize);
			List<InfoStazionePerElenco> stazioni = StazioneManager.ElencoStazioni(parametriRicerca, out int recordTrovati);
			StazioniGridView.VirtualItemCount = recordTrovati;
			StazioniGridView.DataSource = stazioni;
			StazioniGridView.DataBind();
		}

		protected void StazioniGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			StazioniGridView.PageIndex = e.NewPageIndex;
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			if (!oper.SeGestoreDitta)
			{
				CercaStazione(e.NewPageIndex + 1);
			}
			else
			{
				GestoreCercaStazione(e.NewPageIndex + 1);
			}
		}

		protected void CercaStazioneButton_Click(object sender, EventArgs e)
		{
			StazioniGridView.PageIndex = 0;
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			if (!oper.SeGestoreDitta)
			{
				CercaStazione(1);
				GeneraScriptMappa();
			}
		}

		private StazioneManager.RicercaStazione PopolaParametriRicerca(int numeroPagina, int dimensionePagina)
		{
			logger.Debug($"PopolaParametriRicerca");
			decimal? idBacino = null;
			if (BaciniDropDownList.SelectedValue != string.Empty)
				idBacino = decimal.Parse(BaciniDropDownList.SelectedValue);

			decimal? idCorpoIdrico = null;
			if (CorpiIdriciDropDownList.SelectedValue != string.Empty)
				idCorpoIdrico = decimal.Parse(CorpiIdriciDropDownList.SelectedValue);

			decimal? idTipoStazione = null;
			if (TipiStazioneDropDownList.SelectedValue != string.Empty)
				idTipoStazione = decimal.Parse(TipiStazioneDropDownList.SelectedValue);

			decimal? idSito = null;
			if (SitiDropDownList.SelectedValue != string.Empty)
				idSito = decimal.Parse(SitiDropDownList.SelectedValue);

			return new StazioneManager.RicercaStazione
			{
				NumeroPagina = numeroPagina,
				DimensionePagina = dimensionePagina,
				CodiceIdentificativoDescrizione = CodiceIdentificativoDescrizioneTextBox.Text,
				CodiceComune = ComuniDropDownList.SelectedValue,
				CodiceProvincia = ProvinceDropDownList.SelectedValue,
				ReteAppartenenza = TipiReteAppartenenzaDropDownList.SelectedValue,
				GrandezzaRilevata = TipiGrandezzaDropDownList.SelectedValue,
				IdBacino = idBacino,
				Allestimento = TipiAllestimentoDropDownList.SelectedValue,
				IdCorpoIdrico = idCorpoIdrico,
				IdTipoStazione = idTipoStazione,
				Gestore = GestoriDropDownList.SelectedValue,
				EsclusaMonitoraggio = EsclusaMonitoraggioCheckBox.Checked,
				IdSito = idSito
			};
		}

		private void GeneraScriptMappa()
		{
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

			var parametriRicerca = PopolaParametriRicerca(1, int.MaxValue);
			var elencoStazioni = StazioneManager.ElencoStazioni(parametriRicerca, out _);
			var script = StazioneManager.GeneraScriptMappa(elencoStazioni, oper.SeGestione || oper.SeAmministrazione);
			ClientScript.RegisterStartupScript(
				Type.GetType("System.String"),
				"Mappa",
				script
				);
		}

		protected void StazioniGridView_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				// Spunta Esclusa monitoraggio
				Label esclusaMonitoraggioLabel = (Label)e.Row.FindControl("EsclusaMonitoraggioLabel");
				if (DataBinder.Eval(e.Row.DataItem, "EsclusaMonitoraggio") != null)
					esclusaMonitoraggioLabel.Visible = ((bool)DataBinder.Eval(e.Row.DataItem, "EsclusaMonitoraggio"));

				// Registra come postbackcontrol il pulsante per i pdf per consentire il download nell'updatepanel
				ImageButton pdfStazioneImageButton = (ImageButton)e.Row.FindControl("PDFStazioneImageButton");
				ScriptManager.GetCurrent(this).RegisterPostBackControl(pdfStazioneImageButton);

				// Immagini tipo stazione
				string cartellaImmaginiTipiStazione = "../img/TipiStazione";
				if (DataBinder.Eval(e.Row.DataItem, "IdTipo") != null)
				{
					Image immagineTipoStazioneImage = (Image)e.Row.FindControl("ImmagineTipoStazioneImage");
					immagineTipoStazioneImage.ImageUrl = $"{cartellaImmaginiTipiStazione}/{DataBinder.Eval(e.Row.DataItem, "IdTipo")}mini.png";
					immagineTipoStazioneImage.ToolTip = $"{DataBinder.Eval(e.Row.DataItem, "Tipo")}";
				}

				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

				Label codiceIdentificativoLabel = (Label)e.Row.FindControl("CodiceIdentificativoLabel");
				HyperLink visualizzaStazioneHyperLink = (HyperLink)e.Row.FindControl("VisualizzaStazioneHyperLink");

				if (oper.SeGestione || oper.SeAmministrazione)
				{
					visualizzaStazioneHyperLink.Visible = true;
					codiceIdentificativoLabel.Visible = false;
				}
				else
				{
					visualizzaStazioneHyperLink.Visible = false;
					codiceIdentificativoLabel.Visible = true;
				}

				// Nasconde i link per misurazioni e importazione misurazioni
				if (!(oper.SeGestione || oper.SeAmministrazione))
				{
					StazioniGridView.Columns[StazioniGridView.Columns.Count - 1].Visible = false;
				}

				// Nasconde i link per gli interventi
				if (!(oper.SeGestione || oper.SeAmministrazione || oper.SeGestoreDitta))
				{
					StazioniGridView.Columns[StazioniGridView.Columns.Count - 2].Visible = false;
				}
			}
		}

		protected void PDFStazioneImageButton_Click(object sender, ImageClickEventArgs e)
		{
			ImageButton btn = (ImageButton)sender;
			GridViewRow row = (GridViewRow)btn.NamingContainer;
			decimal idStazione = decimal.Parse(StazioniGridView.DataKeys[row.RowIndex].Value.ToString());
			logger.Debug($"Genera PDF stazione {idStazione}");

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

		protected void RimuoviFiltriButton_Click(object sender, EventArgs e)
		{
			Response.Redirect("ElencoStazioni.aspx");
		}

		public string ImpostaVisibilitaMappa()
		{
			if (Page.IsPostBack)
				return "width: 1000px; height: 500px; border: 1px solid #ccc; margin-top: 1em;";
			else
				return "display: none;";
		}

		protected void NuovaStazioneButton_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Stazione/NuovaStazione.aspx");
		}

		protected void GestoreCercaStazioneButton_Click(object sender, EventArgs e)
		{
			StazioniGridView.PageIndex = 0;
			GestoreCercaStazione(1);
		}
		protected void GestoreCercaStazione(int numeroPagina)
		{
			logger.Debug($"Ricerca stazione");
			var parametriRicerca = GestorePopolaParametriRicerca(numeroPagina, StazioniGridView.PageSize);
			List<InfoStazionePerElencoGestore> stazioni = StazioneManager.ElencoStazioniGestore(parametriRicerca, out int recordTrovati);
			StazioniGridView.VirtualItemCount = recordTrovati;
			StazioniGridView.DataSource = stazioni;
			StazioniGridView.DataBind();
		}
		private StazioneManager.RicercaStazione GestorePopolaParametriRicerca(int numeroPagina, int dimensionePagina)
		{
			logger.Debug($"PopolaParametriRicerca");
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			return new StazioneManager.RicercaStazione
			{
				NumeroPagina = numeroPagina,
				DimensionePagina = dimensionePagina,
				CodiceIdentificativoDescrizione = GestoreCodiceIdentificativoDescrizioneTextBox.Text,
				PartitaIvaGestore = oper.OrganizzazioneAttiva().PIVA
			};
		}

	}
}