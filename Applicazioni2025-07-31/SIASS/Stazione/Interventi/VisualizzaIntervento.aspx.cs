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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
	public partial class VisualizzaIntervento : System.Web.UI.Page
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
				if (!decimal.TryParse(Request.QueryString["IdIntervento"], out decimal idIntervento))
				{
					logger.Debug($"Parametro IdIntervento mancante");
					Response.Write($"Parametro IdIntervento mancante");
					Response.End();
					return;
				}

				logger.Info($"Caricamento intervento idintervento:{idIntervento}");

				InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);

				if (infoIntervento == null)
				{
					logger.Warn($"Intervento non trovato idintervento:{idIntervento}");
					Response.Write($"Intervento non trovato: IdIntervento={idIntervento}");
					Response.End();
					return;
				}

				if (!infoIntervento.PrelievoCampioni)
				{
					Response.Redirect($"VisualizzaInterventoSenzaPrelievoCampioni.aspx?IdIntervento={idIntervento}");
				}

				ViewState.Add("IdIntervento", idIntervento);

				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);
				HeaderStazioneResponsive1.PopolaCampi(infoStazione);

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}

				if (infoIntervento.IdTipoRichiedente.HasValue)
					RichiedenteLabel.Text = HttpUtility.HtmlEncode(infoIntervento.DescrizioneTipoRichiedente);
				DataInterventoLabel.Text = infoIntervento.DataIntervento.ToString("dd/MM/yyyy");
				OraInterventoLabel.Text = infoIntervento.OraIntervento;
				if (infoIntervento.DurataIntervento.HasValue)
					DurataInterventoLabel.Text = infoIntervento.DurataIntervento.Value.ToString("F0");
				OraFineInterventoLabel.Text = infoIntervento.OraFineIntervento;

				CodiceCampagnaLabel.Text = HttpUtility.HtmlEncode(infoIntervento.CodiceCampagna);
				if (infoIntervento.NumeroCampioni.HasValue)
					NumeroCampioniLabel.Text = infoIntervento.NumeroCampioni.Value.ToString("F0");
				if (infoIntervento.QuotaCampione.HasValue)
					QuotaCampioneLabel.Text = infoIntervento.QuotaCampione.Value.ToString("F2");

				DatiCampioneBiancoLabel.Text = infoIntervento.DatiCampioneBianco;

				FileDatiLabel.Text = infoIntervento.FileDati;
				if (infoIntervento.IdStrumento.HasValue)
					StrumentoInterventoLabel.Text = infoIntervento.DescrizioneStrumento;
				FileAngoliLabel.Text = HttpUtility.HtmlEncode(infoIntervento.FileAngoli);

				ParteNomeTecnicoLabel.Text = HttpUtility.HtmlEncode(infoIntervento.ParteNomeTecnico);
				ParteAziendaTecnicoLabel.Text = HttpUtility.HtmlEncode(infoIntervento.ParteAziendaTecnico);
				ParteRuoloTecnicoLabel.Text = HttpUtility.HtmlEncode(infoIntervento.ParteRuoloTecnico);
				ParteContattiLabel.Text = HttpUtility.HtmlEncode(infoIntervento.ParteContatti);

				OperatoriInterventoGridView.DataSource = infoIntervento.OperatoriIntervento;
				OperatoriInterventoGridView.DataBind();
				OperatoriSupportoInterventoGridView.DataSource = infoIntervento.OperatoriSupportoIntervento;
				OperatoriSupportoInterventoGridView.DataBind();

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

				MisurazioniRepeater.DataSource = infoIntervento.ParametriDiCampo;
				MisurazioniRepeater.DataBind();

				PacchettiGridView.DataSource = infoIntervento.PacchettiIntervento;
				PacchettiGridView.DataBind();
				AnnotazioniPacchettiLabel.Text = HttpUtility.HtmlEncode(infoIntervento.AnnotazioniPacchetti);

				AnalitiInterventoGridView.DataSource = infoIntervento.Analiti;
				AnalitiInterventoGridView.DataBind();

				AnnotazioniLabel.Text = HttpUtility.HtmlEncode(infoIntervento.Annotazioni);
			}
		}

		protected void ChiudiButton_Click(object sender, EventArgs e)
		{
			decimal idIntervento = (decimal)ViewState["IdIntervento"];
			InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
			Response.Redirect($"ElencoInterventi.aspx?IdStazione={infoIntervento.IdStazione}");
		}



		protected void MisurazioniRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			Label valoreMisurazioneLabel = (Label)e.Item.FindControl("ValoreMisurazioneLabel");
			Label valoreMisurazioneBooleanaLabel = (Label)e.Item.FindControl("ValoreMisurazioneBooleanaLabel");
			Label fonteArpalLabel = (Label)e.Item.FindControl("FonteArpalLabel");

			if (valoreMisurazioneLabel != null && valoreMisurazioneBooleanaLabel != null)
			{
				bool seUnitaMisuraBooleana = (bool)DataBinder.Eval(e.Item.DataItem, "SeUnitaMisuraBooleana");
				if (seUnitaMisuraBooleana)
				{
					valoreMisurazioneLabel.Visible = false;
					valoreMisurazioneBooleanaLabel.Visible = true;
					if (DataBinder.Eval(e.Item.DataItem, "Valore") != null)
					{
						string valore = DataBinder.Eval(e.Item.DataItem, "Valore").ToString();
						if (valore == "1")
							valoreMisurazioneBooleanaLabel.Text = "Sì";
						else
							valoreMisurazioneBooleanaLabel.Text = "No";
					}
				}
				else
				{
					valoreMisurazioneLabel.Visible = true;
					valoreMisurazioneBooleanaLabel.Visible = false;
					if (DataBinder.Eval(e.Item.DataItem, "Valore") != null)
					{
						valoreMisurazioneLabel.Text = DataBinder.Eval(e.Item.DataItem, "Valore").ToString();
					}
				}
				if ((bool)DataBinder.Eval(e.Item.DataItem, "FonteArpal"))
				{
					fonteArpalLabel.Text = "Sì";
				}
				else
				{
					fonteArpalLabel.Text = "No";
				}
			}

		}

	}
}