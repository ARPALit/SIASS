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
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
	public partial class PacchettiALIMS : System.Web.UI.Page
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
			if (!Page.IsPostBack)
			{
				TipiMatriceDropDownList.DataSource = oper.OrganizzazioneAttiva().Matrici;
				TipiMatriceDropDownList.DataBind();

				TipiSediDropDownList.DataSource = InterventoManager.TipiSedeAccettazione();
				TipiSediDropDownList.DataBind();

				AggiornaElencoArgomenti();

				PacchettiPanel.Visible = false;
			}
		}

		protected void TipiMatriceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			AggiornaElencoArgomenti();
		}

		private void AggiornaElencoPacchetti()
		{
			// Pacchetti
			if (!String.IsNullOrEmpty(TipiMatriceDropDownList.SelectedValue) && !String.IsNullOrEmpty(TipiArgomentoDropDownList.SelectedValue) && !String.IsNullOrEmpty(TipiSediDropDownList.SelectedValue))
			{
				var apiAnagrafiche = Global.ApiAnagrafiche;
				logger.Debug($"Chiamata APiAnagrafiche.Pacchetti - Matrice:{TipiMatriceDropDownList.SelectedValue}-Argomento:{TipiArgomentoDropDownList.SelectedValue}-Sede:{TipiSediDropDownList.SelectedValue}");
				var pacchetti = apiAnagrafiche.Pacchetti(TipiMatriceDropDownList.SelectedValue, TipiArgomentoDropDownList.SelectedValue, TipiSediDropDownList.SelectedValue, null);
				PacchettiDropDownList.DataSource = pacchetti;
				PacchettiDropDownList.DataBind();
				AggiornaElencoAnaliti();
				AggiornaElencoContenitori();
				PacchettiPanel.Visible = true;
			}
			else
			{
				PacchettiDropDownList.DataSource = null;
				PacchettiDropDownList.DataBind();
				PacchettiPanel.Visible = false;
			}
		}

		private void AggiornaElencoArgomenti()
		{
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			var argomenti = oper.OrganizzazioneAttiva().Matrici.Where(i => i.Codice == TipiMatriceDropDownList.SelectedValue).FirstOrDefault().Argomenti;
			TipiArgomentoDropDownList.DataSource = argomenti;
			TipiArgomentoDropDownList.DataBind();
		}

		private void AggiornaElencoAnaliti()
		{
			if (!String.IsNullOrEmpty(TipiMatriceDropDownList.SelectedValue) && !String.IsNullOrEmpty(PacchettiDropDownList.SelectedValue))
			{
				var apiAnagrafiche = Global.ApiAnagrafiche;
				var risultatiRicercaAnaliti = InterventoManager.RicercaAnaliti(apiAnagrafiche, null, TipiMatriceDropDownList.SelectedValue, PacchettiDropDownList.SelectedValue, null);
				AnalitiGridView.DataSource = risultatiRicercaAnaliti;
				AnalitiGridView.DataBind();
			}
			else
			{
				AnalitiGridView.DataSource = null;
				AnalitiGridView.DataBind();
			}
		}

		private void AggiornaElencoContenitori()
		{
			if (!String.IsNullOrEmpty(TipiMatriceDropDownList.SelectedValue) && !String.IsNullOrEmpty(PacchettiDropDownList.SelectedValue))
			{
				var apiAnagrafiche = Global.ApiAnagrafiche;
				var risultatiRicercaContenitori = InterventoManager.RicercaContenitori(apiAnagrafiche, PacchettiDropDownList.SelectedValue);
				ContenitoriGridView.DataSource = risultatiRicercaContenitori.OrderBy(i=>i.Descrizione);
				ContenitoriGridView.DataBind();
			}
			else
			{
				AnalitiGridView.DataSource = null;
				AnalitiGridView.DataBind();
			}
		}

		protected void PacchettiDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			AggiornaElencoAnaliti();
			AggiornaElencoContenitori();
		}

		protected void CercaPacchettiButton_Click(object sender, EventArgs e)
		{
			AggiornaElencoPacchetti();
		}
	}
}