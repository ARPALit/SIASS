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
using System.Web.UI.WebControls;

namespace SIASS.Mobile
{
    public partial class Default : System.Web.UI.Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Add("Dispositivo", "Mobile");
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

		}

		protected void CercaButton_Click(object sender, EventArgs e)
        {
            string ricerca = RicercaTextBox.Text.Trim().ToUpper();
            logger.Debug($"Ricerca stazioni criterio:{ricerca}");
            using (SIASSEntities context = new SIASSEntities())
            {
                var elencoStazioni = from s in context.Stazioni
                                     where s.CodiceIdentificativo.ToUpper().StartsWith(ricerca) || s.Descrizione.ToUpper().Contains(ricerca)
                                     orderby s.CodiceIdentificativo, s.Descrizione
                                     select new
                                     {
                                         s.IdStazione,
                                         s.CodiceIdentificativo,
                                         s.Descrizione
                                     };
                ElencoStazioniGridView.DataSource = elencoStazioni.ToList();
                ElencoStazioniGridView.DataBind();
            }
        }

        protected void ElencoStazioniGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink stazioneHyperLink = (HyperLink)e.Row.FindControl("StazioneHyperLink");
                if (DataBinder.Eval(e.Row.DataItem, "CodiceIdentificativo") != null)
                    stazioneHyperLink.Text = DataBinder.Eval(e.Row.DataItem, "CodiceIdentificativo").ToString();
                if (DataBinder.Eval(e.Row.DataItem, "Descrizione") != null)
                    stazioneHyperLink.Text += " " + DataBinder.Eval(e.Row.DataItem, "Descrizione").ToString();
                stazioneHyperLink.Attributes.Add("style", $"display: block;");
            }
        }
    }
}