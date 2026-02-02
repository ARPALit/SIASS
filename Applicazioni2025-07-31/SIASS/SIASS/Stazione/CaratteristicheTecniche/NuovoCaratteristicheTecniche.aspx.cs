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
using System.Web.UI.WebControls;


namespace SIASS
{
    public partial class NuovoCaratteristicheTecniche : System.Web.UI.Page
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
                TipiChiusuraDropDownList.DataSource = CaratteristicheTecnichePozzoManager.TipiChiusura();
                TipiChiusuraDropDownList.DataBind();
                TipiChiusuraDropDownList.Items.Insert(0, new ListItem(String.Empty));

                TipiDestinazioneUsoDropDownList.DataSource = CaratteristicheTecnichePozzoManager.TipiDestinazioneUso();
                TipiDestinazioneUsoDropDownList.DataBind();
                TipiDestinazioneUsoDropDownList.Items.Insert(0, new ListItem(String.Empty));

                TipiFrequenzaUtilizzoDropDownList.DataSource = CaratteristicheTecnichePozzoManager.TipiFrequenzaUtilizzo();
                TipiFrequenzaUtilizzoDropDownList.DataBind();
                TipiFrequenzaUtilizzoDropDownList.Items.Insert(0, new ListItem(String.Empty));

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

            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    CaratteristicheTecnichePozzo caratteristicheTecnichePozzo = new CaratteristicheTecnichePozzo(); ;
                    caratteristicheTecnichePozzo.IdStazione = idStazione;
                    if (String.IsNullOrEmpty(ProfonditaTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.Profondita = null;
                    else
                        caratteristicheTecnichePozzo.Profondita = decimal.Parse(ProfonditaTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(DiametroTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.Diametro = null;
                    else
                        caratteristicheTecnichePozzo.Diametro = decimal.Parse(DiametroTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(RangeSoggiacenzaDaTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.RangeSoggiacenzaDa = null;
                    else
                        caratteristicheTecnichePozzo.RangeSoggiacenzaDa = decimal.Parse(RangeSoggiacenzaDaTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(RangeSoggiacenzaATextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.RangeSoggiacenzaA = null;
                    else
                        caratteristicheTecnichePozzo.RangeSoggiacenzaA = decimal.Parse(RangeSoggiacenzaATextBox.Text.Trim());
                    if (TipiChiusuraDropDownList.SelectedIndex > 0)
                        caratteristicheTecnichePozzo.IdTipoChiusura = decimal.Parse(TipiChiusuraDropDownList.SelectedValue);
                    else
                        caratteristicheTecnichePozzo.IdTipoChiusura = null;
                    if (String.IsNullOrEmpty(QuotaBoccapozzoPcTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.QuotaBoccapozzoPc = null;
                    else
                        caratteristicheTecnichePozzo.QuotaBoccapozzoPc = decimal.Parse(QuotaBoccapozzoPcTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(QuotaBoccapozzoSlmTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.QuotaBoccapozzoSlm = null;
                    else
                        caratteristicheTecnichePozzo.QuotaBoccapozzoSlm = decimal.Parse(QuotaBoccapozzoSlmTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(QuotaPianoRiferimentoSlmTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm = null;
                    else
                        caratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm = decimal.Parse(QuotaPianoRiferimentoSlmTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(DifferenzaPrpcTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.DifferenzaPrpc = null;
                    else
                        caratteristicheTecnichePozzo.DifferenzaPrpc = decimal.Parse(DifferenzaPrpcTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(ProfonditaEmungimentoTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.ProfonditaEmungimento = null;
                    else
                        caratteristicheTecnichePozzo.ProfonditaEmungimento = decimal.Parse(ProfonditaEmungimentoTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(PortataMassimaEsercizioTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.PortataMassimaEsercizio = null;
                    else
                        caratteristicheTecnichePozzo.PortataMassimaEsercizio = decimal.Parse(PortataMassimaEsercizioTextBox.Text.Trim());
                    caratteristicheTecnichePozzo.PresenzaForoSonda = PresenzaForoSondaCheckBox.Checked;
                    if (TipiDestinazioneUsoDropDownList.SelectedIndex > 0)
                        caratteristicheTecnichePozzo.IdTipoDestinazioneUso = decimal.Parse(TipiDestinazioneUsoDropDownList.SelectedValue);
                    else
                        caratteristicheTecnichePozzo.IdTipoDestinazioneUso = null;
                    if (TipiFrequenzaUtilizzoDropDownList.SelectedIndex > 0)
                        caratteristicheTecnichePozzo.IdTipoFrequenzaUtilizzo = decimal.Parse(TipiFrequenzaUtilizzoDropDownList.SelectedValue);
                    else
                        caratteristicheTecnichePozzo.IdTipoFrequenzaUtilizzo = null;
                    caratteristicheTecnichePozzo.Captata = CaptataCheckBox.Checked;
                    caratteristicheTecnichePozzo.InizioValidita = DateTime.Parse(InizioValiditaTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(FineValiditaTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.FineValidita = null;
                    else
                        caratteristicheTecnichePozzo.FineValidita = DateTime.Parse(FineValiditaTextBox.Text.Trim());

                    caratteristicheTecnichePozzo.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					caratteristicheTecnichePozzo.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                    if (!CaratteristicheTecnichePozzoManager.Valida(caratteristicheTecnichePozzo, out string errore))
                    {
                        CustomValidator validatore = new CustomValidator
                        {
                            IsValid = false,
                            ErrorMessage = errore
                        };
                        Page.Validators.Add(validatore);
                    }

                    if (Page.IsValid)
                    {
                        context.CaratteristicheTecnichePozzi.Add(caratteristicheTecnichePozzo);
                        context.SaveChanges();
						logger.Info($"Inserita caratteristica tecnica - id:{caratteristicheTecnichePozzo.IdCaratteristicheTecnichePozzo} - id stazione:{caratteristicheTecnichePozzo.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoCaratteristicheTecniche.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoCaratteristicheTecniche.aspx?IdStazione={ViewState["IdStazione"]}");
        }

    }
}