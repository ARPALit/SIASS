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
using System.Web;
using System.Web.UI;

namespace SIASS
{
    public partial class VisualizzaStrumento : System.Web.UI.Page
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
                    logger.Warn($"Strumento non trovato: IdStrumento={idStrumento}");
                    Response.Write($"Strumento non trovato: IdStrumento={idStrumento}");
                    Response.End();
                    return;
                }

                ViewState.Add("IdStrumento", idStrumento);
                ViewState.Add("IdStazione", infoStrumento.IdStazione);

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoStrumento.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                // Popola dati
                DescrizioneTipoStrumentoLabel.Text = HttpUtility.HtmlEncode(infoStrumento.DescrizioneTipoStrumento);
                NumeroDiSerieLabel.Text = HttpUtility.HtmlEncode(infoStrumento.NumeroDiSerie);
                MarcaLabel.Text = HttpUtility.HtmlEncode(infoStrumento.Marca);
                ModelloLabel.Text = HttpUtility.HtmlEncode(infoStrumento.Modello);
                NumeroInventarioArpalLabel.Text = HttpUtility.HtmlEncode(infoStrumento.NumeroInventarioArpal);
                CaratteristicheLabel.Text = HttpUtility.HtmlEncode(infoStrumento.Caratteristiche);
                CodiceSistemaGestionaleLabel.Text = HttpUtility.HtmlEncode(infoStrumento.CodiceSistemaGestionale);
                InizioValiditaLabel.Text = infoStrumento.InizioValidita.ToString("dd/MM/yyyy");
                if (infoStrumento.FineValidita.HasValue)
                    FineValiditaLabel.Text = infoStrumento.FineValidita.Value.ToString("dd/MM/yyyy");

                // Mostra i dati completi dello strumento solo se non è di tipo "lettura sul campo"
                DatiStrumentoNonVisibileInterventoPanel.Visible = !infoStrumento.VisibileIntervento;

                PacchettiStrumentoGridView.DataSource = StrumentoManager.ElencoPacchettiStrumento(infoStrumento.IdStrumento);
                PacchettiStrumentoGridView.DataBind();

                var elencoSensoriStrumento = SensoreManager.ElencoSensoriStrumento(infoStrumento.IdStrumento);

                SensoriStrumentoGridView.DataSource = elencoSensoriStrumento;
                SensoriStrumentoGridView.DataBind();

                ElencoStrumentiHyperLink.NavigateUrl = $"ElencoStrumenti.aspx?IdStazione={infoStrumento.IdStazione}";

                ModificaStrumentoButton.Visible = oper.SeAmministrazione && !infoStrumento.VisibileIntervento;
                ModificaSensoriButton.Visible = oper.SeAmministrazione;
                RiassegnaSensoriButton.Visible = oper.SeAmministrazione && elencoSensoriStrumento.Count > 0;
                ModificaPacchettiButton.Visible = oper.SeAmministrazione;
                EliminaConfermaButton.Visible = oper.SeAmministrazione && !elencoSensoriStrumento.Any();
            }
        }

        protected void ModificaStrumentoButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ModificaStrumento.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }
        protected void ModificaPacchettiButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"Pacchetti/ElencoPacchetti.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idStrumento = (decimal)ViewState["IdStrumento"];
            using (SIASSEntities context = new SIASSEntities())
            {
                StrumentoStazione s = context.StrumentiStazione.Where(i => i.ID_STRUMENTO_STAZIONE == idStrumento).FirstOrDefault();
                context.StrumentiStazione.Remove(s);
                context.SaveChanges();
            }
            logger.Info($"Eliminato StrumentoStazione IdStrumento:{idStrumento}");
            Response.Redirect($"ElencoStrumenti.aspx?IdStazione={ViewState["IdStazione"]}");
        }

        protected void ModificaSensoriButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"Sensori/ElencoSensori.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }

        protected void RiassegnaSensoriButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"Sensori/RiassegnaSensoriStrumento.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }
    }
}