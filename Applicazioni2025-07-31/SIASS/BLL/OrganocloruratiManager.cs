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
using Oracle.ManagedDataAccess.Client;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SIASS.BLL
{
	class OrganocloruratiManager
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public static List<ConteggioImportazioni> ConteggioImportazioniPerStato(string partitaIvaOperatore)
		{
			string query = $"select " +
				"count(stato) totale_per_stato, " +
				"stato " +
				"from V_IMPORT_ORGANOCLORURATI ";

			// Se specificata, si usa la partita iva per filtrare i risultati
			if (!string.IsNullOrWhiteSpace(partitaIvaOperatore))
			{
				query += $"where piva_operatore = '{partitaIvaOperatore}' ";
			}
			query += "group by stato " +
				"order by stato";

			using (SIASSEntities context = new SIASSEntities())
			{
				List<ConteggioImportazioni> elenco = context.Database.SqlQuery<ConteggioImportazioni>(query).ToList();
				return elenco;
			}
		}

		public static void InserisciImportazione(string operatore, string nomeFile, string partitaIvaOperatore)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				ImportazioneOrganoclorurati imp = new ImportazioneOrganoclorurati
				{
					OPERATORE = operatore,
					NOME_FILE = nomeFile,
					STATO = "In corso",
					DATA_RICEZIONE_FILE = DateTime.Now,
					PIVA_OPERATORE = partitaIvaOperatore
				};
				context.ImportazioniOrganoclorurati.Add(imp);
				context.SaveChanges();
			}
		}

		public static List<InfoImportazione> ElencoImportazioni(string stato, string partitaIvaOperatore)
		{
			string query = $"select * from V_IMPORT_ORGANOCLORURATI where STATO = '{stato}' ";

			// Se specificata, si usa la partita iva per filtrare i risultati
			if (!string.IsNullOrWhiteSpace(partitaIvaOperatore))
			{
				query += $"and piva_operatore = '{partitaIvaOperatore}' ";
			}

			query += "order by DATA_RICEZIONE_FILE desc";

			using (SIASSEntities context = new SIASSEntities())
			{
				List<InfoImportazione> elenco = context.Database.SqlQuery<InfoImportazione>(query).ToList();
				return elenco;
			}
		}

		public static List<MisurazioneOrganoclorurati> ElencoMisurazioni(DateTime dataCampionamento, string sito, string piezometro)
		{
			string query = $"select * from sias_all_organoclorurati where trunc(DATA_CAMPIONAMENTO) = TO_DATE('{dataCampionamento:dd/MM/yyyy}','dd/MM/yyyy') and upper(SITO) = upper('{sito}') and upper(PIEZOMETRO) = upper('{piezometro}') order by DATA_CAMPIONAMENTO, PARAMETRO";

			using (SIASSEntities context = new SIASSEntities())
			{
				List<MisurazioneOrganoclorurati> elenco = context.Database.SqlQuery<MisurazioneOrganoclorurati>(query).ToList();
				return elenco;
			}
		}

		public static List<DateTime> ElencoDateMisurazioni(string sito, string piezometro)
		{
			string query = $"select " +
				"distinct trunc(DATA_CAMPIONAMENTO) data " +
				"from sias_all_organoclorurati " +
				$"where upper(sito) = upper('{sito}') and upper(piezometro) = upper('{piezometro}') " +
				"order by data desc";

			using (SIASSEntities context = new SIASSEntities())
			{
				List<DateTime> elenco = context.Database.SqlQuery<DateTime>(query).ToList();
				return elenco;
			}
		}


		/// <summary>
		/// Verifica se esiste la stazione associata ai codici indicati
		/// </summary>
		/// <param name="codiceIdentificativoStazione"></param>
		/// <param name="codiceIdentificativoSito"></param>
		/// <returns></returns>
		public static bool EsisteStazione(string codiceIdentificativoStazione, string codiceIdentificativoSito)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Codice identificativo stazione: {codiceIdentificativoStazione} Codice identificativo sito: {codiceIdentificativoSito}");

			string testoComando = "select 's' from sias_siti " +
				"inner join siass_stazioni on sias_siti.id_sito = siass_stazioni.id_sito " +
				"where " +
				"upper(sias_siti.codice_identificativo) = :codice_identificativo_sito " +
				"and " +
				"upper(siass_stazioni.codice_identificativo) = :codice_identificativo_stazione";

			OracleCommand cmd = new OracleCommand
			{
				BindByName = true,
				CommandText = testoComando
			};

			cmd.Parameters.Add(new OracleParameter("codice_identificativo_sito", codiceIdentificativoSito));
			cmd.Parameters.Add(new OracleParameter("codice_identificativo_stazione", codiceIdentificativoStazione));

			SIASSEntities context = new SIASSEntities();
			string stringaConnessioneDB = context.Database.Connection.ConnectionString;

			OracleDataReader dr = SIASS.SQLHelper.CreaReader(cmd, stringaConnessioneDB);

			var stazioneTrovata = dr.Read();
			if (stazioneTrovata)
			{
				logger.Debug($"Stazione trovata:{codiceIdentificativoStazione}");
			}
			else
			{
				logger.Debug($"Stazione non trovata:{codiceIdentificativoStazione}");
			}
			return stazioneTrovata;
		}

		public static string PartitaIVAStazione(string codiceIdentificativoStazione, string codiceIdentificativoSito)
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Codice identificativo stazione: {codiceIdentificativoStazione} Codice identificativo sito: {codiceIdentificativoSito}");

			string query = $"SELECT " +
				"siass_dati_amministrativi.piva_gestore PartitaIVAStazione " +
				"FROM " +
				"siass_stazioni " +
				"INNER JOIN sias_siti ON siass_stazioni.id_sito = sias_siti.id_sito " +
				"INNER JOIN siass_dati_amministrativi ON siass_stazioni.id_stazione = siass_dati_amministrativi.id_stazione " +
				"WHERE " +
					$"siass_stazioni.codice_identificativo = '{codiceIdentificativoStazione}' " +
				"AND " +
					$"sias_siti.codice_identificativo = '{codiceIdentificativoSito}' " +
				"AND " +
					"siass_dati_amministrativi.piva_gestore IS NOT NULL " +
				"AND " +
					"siass_dati_amministrativi.fine_validita IS NULL";

			using (SIASSEntities context = new SIASSEntities())
			{
				List<string> elenco = context.Database.SqlQuery<string>(query).ToList();
				if (elenco.Count > 0)
				{
					string partitaIVA = elenco.FirstOrDefault();
					logger.Debug($"Partita IVA trovata;{partitaIVA}");
					return partitaIVA;
				}

				else
					logger.Debug($"Partita IVA non trovata.");
				return null;
			}
		}


		public static EsitoParsingFile ParsingFile(string file)
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name} - File: {file}");

			#region Indice a base zero delle colonne del file Excel

			const int SITO = 0;
			const int PIEZOMETRO = 1;

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
						var stazioneSito = new StazioneSito
						{
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
							stazioneSito.Sito = parser[SITO].Trim().ToUpper();

						// piezometro - obbligatorio
						if (String.IsNullOrEmpty(parser[PIEZOMETRO]))
						{
							string errore = $"Riga {numeroRigaCorrente} Colonna {PIEZOMETRO + 1}: valore 'piezometro' mancante";
							logger.Warn(errore);
							esito.ErroriParsing.Add(errore);
							recordValido = false;
						}
						else
							stazioneSito.Stazione = parser[PIEZOMETRO].Trim().ToUpper();


						if (recordValido)
							esito.StazioniSiti.Add(stazioneSito);
						else
						{
							logger.Warn($"Riga {numeroRigaCorrente} non valida");
							break;
						}
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




		public class ConteggioImportazioni
		{
			public int TOTALE_PER_STATO { get; set; }
			public string STATO { get; set; }
			public string NUMERO_PER_STATO { get { return $"{STATO} ({TOTALE_PER_STATO})"; } }
		}

		public class InfoImportazione
		{
			public decimal ID_IMPORTAZIONE { get; set; }
			public string NOME_FILE { get; set; }
			public string STATO { get; set; }
			public string OPERATORE { get; set; }
			public DateTime DATA_RICEZIONE_FILE { get; set; }
			public string RAGIONE_SOCIALE { get; set; }
		}

		public class StazioneSito
		{
			public string Stazione { get; set; }
			public string Sito { get; set; }
			public string PartitaIVAStazione { get; set; }
		}

		internal class EsitoParsingFile
		{
			/// <summary>
			/// Il parsing è riuscito se non c'è un errore nell'accesso al file
			/// e nessuna misurazione presenta errori
			/// </summary>
			public bool Riuscito
			{
				get
				{
					return string.IsNullOrEmpty(ErroreFile) && !ErroriParsing.Any();
				}
			}
			/// <summary>
			/// Errore generico nell'accesso al file
			/// </summary>
			public string ErroreFile { get; set; }

			/// <summary>
			/// Elenco errori di parsing
			/// </summary>
			public List<string> ErroriParsing = new List<string>();

			/// <summary>
			/// StazioniSito risultato del parsing
			/// </summary>
			public List<StazioneSito> StazioniSiti { get; set; } = new List<StazioneSito>();
		}

	}
}