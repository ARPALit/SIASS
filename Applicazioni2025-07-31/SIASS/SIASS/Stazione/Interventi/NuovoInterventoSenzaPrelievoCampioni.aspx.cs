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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ApiAnagraficheArpal.ConfigurazioneOperatore;
using static SIASS.BLL.InterventoManager;

namespace SIASS
{
	public partial class NuovoInterventoSenzaPrelievoCampioni : System.Web.UI.Page
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

			if (!decimal.TryParse(Request.QueryString["IdStazione"], out decimal idStazione))
			{
				logger.Debug($"Parametro IdStazione mancante");
				Response.Write($"Parametro IdStazione mancante");
				Response.End();
				return;
			}

			InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
			if (infoStazione == null)
			{
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
			ViewState.Add("IdStazione", idStazione);


			if (!Page.IsPostBack)
			{
				// Popola tipologiche
				TipiInterventoDropDownList.DataSource = InterventoManager.TipiIntervento()
					.Where(i => i.SENZA_PRELIEVO_CAMPIONI)
					.OrderBy(i => i.ORDINE)
					.ThenBy(i => i.DESCRIZIONE_TIPO_INTERVENTO)
					.ToList();
				TipiInterventoDropDownList.DataBind();

				TipiArgomentoDropDownList.DataSource = InterventoManager.TipiArgomento();
				TipiArgomentoDropDownList.DataBind();

				TipiRichiedenteDropDownList.DataSource = InterventoManager.TipiRichiedente();
				TipiRichiedenteDropDownList.DataBind();
				TipiRichiedenteDropDownList.Items.Insert(0, new ListItem(String.Empty));
				if (TipiRichiedenteDropDownList.Items.Count > 1)
					TipiRichiedenteDropDownList.SelectedIndex = 1;
				CodiceCampagnaTextBox.Text = DateTime.Now.ToString("MMy");

				List<InfoIntervento.InfoOperatoreIntervento> operatoriIntervento = new List<InfoIntervento.InfoOperatoreIntervento>();
				ViewState.Add("InfoOperatoriIntervento", operatoriIntervento);

				ViewState.Add("InfoOperatoriPerIntervento", InterventoManager.ElencoOperatoriPerIntervento());
				OperatoriPerInterventoDropDownList.DataSource = ViewState["InfoOperatoriPerIntervento"];
				OperatoriPerInterventoDropDownList.DataBind();
				OperatoriPerInterventoDropDownList.Items.Insert(0, string.Empty);
				AggiornaElencoOperatori();

			}
		}


		private void AggiornaElencoOperatori()
		{
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
			OperatoriInterventoGridView.DataSource = infoOperatoriIntervento.OrderBy(i => i.DescrizioneOperatore);
			OperatoriInterventoGridView.DataBind();
		}

		protected void AggiungiOperatoreButton_Click(object sender, EventArgs e)
		{
			if (OperatoriPerInterventoDropDownList.SelectedIndex == 0)
				return;

			decimal idOperatoreSelezionato = decimal.Parse(OperatoriPerInterventoDropDownList.SelectedValue);

			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriPerIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriPerIntervento"];

			if (
				!infoOperatoriIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).Any()
				)
			{
				infoOperatoriIntervento.Add(infoOperatoriPerIntervento.Where(i => i.IdOperatore == idOperatoreSelezionato).FirstOrDefault());
				ViewState.Add("InfoOperatoriIntervento", infoOperatoriIntervento);
				AggiornaElencoOperatori();
			}
			OperatoriPerInterventoDropDownList.SelectedIndex = 0;
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

		protected void AnnullaButton_Click(object sender, EventArgs e)
		{
			decimal idStazione = (decimal)ViewState["IdStazione"];
			Response.Redirect($"ElencoInterventi.aspx?IdStazione={idStazione}");
		}

		protected void SalvaButton_Click(object sender, EventArgs e)
		{
			if (!Page.IsValid)
				return;
			decimal idStazione = (decimal)ViewState["IdStazione"];

			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			var apiAnagrafiche = Global.ApiAnagrafiche;

			logger.Info($"Inserimento intervento per idStazione:{idStazione}");

			List<InfoIntervento.InfoOperatoreIntervento> infoOperatoriIntervento = (List<InfoIntervento.InfoOperatoreIntervento>)ViewState["InfoOperatoriIntervento"];
			if (infoOperatoriIntervento.Count == 0)
			{
				ModelState.AddModelError("", $"Inserire almeno un operatore.");
				return;
			}

			var operatoriIntervento = new List<RichiestaNuovoInterventoSenzaPrelievoCampioni.Operatore>();
			foreach (var o in infoOperatoriIntervento)
			{
				RichiestaNuovoInterventoSenzaPrelievoCampioni.Operatore operatore = new RichiestaNuovoInterventoSenzaPrelievoCampioni.Operatore
				{
					IdOperatore = o.IdOperatore,
					DescrizioneOperatore = o.DescrizioneOperatore
				};
				operatoriIntervento.Add(operatore);
			}


			var richiesta = new InterventoManager.RichiestaNuovoInterventoSenzaPrelievoCampioni()
			{
				IdStazione = idStazione,
				IdTipoIntervento = decimal.Parse(TipiInterventoDropDownList.SelectedValue),
				CodiceArgomento = TipiArgomentoDropDownList.SelectedValue,
				DescrizioneArgomento = TipiArgomentoDropDownList.SelectedItem.Text,
				DataIntervento = DateTime.Parse(DataInterventoTextBox.Text.Trim()),
				OraIntervento = string.IsNullOrWhiteSpace(OraInterventoTextBox.Text.Trim()) ? null : OraInterventoTextBox.Text.Trim(),
				IdTipoRichiedente = TipiRichiedenteDropDownList.SelectedIndex == 0 ? (decimal?)null : decimal.Parse(TipiRichiedenteDropDownList.SelectedValue),
				CodiceCampagna = string.IsNullOrWhiteSpace(CodiceCampagnaTextBox.Text.Trim()) ? null : CodiceCampagnaTextBox.Text.Trim(),
				PrelievoCampioni = false,
				AutoreCreazioneIntervento = $"{oper.Nome} {oper.Cognome}",
				OperatoriIntervento = operatoriIntervento,
				OrganizzazioneCreazione = oper.OrganizzazioneAttiva().Codice
			};

			var risultato = InterventoManager.NuovoInterventoSenzaPrelievoCampioni(richiesta, apiAnagrafiche);
			if (risultato.IsSuccess)
			{
				logger.Info($"Creato intervento id:{risultato.Value}");
				Response.Redirect($"ModificaIntervento.aspx?IdIntervento={risultato.Value}", false);
			}
			else
			{
				ModelState.AddModelError("", $"{risultato.Errors[0].Message} - Contattare l'amministratore del sistema.");
			}
		}
	}
}