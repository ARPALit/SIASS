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
    public partial class ElencoCaratteristicheInstallazione : System.Web.UI.Page
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
                    logger.Debug($"Parametro IdStazione mancante");
                    Response.Write($"Parametro IdStazione mancante");
                    Response.End();
                    return;
                }

                logger.Debug($"Elenco caratteristiche installazione");
                ElencoCaratteristicheInstallazioneGridView.DataSource = CaratteristicheInstallazioneManager.ElencoCaratteristicheInstallazione(idStazione);
                ElencoCaratteristicheInstallazioneGridView.DataBind();

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);
                NuovoHyperLink.NavigateUrl = $"NuovoCaratteristicheInstallazione.aspx?IdStazione={idStazione}";
            }

        }

        protected void ElencoCaratteristicheInstallazioneGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label tempCavoEsternoInGuainaLabel = (Label)e.Row.FindControl("CavoEsternoInGuainaLabel");
                if (bool.Parse(DataBinder.Eval(e.Row.DataItem, "CavoEsternoInGuaina").ToString()))
                    tempCavoEsternoInGuainaLabel.Text = "Sì";
                else
                    tempCavoEsternoInGuainaLabel.Text = "No";

                Label tempCavoSottotracciaLabel = (Label)e.Row.FindControl("CavoSottotracciaLabel");
                if (bool.Parse(DataBinder.Eval(e.Row.DataItem, "CavoSottotraccia").ToString()))
                    tempCavoSottotracciaLabel.Text = "Sì";
                else
                    tempCavoSottotracciaLabel.Text = "No";

                Label tempProtezioneAreaLabel = (Label)e.Row.FindControl("ProtezioneAreaLabel");
                if (bool.Parse(DataBinder.Eval(e.Row.DataItem, "ProtezioneArea").ToString()))
                    tempProtezioneAreaLabel.Text = "Sì";
                else
                    tempProtezioneAreaLabel.Text = "No";
            }
        }
    }
}