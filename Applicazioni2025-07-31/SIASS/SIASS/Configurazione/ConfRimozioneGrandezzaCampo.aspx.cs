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
    public partial class ConfRimozioneGrandezzaCampo : System.Web.UI.Page
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
			if (!oper.SeAmministrazione)
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
            {
                using (SIASSEntities context = new SIASSEntities())
                {
                    logger.Debug($"Caricamento reti appartenenza");
                    var elencoRetiAppartenenza = context.TipiReteAppartenenza.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
                    TipiReteAppartenenzaDropDownList.DataSource = elencoRetiAppartenenza.ToList();
                    TipiReteAppartenenzaDropDownList.DataBind();
                    TipiReteAppartenenzaDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));

                    logger.Debug($"Caricamento elenco bacini");
                    var elencoBacini = context.Bacini.OrderBy(i => i.DescrizioneBacino);
                    BaciniDropDownList.DataSource = elencoBacini.ToList();
                    BaciniDropDownList.DataBind();
                    BaciniDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

                    logger.Debug($"Caricamento tipi allestimento");
                    var elencoTipiAllestimento = context.TipiAllestimento.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
                    TipiAllestimentoDropDownList.DataSource = elencoTipiAllestimento.ToList();
                    TipiAllestimentoDropDownList.DataBind();
                    TipiAllestimentoDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

                    logger.Debug($"Caricamento tipi grandezza");
                    var elencoGrandezze = context.TipiGrandezza.OrderBy(i => i.ORDINE).ThenBy(i => i.NOME_GRANDEZZA);
                    TipiGrandezzaDropDownList.DataSource = elencoGrandezze.ToList();
                    TipiGrandezzaDropDownList.DataBind();
                    TipiGrandezzaDropDownList.Items.Insert(0, new ListItem("- tutte -", ""));

                    logger.Debug($"Caricamento corpi idrici");
                    var elencoCorpiIdrici = context.CorpiIdrici.OrderBy(i => i.DescrizioneCorpoIdrico);
                    CorpiIdriciDropDownList.DataSource = elencoCorpiIdrici.ToList();
                    CorpiIdriciDropDownList.DataBind();
                    CorpiIdriciDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

                    logger.Debug($"Caricamento tipi stazione");
                    var elencoTipiStazione = context.TipiStazione.OrderBy(i => i.DescrizioneTipoStazione);
                    TipiStazioneDropDownList.DataSource = elencoTipiStazione.ToList();
                    TipiStazioneDropDownList.DataBind();
                    TipiStazioneDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

                    logger.Debug($"Caricamento elenco siti");
                    var elencoSiti = context.Siti.OrderBy(i => i.DESCRIZIONE);
                    SitiDropDownList.DataSource = elencoSiti.ToList();
                    SitiDropDownList.DataBind();
                    SitiDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));

                    logger.Debug($"Caricamento elenco tipi grandezza");
                    ElencoTipiGrandezzaDropDownList.DataSource = ConfigurazioneManager.ElencoGrandezzeStazioneInUso();
                    ElencoTipiGrandezzaDropDownList.DataBind();
                    ElencoTipiGrandezzaDropDownList.Items.Insert(0, string.Empty);

                    logger.Debug($"Caricamento elenco tipi unita misura");
                    ElencoTipiUnitaMisuraDropDownList.DataSource = ConfigurazioneManager.ElencoUnitaMisuraStazioneInUso();
                    ElencoTipiUnitaMisuraDropDownList.DataBind();
                    ElencoTipiUnitaMisuraDropDownList.Items.Insert(0, string.Empty);

                    logger.Debug($"Caricamento elenco gestori");
                    var elencoGestori = context.DatiAmministrativi.Where(i => i.Gestore != null).Select(i => i.Gestore).Distinct().OrderBy(i => i);
                    GestoriDropDownList.DataSource = elencoGestori.ToList();
                    GestoriDropDownList.DataBind();
                    GestoriDropDownList.Items.Insert(0, new ListItem("- tutti -", ""));
                }

                EsclusaMonitoraggioCheckBox.Checked = true;

                AggiornaElencoProvince();
                AggiornaElencoComuni();
                RimuoviButton.Visible = false;

                ConfigurazioneMultiView.SetActiveView(SelezioneGrandezzaView);
            }
        }

        protected void ProvinceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AggiornaElencoComuni();
        }

        private void AggiornaElencoProvince()
        {
            logger.Debug($"Caricamento elenco province");
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
            logger.Debug($"Caricamento elenco comuni");
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

        protected void CercaStazione(int numeroPagina)
        {
            logger.Debug($"Ricerca stazione");
            var parametriRicerca = PopolaParametriRicerca(numeroPagina, StazioniGridView.PageSize);
            List<InfoStazionePerElenco> stazioni = StazioneManager.ElencoStazioni(parametriRicerca, out int recordTrovati);
            StazioniGridView.VirtualItemCount = recordTrovati;
            StazioniGridView.DataSource = stazioni;
            StazioniGridView.DataBind();

            if (stazioni.Any())
            {
                RimuoviButton.Visible = true;
            }
            else
            {
                RimuoviButton.Visible = false;
            }
        }

        protected void StazioniGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StazioniGridView.PageIndex = e.NewPageIndex;
            CercaStazione(e.NewPageIndex + 1);
        }

        protected void CercaStazioneButton_Click(object sender, EventArgs e)
        {
            StazioniGridView.PageIndex = 0;
            CercaStazione(1);
        }

        private StazioneManager.RicercaStazione PopolaParametriRicerca(int numeroPagina, int dimensionePagina)
        {
            logger.Debug($"PopolaParametriRicerca");
            decimal? idBacino = null;
            if (BaciniDropDownList.SelectedValue != string.Empty)
                idBacino = decimal.Parse(BaciniDropDownList.SelectedValue);

            decimal? idCorpoIdrico = null;
            if (CorpiIdriciDropDownList.SelectedValue != string.Empty)
                idCorpoIdrico = decimal.Parse(CorpiIdriciDropDownList.SelectedValue);

            decimal? idTipoStazione = null;
            if (TipiStazioneDropDownList.SelectedValue != string.Empty)
                idTipoStazione = decimal.Parse(TipiStazioneDropDownList.SelectedValue);

            decimal? idSito = null;
            if (SitiDropDownList.SelectedValue != string.Empty)
                idSito = decimal.Parse(SitiDropDownList.SelectedValue);

            return new StazioneManager.RicercaStazione
            {
                NumeroPagina = numeroPagina,
                DimensionePagina = dimensionePagina,
                CodiceIdentificativoDescrizione = CodiceIdentificativoDescrizioneTextBox.Text,
                CodiceComune = ComuniDropDownList.SelectedValue,
                CodiceProvincia = ProvinceDropDownList.SelectedValue,
                ReteAppartenenza = TipiReteAppartenenzaDropDownList.SelectedValue,
                GrandezzaRilevata = TipiGrandezzaDropDownList.SelectedValue,
                IdBacino = idBacino,
                Allestimento = TipiAllestimentoDropDownList.SelectedValue,
                IdCorpoIdrico = idCorpoIdrico,
                IdTipoStazione = idTipoStazione,
                Gestore = GestoriDropDownList.SelectedValue,
                EsclusaMonitoraggio = EsclusaMonitoraggioCheckBox.Checked,
                IdSito = idSito
            };
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
            }
        }

        protected void SelezioneStazioniButton_Click(object sender, EventArgs e)
        {
            if (ElencoTipiGrandezzaDropDownList.SelectedIndex == 0)
                return;
            if (ElencoTipiUnitaMisuraDropDownList.SelectedIndex == 0)
                return;

            ConfigurazioneMultiView.SetActiveView(SelezioneStazioniView);
        }

        protected void RimuoviButton_Click(object sender, EventArgs e)
        {
            ConfermaLabel.Text = $"Rimuovere {ElencoTipiGrandezzaDropDownList.SelectedItem.Text} ({ElencoTipiUnitaMisuraDropDownList.SelectedItem.Text}) dalle {StazioniGridView.Rows.Count} stazioni selezionate?";
            ConfigurazioneMultiView.SetActiveView(ConfermaView);
        }

        protected void ConfermaButton_Click(object sender, EventArgs e)
        {
            logger.Debug($"Rimozione grandezza {ElencoTipiGrandezzaDropDownList.SelectedValue} tipo unita misura {ElencoTipiUnitaMisuraDropDownList.SelectedValue}");
            int numeroStazioniSelezionate = StazioniGridView.Rows.Count;
            int grandezzeStazioniRimosse = 0;
            int grandezzeStazioniNonRimosse = 0;
            for (int i = 0; i < numeroStazioniSelezionate; i++)
            {
                decimal idStazione = (decimal)StazioniGridView.DataKeys[i].Values["IdStazione"];
                if (ConfigurazioneManager.RimuoviGrandezzaStazione(
                    idStazione,
                    ElencoTipiGrandezzaDropDownList.SelectedValue,
                    ElencoTipiUnitaMisuraDropDownList.SelectedValue
                    ))
                {
                    logger.Debug($"IdStazione {idStazione} grandezza rimossa");
                    grandezzeStazioniRimosse++;
                }
                else
                {
                    logger.Debug($"IdStazione {idStazione} grandezza non rimossa");
                    grandezzeStazioniNonRimosse++;
                }
            }
            EsitoLabel.Text = $"Operazione completata.<br />Stazioni elaborate: {numeroStazioniSelezionate}<br />Grandezze non rimosse (usate da misurazioni): {grandezzeStazioniNonRimosse}";
            ConfigurazioneMultiView.SetActiveView(EsitoView);
        }
    }
}
