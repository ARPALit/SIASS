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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SIASS.BLL.InterventoManager;

namespace SIASS
{
	public partial class ModificaIntervento : System.Web.UI.Page
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

			// Disabilita il pulsante Salva dopo il click per evitare inserimenti multipli
			SalvaButton.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(SalvaButton, null) + ";");

			if (!Page.IsPostBack)
			{
				ModificaInterventoMultiView.SetActiveView(InterventoView);

				if (!decimal.TryParse(Request.QueryString["IdIntervento"], out decimal idIntervento))
				{
					logger.Debug($"Parametro IdIntervento mancante");
					AvvisoLabel.Text = $"Parametro IdIntervento mancante";
					ModificaInterventoMultiView.SetActiveView(AvvisoView);
					return;
				}

				logger.Info($"Caricamento intervento idintervento:{idIntervento}");

				InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);

				if (infoIntervento == null)
				{
					logger.Warn($"Intervento non trovato idintervento:{idIntervento}");
					AvvisoLabel.Text = $"Intervento non trovato: IdIntervento={idIntervento}";
					ModificaInterventoMultiView.SetActiveView(AvvisoView);
					return;
				}

				string percorsoFile = $"{ConfigurationManager.AppSettings["CartellaVerbaliInterventi"].Replace("[ID_STAZIONE]", infoIntervento.IdStazione.ToString())}Verbale{infoIntervento.IdIntervento}.pdf";

				if (File.Exists(percorsoFile))
				{
					// Non è possibile modificare l'intervento perché è già stato generato il verbale (esiste il file)
					logger.Warn($"Intervento non modificabile idintervento:{idIntervento}");
					AvvisoLabel.Text = $"Intervento non modificabile: IdIntervento={idIntervento}";
					ModificaInterventoMultiView.SetActiveView(AvvisoView);
					return;
				}

				if (!infoIntervento.PrelievoCampioni)
				{
					Response.Redirect($"ModificaInterventoSenzaPrelievoCampioni.aspx?IdIntervento={idIntervento}");
				}

				ViewState.Add("IdIntervento", idIntervento);

				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);
				HeaderStazioneResponsive1.PopolaCampi(infoStazione);

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}

				// Popola tipologiche
				TipiRichiedenteDropDownList.DataSource = InterventoManager.TipiRichiedente();
				TipiRichiedenteDropDownList.DataBind();
				TipiRichiedenteDropDownList.Items.Insert(0, new ListItem(String.Empty));
				if (infoIntervento.IdTipoRichiedente.HasValue)
					TipiRichiedenteDropDownList.SelectedValue = infoIntervento.IdTipoRichiedente.Value.ToString();


				TipoInterventoLabel.Text = infoIntervento.DescrizioneTipoIntervento;
				ArgomentoLabel.Text = infoIntervento.DescrizioneArgomento;
				MatriceLabel.Text = infoIntervento.DescrizioneMatrice;
				SedeAccettazioneLabel.Text = infoIntervento.DenominazioneSedeAccettazione;

				TipiStrumentoInterventoDropDownList.DataSource = InterventoManager.TipiStrumentoIntervento();
				TipiStrumentoInterventoDropDownList.DataBind();
				TipiStrumentoInterventoDropDownList.Items.Insert(0, new ListItem(String.Empty));

				ViewState.Add("InfoOperatoriPerIntervento", InterventoManager.ElencoOperatoriPerIntervento());
				OperatoriPerInterventoDropDownList.DataSource = ViewState["InfoOperatoriPerIntervento"];
				OperatoriPerInterventoDropDownList.DataBind();
				OperatoriPerInterventoDropDownList.Items.Insert(0, string.Empty);

				OperatoriSupportoPerInterventoDropDownList.DataSource = ViewState["InfoOperatoriPerIntervento"];
				OperatoriSupportoPerInterventoDropDownList.DataBind();
				OperatoriSupportoPerInterventoDropDownList.Items.Insert(0, string.Empty);

				var apiAnagrafiche = Global.ApiAnagrafiche;
				var pacchetti = apiAnagrafiche.Pacchetti(infoIntervento.CodiceMatrice, infoIntervento.CodiceArgomento, infoIntervento.CodiceSedeAccettazione, null);
				var pacchettiOrdinati = InterventoManager.ElencoPacchettiPerSelezione(pacchetti, infoStazione.IdStazione);

				foreach (var p in pacchettiOrdinati)
				{
					PacchettiDropDownList.Items.Add(new ListItem($"{p.PackIdentity} - {p.PackName}", p.PackIdentity));
				}
				PacchettiDropDownList.Items.Insert(0, string.Empty);

				// Popola dati

				DataInterventoTextBox.Text = infoIntervento.DataIntervento.ToString("dd/MM/yyyy");
				OraInterventoTextBox.Text = infoIntervento.OraIntervento;
				if (infoIntervento.DurataIntervento.HasValue)
					DurataInterventoTextBox.Text = infoIntervento.DurataIntervento.Value.ToString("F0");
				OraFineInterventoTextBox.Text = infoIntervento.OraFineIntervento;
				if (infoIntervento.NumeroCampioni.HasValue)
					NumeroCampioniTextBox.Text = infoIntervento.NumeroCampioni.Value.ToString("F0");
				if (infoIntervento.QuotaCampione.HasValue)
					QuotaCampioneTextBox.Text = infoIntervento.QuotaCampione.Value.ToString("F2");

				DatiCampioneBiancoTextBox.Text = infoIntervento.DatiCampioneBianco;
				DatiCampioneBiancoTextBox.Attributes.Add("placeholder", "Indicare serigrafia, lotto e scadenza.");

				CodiceCampagnaTextBox.Text = infoIntervento.CodiceCampagna;
				FileDatiTextBox.Text = infoIntervento.FileDati;
				if (infoIntervento.IdStrumento.HasValue)
					TipiStrumentoInterventoDropDownList.SelectedValue = infoIntervento.IdStrumento.ToString();
				FileAngoliTextBox.Text = infoIntervento.FileAngoli;

				ParteNomeTecnicoTextBox.Text = infoIntervento.ParteNomeTecnico;
				ParteAziendaTecnicoTextBox.Text = infoIntervento.ParteAziendaTecnico;
				ParteRuoloTecnicoTextBox.Text = infoIntervento.ParteRuoloTecnico;
				ParteContattiTextBox.Text = infoIntervento.ParteContatti;

				AnnotazioniTextBox.Text = infoIntervento.Annotazioni;
				AnnotazioniPacchettiTextBox.Text = infoIntervento.AnnotazioniPacchetti;

				SiglaVerbaleLabel.Text = infoIntervento.SiglaVerbale;

				ViewState.Add("InfoOperatoriIntervento", infoIntervento.OperatoriIntervento);
				AggiornaElencoOperatori();
				ViewState.Add("InfoOperatoriSupportoIntervento", infoIntervento.OperatoriSupportoIntervento);
				AggiornaElencoOperatoriSupporto();

				ViewState.Add("InfoPacchettiIntervento", infoIntervento.PacchettiIntervento);
				PacchettiGridView.DataSource = infoIntervento.PacchettiIntervento;
				PacchettiGridView.DataBind();

				ViewState.Add("InfoAnalitiIntervento", infoIntervento.Analiti);
				AggiornaElencoAnaliti(infoIntervento.Analiti);

				ViewState.Add("InfoParametriDiCampo", infoIntervento.ParametriDiCampo);
				AggiornaElencoMisurazioni(infoIntervento.ParametriDiCampo);


				// Nasconde/Mostra campi in base a tipo stazione
				FileDatiPanel.Visible = false;
				TipiStrumentoInterventoPanel.Visible = false;
				FileAngoliPanel.Visible = false;

				if (infoIntervento.CategoriaStazione == "Sorgente")
				{
					FileDatiPanel.Visible = true;
				}
				if (infoIntervento.CategoriaStazione == "Pozzo")
				{
					FileDatiPanel.Visible = true;
					TipiStrumentoInterventoPanel.Visible = true;
				}
				if (infoIntervento.CategoriaStazione == "Piezometro")
				{
					FileDatiPanel.Visible = true;
					TipiStrumentoInterventoPanel.Visible = true;
				}
				if (infoIntervento.CategoriaStazione == "Inclinometro")
				{
					FileAngoliPanel.Visible = true;
				}
				if (infoIntervento.CategoriaStazione == "Lago")
				{
					FileDatiPanel.Visible = true;
				}
			}
		}
		private void AggiornaElencoOperatori()
		{
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
			OperatoriInterventoGridView.DataSource = infoOperatoriIntervento.OrderBy(i => i.DescrizioneOperatore);
			OperatoriInterventoGridView.DataBind();
		}
		private void AggiornaElencoOperatoriSupporto()
		{
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriSupportoIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriSupportoIntervento"];
			OperatoriSupportoInterventoGridView.DataSource = infoOperatoriSupportoIntervento.OrderBy(i => i.DescrizioneOperatore);
			OperatoriSupportoInterventoGridView.DataBind();
		}

		protected void AggiungiOperatoreButton_Click(object sender, EventArgs e)
		{
			if (OperatoriPerInterventoDropDownList.SelectedIndex == 0)
				return;

			decimal idOperatoreSelezionato = decimal.Parse(OperatoriPerInterventoDropDownList.SelectedValue);

			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriSupportoIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriSupportoIntervento"];
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriPerIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriPerIntervento"];

			if (
				!infoOperatoriIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).Any()
				&&
				!infoOperatoriSupportoIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).Any()
				)
			{
				infoOperatoriIntervento.Add(infoOperatoriPerIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).FirstOrDefault());
				ViewState.Add("InfoOperatoriIntervento", infoOperatoriIntervento);
				AggiornaElencoOperatori();
			}
			OperatoriPerInterventoDropDownList.SelectedIndex = 0;
		}
		protected void AggiungiOperatoreSupportoButton_Click(object sender, EventArgs e)
		{
			if (OperatoriSupportoPerInterventoDropDownList.SelectedIndex == 0)
				return;

			decimal idOperatoreSelezionato = decimal.Parse(OperatoriSupportoPerInterventoDropDownList.SelectedValue);

			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriSupportoIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriSupportoIntervento"];
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriPerIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriPerIntervento"];

			if (
				!infoOperatoriSupportoIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).Any()
				&&
				!infoOperatoriIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).Any()
				)
			{
				infoOperatoriSupportoIntervento.Add(infoOperatoriPerIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).FirstOrDefault());
				ViewState.Add("InfoOperatoriSupportoIntervento", infoOperatoriSupportoIntervento);
				AggiornaElencoOperatoriSupporto();
			}
			OperatoriSupportoPerInterventoDropDownList.SelectedIndex = 0;
		}

		protected void OperatoriInterventoGridView_SelectedIndexChanged(object sender, EventArgs e)
		{
			decimal idOperatoreDaRimuovere = (decimal)OperatoriInterventoGridView.SelectedValue;

			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];

			var operatoreDaRimuovere = infoOperatoriIntervento.Where(i => i.IdOperatore == idOperatoreDaRimuovere).FirstOrDefault();
			if (operatoreDaRimuovere != null)
			{
				infoOperatoriIntervento.Remove(operatoreDaRimuovere);
				ViewState.Add("InfoOperatoriIntervento", infoOperatoriIntervento);
				AggiornaElencoOperatori();
			}
		}
		protected void OperatoriSupportoInterventoGridView_SelectedIndexChanged(object sender, EventArgs e)
		{
			decimal idOperatoreDaRimuovere = (decimal)OperatoriSupportoInterventoGridView.SelectedValue;

			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriSupportoIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriSupportoIntervento"];
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriPerIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriPerIntervento"];

			var operatoreDaRimuovere = infoOperatoriSupportoIntervento.Where(i => i.IdOperatore == idOperatoreDaRimuovere).FirstOrDefault();
			if (operatoreDaRimuovere != null)
			{
				infoOperatoriSupportoIntervento.Remove(operatoreDaRimuovere);
				ViewState.Add("InfoOperatoriSupportoIntervento", infoOperatoriSupportoIntervento);
				AggiornaElencoOperatoriSupporto();
			}
		}

		protected void AnnullaButton_Click(object sender, EventArgs e)
		{
			decimal idIntervento = (decimal)ViewState["IdIntervento"];
			InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
			Response.Redirect($"ElencoInterventi.aspx?IdStazione={infoIntervento.IdStazione}");
		}

		protected void EliminaButton_Click(object sender, EventArgs e)
		{
			decimal idIntervento = (decimal)ViewState["IdIntervento"];
			using (SIASSEntities context = new SIASSEntities())
			{
				// Carica l'oggetto dal modello
				Intervento i = (from j in context.Interventi
								where j.ID_INTERVENTO == idIntervento
								select j).FirstOrDefault();

				if (i != null)
				{
					context.Interventi.Remove(i);
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					logger.Info($"Eliminato intervento - id:{i.ID_INTERVENTO} - id stazione:{i.ID_STAZIONE} - Operatore:{oper.Nome} {oper.Cognome}");
					context.SaveChanges();
					// Elimina eventuale cartella allegati
					string percorsoCartella = HttpContext.Current.Server.MapPath($"~/File/Allegati/Stazione{i.ID_STAZIONE}/Intervento{i.ID_INTERVENTO}");
					DirectoryInfo cartella = new DirectoryInfo(percorsoCartella);
					if (cartella.Exists)
						cartella.Delete(true);

					Response.Redirect($"ElencoInterventi.aspx?IdStazione={i.ID_STAZIONE}");
				}
				else
				{
					Response.Write($"Intervento {idIntervento} non trovato");
					Response.End();
				}
			}

		}

		protected void SalvaButton_Click(object sender, EventArgs e)
		{
			if (AnnotazioniTextBox.Text.Trim().Length > 1000)
			{
				CustomValidator validatoreDatiCampioneBianco = new CustomValidator
				{
					IsValid = false,
					ErrorMessage = "Dati campione bianco: lunghezza massima 1000 caratteri"
				};
				Page.Validators.Add(validatoreDatiCampioneBianco);
			}
			if (AnnotazioniTextBox.Text.Trim().Length > 2000)
			{
				CustomValidator validatoreAnnotazioni = new CustomValidator
				{
					IsValid = false,
					ErrorMessage = "Annotazioni: lunghezza massima 2000 caratteri"
				};
				Page.Validators.Add(validatoreAnnotazioni);
			}
			if (AnnotazioniPacchettiTextBox.Text.Trim().Length > 2000)
			{
				CustomValidator validatoreAnnotazioniPacchetti = new CustomValidator
				{
					IsValid = false,
					ErrorMessage = "Annotazioni pacchetti: lunghezza massima 2000 caratteri"
				};
				Page.Validators.Add(validatoreAnnotazioniPacchetti);
			}

			if (Page.IsValid)
			{
				decimal idIntervento = (decimal)ViewState["IdIntervento"];
				logger.Info($"Aggiornamento intervento {idIntervento}");

				var intervento = InterventoManager.CaricaInfoIntervento(idIntervento);

				if (intervento == null)
				{
					logger.Warn($"Intervento {idIntervento} non trovato");
					Response.Write($"Intervento {idIntervento} non trovato");
					Response.End();
					return;
				}

				if (TipiRichiedenteDropDownList.SelectedIndex > 0)
					intervento.IdTipoRichiedente = decimal.Parse(TipiRichiedenteDropDownList.SelectedValue);
				else
					intervento.IdTipoRichiedente = null;
				intervento.DataIntervento = DateTime.Parse(DataInterventoTextBox.Text.Trim());

				if (string.IsNullOrEmpty(OraInterventoTextBox.Text.Trim()))
					intervento.OraIntervento = null;
				else
					intervento.OraIntervento = OraInterventoTextBox.Text.Trim();
				if (String.IsNullOrEmpty(DurataInterventoTextBox.Text.Trim()))
					intervento.DurataIntervento = null;
				else
					intervento.DurataIntervento = decimal.Parse(DurataInterventoTextBox.Text.Trim());
				if (string.IsNullOrEmpty(OraFineInterventoTextBox.Text.Trim()))
					intervento.OraFineIntervento = null;
				else
					intervento.OraFineIntervento = OraFineInterventoTextBox.Text.Trim();
				if (string.IsNullOrEmpty(NumeroCampioniTextBox.Text.Trim()))
					intervento.NumeroCampioni = null;
				else
					intervento.NumeroCampioni = decimal.Parse(NumeroCampioniTextBox.Text.Trim());
				if (string.IsNullOrEmpty(QuotaCampioneTextBox.Text.Trim()))
					intervento.QuotaCampione = null;
				else
					intervento.QuotaCampione = decimal.Parse(QuotaCampioneTextBox.Text.Trim());

				if (String.IsNullOrEmpty(DatiCampioneBiancoTextBox.Text.Trim()))
				{
					intervento.DatiCampioneBianco = null;
					intervento.CampioneBianco = false;
				}
				else
				{
					intervento.DatiCampioneBianco = DatiCampioneBiancoTextBox.Text.Trim();
					intervento.CampioneBianco = true;
				}

				if (string.IsNullOrEmpty(CodiceCampagnaTextBox.Text.Trim()))
					intervento.CodiceCampagna = null;
				else
					intervento.CodiceCampagna = CodiceCampagnaTextBox.Text.Trim();
				if (string.IsNullOrEmpty(FileDatiTextBox.Text.Trim()))
					intervento.FileDati = null;
				else
					intervento.FileDati = FileDatiTextBox.Text.Trim();
				if (string.IsNullOrEmpty(FileAngoliTextBox.Text.Trim()))
					intervento.FileAngoli = null;
				else
					intervento.FileAngoli = FileAngoliTextBox.Text.Trim();
				if (TipiStrumentoInterventoDropDownList.SelectedIndex > 0)
					intervento.IdStrumento = decimal.Parse(TipiStrumentoInterventoDropDownList.SelectedValue);
				else
					intervento.IdStrumento = null;

				if (String.IsNullOrEmpty(ParteNomeTecnicoTextBox.Text.Trim()))
					intervento.ParteNomeTecnico = null;
				else
					intervento.ParteNomeTecnico = ParteNomeTecnicoTextBox.Text.Trim();
				if (String.IsNullOrEmpty(ParteAziendaTecnicoTextBox.Text.Trim()))
					intervento.ParteAziendaTecnico = null;
				else
					intervento.ParteAziendaTecnico = ParteAziendaTecnicoTextBox.Text.Trim();
				if (String.IsNullOrEmpty(ParteRuoloTecnicoTextBox.Text.Trim()))
					intervento.ParteRuoloTecnico = null;
				else
					intervento.ParteRuoloTecnico = ParteRuoloTecnicoTextBox.Text.Trim();
				if (String.IsNullOrEmpty(ParteContattiTextBox.Text.Trim()))
					intervento.ParteContatti = null;
				else
					intervento.ParteContatti = ParteContattiTextBox.Text.Trim();

				if (String.IsNullOrEmpty(AnnotazioniTextBox.Text.Trim()))
					intervento.Annotazioni = null;
				else
					intervento.Annotazioni = AnnotazioniTextBox.Text.Trim();

				if (String.IsNullOrEmpty(AnnotazioniPacchettiTextBox.Text.Trim()))
					intervento.AnnotazioniPacchetti = null;
				else
					intervento.AnnotazioniPacchetti = AnnotazioniPacchettiTextBox.Text.Trim();

				intervento.UltimoAggiornamento = DateTime.Now;
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				intervento.AutoreUltimoAggiornamento = $"{oper.Nome} {oper.Cognome}";

				logger.Info($"Aggiornato intervento - id:{intervento.IdIntervento} - id stazione:{intervento.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");


				// Aggiorna operatori
				List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
				if (infoOperatoriIntervento.Count == 0)
				{
					ModelState.AddModelError("", $"Inserire almeno un operatore.");
					return;
				}
				intervento.OperatoriIntervento = infoOperatoriIntervento;

				List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriSupportoIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriSupportoIntervento"];
				intervento.OperatoriSupportoIntervento = infoOperatoriSupportoIntervento;

				intervento.AutoreUltimoAggiornamento = $"{oper.Nome} {oper.Cognome}";

				// Aggiorna pacchetti
				var pacchettiIntervento = (List<InfoIntervento.InfoPacchettoIntervento>)ViewState["InfoPacchettiIntervento"];
				intervento.PacchettiIntervento = pacchettiIntervento;

				// Aggiorna analiti
				var analitiIntervento = (List<InfoAnalita>)ViewState["InfoAnalitiIntervento"];
				intervento.Analiti = analitiIntervento;



				// Aggiorna le misurazioni
				logger.Info($"Aggiornamento misurazioni idIntervento:{intervento.IdIntervento}");
				List<InfoParametroCampo> parametriDicampoDaAggiornare = new List<InfoParametroCampo>();
				foreach (RepeaterItem rpitem in MisurazioniRepeater.Items)
				{
					TextBox idGrandezzaTextBox = (TextBox)rpitem.FindControl("IdGrandezzaTextBox");
					TextBox valoreMisurazioneTextBox = (TextBox)rpitem.FindControl("ValoreMisurazioneTextBox");
					DropDownList valoreMisurazioneBooleanaDropDownList = (DropDownList)rpitem.FindControl("ValoreMisurazioneBooleanaDropDownList");
					CheckBox fonteArpalCheckBox = (CheckBox)rpitem.FindControl("FonteArpalCheckBox");

					// In base al controllo visibile legge il valore dalla texbox o alla dropdown
					string valoreMisurazione;
					if (valoreMisurazioneTextBox.Visible)
					{
						valoreMisurazione = valoreMisurazioneTextBox.Text.Trim();
					}
					else
					{
						valoreMisurazione = valoreMisurazioneBooleanaDropDownList.SelectedValue;
					}

					var misurazione = ((List<InfoParametroCampo>)ViewState["InfoParametriDiCampo"]).Where(i => i.IdGrandezzaStazione == decimal.Parse(idGrandezzaTextBox.Text)).FirstOrDefault();

					if (misurazione != null)
					{
						if (String.IsNullOrEmpty(valoreMisurazione))
							misurazione.Valore = null;
						else
						{
							misurazione.Valore = decimal.Parse(valoreMisurazione);
						}
						misurazione.FonteArpal = fonteArpalCheckBox.Checked;
					}
					parametriDicampoDaAggiornare.Add(misurazione);
				}
				intervento.ParametriDiCampo = parametriDicampoDaAggiornare;

				var risultatoAggiornamento = InterventoManager.AggiornaIntervento(intervento, true);
				if (risultatoAggiornamento.IsSuccess)
				{
					logger.Info($"Intervento aggiornato:{intervento.IdIntervento}");
				}
				else
				{
					logger.Error($"Intervento non aggiornato:{intervento.IdIntervento} - Errore:{risultatoAggiornamento.Errors[0].Message}");
					AvvisoLabel.Text = $"Intervento non aggiornato:{intervento.IdIntervento} - Errore:{risultatoAggiornamento.Errors[0].Message}";
					ModificaInterventoMultiView.SetActiveView(AvvisoView);
					return;
				}

				Response.Redirect($"ElencoInterventi.aspx?IdStazione={intervento.IdStazione}");

			}

		}

		protected void MisurazioniRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			TextBox valoreMisurazioneTextBox = (TextBox)e.Item.FindControl("ValoreMisurazioneTextBox");
			DropDownList valoreMisurazioneBooleanaDropDownList = (DropDownList)e.Item.FindControl("ValoreMisurazioneBooleanaDropDownList");
			Label grandezzaEUnitaMisuraLabel = (Label)e.Item.FindControl("GrandezzaEUnitaMisuraLabel");
			CheckBox fonteArpalCheckBox = (CheckBox)e.Item.FindControl("FonteArpalCheckBox");

			if (valoreMisurazioneTextBox != null && valoreMisurazioneBooleanaDropDownList != null)
			{
				bool seUnitaMisuraBooleana = (bool)DataBinder.Eval(e.Item.DataItem, "SeUnitaMisuraBooleana");
				if (seUnitaMisuraBooleana)
				{
					valoreMisurazioneTextBox.Visible = false;
					valoreMisurazioneBooleanaDropDownList.Visible = true;

					valoreMisurazioneBooleanaDropDownList.Items.Add(new ListItem(String.Empty, String.Empty));
					valoreMisurazioneBooleanaDropDownList.Items.Add(new ListItem("Sì", "1"));
					valoreMisurazioneBooleanaDropDownList.Items.Add(new ListItem("No", "0"));

					grandezzaEUnitaMisuraLabel.AssociatedControlID = valoreMisurazioneBooleanaDropDownList.ID;
					if (DataBinder.Eval(e.Item.DataItem, "Valore") != null)
					{
						valoreMisurazioneBooleanaDropDownList.SelectedValue = DataBinder.Eval(e.Item.DataItem, "Valore").ToString();
					}
				}
				else
				{
					valoreMisurazioneTextBox.Visible = true;
					valoreMisurazioneBooleanaDropDownList.Visible = false;

					decimal numeroDecimali = (decimal)DataBinder.Eval(e.Item.DataItem, "NumeroDecimali");
					valoreMisurazioneTextBox.Attributes.Add("placeholder", $"{numeroDecimali} decimali");
					if (DataBinder.Eval(e.Item.DataItem, "Valore") != null)
					{
						valoreMisurazioneTextBox.Text = DataBinder.Eval(e.Item.DataItem, "Valore").ToString();
					}
					grandezzaEUnitaMisuraLabel.AssociatedControlID = valoreMisurazioneTextBox.ID;
				}
				fonteArpalCheckBox.Checked = (bool)DataBinder.Eval(e.Item.DataItem, "FonteArpal");
			}

		}

		private static void AggiuntaAnalita(List<InfoAnalita> elencoAnaliti, InfoAnalita analitaDaAggiungere)
		{
			EliminazioneAnalita(elencoAnaliti, analitaDaAggiungere.Codice);
			elencoAnaliti.Add(analitaDaAggiungere);
		}

		private static void EliminazioneAnalita(List<InfoAnalita> elencoAnaliti, string codiceAnalitaDaEliminare)
		{
			var analita = elencoAnaliti.Find(a => string.Equals(codiceAnalitaDaEliminare, a.Codice, StringComparison.InvariantCultureIgnoreCase));
			if (analita != null)
				elencoAnaliti.Remove(analita);
		}


		protected void ApriRicercaAnalitiButton_Click(object sender, EventArgs e)
		{
			ModificaInterventoMultiView.SetActiveView(RicercaAnalitiView);
		}

		protected void AnnullaRicercaAnalitiButton_Click(object sender, EventArgs e)
		{
			ModificaInterventoMultiView.SetActiveView(InterventoView);
		}

		protected void CercaAnalitiButton_Click(object sender, EventArgs e)
		{
			// Almeno due caratteri per fare la ricerca
			if (RicercaAnalitiTextBox.Text.Trim().Length < 2)
				return;
			var apiAnagrafiche = Global.ApiAnagrafiche;
			decimal idIntervento = (decimal)ViewState["IdIntervento"];
			InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
			var risultatiRicercaAnaliti = InterventoManager.RicercaAnaliti(apiAnagrafiche, "laboratorio", infoIntervento.CodiceMatrice, null, RicercaAnalitiTextBox.Text.Trim());
			ViewState.Add("RisultatiRicercaAnaliti", risultatiRicercaAnaliti);
			ElencoRisultatiRicercaAnalitiGridView.DataSource = risultatiRicercaAnaliti;
			ElencoRisultatiRicercaAnalitiGridView.DataBind();
		}

		protected void ElencoRisultatiRicercaAnalitiGridView_SelectedIndexChanged(object sender, EventArgs e)
		{
			var risultatiRicercaAnaliti = (List<InfoAnalita>)ViewState["RisultatiRicercaAnaliti"];
			var analitaSelezionato = risultatiRicercaAnaliti.Where(i => i.Codice.Equals(ElencoRisultatiRicercaAnalitiGridView.SelectedValue.ToString())).FirstOrDefault();
			var analitiIntervento = (List<InfoAnalita>)ViewState["InfoAnalitiIntervento"];
			AggiuntaAnalita(analitiIntervento, analitaSelezionato);
			ViewState.Add("InfoAnalitiIntervento", analitiIntervento);
			AggiornaElencoAnaliti(analitiIntervento);
			ModificaInterventoMultiView.SetActiveView(InterventoView);
		}

		private void AggiornaElencoAnaliti(List<InfoAnalita> analitiIntervento)
		{
			AnalitiInterventoGridView.DataSource = analitiIntervento;
			AnalitiInterventoGridView.DataBind();
		}

		private void AggiornaElencoMisurazioni(List<InfoParametroCampo> parametriDiCampo)
		{
			MisurazioniRepeater.DataSource = parametriDiCampo;
			MisurazioniRepeater.DataBind();
		}
		protected void AnalitiInterventoGridView_SelectedIndexChanged(object sender, EventArgs e)
		{
			var codiceAnalitaSelezionato = AnalitiInterventoGridView.SelectedValue.ToString();
			var analitiIntervento = (List<InfoAnalita>)ViewState["InfoAnalitiIntervento"];
			EliminazioneAnalita(analitiIntervento, codiceAnalitaSelezionato);
			ViewState.Add("InfoAnalitiIntervento", analitiIntervento);
			AggiornaElencoAnaliti(analitiIntervento);
		}


		protected void PacchettiGridView_SelectedIndexChanged(object sender, EventArgs e)
		{
			string codicePacchettoDaRimuovere = PacchettiGridView.SelectedDataKey["CodicePacchetto"].ToString();

			List<InfoIntervento.InfoPacchettoIntervento> pacchettiIntervento = (List<InfoIntervento.InfoPacchettoIntervento>)ViewState["InfoPacchettiIntervento"];

			var pacchettoDaRimuovere = pacchettiIntervento.Where(i => i.CodicePacchetto == codicePacchettoDaRimuovere).FirstOrDefault();
			if (pacchettoDaRimuovere != null)
			{
				pacchettiIntervento.Remove(pacchettoDaRimuovere);

				// Rimuove i relativi parametri di campo
				var parametriDiCampo = (List<InfoParametroCampo>)ViewState["InfoParametriDiCampo"];

				parametriDiCampo.RemoveAll(i => i.CodicePacchetto == codicePacchettoDaRimuovere);

				ViewState.Add("InfoParametriDiCampo", parametriDiCampo);
				AggiornaElencoMisurazioni(parametriDiCampo);

				ViewState.Add("PacchettiIntervento", pacchettiIntervento);
				PacchettiGridView.DataSource = pacchettiIntervento;
				PacchettiGridView.DataBind();

				// Rimuove gli analiti
				var analitiIntervento = (List<InfoAnalita>)ViewState["InfoAnalitiIntervento"];
				analitiIntervento.RemoveAll(i => i.CodicePacchetto == codicePacchettoDaRimuovere);
				ViewState.Add("InfoAnalitiIntervento", analitiIntervento);
				AggiornaElencoAnaliti(analitiIntervento);

			}
		}

		protected void AggiungiPacchettoButton_Click(object sender, EventArgs e)
		{
			if (PacchettiDropDownList.SelectedIndex == 0)
			{
				return;
			}
			InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento((decimal)ViewState["IdIntervento"]);
			var apiAnagrafiche = Global.ApiAnagrafiche;

			List<InfoIntervento.InfoPacchettoIntervento> pacchettiIntervento = (List<InfoIntervento.InfoPacchettoIntervento>)ViewState["InfoPacchettiIntervento"];
			string codice = PacchettiDropDownList.SelectedValue;
			string descrizione = PacchettiDropDownList.SelectedItem.Text;

			var analitiIntervento = (List<InfoAnalita>)ViewState["InfoAnalitiIntervento"];
			List<InfoAnalita> analitiDaAggiungere = new List<InfoAnalita>();


			if (pacchettiIntervento.Any(i => i.CodicePacchetto == codice))
			{
				// Se il pacchetto è già in elenco non viene aggiunto
				return;
			}

			// Pacchetto non in elenco, viene aggiunto

			// Prima di aggiungere il pacchetto carica analiti e misurazioni

			// Analiti
			var risultatoLetturaAnaliti = InterventoManager.Analiti(infoIntervento.CodiceMatrice, infoIntervento.CodiceArgomento, new List<string> { codice }, linea: "laboratorio", apiAnagrafiche);
			if (risultatoLetturaAnaliti.IsFailed)
			{
				logger.Error($"{risultatoLetturaAnaliti.Errors[0].Message} - Contattare l'amministratore del sistema.");
				Response.End();
				AvvisoLabel.Text = $"{risultatoLetturaAnaliti.Errors[0].Message} - Contattare l'amministratore del sistema.";
				ModificaInterventoMultiView.SetActiveView(AvvisoView);
				return;
			}
			else
			{
				analitiDaAggiungere = risultatoLetturaAnaliti.Value;
				logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Analiti da aggiungere: {JsonSerializer.Serialize(analitiDaAggiungere)}");
			}

			// Aggiunge solo gli analiti non già presenti
			foreach (var a in analitiDaAggiungere)
			{
				if (!analitiIntervento.Any(i => i.Codice == a.Codice))
				{
					analitiIntervento.Add(a);
				}
			}

			// Carica le misurazioni verificando che le relative grandezze siano configurate per la stazione

			List<InfoAnalita> misurazioniDaAggiungere = new List<InfoAnalita>();
			var risultatoLetturaMisurazioni = Analiti(infoIntervento.CodiceMatrice, infoIntervento.CodiceArgomento, new List<string> { codice }, linea: "territorio", apiAnagrafiche);
			if (risultatoLetturaMisurazioni.IsFailed)
			{
				AvvisoLabel.Text = $"{risultatoLetturaMisurazioni.Errors[0].Message} - Contattare l'amministratore del sistema.";
				ModificaInterventoMultiView.SetActiveView(AvvisoView);
				return;
			}
			else
			{
				misurazioniDaAggiungere = risultatoLetturaMisurazioni.Value;
				logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Misurazioni: {JsonSerializer.Serialize(misurazioniDaAggiungere)}");
			}

			// Costruisce l'elenco dei parametri di campo dell'intervento verificando che le misurazioni siano associate a grandezze della stazione
			// Se la verifica delle grandezze stazioni fallisce, non aggiunge pacchetto, analiti e misurazioni e segnala errore
			List<ParametroCampoInterventi> parametriCampoInterventoDaAggiungere = new List<ParametroCampoInterventi>();
			var risultatoParametriCampoIntervento = InterventoManager.ParametriCampoIntervento(infoIntervento.IdStazione, misurazioniDaAggiungere);
			if (risultatoParametriCampoIntervento.IsFailed)
			{
				AvvisoLabel.Text = $"{risultatoParametriCampoIntervento.Errors[0].Message} - Contattare l'amministratore del sistema.";
				ModificaInterventoMultiView.SetActiveView(AvvisoView);
				return;
			}
			else
			{
				parametriCampoInterventoDaAggiungere = risultatoParametriCampoIntervento.Value;
				logger.Debug($"{MethodBase.GetCurrentMethod().Name} - ParametriCampoIntervento: {JsonSerializer.Serialize(parametriCampoInterventoDaAggiungere)}");
			}
			// Aggiunge solo i parametri non già presenti
			var parametriDiCampo = (List<InfoParametroCampo>)ViewState["InfoParametriDiCampo"];

			using (SIASSEntities context = new SIASSEntities())
			{
				var grandezzeStazione = context.GrandezzeStazione.Where(i => i.ID_STAZIONE == infoIntervento.IdStazione).ToList();

				foreach (var p in parametriCampoInterventoDaAggiungere)
				{
					if (!parametriDiCampo.Any(i => i.IdGrandezzaStazione == p.ID_GRANDEZZA_STAZIONE))
					{
						parametriDiCampo.Add(
							new InfoParametroCampo
							{
								IdGrandezzaStazione = p.ID_GRANDEZZA_STAZIONE,
								Grandezza = grandezzeStazione.Where(i => i.ID_GRANDEZZA_STAZIONE == p.ID_GRANDEZZA_STAZIONE).FirstOrDefault().GRANDEZZA,
								UnitaMisura = grandezzeStazione.Where(i => i.ID_GRANDEZZA_STAZIONE == p.ID_GRANDEZZA_STAZIONE).FirstOrDefault().UNITA_MISURA,
								SeUnitaMisuraBooleana = context.GrandezzeStazione.Where(i => i.ID_STAZIONE == infoIntervento.IdStazione).Where(i => i.ID_GRANDEZZA_STAZIONE == p.ID_GRANDEZZA_STAZIONE).FirstOrDefault().TipoUnitaMisura.SE_BOOLEANO,
								NumeroDecimali = grandezzeStazione.Where(i => i.ID_GRANDEZZA_STAZIONE == p.ID_GRANDEZZA_STAZIONE).FirstOrDefault().NUMERO_DECIMALI,
								FonteArpal = true,
								CodicePacchetto = p.PACCHETTO_CODICE
							}
							);
					}
				}
			}

			// Aggiunge pacchetto
			pacchettiIntervento.Add(new InfoIntervento.InfoPacchettoIntervento
			{
				IdIntervento = infoIntervento.IdIntervento,
				CodiceArgomento = infoIntervento.CodiceArgomento,
				CodicePacchetto = codice,
				DescrizionePacchetto = descrizione,
				Sede = infoIntervento.CodiceSedeAccettazione
			}
			);

			ViewState.Add("InfoParametriDiCampo", parametriDiCampo);
			AggiornaElencoMisurazioni(parametriDiCampo);

			ViewState.Add("InfoAnalitiIntervento", analitiIntervento);
			AggiornaElencoAnaliti(analitiIntervento);

			pacchettiIntervento = pacchettiIntervento.OrderBy(i => i.CodicePacchetto).ToList();
			ViewState.Add("PacchettiIntervento", pacchettiIntervento);
			PacchettiGridView.DataSource = pacchettiIntervento;
			PacchettiGridView.DataBind();
		}

        protected void AnnullaAvvisoButton_Click(object sender, EventArgs e)
        {
			decimal idIntervento = (decimal)ViewState["IdIntervento"];
			Response.Redirect($"ModificaIntervento.aspx?IdIntervento={idIntervento}");
		}
	}
}