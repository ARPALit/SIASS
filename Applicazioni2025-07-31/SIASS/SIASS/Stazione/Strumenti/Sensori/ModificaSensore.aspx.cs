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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
    public partial class ModificaSensore : System.Web.UI.Page
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
                string codiceIdentificativo = Request.QueryString["CodiceIdentificativo"];

                InfoSensore infoSensore = SensoreManager.CaricaInfoSensore(codiceIdentificativo);

                if (infoSensore == null)
                {
                    logger.Warn($"Sensore non trovato: CodiceIdentificativo={codiceIdentificativo}");
                    Response.Write($"Sensore non trovato: CodiceIdentificativo={codiceIdentificativo}");
                    Response.End();
                    return;
                }

                ViewState.Add("CodiceIdentificativo", codiceIdentificativo);
                ViewState.Add("IdStrumento", infoSensore.IdStrumentoStazione);

                InfoStrumento infoStrumento = StrumentoManager.CaricaInfoStrumento(infoSensore.IdStrumentoStazione);

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoStrumento.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                // Popola dati
                DescrizioneTipoStrumentoHyperLink.Text = HttpUtility.HtmlEncode(infoStrumento.DescrizioneTipoStrumento);
                DescrizioneTipoStrumentoHyperLink.NavigateUrl = $"~/Stazione/Strumenti/VisualizzaStrumento.aspx?IdStrumento={infoStrumento.IdStrumento}";
                NumeroDiSerieLabel.Text = HttpUtility.HtmlEncode(infoStrumento.NumeroDiSerie);

                // Mostra i dati completi dello strumento solo se non è di tipo "lettura sul campo"
                DatiStrumentoNonVisibileInterventoPanel.Visible = !infoStrumento.VisibileIntervento;

                List<GrandezzaStazione> elencoGrandezzeStazione = SensoreManager.ElencoGrandezzeStazione(infoStrumento.IdStazione);
                var grandezzeStazione = elencoGrandezzeStazione.Select(
                    g => new
                    {
                        IdGrandezzaStazione = g.ID_GRANDEZZA_STAZIONE,
                        GrandezzaEUnitaMisura = $"{g.GRANDEZZA} ({g.UNITA_MISURA})"
                    }
                    )
                    .OrderBy(i => i.GrandezzaEUnitaMisura);
                GrandezzeStazioneDropDownList.DataSource = grandezzeStazione;
                GrandezzeStazioneDropDownList.DataBind();

                List<TipoEspressioneRisultato> tipiEspressioneRisultato = SensoreManager.TipiEspresssioneRisultato();
                TipiEspressioneRisultatoDropDownList.DataSource = tipiEspressioneRisultato;
                TipiEspressioneRisultatoDropDownList.DataBind();
                TipiEspressioneRisultatoDropDownList.Items.Insert(0, new ListItem(string.Empty));

                List<TipoMetodo> tipiMetodo = SensoreManager.TipiMetodo();
                TipiMetodoDropDownList.DataSource = tipiMetodo;
                TipiMetodoDropDownList.DataBind();
                TipiMetodoDropDownList.Items.Insert(0, new ListItem(string.Empty));

                List<TipoUnitaMisura2021> tipiUnitaMisura = SensoreManager.TipiUnitaMisura();
                TipiUnitaMisuraDropDownList.DataSource = tipiUnitaMisura;
                TipiUnitaMisuraDropDownList.DataBind();
                TipiUnitaMisuraDropDownList.Items.Insert(0, new ListItem(string.Empty));

                // Popola dati sensore
                CodiceIdentificativoLabel.Text = HttpUtility.HtmlEncode(infoSensore.CodiceIdentificativo);
                GrandezzeStazioneDropDownList.SelectedValue = infoSensore.IdGrandezza.ToString();
                CodicePMCTextBox.Text = infoSensore.CodicePMC;
                if (!String.IsNullOrEmpty(infoSensore.UnitaMisuraSensore))
                    TipiUnitaMisuraDropDownList.SelectedValue = infoSensore.UnitaMisuraSensore;
                if (infoSensore.IdTipoEspressioneRisultato.HasValue)
                    TipiEspressioneRisultatoDropDownList.SelectedValue = infoSensore.IdTipoEspressioneRisultato.ToString();
                FrequenzaAcquisizioneTextBox.Text = infoSensore.FrequenzaAcquisizione;
                if (infoSensore.IdTipoMetodo.HasValue)
                    TipiMetodoDropDownList.SelectedValue = infoSensore.IdTipoMetodo.ToString();
                if (infoSensore.CoefficienteConversioneUnitaMisura.HasValue)
                    CoefficienteConversioneUnitaMisuraTextBox.Text = infoSensore.CoefficienteConversioneUnitaMisura.ToString();

                // Mostra i dati completi del sensore solo se lo strumento non è di tipo "lettura sul campo"
                DatiSensoreNonVisibileInterventoPanel.Visible = !infoStrumento.VisibileIntervento;
            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (TipiUnitaMisuraDropDownList.SelectedIndex == 0 ^ String.IsNullOrEmpty(CoefficienteConversioneUnitaMisuraTextBox.Text.Trim()))
            {
                CustomValidator validatore = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Se si indica l'unità di misura va indicato anche il Coefficiente conversione e viceversa."
                };
                Page.Validators.Add(validatore);
                return;
            }

            if (Page.IsValid)
            {
                string codiceIdentificativo = (string)ViewState["CodiceIdentificativo"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    Sensore2021 sensore = context.Sensori2021.Where(i => i.CODICE_IDENTIFICATIVO.ToUpper() == codiceIdentificativo.ToUpper()).FirstOrDefault();

                    sensore.ID_GRANDEZZA_STAZIONE = decimal.Parse(GrandezzeStazioneDropDownList.SelectedValue);
                    if (String.IsNullOrEmpty(CodicePMCTextBox.Text.Trim()))
                        sensore.CODICE_PMC = null;
                    else
                        sensore.CODICE_PMC = CodicePMCTextBox.Text.Trim();
                    if (TipiEspressioneRisultatoDropDownList.SelectedIndex == 0)
                        sensore.ID_TIPO_ESPRESS_RISULTATO = null;
                    else
                        sensore.ID_TIPO_ESPRESS_RISULTATO = decimal.Parse(TipiEspressioneRisultatoDropDownList.SelectedValue);
                    if (TipiUnitaMisuraDropDownList.SelectedIndex == 0)
                        sensore.UNITA_MISURA = null;
                    else
                        sensore.UNITA_MISURA = TipiUnitaMisuraDropDownList.SelectedValue;
                    if (String.IsNullOrEmpty(FrequenzaAcquisizioneTextBox.Text.Trim()))
                        sensore.FREQUENZA_ACQUISIZIONE = null;
                    else
                        sensore.FREQUENZA_ACQUISIZIONE = FrequenzaAcquisizioneTextBox.Text.Trim();
                    if (TipiMetodoDropDownList.SelectedIndex == 0)
                        sensore.ID_TIPO_METODO = null;
                    else
                        sensore.ID_TIPO_METODO = decimal.Parse(TipiMetodoDropDownList.SelectedValue);
                    if (String.IsNullOrEmpty(CoefficienteConversioneUnitaMisuraTextBox.Text.Trim()))
                        sensore.COEFF_CONVER_UNITA_MISURA = null;
                    else
                        sensore.COEFF_CONVER_UNITA_MISURA = decimal.Parse(CoefficienteConversioneUnitaMisuraTextBox.Text.Trim());

                    sensore.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					sensore.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                    context.SaveChanges();
                    logger.Info($"Aggiornato sensore codiceIdentificativo:{codiceIdentificativo}");
                    Response.Redirect($"ElencoSensori.aspx?IdStrumento={ViewState["IdStrumento"]}");
                }
            }

        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoSensori.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            string codiceIdentificativo = (string)ViewState["CodiceIdentificativo"];
            using (SIASSEntities context = new SIASSEntities())
            {
                if (context.Misurazioni2021.Any(i => i.CODICE_IDENTIFICATIVO_SENSORE == codiceIdentificativo))
                {
                    CustomValidator validatore = new CustomValidator
                    {
                        IsValid = false,
                        ErrorMessage = "Non è possibile eliminare questo sensore: esistono misurazioni associate."
                    };
                    Page.Validators.Add(validatore);
                    return;
                }

                Sensore2021 sensore = context.Sensori2021.FirstOrDefault(i => i.CODICE_IDENTIFICATIVO == codiceIdentificativo);
                context.Sensori2021.Remove(sensore);
                context.SaveChanges();
            }
            Response.Redirect($"ElencoSensori.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }

    }
}