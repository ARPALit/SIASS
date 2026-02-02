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
using static SIASS.Model.InfoIntervento;

namespace SIASS
{
    public partial class ModificaAllegatoIntervento : System.Web.UI.Page
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
			if (!(oper.SeAmministrazione || oper.SeGestione || oper.SeGestoreDitta))
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
            {
                if (!decimal.TryParse(Request.QueryString["IdAllegatoIntervento"], out decimal idAllegatoIntervento))
                {
                    logger.Debug($"Parametro IdAllegatoIntervento mancante");
                    Response.Write($"Parametro IdAllegatoIntervento mancante");
                    Response.End();
                    return;
                }

                InfoAllegatoIntervento infoAllegatoIntervento = AllegatoInterventoManager.CaricaInfoAllegatoIntervento(idAllegatoIntervento);
                if (infoAllegatoIntervento == null)
                {
                    Response.Write($"Allegato non trovato: IdAllegatoIntervento={idAllegatoIntervento}");
                    Response.End();
                    return;
                }
                ViewState.Add("IdAllegatoIntervento", infoAllegatoIntervento.IdAllegatoIntervento);

                InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(infoAllegatoIntervento.IdIntervento);

				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}


				HeaderInterventoResponsive1.PopolaCampi(infoIntervento);
                NomeFileAllegatoHyperLink.Text = infoAllegatoIntervento.NomeFileAllegato;
                NomeFileAllegatoHyperLink.NavigateUrl = $"~/File/Allegati/Stazione{infoAllegatoIntervento.IdStazione}/Intervento{infoAllegatoIntervento.IdIntervento}/{infoAllegatoIntervento.NomeFileAllegato}";
                DescrizioneAllegatoTextBox.Text = infoAllegatoIntervento.DescrizioneAllegato;
                DataOraInserimentoLabel.Text = infoAllegatoIntervento.DataOraInserimento.ToString("dd/MM/yyyy HH:mm");
            }

        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            decimal idAllegatoIntervento = (decimal)ViewState["IdAllegatoIntervento"];
            InfoAllegatoIntervento infoAllegatoIntervento = AllegatoInterventoManager.CaricaInfoAllegatoIntervento(idAllegatoIntervento);
            Response.Redirect($"ElencoAllegatiIntervento.aspx?IdIntervento={infoAllegatoIntervento.IdIntervento}");
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                decimal idAllegatoIntervento = (decimal)ViewState["IdAllegatoIntervento"];

                AllegatoIntervento a = (from i in context.AllegatiIntervento
                                        where i.IdAllegatoIntervento == idAllegatoIntervento
                                        select i).FirstOrDefault();

                a.DescrizioneAllegato = DescrizioneAllegatoTextBox.Text.Trim();

                a.ULTIMO_AGGIORNAMENTO = DateTime.Now;
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				a.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                context.SaveChanges();
				logger.Info(string.Format("Aggiornato allegato - id:{0} - id Intervento:{1} - Operatore:{2}",
                    a.IdAllegatoIntervento, a.IdIntervento, $"{oper.Nome} {oper.Cognome}"));

                Response.Redirect($"ElencoAllegatiIntervento.aspx?IdIntervento={a.IdIntervento}");
            }

        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idAllegatoIntervento = (decimal)ViewState["IdAllegatoIntervento"];

            using (SIASSEntities context = new SIASSEntities())
            {
                // Carica l'oggetto dal modello (il primo, se non trovata viene restituito il default cioè null)
                AllegatoIntervento a = (from i in context.AllegatiIntervento
                                        where i.IdAllegatoIntervento == idAllegatoIntervento
                                        select i).FirstOrDefault();

                if (a == null)
                {
                    Response.Write($"Allegato non trovato: IdAllegatoIntervento={idAllegatoIntervento}");
                    Response.End();
                    return;
                }

                InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(a.IdIntervento);

                // Elimina il file
                FileInfo file;
                String nomeFile;
                nomeFile = HttpContext.Current.Server.MapPath($"~/File/Allegati/Stazione{infoIntervento.IdStazione}/Intervento{infoIntervento.IdIntervento}/{a.NomeFileAllegato}");

                file = new FileInfo(nomeFile);
                if (file.Exists)
                    File.Delete(nomeFile);

				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				logger.Info(string.Format("Eliminato allegato - id:{0} - id Intervento:{1} - Operatore:{2}",
                    a.IdAllegatoIntervento, a.IdIntervento, $"{oper.Nome} {oper.Cognome}"));

                context.AllegatiIntervento.Remove(a);
                context.SaveChanges();

                Response.Redirect($"ElencoAllegatiIntervento.aspx?IdIntervento={infoIntervento.IdIntervento}");
            }
        }
    }
}