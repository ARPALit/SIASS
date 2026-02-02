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

namespace SIASS
{
    public partial class UltimoInterventoUltimaMisurazione : System.Web.UI.Page
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
			if (!(oper.SeAmministrazione || oper.SeGestione || oper.SeVisualizzazione))
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
            {
                using (SIASSEntities context = new SIASSEntities())
                {
                    var elencoProvince = context.Province.OrderBy(i => i.DenominazioneProvincia);
                    ProvinceDropDownList.DataSource = elencoProvince.ToList();
                    ProvinceDropDownList.DataBind();
                    ProvinceDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));

                    var elencoTipiStazione = context.TipiStazione.OrderBy(i => i.DescrizioneTipoStazione);
                    TipiStazioneDropDownList.DataSource = elencoTipiStazione.ToList();
                    TipiStazioneDropDownList.DataBind();
                    TipiStazioneDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

                    TipiReteAppartenenzaDropDownList.DataSource = StazioneManager.TipiReteAppartenenza();
                    TipiReteAppartenenzaDropDownList.DataBind();
                    TipiReteAppartenenzaDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));

                    OrdinamentoDropDownList.DataSource = StazioneManager.CampiOrdinamentoRicercaUltimaMisurazioneEIntervento();
                    OrdinamentoDropDownList.DataBind();
                    OrdinamentoDropDownList.SelectedValue = "Data ultima misurazione (più recente prima)";

                    EsclusaMonitoraggioCheckBox.Checked = true;
                }
            }
        }

        protected void CercaStazioneButton_Click(object sender, EventArgs e)
        {
            logger.Info("Ultimo intervento Ultima misurazione");
            decimal? idTipoStazione = null;
            if (TipiStazioneDropDownList.SelectedIndex > 0)
                idTipoStazione = decimal.Parse(TipiStazioneDropDownList.SelectedValue);
            // Mostra/Nasconde colonne ultima misurazione
            StazioniGridView.Columns[10].Visible = SeUltimaMisurazioneCheckBox.Checked;
            StazioniGridView.Columns[11].Visible = SeUltimaMisurazioneCheckBox.Checked;
            StazioniGridView.Columns[12].Visible = SeUltimaMisurazioneCheckBox.Checked;

            StazioniGridView.DataSource = StazioneManager.UltimaMisurazioneEIntervento(new StazioneManager.RicercaUltimaMisurazioneEIntervento()
            {
                CodiceProvincia = ProvinceDropDownList.SelectedValue,
                IdTipoStazione = idTipoStazione,
                EsclusaMonitoraggio = EsclusaMonitoraggioCheckBox.Checked,
                Ordinamento = OrdinamentoDropDownList.SelectedValue,
                SeUltimaMisurazione = SeUltimaMisurazioneCheckBox.Checked,
                ReteAppartenenza = TipiReteAppartenenzaDropDownList.SelectedValue
            }
            );
            StazioniGridView.DataBind();
        }

        protected void StazioniGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Spunta Esclusa monitoraggio
                Label esclusaMonitoraggioLabel = (Label)e.Row.FindControl("EsclusaMonitoraggioLabel");
                if (DataBinder.Eval(e.Row.DataItem, "EsclusaMonitoraggio") != null)
                    esclusaMonitoraggioLabel.Visible = ((bool)DataBinder.Eval(e.Row.DataItem, "EsclusaMonitoraggio"));

                // Immagini tipo stazione
                string cartellaImmaginiTipiStazione = Utils.ApplicationUrlRoot() + "/img/TipiStazione";
                if (DataBinder.Eval(e.Row.DataItem, "IdTipo") != null)
                {
                    Image immagineTipoStazioneImage = (Image)e.Row.FindControl("ImmagineTipoStazioneImage");
                    immagineTipoStazioneImage.ImageUrl = $"{cartellaImmaginiTipiStazione}/{DataBinder.Eval(e.Row.DataItem, "IdTipo")}mini.png";
                    immagineTipoStazioneImage.ToolTip = $"{DataBinder.Eval(e.Row.DataItem, "Tipo")}";
                }


                // Ultima misurazione validata
                if (SeUltimaMisurazioneCheckBox.Checked)
                {
                    HyperLink visualizzaMisurazioniHyperLink = (HyperLink)e.Row.FindControl("VisualizzaMisurazioniHyperLink");
                    if (DataBinder.Eval(e.Row.DataItem, "UltimaMisurazioneValidata") != null)
                    {
                        if ((decimal)DataBinder.Eval(e.Row.DataItem, "UltimaMisurazioneValidata") == 0)
                            visualizzaMisurazioniHyperLink.Text = "Non validata";
                        else
                            visualizzaMisurazioniHyperLink.Text = "Validata";
                    }
                    else
                    {
                        visualizzaMisurazioniHyperLink.Visible = false;
                    }
                }

				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

				Label tempCodiceIdentificativoLabel = (Label)e.Row.FindControl("CodiceIdentificativoLabel");
                HyperLink visualizzaStazioneHyperLink = (HyperLink)e.Row.FindControl("VisualizzaStazioneHyperLink");

                if (oper.SeAmministrazione || oper.SeGestione)
                {
                    visualizzaStazioneHyperLink.Visible = true;
                    tempCodiceIdentificativoLabel.Visible = false;
                }
                else
                {
                    visualizzaStazioneHyperLink.Visible = false;
                    tempCodiceIdentificativoLabel.Visible = true;
                }
            }
        }

        protected void RimuoviFiltriButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("UltimoInterventoUltimaMisurazione.aspx");
        }

    }
}