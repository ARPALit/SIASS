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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
    public partial class ElencoSiti : System.Web.UI.Page
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
                HyperLink elencoSitiHyperLink = (HyperLink)Master.FindControl("ElencoSitiHyperLink");
                elencoSitiHyperLink.CssClass += " active";

                AggiornaElencoProvince();
                AggiornaElencoComuni();
            }
        }

        protected void SitiGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SitiGridView.PageIndex = e.NewPageIndex;
            CercaSito(e.NewPageIndex + 1);
        }

        protected void CercaSito(int numeroPagina)
        {
            logger.Debug($"Ricerca sito");
            List<InfoSitoPerElenco> siti = SitoManager.ElencoSiti(CodiceIdentificativoDescrizioneTextBox.Text,
                ComuniDropDownList.SelectedValue, ProvinceDropDownList.SelectedValue,
                numeroPagina, SitiGridView.PageSize, out int recordTrovati);
            SitiGridView.VirtualItemCount = recordTrovati;
            SitiGridView.DataSource = siti;
            SitiGridView.DataBind();
            GeneraScriptMappa(siti);
        }

        protected void CercaSitoButton_Click(object sender, EventArgs e)
        {
            AggiornaElencoSiti();
        }

        protected void ProvinceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AggiornaElencoComuni();
        }

        private void AggiornaElencoProvince()
        {
            logger.Debug($"AggiornaElencoProvince");
            using (SIASSEntities context = new SIASSEntities())
            {
                var elencoProvince = context.Province.OrderBy(i => i.DenominazioneProvincia);
                ProvinceDropDownList.DataSource = elencoProvince.ToList();
                ProvinceDropDownList.DataBind();
                ProvinceDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));
            }
        }

        private void AggiornaElencoComuni()
        {
            logger.Debug($"AggiornaElencoComuni");
            using (SIASSEntities context = new SIASSEntities())
            {
                IEnumerable<Comune> elencoComuni = context.Comuni;
                if (ProvinceDropDownList.SelectedIndex > 0)
                    elencoComuni = elencoComuni.Where(i => i.CodiceProvincia.Equals(ProvinceDropDownList.SelectedValue));

                ComuniDropDownList.DataSource = elencoComuni.OrderBy(i => i.DenominazioneComune).ToList();
                ComuniDropDownList.DataBind();
                ComuniDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));
            }
        }

        private void AggiornaElencoSiti()
        {
            SitiGridView.PageIndex = 0;
            CercaSito(1);
        }

        protected void RimuoviFiltriButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("ElencoSiti.aspx");
        }

        public string ImpostaVisibilitaMappa()
        {
            if (Page.IsPostBack)
                return "width: 1000px; height: 500px; border: 1px solid #ccc; margin-top: 1em;";
            else
                return "display: none;";
        }

        private void GeneraScriptMappa(List<InfoSitoPerElenco> elencoSiti)
        {
            var script = SitoManager.GeneraScriptMappa(elencoSiti, true);
            ClientScript.RegisterStartupScript(
                Type.GetType("System.String"),
                "Mappa",
                script
                );
        }
    }
}