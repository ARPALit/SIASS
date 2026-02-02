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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
	public partial class ElencoInterventi : System.Web.UI.Page
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
				if (!decimal.TryParse(Request.QueryString["IdStazione"], out decimal idStazione))
				{
					logger.Debug($"Parametro IdStazione mancante");
					Response.Write($"Parametro IdStazione mancante");
					Response.End();
					return;
				}

				AggiornaElencoInterventi();
				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
				if (infoStazione == null)
				{
					logger.Warn($"Stazione non trovata: IdStazione={idStazione}");
					Response.Write($"Stazione non trovata: IdStazione={idStazione}");
					Response.End();
					return;
				}

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}

				HeaderStazioneResponsive1.PopolaCampi(infoStazione);
				logger.Debug($"Elenco interventi: IdStazione={idStazione}");

				AvvisoLabel.Visible = false;
				if (!String.IsNullOrEmpty(infoStazione.ReteAppartenenza) && infoStazione.ReteAppartenenza.ToUpper() == "ORGANOCLORURATI")
				{
					if (infoStazione.Localizzazione == null || string.IsNullOrEmpty(infoStazione.Localizzazione.CodiceSIRAL))
					{
						AvvisoLabel.Text = "NOTA: la stazione appartiene alla rete Organoclorurati ma è priva di codice SIRAL. Per inviare i verbali ad ALIMS occorre indicare il codice SIRAL nella sezione Localizzazione.";
						AvvisoLabel.Visible = true;
					}
				}

				NuovoInterventoConPrelievoCampioniHyperLink.NavigateUrl = $"NuovoIntervento.aspx?IdStazione={idStazione}";
				NuovoInterventoSenzaPrelievoCampioniHyperLink.NavigateUrl = $"NuovoInterventoSenzaPrelievoCampioni.aspx?IdStazione={idStazione}";
				VisualizzaDatiAlimsStazioneHyperLink.NavigateUrl = $"~/Stazione/VisualizzaDatiAlimsStazione.aspx?IdStazione={idStazione}";
				ViewState.Add("IdStazione", idStazione);

				InterventiMultiView.SetActiveView(InterventiView);

				// Legenda tipi verbale
				string percorsoFile = Server.MapPath($"~/App_Data/legenda_tipi_verbale.html");
				if (File.Exists(percorsoFile))
					LegendaTipiVerbale.InnerHtml = File.ReadAllText(percorsoFile);

			}
		}

		protected void ElencoInterventiGridView_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			decimal idIntervento = decimal.Parse(e.CommandArgument.ToString());

			switch (e.CommandName)
			{
				case "Genera":
					ViewState.Add("IdIntervento", idIntervento);
					// E' possibile generare un intervento solo se l'intervento ha pacchetti
					var intervento = InterventoManager.CaricaInfoIntervento(idIntervento);
					if (intervento.PrelievoCampioni && (intervento.PacchettiIntervento == null || intervento.PacchettiIntervento.Count == 0))
					{
						NonPossibileGenerareVerbaleLabel.Text = "Non è possibile generare il verbale perché l'intervento non contiene pacchetti.";
						InterventiMultiView.SetActiveView(NonPossibileGenerareVerbaleView);
						return;
					}
					else
					{
						InterventiMultiView.SetActiveView(GeneraVerbaleView);
					}
					break;
				case "Scarica":
					ScaricaVerbaleIntervento(idIntervento);
					break;
				default:
					break;
			}

			ViewState.Add("Azione", e.CommandName);
			InterventiMultiView.SetActiveView(GeneraVerbaleView);
		}

		protected void ElencoInterventiGridView_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				// Pulsanti genera / scarica verbale
				// Se l'intervento risulta inviato (interventi V1) non mostra il pulsante di generazione
				// Se il file verbale è presente mostra il pulsante scarica, altrimenti mostra quello di generazione

				string statoInvioVerbaleV1 = null;
				if (DataBinder.Eval(e.Row.DataItem, "StatoInvioVerbaleV1") != null)
				{
					statoInvioVerbaleV1 = DataBinder.Eval(e.Row.DataItem, "StatoInvioVerbaleV1").ToString();
				}
				string versione = DataBinder.Eval(e.Row.DataItem, "Versione").ToString();
				decimal idIntervento = (decimal)DataBinder.Eval(e.Row.DataItem, "IdIntervento");

				LinkButton generaVerbaleLinkButton = (LinkButton)e.Row.FindControl("GeneraVerbaleLinkButton");
				LinkButton scaricaVerbaleLinkButton = (LinkButton)e.Row.FindControl("ScaricaVerbaleLinkButton");
				HyperLink dataInterventoHyperLink = (HyperLink)e.Row.FindControl("DataInterventoHyperLink");
				Label inviatoLabel = (Label)e.Row.FindControl("InviatoLabel");

				decimal idStazione = decimal.Parse(Request.QueryString["IdStazione"]);

				string percorsoFile = $"{ConfigurationManager.AppSettings["CartellaVerbaliInterventi"].Replace("[ID_STAZIONE]", idStazione.ToString())}Verbale{idIntervento}.pdf";

				if (versione == "V1")
				{
					// Versione precedente intervento
					dataInterventoHyperLink.NavigateUrl = $"VisualizzaInterventoV1.aspx?IdIntervento={idIntervento}";
					if (statoInvioVerbaleV1 == "Inviato")
					{
						generaVerbaleLinkButton.Visible = false;
						scaricaVerbaleLinkButton.Visible = false;
						inviatoLabel.Visible = true;
						inviatoLabel.Text = "Allegato su ALIMS";
					}
					else
					{
						if (File.Exists(percorsoFile))
						{
							generaVerbaleLinkButton.Visible = false;
							scaricaVerbaleLinkButton.Visible = true;
						}
						else
						{
							generaVerbaleLinkButton.Visible = true;
							scaricaVerbaleLinkButton.Visible = false;
						}
					}
				}
				else
				{
					// Versione corrente
					if (File.Exists(percorsoFile))
					{
						generaVerbaleLinkButton.Visible = false;
						scaricaVerbaleLinkButton.Visible = true;
						dataInterventoHyperLink.NavigateUrl = $"VisualizzaIntervento.aspx?IdIntervento={idIntervento}";
					}
					else
					{
						bool prelievoCampioni = (bool)DataBinder.Eval(e.Row.DataItem, "PrelievoCampioni");
						if (prelievoCampioni)
						{
							dataInterventoHyperLink.NavigateUrl = $"ModificaIntervento.aspx?IdIntervento={idIntervento}";
						}
						else
						{
							dataInterventoHyperLink.NavigateUrl = $"ModificaInterventoSenzaPrelievoCampioni.aspx?IdIntervento={idIntervento}";
						}
						generaVerbaleLinkButton.Visible = true;
						scaricaVerbaleLinkButton.Visible = false;
					}
				}
			}
		}

		private void AggiornaElencoInterventi()
		{
			decimal idStazione = decimal.Parse(Request.QueryString["IdStazione"]);
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			ElencoInterventiGridView.DataSource = InterventoManager.ElencoInterventiStazione(idStazione, oper.OrganizzazioneAttiva().Codice);
			ElencoInterventiGridView.DataBind();
		}

		protected void GeneraVerbaleView_Activate(object sender, EventArgs e)
		{
			ModelliVerbaleDropDownList.DataSource = StazioneManager.CategorieStazione();
			ModelliVerbaleDropDownList.DataBind();
			ModelliVerbaleDropDownList.Items.Add(ConfigurationManager.AppSettings["NomeReteOrganoclorurati"]);

			List<ListItem> list = new List<ListItem>();

			foreach (ListItem litem in ModelliVerbaleDropDownList.Items)
			{
				list.Add(litem);
			}

			List<ListItem> sorted = list.OrderBy(b => b.Text).ToList();

			ModelliVerbaleDropDownList.Items.Clear();

			foreach (ListItem litem in sorted)
			{
				ModelliVerbaleDropDownList.Items.Add(litem);
			}
		}

		protected void TornaElencoInterventiButton_Click(object sender, EventArgs e)
		{
			decimal idStazione = (decimal)ViewState["IdStazione"];
			Response.Redirect($"ElencoInterventi.aspx?IdStazione={idStazione}");
		}

		protected void GeneraVerbaleDaModelloButton_Click(object sender, EventArgs e)
		{
			string modelloVerbale = ModelliVerbaleDropDownList.SelectedValue;
			decimal idIntervento = (decimal)ViewState["IdIntervento"];
			decimal idStazione = decimal.Parse(Request.QueryString["IdStazione"]);

			byte[] bytes = VerbaleInterventoManager.GeneraVerbaleIntervento(idIntervento, modelloVerbale);
			string percorsoCartella = $"{ConfigurationManager.AppSettings["CartellaVerbaliInterventi"].Replace("[ID_STAZIONE]", idStazione.ToString())}";
			if (!Directory.Exists(percorsoCartella))
			{
				logger.Debug($"Creazione cartella:{percorsoCartella}");
				Directory.CreateDirectory(percorsoCartella);
			}
			string percorsoFile = $"{ConfigurationManager.AppSettings["CartellaVerbaliInterventi"].Replace("[ID_STAZIONE]", idStazione.ToString())}Verbale{idIntervento}.pdf";
			logger.Debug($"Creazione file:{percorsoFile}");
			using (var stream = File.Create(percorsoFile))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
			InterventoManager.ImpostaDataCreazioneVerbale(idIntervento);
			InterventoManager.AggiornaModelloVerbaleSelezionato(idIntervento, modelloVerbale);
			Response.Redirect($"ElencoInterventi.aspx?IdStazione={idStazione}");
		}

		private void ScaricaVerbaleIntervento(decimal idIntervento)
		{
			logger.Debug($"ScaricaVerbaleIntervento:{idIntervento}");
			decimal idStazione = decimal.Parse(Request.QueryString["IdStazione"]);
			string percorsoFile = $"{ConfigurationManager.AppSettings["CartellaVerbaliInterventi"].Replace("[ID_STAZIONE]", idStazione.ToString())}Verbale{idIntervento}.pdf";

			FileInfo file = new FileInfo(percorsoFile);

			// Se il file esiste lo scarica altrimenti mostra errore
			if (file.Exists)
			{
				long dimensione = file.Length;
				Response.Clear();
				Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(file.Name));
				Response.AddHeader("Content-Length", dimensione.ToString());
				Response.ContentType = "application/octet-stream";
				Response.WriteFile(percorsoFile);
				Response.Flush();
				Response.End();
			}
			else
			{
				logger.Error($"File non trovato {percorsoFile}");
				Response.Write($"File non trovato {percorsoFile}");
				Response.End();
			}
		}

		protected void VisualizzaDatiDaALIMSCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			AggiornaElencoInterventi();
		}

	}
}