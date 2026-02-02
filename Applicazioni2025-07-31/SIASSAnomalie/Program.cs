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

using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using SIASSAnomalie.Features;
using SIASSAnomalie.Model;
using System.Configuration;
using System.Reflection;

namespace SIASSAnomalie
{
	internal static class Program
	{
		static void Main()
		{
			//var test = new TEST();
			//test.PROVA();
			//return;

			// Caricamento configurazione da appsettings.json
			var configurationRoot = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			// Lettura configurazione NLog
			LogManager.Configuration = new NLogLoggingConfiguration(configurationRoot.GetSection("NLog"));

			// Creazione logger
			var logger = LogManager.GetCurrentClassLogger();

			try
			{
				logger.Info($"Avvio programma - Versione: {Utils.Version()} - Hostname: {System.Net.Dns.GetHostName()}");

				// Lettura impostazioni di configurazione da appsettings.json in un'istanza di Configurazione
				var configurazione = configurationRoot.GetSection("Configurazione").Get<Configurazione>() ?? throw new ConfigurationErrorsException("Sezione 'Configurazione' mancante in appsettings");
				// Validazione impostazioni di configurazione
				configurazione.Valida();

				// Legge l'elenco dei controlli da elaborare
				using var context = new SIASSContext(configurazione.StringaConnessioneDB);
				var elencoControlli = context.SIAS_CONTROLLI_ANOMALIE.Where(i => i.ABILITATO).ToList();

				if (elencoControlli.Count == 0)
				{
					logger.Info("Nessun controllo da elaborare trovato.");
				}
				else
				{
					foreach (var controllo in elencoControlli)
					{
						logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' NomeControllo:'{controllo.NOME_CONTROLLO}'");
						// Riceva gli id grandezza stazione a cui applicare il controllo
						var elencoStazioneEGrandezza = ElencoStazioneEGrandezza(controllo, configurazione.StringaConnessioneDB);

						// Esegue il controllo
						switch (controllo.CODICE_FUNZIONE)
						{
							case "TEST":
								var controlloTEST = new Features.ControlloTEST();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloTEST.Risposta risposta = controlloTEST.EsegueControllo(
										new ControlloTEST.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case


							case "SoggiacenzaNegativa":
								var controlloSoggiacenzaNegativa = new Features.ControlloSoggiacenzaNegativa();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloSoggiacenzaNegativa.Risposta risposta = controlloSoggiacenzaNegativa.EsegueControllo(
										new ControlloSoggiacenzaNegativa.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							case "TrasmissioneDati":
								var controlloTrasmissioneDati = new Features.ControlloTrasmissioneDati();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloTrasmissioneDati.Risposta risposta = controlloTrasmissioneDati.EsegueControllo(
										new ControlloTrasmissioneDati.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							case "PlateauSoggiacenza":
								var controlloPlateauSoggiacenza = new Features.ControlloPlateauSoggiacenza();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloPlateauSoggiacenza.Risposta risposta = controlloPlateauSoggiacenza.EsegueControllo(
										new ControlloPlateauSoggiacenza.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							case "SpikeIstantaneo":
								var controlloSpikeIstantaneo = new Features.ControlloSpikeIstantaneo();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloSpikeIstantaneo.Risposta risposta = controlloSpikeIstantaneo.EsegueControllo(
										new ControlloSpikeIstantaneo.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							case "SpikeIstantaneoNum":
								var controlloSpikeIstantaneoNumeroMisurazioni = new Features.ControlloSpikeIstantaneoNumeroMisurazioni();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloSpikeIstantaneoNumeroMisurazioni.Risposta risposta = controlloSpikeIstantaneoNumeroMisurazioni.EsegueControllo(
										new ControlloSpikeIstantaneoNumeroMisurazioni.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							case "OutlierSoggiacenza":
								var controlloOutlierSoggiacenza = new Features.ControlloOutlierSoggiacenza();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloOutlierSoggiacenza.Risposta risposta = controlloOutlierSoggiacenza.EsegueControllo(
										new ControlloOutlierSoggiacenza.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							case "EscursionePozzo":
								var controlloEscursionePozzo = new Features.ControlloEscursionePozzo();
								foreach (var sg in elencoStazioneEGrandezza)
								{
									logger.Debug($"Controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}' per IDStazione:{sg.IdStazione} IdGrandezzaStazione:{sg.IdGrandezzaStazione}");
									ControlloEscursionePozzo.Risposta risposta = controlloEscursionePozzo.EsegueControllo(
										new ControlloEscursionePozzo.Richiesta()
										{
											IdGrandezzaStazione = sg.IdGrandezzaStazione,
											ConfigurazioneJSON = controllo.CONFIGURAZIONE_JSON,
											StringaConnessioneDB = configurazione.StringaConnessioneDB
										}
										);

									SalvataggioEsito(context, controllo.ID_CONTROLLO, risposta.DescrizioneEsito);

									if (risposta.EsecuzioneRiuscita)
									{
										logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': riuscita.");
										if (risposta.PresenzaAnomalia)
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': rilevata anomalia.");
											InserimentoAnomalia(context, controllo.ID_CONTROLLO, sg.IdStazione, risposta.Valore, risposta.DataValore);
										}
										else
										{
											logger.Debug($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': nessuna anomalia.");
										}
									}
									else
									{
										logger.Error($"Esecuzione controllo '{controllo.CODICE_FUNZIONE}': fallita.");
										break; // esce dal ciclo delle grandezzestazioni
									}
								}
								break; // case

							default:
								logger.Error($"Codice funzione non corrispondente: '{controllo.CODICE_FUNZIONE}'");
								break;
						}
						logger.Debug($"Fine controllo CodiceFunzione: '{controllo.CODICE_FUNZIONE}'");
					}
				}

				logger.Info("Elaborazione completata");
			}
			catch (Exception exception)
			{
				logger.Error(Utils.GetExceptionDetails(exception));
				Environment.ExitCode = -1;
			}
			finally
			{
				logger.Info("Flush messaggi di log");
				LogManager.Flush();
			}
		}

		public static void SalvataggioEsito(SIASSContext context, decimal idControllo, string esito)
		{
			var logger = LogManager.GetCurrentClassLogger();
			logger.Debug(MethodBase.GetCurrentMethod().Name);

			logger.Debug($"Salvataggio esito idControllo:{idControllo}");

			var controllo = context.SIAS_CONTROLLI_ANOMALIE.Where(i => i.ID_CONTROLLO == idControllo).FirstOrDefault();
			if (controllo == null)
			{
				logger.Warn($"Controllo non trovato id_controllo:{idControllo}");
				return;
			}

			controllo.ESITO_ULTIMA_ESECUZIONE = esito;
			controllo.DATA_ULTIMA_ESECUZIONE = DateTime.Now;
			context.SaveChanges();
		}

		public static void InserimentoAnomalia(SIASSContext context, decimal idControllo, decimal idStazione, decimal? valore, DateTime? dataValore)
		{
			var logger = LogManager.GetCurrentClassLogger();
			logger.Debug(MethodBase.GetCurrentMethod().Name);

			logger.Debug($"Inserimento anomalia idControllo:{idControllo} idStazione:{idStazione}");

			var anomalia = new SIAS_ANOMALIE();
			anomalia.ID_CONTROLLO = idControllo;
			anomalia.ID_STAZIONE = idStazione;
			anomalia.DATA_CONTROLLO = DateTime.Now;
			anomalia.VALORE = valore;
			anomalia.DATA_VALORE = dataValore;
			context.SIAS_ANOMALIE.Add(anomalia);
			context.SaveChanges();

			logger.Debug($"Inserita anomalia IdAnomalia:{anomalia.ID_ANOMALIA}");
		}

		/// <summary>
		/// Elenco degli id stazioni a cui applicare il controllo
		/// </summary>
		/// <param name="controlloAnomalia"></param>
		/// <param name="stringaConnessioneDB"></param>
		/// <returns></returns>
		private static List<decimal> ElencoIdStazioni(SIAS_CONTROLLI_ANOMALIE controlloAnomalia, string stringaConnessioneDB)
		{
			var logger = LogManager.GetCurrentClassLogger();
			logger.Debug(MethodBase.GetCurrentMethod().Name);
			// Legge l'elenco delle stazioni da elaborare
			using var context = new SIASSContext(stringaConnessioneDB);
			var elencoStazioni = context.SIASS_STAZIONI.Where(i => i.CONTROLLO_ANOMALIE == true).ToList();
			var elencoIDStazioniPerRete = context.SIAS_STAZIONI_RETI.Where(i => i.RETE == controlloAnomalia.RETE).Select(i => i.ID_STAZIONE.Value).ToList();
			elencoStazioni = elencoStazioni.Where(i => elencoIDStazioniPerRete.Contains(i.ID_STAZIONE)).ToList();
			if (controlloAnomalia.ID_TIPO_STAZIONE != null)
			{
				elencoStazioni = elencoStazioni.Where(i => i.ID_TIPO_STAZIONE == controlloAnomalia.ID_TIPO_STAZIONE.Value).ToList();
			}
			return elencoStazioni.Select(i => i.ID_STAZIONE).ToList();
		}



		private static List<StazioneEGrandezza> ElencoStazioneEGrandezza(SIAS_CONTROLLI_ANOMALIE controlloAnomalia, string stringaConnessioneDB)
		{
			var logger = LogManager.GetCurrentClassLogger();
			logger.Debug(MethodBase.GetCurrentMethod().Name);
			var elencoIdStazioni = ElencoIdStazioni(controlloAnomalia, stringaConnessioneDB);

			var elencoStazioneEGrandezza = new List<StazioneEGrandezza>();

			using var context = new SIASSContext(stringaConnessioneDB);
			foreach (var idStazione in elencoIdStazioni)
			{
				logger.Debug($"Ricerca IdGrandezzaStazione per IdStazione:{idStazione}");
				var elencoGrandezzeStazione = context.SIAS_GRANDEZZE_STAZIONE
					.Where(
					i => i.GRANDEZZA == controlloAnomalia.GRANDEZZA
					&&
					i.UNITA_MISURA == controlloAnomalia.NOME_UNITA_MISURA
					&&
					i.ID_STAZIONE == idStazione
					)
					.ToList();
				if (elencoGrandezzeStazione.Any())
				{
					decimal? idGrandezzaStazione = elencoGrandezzeStazione.Select(i => i.ID_GRANDEZZA_STAZIONE).FirstOrDefault();
					if (idGrandezzaStazione != null)
					{
						logger.Debug($"Trovata IdGrandezzaStazione:{idGrandezzaStazione} per IdStazione:{idStazione}");
						var sg = new StazioneEGrandezza();
						sg.IdStazione = idStazione;
						sg.IdGrandezzaStazione = idGrandezzaStazione.Value;
						elencoStazioneEGrandezza.Add(sg);
					}
				}
				else
				{
					logger.Debug($"Nessuna IdGrandezzaStazione per IdStazione:{idStazione}");
				}
			}
			return elencoStazioneEGrandezza;
		}

		public class StazioneEGrandezza
		{
			public decimal IdStazione { get; set; }
			public decimal IdGrandezzaStazione { get; set; }
		}

	}
}
