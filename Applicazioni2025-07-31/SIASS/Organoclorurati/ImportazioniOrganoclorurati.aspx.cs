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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
	public partial class ImportazioniOrganoclorurati : System.Web.UI.Page
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
				logger.Debug($"Elenco stati");
				// Se l'operatore è un gestore ditta, si usa la partita IVA per filtrare le importazioni di sua competenza
				string partitaIvaOperatore = null;
				if (oper.SeGestoreDitta)
					partitaIvaOperatore = oper.OrganizzazioneAttiva().PIVA;
				var elencoStati = OrganocloruratiManager.ConteggioImportazioniPerStato(partitaIvaOperatore);
				if (elencoStati.Count > 0)
				{
					StatiDropDownList.DataSource = elencoStati;
					StatiDropDownList.DataBind();
				}
				else
					ElencoImportazioniPanel.Visible = false;
			}
		}

		protected void NuovaImportazioneButton_Click(object sender, EventArgs e)
		{
			Response.Redirect("NuovaImportazioneOrganoclorurati.aspx");
		}

		protected void VisualizzaButton_Click(object sender, EventArgs e)
		{
			ImportazioniGridView.PageIndex = 0;
			AggiornaElencoImportazioni();
		}

		protected void ImportazioniGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			ImportazioniGridView.PageIndex = e.NewPageIndex;
			AggiornaElencoImportazioni();
		}

		private void AggiornaElencoImportazioni()
		{
			logger.Debug($"AggiornaElencoImportazioni");
			string stato = StatiDropDownList.SelectedValue;
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			// Se l'operatore è un gestore ditta, si usa la partita IVA per filtrare le importazioni di sua competenza
			string partitaIvaOperatore = null;
			if (oper.SeGestoreDitta)
				partitaIvaOperatore = oper.OrganizzazioneAttiva().PIVA;
			ImportazioniGridView.DataSource = OrganocloruratiManager.ElencoImportazioni(stato, partitaIvaOperatore);
			ImportazioniGridView.DataBind();
		}

		protected void ImportazioniGridView_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				HyperLink rapportoHyperLink = (HyperLink)e.Row.FindControl("RapportoHyperLink");
				if (DataBinder.Eval(e.Row.DataItem, "STATO") != null)
				{
					string stato = DataBinder.Eval(e.Row.DataItem, "STATO").ToString();
					if (stato == "In corso")
						rapportoHyperLink.Visible = false;
					else
					{
						decimal idImportazione = (decimal)DataBinder.Eval(e.Row.DataItem, "ID_IMPORTAZIONE");
						rapportoHyperLink.NavigateUrl = $"~/File/ImportazioniOrganoclorurati/Rapporti/{idImportazione}.txt";
					}
				}
			}
		}
	}
}