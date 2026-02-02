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
    public partial class ModificaDatiAmministrativi : System.Web.UI.Page
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
                if (!decimal.TryParse(Request.QueryString["IdDatiAmministrativi"], out decimal idDatiAmministrativi))
                {
                    logger.Debug($"Parametro IdDatiAmministrativi mancante");
                    Response.Write($"Parametro IdDatiAmministrativi mancante");
                    Response.End();
                    return;
                }

                InfoDatiAmministrativi infoDatiAmministrativi = DatiAmministrativiManager.CaricaInfoDatiAmministrativi(idDatiAmministrativi);

                if (infoDatiAmministrativi == null)
                {
                    logger.Warn($"InfoDatiAmministrativi non trovato: IdDatiAmministrativi={idDatiAmministrativi}");
                    Response.Write($"InfoDatiAmministrativi non trovato: IdDatiAmministrativi={idDatiAmministrativi}");
                    Response.End();
                    return;
                }

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoDatiAmministrativi.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                ViewState.Add("IdDatiAmministrativi", idDatiAmministrativi);
                ViewState.Add("IdStazione", infoDatiAmministrativi.IdStazione);

                GestoreTextBox.Text = infoDatiAmministrativi.Gestore;
                IndirizzoGestoreTextBox.Text = infoDatiAmministrativi.IndirizzoGestore;
                TelefonoGestoreTextBox.Text = infoDatiAmministrativi.TelefonoGestore;
                PartitaIVAGestoreTextBox.Text = infoDatiAmministrativi.PartitaIVAGestore;
				RiferimentoGestoreTextBox.Text = infoDatiAmministrativi.RiferimentoGestore;


                InizioValiditaTextBox.Text = infoDatiAmministrativi.InizioValidita.ToString("dd/MM/yyyy");
                if (infoDatiAmministrativi.FineValidita.HasValue)
                    FineValiditaTextBox.Text = infoDatiAmministrativi.FineValidita.Value.ToString("dd/MM/yyyy");

            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idDatiAmministrativi = (decimal)ViewState["IdDatiAmministrativi"];
                logger.Debug($"Salva InfoDatiAmministrativi IdDatiAmministrativi={idDatiAmministrativi}");
                using (SIASSEntities context = new SIASSEntities())
                {
                    DatiAmministrativi datiAmministrativi = context.DatiAmministrativi.Where(i => i.IdDatiAmministrativi == idDatiAmministrativi).FirstOrDefault();
                    if (datiAmministrativi == null)
                    {
                        logger.Warn($"InfoDatiAmministrativi non trovato: IdDatiAmministrativi={idDatiAmministrativi}");
                        Response.Write($"InfoDatiAmministrativi non trovato: IdDatiAmministrativi={idDatiAmministrativi}");
                        Response.End();
                        return;
                    }

                    if (String.IsNullOrEmpty(GestoreTextBox.Text.Trim()))
                        datiAmministrativi.Gestore = null;
                    else
                        datiAmministrativi.Gestore = GestoreTextBox.Text.Trim();
                    if (String.IsNullOrEmpty(IndirizzoGestoreTextBox.Text.Trim()))
                        datiAmministrativi.IndirizzoGestore = null;
                    else
                        datiAmministrativi.IndirizzoGestore = IndirizzoGestoreTextBox.Text.Trim();
                    if (String.IsNullOrEmpty(TelefonoGestoreTextBox.Text.Trim()))
                        datiAmministrativi.TelefonoGestore = null;
                    else
                        datiAmministrativi.TelefonoGestore = TelefonoGestoreTextBox.Text.Trim();
					if (String.IsNullOrEmpty(PartitaIVAGestoreTextBox.Text.Trim()))
						datiAmministrativi.PIVA_GESTORE = null;
					else
						datiAmministrativi.PIVA_GESTORE = PartitaIVAGestoreTextBox.Text.Trim();
					if (String.IsNullOrEmpty(RiferimentoGestoreTextBox.Text.Trim()))
                        datiAmministrativi.RiferimentoGestore = null;
                    else
                        datiAmministrativi.RiferimentoGestore = RiferimentoGestoreTextBox.Text.Trim();


                    datiAmministrativi.InizioValidita = DateTime.Parse(InizioValiditaTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(FineValiditaTextBox.Text.Trim()))
                        datiAmministrativi.FineValidita = null;
                    else
                        datiAmministrativi.FineValidita = DateTime.Parse(FineValiditaTextBox.Text.Trim());

                    datiAmministrativi.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					datiAmministrativi.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                    if (Page.IsValid)
                    {
                        context.SaveChanges();
						logger.Info($"Modificato dati amministrativi - id:{datiAmministrativi.IdDatiAmministrativi} - id stazione:{datiAmministrativi.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoDatiAmministrativi.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoDatiAmministrativi.aspx?IdStazione={ViewState["IdStazione"]}");
        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idDatiAmministrativi = (decimal)ViewState["IdDatiAmministrativi"];
            logger.Debug($"Elimina DatiAmministrativi: IdDatiAmministrativi={idDatiAmministrativi}");
            using (SIASSEntities context = new SIASSEntities())
            {
                DatiAmministrativi datiAmministrativi = context.DatiAmministrativi.Where(i => i.IdDatiAmministrativi == idDatiAmministrativi).FirstOrDefault();
                if (datiAmministrativi != null)
                {
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					logger.Info($"Eliminato dati amministrativi - id:{datiAmministrativi.IdDatiAmministrativi} - id stazione:{datiAmministrativi.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                    context.DatiAmministrativi.Remove(datiAmministrativi);
                    context.SaveChanges();
                }
            }
            Response.Redirect($"ElencoDatiAmministrativi.aspx?IdStazione={ViewState["IdStazione"]}");
        }
    }
}