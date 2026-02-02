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

using ApiAnagraficheArpal;
using FluentResults;
using Microsoft.Reporting.WebForms;
using NLog;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web.UI.WebControls.Expressions;
using static SIASS.BLL.InformazioniFileImportazione;

namespace SIASS.BLL
{
	public static class InterventoManager
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		#region Tipologiche 

		public static List<TipoIntervento> TipiIntervento()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiIntervento.ToList();
				return elenco;
			}
		}

		public static List<TipoRichiedente> TipiRichiedente()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiRichiedente.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE_TIPO_RICHIEDENTE);
				return elenco.ToList();
			}
		}

		public static List<TipoStrumentoIntervento> TipiStrumentoIntervento()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiStrumentoIntervento.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE_STRUMENTO);
				return elenco.ToList();
			}
		}

		public static List<TipoSedeAccettazione> TipiSedeAccettazione()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiSedeAccettazione.OrderBy(i => i.DENOMINAZIONE_SEDE);
				return elenco.ToList();
			}
		}

		public static List<TipoArgomento> TipiArgomento()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiArgomento.OrderBy(i => i.Ordine).ThenBy(i => i.Descrizione);
				return elenco.ToList();
			}
		}

		#endregion Tipologiche 

		#region Operatori intervento

		public static List<InfoIntervento.InfoOperatoreIntervento> ElencoOperatoriIntervento(decimal idIntervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = from operatoreIntervento in context.OperatoriInterventi
							 where operatoreIntervento.ID_INTERVENTO == idIntervento
							 orderby operatoreIntervento.DESCRIZIONE_OPERATORE
							 select new InfoIntervento.InfoOperatoreIntervento
							 {
								 IdOperatoreIntervento = operatoreIntervento.ID_OPERATORE_INTERVENTO,
								 IdOperatore = operatoreIntervento.ID_OPERATORE,
								 IdIntervento = operatoreIntervento.ID_INTERVENTO,
								 DescrizioneOperatore = operatoreIntervento.DESCRIZIONE_OPERATORE
							 };
				return elenco.ToList();
			}
		}

		public static List<InfoIntervento.InfoOperatoreIntervento> ElencoOperatoriSupportoIntervento(decimal idIntervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = from operatoreSupportoIntervento in context.OperatoriSupportoInterventi
							 where operatoreSupportoIntervento.ID_INTERVENTO == idIntervento
							 orderby operatoreSupportoIntervento.DESCRIZIONE_OPERATORE
							 select new InfoIntervento.InfoOperatoreIntervento
							 {
								 IdOperatoreIntervento = operatoreSupportoIntervento.ID_OPERATORE_INTERV_SUP,
								 IdOperatore = operatoreSupportoIntervento.ID_OPERATORE,
								 IdIntervento = operatoreSupportoIntervento.ID_INTERVENTO,
								 DescrizioneOperatore = operatoreSupportoIntervento.DESCRIZIONE_OPERATORE
							 };
				return elenco.ToList();
			}
		}

		/// <summary>
		/// Elenco degli operatori selezionabili in un intervento
		/// </summary>
		/// <returns></returns>
		public static List<InfoIntervento.InfoOperatoreIntervento> ElencoOperatoriPerIntervento()
		{
			var utenti = GSOManager.UtentiPerApplicazione(ConfigurationManager.AppSettings["IdentificativoApplicativo"]);
			string profiliGSO = ConfigurationManager.AppSettings["ProfiliGSOOperatoriInterventi"];

			utenti = utenti.Where(i => profiliGSO.Split(';').ToList().Contains(i.DescrizioneProfilo)).ToList();
			List<InfoIntervento.InfoOperatoreIntervento> elencoOperatoriPerIntervento = new List<InfoIntervento.InfoOperatoreIntervento>();
			foreach (var utente in utenti)
			{
				InfoIntervento.InfoOperatoreIntervento op = new InfoIntervento.InfoOperatoreIntervento
				{
					DescrizioneOperatore = utente.Cognome + " " + utente.Nome,
					IdOperatore = utente.IdUtente
				};
				if (!elencoOperatoriPerIntervento.Exists(i => i.IdOperatore == op.IdOperatore))
					elencoOperatoriPerIntervento.Add(op);
			}
			return elencoOperatoriPerIntervento.OrderBy(i => i.DescrizioneOperatore).ToList();
		}

		#endregion Operatori intervento

		public static List<InfoIntervento> ElencoInterventiStazione(decimal idStazione, string codiceOrganizzazione = null)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = (from intervento in context.Interventi
							  where intervento.ID_STAZIONE == idStazione
							  orderby intervento.DATA_INTERVENTO descending, intervento.TipoIntervento.DESCRIZIONE_TIPO_INTERVENTO
							  select new InfoIntervento
							  {
								  IdIntervento = intervento.ID_INTERVENTO,
								  IdStazione = intervento.ID_STAZIONE,
								  IdTipoIntervento = intervento.ID_TIPO_INTERVENTO,
								  DescrizioneTipoIntervento = intervento.TipoIntervento.DESCRIZIONE_TIPO_INTERVENTO,
								  IdTipoRichiedente = intervento.ID_TIPO_RICHIEDENTE,
								  DescrizioneTipoRichiedente = (intervento.ID_TIPO_RICHIEDENTE == null ? null : intervento.TipoRichiedente.DESCRIZIONE_TIPO_RICHIEDENTE),
								  CodiceArgomento = intervento.CODICE_ARGOMENTO,
								  DescrizioneArgomento = intervento.DESCRIZIONE_ARGOMENTO,
								  CodiceMatrice = intervento.CODICE_MATRICE,
								  DescrizioneMatrice = intervento.DESCRIZIONE_MATRICE,
								  DataIntervento = intervento.DATA_INTERVENTO,
								  OraIntervento = intervento.ORA_INTERVENTO,
								  DurataIntervento = intervento.DURATA_INTERVENTO,
								  CodiceCampagna = intervento.CODICE_CAMPAGNA,
								  FileDati = intervento.FILE_DATI,
								  FileAngoli = intervento.FILE_ANGOLI,
								  IdStrumento = intervento.ID_STRUMENTO,
								  DescrizioneStrumento = (intervento.ID_STRUMENTO == null ? null : intervento.TipoStrumentoIntervento.DESCRIZIONE_STRUMENTO),
								  UltimoAggiornamento = intervento.ULTIMO_AGGIORNAMENTO,
								  AutoreUltimoAggiornamento = intervento.AUTORE_ULTIMO_AGGIORNAMENTO,
								  NumeroCampioni = intervento.NUMERO_CAMPIONI,
								  Annotazioni = intervento.ANNOTAZIONI,
								  CodiceIdentificativoStazione = intervento.Stazione.CodiceIdentificativo,
								  OrganizzazioneCreazione = intervento.ORGANIZZAZIONE_CREAZIONE,
								  StatoInvioVerbaleV1 = intervento.STATO_INVIO_VERBALE_V1,
								  Versione = intervento.VERSIONE,
								  PrelievoCampioni = intervento.PRELIEVO_CAMPIONI
							  }).ToList();

				if (!string.IsNullOrWhiteSpace(codiceOrganizzazione))
				{
					elenco = elenco.Where(i => i.OrganizzazioneCreazione.ToUpper() == codiceOrganizzazione.ToUpper()).ToList();
				}

				var codiceStazione = context.Stazioni.First(i => i.IdStazione == idStazione).CodiceIdentificativo;
				var interventiAlims = context.AlimsInterventi.Where(i => i.STAZIONE == codiceStazione).ToList();
				foreach (InfoIntervento i in elenco)
				{
					// Aggiunge gli operatori
					i.OperatoriIntervento = ElencoOperatoriIntervento(i.IdIntervento);
					i.OperatoriSupportoIntervento = ElencoOperatoriSupportoIntervento(i.IdIntervento);

					// Aggiunge Codice Campione e RDP da ALIMS
					var elencoCodiceCampioneRDPDaInterventiAlims = interventiAlims
						.Where(ai => ai.SIGLA_VERBALE.ToUpper() == i.SiglaVerbale.ToUpper())
						.Select(j => new { j.CODICE_CAMPIONE, j.RDP })
						.OrderBy(j => j.CODICE_CAMPIONE);
					var stringBuilder = new StringBuilder();
					foreach (var ai in elencoCodiceCampioneRDPDaInterventiAlims)
						stringBuilder.Append($"{ai.CODICE_CAMPIONE} / {ai.RDP}<br />");
					i.ElencoCodiceCampioneRDP = stringBuilder.ToString();
				}
				return elenco.ToList();
			}
		}

		public static List<InfoInterventoPerElenco> ElencoInterventiPeriodo(DateTime dataInizioPeriodo, DateTime dataFinePeriodo)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				return (from intervento in context.Interventi
						where DbFunctions.TruncateTime(intervento.DATA_INTERVENTO).Value >= dataInizioPeriodo.Date
						&&
						DbFunctions.TruncateTime(intervento.DATA_INTERVENTO).Value <= dataFinePeriodo.Date
						orderby intervento.DATA_INTERVENTO descending
						select new InfoInterventoPerElenco
						{
							IdIntervento = intervento.ID_INTERVENTO,
							IdStazione = intervento.ID_STAZIONE,
							CodiceIdentificativoStazione = intervento.Stazione.CodiceIdentificativo,
							DescrizioneStazione = intervento.Stazione.Descrizione,
							DescrizioneTipoIntervento = intervento.TipoIntervento.DESCRIZIONE_TIPO_INTERVENTO,
							DataIntervento = intervento.DATA_INTERVENTO,
							OraIntervento = intervento.ORA_INTERVENTO,
							DurataIntervento = intervento.DURATA_INTERVENTO
						}).ToList();
			}
		}

		public static InfoIntervento UltimoIntervento(decimal idStazione, SIASSEntities context)
		{
			Intervento interv = context.Interventi.OrderByDescending(i => i.DATA_INTERVENTO).FirstOrDefault(i => i.ID_STAZIONE == idStazione);
			if (interv != null)
				return CaricaInfoIntervento(interv.ID_INTERVENTO);
			else
				return null;
		}

		/// <summary>
		/// Misurazioni associate all'intervento
		/// </summary>
		/// <param name="idIntervento"></param>
		/// <returns></returns>
		public static List<InfoMisurazione> MisurazioniPerIntervento(decimal idIntervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.Misurazioni2021
					.Where(i => i.ID_INTERVENTO == idIntervento)
					.Select(i => new InfoMisurazione()
					{
						IdMisurazione = i.ID_MISURAZIONE,
						Grandezza = i.Grandezza.GRANDEZZA,
						UnitaMisura = i.Grandezza.UNITA_MISURA,
						DataMisurazione = i.DATA_MISURAZIONE,
						Valore = i.VALORE,
						IdGrandezzaStazione = i.ID_GRANDEZZA_STAZIONE,
						CodiceIdentificativoSensore = i.CODICE_IDENTIFICATIVO_SENSORE,
						CodicePMC = i.SIAS_SENSORI.CODICE_PMC,
						FonteArpal = i.FONTE_ARPAL,
						SeUnitaMisuraBooleana = i.Grandezza.TipoUnitaMisura.SE_BOOLEANO,
						NumeroDecimali = i.Grandezza.NUMERO_DECIMALI,
						CodicePacchetto = i.PACCHETTO_CODICE
					})
					.OrderBy(i => i.Grandezza)
					.ToList();

				// Se il codice PMC non è presente nel sensore è utilizzato quello associato al tipo grandezza (se esiste)
				foreach (var misurazione in elenco.Where(i => i.CodicePMC == null))
				{
					var grandezza = context.TipiGrandezza.FirstOrDefault(i => i.NOME_GRANDEZZA == misurazione.Grandezza);
					if (grandezza != null && grandezza.PMC_USUALE != null)
						misurazione.CodicePMC = grandezza.PMC_USUALE;
				}

				return elenco;
			}
		}

		#region PDF Interventi stazione

		/// <summary>
		/// PDF per gli interventi della stazione indicata
		/// </summary>
		/// <param name="idStazione"></param>
		/// <returns></returns>
		public static byte[] GeneraTuttiPDFInterventiStazione(decimal idStazione, string codiceOrganizzazione = null)
		{
			return GeneraSingoloPDFInterventi(ElencoInterventiStazione(idStazione, codiceOrganizzazione)
				.OrderBy(i => i.DataIntervento)
				.Select(i => i.IdIntervento)
				.ToList());
		}

		/// <summary>
		/// Generazione del report contenente i PPDF degli interventi indicati.
		/// </summary>
		/// <param name="elencoIdIntervento"></param>
		/// <returns></returns>
		private static byte[] GeneraSingoloPDFInterventi(List<decimal> elencoIdIntervento)
		{
			using (LocalReport localReport = new LocalReport())
			{
				localReport.ReportPath = $@"rdlc\SchedaInterventi.rdlc";
				localReport.DataSources.Add(new ReportDataSource("Interventi", VerbaleInterventoManager.ElencoInterventiPerVerbale(elencoIdIntervento)));
				return localReport.Render("PDF");
			}
		}

		#endregion PDF Interventi stazione

		public static void AggiornaModelloVerbaleSelezionato(decimal idIntervento, string modelloVerbale)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - IdIntervento: {idIntervento} ModelloVerbale: {modelloVerbale}");
			using (SIASSEntities context = new SIASSEntities())
			{
				var intervento = context.Interventi.FirstOrDefault(i => i.ID_INTERVENTO == idIntervento) ??
					throw new ArgumentException($"Intervento con id {idIntervento} non trovato.");
				intervento.MOD_VERBALE_SELEZIONATO = modelloVerbale;
				context.SaveChanges();
			}
		}

		internal static List<InfoAnalita> AnalitiPerIntervento(decimal idIntervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				return context.AnalitiInterventi
					.Where(i => i.ID_INTERVENTO == idIntervento)
					.Where(i => !i.RIMOSSO_DA_OPER)
					.OrderBy(i => i.CODICE)
					.Select(i => new InfoAnalita()
					{
						IdAnalitaIntervento = i.ID_ANALITA_INTERVENTO,
						Codice = i.CODICE,
						Descrizione = i.DESCRIZIONE,
						CodiceMetodo = i.METODO_CODICE,
						DescrizioneMetodo = i.METODO_DESCRIZIONE,
						CodicePacchetto = i.PACCHETTO_CODICE,
						DescrizionePacchetto = i.PACCHETTO_DESCRIZIONE,
						UnitaMisura = i.UNITA_MISURA,
						ValoreLimite = i.VALORE_LIMITE
					})
					.ToList();
			}
		}

		private static void AggiornaSiglaVerbale(decimal idIntervento)
		{
			logger.Debug($"Aggiornamento sigla verbale IdIntervento:{idIntervento}");
			var infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);
			string siglaVerbale = infoIntervento.SiglaVerbale;
			using (SIASSEntities context = new SIASSEntities())
			{
				var intervento = context.Interventi.FirstOrDefault(i => i.ID_INTERVENTO == idIntervento) ??
					throw new ArgumentException($"Intervento con id {idIntervento} non trovato.");
				intervento.SIGLA_VERBALE = siglaVerbale;
				context.SaveChanges();
			}
		}
		public static void ImpostaDataCreazioneVerbale(decimal idIntervento)
		{
			logger.Debug($"Impostazione data creazione verbale IdIntervento:{idIntervento}");
			using (SIASSEntities context = new SIASSEntities())
			{
				var intervento = context.Interventi.FirstOrDefault(i => i.ID_INTERVENTO == idIntervento) ??
					throw new ArgumentException($"Intervento con id {idIntervento} non trovato.");
				intervento.CREAZIONE_VERBALE = DateTime.Now;
				context.SaveChanges();
			}
		}

		#region Nuovo intervento

		internal static Result<decimal> NuovoIntervento(RichiestaNuovoIntervento richiesta, ApiAnagrafiche apiAnagrafiche)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - Richiesta: {JsonSerializer.Serialize(richiesta)}");

			try
			{
				// Validazione della richiesta
				var validazione = richiesta.Valida();
				if (validazione.IsFailed)
				{
					logger.Error($"{MethodBase.GetCurrentMethod().Name} - Validazione fallita: {validazione.Errors[0].Message}");
					return Result.Fail<decimal>(validazione.Errors[0].Message);
				}

				var codiciPacchetto = richiesta.PacchettiIntervento.Select(i => i.CodicePacchetto).ToList();

				// Lettura analiti e misurazioni, la differenza è che gli analiti sono letti dalla linea "laboratorio" e le misurazioni da quella "territorio"
				List<InfoAnalita> analiti;
				if (richiesta.PrelievoCampioni)
				{
					var risultatoLetturaAnaliti = Analiti(richiesta.CodiceMatrice, richiesta.CodiceArgomento, codiciPacchetto, linea: "laboratorio", apiAnagrafiche);
					if (risultatoLetturaAnaliti.IsFailed)
						return Result.Fail<decimal>(risultatoLetturaAnaliti.Errors[0].Message);
					else
					{
						analiti = risultatoLetturaAnaliti.Value;
						logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Analiti: {JsonSerializer.Serialize(analiti)}");
					}
				}
				else
				{
					analiti = new List<InfoAnalita>();
				}

				List<InfoAnalita> misurazioni;
				var risultatoLetturaMisurazioni = Analiti(richiesta.CodiceMatrice, richiesta.CodiceArgomento, codiciPacchetto, linea: "territorio", apiAnagrafiche);
				if (risultatoLetturaMisurazioni.IsFailed)
					return Result.Fail<decimal>(risultatoLetturaMisurazioni.Errors[0].Message);
				else
				{
					misurazioni = risultatoLetturaMisurazioni.Value;
					logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Misurazioni: {JsonSerializer.Serialize(misurazioni)}");
				}

				// Costruisce l'elenco dei parametri di campo dell'intervento verificando che le misurazioni siano associate a grandezze della stazione
				List<ParametroCampoInterventi> parametriCampoIntervento;
				var risultatoParametriCampoIntervento = ParametriCampoIntervento(richiesta.IdStazione, misurazioni);
				if (risultatoParametriCampoIntervento.IsFailed)
					return Result.Fail<decimal>(risultatoParametriCampoIntervento.Errors[0].Message);
				else
				{
					parametriCampoIntervento = risultatoParametriCampoIntervento.Value;
					logger.Debug($"{MethodBase.GetCurrentMethod().Name} - ParametriCampoIntervento: {JsonSerializer.Serialize(parametriCampoIntervento)}");
				}

				// Inserimento dell'intervento
				using (SIASSEntities context = new SIASSEntities())
				{
					Intervento intervento = new Intervento()
					{
						ID_STAZIONE = richiesta.IdStazione,
						ID_TIPO_INTERVENTO = richiesta.IdTipoIntervento,
						CODICE_MATRICE = richiesta.CodiceMatrice,
						DESCRIZIONE_MATRICE = richiesta.DescrizioneMatrice,
						CODICE_ARGOMENTO = richiesta.CodiceArgomento,
						DESCRIZIONE_ARGOMENTO = richiesta.DescrizioneArgomento,
						CODICE_SEDE_ACCETTAZIONE = richiesta.CodiceSedeAccettazione,
						PRELIEVO_CAMPIONI = richiesta.PrelievoCampioni,
						DATA_INTERVENTO = richiesta.DataIntervento,
						ORA_INTERVENTO = richiesta.OraIntervento,
						ID_TIPO_RICHIEDENTE = richiesta.IdTipoRichiedente,
						CODICE_CAMPAGNA = richiesta.CodiceCampagna,
						AUTORE_ULTIMO_AGGIORNAMENTO = richiesta.AutoreCreazioneIntervento,
						ULTIMO_AGGIORNAMENTO = DateTime.Now,
						ORGANIZZAZIONE_CREAZIONE = richiesta.OrganizzazioneCreazione,
						NUMERO_CAMPIONI = richiesta.PrelievoCampioni ? 1 : 0, // Il numero campioni è impostato a 1 se l'intervento include campionamento, altrimento 0
						VERSIONE = "V2"
					};
					context.Interventi.Add(intervento);

					// Inserimento degli operatori
					foreach (var operatore in richiesta.OperatoriIntervento)
					{
						intervento.OperatoriIntervento.Add(new OperatoreIntervento()
						{
							ID_OPERATORE = operatore.IdOperatore,
							ID_INTERVENTO = intervento.ID_INTERVENTO,
							DESCRIZIONE_OPERATORE = operatore.DescrizioneOperatore
						});
					}

					// Aggiunta dei pacchetti
					foreach (var pacchetto in richiesta.PacchettiIntervento)
					{
						intervento.Pacchetti.Add(new PacchettoIntervento()
						{
							CODICE = pacchetto.CodicePacchetto,
							DESCRIZIONE = pacchetto.DescrizionePacchetto,
							SEDE = pacchetto.Sede,
							COD_ARGOMENTO = pacchetto.CodiceArgomento
						});
					}

					// Aggiunta degli analiti
					foreach (var analita in analiti)
					{
						intervento.Analiti.Add(new AnalitaIntervento()
						{
							CODICE = analita.Codice,
							DESCRIZIONE = analita.Descrizione,
							METODO_CODICE = analita.CodiceMetodo,
							METODO_DESCRIZIONE = analita.DescrizioneMetodo,
							PACCHETTO_CODICE = analita.CodicePacchetto,
							PACCHETTO_DESCRIZIONE = analita.DescrizionePacchetto,
							UNITA_MISURA = analita.UnitaMisura,
							VALORE_LIMITE = analita.ValoreLimite,
							LINEA_LAV = analita.LineaLav
						});
					}

					// Aggiunta dei parametri di campo
					foreach (var parametro in parametriCampoIntervento)
					{
						intervento.ParametriCampo.Add(new ParametroCampoInterventi()
						{
							ID_GRANDEZZA_STAZIONE = parametro.ID_GRANDEZZA_STAZIONE,
							CODICE = parametro.CODICE,
							DESCRIZIONE = parametro.DESCRIZIONE,
							METODO_CODICE = parametro.METODO_CODICE,
							PACCHETTO_CODICE = parametro.PACCHETTO_CODICE
						});
					}

					context.SaveChanges();

					AggiornaSiglaVerbale(intervento.ID_INTERVENTO);

					return Result.Ok<decimal>(intervento.ID_INTERVENTO);
				}
			}
			catch (Exception ex)
			{
				string dettagli = Utils.GetExceptionDetails(ex);
				logger.Error(ex, $"{MethodBase.GetCurrentMethod().Name} - {dettagli}");
				return Result.Fail<decimal>(dettagli);
			}
		}

		public static Result<List<InfoAnalita>> Analiti(string codiceMatrice, string codiceArgomento, List<string> codiciPacchetto, string linea, ApiAnagrafiche apiAnagrafiche)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - CodiceMatrice: {codiceMatrice} CodiceArgomento: {codiceArgomento} " +
				$"CodiciPacchetto: {string.Join(", ", codiciPacchetto)} Linea: {linea}");

			var analiti = new List<InfoAnalita>();

			try
			{
				foreach (var codicePacchetto in codiciPacchetto)
				{
					var analitiPacchetto = apiAnagrafiche.Analiti(codiceMatrice, codiceArgomento, codicePacchetto, null, linea);
					foreach (var analitaPacchetto in analitiPacchetto)
					{
						// Se l'analita non è già presente nella lista analiti lo aggiunge
						if (!analiti.Exists(i => i.Codice == analitaPacchetto.AnalitaIdentity))
						{
							analiti.Add(new InfoAnalita()
							{
								Codice = analitaPacchetto.AnalitaIdentity,
								Descrizione = analitaPacchetto.AnalitaName,
								CodiceMetodo = analitaPacchetto.MetodoIdentity,
								DescrizioneMetodo = analitaPacchetto.MetodoName,
								CodicePacchetto = analitaPacchetto.PackIdentity,
								DescrizionePacchetto = analitaPacchetto.PackName,
								// Se ParamUnits è una stringa vuota o di soli spazi è impostata a null
								UnitaMisura = string.IsNullOrWhiteSpace(analitaPacchetto.ParamUnits) ? null : analitaPacchetto.ParamUnits.Trim(),
								ValoreLimite = analitaPacchetto.ValoreLimite,
								LineaLav = analitaPacchetto.LineaLav
							});
						}
					}
				}
			}
			catch (ApiAnagraficheException ex)
			{
				logger.Error(ex, $"{MethodBase.GetCurrentMethod().Name} - Errore durante la chiamata API per la lettura degli analiti {ex.Message}");
				return Result.Fail<List<InfoAnalita>>(ex.Message);
			}
			catch (Exception ex)
			{
				logger.Error(ex, $"{MethodBase.GetCurrentMethod().Name} - {ex.Message}");
				return Result.Fail<List<InfoAnalita>>(ex.Message);
			}

			return Result.Ok(analiti);
		}

		/// <summary>
		/// Costruisce l'elenco dei paraemtri di campo dell'intervento associando a ogni misurazione una grandezza della stazione
		/// </summary>
		/// <param name="idStazione"></param>
		/// <param name="misurazioni"></param>
		/// <returns></returns>
		public static Result<List<ParametroCampoInterventi>> ParametriCampoIntervento(decimal idStazione, List<InfoAnalita> misurazioni)
		{
			var parametri = new List<ParametroCampoInterventi>();
			using (SIASSEntities context = new SIASSEntities())
			{
				var grandezzeStazione = context.GrandezzeStazione.Where(i => i.ID_STAZIONE == idStazione).ToList();
				foreach (var misurazione in misurazioni)
				{
					// Verifica la corrispondenza del codice analita nella tabella dei tipi grandezza
					var tipoGrandezza = context.TipiGrandezza.Where(i => i.NOME_GRANDEZZA == misurazione.Descrizione).FirstOrDefault();
					if (tipoGrandezza == null)
						return Result.Fail($"La grandezza '{misurazione.Descrizione}' non è presente nella stazione {idStazione}");
					var verificaCorrispondezaAnalita = (misurazione.Codice == tipoGrandezza.ANALITA_IDENTITY);
					if (!verificaCorrispondezaAnalita)
						return Result.Fail($"Il codice analita '{misurazione.Codice}' per la grandezza '{misurazione.Descrizione}' non corrisponde per la stazione {idStazione}");


					// La grandezza riportata nella misurazione deve essere tra quelle associate alla stazione
					var grandezzaStazione = grandezzeStazione.FirstOrDefault(i => i.GRANDEZZA.Equals(misurazione.Descrizione, StringComparison.InvariantCultureIgnoreCase));
					if (grandezzaStazione == null)
						return Result.Fail($"La grandezza '{misurazione.Descrizione}' non è presente nella stazione {idStazione}");
					if (string.IsNullOrWhiteSpace(misurazione.UnitaMisura))
					{
						// Se la misurazione non ha unità di misura la grandezza deve essere di tipo booleano
						if (!grandezzaStazione.TipoGrandezza.BOOLEANA)
							return Result.Fail($"La grandezza '{grandezzaStazione.GRANDEZZA}' associata alla stazione con id {idStazione} non è di tipo booleano " +
								$"quindi deve essere associata una misurazione con unità di misura (codice misurazione: {misurazione.Codice})");
					}
					else
					{
						// altrimenti la misurazione deve avere la stessa unità di misura della grandezza
						if (!misurazione.UnitaMisura.Equals(grandezzaStazione.UNITA_MISURA, StringComparison.InvariantCultureIgnoreCase))
							return Result.Fail($"La grandezza '{grandezzaStazione.GRANDEZZA}' associata alla stazione con id {idStazione} " +
								$"ha unità di misura '{grandezzaStazione.UNITA_MISURA}' diversa da quella della misurazione '{misurazione.UnitaMisura}'");
					}


					parametri.Add(new ParametroCampoInterventi()
					{
						ID_GRANDEZZA_STAZIONE = grandezzaStazione.ID_GRANDEZZA_STAZIONE,
						CODICE = misurazione.Codice,
						DESCRIZIONE = misurazione.Descrizione,
						METODO_CODICE = misurazione.CodiceMetodo,
						PACCHETTO_CODICE = misurazione.CodicePacchetto
					});
				}
			}
			return Result.Ok(parametri);
		}

		internal class RichiestaNuovoIntervento
		{
			public decimal IdStazione { get; set; }
			public decimal IdTipoIntervento { get; set; }
			public string CodiceMatrice { get; set; }
			public string DescrizioneMatrice { get; set; }
			public string CodiceArgomento { get; set; }
			public string DescrizioneArgomento { get; set; }
			public DateTime DataIntervento { get; set; }
			public string OraIntervento { get; set; }
			public decimal? IdTipoRichiedente { get; set; }
			public string CodiceCampagna { get; set; }
			public string CodiceSedeAccettazione { get; set; }
			public bool PrelievoCampioni { get; set; }
			public string AutoreCreazioneIntervento { get; set; }
			public List<Operatore> OperatoriIntervento { get; set; }
			public List<Pacchetto> PacchettiIntervento { get; set; }
			/// <summary>
			/// Codice dell'organizzazione che ha creato l'intervento. 
			/// E' utilizzato per decidere chi può modificare l'intervento.
			/// </summary>
			public string OrganizzazioneCreazione { get; set; }
			public Result Valida()
			{
				if (IdStazione == 0)
					return Result.Fail("IdStazione non può essere zero.");
				if (IdTipoIntervento == 0)
					return Result.Fail("IdTipoIntervento non può essere zero.");
				if (string.IsNullOrEmpty(CodiceMatrice))
					return Result.Fail("CodiceMatrice non può essere null o vuoto.");
				if (string.IsNullOrEmpty(DescrizioneMatrice))
					return Result.Fail("DescrizioneMatrice non può essere null o vuoto.");
				if (string.IsNullOrEmpty(CodiceArgomento))
					return Result.Fail("CodiceArgomento non può essere null o vuoto.");
				if (string.IsNullOrEmpty(DescrizioneArgomento))
					return Result.Fail("DescrizioneArgomento non può essere null o vuoto.");
				if (DataIntervento == default(DateTime))
					return Result.Fail("DataIntervento non può essere il valore predefinito.");
				if (string.IsNullOrEmpty(CodiceSedeAccettazione))
					return Result.Fail("CodiceSedeAccettazione non può essere null o vuoto.");
				if (string.IsNullOrEmpty(AutoreCreazioneIntervento))
					return Result.Fail("AutoreCreazioneIntervento non può essere null o vuoto.");
				if (string.IsNullOrEmpty(OrganizzazioneCreazione))
					return Result.Fail("OrganizzazioneCreazione non può essere null o vuoto.");
				if (OperatoriIntervento == null || OperatoriIntervento.Count == 0)
					return Result.Fail("OperatoriIntervento deve contenere almeno un elemento.");

				return Result.Ok();
			}

			internal class Operatore
			{
				public decimal IdOperatore { get; set; }
				public string DescrizioneOperatore { get; set; }
			}

			internal class Pacchetto
			{
				public string CodicePacchetto { get; set; }
				public string DescrizionePacchetto { get; set; }
				public string CodiceArgomento { get; set; }
				public string Sede { get; set; }
			}
		}

		#endregion Nuovo intervento

		#region Dettagli intervento

		public static InfoIntervento CaricaInfoIntervento(decimal idIntevento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				InfoIntervento infoIntervento = (from intervento in context.Interventi
												 where intervento.ID_INTERVENTO == idIntevento
												 select new InfoIntervento
												 {
													 IdIntervento = intervento.ID_INTERVENTO,
													 IdStazione = intervento.ID_STAZIONE,
													 IdTipoIntervento = intervento.ID_TIPO_INTERVENTO,
													 DescrizioneTipoIntervento = intervento.TipoIntervento.DESCRIZIONE_TIPO_INTERVENTO,
													 IdTipoRichiedente = intervento.ID_TIPO_RICHIEDENTE,
													 DescrizioneTipoRichiedente = (intervento.ID_TIPO_RICHIEDENTE == null ? null : intervento.TipoRichiedente.DESCRIZIONE_TIPO_RICHIEDENTE),
													 CodiceArgomento = intervento.CODICE_ARGOMENTO,
													 DescrizioneArgomento = intervento.DESCRIZIONE_ARGOMENTO,
													 CodiceMatrice = intervento.CODICE_MATRICE,
													 DescrizioneMatrice = intervento.DESCRIZIONE_MATRICE,
													 DataIntervento = intervento.DATA_INTERVENTO,
													 OraIntervento = intervento.ORA_INTERVENTO,
													 DurataIntervento = intervento.DURATA_INTERVENTO,
													 CodiceCampagna = intervento.CODICE_CAMPAGNA,
													 FileDati = intervento.FILE_DATI,
													 FileAngoli = intervento.FILE_ANGOLI,
													 IdStrumento = intervento.ID_STRUMENTO,
													 DescrizioneStrumento = (intervento.ID_STRUMENTO == null ? null : intervento.TipoStrumentoIntervento.DESCRIZIONE_STRUMENTO),
													 CategoriaStazione = intervento.Stazione.TipoStazione.Categoria,
													 DescrizioneStazione = intervento.Stazione.Descrizione,
													 CodiceIdentificativoStazione = intervento.Stazione.CodiceIdentificativo,
													 UltimoAggiornamento = intervento.ULTIMO_AGGIORNAMENTO,
													 AutoreUltimoAggiornamento = intervento.AUTORE_ULTIMO_AGGIORNAMENTO,
													 NumeroCampioni = intervento.NUMERO_CAMPIONI,
													 Annotazioni = intervento.ANNOTAZIONI,
													 ParteNomeTecnico = intervento.PARTE_NOME_TECNICO,
													 ParteAziendaTecnico = intervento.PARTE_AZIENDA_TECNICO,
													 ParteRuoloTecnico = intervento.PARTE_RUOLO_TECNICO,
													 ParteContatti = intervento.PARTE_CONTATTI,
													 OraFineIntervento = intervento.ORA_FINE_INTERVENTO,
													 QuotaCampione = intervento.QUOTA_CAMPIONE,
													 AnnotazioniPacchetti = intervento.ANNOTAZIONI_PACCHETTI,
													 CampioneBianco = intervento.CAMPIONE_BIANCO,
													 ModelloVerbaleSelezionato = intervento.MOD_VERBALE_SELEZIONATO,
													 DatiCampioneBianco = intervento.DATI_CAMPIONE_BIANCO,
													 CodiceSedeAccettazione = intervento.CODICE_SEDE_ACCETTAZIONE,
													 DenominazioneSedeAccettazione = intervento.SedeAccettazione.DENOMINAZIONE_SEDE,
													 PrelievoCampioni = intervento.PRELIEVO_CAMPIONI,
													 OrganizzazioneCreazione = intervento.ORGANIZZAZIONE_CREAZIONE,
													 StatoInvioVerbaleV1 = intervento.STATO_INVIO_VERBALE_V1,
													 Versione = intervento.VERSIONE
												 }).FirstOrDefault();

				if (infoIntervento == null)
					return null;

				// Aggiunge gli operatori
				infoIntervento.OperatoriIntervento = ElencoOperatoriIntervento(infoIntervento.IdIntervento);
				infoIntervento.OperatoriSupportoIntervento = ElencoOperatoriSupportoIntervento(infoIntervento.IdIntervento);

				// Aggiunge i pacchetti
				// In base alla versione dell'intervento li prende da due tabelle diverse
				if (infoIntervento.Versione == "V1")
				{
					infoIntervento.PacchettiIntervento = ElencoPacchettiInterventoV1(infoIntervento.IdIntervento);
				}
				else
				{
					infoIntervento.PacchettiIntervento = ElencoPacchettiIntervento(infoIntervento.IdIntervento);

				}

				// Parametri di campo (con o senza misurazione)
				infoIntervento.ParametriDiCampo = ElencoParametriCampoPerIntervento(infoIntervento.IdIntervento);

				// Analiti
				infoIntervento.Analiti = AnalitiPerIntervento(infoIntervento.IdIntervento);

				return infoIntervento;
			}
		}

		private static List<InfoIntervento.InfoPacchettoIntervento> ElencoPacchettiIntervento(decimal idIntervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = from pacchettoIntervento in context.PacchettiInterventi
							 where pacchettoIntervento.ID_INTERVENTO == idIntervento
							 orderby pacchettoIntervento.CODICE
							 select new InfoIntervento.InfoPacchettoIntervento
							 {
								 IdPacchettoIntervento = pacchettoIntervento.ID_PACCHETTO_INTERVENTI,
								 IdIntervento = pacchettoIntervento.ID_INTERVENTO,
								 DescrizionePacchetto = pacchettoIntervento.DESCRIZIONE,
								 CodicePacchetto = pacchettoIntervento.CODICE,
								 Sede = pacchettoIntervento.SEDE,
							 };
				return elenco.ToList();
			}
		}

		private static List<InfoIntervento.InfoPacchettoIntervento> ElencoPacchettiInterventoV1(decimal idIntervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = from pacchettoIntervento in context.PacchettiInterventiV1
							 where pacchettoIntervento.ID_INTERVENTO == idIntervento
							 orderby pacchettoIntervento.TipoPacchetto.ORDINE
							 select new InfoIntervento.InfoPacchettoIntervento
							 {
								 IdPacchettoIntervento = pacchettoIntervento.ID_PACCHETTO_INTERVENTO,
								 IdIntervento = pacchettoIntervento.ID_INTERVENTO,
								 DescrizionePacchetto = pacchettoIntervento.TipoPacchetto.DESCRIZIONE_PACCHETTO,
								 CodicePacchetto = pacchettoIntervento.TipoPacchetto.CODICE_ALIMS,
								 Sede = null
							 };
				return elenco.ToList();
			}
		}


		private static List<InfoParametroCampo> ElencoParametriCampoPerIntervento(decimal idIntervento)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdIntervento: {idIntervento}");

			// I parametri di campo definiscono le misurazioni da fare.
			// L'elenco è generato in base ai valori presenti nella tabella misurazioni
			// aggiungendo i parametri (grandezze stazione) che non ci sono perché il valore non è stato registrato.

			// Misurazioni presenti nell'intervento e grandezze corrispondenti
			var misurazioni = MisurazioniPerIntervento(idIntervento);
			var idGrandezzeConMisurazioni = misurazioni.Select(i => i.IdGrandezzaStazione).Distinct().ToList();
			using (SIASSEntities context = new SIASSEntities())
			{
				// Grandezze senza misurazioni
				var grandezzeSenzaMisurazione = context.ParametriCampoInterventi
				  .Where(i => i.ID_INTERVENTO == idIntervento)
				  .Where(i => !idGrandezzeConMisurazioni.Contains(i.ID_GRANDEZZA_STAZIONE))
				  .Select(i => new InfoParametroCampo()
				  {
					  IdGrandezzaStazione = i.ID_GRANDEZZA_STAZIONE,
					  Grandezza = i.GrandezzaStazione.GRANDEZZA,
					  UnitaMisura = i.GrandezzaStazione.UNITA_MISURA,
					  SeUnitaMisuraBooleana = i.GrandezzaStazione.TipoUnitaMisura.SE_BOOLEANO,
					  NumeroDecimali = i.GrandezzaStazione.NUMERO_DECIMALI,
					  FonteArpal = true,
					  CodicePacchetto = i.PACCHETTO_CODICE
				  })
				  .ToList();

				// Inizializzazione elenco parametri campo con elenco grandezze senza misurazione
				var parametriCampo = new List<InfoParametroCampo>(grandezzeSenzaMisurazione);
				// Aggiunta delle misurazioni
				parametriCampo.AddRange(misurazioni.Select(i => new InfoParametroCampo()
				{
					IdGrandezzaStazione = i.IdGrandezzaStazione,
					Grandezza = i.Grandezza,
					UnitaMisura = i.UnitaMisura,
					NumeroDecimali = i.NumeroDecimali,
					SeUnitaMisuraBooleana = i.SeUnitaMisuraBooleana,
					IdMisurazione = i.IdMisurazione,
					DataMisurazione = i.DataMisurazione,
					Valore = i.Valore,
					FonteArpal = i.FonteArpal,
					CodicePacchetto = i.CodicePacchetto
				}));

				return parametriCampo.OrderBy(i => i.Grandezza).ToList();
			}
		}

		#endregion Dettagli intervento

		#region Aggiorna intervento

		internal static Result AggiornaIntervento(InfoIntervento intervento, bool conParametriDiCampo)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - Intervento: {JsonSerializer.Serialize(intervento)}");

			try
			{
				using (SIASSEntities context = new SIASSEntities())
				{
					Intervento interventoDaAggiornare = context.Interventi.FirstOrDefault(i => i.ID_INTERVENTO == intervento.IdIntervento);
					if (interventoDaAggiornare == null)
						return Result.Fail($"Intervento con id {intervento.IdIntervento} non trovato.");

					// Aggiornamento dei dati dell'intervento, solo quelli aggiornabili
					interventoDaAggiornare.ID_TIPO_INTERVENTO = intervento.IdTipoIntervento;
					interventoDaAggiornare.ID_TIPO_RICHIEDENTE = intervento.IdTipoRichiedente;
					interventoDaAggiornare.DATA_INTERVENTO = intervento.DataIntervento;
					interventoDaAggiornare.DURATA_INTERVENTO = intervento.DurataIntervento;
					interventoDaAggiornare.CODICE_CAMPAGNA = intervento.CodiceCampagna;
					interventoDaAggiornare.FILE_ANGOLI = intervento.FileAngoli;
					interventoDaAggiornare.FILE_DATI = intervento.FileDati;
					interventoDaAggiornare.ID_STRUMENTO = intervento.IdStrumento;
					interventoDaAggiornare.ORA_INTERVENTO = intervento.OraIntervento;
					interventoDaAggiornare.NUMERO_CAMPIONI = intervento.NumeroCampioni;
					interventoDaAggiornare.ANNOTAZIONI = intervento.Annotazioni;
					interventoDaAggiornare.PARTE_NOME_TECNICO = intervento.ParteNomeTecnico;
					interventoDaAggiornare.PARTE_AZIENDA_TECNICO = intervento.ParteAziendaTecnico;
					interventoDaAggiornare.PARTE_RUOLO_TECNICO = intervento.ParteRuoloTecnico;
					interventoDaAggiornare.PARTE_CONTATTI = intervento.ParteContatti;
					interventoDaAggiornare.ORA_FINE_INTERVENTO = intervento.OraFineIntervento;
					interventoDaAggiornare.QUOTA_CAMPIONE = intervento.QuotaCampione;
					interventoDaAggiornare.ANNOTAZIONI_PACCHETTI = intervento.AnnotazioniPacchetti;
					interventoDaAggiornare.CAMPIONE_BIANCO = intervento.CampioneBianco;
					interventoDaAggiornare.DATI_CAMPIONE_BIANCO = intervento.DatiCampioneBianco;
					interventoDaAggiornare.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					interventoDaAggiornare.AUTORE_ULTIMO_AGGIORNAMENTO = intervento.AutoreUltimoAggiornamento;

					context.SaveChanges();
				}

				AggiornaOperatoriIntervento(intervento);

				AggiornaSiglaVerbale(intervento.IdIntervento);

				// Misurazioni
				if (conParametriDiCampo)
				{
					foreach (var misurazione in intervento.ParametriDiCampo)
					{
						MisurazioniManager.InserisceAggiornaEliminaMisurazioneIntervento(
							dataMisurazione: intervento.DataIntervento,
							idGrandezzaStazione: misurazione.IdGrandezzaStazione,
							ultimoAggiornamento: DateTime.Now,
							autoreUltimoAggiornamento: intervento.AutoreUltimoAggiornamento,
							idIntervento: intervento.IdIntervento,
							codiceIdentificativoSensore: null,
							valore: misurazione.Valore,
							fonteArpal: misurazione.FonteArpal,
							codicePacchetto: misurazione.CodicePacchetto
							);
					}

					AggiornaPacchettiIntervento(intervento, out List<string> codiciPacchettiEliminati);

					string erroreAggiuntaParametri = AggiungeParametriDiCampoMancanti(intervento);
					if (!string.IsNullOrWhiteSpace(erroreAggiuntaParametri))
					{
						logger.Error(erroreAggiuntaParametri);
						return Result.Fail(erroreAggiuntaParametri);
					}

					MisurazioniManager.EliminaMisurazioniPerPacchettiRimossi(codiciPacchettiEliminati);

					AggiornaAnalitiIntervento(intervento);
				}
				logger.Info($"Aggiornato intervento - id:{intervento.IdIntervento} - id stazione:{intervento.IdStazione} - Operatore:{intervento.AutoreUltimoAggiornamento}");
				return Result.Ok();
			}
			catch (Exception ex)
			{
				string dettagli = Utils.GetExceptionDetails(ex);
				logger.Error(ex, $"{MethodBase.GetCurrentMethod().Name} - {dettagli}");
				return Result.Fail(dettagli);
			}
		}

		private static string AggiungeParametriDiCampoMancanti(InfoIntervento intervento)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdIntervento: {intervento.IdIntervento}");
			var apiAnagrafiche = Global.ApiAnagrafiche;

			using (SIASSEntities context = new SIASSEntities())
			{
				List<InfoAnalita> misurazioni = new List<InfoAnalita>();
				var risultatoLetturaMisurazioni = Analiti(intervento.CodiceMatrice, intervento.CodiceArgomento, intervento.PacchettiIntervento.Select(i => i.CodicePacchetto).ToList(), linea: "territorio", apiAnagrafiche);
				if (risultatoLetturaMisurazioni.IsFailed)
				{
					logger.Error(risultatoLetturaMisurazioni.Errors[0].Message);
					return risultatoLetturaMisurazioni.Errors[0].Message;
				}
				else
				{
					misurazioni = risultatoLetturaMisurazioni.Value;
					logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Misurazioni: {JsonSerializer.Serialize(misurazioni)}");
				}

				// Costruisce l'elenco dei parametri di campo dell'intervento verificando che le misurazioni siano associate a grandezze della stazione
				List<ParametroCampoInterventi> parametriCampoIntervento = new List<ParametroCampoInterventi>();
				var risultatoParametriCampoIntervento = ParametriCampoIntervento(intervento.IdStazione, misurazioni);
				if (risultatoParametriCampoIntervento.IsFailed)
				{
					logger.Error(risultatoParametriCampoIntervento.Errors[0].Message);
					return risultatoParametriCampoIntervento.Errors[0].Message;
				}
				else
				{
					parametriCampoIntervento = risultatoParametriCampoIntervento.Value;
					logger.Debug($"{MethodBase.GetCurrentMethod().Name} - ParametriCampoIntervento: {JsonSerializer.Serialize(parametriCampoIntervento)}");
				}

				var parametriDiCampoPresenti = context.ParametriCampoInterventi.Where(i => i.ID_INTERVENTO == intervento.IdIntervento).ToList();
				foreach (var p in parametriCampoIntervento)
				{
					if (!parametriDiCampoPresenti.Any(i => i.ID_GRANDEZZA_STAZIONE == p.ID_GRANDEZZA_STAZIONE))
					{
						context.ParametriCampoInterventi.Add(new ParametroCampoInterventi
						{
							ID_GRANDEZZA_STAZIONE = p.ID_GRANDEZZA_STAZIONE,
							CODICE = p.CODICE,
							DESCRIZIONE = p.DESCRIZIONE,
							METODO_CODICE = p.METODO_CODICE,
							PACCHETTO_CODICE = p.PACCHETTO_CODICE,
							ID_INTERVENTO = intervento.IdIntervento
						}
						);
					}
				}
				context.SaveChanges();
			}
			return null;
		}


		private static void AggiornaOperatoriIntervento(InfoIntervento intervento)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdIntervento: {intervento.IdIntervento}");

			using (SIASSEntities context = new SIASSEntities())
			{
				// Operatori
				logger.Info($"Eliminazione operatori IdIntervento:{intervento.IdIntervento}");
				context.Database.ExecuteSqlCommand($"DELETE FROM SIAS_OPERATORI_INTERVENTI WHERE ID_INTERVENTO = {intervento.IdIntervento}");
				context.SaveChanges();
				logger.Info($"Eliminati operatori IdIntervento:{intervento.IdIntervento}");

				foreach (var o in intervento.OperatoriIntervento)
				{
					OperatoreIntervento operatore = new OperatoreIntervento
					{
						ID_INTERVENTO = intervento.IdIntervento,
						ID_OPERATORE = o.IdOperatore,
						DESCRIZIONE_OPERATORE = o.DescrizioneOperatore
					};
					context.OperatoriInterventi.Add(operatore);
					logger.Info($"Inserimento operatore IdIntervento:{intervento.IdIntervento} idOperatore:{o.IdOperatore}");
				}

				// Operatori a supporto
				logger.Info($"Eliminazione operatori a supporto IdIntervento:{intervento.IdIntervento}");
				context.Database.ExecuteSqlCommand($"DELETE FROM SIAS_OPERATORI_INTERV_SUP WHERE ID_INTERVENTO = {intervento.IdIntervento}");
				context.SaveChanges();
				logger.Info($"Eliminati operatori a supporto IdIntervento:{intervento.IdIntervento}");

				foreach (var o in intervento.OperatoriSupportoIntervento)
				{
					OperatoreSupportoIntervento operatore = new OperatoreSupportoIntervento
					{
						ID_INTERVENTO = intervento.IdIntervento,
						ID_OPERATORE = o.IdOperatore,
						DESCRIZIONE_OPERATORE = o.DescrizioneOperatore
					};
					context.OperatoriSupportoInterventi.Add(operatore);
					logger.Info($"Inserimento operatore a supporto IdIntervento:{intervento.IdIntervento} idOperatore:{o.IdOperatore}");
				}
				context.SaveChanges();
			}
		}

		private static void AggiornaAnalitiIntervento(InfoIntervento intervento)
		{
			// La funzione aggiorna il DB in base all'elenco di oggetti InfoAnalita ricevuto seguendo queste regole:
			// - Se un elemento nell'elenco ricevuto non ha IdAnalitaIntervento l'analita è inserito nel DB e il record marcato come "aggiunto da operatore"
			// - Se l'id sul DB non è tra quelli nell'elenco ricevuto il record è marcato come "rimosso da operatore"

			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdIntervento: {intervento.IdIntervento}");

			using (SIASSEntities context = new SIASSEntities())
			{
				// Elenco id analiti da mantenere
				var idAnalitiDaMantenere = intervento.Analiti.Where(i => i.IdAnalitaIntervento.HasValue).Select(i => i.IdAnalitaIntervento.Value).ToList();

				// Eliminazione analiti non presenti nell'elenco
				var idAnalitiDaEliminare = context.AnalitiInterventi
					.Where(i => i.ID_INTERVENTO == intervento.IdIntervento)
					.Where(i => !idAnalitiDaMantenere.Contains(i.ID_ANALITA_INTERVENTO))
					.Select(i => i.ID_ANALITA_INTERVENTO)
					.ToList();
				foreach (var id in idAnalitiDaEliminare)
				{
					var analita = context.AnalitiInterventi.FirstOrDefault(i => i.ID_ANALITA_INTERVENTO == id);
					if (analita != null)
					{
						analita.RIMOSSO_DA_OPER = true;
					}
				}
				context.SaveChanges();

				foreach (var infoAnalita in intervento.Analiti.Where(i => !i.IdAnalitaIntervento.HasValue))
				{
					context.AnalitiInterventi.Add(new AnalitaIntervento()
					{
						ID_INTERVENTO = intervento.IdIntervento,
						CODICE = infoAnalita.Codice,
						DESCRIZIONE = infoAnalita.Descrizione,
						METODO_CODICE = infoAnalita.CodiceMetodo,
						METODO_DESCRIZIONE = infoAnalita.DescrizioneMetodo,
						PACCHETTO_CODICE = infoAnalita.CodicePacchetto,
						PACCHETTO_DESCRIZIONE = infoAnalita.DescrizionePacchetto,
						UNITA_MISURA = infoAnalita.UnitaMisura,
						VALORE_LIMITE = infoAnalita.ValoreLimite,
						LINEA_LAV = infoAnalita.LineaLav,
						AGGIUNTO_DA_OPER = true
					});
				}
				context.SaveChanges();
			}
		}

		public static void AggiornaParametriDiCampo(InfoIntervento intervento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
			}
		}


		internal static List<InfoAnalita> RicercaAnaliti(ApiAnagrafiche apiAnagrafiche, string linea, string codiceMatrice,
			string codicePacchetto = null, string parteNomeAnalita = null)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - CodiceMatrice: {codiceMatrice} CodicePacchetto: {codicePacchetto} - ParteNomeAnalita: {parteNomeAnalita}");

			var risultati = new List<InfoAnalita>();
			var analiti = apiAnagrafiche.Analiti(codiceMatrice, codiceArgomento: null, codicePacchetto, parteNomeAnalita, linea);
			foreach (var analita in analiti)
			{
				risultati.Add(new InfoAnalita()
				{
					Codice = analita.AnalitaIdentity,
					Descrizione = analita.AnalitaName,
					CodiceMetodo = analita.MetodoIdentity,
					DescrizioneMetodo = analita.MetodoName,
					CodicePacchetto = analita.PackIdentity,
					DescrizionePacchetto = analita.PackName,
					UnitaMisura = analita.ParamUnits,
					ValoreLimite = analita.ValoreLimite,
					LineaLav = analita.LineaLav,
					LDQ = analita.LDQ,
					LimiteIndic = analita.LimiteIndic,
					LimiteMinimo = analita.LimiteMinimo,
					LimiteMassimo = analita.LimiteMassimo,
					LaboratorioAnalisi = analita.LaboratorioAnalisi
				});
			}

			return risultati;
		}

		internal static List<InfoContenitore> RicercaContenitori(ApiAnagrafiche apiAnagrafiche, string codicePacchetto)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - CodicePacchetto: {codicePacchetto}");

			var risultati = new List<InfoContenitore>();
			var contenitori = apiAnagrafiche.Contenitori(codicePacchetto);
			foreach (var contenitore in contenitori)
			{
				risultati.Add(new InfoContenitore()
				{
					Pacchetto = contenitore.Identity,
					Descrizione = contenitore.BottleType,
					Quantita = contenitore.Quantity
				});
			}

			return risultati;
		}


		private static void AggiornaPacchettiIntervento(InfoIntervento intervento, out List<string> codiciPacchettiEliminati)
		{
			// La funzione aggiorna il DB in base all'elenco di pacchetti ricevuto seguendo queste regole:
			// - Se un elemento nell'elenco ricevuto non ha Id pacchetto è inserito nel DB
			// - Se l'id sul DB non è tra quelli nell'elenco ricevuto il record è eliminato

			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdIntervento: {intervento.IdIntervento}");

			using (SIASSEntities context = new SIASSEntities())
			{
				// Elenco id pacchetti da mantenere
				var idPacchettiDaMantenere = intervento.PacchettiIntervento.Where(i => i.IdPacchettoIntervento.HasValue).Select(i => i.IdPacchettoIntervento.Value).ToList();

				// Eliminazione pacchetti non presenti nell'elenco
				var idPacchettiDaEliminare = context.PacchettiInterventi
					.Where(i => i.ID_INTERVENTO == intervento.IdIntervento)
					.Where(i => !idPacchettiDaMantenere.Contains(i.ID_PACCHETTO_INTERVENTI))
					.Select(i => i.ID_PACCHETTO_INTERVENTI)
					.ToList();

				codiciPacchettiEliminati = new List<string>();
				foreach (var id in idPacchettiDaEliminare)
				{
					var pacchetto = context.PacchettiInterventi.FirstOrDefault(i => i.ID_PACCHETTO_INTERVENTI == id);
					if (pacchetto != null)
					{
						codiciPacchettiEliminati.Add(pacchetto.CODICE);
						context.PacchettiInterventi.Remove(pacchetto);
					}
				}
				context.SaveChanges();

				// Pacchetti da aggiungere
				foreach (var infoPacchetto in intervento.PacchettiIntervento.Where(i => !i.IdPacchettoIntervento.HasValue))
				{
					context.PacchettiInterventi.Add(new PacchettoIntervento()
					{
						ID_INTERVENTO = intervento.IdIntervento,
						COD_ARGOMENTO = infoPacchetto.CodiceArgomento,
						CODICE = infoPacchetto.CodicePacchetto,
						DESCRIZIONE = infoPacchetto.DescrizionePacchetto,
						SEDE = infoPacchetto.Sede
					});
				}
				context.SaveChanges();
			}
		}

		#endregion Aggiorna intervento

		#region Nuovo intervento senza prelievo campioni
		internal static Result<decimal> NuovoInterventoSenzaPrelievoCampioni(RichiestaNuovoInterventoSenzaPrelievoCampioni richiesta, ApiAnagrafiche apiAnagrafiche)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - Richiesta: {JsonSerializer.Serialize(richiesta)}");

			try
			{
				// Validazione della richiesta
				var validazione = richiesta.Valida();
				if (validazione.IsFailed)
				{
					logger.Error($"{MethodBase.GetCurrentMethod().Name} - Validazione fallita: {validazione.Errors[0].Message}");
					return Result.Fail<decimal>(validazione.Errors[0].Message);
				}

				// Inserimento dell'intervento
				using (SIASSEntities context = new SIASSEntities())
				{
					Intervento intervento = new Intervento()
					{
						ID_STAZIONE = richiesta.IdStazione,
						ID_TIPO_INTERVENTO = richiesta.IdTipoIntervento,
						CODICE_ARGOMENTO = richiesta.CodiceArgomento,
						DESCRIZIONE_ARGOMENTO = richiesta.DescrizioneArgomento,
						PRELIEVO_CAMPIONI = richiesta.PrelievoCampioni,
						DATA_INTERVENTO = richiesta.DataIntervento,
						ORA_INTERVENTO = richiesta.OraIntervento,
						ID_TIPO_RICHIEDENTE = richiesta.IdTipoRichiedente,
						CODICE_CAMPAGNA = richiesta.CodiceCampagna,
						AUTORE_ULTIMO_AGGIORNAMENTO = richiesta.AutoreCreazioneIntervento,
						ULTIMO_AGGIORNAMENTO = DateTime.Now,
						ORGANIZZAZIONE_CREAZIONE = richiesta.OrganizzazioneCreazione,
						VERSIONE = "V2"
					};
					context.Interventi.Add(intervento);

					// Inserimento degli operatori
					foreach (var operatore in richiesta.OperatoriIntervento)
					{
						intervento.OperatoriIntervento.Add(new OperatoreIntervento()
						{
							ID_OPERATORE = operatore.IdOperatore,
							ID_INTERVENTO = intervento.ID_INTERVENTO,
							DESCRIZIONE_OPERATORE = operatore.DescrizioneOperatore
						});
					}

					context.SaveChanges();

					AggiornaSiglaVerbale(intervento.ID_INTERVENTO);

					return Result.Ok<decimal>(intervento.ID_INTERVENTO);
				}
			}
			catch (Exception ex)
			{
				string dettagli = Utils.GetExceptionDetails(ex);
				logger.Error(ex, $"{MethodBase.GetCurrentMethod().Name} - {dettagli}");
				return Result.Fail<decimal>(dettagli);
			}
		}

		internal class RichiestaNuovoInterventoSenzaPrelievoCampioni
		{
			public decimal IdStazione { get; set; }
			public decimal IdTipoIntervento { get; set; }
			public string CodiceArgomento { get; set; }
			public string DescrizioneArgomento { get; set; }
			public DateTime DataIntervento { get; set; }
			public string OraIntervento { get; set; }
			public decimal? IdTipoRichiedente { get; set; }
			public string CodiceCampagna { get; set; }
			public bool PrelievoCampioni { get; set; }
			public string AutoreCreazioneIntervento { get; set; }
			public List<Operatore> OperatoriIntervento { get; set; }
			/// <summary>
			/// Codice dell'organizzazione che ha creato l'intervento. 
			/// E' utilizzato per decidere chi può modificare l'intervento.
			/// </summary>
			public string OrganizzazioneCreazione { get; set; }
			public Result Valida()
			{
				if (IdStazione == 0)
					return Result.Fail("IdStazione non può essere zero.");
				if (IdTipoIntervento == 0)
					return Result.Fail("IdTipoIntervento non può essere zero.");
				if (string.IsNullOrEmpty(CodiceArgomento))
					return Result.Fail("CodiceArgomento non può essere null o vuoto.");
				if (string.IsNullOrEmpty(DescrizioneArgomento))
					return Result.Fail("DescrizioneArgomento non può essere null o vuoto.");
				if (DataIntervento == default(DateTime))
					return Result.Fail("DataIntervento non può essere il valore predefinito.");
				if (string.IsNullOrEmpty(AutoreCreazioneIntervento))
					return Result.Fail("AutoreCreazioneIntervento non può essere null o vuoto.");
				if (string.IsNullOrEmpty(OrganizzazioneCreazione))
					return Result.Fail("OrganizzazioneCreazione non può essere null o vuoto.");
				if (OperatoriIntervento == null || OperatoriIntervento.Count == 0)
					return Result.Fail("OperatoriIntervento deve contenere almeno un elemento.");
				return Result.Ok();
			}

			internal class Operatore
			{
				public decimal IdOperatore { get; set; }
				public string DescrizioneOperatore { get; set; }
			}

		}

		#endregion Nuovo intervento


		/// <summary>
		/// Elenco dei sensori visibili all'interno di un intervento
		/// </summary>
		/// <param name="idStazione"></param>
		/// <returns></returns>
		public static List<InfoSensore> SensoriVisibiliIntervento(decimal idStazione)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				return context.Sensori2021
					.Where(i => i.Strumento.ID_STAZIONE == idStazione && i.Strumento.Tipo.VISIBILE_INTERVENTO)
					.Select(i => new InfoSensore()
					{
						CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
						Grandezza = i.Grandezza.GRANDEZZA,
						IdGrandezza = i.ID_GRANDEZZA_STAZIONE,
						UnitaMisura = i.Grandezza.UNITA_MISURA, // Per i sensori visibili dall'intervento l'unità di misura è quella della grandezza cui sono associati
						NumeroDecimali = i.Grandezza.NUMERO_DECIMALI,
						SeUnitaMisuraBooleana = i.Grandezza.TipoUnitaMisura.SE_BOOLEANO
					})
					.OrderBy(i => i.Grandezza)
					.ToList();
			}
		}

		/// <summary>
		/// Ordina i pacchetti passati mettendo prima quelli già usati in interventi della stazione o associati a strumenti della stazione
		/// </summary>
		/// <param name="elencoPacchettiDaOrdinare"></param>
		/// <param name="idStazione"></param>
		/// <returns></returns>
		public static List<InfoPacchettoPerSelezione> ElencoPacchettiPerSelezione(List<Pacchetto> elencoPacchettiDaOrdinare, decimal idStazione)
		{
			var pacchettiOrdinati = new List<InfoPacchettoPerSelezione>();
			var codiciPacchettiUtilizzatiPerStazione = new List<string>();
			var codiciAlimsPaccchettiStrumentiStazione = new List<string>();
			using (SIASSEntities context = new SIASSEntities())
			{
				codiciPacchettiUtilizzatiPerStazione = context.PacchettiInterventi.Where(i => i.Intervento.ID_STAZIONE == idStazione).Select(i => i.CODICE).ToList();

				codiciAlimsPaccchettiStrumentiStazione = (
					from ss in context.StrumentiStazione
					join ps in context.PacchettiStrumenti on ss.ID_STRUMENTO_STAZIONE equals ps.ID_STRUMENTO_STAZIONE
					join tp in context.TipiPacchetto on ps.ID_PACCHETTO equals tp.ID_PACCHETTO
					where ss.ID_STAZIONE == idStazione
					select tp.CODICE_ALIMS
					).Distinct().ToList();
			}

			foreach (var p in elencoPacchettiDaOrdinare)
			{
				var nuovoPacchetto = new InfoPacchettoPerSelezione
				{
					PackIdentity = p.PackIdentity,
					PackName = p.PackName,
					CodArgomento = p.CodArgomento,
					Sede = p.Sede,
					Ordine = 2
				};
				if (
					codiciPacchettiUtilizzatiPerStazione.Any(i => i == nuovoPacchetto.PackIdentity)
					||
					codiciAlimsPaccchettiStrumentiStazione.Any(i => i == nuovoPacchetto.PackIdentity)
					)
				{
					nuovoPacchetto.Ordine = 1;
				}
				pacchettiOrdinati.Add(nuovoPacchetto);
			}

			return (pacchettiOrdinati.OrderBy(i => i.Ordine).ThenBy(i => i.PackIdentity).ThenBy(i => i.PackName).ToList());
		}

	}
}
