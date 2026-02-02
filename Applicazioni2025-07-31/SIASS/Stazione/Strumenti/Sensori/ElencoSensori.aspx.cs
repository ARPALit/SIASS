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
using System.Web;
using System.Web.UI;

namespace SIASS
{
    public partial class ElencoSensori : System.Web.UI.Page
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
			if (!oper.SeAmministrazione)
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
            {
                if (!decimal.TryParse(Request.QueryString["IdStrumento"], out decimal idStrumento))
                {
                    logger.Debug($"Parametro IdStrumento mancante");
                    Response.Write($"Parametro IdStrumento mancante");
                    Response.End();
                    return;
                }

                InfoStrumento infoStrumento = StrumentoManager.CaricaInfoStrumento(idStrumento);

                if (infoStrumento == null)
                {
                    Response.Write($"Strumento non trovato: IdStrumento={idStrumento}");
                    Response.End();
                    return;
                }

                ViewState.Add("IdStrumento", idStrumento);
                ViewState.Add("IdStazione", infoStrumento.IdStazione);

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoStrumento.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                // Popola dati
                DescrizioneTipoStrumentoHyperLink.Text = HttpUtility.HtmlEncode(infoStrumento.DescrizioneTipoStrumento);
                DescrizioneTipoStrumentoHyperLink.NavigateUrl = $"~/Stazione/Strumenti/VisualizzaStrumento.aspx?IdStrumento={infoStrumento.IdStrumento}";
                NumeroDiSerieLabel.Text = HttpUtility.HtmlEncode(infoStrumento.NumeroDiSerie);

                // Mostra i dati completi dello strumento solo se non è di tipo "lettura sul campo"
                DatiStrumentoNonVisibileInterventoPanel.Visible = !infoStrumento.VisibileIntervento;

                NuovoHyperLink.NavigateUrl = $"NuovoSensore.aspx?IdStrumento={infoStrumento.IdStrumento}";

                var elencoSensoriStrumento = SensoreManager.ElencoSensoriStrumento(infoStrumento.IdStrumento);
                SensoriStrumentoGridView.DataSource = elencoSensoriStrumento;
                SensoriStrumentoGridView.DataBind();
            }
        }
    }
}