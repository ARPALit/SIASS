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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SIASS
{
    public partial class ElencoMisurazioniPerGrandezza : System.Web.UI.Page
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

			SogliaSpikeLabel.Text = "";
            ValoreMedioLabel.Text = "";

            if (!Page.IsPostBack)
            {
                if (!decimal.TryParse(Request.QueryString["IdGrandezzaStazione"], out decimal idGrandezzaStazione))
                {
                    logger.Debug($"Parametro IdGrandezzaStazione mancante");
                    Response.Write($"Parametro IdGrandezzaStazione mancante");
                    Response.End();
                    return;
                }

                using (SIASSEntities context = new SIASSEntities())
                {
                    logger.Debug($"GrandezzeStazione IdGrandezzaStazione:{idGrandezzaStazione}");
                    GrandezzaStazione g = context.GrandezzeStazione.Where(i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione).FirstOrDefault();
                    ViewState.Add("IdGrandezzaStazione", g.ID_GRANDEZZA_STAZIONE);

                    InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(g.ID_STAZIONE);
                    HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                    GrandezzaLabel.Text = $"{g.TipoGrandezza.NOME_GRANDEZZA} ({g.TipoUnitaMisura.NOME_UNITA_MISURA})";

                    ElencoGrandezzeHyperLink.NavigateUrl = $"ElencoGrandezze.aspx?IdStazione={g.ID_STAZIONE}";

                    // Date della prima e dell'ultima misurazione

                    MisurazioniManager.DatePrimaUltimaMisurazione2021(
                        g.ID_GRANDEZZA_STAZIONE,
                        out DateTime? dataPrima,
                        out DateTime? dataUltima,
                        out int? conteggio
                        );

                    if (dataPrima != null && dataUltima != null)
                    {
                        DateMisurazioniLabel.Text = $"Sono presenti {conteggio.Value} misurazioni dal {dataPrima.Value.ToString(CostantiGenerali.FORMATO_DATA)} al {dataUltima.Value.ToString(CostantiGenerali.FORMATO_DATA)}.";
                    }
                    else
                        DateMisurazioniLabel.Text = "Non sono presenti misurazioni.";

                    if (conteggio.Value <= 1)
                    {
                        OpzioniRicercaUpdatePanel.Visible = false;
                    }

                    // Imposta la data di fine intervallo a oggi o all'ultimo giorno in cui ci sono dati (se precedente a oggi)
                    DateTime dataFine = DateTime.Now;
                    if (dataUltima != null)
                    {
                        if (dataUltima.Value.Date < DateTime.Now.Date)
                            dataFine = dataUltima.Value;
                    }
                    DataFineTextBox.Text = dataFine.ToString(CostantiGenerali.FORMATO_DATA);


                    // Imposta l'intervallo agli ultimi X giorni (da configurazione)
                    DateTime dataInizio = dataFine.AddDays(-int.Parse(ConfigurationManager.AppSettings["GiorniDataInizioFiltroMisurazioni"]));
                    if (dataPrima.HasValue)
                    {
                        // Se la data iniziale cade prima della prima data per cui ci sono dati viene impostata a quest'ultima
                        if (dataInizio.Date < dataPrima.Value.Date)
                            dataInizio = dataPrima.Value;
                    }

                    // Imposta l'intervallo agli ultimi X giorni (da configurazione)
                    DataInizioTextBox.Text = dataInizio.ToString(CostantiGenerali.FORMATO_DATA);

                    OperazioniSuSelezionatePanel.Visible = false;

                    GraficoPanel.Visible = false;

                    // Disabilita campi e validatori per spike e plateau
                    MoltiplicatoreDeviazioneStandardTextBox.Enabled = false;
                    MoltiplicatoreDeviazioneStandardRangeValidator.Enabled = false;
                    MoltiplicatoreDeviazioneStandardRequiredFieldValidator.Enabled = false;
                    EvidenziaPlateauCheckBox.Checked = false;
                    DimensionePlateauTextBox.Enabled = false;
                    DimensionePlateauRangeValidator.Enabled = false;
                    DimensionePlateauRequiredFieldValidator.Enabled = false;

                    MoltiplicatoreDeviazioneStandardTextBox.Text = "2";
                    DimensionePlateauTextBox.Text = "10";
                }

                // Carica istruzioni
                IstruzioniLabel.Text = System.IO.File.ReadAllText(Server.MapPath(@"IstruzioniMisurazioni.html"));
            }

        }

        protected void AggiornaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                AggiornaElencoMisurazioni();
        }

        protected void SelezionaTutteCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox selezionaTutteCheckBox = (CheckBox)sender;
            foreach (GridViewRow riga in ElencoMisurazioniGridView.Rows)
            {
                if (riga.RowType == DataControlRowType.DataRow)
                {
                    CheckBox selezionaCheckBox = (CheckBox)(riga.Cells[0].FindControl("SelezionaCheckBox"));
                    selezionaCheckBox.Checked = selezionaTutteCheckBox.Checked;
                }
            }
        }

        protected void ElencoMisurazioniGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Imposta riga
                Label statoLabel = (Label)e.Row.FindControl("StatoLabel");
                string validata = DataBinder.Eval(e.Row.DataItem, "Validata").ToString();
                switch (validata)
                {
                    case "0":
                        statoLabel.Text = "Da validare";
                        e.Row.Attributes.Add("style", "background-color: #eee");
                        break;
                    case "1":
                        statoLabel.Text = "Valida";
                        e.Row.Attributes.Add("style", "background-color: #cfc");
                        break;
                    case "2":
                        statoLabel.Text = "Non valida";
                        e.Row.Attributes.Add("style", "background-color: #fcc");
                        break;
                    default:
                        statoLabel.Text = "STATO SCONOSCIUTO";
                        break;
                }

                if (EvidenziaSpikeCheckBox.Checked || EvidenziaPlateauCheckBox.Checked)
                {
                    CheckBox selezionaCheckBox = (CheckBox)e.Row.FindControl("SelezionaCheckBox");
                    bool spike = (bool)DataBinder.Eval(e.Row.DataItem, "Spike");
                    bool plateau = (bool)DataBinder.Eval(e.Row.DataItem, "Plateau");
                    Label spikePlateauLabel = (Label)e.Row.FindControl("SpikePlateauLabel");
                    if (spike || plateau) selezionaCheckBox.Checked = true;
                    if (spike) spikePlateauLabel.Text = "Spike";
                    if (plateau) spikePlateauLabel.Text = "Plateau";
                }
            }
        }

        protected void OkButton_Click(object sender, EventArgs e)
        {
            List<decimal> idMisurazioni = new List<decimal>();

            foreach (GridViewRow riga in ElencoMisurazioniGridView.Rows)
            {
                if (riga.RowType == DataControlRowType.DataRow)
                {
                    // Aggiorna lo stato delle misurazioni selezionate
                    CheckBox selezionaCheckBox = (CheckBox)(riga.Cells[0].FindControl("SelezionaCheckBox"));
                    if (selezionaCheckBox.Checked)
                    {
                        // Aggiorna lo stato della misurazione
                        decimal idMisurazione = decimal.Parse(ElencoMisurazioniGridView.DataKeys[riga.RowIndex].Value.ToString());
                        idMisurazioni.Add(idMisurazione);
                    }
                }
            }

            if (idMisurazioni.Count > 0)
            {
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				MisurazioniManager.AggiornaStatoMisurazioni2021(idMisurazioni, decimal.Parse(NuovoStatoDropDownList.SelectedValue), $"{oper.Nome} {oper.Cognome}");
                AggiornaElencoMisurazioni();
            }

        }

        private void AggiornaElencoMisurazioni()
        {
            if (Page.IsValid)
            {
                AvvisoLabel.Visible = false;

                // Se lo stato non è "tutti" ne imposta il valore
                decimal? statoValidazione = null;
                if (StatoValidazioneDropDownList.SelectedIndex > 0)
                    statoValidazione = decimal.Parse(StatoValidazioneDropDownList.SelectedValue);

                decimal idGrandezzaStazione = (decimal)ViewState["IdGrandezzaStazione"];

                decimal? valoreMinimo = null;
                if (!String.IsNullOrEmpty(ValoreMinimoTextBox.Text.Trim()))
                    valoreMinimo = decimal.Parse(ValoreMinimoTextBox.Text.Trim());
                decimal? valoreMassimo = null;
                if (!String.IsNullOrEmpty(ValoreMassimoTextBox.Text.Trim()))
                    valoreMassimo = decimal.Parse(ValoreMassimoTextBox.Text.Trim());

                List<InfoMisurazionePerValidazione> misurazioni = MisurazioniManager.ElencoMisurazioni2021(
                    idGrandezzaStazione,
                    DateTime.Parse(DataInizioTextBox.Text),
                    DateTime.Parse(DataFineTextBox.Text),
                    statoValidazione,
                    false,
                    valoreMinimo,
                    valoreMassimo,
                    out int conteggio
                    );

                if (EvidenziaSpikeCheckBox.Checked)
                {
                    double moltiplicatoreDeviazioneStandard = double.Parse(MoltiplicatoreDeviazioneStandardTextBox.Text);
                    IndividuaSpike(misurazioni, moltiplicatoreDeviazioneStandard, out double valoreMedio, out _, out double valoreSoglia);
                    SogliaSpikeLabel.Text = $"Soglia spike: {valoreSoglia}";
                    ValoreMedioLabel.Text = $"Valore medio: {valoreMedio}";
                    ElencoMisurazioniGridView.Columns[4].HeaderText = "Spike";
                }

                if (EvidenziaPlateauCheckBox.Checked)
                {
                    int dimensionePlateau = int.Parse(DimensionePlateauTextBox.Text);
                    IndividuaPlateau(misurazioni, dimensionePlateau);
                    ElencoMisurazioniGridView.Columns[4].HeaderText = "Plateau";
                }

                ElencoMisurazioniGridView.DataSource = misurazioni;
                ElencoMisurazioniGridView.DataBind();

                ElencoMisurazioniGridView.Columns[4].Visible = (EvidenziaSpikeCheckBox.Checked || EvidenziaPlateauCheckBox.Checked);

                if (conteggio > int.Parse(ConfigurationManager.AppSettings["NumeroMassimoMisurazioni"]))
                {
                    AvvisoLabel.Text = String.Format(
                        "Sono state trovate {0} misurazioni. Verranno mostrate solo le prime {1}. Indicare criteri più restrittivi.",
                        conteggio,
                        ConfigurationManager.AppSettings["NumeroMassimoMisurazioni"]
                        );
                    AvvisoLabel.Visible = true;
                }
                else
                {
                    if (conteggio > 0)
                    {
                        AvvisoLabel.Text = String.Format(
                            "Sono state trovate {0} misurazioni.",
                            conteggio
                            );
                        AvvisoLabel.Visible = true;
                    }
                }

                if (conteggio > 0)
                    OperazioniSuSelezionatePanel.Visible = true;
                else
                    OperazioniSuSelezionatePanel.Visible = false;

                // Disegna grafico
                GraficoPanel.Visible = false;
                if (VisualizzaGraficoCheckBox.Checked && conteggio > 0)
                {
                    GraficoPanel.Visible = true;
                    if (EvidenziaSpikeCheckBox.Checked)
                    {
                        DisegnaGraficoSpikePlateau(misurazioni, TipoEvidenziazioneGrafico.Spike);
                        return;
                    }

                    if (EvidenziaPlateauCheckBox.Checked)
                    {
                        DisegnaGraficoSpikePlateau(misurazioni, TipoEvidenziazioneGrafico.Plateau);
                        return;
                    }

                    DisegnaGrafico(misurazioni);
                }
            }
        }

        #region Grafico
        private void DisegnaGrafico(List<InfoMisurazionePerValidazione> misurazioni)
        {
            // Caricamento template
            string percorsoFileTemplate = HttpContext.Current.Server.MapPath("~/TemplateGraficoMisura.html");
            // Verifica esistenza file
            if (!File.Exists(percorsoFileTemplate))
            {
                string messaggioErrore = $"File template {percorsoFileTemplate} non trovato";
                logger.Error(messaggioErrore);
                throw new ApplicationException(messaggioErrore);
            }
            // Lettura del file template
            var testoTemplate = System.IO.File.ReadAllText(percorsoFileTemplate);

            // Parametri del grafico
            string valori = string.Join(",", misurazioni.Select(i => i.Valore.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)));
            string etichette = string.Join(",", misurazioni.Select(i => $"'{i.DataMisurazione:dd/MM/yy}'"));
            string coloriValori = string.Join(",", misurazioni.Select(i => "'blue'"));


            // Sostituzione etichette, valori e colori
            testoTemplate = testoTemplate
                .Replace("$ETICHETTE$", etichette)
                .Replace("$VALORI$", valori)
                .Replace("$COLORI_VALORI$", coloriValori);

            // Registrazione script
            String csname1 = "GraficoMisurazioni";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                cs.RegisterStartupScript(cstype, csname1, testoTemplate);
            }
        }

        private void DisegnaGraficoSpikePlateau(List<InfoMisurazionePerValidazione> misurazioni, TipoEvidenziazioneGrafico evidenzia)
        {
            // Caricamento template
            string percorsoFileTemplate = HttpContext.Current.Server.MapPath("~/TemplateGraficoMisura.html");
            // Verifica esistenza file
            if (!File.Exists(percorsoFileTemplate))
            {
                string messaggioErrore = $"File template {percorsoFileTemplate} non trovato";
                logger.Error(messaggioErrore);
                throw new ApplicationException(messaggioErrore);
            }
            // Lettura del file template
            var testoTemplate = System.IO.File.ReadAllText(percorsoFileTemplate);

            // Parametri del grafico
            string valori = string.Join(",", misurazioni.Select(i => i.Valore.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)));
            string etichette = string.Join(",", misurazioni.Select(i => $"'{i.DataMisurazione:dd/MM/yy}'"));
            string coloriValori = null;
            if (evidenzia == TipoEvidenziazioneGrafico.Spike)
                coloriValori = string.Join(",", misurazioni.Select(i => i.Spike ? "'red'" : "'blue'"));
            else
                coloriValori = string.Join(",", misurazioni.Select(i => i.Plateau ? "'red'" : "'blue'"));


            // Sostituzione etichette, valori e colori
            testoTemplate = testoTemplate
                .Replace("$ETICHETTE$", etichette)
                .Replace("$VALORI$", valori)
                .Replace("$COLORI_VALORI$", coloriValori);

            // Registrazione script
            String csname1 = "GraficoMisurazioni";
            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                cs.RegisterStartupScript(cstype, csname1, testoTemplate);
            }
        }

        private enum TipoEvidenziazioneGrafico
        {
            Spike,
            Plateau
        }

        #endregion Grafico

        #region Validazione

        public static void IndividuaSpike(List<InfoMisurazionePerValidazione> misurazioni, double moltiplicatoreDeviazioneStandard,
            out double valoreMedio, out double deviazioneStandard, out double valoreDiSoglia)
        {
            if (misurazioni.Count == 0)
            {
                valoreMedio = 0;
                deviazioneStandard = 0;
                valoreDiSoglia = 0;
                return;
            }
            var valori = misurazioni.Select(i => (double)i.Valore);
            var media = Enumerable.Average(valori);
            valoreMedio = media;

            // Calcolo della deviazione standard
            double somma = valori.Sum(d => Math.Pow(d - media, 2));
            deviazioneStandard = Math.Sqrt((somma) / (valori.Count() - 1));

            // Calcolo della soglia che identifica lo spike
            var soglia = media + moltiplicatoreDeviazioneStandard * deviazioneStandard;
            valoreDiSoglia = soglia;

            misurazioni.Where(m => m.Valore > (decimal)soglia).Select(m => { m.Spike = true; return m; }).ToList();
        }

        public static void IndividuaPlateau(List<InfoMisurazionePerValidazione> misurazioni, int dimensionePlateau)
        {
            if (misurazioni.Count == 0)
                return;

            decimal? valoreCorrente = null;
            var elencoIdPlateauCorrente = new List<decimal>(); // Id delle misurazioni del plateau corrente
            var elencoIdPlateau = new List<decimal>(); // Id di tutte le misurazioni appartenenti a plateau
            foreach (var misurazioneCorrente in misurazioni)
            {
                if (valoreCorrente == null)
                    valoreCorrente = misurazioneCorrente.Valore;
                if (misurazioneCorrente.Valore == valoreCorrente)
                    elencoIdPlateauCorrente.Add(misurazioneCorrente.IdMisurazione);
                else
                {
                    valoreCorrente = misurazioneCorrente.Valore;
                    if (elencoIdPlateauCorrente.Count >= dimensionePlateau)
                        elencoIdPlateau = elencoIdPlateau.Union(elencoIdPlateauCorrente).ToList();
                    elencoIdPlateauCorrente = new List<decimal>() { misurazioneCorrente.IdMisurazione };
                }
            }
            if (elencoIdPlateauCorrente.Count >= dimensionePlateau)
                elencoIdPlateau = elencoIdPlateau.Union(elencoIdPlateauCorrente).ToList();

            // Registrazione delle misurazioni che appartengono a plateau
            misurazioni.Where(m => elencoIdPlateau.Contains(m.IdMisurazione)).Select(m => { m.Plateau = true; return m; }).ToList();
        }


        #endregion Validazione

        protected void EvidenziaSpikeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EvidenziaSpikeCheckBox.Checked)
            {
                MoltiplicatoreDeviazioneStandardTextBox.Enabled = true;
                MoltiplicatoreDeviazioneStandardRangeValidator.Enabled = true;
                MoltiplicatoreDeviazioneStandardRequiredFieldValidator.Enabled = true;
            }
            else
            {
                MoltiplicatoreDeviazioneStandardTextBox.Enabled = false;
                MoltiplicatoreDeviazioneStandardRangeValidator.Enabled = false;
                MoltiplicatoreDeviazioneStandardRequiredFieldValidator.Enabled = false;
            }
            EvidenziaPlateauCheckBox.Checked = false;
            DimensionePlateauTextBox.Enabled = false;
            DimensionePlateauRangeValidator.Enabled = false;
            DimensionePlateauRequiredFieldValidator.Enabled = false;
        }

        protected void EvidenziaPlateauCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EvidenziaPlateauCheckBox.Checked)
            {
                DimensionePlateauTextBox.Enabled = true;
                DimensionePlateauRangeValidator.Enabled = true;
                DimensionePlateauRequiredFieldValidator.Enabled = true;
            }
            else
            {
                DimensionePlateauTextBox.Enabled = false;
                DimensionePlateauRangeValidator.Enabled = false;
                DimensionePlateauRequiredFieldValidator.Enabled = false;
            }
            EvidenziaSpikeCheckBox.Checked = false;
            MoltiplicatoreDeviazioneStandardTextBox.Enabled = false;
            MoltiplicatoreDeviazioneStandardRangeValidator.Enabled = false;
            MoltiplicatoreDeviazioneStandardRequiredFieldValidator.Enabled = false;
        }
    }
}