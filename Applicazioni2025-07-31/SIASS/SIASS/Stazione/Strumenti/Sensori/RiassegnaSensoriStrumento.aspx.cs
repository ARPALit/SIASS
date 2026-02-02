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
    public partial class RiassegnaSensoriStrumento : System.Web.UI.Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static decimal gIdStrumento;
        private static List<string> gElencoSensoriDaRiassegnare;
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

                ViewState.Add("IdStrumento", idStrumento);
                ViewState.Add("IdStazione", infoStrumento.IdStazione);

                InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoStrumento.IdStazione);
                HeaderStazioneResponsive1.PopolaCampi(infoStazione);

                // Popola dati
                DescrizioneTipoStrumentoHyperLink.Text = HttpUtility.HtmlEncode(infoStrumento.DescrizioneTipoStrumento);
                DescrizioneTipoStrumentoHyperLink.NavigateUrl = $"~/Stazione/Strumenti/VisualizzaStrumento.aspx?IdStrumento={infoStrumento.IdStrumento}";
                NumeroDiSerieLabel.Text = HttpUtility.HtmlEncode(infoStrumento.NumeroDiSerie);

                var elencoSensoriStrumento = SensoreManager.ElencoSensoriStrumento(infoStrumento.IdStrumento);
                SensoriStrumentoGridView.DataSource = elencoSensoriStrumento;
                SensoriStrumentoGridView.DataBind();

                var elencoStrumenti = StrumentoManager.ElencoStrumenti(infoStrumento.IdStazione);
                ElencoStrumentiGridView.DataSource = elencoStrumenti
                    .Where(i => i.IdStrumento != idStrumento)
                    .OrderBy(i => i.Marca)
                    .ThenBy(i => i.Modello);
                ElencoStrumentiGridView.DataBind();
                RiassegnaSensoriMultiView.SetActiveView(SelezioneSensoriView);
            }
        }

        protected void ElencoStrumentiGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            gIdStrumento = (decimal)ElencoStrumentiGridView.SelectedValue;
            InfoStrumento infoStrumento = StrumentoManager.CaricaInfoStrumento(gIdStrumento);
            gElencoSensoriDaRiassegnare = new List<string>();
            foreach (GridViewRow riga in SensoriStrumentoGridView.Rows)
            {
                if (riga.RowType == DataControlRowType.DataRow)
                {
                    CheckBox selezionaCheckBox = (CheckBox)(riga.Cells[0].FindControl("SelezionaCheckBox"));
                    if (selezionaCheckBox.Checked)
                    {
                        string codiceIdentificativo = SensoriStrumentoGridView.DataKeys[riga.RowIndex].Value.ToString();
                        gElencoSensoriDaRiassegnare.Add(codiceIdentificativo);
                    }
                }
            }
            if (gElencoSensoriDaRiassegnare.Count > 0)
            {
                string sensori = "il sensore";
                if (gElencoSensoriDaRiassegnare.Count > 1)
                    sensori = "i sensori";
                sensori += " " + string.Join(", ", gElencoSensoriDaRiassegnare.ToArray());
                ConfermaLabel.Text = $"Riassegnare {sensori} allo strumento {infoStrumento.DescrizioneTipoStrumento} {infoStrumento.NumeroDiSerie}?";
                RiassegnaSensoriMultiView.SetActiveView(ConfermaView);
            }
        }

        protected void RiassegnaButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Riassegnazione sensori");
            using (SIASSEntities context = new SIASSEntities())
            {
                foreach (var codiceSensore in gElencoSensoriDaRiassegnare)
                {
                    Sensore2021 s = context.Sensori2021.Where(i => i.CODICE_IDENTIFICATIVO.ToUpper() == codiceSensore.ToUpper()).FirstOrDefault();
                    logger.Debug($"Riassegnazione sensore {s.CODICE_IDENTIFICATIVO} a idstrumento {gIdStrumento}");
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					Sensore2021 sCopia = new Sensore2021
                    {
                        AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}",
                        CODICE_IDENTIFICATIVO = $"EX-{s.CODICE_IDENTIFICATIVO_CREAZ}-{DateTime.Now:yyMMddHHmmss}",
                        CODICE_IDENTIFICATIVO_CREAZ = s.CODICE_IDENTIFICATIVO_CREAZ,
                        CODICE_PMC = s.CODICE_PMC,
                        COEFF_CONVER_UNITA_MISURA = s.COEFF_CONVER_UNITA_MISURA,
                        FREQUENZA_ACQUISIZIONE = s.FREQUENZA_ACQUISIZIONE,
                        ID_GRANDEZZA_STAZIONE = s.ID_GRANDEZZA_STAZIONE,
                        ID_STRUMENTO_STAZIONE = s.ID_STRUMENTO_STAZIONE,
                        ID_TIPO_ESPRESS_RISULTATO = s.ID_TIPO_ESPRESS_RISULTATO,
                        ID_TIPO_METODO = s.ID_TIPO_METODO,
                        ULTIMO_AGGIORNAMENTO = DateTime.Now,
                        UNITA_MISURA = s.UNITA_MISURA
                    };
                    context.Sensori2021.Add(sCopia);
                    logger.Debug($"Creazione copia sensore {sCopia.CODICE_IDENTIFICATIVO}");
                    s.ID_STRUMENTO_STAZIONE = gIdStrumento;
                    s.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";
                    s.ULTIMO_AGGIORNAMENTO = DateTime.Now;
                    context.SaveChanges();
                }
            }
            Response.Redirect($"../VisualizzaStrumento.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect($"RiassegnaSensoriStrumento.aspx?IdStrumento={ViewState["IdStrumento"]}");
        }
    }
}
