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
    public partial class ElencoAllegatiIntervento : System.Web.UI.Page
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


                ElencoAllegatiInterventoGridView.DataSource = AllegatoInterventoManager.ElencoAllegatiIntervento(idIntervento);
                ElencoAllegatiInterventoGridView.DataBind();

                InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
                HeaderInterventoResponsive1.PopolaCampi(infoIntervento);

				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}

				NuovoHyperLink.NavigateUrl = "NuovoAllegatoIntervento.aspx?IdIntervento=" + idIntervento.ToString();
            }
        }

    }
}