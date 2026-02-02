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
    public partial class NuovoStrumento : System.Web.UI.Page
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
                if (!decimal.TryParse(Request.QueryString["IdStazione"], out decimal idStazione))
                {
                    logger.Debug($"Parametro IdStazione mancante");
                    Response.Write($"Parametro IdStazione mancante");
                    Response.End();
                    return;
                }
                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);
                ViewState.Add("IdStazione", idStazione);

                // Popola dropdown tipi strumento.
                // Se per la stazione è già stato creato quello di lettura sul campo (visibile intervento) non viene incluso
                var tipiStrumento = StrumentoManager.TipiStrumento();

                // Mette in sessone l'id tipo strumento visibile intervento per utilzzarlo quando deve mostrare o nascondere campi
                decimal idTipoStrumentoVisibileIntervento = tipiStrumento.Where(i => i.VISIBILE_INTERVENTO == true).FirstOrDefault().ID_TIPO_STRUMENTO;
                ViewState.Add("IdTipoStrumentoVisibileIntervento", idTipoStrumentoVisibileIntervento);

                bool esisteStrumentoVisibileIntervento = StrumentoManager.ElencoStrumenti(idStazione).Any(i => i.VisibileIntervento == true);
                if (esisteStrumentoVisibileIntervento)
                    TipiStrumentoDropDownList.DataSource = tipiStrumento.Where(i => i.VISIBILE_INTERVENTO == false);
                else
                    TipiStrumentoDropDownList.DataSource = tipiStrumento;
                TipiStrumentoDropDownList.DataBind();
            }

        }

        /// <summary>
        /// Mostra/nasconde campi in base al tipo di strumento
        /// </summary>
        private void MostraNascondeCampiStrumento()
        {
            decimal idTipoStrumentoVisibileIntervento = (decimal)ViewState["IdTipoStrumentoVisibileIntervento"];
            if (decimal.Parse(TipiStrumentoDropDownList.SelectedValue) == idTipoStrumentoVisibileIntervento)
                DatiStrumentoNonVisibileInterventoPanel.Visible = false;
            else
                DatiStrumentoNonVisibileInterventoPanel.Visible = true;
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    StrumentoStazione s = new StrumentoStazione
                    {
                        ID_STAZIONE = idStazione,
                        ID_TIPO_STRUMENTO = decimal.Parse(TipiStrumentoDropDownList.SelectedValue)
                    };
                    decimal idTipoStrumentoVisibileIntervento = (decimal)ViewState["IdTipoStrumentoVisibileIntervento"];
                    if (decimal.Parse(TipiStrumentoDropDownList.SelectedValue) == idTipoStrumentoVisibileIntervento)
                    {
                        s.INIZIO_VALIDITA = DateTime.Now;
                    }
                    else
                    {
                        s.NUMERO_DI_SERIE = String.IsNullOrEmpty(NumeroDiSerieTextBox.Text.Trim()) ? null : NumeroDiSerieTextBox.Text.Trim();
                        s.MARCA = String.IsNullOrEmpty(MarcaTextBox.Text.Trim()) ? null : MarcaTextBox.Text.Trim();
                        s.MODELLO = String.IsNullOrEmpty(ModelloTextBox.Text.Trim()) ? null : ModelloTextBox.Text.Trim();
                        s.NUMERO_INVENTARIO_ARPAL = String.IsNullOrEmpty(NumeroInventarioArpalTextBox.Text.Trim()) ? null : NumeroInventarioArpalTextBox.Text.Trim();
                        s.CARATTERISTICHE = String.IsNullOrEmpty(CaratteristicheTextBox.Text.Trim()) ? null : CaratteristicheTextBox.Text.Trim();
                        s.CODICE_SISTEMA_GESTIONALE = String.IsNullOrEmpty(CodiceSistemaGestionaleTextBox.Text.Trim()) ? null : CodiceSistemaGestionaleTextBox.Text.Trim();
                        s.INIZIO_VALIDITA = DateTime.Parse(InizioValiditaTextBox.Text.Trim());
                        if (String.IsNullOrEmpty(FineValiditaTextBox.Text.Trim()))
                            s.FINE_VALIDITA = null;
                        else
                            s.FINE_VALIDITA = DateTime.Parse(FineValiditaTextBox.Text.Trim());
                    }

                    s.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					s.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                    context.StrumentiStazione.Add(s);
                    context.SaveChanges();
                    logger.Info($"Inserito nuovo strumento per idStazione:{idStazione}");
                    Response.Redirect($"ElencoStrumenti.aspx?IdStazione={idStazione}");
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoStrumenti.aspx?IdStazione={ViewState["IdStazione"]}");
        }

        protected void TipiStrumentoDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostraNascondeCampiStrumento();
        }
    }
}