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
using Oracle.ManagedDataAccess.Client;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SIASS.BLL
{
	public static class EsportazioneVerbaleManager
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		public static List<VerbalePerEsportazione> ElencoVerbali()
		{
			logger.Info($"{MethodBase.GetCurrentMethod().Name}");

			List<VerbalePerEsportazione> elencoVerbali = new List<VerbalePerEsportazione>();

			foreach (var idIntervento in ElencoIdInterventiDaPubblicare())
			{
				VerbalePerEsportazione verbale = CreaVerbaleIntervento(idIntervento);
				if (verbale != null)
					elencoVerbali.Add(verbale);
			}

			return elencoVerbali;
		}

		private static VerbalePerEsportazione CreaVerbaleIntervento(decimal idIntervento)
		{
			const string formatoDataOra = "yyyy-MM-ddTHH:mm:sszzz";

			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdIntervento:{idIntervento}");

			var verbale = new VerbalePerEsportazione();

			// Estrazione informazioni verbale dall'intervento
			var infoIntervento = InterventoManager.CaricaInfoIntervento(idIntervento);

			verbale.SiglaVerbale = infoIntervento.SiglaVerbale;

			verbale.DataOraVerbale = infoIntervento.DataIntervento.ToString(formatoDataOra);

			verbale.NumeroCampioniRilevati = infoIntervento.NumeroCampioni;

			using (SIASSEntities context = new SIASSEntities())
			{
				var richiedente = context.TipiRichiedente.FirstOrDefault(i => i.DESCRIZIONE_TIPO_RICHIEDENTE == infoIntervento.DescrizioneTipoRichiedente);
				if (richiedente != null)
					verbale.CodiceRichiedente = richiedente.CODICE_ALIMS;

				verbale.CodiceArgomento = infoIntervento.CodiceArgomento;

				foreach (var pacchetto in infoIntervento.PacchettiIntervento)
					verbale.CodiciPacchetto.Add(pacchetto.CodicePacchetto);

				// Se la stazione è della rete organoclorurati va indicato il codice siral (da localizzazione) invece del codice stazione
				verbale.CodiceStazione = infoIntervento.CodiceIdentificativoStazione;
				InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(infoIntervento.IdStazione);
				if (infoStazione.RetiAppartenenza.Contains("ORGANOCLORURATI", StringComparer.InvariantCultureIgnoreCase))
				{
					if (infoStazione.Localizzazione == null || string.IsNullOrEmpty(infoStazione.Localizzazione.CodiceSIRAL))
					{
						logger.Warn($"Generazione verbale IdStazione {infoStazione.IdStazione}: la stazione appartiene alla rete Organoclorurati ma il codice SIRAL nella sezione Localizzazione della stazione non è presente.");
						verbale.CodiceStazione = "Codice SIRAL non disponibile";
					}
					else
					{
						verbale.CodiceStazione = infoStazione.Localizzazione.CodiceSIRAL;
					}
				}
				else
				{
					verbale.CodiceStazione = infoIntervento.CodiceIdentificativoStazione;
				}

				verbale.DataOraIntervento = infoIntervento.DataIntervento.ToString(formatoDataOra);

				// Codice ALIMS del primo operatore; è usata la matricola del primo operatore su ANSOLO
				var elencoOperatori = InterventoManager.ElencoOperatoriIntervento(idIntervento);
				if (elencoOperatori != null && elencoOperatori.Count > 0)
				{
					int idOperatore = (int)elencoOperatori.OrderBy(i => i.DescrizioneOperatore).First().IdOperatore;
					bool matricolaDaAnsolo = ANSOLOManager.MatricolaAnsolo(idOperatore, out string matricola);

					if (matricolaDaAnsolo)
					{
						verbale.Prelevatore = matricola.PadLeft(6, '0'); //Per alims la matricola viene paddata a 6 caratteri con 0 davanti
						logger.Debug($"Invio ad alims con matricola {verbale.Prelevatore}");
					}
					else
					{
						logger.Warn($"Generazione verbale IdIntervento {idIntervento} - Matricola operatore Id:{idOperatore} non trovata su ANSOLO");
						verbale.Prelevatore = "Matricola non disponibile";
					}
				}
				else
				{
					logger.Warn($"Generazione verbale IdIntervento {idIntervento} - operatori non presenti nell'intervento");
					verbale.Prelevatore = "Matricola non disponibile";
				}

				verbale.Matrice = infoIntervento.CodiceMatrice;

				verbale.Annotazioni = infoIntervento.Annotazioni;
				verbale.Misurazioni = MisurazioniPerIntervento(idIntervento);

				// Analiti
				foreach (var analita in InterventoManager.AnalitiPerIntervento(idIntervento))
				{
					verbale.Analiti.Add(new VerbalePerEsportazione.Analita
					{
						Codice = analita.Codice,
						Descrizione = analita.Descrizione,
						CodiceMetodo = analita.CodiceMetodo,
						CodicePacchetto = analita.CodicePacchetto
					});
				}

				verbale.Analiti = verbale.Analiti.OrderBy(i => i.CodicePacchetto).ThenBy(i => i.Descrizione).ToList();

				verbale.QuotaCampione = infoIntervento.QuotaCampione;

				verbale.CampioneBianco = infoIntervento.CampioneBianco;

				verbale.DatiCampioneBianco = infoIntervento.DatiCampioneBianco;

				// Carica da file il PDF del verbale codificato in base64
				string percorsoFile = $"{ConfigurationManager.AppSettings["CartellaVerbaliInterventi"].Replace("[ID_STAZIONE]", infoStazione.IdStazione.ToString())}Verbale{idIntervento}.pdf";
				if (!System.IO.File.Exists(percorsoFile))
				{
					logger.Error($"Generazione verbale IdIntervento {idIntervento} - file verbale non trovato");
					return null;
				}
				verbale.FileVerbale = Convert.ToBase64String(System.IO.File.ReadAllBytes(percorsoFile));
				verbale.NomeFileVerbale = $"{infoIntervento.SiglaVerbale}.pdf";
			}

			return verbale;
		}

		private static List<decimal> ElencoIdInterventiDaPubblicare()
		{
			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Esecuzione della query per ottenere gli interventi da esportare.");

			// Sono selezionati gli interventi che non compaiono in quelli della vista alims_interventi aventi codice campione non nullo
			string testoComando = @"
                select id_intervento 
                from sias_interventi 
                where creazione_verbale is not null 
                and sias_interventi.prelievo_campioni = 1
                and sigla_verbale not in
                (
                    select sigla_verbale 
                    from alims_interventi 
                    where codice_campione is not null
                )";

			List<decimal> elencoId = new List<decimal>();

			OracleCommand cmd = new OracleCommand
			{
				BindByName = true,
				CommandText = testoComando
			};

			SIASSEntities context = new SIASSEntities();
			string stringaConnessioneDB = context.Database.Connection.ConnectionString;

			cmd.Connection = new OracleConnection(stringaConnessioneDB);
			cmd.Connection.Open();

			DataSet dataSet = new DataSet();

			using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
			{
				dataAdapter.SelectCommand = cmd;
				dataAdapter.Fill(dataSet);
			}

			cmd.Connection.Close();

			if (dataSet.Tables[0].Rows.Count > 0)
			{
				elencoId = (from DataRow dr in dataSet.Tables[0].Rows
							select Convert.ToDecimal(dr["id_intervento"])).ToList();
			}

			logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Trovati {elencoId.Count} interventi: {string.Join(",", elencoId)}");

			return elencoId;
		}




		public static List<VerbalePerEsportazione.Misurazione> MisurazioniPerIntervento(decimal idIntervento)
		{
			var elencoMisurazioni = new List<VerbalePerEsportazione.Misurazione>();
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.Misurazioni2021
					.Where(i => i.ID_INTERVENTO == idIntervento)
					.Select(i => new InfoMisurazione()
					{
						IdMisurazione = i.ID_MISURAZIONE,
						Grandezza = i.Grandezza.GRANDEZZA,
						IdGrandezzaStazione = i.ID_GRANDEZZA_STAZIONE,
						Valore = i.VALORE
					})
					.OrderBy(i => i.Grandezza)
					.ToList();

				// Recupera il codice metodo per ogni misurazione
				foreach (var misurazione in elenco)
				{
					var misurazionePerEsportazione = new VerbalePerEsportazione.Misurazione();
					misurazionePerEsportazione.Valore = misurazione.Valore;
					var par = context.ParametriCampoInterventi.Where(i => i.ID_GRANDEZZA_STAZIONE == misurazione.IdGrandezzaStazione && i.ID_INTERVENTO == idIntervento).FirstOrDefault();
					if (par != null)
					{
						misurazionePerEsportazione.CodiceAnalita = par.CODICE;
						misurazionePerEsportazione.CodiceMetodo = par.METODO_CODICE;
						misurazionePerEsportazione.CodicePacchetto = par.PACCHETTO_CODICE;
					}
					else
						logger.Warn($"Codice metodo non trovato per misurazione:{misurazione.IdMisurazione}");
					elencoMisurazioni.Add(misurazionePerEsportazione);
				}

				return elencoMisurazioni.OrderBy(i => i.CodiceAnalita).ToList();
			}
		}



	}







	/// <summary>
	/// Dati del verbale da esportare mediante enpoint REST
	/// </summary>
	public class VerbalePerEsportazione
	{
		public string SiglaVerbale { get; set; }
		public string DataOraVerbale { get; set; }
		public decimal? NumeroCampioniRilevati { get; set; }
		public string CodiceRichiedente { get; set; }
		public string CodiceArgomento { get; set; }
		public List<string> CodiciPacchetto { get; set; } = new List<string>();
		public string CodiceStazione { get; set; }
		public string DataOraIntervento { get; set; }
		public string Prelevatore { get; set; }
		public string Matrice { get; set; }
		public string Annotazioni { get; set; }
		public List<Misurazione> Misurazioni { get; set; } = new List<Misurazione>();
		public List<Analita> Analiti { get; set; } = new List<Analita>();
		public decimal? QuotaCampione { get; set; }
		public bool CampioneBianco { get; set; }
		public string DatiCampioneBianco { get; set; }
		public string NomeFileVerbale { get; set; }
		public string FileVerbale { get; set; }
		public class Misurazione
		{
			public string CodiceAnalita { get; set; }
			public decimal Valore { get; set; }
			public string CodiceMetodo { get; set; }
			public string CodicePacchetto { get; set; }
		}
		public class Analita
		{
			public string Codice { get; set; }
			public string Descrizione { get; set; }
			public string CodiceMetodo { get; set; }
			public string CodicePacchetto { get; set; }
		}

	}
}