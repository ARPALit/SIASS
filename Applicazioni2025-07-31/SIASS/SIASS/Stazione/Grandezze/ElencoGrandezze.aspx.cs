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
    public partial class ElencoGrandezze : System.Web.UI.Page
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
                ViewState.Add("IdStazione", idStazione);

                if (oper.SeAmministrazione)
                {
                    GrandezzeStazionePanel.Visible = true;
                    NuovaGrandezzaHyperLink.NavigateUrl = $"NuovaGrandezza.aspx?IdStazione={idStazione}";
                }
                else
                    GrandezzeStazionePanel.Visible = false;
                AggiornaElencoGrandezzeStazione();

            }
        }

        private void AggiornaElencoGrandezzeStazione()
        {
            decimal idStazione = (decimal)ViewState["IdStazione"];
            logger.Debug($"AggiornaElencoGrandezzeStazione idStazione:{idStazione}");
            var elencoGrandezzeStazione = SensoreManager.ElencoGrandezzeStazione(idStazione);

            // Elenco per configurazione
            GrandezzeStazioneGridView.DataSource = elencoGrandezzeStazione;
            GrandezzeStazioneGridView.DataBind();

            List<GrandezzaConDatiMisurazione> grandezzeConDatiMisurazione = new List<GrandezzaConDatiMisurazione>();
            foreach (var g in elencoGrandezzeStazione)
            {
                var gdm = new GrandezzaConDatiMisurazione
                {
                    ID_GRANDEZZA_STAZIONE = g.ID_GRANDEZZA_STAZIONE,
                    GRANDEZZA = g.GRANDEZZA,
                    UNITA_MISURA = g.UNITA_MISURA,
                    DATA_MISURAZIONE = null,
                    VALIDATA = null
                };
                grandezzeConDatiMisurazione.Add(gdm);
            }
            GrandezzeMisurazioniGridView.Columns[2].Visible = false;
            GrandezzeMisurazioniGridView.Columns[3].Visible = false;
            GrandezzeMisurazioniGridView.DataSource = grandezzeConDatiMisurazione.OrderBy(i => i.GRANDEZZA);
            GrandezzeMisurazioniGridView.DataBind();
        }

        protected void GrandezzeStazioneGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal idGrandezzaStazione = (decimal)GrandezzeStazioneGridView.SelectedValue;
            using (SIASSEntities context = new SIASSEntities())
            {
                if (context.Sensori2021.Where(i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione).Any())
                {
                    CustomValidator validatoreAnnotazioni = new CustomValidator
                    {
                        IsValid = false,
                        ErrorMessage = "Non è possibile eliminare questa grandezza: risultano sensori associati."
                    };
                    Page.Validators.Add(validatoreAnnotazioni);
                }
                else
                {
                    var grandezza = context.GrandezzeStazione.Where(i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione).FirstOrDefault();
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					logger.Info($"Eliminata grandezza - id:{grandezza.GRANDEZZA} - id stazione:{grandezza.ID_STAZIONE} - Operatore:{oper.Nome} {oper.Cognome}");
                    context.GrandezzeStazione.Remove(grandezza);
                    context.SaveChanges();
                    AggiornaElencoGrandezzeStazione();
                }
            }

        }

        protected void MostraDatiMisurazioniButton_Click(object sender, EventArgs e)
        {
            MostraDatiMisurazioniButton.Visible = false;
            MostraDatiMisurazioniLabel.Visible = false;
            decimal idStazione = (decimal)ViewState["IdStazione"];
            var elencoGrandezzeStazione = SensoreManager.ElencoGrandezzeStazione(idStazione);

            using (SIASSEntities context = new SIASSEntities())
            {
                var datiUltimeMisurazioni = context.MV_ULTIMA_MISURAZIONE.Where(i => i.ID_STAZIONE == idStazione).ToList();
                List<GrandezzaConDatiMisurazione> grandezzeConDatiMisurazione = new List<GrandezzaConDatiMisurazione>();
                foreach (var g in elencoGrandezzeStazione)
                {
                    var gdm = new GrandezzaConDatiMisurazione
                    {
                        ID_GRANDEZZA_STAZIONE = g.ID_GRANDEZZA_STAZIONE,
                        GRANDEZZA = g.GRANDEZZA,
                        UNITA_MISURA = g.UNITA_MISURA
                    };
                    var datiMisurazioniGrandezza = datiUltimeMisurazioni.Where(i => i.ID_GRANDEZZA_STAZIONE == g.ID_GRANDEZZA_STAZIONE).FirstOrDefault();
                    if (datiMisurazioniGrandezza != null)
                    {
                        gdm.DATA_MISURAZIONE = datiMisurazioniGrandezza.DATA_MISURAZIONE;
                        gdm.VALIDATA = datiMisurazioniGrandezza.VALIDATA;
                    }
                    grandezzeConDatiMisurazione.Add(gdm);
                }
                GrandezzeMisurazioniGridView.Columns[2].Visible = true;
                GrandezzeMisurazioniGridView.Columns[3].Visible = true;
                GrandezzeMisurazioniGridView.DataSource = grandezzeConDatiMisurazione.OrderBy(i => i.GRANDEZZA);
                GrandezzeMisurazioniGridView.DataBind();
            }
        }

        private class GrandezzaConDatiMisurazione
        {
            public decimal ID_GRANDEZZA_STAZIONE { get; set; }
            public decimal ID_STAZIONE { get; set; }
            public string GRANDEZZA { get; set; }
            public string UNITA_MISURA { get; set; }
            public decimal? VALIDATA { get; set; }
            public DateTime? DATA_MISURAZIONE { get; set; }

        }

        protected void GrandezzeMisurazioniGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Ultima misurazione validata
                Label validataLabel = (Label)e.Row.FindControl("ValidataLabel");
                if (DataBinder.Eval(e.Row.DataItem, "VALIDATA") != null)
                {
                    if ((decimal)DataBinder.Eval(e.Row.DataItem, "VALIDATA") == 0)
                        validataLabel.Text = "Non validata";
                    else
                        validataLabel.Text = "Validata";
                }
                else
                {
                    validataLabel.Visible = false;
                }
            }
        }
    }
}