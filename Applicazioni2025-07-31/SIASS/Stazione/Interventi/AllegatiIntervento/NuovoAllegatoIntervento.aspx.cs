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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using static SIASS.Model.InfoIntervento;

namespace SIASS
{
    public partial class NuovoAllegatoIntervento : System.Web.UI.Page
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
                if (!decimal.TryParse(Request.QueryString["IdIntervento"], out decimal idIntervento))
                {
                    logger.Debug($"Parametro IdIntervento mancante");
                    Response.Write($"Parametro IdIntervento mancante");
                    Response.End();
                    return;
                }

                InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
                HeaderInterventoResponsive1.PopolaCampi(infoIntervento);

				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}

				ViewState.Add("IdIntervento", idIntervento);
            }
        }

        /// <summary>
        /// Ritorna all'elenco costruendo la stringa con idstazione e idintervento
        /// </summary>
        private void RitornaElencoAllegati()
        {
            decimal idIntervento = (decimal)ViewState["IdIntervento"];
            Response.Redirect($"ElencoAllegatiIntervento.aspx?IdIntervento={idIntervento}");
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            var a = new AllegatoIntervento
            {
                IdIntervento = decimal.Parse(ViewState["IdIntervento"].ToString())
            };

            // Carica file

            // Verifica presenza file
            if (!NomeFileAllegatoFileUpload.HasFile)
            {
                ModelState.AddModelError("", "File non specificato");
                return;
            }

            // Verifica dimensione
            int i = NomeFileAllegatoFileUpload.PostedFile.ContentLength;
            // Verifica massimo limite consentito
            if (i > int.Parse(ConfigurationManager.AppSettings["DimensioneMassimaFileDaCaricare"]))
            {
                // Se il file è troppo grande, avvisa l'utente
                ModelState.AddModelError("", "File troppo grande - dimensione massima: " + (int.Parse(ConfigurationManager.AppSettings["DimensioneMassimaFileDaCaricare"]) / 1048576) + " MByte.");
                return;
            }

			string[] estensioniAccettate = ConfigurationManager.AppSettings["EstensioniAccettate"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			string estensioneFileAllegato = Path.GetExtension(NomeFileAllegatoFileUpload.FileName).ToLower();
			if (!estensioniAccettate.Contains(estensioneFileAllegato))
			{
				// Estensione non accettata, avvisa l'utente
				ModelState.AddModelError("", "Tipo di file non accettato - Estensioni utilizzabili: " + ConfigurationManager.AppSettings["EstensioniAccettate"]);
				return;
			}


			// Ottiene il nome del file dal controllo
			a.NomeFileAllegato = NomeFileAllegatoFileUpload.FileName;

            a.DescrizioneAllegato = DescrizioneAllegatoTextBox.Text.Trim();

            // Imposta la data di inserimento a now
            a.DataOraInserimento = DateTime.Now;

            a.ULTIMO_AGGIORNAMENTO = DateTime.Now;
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			a.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

			if (a.NomeFileAllegato.Length > 100)
			{
				ModelState.AddModelError("", "Il nome del file può essere lungo massimo 100 caratteri.");
				return;
			}

			if (!Regex.IsMatch(a.NomeFileAllegato, @"^[0-9a-zA-Z.]{1,100}$", RegexOptions.IgnoreCase))
			{
				ModelState.AddModelError("", "Il nome del file deve essere composto solo da lettere, cifre e punti");
				return;
			}

			using (SIASSEntities context = new SIASSEntities())
            {
                context.AllegatiIntervento.Add(a);
                context.SaveChanges();

                // Carica l'oggetto info per avere i dati di stazione
                InfoAllegatoIntervento infoAllegatoIntervento = AllegatoInterventoManager.CaricaInfoAllegatoIntervento(a.IdAllegatoIntervento);

                // Salva il file creando eventualmente la cartella della stazione e dell'intervento
                string cartellaStazione = "~/File/Allegati/Stazione" + infoAllegatoIntervento.IdStazione;
                if (!Directory.Exists(Server.MapPath(cartellaStazione)))
                    Directory.CreateDirectory(Server.MapPath(cartellaStazione));
                string cartellaIntervento = cartellaStazione + "/Intervento" + infoAllegatoIntervento.IdIntervento;
                if (!Directory.Exists(Server.MapPath(cartellaIntervento)))
                    Directory.CreateDirectory(Server.MapPath(cartellaIntervento));
                NomeFileAllegatoFileUpload.SaveAs(Page.MapPath(cartellaIntervento + "/" + infoAllegatoIntervento.NomeFileAllegato));

				logger.Info(string.Format("Inserito allegato intervento - id:{0} - id intervento:{1} - Operatore:{2}",
                    infoAllegatoIntervento.IdAllegatoIntervento, infoAllegatoIntervento.IdIntervento, $"{oper.Nome} {oper.Cognome}"));

                RitornaElencoAllegati();
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            RitornaElencoAllegati();
        }
    }
}