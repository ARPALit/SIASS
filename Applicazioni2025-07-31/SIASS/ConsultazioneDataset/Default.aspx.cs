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
using System.Web.UI;
using static SIASS.CostruzioneElenchiDataset;

namespace SIASS
{
    public partial class Default : System.Web.UI.Page
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
			if (!(oper.SeAmministrazione || oper.SeGestione || oper.SeConsultazioneDataset || oper.SeVisualizzazione))
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
            {
                string percorsoFile = Server.MapPath($"~/App_Data/IntroduzioneDataset.html");
                if (File.Exists(percorsoFile))
                    ContenutoIntroduzione.InnerHtml = File.ReadAllText(percorsoFile);

                ReteMonitoraggioHyperLink.NavigateUrl = ConfigurationManager.AppSettings["URLDatasetReteMonitoraggio"];
                ReteMonitoraggioHyperLink.Text = ConfigurationManager.AppSettings["URLDatasetReteMonitoraggio"];
                ReteMonitoraggioAnnoLabel.Text = $"{ConfigurationManager.AppSettings["URLDatasetReteMonitoraggio"]}&mese=MM&anno=AAAA&stazione=CODICE_STAZIONE";

                logger.Debug("CostruzioneElenchiDataset");
                var result = CostruzioneElenchiDataset.Run();

                if (result.Failed)
                {
                    logger.Error($"{result.Error.Message} - {result.Error.Details}");
                    Response.Write($"{result.Error.Message} - {result.Error.Details}");
                    Response.End();
                    return;
                }

                // Popola gli elenchi mettendo due elementi per riga
                // Se gli elementi sono dispari, ne aggiunge uno vuoto alla fine

                logger.Debug("Aggiornamento storico");
                if (result.Data.ElencoStoricoMisurazioni.Count % 2 != 0)
                {
                    result.Data.ElencoStoricoMisurazioni.Add(new LinkAURL(null, null));
                }
                StoricoMisurazioniRepeater.DataSource = result.Data.ElencoStoricoMisurazioni;
                StoricoMisurazioniRepeater.DataBind();

                logger.Debug("Aggiornamento anno corrente");
                if (result.Data.ElencoMisurazioniAnnoCorrente.Count % 2 != 0)
                {
                    result.Data.ElencoMisurazioniAnnoCorrente.Add(new LinkAURL(null, null));
                }
                MisurazioniAnnoCorrenteRepeater.DataSource = result.Data.ElencoMisurazioniAnnoCorrente;
                MisurazioniAnnoCorrenteRepeater.DataBind();

            }

        }
    }
}