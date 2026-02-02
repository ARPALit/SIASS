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
    public partial class ModificaLocalizzazione : System.Web.UI.Page
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
                using (SIASSEntities context = new SIASSEntities())
                {
                    ComuniDropDownList.DataSource = context.Comuni.OrderBy(i => i.DenominazioneComune).ToList();
                    ComuniDropDownList.DataBind();
                    ComuniDropDownList.Items.Insert(0, new ListItem(String.Empty));

                    BaciniDropDownList.DataSource = context.Bacini.OrderBy(i => i.DescrizioneBacino).ToList();
                    BaciniDropDownList.DataBind();
                    BaciniDropDownList.Items.Insert(0, new ListItem(String.Empty));

                    CorpiIdriciDropDownList.DataSource = context.CorpiIdrici.OrderBy(i => i.DescrizioneCorpoIdrico).ToList();
                    CorpiIdriciDropDownList.DataBind();
                    CorpiIdriciDropDownList.Items.Insert(0, new ListItem(String.Empty));

                    if (!decimal.TryParse(Request.QueryString["IdLocalizzazione"], out decimal idLocalizzazione))
                    {
                        logger.Debug($"Parametro IdLocalizzazione mancante");
                        Response.Write($"Parametro IdLocalizzazione mancante");
                        Response.End();
                        return;
                    }

                    InfoLocalizzazione infoLocalizzazione = LocalizzazioneManager.CaricaInfoLocalizzazione(idLocalizzazione);

                    if (infoLocalizzazione == null)
                    {
                        Response.Write($"Localizzazione non trovato: IdLocalizzazione={idLocalizzazione}");
                        Response.End();
                        return;
                    }

                    InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoLocalizzazione.IdStazione);
                    HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                    ViewState.Add("IdLocalizzazione", idLocalizzazione);
                    ViewState.Add("IdStazione", infoLocalizzazione.IdStazione);

                    ComuniDropDownList.SelectedValue = infoLocalizzazione.CodiceComune;
                    LocalitaTextBox.Text = infoLocalizzazione.Localita;
                    BaciniDropDownList.SelectedValue = infoLocalizzazione.IdBacino.ToString();
                    CorpiIdriciDropDownList.SelectedValue = infoLocalizzazione.IdCorpoIdrico.ToString();
                    CTRTextBox.Text = infoLocalizzazione.CTR;
                    LongitudineTextBox.Text = infoLocalizzazione.Longitudine.ToString();
                    LatitudineTextBox.Text = infoLocalizzazione.Latitudine.ToString();
                    LongitudineGaussBoagaTextBox.Text = infoLocalizzazione.LongitudineGaussBoaga.ToString();
                    LatitudineGaussBoagaTextBox.Text = infoLocalizzazione.LatitudineGaussBoaga.ToString();
                    if (infoLocalizzazione.QuotaPianoCampagna.HasValue)
                        QuotaPianoCampagnaTextBox.Text = infoLocalizzazione.QuotaPianoCampagna.ToString();
                    CodiceSIRALTextBox.Text = infoLocalizzazione.CodiceSIRAL;
                    InizioValiditaTextBox.Text = infoLocalizzazione.InizioValidita.ToString("dd/MM/yyyy");
                    if (infoLocalizzazione.FineValidita.HasValue)
                        FineValiditaTextBox.Text = infoLocalizzazione.FineValidita.Value.ToString("dd/MM/yyyy");

                }
            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idLocalizzazione = (decimal)ViewState["IdLocalizzazione"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    Localizzazione localizzazione = context.Localizzazioni.Where(i => i.IdLocalizzazione == idLocalizzazione).FirstOrDefault();

                    if (localizzazione == null)
                    {
                        Response.Write($"Localizzazione non trovato: IdLocalizzazione={idLocalizzazione}");
                        Response.End();
                        return;
                    }

                    localizzazione.CodiceComune = ComuniDropDownList.SelectedValue;
                    if (String.IsNullOrEmpty(LocalitaTextBox.Text.Trim()))
                        localizzazione.Localita = null;
                    else
                        localizzazione.Localita = LocalitaTextBox.Text.Trim();
                    localizzazione.IdBacino = decimal.Parse(BaciniDropDownList.SelectedValue);
                    localizzazione.IdCorpoIdrico = decimal.Parse(CorpiIdriciDropDownList.SelectedValue);
                    if (String.IsNullOrEmpty(CTRTextBox.Text.Trim()))
                        localizzazione.CTR = null;
                    else
                        localizzazione.CTR = CTRTextBox.Text.Trim();
                    localizzazione.Latitudine = decimal.Parse(LatitudineTextBox.Text.Trim());
                    localizzazione.Longitudine = decimal.Parse(LongitudineTextBox.Text.Trim());
                    localizzazione.LatitudineGaussBoaga = decimal.Parse(LatitudineGaussBoagaTextBox.Text.Trim());
                    localizzazione.LongitudineGaussBoaga = decimal.Parse(LongitudineGaussBoagaTextBox.Text.Trim());

                    if (String.IsNullOrEmpty(QuotaPianoCampagnaTextBox.Text.Trim()))
                        localizzazione.QuotaPianoCampagna = null;
                    else
                        localizzazione.QuotaPianoCampagna = decimal.Parse(QuotaPianoCampagnaTextBox.Text.Trim());

                    if (String.IsNullOrEmpty(CodiceSIRALTextBox.Text.Trim()))
                        localizzazione.CodiceSIRAL = null;
                    else
                        localizzazione.CodiceSIRAL = CodiceSIRALTextBox.Text.Trim();

                    localizzazione.InizioValidita = DateTime.Parse(InizioValiditaTextBox.Text.Trim());
                    if (String.IsNullOrEmpty(FineValiditaTextBox.Text.Trim()))
                        localizzazione.FineValidita = null;
                    else
                        localizzazione.FineValidita = DateTime.Parse(FineValiditaTextBox.Text.Trim());

                    localizzazione.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					localizzazione.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";


                    if (!LocalizzazioneManager.Valida(localizzazione, out string errore))
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
						logger.Info($"Aggiornata localizzazione - id:{localizzazione.IdLocalizzazione} - id stazione:{localizzazione.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoLocalizzazioni.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }

        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoLocalizzazioni.aspx?IdStazione={ViewState["IdStazione"]}");
        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idLocalizzazione = (decimal)ViewState["IdLocalizzazione"];
            using (SIASSEntities context = new SIASSEntities())
            {
                Localizzazione localizzazione = context.Localizzazioni.Where(i => i.IdLocalizzazione == idLocalizzazione).FirstOrDefault();
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				logger.Info($"Eliminata localizzazione - id:{localizzazione.IdLocalizzazione} - id stazione:{localizzazione.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                context.Localizzazioni.Remove(localizzazione);
                context.SaveChanges();
            }
            Response.Redirect($"ElencoLocalizzazioni.aspx?IdStazione={ViewState["IdStazione"]}");
        }
    }
}