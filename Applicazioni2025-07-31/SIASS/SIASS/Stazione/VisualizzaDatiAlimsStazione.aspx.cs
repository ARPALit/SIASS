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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
    public partial class VisualizzaDatiAlimsStazione : System.Web.UI.Page
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

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
                if (infoStazione == null)
                {
                    logger.Debug($"Stazione non trovata IdStazione:{idStazione}");
                    Response.Write($"Stazione non trovata IdStazione:{idStazione}");
                    Response.End();
                    return;
                }
                ViewState.Add("CodiceStazione", infoStazione.CodiceIdentificativo);

                HeaderStazioneResponsive1.PopolaCampi(infoStazione);
                InterventiStazioneHyperLink.NavigateUrl = $"Interventi/ElencoInterventi.aspx?IdStazione={infoStazione.IdStazione}";

                logger.Debug($"AnniConDatiPerStazione {infoStazione.CodiceIdentificativo}");
                var anni = ALIMSManager.AnniConDatiPerStazione(infoStazione.CodiceIdentificativo);
                if (anni.Rows.Count == 0)
                {
                    logger.Debug($"Nessun dato");
                    DatiPanel.Visible = false;
                    return;
                }

                // Popola la dropdown degli anni
                AnniDropDownList.DataSource = anni;
                AnniDropDownList.DataBind();
                AnniDropDownList.Items.Insert(0, new ListItem("tutti", string.Empty));
                // Preseleziona il primo anno
                AnniDropDownList.SelectedIndex = 1;

                NessunDatoDisponibileLabel.Visible = false;

                AggiornaDatiAlims();
            }
        }

        protected void AnniDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AggiornaDatiAlims();
        }

        private void AggiornaDatiAlims()
        {
            string codiceStazione = ViewState["CodiceStazione"].ToString();
            logger.Debug($"Carica dati alims stazione {codiceStazione}");
            decimal? anno = null;
            if (AnniDropDownList.SelectedIndex > 0)
                anno = decimal.Parse(AnniDropDownList.SelectedValue);
            DataTable dtParametri = ALIMSManager.ParametriPerStazioneEAnno(codiceStazione, anno);
            DatiAlimsGridView.DataSource = dtParametri;
            DatiAlimsGridView.DataBind();
        }
    }
}