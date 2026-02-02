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
    public partial class ModificaCaratteristicheTecniche : System.Web.UI.Page
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

                if (!decimal.TryParse(Request.QueryString["IdCaratteristicheTecniche"], out decimal idCaratteristicheTecnichePozzo))
                {
                    logger.Debug($"Parametro IdCaratteristicheTecniche mancante");
                    Response.Write($"Parametro IdCaratteristicheTecniche mancante");
                    Response.End();
                    return;
                }

                InfoCaratteristicheTecnichePozzo infoCaratteristicheTecnichePozzo = CaratteristicheTecnichePozzoManager.CaricaInfoCaratteristicheTecnichePozzo(idCaratteristicheTecnichePozzo);

                if (infoCaratteristicheTecnichePozzo == null)
                {
                    logger.Warn($"InfoCaratteristicheTecnichePozzo non trovato: IdCaratteristicheTecniche={idCaratteristicheTecnichePozzo}");
                    Response.Write($"InfoCaratteristicheTecnichePozzo non trovato: IdCaratteristicheTecniche={idCaratteristicheTecnichePozzo}");
                    Response.End();
                    return;
                }

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoCaratteristicheTecnichePozzo.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                ViewState.Add("IdCaratteristicheTecnichePozzo", idCaratteristicheTecnichePozzo);
                ViewState.Add("IdStazione", infoCaratteristicheTecnichePozzo.IdStazione);

                if (infoCaratteristicheTecnichePozzo.Profondita.HasValue)
                    ProfonditaTextBox.Text = infoCaratteristicheTecnichePozzo.Profondita.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.Diametro.HasValue)
                    DiametroTextBox.Text = infoCaratteristicheTecnichePozzo.Diametro.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.RangeSoggiacenzaDa.HasValue)
                    RangeSoggiacenzaDaTextBox.Text = infoCaratteristicheTecnichePozzo.RangeSoggiacenzaDa.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.RangeSoggiacenzaA.HasValue)
                    RangeSoggiacenzaATextBox.Text = infoCaratteristicheTecnichePozzo.RangeSoggiacenzaA.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.IdTipoChiusura.HasValue)
                    TipiChiusuraDropDownList.SelectedValue = infoCaratteristicheTecnichePozzo.IdTipoChiusura.ToString();
                if (infoCaratteristicheTecnichePozzo.QuotaBoccapozzoPc.HasValue)
                    QuotaBoccapozzoPcTextBox.Text = infoCaratteristicheTecnichePozzo.QuotaBoccapozzoPc.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.QuotaBoccapozzoSlm.HasValue)
                    QuotaBoccapozzoSlmTextBox.Text = infoCaratteristicheTecnichePozzo.QuotaBoccapozzoSlm.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm.HasValue)
                    QuotaPianoRiferimentoSlmTextBox.Text = infoCaratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.DifferenzaPrpc.HasValue)
                    DifferenzaPrpcTextBox.Text = infoCaratteristicheTecnichePozzo.DifferenzaPrpc.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.ProfonditaEmungimento.HasValue)
                    ProfonditaEmungimentoTextBox.Text = infoCaratteristicheTecnichePozzo.ProfonditaEmungimento.Value.ToString();
                if (infoCaratteristicheTecnichePozzo.PortataMassimaEsercizio.HasValue)
                    PortataMassimaEsercizioTextBox.Text = infoCaratteristicheTecnichePozzo.PortataMassimaEsercizio.Value.ToString();
                PresenzaForoSondaCheckBox.Checked = infoCaratteristicheTecnichePozzo.PresenzaForoSonda;
                if (infoCaratteristicheTecnichePozzo.IdTipoDestinazioneUso.HasValue)
                    TipiDestinazioneUsoDropDownList.SelectedValue = infoCaratteristicheTecnichePozzo.IdTipoDestinazioneUso.ToString();
                if (infoCaratteristicheTecnichePozzo.IdTipoFrequenzaUtilizzo.HasValue)
                    TipiFrequenzaUtilizzoDropDownList.SelectedValue = infoCaratteristicheTecnichePozzo.IdTipoFrequenzaUtilizzo.ToString();
                CaptataCheckBox.Checked = infoCaratteristicheTecnichePozzo.Captata;
                InizioValiditaTextBox.Text = infoCaratteristicheTecnichePozzo.InizioValidita.ToString("dd/MM/yyyy");
                if (infoCaratteristicheTecnichePozzo.FineValidita.HasValue)
                    FineValiditaTextBox.Text = infoCaratteristicheTecnichePozzo.FineValidita.Value.ToString("dd/MM/yyyy");

            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idCaratteristicheTecnichePozzo = (decimal)ViewState["IdCaratteristicheTecnichePozzo"];
                logger.Warn($"Salva CaratteristicheTecnichePozzo: idCaratteristicheTecnichePozzo={idCaratteristicheTecnichePozzo}");
                using (SIASSEntities context = new SIASSEntities())
                {
                    CaratteristicheTecnichePozzo caratteristicheTecnichePozzo = context.CaratteristicheTecnichePozzi.Where(i => i.IdCaratteristicheTecnichePozzo == idCaratteristicheTecnichePozzo).FirstOrDefault();

                    if (caratteristicheTecnichePozzo == null)
                    {
                        logger.Warn($"CaratteristicheTecnichePozzo non trovato: idCaratteristicheTecnichePozzo={idCaratteristicheTecnichePozzo}");
                        Response.Write($"CaratteristicheTecnichePozzo non trovato: idCaratteristicheTecnichePozzo={idCaratteristicheTecnichePozzo}");
                        Response.End();
                        return;
                    }

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
                    if (String.IsNullOrEmpty(ProfonditaEmungimentoTextBox.Text.Trim()))
                        caratteristicheTecnichePozzo.ProfonditaEmungimento = null;
                    else
                        caratteristicheTecnichePozzo.ProfonditaEmungimento = decimal.Parse(ProfonditaEmungimentoTextBox.Text.Trim());
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
                        context.SaveChanges();
						logger.Info($"Modificata caratteristica tecnica - id:{caratteristicheTecnichePozzo.IdCaratteristicheTecnichePozzo} - id stazione:{caratteristicheTecnichePozzo.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoCaratteristicheTecniche.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoCaratteristicheTecniche.aspx?IdStazione={ViewState["IdStazione"]}");
        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idCaratteristicheTecnichePozzo = (decimal)ViewState["IdCaratteristicheTecnichePozzo"];
            logger.Debug($"Elimina CaratteristicheTecnichePozzo: idCaratteristicheTecnichePozzo={idCaratteristicheTecnichePozzo}");
            using (SIASSEntities context = new SIASSEntities())
            {
                CaratteristicheTecnichePozzo caratteristicheTecnichePozzo = context.CaratteristicheTecnichePozzi.Where(i => i.IdCaratteristicheTecnichePozzo == idCaratteristicheTecnichePozzo).FirstOrDefault();
                if (caratteristicheTecnichePozzo != null)
                {
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
                    logger.Info($"Eliminata caratteristica tecnica - id:{caratteristicheTecnichePozzo.IdCaratteristicheTecnichePozzo} - id stazione:{caratteristicheTecnichePozzo.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                    context.CaratteristicheTecnichePozzi.Remove(caratteristicheTecnichePozzo);
                    context.SaveChanges();
                }
            }
            Response.Redirect($"ElencoCaratteristicheTecniche.aspx?IdStazione={ViewState["IdStazione"]}");
        }
    }
}