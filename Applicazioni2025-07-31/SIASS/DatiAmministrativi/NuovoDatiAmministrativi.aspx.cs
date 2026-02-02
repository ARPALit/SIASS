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

namespace SIASS
{
    public partial class NuovoDatiAmministrativi : System.Web.UI.Page
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
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                ViewState.Add("IdStazione", infoStazione.IdStazione);

            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    DatiAmministrativi datiAmministrativi = new DatiAmministrativi
                    {
                        IdStazione = idStazione
                    };

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
                        context.DatiAmministrativi.Add(datiAmministrativi);
                        context.SaveChanges();
						logger.Info($"Inserito dati amministrativi - id:{datiAmministrativi.IdDatiAmministrativi} - id stazione:{datiAmministrativi.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoDatiAmministrativi.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoDatiAmministrativi.aspx?IdStazione={ViewState["IdStazione"]}");
        }
    }
}