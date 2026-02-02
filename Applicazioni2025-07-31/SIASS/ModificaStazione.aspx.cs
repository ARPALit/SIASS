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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
    public partial class ModificaStazione : System.Web.UI.Page
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
                if (!decimal.TryParse(Request.QueryString["IdStazione"], out decimal idStazione))
                {
                    logger.Debug($"Parametro IdStazione mancante");
                    Response.Write($"Parametro IdStazione mancante");
                    Response.End();
                    return;
                }

                using (SIASSEntities context = new SIASSEntities())
                {
                    var s = context.Stazioni.Where(i => i.IdStazione == idStazione).FirstOrDefault();
                    if (s == null)
                    {
                        logger.Warn($"Stazione non trovata: IdStazione={idStazione}");
                        Response.Write($"Stazione non trovata: IdStazione={idStazione}");
                        Response.End();
                        return;
                    }

                    ViewState.Add("IdStazione", s.IdStazione);

                    // Popola dropdown
                    TipiStazioneDropDownList.DataSource = StazioneManager.TipiStazione();
                    TipiStazioneDropDownList.DataBind();

                    AllegatiFotoStazioneDropDownList.DataSource = AllegatoStazioneManager.ElencoAllegatiStazione(s.IdStazione);
                    AllegatiFotoStazioneDropDownList.DataBind();
                    AllegatiFotoStazioneDropDownList.Items.Insert(0, string.Empty);

                    AllegatiMappaDropDownList.DataSource = AllegatoStazioneManager.ElencoAllegatiStazione(s.IdStazione);
                    AllegatiMappaDropDownList.DataBind();
                    AllegatiMappaDropDownList.Items.Insert(0, string.Empty);

                    TipiAllestimentoDropDownList.DataSource = StazioneManager.TipiAllestimento();
                    TipiAllestimentoDropDownList.DataBind();
                    TipiAllestimentoDropDownList.Items.Insert(0, string.Empty);

                    SitiDropDownList.DataSource = SitoManager.ElencoSitiPerSelezione();
                    SitiDropDownList.DataBind();
                    SitiDropDownList.Items.Insert(0, string.Empty);

                    ReteAppartenenzaCheckBoxList.DataSource = StazioneManager.TipiReteAppartenenza();
                    ReteAppartenenzaCheckBoxList.DataBind();

                    FinalitaCheckBoxList.DataSource = StazioneManager.TipiFinalitaStazioni();
                    FinalitaCheckBoxList.DataBind();


                    CodiceIdentificativoLabel.Text = s.CodiceIdentificativo;
                    DescrizioneTextBox.Text = s.Descrizione;
                    EsclusaMonitoraggioCheckBox.Checked = s.EsclusaMonitoraggio;
                    TipiStazioneDropDownList.SelectedValue = s.IdTipoStazione.ToString();
                    AnnotazioniTextBox.Text = s.Annotazioni;
                    if (s.IdAllegatoFotoStazione.HasValue)
                        AllegatiFotoStazioneDropDownList.SelectedValue = s.IdAllegatoFotoStazione.Value.ToString();
                    if (s.IdAllegatoMappa.HasValue)
                        AllegatiMappaDropDownList.SelectedValue = s.IdAllegatoMappa.Value.ToString();
                    if (!string.IsNullOrEmpty(s.Allestimento))
                        TipiAllestimentoDropDownList.SelectedValue = s.Allestimento;
                    TeletrasmissioneCheckBox.Checked = s.Teletrasmissione;
                    PuntoConformitaCheckBox.Checked = s.PUNTO_CONFORMITA;
                    if (s.IdSito.HasValue)
                        SitiDropDownList.SelectedValue = s.IdSito.Value.ToString();

                    for (int r = 0; r < ReteAppartenenzaCheckBoxList.Items.Count; r++)
                    {
                        string descrizione = ReteAppartenenzaCheckBoxList.Items[r].Value;
                        if (context.StazioniReti.Where(i => i.RETE == descrizione && i.ID_STAZIONE == s.IdStazione).FirstOrDefault() != null)
                            ReteAppartenenzaCheckBoxList.Items[r].Selected = true;
                    }

                    for (int f = 0; f < FinalitaCheckBoxList.Items.Count; f++)
                    {
                        string descrizione = FinalitaCheckBoxList.Items[f].Value;
                        if (context.FinalitaStazioni.Where(i => i.FINALITA == descrizione && i.ID_STAZIONE == s.IdStazione).FirstOrDefault() != null)
                            FinalitaCheckBoxList.Items[f].Selected = true;
                    }

                    ControlloAnomalieCheckBox.Checked = s.CONTROLLO_ANOMALIE;
                }

            }
        }

        protected void SalvaButton_Click(object sender, EventArgs e)
        {
            if (AnnotazioniTextBox.Text.Trim().Length > 1000)
            {
                CustomValidator validatoreAnnotazioni = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Annotazioni: lunghezza massima 1000 caratteri"
                };
                Page.Validators.Add(validatoreAnnotazioni);
            }
            if (Page.IsValid)
            {
                decimal idStazione = (decimal)ViewState["IdStazione"];
                logger.Debug($"Aggiorna stazione idStazione:{idStazione}");

                using (SIASSEntities context = new SIASSEntities())
                {
                    var s = context.Stazioni.Where(i => i.IdStazione == idStazione).FirstOrDefault();

                    if (s == null)
                    {
                        logger.Warn($"Stazione non trovata IdStazione:{idStazione}");
                        Response.Write($"Stazione non trovata IdStazione:{idStazione}");
                        Response.End();
                        return;
                    }

                    s.Descrizione = DescrizioneTextBox.Text.Trim();
                    s.EsclusaMonitoraggio = EsclusaMonitoraggioCheckBox.Checked;
                    s.IdTipoStazione = decimal.Parse(TipiStazioneDropDownList.SelectedValue);
                    if (string.IsNullOrEmpty(AnnotazioniTextBox.Text.Trim()))
                        s.Annotazioni = null;
                    else
                        s.Annotazioni = AnnotazioniTextBox.Text.Trim();
                    if (AllegatiFotoStazioneDropDownList.SelectedIndex == 0)
                        s.IdAllegatoFotoStazione = null;
                    else
                        s.IdAllegatoFotoStazione = decimal.Parse(AllegatiFotoStazioneDropDownList.SelectedValue);
                    if (AllegatiMappaDropDownList.SelectedIndex == 0)
                        s.IdAllegatoMappa = null;
                    else
                        s.IdAllegatoMappa = decimal.Parse(AllegatiMappaDropDownList.SelectedValue);
                    if (TipiAllestimentoDropDownList.SelectedIndex == 0)
                        s.Allestimento = null;
                    else
                        s.Allestimento = TipiAllestimentoDropDownList.SelectedValue;
                    s.Teletrasmissione = TeletrasmissioneCheckBox.Checked;
                    s.PUNTO_CONFORMITA = PuntoConformitaCheckBox.Checked;
                    if (SitiDropDownList.SelectedIndex == 0)
                        s.IdSito = null;
                    else
                        s.IdSito = decimal.Parse(SitiDropDownList.SelectedValue);

                    s.CONTROLLO_ANOMALIE = ControlloAnomalieCheckBox.Checked;

                    s.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
					s.AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}";

                    context.SaveChanges();

                    logger.Info($"Modificata stazione id:{s.IdStazione} operatore:{oper.Nome} {oper.Cognome}");

                    for (int r = 0; r < ReteAppartenenzaCheckBoxList.Items.Count; r++)
                    {
                        string descrizione = ReteAppartenenzaCheckBoxList.Items[r].Value;
                        StazioneReti sr = context.StazioniReti.Where(i => i.RETE == descrizione && i.ID_STAZIONE == s.IdStazione).FirstOrDefault();
                        if (ReteAppartenenzaCheckBoxList.Items[r].Selected)
                        {
                            if (sr == null)
                            {
                                StazioneReti nuovaRete = new StazioneReti
                                {
                                    ID_STAZIONE = s.IdStazione,
                                    RETE = descrizione
                                };
                                logger.Debug($"Aggiunta rete:{nuovaRete.RETE} a idStazione:{nuovaRete.ID_STAZIONE}");
                                context.StazioniReti.Add(nuovaRete);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            if (sr != null)
                            {
                                logger.Debug($"Rimozione rete:{sr.RETE} da idStazione:{s.IdStazione}");
                                context.StazioniReti.Remove(sr);
                                context.SaveChanges();
                            }
                        }
                    }

                    for (int f = 0; f < FinalitaCheckBoxList.Items.Count; f++)
                    {
                        string descrizione = FinalitaCheckBoxList.Items[f].Value;
                        FinalitaStazione fs = context.FinalitaStazioni.Where(i => i.FINALITA == descrizione && i.ID_STAZIONE == s.IdStazione).FirstOrDefault();
                        if (FinalitaCheckBoxList.Items[f].Selected)
                        {
                            if (fs == null)
                            {
                                FinalitaStazione nuovaFinalita = new FinalitaStazione
                                {
                                    ID_STAZIONE = s.IdStazione,
                                    FINALITA = descrizione
                                };
                                logger.Debug($"Aggiunta finalita:{nuovaFinalita.FINALITA} a idStazione:{nuovaFinalita.ID_STAZIONE}");
                                context.FinalitaStazioni.Add(nuovaFinalita);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            if (fs != null)
                            {
                                logger.Debug($"Rimozione finalita:{fs.FINALITA} da idStazione:{s.IdStazione}");
                                context.FinalitaStazioni.Remove(fs);
                                context.SaveChanges();
                            }
                        }
                    }

                    Response.Redirect($"VisualizzaStazione.aspx?IdStazione={idStazione}");
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            decimal idStazione = (decimal)ViewState["IdStazione"];
            Response.Redirect($"VisualizzaStazione.aspx?IdStazione={idStazione}");
        }

        protected void EliminaButton_Click(object sender, EventArgs e)
        {
            decimal idStazione = (decimal)ViewState["IdStazione"];

            if (StazioneManager.EsisteUltimaMisurazioneOIntervento(idStazione))
            {
                // Non è possibile eliminare la stazione, ci sono misurazioni e/o interventi
                CustomValidator validatoreEliminazione = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Non è possibile eliminare la stazione: ci sono misurazioni e/o interventi associati."
                };
                Page.Validators.Add(validatoreEliminazione);
                return;
            }

            using (SIASSEntities context = new SIASSEntities())
            {
                // Carica l'oggetto dal modello
                Stazione s = (from j in context.Stazioni
                              where j.IdStazione == idStazione
                              select j).FirstOrDefault();

				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				logger.Info($"Eliminata stazione - id:{s.IdStazione} - Operatore:{oper.Nome} {oper.Cognome}");

                context.Stazioni.Remove(s);
                context.SaveChanges();

                // Elimina eventuale cartella allegati
                string percorsoCartella = HttpContext.Current.Server.MapPath($"~/File/Allegati/Stazione{idStazione}");
                DirectoryInfo cartella = new DirectoryInfo(percorsoCartella);
                if (cartella.Exists)
                    cartella.Delete(true);

                Response.Redirect("~/Stazione/ElencoStazioni.aspx");

            }

        }
    }
}