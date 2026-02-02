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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SIASS
{
    public partial class ModificaAllegatoStazione : System.Web.UI.Page
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
                if (!decimal.TryParse(Request.QueryString["IdAllegatoStazione"], out decimal idAllegatoStazione))
                {
                    logger.Debug($"Parametro IdAllegatoStazione mancante");
                    Response.Write($"Parametro IdAllegatoStazione mancante");
                    Response.End();
                    return;
                }

                logger.Debug($"Caricamento IdAllegatoStazione:{idAllegatoStazione}");

                InfoAllegatoStazione infoAllegatoStazione = AllegatoStazioneManager.CaricaInfoAllegatoStazione(idAllegatoStazione);
                if (infoAllegatoStazione == null)
                {
                    logger.Warn($"Allegato non trovato: IdAllegatoStazione={idAllegatoStazione}");
                    Response.Write($"Allegato non trovato: IdAllegatoStazione={idAllegatoStazione}");
                    Response.End();
                    return;
                }

                ViewState.Add("IdAllegatoStazione", infoAllegatoStazione.IdAllegatoStazione);

                logger.Debug($"CaricaInfoStazione {infoAllegatoStazione.IdStazione}");
                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoAllegatoStazione.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);
                NomeFileAllegatoHyperLink.Text = HttpUtility.HtmlEncode(infoAllegatoStazione.NomeFileAllegato);
                NomeFileAllegatoHyperLink.NavigateUrl = $"~/File/Allegati/Stazione{infoAllegatoStazione.IdStazione}/{infoAllegatoStazione.NomeFileAllegato}";
                DescrizioneAllegatoTextBox.Text = infoAllegatoStazione.DescrizioneAllegato;
                DataOraInserimentoLabel.Text = infoAllegatoStazione.DataOraInserimento.ToString("dd/MM/yyyy HH:mm");
            }

        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            decimal idAllegatoStazione = (decimal)ViewState["IdAllegatoStazione"];
            InfoAllegatoStazione infoAllegatoStazione = AllegatoStazioneManager.CaricaInfoAllegatoStazione(idAllegatoStazione);
            Response.Redirect($"ElencoAllegatiStazione.aspx?IdStazione={infoAllegatoStazione.IdStazione}");
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                decimal idAllegatoStazione = (decimal)ViewState["IdAllegatoStazione"];
                logger.Debug($"Salva allegato stazione {idAllegatoStazione}");

                AllegatoStazione a = (from i in context.AllegatiStazione
                                      where i.IdAllegatoStazione == idAllegatoStazione
                                      select i).FirstOrDefault();

                a.DescrizioneAllegato = DescrizioneAllegatoTextBox.Text.Trim();

                a.ULTIMO_AGGIORNAMENTO = DateTime.Now;
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				a.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                context.SaveChanges();
                logger.Info(string.Format("Aggiornato allegato - id:{0} - id stazione:{1} - Operatore:{2}",
                    a.IdAllegatoStazione, a.IdStazione, $"{oper.Nome} {oper.Cognome}"));

                Response.Redirect($"ElencoAllegatiStazione.aspx?IdStazione={a.IdStazione}");
            }

        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idAllegatoStazione = (decimal)ViewState["IdAllegatoStazione"];
            logger.Debug($"Elimina allegato stazione {idAllegatoStazione}");

            using (SIASSEntities context = new SIASSEntities())
            {
                // Carica l'oggetto dal modello (il primo, se non trovata viene restituito il default cioè null)
                AllegatoStazione a = (from i in context.AllegatiStazione
                                      where i.IdAllegatoStazione == idAllegatoStazione
                                      select i).FirstOrDefault();

                if (a == null)
                {
                    logger.Warn($"Allegato non trovato: IdAllegatoStazione={idAllegatoStazione}");
                    Response.Write($"Allegato non trovato: IdAllegatoStazione={idAllegatoStazione}");
                    Response.End();
                    return;
                }

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(a.IdStazione);

                // Elimina il file
                FileInfo file;
                String nomeFile;
                nomeFile = HttpContext.Current.Server.MapPath($"~/File/Allegati/Stazione{a.IdStazione}/{a.NomeFileAllegato}");

                file = new FileInfo(nomeFile);
                if (file.Exists)
                    File.Delete(nomeFile);
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				logger.Info(string.Format("Eliminato allegato - id:{0} - id Stazione:{1} - Operatore:{2}",
                    a.IdAllegatoStazione, a.IdStazione, $"{oper.Nome} {oper.Cognome}"));

                context.AllegatiStazione.Remove(a);
                context.SaveChanges();

                Response.Redirect($"ElencoAllegatiStazione.aspx?IdStazione={infoStazione.IdStazione}");
            }
        }
    }
}