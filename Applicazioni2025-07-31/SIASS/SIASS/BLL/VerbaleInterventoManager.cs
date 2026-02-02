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

using Microsoft.Reporting.WebForms;
using NLog;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SIASS.BLL
{
	public static class VerbaleInterventoManager
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public static List<InfoInterventoPerVerbale> ElencoInterventiPerVerbale(List<decimal> elencoIdIntervento)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - {JsonSerializer.Serialize(elencoIdIntervento)}");

			// Nome grandezza spurgo usato per generare il dato di presenza spurgo per il verbale organoclorurati
			// deve coincidere con il valore della tabella SIAS_TIPI_GRANDEZZA
			string nomeGrandezzaSpurgo = ConfigurationManager.AppSettings["NomeGrandezzaSpurgo"];

			var interventi = new List<InfoInterventoPerVerbale>();
			foreach (var idIntervento in elencoIdIntervento)
			{
				var infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
				if (infoIntervento == null)
					throw new ArgumentException($"ElencoInterventiPerVerbale - {idIntervento} non trovato");

				var infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);

				var misurazioniIntervento = InterventoManager.MisurazioniPerIntervento(idIntervento);

				var analitiIntervento = InterventoManager.AnalitiPerIntervento(idIntervento);

				var infoInterventoPerVerbale = new InfoInterventoPerVerbale()
				{
					IdIntervento = infoIntervento.IdIntervento,
					CodiceStazione = infoIntervento.CodiceIdentificativoStazione,
					DescrizioneStazione = infoIntervento.DescrizioneStazione,
					TipoStazione = infoStazione.DescrizioneTipoStazione,
					LocalitaStazione = infoStazione.Localizzazione?.Localita,
					ComuneStazione = infoStazione.Localizzazione?.DenominazioneComune,
					ProvinciaStazione = infoStazione.Localizzazione?.DenominazioneProvincia,
					TipoIntervento = infoIntervento.DescrizioneTipoIntervento,
					Richiedente = infoIntervento.DescrizioneTipoRichiedente,
					Argomento = infoIntervento.DescrizioneArgomento,
					Matrice = infoIntervento.DescrizioneMatrice,
					Operatori = infoIntervento.DescrizioneOperatoriIntervento,
					OperatoriSupporto = infoIntervento.DescrizioneOperatoriSupportoIntervento,
					SiglaVerbale = infoIntervento.SiglaVerbale,
					DataIntervento = infoIntervento.DataIntervento,
					DurataIntervento = infoIntervento.DurataIntervento,
					OraIntervento = infoIntervento.OraIntervento,
					CodiceCampagna = infoIntervento.CodiceCampagna,
					PacchettiAnalisiLaboratorio = String.Join(", ", infoIntervento.PacchettiIntervento
						.OrderBy(i => i.CodicePacchetto)
						.ThenBy(i => i.DescrizionePacchetto)
						.Select(i => i.DescrizionePacchetto)
						.ToArray()),
					FileDati = infoIntervento.FileDati,
					Misurazioni = ElencoMisurazioniPerVerbale(misurazioniIntervento),
					StrumentoUsato = infoIntervento.DescrizioneStrumento,
					FileAngoli = infoIntervento.FileAngoli,
					NumeroCampioni = infoIntervento.NumeroCampioni,
					Annotazioni = infoIntervento.Annotazioni,
					OraFineIntervento = infoIntervento.OraFineIntervento,
					QuotaCampione = infoIntervento.QuotaCampione,
					CampioneBianco = infoIntervento.CampioneBianco,
					AnnotazioniPacchetti = infoIntervento.AnnotazioniPacchetti,
					DescrizioneSito = infoStazione.DescrizioneSito,
					IndirizzoSito = infoStazione.IndirizzoSito,
					ComuneSito = infoStazione.ComuneSito,
					ParteNomeTecnico = infoIntervento.ParteNomeTecnico,
					ParteAziendaTecnico = infoIntervento.ParteAziendaTecnico,
					SeSpurgo = misurazioniIntervento.Exists(m => m.Grandezza == nomeGrandezzaSpurgo && m.Valore == 1),
					DatiCampioneBianco = infoIntervento.DatiCampioneBianco,
					DenominazioneSedeAccettazione = infoIntervento.DenominazioneSedeAccettazione,
					Analiti = ElencoAnalitiPerVerbale(analitiIntervento)
				};

				interventi.Add(infoInterventoPerVerbale);
			}
			return interventi;
		}

		private static string ElencoAnalitiPerVerbale(List<InfoAnalita> analitiIntervento)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var analita in analitiIntervento)
			{
				if (sb.Length > 0) sb.Append(" | ");
				sb.Append($"{analita.Codice} - {analita.Descrizione}");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Generazione del report contenente i verbali degli interventi indicati.
		/// Il tipo di verbale utilizzato è determinato dalla categoria della stazione tranne che per
		/// le stazioni appartenenti alla rete organoclorurati che hanno un verbale specifico
		/// </summary>
		/// <param name="elencoIdIntervento"></param>
		/// <param name="categoriaStazione"></param>
		/// <param name="reteStazione"></param>
		/// <returns></returns>
		private static byte[] GeneraVerbaleInterventi(List<decimal> elencoIdIntervento, string modelloVerbale)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - Id Intervento: {JsonSerializer.Serialize(elencoIdIntervento)} - Modello verbale: {modelloVerbale}");

			using (LocalReport localReport = new LocalReport())
			{
				localReport.ReportPath = $@"rdlc\VerbaliIntervento\{modelloVerbale}.rdlc";

				localReport.DataSources.Add(new ReportDataSource("Interventi", ElencoInterventiPerVerbale(elencoIdIntervento)));
				localReport.DataSources.Add(new ReportDataSource("Misurazioni", InterventoManager.MisurazioniPerIntervento(elencoIdIntervento.FirstOrDefault())));
				localReport.DataSources.Add(new ReportDataSource("Analiti", InterventoManager.AnalitiPerIntervento(elencoIdIntervento.FirstOrDefault())));

				localReport.SetParameters(new ReportParameter[] {
					new ReportParameter("DataVerbale", "Data verbale: " + DateTime.Now.ToString(CostantiGenerali.FORMATO_DATA_ORA_ALLE))
				});

				return localReport.Render("PDF");
			}
		}

		/// <summary>
		/// Generazione del report contenente i verbali degli interventi indicati.
		/// Il tipo di verbale utilizzato è determinato dalla categoria della stazione tranne che per
		/// le stazioni appartenenti alla rete organoclorurati che hanno un verbale specifico
		/// </summary>
		/// <param name="elencoIdIntervento"></param>
		/// <param name="categoriaStazione"></param>
		/// <param name="reteStazione"></param>
		/// <returns></returns>
		private static byte[] GeneraVerbaleInterventiV1(List<decimal> elencoIdIntervento, string modelloVerbale)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - Id Intervento: {JsonSerializer.Serialize(elencoIdIntervento)} - Modello verbale: {modelloVerbale}");

			using (LocalReport localReport = new LocalReport())
			{
				localReport.ReportPath = $@"rdlc\VerbaliInterventoV1\{modelloVerbale}.rdlc";

				localReport.DataSources.Add(new ReportDataSource("Interventi", ElencoInterventiPerVerbale(elencoIdIntervento)));
				localReport.DataSources.Add(new ReportDataSource("Misurazioni", InterventoManager.MisurazioniPerIntervento(elencoIdIntervento.FirstOrDefault())));

				return localReport.Render("PDF");
			}
		}

		/// <summary>
		/// Conversione dell'elenco misurazioni in una stringa con ritorni a capo e gestione dell'unità misura booleana
		/// </summary>
		/// <param name="misurazioni"></param>
		/// <returns></returns>
		private static string ElencoMisurazioniPerVerbale(List<InfoMisurazione> misurazioni)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var misurazione in misurazioni)
			{
				string fonteARPAL = misurazione.FonteArpal ? "Sì" : "No";
				if (sb.Length > 0) sb.Append(Environment.NewLine);
				if (misurazione.SeUnitaMisuraBooleana)
				{
					string valoreMisurazioneBooleana = misurazione.Valore == 1 ? "Sì" : "No";
					sb.Append($"{misurazione.Grandezza} ({misurazione.UnitaMisura}): {valoreMisurazioneBooleana} - Fonte ARPAL: {fonteARPAL}");
				}
				else
					sb.Append($"{misurazione.Grandezza}: {misurazione.Valore} {misurazione.UnitaMisura} - Fonte ARPAL: {fonteARPAL}");
			}
			return sb.ToString();
		}

		public static byte[] GeneraVerbaleIntervento(decimal idIntervento, string modelloVerbale)
		{
			var infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
			if (infoIntervento == null)
				throw new ArgumentException($"GeneraVerbaleIntervento - {idIntervento} non trovato");

			if (infoIntervento.Versione == "V1")
			{
				return GeneraVerbaleInterventiV1(new List<decimal>() { idIntervento }, modelloVerbale);
			}
			else
			{
				return GeneraVerbaleInterventi(new List<decimal>() { idIntervento }, modelloVerbale);
			}

		}
	}
}