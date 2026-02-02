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
    public partial class NuovaStazione : System.Web.UI.Page
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
                    // Popola dropdown
                    TipiStazioneDropDownList.DataSource = StazioneManager.TipiStazione();
                    TipiStazioneDropDownList.DataBind();

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
                }
                NuovaStazioneMultiView.SetActiveView(NuovaStazioneView);
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
                bool codiceIdentificativoPresente = false;
                using (SIASSEntities context = new SIASSEntities())
                {
                    // Codice identificativo già in archivio
                    codiceIdentificativoPresente = context.Stazioni.Any(i => i.CodiceIdentificativo.ToUpper() == CodiceIdentificativoTextBox.Text.Trim().ToUpper());
                }

                if (codiceIdentificativoPresente)
                {
                    // Il codice identificativo inserito è già usato per un'altra stazione, chiede conferma.
                    NuovaStazioneMultiView.SetActiveView(CodiceIdentificativoPresenteView);
                }
                else
                {
                    SalvaStazione();
                }
            }
        }

        protected void AnnullaButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Stazione/ElencoStazioni.aspx");
        }

        private void SalvaStazione()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var s = new Stazione
                {
                    CodiceIdentificativo = CodiceIdentificativoTextBox.Text.Trim().ToUpper(),
                    Descrizione = DescrizioneTextBox.Text.Trim(),
                    EsclusaMonitoraggio = EsclusaMonitoraggioCheckBox.Checked,
                    IdTipoStazione = decimal.Parse(TipiStazioneDropDownList.SelectedValue)
                };
                if (string.IsNullOrEmpty(AnnotazioniTextBox.Text.Trim()))
                    s.Annotazioni = null;
                else
                    s.Annotazioni = AnnotazioniTextBox.Text.Trim();
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

                context.Stazioni.Add(s);
                context.SaveChanges();

                for (int r = 0; r < ReteAppartenenzaCheckBoxList.Items.Count; r++)
                {
                    string descrizione = ReteAppartenenzaCheckBoxList.Items[r].Value;

                    if (ReteAppartenenzaCheckBoxList.Items[r].Selected)
                    {
                        StazioneReti nuovaRete = new StazioneReti
                        {
                            ID_STAZIONE = s.IdStazione,
                            RETE = descrizione
                        };
                        context.StazioniReti.Add(nuovaRete);
                        context.SaveChanges();
                    }
                }

                for (int f = 0; f < FinalitaCheckBoxList.Items.Count; f++)
                {
                    string descrizione = FinalitaCheckBoxList.Items[f].Value;

                    if (FinalitaCheckBoxList.Items[f].Selected)
                    {
                        FinalitaStazione nuovaFinalita = new FinalitaStazione
                        {
                            ID_STAZIONE = s.IdStazione,
                            FINALITA = descrizione
                        };
                        context.FinalitaStazioni.Add(nuovaFinalita);
                        context.SaveChanges();
                    }
                }

                logger.Info($"Inserita nuova stazione id:{s.IdStazione} operatore:{oper.Nome} {oper.Cognome}");
                Response.Redirect($"VisualizzaStazione.aspx?IdStazione={s.IdStazione}");
            }
        }

        protected void ConfermaAnnullaButton_Click(object sender, EventArgs e)
        {
            NuovaStazioneMultiView.SetActiveView(NuovaStazioneView);
        }

        protected void ConfermaSalvaButton_Click(object sender, EventArgs e)
        {
            SalvaStazione();
        }
    }
}