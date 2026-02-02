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
    public partial class VisualizzaSito : System.Web.UI.Page
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
			if (!(oper.SeGestione || oper.SeAmministrazione))
			{
                OperatoreManager.Logout();
                return;
            }

            if (!Page.IsPostBack)
            {
                if (!decimal.TryParse(Request.QueryString["IdSito"], out decimal idSito))
                {
                    logger.Debug($"Parametro idSito mancante");
                    Response.Write($"Parametro idSito mancante");
                    Response.End();
                    return;
                }

                InfoSito infoSito = SitoManager.CaricaSito(idSito);

                if (infoSito == null)
                {
                    Response.Write($"Sito non trovato: IdSito:{idSito}");
                    Response.End();
                    return;
                }

                logger.Debug($"Visualizza sito: {idSito}");

                CodiceIdentificativoLabel.Text = HttpUtility.HtmlEncode(infoSito.CodiceIdentificativo);
                DescrizioneLabel.Text = HttpUtility.HtmlEncode(infoSito.Descrizione);
                ComuneProvinciaLabel.Text = HttpUtility.HtmlEncode(infoSito.ComuneProvincia);
                IndirizzoLabel.Text = string.IsNullOrEmpty(infoSito.Indirizzo) ? "N.D." : HttpUtility.HtmlEncode(infoSito.Indirizzo);
                CodiceIFFILabel.Text = string.IsNullOrEmpty(infoSito.CodiceIFFI) ? "N.D." : HttpUtility.HtmlEncode(infoSito.CodiceIFFI);
                LatitudineLabel.Text = infoSito.Latitudine.ToString();
                LongitudineLabel.Text = infoSito.Longitudine.ToString();
                LatitudineGaussBoagaLabel.Text = infoSito.LatitudineGaussBoaga.ToString();
                LongitudineGaussBoagaLabel.Text = infoSito.LongitudineGaussBoaga.ToString();
                QuotaPianoCampagnaLabel.Text = !infoSito.QuotaPianoCampagna.HasValue ? "N.D." : infoSito.QuotaPianoCampagna.ToString();
                SuperficieLabel.Text = !infoSito.Superficie.HasValue ? "N.D." : infoSito.Superficie.ToString();

                StazioniGridView.DataSource = infoSito.Stazioni;
                StazioniGridView.DataBind();
            }
        }
    }
}
