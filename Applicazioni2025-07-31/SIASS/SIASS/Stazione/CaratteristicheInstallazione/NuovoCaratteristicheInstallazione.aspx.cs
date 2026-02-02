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
    public partial class NuovoCaratteristicheInstallazione : System.Web.UI.Page
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
                TipiFissaggioTrasmettitoreDropDownList.DataSource = CaratteristicheInstallazioneManager.TipiFissaggioTrasmettitore();
                TipiFissaggioTrasmettitoreDropDownList.DataBind();
                TipiFissaggioTrasmettitoreDropDownList.Items.Insert(0, new ListItem(String.Empty));

                TipiAccessoDropDownList.DataSource = CaratteristicheInstallazioneManager.TipiAccesso();
                TipiAccessoDropDownList.DataBind();
                TipiAccessoDropDownList.Items.Insert(0, new ListItem(String.Empty));

                if (!decimal.TryParse(Request.QueryString["IdStazione"], out decimal idStazione))
                {
                    logger.Debug($"Parametro IdStazione mancante");
                    Response.Write($"Parametro IdStazione mancante");
                    Response.End();
                    return;
                }

                logger.Debug($"CaricaInfoStazione:{idStazione}");
                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                ViewState.Add("IdStazione", idStazione);

            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (OsservazioniTextBox.Text.Trim().Length > 1000)
            {
                CustomValidator validatoreAnnotazioni = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Osservazioni: lunghezza massima 1000 caratteri"
                };
                Page.Validators.Add(validatoreAnnotazioni);
            }

            if (SicurezzaTextBox.Text.Trim().Length > 1000)
            {
                CustomValidator validatoreAnnotazioni = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Sicurezza: lunghezza massima 1000 caratteri"
                };
                Page.Validators.Add(validatoreAnnotazioni);
            }

            if (Page.IsValid)
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                logger.Debug($"Salva CaratteristicheInstallazione idStazione:{idStazione}");
                using (SIASSEntities context = new SIASSEntities())
                {
                    CaratteristicheInstallazione caratteristicheInstallazione = new CaratteristicheInstallazione
                    {
                        IdStazione = idStazione
                    };

                    if (TipiFissaggioTrasmettitoreDropDownList.SelectedIndex > 0)
                        caratteristicheInstallazione.IdTipoFissaggioTrasmettitore = decimal.Parse(TipiFissaggioTrasmettitoreDropDownList.SelectedValue);
                    else
                        caratteristicheInstallazione.IdTipoFissaggioTrasmettitore = null;
                    caratteristicheInstallazione.CavoEsternoInGuaina = CavoEsternoInGuainaCheckBox.Checked;
                    caratteristicheInstallazione.CavoSottotraccia = CavoSottotracciaCheckBox.Checked;
                    caratteristicheInstallazione.ProtezioneArea = ProtezioneAreaCheckBox.Checked;
                    if (TipiAccessoDropDownList.SelectedIndex > 0)
                        caratteristicheInstallazione.IdTipoAccesso = decimal.Parse(TipiAccessoDropDownList.SelectedValue);
                    else
                        caratteristicheInstallazione.IdTipoAccesso = null;

                    if (String.IsNullOrEmpty(OsservazioniTextBox.Text.Trim()))
                        caratteristicheInstallazione.Osservazioni = null;
                    else
                        caratteristicheInstallazione.Osservazioni = OsservazioniTextBox.Text.Trim();
                    if (String.IsNullOrEmpty(SicurezzaTextBox.Text.Trim()))
                        caratteristicheInstallazione.Sicurezza = null;
                    else
                        caratteristicheInstallazione.Sicurezza = SicurezzaTextBox.Text.Trim();

					if (String.IsNullOrEmpty(ProfonditaSensoreTextBox.Text.Trim()))
						caratteristicheInstallazione.PROFONDITA_SENSORE = null;
					else
						caratteristicheInstallazione.PROFONDITA_SENSORE = decimal.Parse(ProfonditaSensoreTextBox.Text.Trim());

					caratteristicheInstallazione.InizioValidita = DateTime.Parse(InizioValiditaTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(FineValiditaTextBox.Text.Trim()))
                        caratteristicheInstallazione.FineValidita = null;
                    else
                        caratteristicheInstallazione.FineValidita = DateTime.Parse(FineValiditaTextBox.Text.Trim());

                    caratteristicheInstallazione.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					caratteristicheInstallazione.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                    if (!CaratteristicheInstallazioneManager.Valida(caratteristicheInstallazione, out string errore))
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
                        context.CaratteristicheInstallazioni.Add(caratteristicheInstallazione);
                        context.SaveChanges();
                        logger.Info($"Inserita caratteristica installazione - id:{caratteristicheInstallazione.IdCaratteristicheInstallazione} - id stazione:{caratteristicheInstallazione.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoCaratteristicheInstallazione.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoCaratteristicheInstallazione.aspx?IdStazione={ViewState["IdStazione"]}");
        }
    }
}