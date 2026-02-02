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
using System.Configuration;
using System.Linq;
using System.Web.UI;

namespace SIASS
{
	public partial class MasterPageResponsive : System.Web.UI.MasterPage
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		// Flag utilizzato dalle pagine contenuto per indicare alla master page
		// di nascondere le voci della barra di menù tranne il menù principale
		public bool NascondiVociBarraMenu = false;
		// Flag utilizzato dalle pagine contenuto per indicare alla master page
		// di nascondere la voce della barra di menù per la pagina principale
		public bool NascondiVoceBarraMenuPaginaPrincipale = false;

		// Url della guida per la pagina corrente
		public string UrlGuida = string.Empty;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
				// recupero e verifica presenza informazioni operatore
				if (oper == null)
				{
					OperatoreManager.Logout();
					return;
				}

				if (!String.IsNullOrEmpty(Session["Dispositivo"] as string) && (Session["Dispositivo"].ToString() == "Mobile"))
					PaginaPrincipaleHyperLink.NavigateUrl = "~/Mobile/Default.aspx";
				else
					PaginaPrincipaleHyperLink.NavigateUrl = "~/Stazione/ElencoStazioni.aspx";

				ElencoSitiHyperLink.NavigateUrl = "~/Sito/ElencoSiti.aspx";
				UltimoInterventoUltimaMisurazioneHyperLink.NavigateUrl = "~/Stazione/UltimoInterventoUltimaMisurazione.aspx";
				PacchettiHyperLink.NavigateUrl = "~/PacchettiALIMS.aspx";
				ElencoInterventiPeriodoHyperLink.NavigateUrl = "~/Stazione/Interventi/ElencoInterventiPeriodo.aspx";
				OrganocloruratiHyperLink.NavigateUrl = "~/Organoclorurati/ImportazioniOrganoclorurati.aspx";
				ConfigurazioneHyperLink.NavigateUrl = "~/Configurazione/Configurazione.aspx";
				DatasetHyperLink.NavigateUrl = "~/ConsultazioneDataset/Default.aspx";

				// impostazione pagina uscita
				IntranetARPALHyperLink.NavigateUrl = ConfigurationManager.AppSettings.Get("URLUscita");

				// visualizzazione logon dell'operatore connesso

				LoginOperatoreCollegatoLabel.Text = $"{oper.Nome} {oper.Cognome}";
				// visulizza il nome del profilo (se presente) e nel caso in cui 
				// l'operatore abbia più profili attiva il collegamento alla pagina di selezione
				string nomeProfilo = "[nessun profilo attivo]";

				// Se c'è un profilo attivo e ha descrizione, mostra descrizione, altrimenti codice
				if (!string.IsNullOrWhiteSpace(oper.ProfiloAttivo()))
				{
					if (!string.IsNullOrWhiteSpace(oper.DescrizioneProfiloAttivo()))
						nomeProfilo = $"({oper.DescrizioneProfiloAttivo()})";
					else
						nomeProfilo = $"({oper.ProfiloAttivo()})";
				}

				SelezioneProfiloHyperLink.Text = nomeProfilo;
				SelezioneProfiloHyperLink.Visible = oper.SePossibileSelezioneProfilo;
				ProfiloLabel.Text = SelezioneProfiloHyperLink.Text;
				ProfiloLabel.Visible = !SelezioneProfiloHyperLink.Visible;

				// visualizza la voce della barra menù verso la pagina principale
				if (NascondiVoceBarraMenuPaginaPrincipale)
				{
					PaginaPrincipaleHyperLink.Visible = false;
					ElencoSitiHyperLink.Visible = false;
					UltimoInterventoUltimaMisurazioneHyperLink.Visible = false;
					PacchettiHyperLink.Visible = false;
					ElencoInterventiPeriodoHyperLink.Visible = false;
					ReportisticaHyperLink.Visible = false;
					ReportisticaHyperLink.Visible = false;
					OrganocloruratiHyperLink.Visible = false;
					ConfigurazioneHyperLink.Visible = false;
					DatasetHyperLink.Visible = false;
				}
				else
				{
					PaginaPrincipaleHyperLink.Visible = true;
					ElencoSitiHyperLink.Visible = oper.SeAmministrazione || oper.SeGestione;
					UltimoInterventoUltimaMisurazioneHyperLink.Visible = oper.SeAmministrazione || oper.SeGestione;
					PacchettiHyperLink.Visible = oper.SeAmministrazione || oper.SeGestione;
					ElencoInterventiPeriodoHyperLink.Visible = oper.SeAmministrazione;
					ReportisticaHyperLink.NavigateUrl = oper.LinkReportProfiloAttivo;
					ReportisticaHyperLink.Visible = !String.IsNullOrEmpty(oper.LinkReportProfiloAttivo);
					OrganocloruratiHyperLink.Visible = oper.SeAmministrazione || oper.SeGestoreDitta;
					ConfigurazioneHyperLink.Visible = oper.SeAmministrazione;
					DatasetHyperLink.Visible = oper.SeAmministrazione || oper.SeGestione || oper.SeConsultazioneDataset;
				}
			}
		}
	}
}
