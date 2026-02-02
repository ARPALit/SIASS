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
    public partial class NuovaLocalizzazione : System.Web.UI.Page
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
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                using (SIASSEntities context = new SIASSEntities())
                {
                    Localizzazione localizzazione = new Localizzazione
                    {
                        IdStazione = idStazione,

                        CodiceComune = ComuniDropDownList.SelectedValue
                    };
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
                        context.Localizzazioni.Add(localizzazione);
                        context.SaveChanges();
						logger.Info($"Inserita localizzazione - id:{localizzazione.IdLocalizzazione} - id stazione:{localizzazione.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");
                        Response.Redirect($"ElencoLocalizzazioni.aspx?IdStazione={ViewState["IdStazione"]}");
                    }
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoLocalizzazioni.aspx?IdStazione={ViewState["IdStazione"]}");
        }

    }
}