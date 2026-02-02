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

using CsvHelper.Excel;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SIASSImportOrganoclorurati
{
	internal class Program
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		static void Main()
		{
			logger.Info("Inizio elaborazione");

			// Se non ci sono unità di miusra in configurazione non è possiible fare il parsing dei file
			logger.Info("Lettura elenco unità di misura accettate");
			List<string> elencoUnitaMisuraAccettate = DAL.ElencoUnitaMisuraAccettate();
			if (elencoUnitaMisuraAccettate.Count == 0)
			{
				logger.Warn("Non è stata trovata alcuna unità di misura nella tabella sias_organo_anag_param");
				logger.Warn("Esecuzione terminata senza elaborazione");
				return;
			}

			// Elenco importazioni
			var elencoImportazioni = DAL.ElencoImportazioniDaElaborare();
			if (elencoImportazioni.Count == 0)
				logger.Info("Non sono state trovate importazioni da elaborare");
			else
				foreach (var importazione in elencoImportazioni)
					ImportazioneFile(importazione, elencoUnitaMisuraAccettate);
			logger.Info("Elaborazione completata");
		}

		private static void ImportazioneFile(Importazione importazione, List<string> elencoUnitaMisuraAccettate)
		{
			logger.Debug($"Call {MethodBase.GetCurrentMethod().Name} - IdImportazione: {importazione.IdImportazione}");
			try
			{
				string percorsoFileReport = System.IO.Path.Combine(Properties.Settings.Default.PercorsoReportImportazione, $"{importazione.IdImportazione}.txt");

				// Parsing file
				string file = System.IO.Path.Combine(Properties.Settings.Default.PercorsoFileDaImportare, importazione.NomeFile);
				var esitoParsing = ParsingFile(file, elencoUnitaMisuraAccettate);
				if (!esitoParsing.Riuscito)
				{
					string tipoErrore = esitoParsing.ErroreFile == null ? "errore nell'accesso al file" : "uno o più errori di parsing del file";
					logger.Warn($"Parsing fallito ({tipoErrore}): {file}");

					// Registrazione esito importazione
					DAL.AggiornamentoStatoImportazione(importazione.IdImportazione, DAL.IMPORTAZIONE_FALLITA);
					// Salvataggio report su file
					File.WriteAllText(percorsoFileReport, ReportEsitoManager.ParsingFallito(importazione, esitoParsing), System.Text.Encoding.UTF8);
					return;
				}

				// Verifica presenza stazioni per i dati da importare
				List<string> stazioniMancanti = VerificaPresenzaStazioni(esitoParsing.Misurazioni);
				if (stazioniMancanti.Count > 0)
				{
					logger.Warn($"Verifica presenza stazioni per i dati da importare: mancano {stazioniMancanti.Count} stazioni");

					// Registrazione esito importazione
					DAL.AggiornamentoStatoImportazione(importazione.IdImportazione, DAL.IMPORTAZIONE_FALLITA);
					// Salvataggio report su file
					File.WriteAllText(percorsoFileReport, ReportEsitoManager.VerificaPresenzaStazioniFallita(importazione, stazioniMancanti), System.Text.Encoding.UTF8);
					return;
				}

				// Inserimento misurazioni in DB
				var esitoInserimento = InserimentoMisurazioni(importazione, esitoParsing.Misurazioni);

				// Registrazione esito importazione e salvataggio report su file
				if (esitoInserimento.Errore != null)
				{
					DAL.AggiornamentoStatoImportazione(importazione.IdImportazione, DAL.IMPORTAZIONE_FALLITA);
					File.WriteAllText(percorsoFileReport, ReportEsitoManager.InserimentoMisurazioniFallito(importazione), System.Text.Encoding.UTF8);
				}
				else
				{
					DAL.AggiornamentoStatoImportazione(importazione.IdImportazione, DAL.IMPORTAZIONE_RIUSCITA);
					File.WriteAllText(percorsoFileReport, ReportEsitoManager.ImportazioneRiuscita(importazione, esitoInserimento), System.Text.Encoding.UTF8);
				}

			}
			catch (Exception ex)
			{
				string errore = Utils.GetExceptionDetails(ex);
				logger.Error(errore);
				DAL.AggiornamentoStatoImportazione(importazione.IdImportazione, DAL.IMPORTAZIONE_FALLITA);
			}
		}

		/// <summary>
		/// Verifica presenza in DB delle stazioni presenti nei dati (compreso il codice sito)
		/// Il codice sito in cui si trova la stazione corrisponde alla proprietà "sito" dei dati da importare
		/// Il codice stazione corrisponde alla proprietà "piezometro" dei dati da importare
		/// I valori di sito e piezometro sono convertiti in maiuscolo durante il parsing
		/// </summary>
		/// <param name="misurazioni"></param>
		/// <returns></returns>
		private static List<string> VerificaPresenzaStazioni(List<MisurazioneOrganoclorurati> misurazioni)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name}");

			var stazioniMancanti = new List<string>();

			// Elenco dei codici sito e stazione estratti dai dati
			var codiciStazioniPresentiNeiDati = misurazioni
				.Select(i =>
				new
				{
					CodiceIdentificativoSito = i.Sito,
					CodiceIdentificativoStazione = i.Piezometro
				})
				.Distinct()
				.ToList();

			// Verifica presenza su DB
			foreach (var codiceStazione in codiciStazioniPresentiNeiDati)
			{
				if (!DAL.EsisteStazione(codiceStazione.CodiceIdentificativoStazione, codiceStazione.CodiceIdentificativoSito))
				{
					string codiceSitoEStazione = $"{codiceStazione.CodiceIdentificativoSito} - {codiceStazione.CodiceIdentificativoStazione}";
					logger.Warn($"Stazione non presente in base dati: {codiceSitoEStazione}");
					stazioniMancanti.Add(codiceSitoEStazione);
				}
			}

			return stazioniMancanti;
		}

		private static EsitoInserimento InserimentoMisurazioni(Importazione importazione, List<MisurazioneOrganoclorurati> misurazioni)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name}");

			var esito = new EsitoInserimento();

			try
			{
				foreach (var misurazione in misurazioni)
				{
					// Si prova prima ad aggiornare il record
					// se l'aggiornamento ha successo il record è registrato altrimenti inserito
					if (DAL.AggiornamentoMisurazione(misurazione, importazione.IdImportazione))
						esito.MisurazioniAggiornate.Add(misurazione);
					else
						DAL.InserimentoMisurazione(misurazione, importazione.IdImportazione);
				}
				esito.NumeroMisurazioniImportate = misurazioni.Count;
				logger.Debug($"Misurazioni inserite: {misurazioni.Count}, di cui {esito.MisurazioniAggiornate.Count} aggiornate");
			}
			catch (Exception ex)
			{
				string errore = Utils.GetExceptionDetails(ex);
				logger.Error(errore);
				esito.Errore = errore;
			}

			return esito;
		}

		private static EsitoParsingFile ParsingFile(string file, List<string> elencoUnitaMisuraAccettate)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - File: {file}");

			#region Indice a base zero delle colonne del file Excel

			const int SITO = 0;
			const int PIEZOMETRO = 1;
			const int DATA_CAMPIONAMENTO = 2;
			const int FONTE = 3;
			const int PARAMETRO = 4;
			const int CONC_SIGN = 5;
			const int CONC_VAL = 6;
			const int LOQ = 7;
			const int INCERTEZZA = 8;
			const int CONC_UDM = 9;

			#endregion

			var esito = new EsitoParsingFile();

			if (!File.Exists(file))
			{
				string errore = $"File non trovato";
				logger.Error(errore);
				esito.ErroreFile = errore;
				return esito;
			}

			try
			{
				using (ExcelParser parser = new ExcelParser(file))
				{
					parser.Read();
					int numeroRigaCorrente = 1;
					while (parser.Read())
					{
						bool recordValido = true;
						logger.Debug($"Parsing riga: {++numeroRigaCorrente}");
						var misurazione = new MisurazioneOrganoclorurati
						{
							NumeroRigaFile = numeroRigaCorrente
						};

						// sito - obbligatorio
						if (String.IsNullOrEmpty(parser[SITO]))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {SITO + 1}: valore 'sito' mancante";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							misurazione.Sito = parser[SITO].Trim().ToUpper();

						// piezometro - obbligatorio
						if (String.IsNullOrEmpty(parser[PIEZOMETRO]))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {PIEZOMETRO + 1}: valore 'piezometro' mancante";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							misurazione.Piezometro = parser[PIEZOMETRO].Trim().ToUpper();

						// data_campionamento - obbligatoria
						if (!DateTime.TryParse(parser[DATA_CAMPIONAMENTO], out DateTime valoreDataCampionamento))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {DATA_CAMPIONAMENTO + 1}: valore 'data_campionamento' mancante o non valido";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							misurazione.DataCampionamento = valoreDataCampionamento;

						// fonte - obbligatoria
						if (String.IsNullOrEmpty(parser[FONTE]))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {FONTE + 1}: valore 'fonte' mancante";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							misurazione.Fonte = parser[FONTE].Trim().ToUpper();

						// parametro - obbligatorio
						if (String.IsNullOrEmpty(parser[PARAMETRO]))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {PARAMETRO + 1}: valore 'parametro' mancante";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							misurazione.Parametro = parser[PARAMETRO].Trim().ToLower();

						// conc_sign
						if (!String.IsNullOrEmpty(parser[CONC_SIGN]))
							misurazione.ConcSign = parser[CONC_SIGN].Trim();

						// conc_val - obbligatorio
						if (!Decimal.TryParse(parser[CONC_VAL], NumberStyles.Any, CultureInfo.CurrentCulture, out decimal valoreConcVal))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {CONC_VAL + 1}: valore 'conc_val' mancante o non valido";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							misurazione.ConcVal = valoreConcVal;

						// loq (se vale NA è considerato null)
						if (!String.IsNullOrEmpty(parser[LOQ]) && parser[LOQ] != "NA")
						{
							if (!Decimal.TryParse(parser[LOQ], NumberStyles.Any, CultureInfo.CurrentCulture, out decimal valoreLoq))
							{
								string errore = $"Riga {numeroRigaCorrente} Colonna {LOQ + 1}: valore 'loq' non valido";
								logger.Warn(errore);
								esito.ErroriParsing.Add(errore);
								recordValido = false;
							}
							else
								misurazione.Loq = valoreLoq;
						}

						// incertezza (se vale NA è considerato null)
						if (!String.IsNullOrEmpty(parser[INCERTEZZA]) && parser[INCERTEZZA] != "NA")
						{
							if (!Decimal.TryParse(parser[INCERTEZZA], NumberStyles.Any, CultureInfo.CurrentCulture, out decimal valoreIncertezza))
							{
								string errore = $"Riga {numeroRigaCorrente} Colonna {INCERTEZZA + 1}: valore 'incertezza' non valido";
								logger.Warn(errore);
								esito.ErroriParsing.Add(errore);
								recordValido = false;
							}
							else
								misurazione.Incertezza = valoreIncertezza;
						}

						// conc_udm
						if (!String.IsNullOrEmpty(parser[CONC_UDM]))
						{
							// Verifica che l'unità di misura sia tra quelle in configurazione
							string conc_udm = parser[CONC_UDM].Trim();
							if (!elencoUnitaMisuraAccettate.Contains(conc_udm))
							{
								string errore = $"Riga {numeroRigaCorrente} Colonna {CONC_UDM + 1}: valore 'CONC_UDM'='{conc_udm}' non trovato in configurazione nella tabella SIAS_ORGANO_ANAG_PARAM";
								logger.Warn(errore);
								esito.ErroriParsing.Add(errore);
								recordValido = false;
							}
							else
							{
								misurazione.ConcUdm = conc_udm;
							}
						}

						if (recordValido)
							esito.Misurazioni.Add(misurazione);
						else
							logger.Warn($"Riga {numeroRigaCorrente} non valida");
					}
				}
			}
			catch (Exception ex)
			{
				string errore = Utils.GetExceptionDetails(ex);
				logger.Error(errore);
				esito.ErroreFile = ex.Message;
			}

			return esito;
		}
	}
}
