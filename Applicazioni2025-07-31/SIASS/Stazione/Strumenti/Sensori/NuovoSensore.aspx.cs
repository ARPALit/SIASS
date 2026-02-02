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
    public partial class NuovoSensore : System.Web.UI.Page
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
                    if (!decimal.TryParse(Request.QueryString["IdStrumento"], out decimal idStrumento))
                    {
                        logger.Debug($"Parametro IdStrumento mancante");
                        Response.Write($"Parametro IdStrumento mancante");
                        Response.End();
                        return;
                    }

                    InfoStrumento infoStrumento = StrumentoManager.CaricaInfoStrumento(idStrumento);

                    if (infoStrumento == null)
                    {
                        logger.Warn($"Strumento non trovato: IdStrumento={idStrumento}");
                        Response.Write($"Strumento non trovato: IdStrumento={idStrumento}");
                        Response.End();
                        return;
                    }

                    ViewState.Add("IdStrumento", infoStrumento.IdStrumento);

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

                    // Mostra i dati completi del sensore solo se lo strumento non è di tipo "lettura sul campo"
                    DatiSensoreNonVisibileInterventoPanel.Visible = !infoStrumento.VisibileIntervento;
                }
            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            string codiceIdentificativo = CodiceIdentificativoTextBox.Text.Trim();

            // Verifica che non esista già un sensore con lo stesso codice identificativo
            using (SIASSEntities context = new SIASSEntities())
            {
                bool sensoreEsistente = context.Sensori2021.Where(i => i.CODICE_IDENTIFICATIVO.ToUpper() == codiceIdentificativo.ToUpper()).Any();
                if (sensoreEsistente)
                {
                    CustomValidator validatore = new CustomValidator
                    {
                        IsValid = false,
                        ErrorMessage = "Esiste già un sensore con questo codice identificativo."
                    };
                    Page.Validators.Add(validatore);
                    return;
                }
            }

            if (TipiUnitaMisuraDropDownList.SelectedIndex == 0 ^ String.IsNullOrEmpty(CoefficienteConversioneUnitaMisuraTextBox.Text.Trim()))
            {
                CustomValidator validatore = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Se si indica l'unità di misura va indicato anche il coefficiente conversione e viceversa."
                };
                Page.Validators.Add(validatore);
                return;
            }

            if (Page.IsValid)
            {
                decimal idStrumento = (decimal)ViewState["IdStrumento"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    Sensore2021 sensore = new Sensore2021
                    {
                        CODICE_IDENTIFICATIVO = codiceIdentificativo,
                        CODICE_IDENTIFICATIVO_CREAZ = codiceIdentificativo,
                        ID_STRUMENTO_STAZIONE = idStrumento,
                        ID_GRANDEZZA_STAZIONE = decimal.Parse(GrandezzeStazioneDropDownList.SelectedValue)
                    };

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

                    context.Sensori2021.Add(sensore);
                    context.SaveChanges();
                    Response.Redirect($"ElencoSensori.aspx?IdStrumento={ViewState["IdStrumento"]}");
                }
            }

        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoSensori.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }
    }
}