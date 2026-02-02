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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIASS
{
    public partial class VisualizzaDatiAlimsIntervento : System.Web.UI.Page
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
			if (!(oper.SeAmministrazione || oper.SeGestione || oper.SeGestoreDitta))
			{
				OperatoreManager.Logout();
				return;
			}

			if (!Page.IsPostBack)
            {
                if (!decimal.TryParse(Request.QueryString["IdIntervento"], out decimal idIntervento))
                {
                    logger.Debug($"Parametro IdIntervento mancante");
                    Response.Write($"Parametro IdIntervento mancante");
                    Response.End();
                    return;
                }

                InfoIntervento infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);

                if (infoIntervento == null)
                {
                    Response.Write($"Intervento non trovato: IdIntervento={idIntervento}");
                    Response.End();
                    return;
                }

				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);
				if (infoStazione == null)
				{
					Response.Write($"Stazione non trovata: IdStazione={infoIntervento.IdStazione}");
					Response.End();
					return;
				}

				if (oper.SeGestoreDitta && !oper.SeCreazioneInterventoStazione(infoStazione.DatiAmministrativi.PartitaIVAGestore, null))
				{
					OperatoreManager.Logout();
					return;
				}

				HeaderInterventoResponsive1.PopolaCampi(infoIntervento);

                DataTable dtParametri = ALIMSManager.ParametriPerStazioneEDataIntervento(infoIntervento.CodiceIdentificativoStazione, infoIntervento.DataIntervento);

                // Cicla le colonne e genera dinamicamente le corrispondenti nella tabella cmabiano i nomi delle intestazioni
                foreach (DataColumn col in dtParametri.Columns)
                {
                    BoundField bField = new BoundField
                    {
                        DataField = col.ColumnName
                    };
                    if (col.Ordinal == 0)
                        bField.HeaderText = "Parametro/Data intervento";
                    else
                        bField.HeaderText = col.ColumnName.Replace("D", "").Replace("_", "/");
                    // Segna la colonna corrispondente all'intervento selezionato
                    if (infoIntervento.DataIntervento.ToString("dd/MM/yyyy") == bField.HeaderText)
                        ViewState.Add("IndiceColonnaInterventoCorrente", col.Ordinal);
                    DatiAlimsGridView.Columns.Add(bField);
                }

                DatiAlimsGridView.DataSource = dtParametri;
                DatiAlimsGridView.DataBind();

                InterventiStazioneHyperLink.NavigateUrl = $"ElencoInterventi.aspx?IdStazione={infoIntervento.IdStazione}";
            }

        }

        protected void DatiAlimsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (ViewState["IndiceColonnaInterventoCorrente"] == null)
                return;

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                // Cambia il colore della colonna corrispondente all'intervento selezionato
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i == (int)ViewState["IndiceColonnaInterventoCorrente"])
                        e.Row.Cells[i].BackColor = System.Drawing.Color.LightCyan;
                }
            }
        }
    }
}