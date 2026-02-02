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
    public partial class ModificaStrumento : System.Web.UI.Page
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
                using (SIASSEntities context = new SIASSEntities())
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

                    InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoStrumento.IdStazione);
                    HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                    ViewState.Add("IdStrumento", infoStrumento.IdStrumento);
                    ViewState.Add("IdStazione", infoStrumento.IdStazione);

                    DescrizioneTipoStrumentoLabel.Text = HttpUtility.HtmlEncode(infoStrumento.DescrizioneTipoStrumento);

                    if (infoStrumento.VisibileIntervento)
                    {
                        DatiStrumentoNonVisibileInterventoPanel.Visible = false;
                    }
                    else
                    {
                        NumeroDiSerieTextBox.Text = infoStrumento.NumeroDiSerie;
                        MarcaTextBox.Text = infoStrumento.Marca;
                        ModelloTextBox.Text = infoStrumento.Modello;
                        NumeroInventarioArpalTextBox.Text = infoStrumento.NumeroInventarioArpal;
                        CaratteristicheTextBox.Text = infoStrumento.Caratteristiche;
                        CodiceSistemaGestionaleTextBox.Text = infoStrumento.CodiceSistemaGestionale;

                        InizioValiditaTextBox.Text = infoStrumento.InizioValidita.ToString("dd/MM/yyyy");
                        if (infoStrumento.FineValidita.HasValue)
                            FineValiditaTextBox.Text = infoStrumento.FineValidita.Value.ToString("dd/MM/yyyy");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"VisualizzaStrumento.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idStrumento = (decimal)ViewState["IdStrumento"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    StrumentoStazione s = context.StrumentiStazione.Where(i => i.ID_STRUMENTO_STAZIONE == idStrumento).FirstOrDefault();

                    if (!s.Tipo.VISIBILE_INTERVENTO)
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

                    context.SaveChanges();
                    logger.Info($"Aggiornato strumento idStrumento:{idStrumento}");
                    Response.Redirect($"VisualizzaStrumento.aspx?IdStrumento={ViewState["IdStrumento"]}");
                }
            }
        }

    }
}
