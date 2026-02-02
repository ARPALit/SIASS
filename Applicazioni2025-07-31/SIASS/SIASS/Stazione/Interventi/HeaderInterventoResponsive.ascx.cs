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

using SIASS.BLL;
using SIASS.Model;
using System;
using System.Web;

namespace SIASS
{
	public partial class HeaderInterventoResponsive : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public void PopolaCampi(InfoIntervento infoIntervento)
		{

			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
			if (oper.SeGestione || oper.SeAmministrazione)
			{
				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);
				if (Session["Dispositivo"].ToString() == "Mobile")
					StazioneHyperLink.NavigateUrl = $"~/Stazione/Interventi/ElencoInterventi.aspx?IdStazione={infoIntervento.IdStazione}";
				else
					StazioneHyperLink.NavigateUrl = $"~/Stazione/VisualizzaStazione.aspx?IdStazione={infoIntervento.IdStazione}";

				string localizzazione = "";
				if (infoStazione.Localizzazione != null)
					localizzazione = $" - {infoStazione.Localizzazione.Localita} {infoStazione.Localizzazione.DenominazioneComune} {infoStazione.Localizzazione.SiglaProvincia}";
				StazioneHyperLink.Text = HttpUtility.HtmlEncode($"{infoStazione.CodiceIdentificativo} - {infoStazione.Descrizione} {localizzazione}");
			}
			else
			{
				NavigazioneGestionePanel.Visible = false;
			}
			ElencoInterventiHyperLink.NavigateUrl = $"~/Stazione/Interventi/ElencoInterventi.aspx?IdStazione={infoIntervento.IdStazione}";

		}
	}
}