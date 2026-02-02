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
    public partial class NuovaGrandezza : System.Web.UI.Page
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

                using (SIASSEntities context = new SIASSEntities())
                {
                    var tipiGrandezza = context.TipiGrandezza.OrderBy(i => i.ORDINE).ThenBy(i => i.NOME_GRANDEZZA).ToList();
                    TipiGrandezzaDropDownList.DataSource = tipiGrandezza;
                    TipiGrandezzaDropDownList.DataBind();

                    var tipiUnitaMisura = context.TipiUnitaMisura2021.OrderBy(i => i.ORDINE).ThenBy(i => i.NOME_UNITA_MISURA).ToList();
                    TipiUnitaMisuraDropDownList.DataSource = tipiUnitaMisura;
                    TipiUnitaMisuraDropDownList.DataBind();

                    for (int i = 0; i <= 5; i++)
                        NumeroDecimaliDropDownList.Items.Add(new ListItem($"{i}"));
                }
            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                GrandezzaStazione grandezza = new GrandezzaStazione
                {
                    ID_STAZIONE = idStazione,
                    GRANDEZZA = TipiGrandezzaDropDownList.SelectedValue,
                    UNITA_MISURA = TipiUnitaMisuraDropDownList.SelectedValue,
                    NUMERO_DECIMALI = decimal.Parse(NumeroDecimaliDropDownList.SelectedValue)
                };

                if (context.GrandezzeStazione.Where(i => i.ID_STAZIONE == grandezza.ID_STAZIONE && i.GRANDEZZA.ToUpper() == grandezza.GRANDEZZA.ToUpper()).Any())
                {
                    CustomValidator validatoreAnnotazioni = new CustomValidator
                    {
                        IsValid = false,
                        ErrorMessage = "Grandezza già presente per questa stazione."
                    };
                    Page.Validators.Add(validatoreAnnotazioni);
                }

                if (Page.IsValid)
                {
                    context.GrandezzeStazione.Add(grandezza);
                    context.SaveChanges();
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					logger.Info($"Inserito grandezza - id:{grandezza.GRANDEZZA} - id stazione:{grandezza.ID_STAZIONE} - Operatore:{oper.Nome} {oper.Cognome}");
                    Response.Redirect($"ElencoGrandezze.aspx?IdStazione={ViewState["IdStazione"]}");
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"ElencoGrandezze.aspx?IdStazione={ViewState["IdStazione"]}");
        }
    }
}