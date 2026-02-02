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
using System.Linq;
using System.Web.UI;

namespace SIASS
{
	public partial class SelezioneProfilo : System.Web.UI.Page
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		protected void Page_Load(object sender, EventArgs e)
		{
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

			if (oper == null)
			{
				OperatoreManager.Logout();
				Response.End();
				return;
			}

			if (!Page.IsPostBack)
			{
				try
				{

					// visualizzazione profilo corrente
					if (String.IsNullOrEmpty(oper.ProfiloAttivo()))
					{
						Master.NascondiVoceBarraMenuPaginaPrincipale = true;
						ProfiloAttivoLabel.Text = "Nessun profilo attivo";
					}
					else
					{
						Master.NascondiVoceBarraMenuPaginaPrincipale = false;
						ProfiloAttivoLabel.Text = $"{oper.OrganizzazioneAttiva().RagioneSociale} - {oper.ProfiloAttivo()}";
					}

					// popolamento elenco profili
					ElencoOrganizzazioniDropDownList.DataSource = oper.Organizzazioni;
					ElencoOrganizzazioniDropDownList.DataBind();

					AggiornaElencoProfili();

					// Nasconde le voci della barra menù
					//Master.NascondiVociBarraMenu = true;

				}
				catch (Exception ex)
				{
					logger.Error("SelezioneProfilo Erore: {0}", ex.Message);
				}
			}
		}

		protected void SelezioneProfiloButton_Click(object sender, EventArgs e)
		{
			try
			{
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				oper.ImpostaOrganizzazioneAttiva(ElencoOrganizzazioniDropDownList.SelectedValue);
				oper.ImpostaProfiloAttivo(ElencoProfiliDropDownList.SelectedValue);
				// redirect verso menù principale
				string paginaRedirect = "Stazione/ElencoStazioni.aspx";
				if (!String.IsNullOrEmpty(Session["Dispositivo"] as string) && (Session["Dispositivo"].ToString() == "Mobile"))
					paginaRedirect = "Mobile/Default.aspx";
				Response.Redirect(paginaRedirect, true);
			}
			catch (System.Threading.ThreadAbortException)
			{
				// Eccezione generata dalla redirect
			}
			catch (Exception ex)
			{
				logger.Error("SelezioneProfilo Erore: {0}", ex.Message);
			}

		}

		protected void ElencoOrganizzazioniDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			AggiornaElencoProfili();
		}
		private void AggiornaElencoProfili()
		{
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			ElencoProfiliDropDownList.DataSource = oper.Organizzazioni
				.Where(i => i.Codice.Equals(ElencoOrganizzazioniDropDownList.SelectedValue))
				.FirstOrDefault()
				.Profili;
			ElencoProfiliDropDownList.DataBind();
		}
	}
}