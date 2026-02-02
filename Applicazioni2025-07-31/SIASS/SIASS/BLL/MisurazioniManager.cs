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
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace SIASS.BLL
{
	static class MisurazioniManager
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// Ritorna i tipi di unità di misura usati nelle misurazioni di una centralina
		/// </summary>
		/// <param name="idCaratteristicheCentralina"></param>
		/// <returns></returns>
		public static List<TipoUnitaMisura> ElencoUnitaMisura(decimal idCaratteristicheCentralina)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elencoIdTipiUnitaMisuraPerCentralina = context.Misurazioni
					.Where(i => i.IdCaratteristicheCentralina == idCaratteristicheCentralina)
					.Select(i => i.IdTipoUnitaMisura)
					.Distinct();

				var elenco = context.TipiUnitaMisura
					.Where(i => elencoIdTipiUnitaMisuraPerCentralina.Contains(i.IdTipoUnitaMisura))
					.OrderBy(i => i.Grandezza)
					.ToList();

				return elenco;
			}
		}

		/// <summary>
		/// Elenco misurazioni
		/// </summary>
		/// <param name="idCaratteristicheCentralina"></param>
		/// <param name="idTipoUnitaMisura"></param>
		/// <param name="dataInizio"></param>
		/// <param name="dataFine"></param>
		/// <param name="statoValidazione"></param>
		/// <param name="ritornaTutte">se false ritorna al massimo il numero di misurazioni indicato in configurazione</param>
		/// <param name="conteggio"></param>
		/// <returns></returns>
		public static List<Misurazione> ElencoMisurazioni(
			decimal idCaratteristicheCentralina,
			decimal idTipoUnitaMisura,
			DateTime dataInizio,
			DateTime dataFine,
			decimal? statoValidazione,
			bool ritornaTutte,
			out int conteggio
			)
		{
			using (SIASSEntities context = new SIASSEntities())
			{

				var elenco = context.Misurazioni
					.Where(
					i => i.IdCaratteristicheCentralina == idCaratteristicheCentralina
						&&
						i.IdTipoUnitaMisura == idTipoUnitaMisura
						&&
						DbFunctions.TruncateTime(i.DataMisurazione).Value >= dataInizio.Date
						&&
						DbFunctions.TruncateTime(i.DataMisurazione).Value <= dataFine.Date
						);

				if (statoValidazione.HasValue)
					elenco = elenco.Where(i => i.Validata == statoValidazione);

				conteggio = elenco.Count();

				if (ritornaTutte)
				{
					elenco = elenco
						.OrderBy(i => i.DataMisurazione)
						;
				}
				else
				{
					elenco = elenco
						.OrderBy(i => i.DataMisurazione)
						.Take(int.Parse(ConfigurationManager.AppSettings["NumeroMassimoMisurazioni"]));
				}

				return elenco.ToList();
			}
		}

		/// <summary>
		/// Ritorna per la centralina le date della prima e dell'ultima misurazione il conteggio delle misurazioni
		/// </summary>
		/// <param name="idCaratteristicheCentralina"></param>
		/// <param name="idTipoUnitaMisura"></param>
		/// <param name="dataPrima"></param>
		/// <param name="dataUltima"></param>
		/// <param name="conteggio"></param>
		public static void DatePrimaUltimaMisurazione(
			decimal idCaratteristicheCentralina,
			decimal idTipoUnitaMisura,
			out DateTime? dataPrima,
			out DateTime? dataUltima,
			out int? conteggio
			)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				dataPrima = null;
				dataUltima = null;
				conteggio = null;

				conteggio = context.Misurazioni
					.Count(i => i.IdCaratteristicheCentralina == idCaratteristicheCentralina
					&&
					i.IdTipoUnitaMisura == idTipoUnitaMisura);

				if (conteggio > 0)
				{
					dataPrima = context.Misurazioni
						.Where(
							i => i.IdCaratteristicheCentralina == idCaratteristicheCentralina
							&&
							i.IdTipoUnitaMisura == idTipoUnitaMisura
							)
						.Min(i => i.DataMisurazione);

					dataUltima = context.Misurazioni
						.Where(
						i => i.IdCaratteristicheCentralina == idCaratteristicheCentralina
						&&
						i.IdTipoUnitaMisura == idTipoUnitaMisura
						)
						.Max(i => i.DataMisurazione);
				}

			}
		}

		/// <summary>
		/// Aggiorna lo stato delle misurazioni i cui id sono nell'elenco passato
		/// </summary>
		/// <param name="idMisurazioni"></param>
		/// <param name="stato"></param>
		public static void AggiornaStatoMisurazioni(List<decimal> idMisurazioni, decimal stato)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elencoMisurazioniDaAggiornare = context.Misurazioni
					.Where(i => idMisurazioni.Contains(i.IdMisurazione)).ToList();

				foreach (Misurazione m in elencoMisurazioniDaAggiornare)
				{
					m.Validata = stato;
				}
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Ritorna grandezza e unità misura per il tipo richiesto
		/// </summary>
		/// <param name="idTipoUnitaMisura"></param>
		/// <returns></returns>
		public static string DescrizioneUnitaMisuraCompleta(decimal idTipoUnitaMisura)
		{
			string descrizione = null;

			using (SIASSEntities context = new SIASSEntities())
			{
				var tipoUnitaMisura = context.TipiUnitaMisura
					.Where(i => i.IdTipoUnitaMisura == idTipoUnitaMisura)
					.FirstOrDefault();
				if (tipoUnitaMisura != null)
					descrizione = String.Format("{0} ({1})", tipoUnitaMisura.Grandezza, tipoUnitaMisura.DescrizioneTipoUnitaMisura);
			}

			return descrizione;
		}

		/// <summary>
		/// Imposta tutte le misurazioni da validare a "valida"
		/// </summary>
		/// <param name="idCaratteristicheCentralina"></param>
		/// <returns></returns>
		public static int ImpostaTutteValida(decimal idCaratteristicheCentralina)
		{
			int numeroValidate = 0;

			using (SIASSEntities context = new SIASSEntities())
			{
				string sql = "UPDATE SIASS_MISURAZIONI " +
					 "SET VALIDATA = 1 " +
					 "WHERE " +
					 "ID_CARATT_CENTRALINA = " + idCaratteristicheCentralina.ToString() +
					 " AND " +
					 "VALIDATA = 0";

				numeroValidate = context.Database.ExecuteSqlCommand(sql);
			}

			return numeroValidate;
		}

		// Funzioni per la versione 2021 (tabelle SIAS_)

		#region Misurazioni 2021

		/// <summary>
		/// Ritorna per la grandezza le date della prima e dell'ultima misurazione e il conteggio delle misurazioni
		/// </summary>
		public static void DatePrimaUltimaMisurazione2021(
			decimal idGrandezzaStazione,
			out DateTime? dataPrima,
			out DateTime? dataUltima,
			out int? conteggio
			)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				dataPrima = null;
				dataUltima = null;
				conteggio = null;

				conteggio = context.Misurazioni2021
					.Count(i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione
					);

				if (conteggio > 0)
				{
					dataPrima = context.Misurazioni2021
						.Where(
							i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione
							)
						.Min(i => i.DATA_MISURAZIONE);

					dataUltima = context.Misurazioni2021
						.Where(
							i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione
						)
						.Max(i => i.DATA_MISURAZIONE);
				}

			}
		}

		/// <summary>
		/// Elenco misurazioni
		/// </summary>
		/// <param name="dataInizio"></param>
		/// <param name="dataFine"></param>
		/// <param name="statoValidazione"></param>
		/// <param name="ritornaTutte">se false ritorna al massimo il numero di misurazioni indicato in configurazione</param>
		/// <param name="conteggio"></param>
		/// <returns></returns>
		public static List<InfoMisurazionePerValidazione> ElencoMisurazioni2021(
			decimal idGrandezzaStazione,
			DateTime dataInizio,
			DateTime dataFine,
			decimal? statoValidazione,
			bool ritornaTutte,
			decimal? valoreMinimo,
			decimal? valoreMassimo,
			out int conteggio
			)
		{
			using (SIASSEntities context = new SIASSEntities())
			{

				var elenco = context.Misurazioni2021
					.Where(
					i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione
						&&
						DbFunctions.TruncateTime(i.DATA_MISURAZIONE).Value >= dataInizio.Date
						&&
						DbFunctions.TruncateTime(i.DATA_MISURAZIONE).Value <= dataFine.Date
						);

				if (statoValidazione.HasValue)
					elenco = elenco.Where(i => i.VALIDATA == statoValidazione);

				if (valoreMinimo.HasValue)
					elenco = elenco.Where(i => i.VALORE >= valoreMinimo);
				if (valoreMassimo.HasValue)
					elenco = elenco.Where(i => i.VALORE <= valoreMassimo);

				conteggio = elenco.Count();

				if (ritornaTutte)
				{
					elenco = elenco
						.OrderBy(i => i.DATA_MISURAZIONE)
						;
				}
				else
				{
					elenco = elenco
						.OrderBy(i => i.DATA_MISURAZIONE)
						.Take(int.Parse(ConfigurationManager.AppSettings["NumeroMassimoMisurazioni"]));
				}

				return elenco.Select(i => new InfoMisurazionePerValidazione()
				{
					IdMisurazione = i.ID_MISURAZIONE,
					DataMisurazione = i.DATA_MISURAZIONE,
					Valore = i.VALORE,
					Validata = i.VALIDATA,
					Spike = false,
					Plateau = false,
					Intervento = i.ID_INTERVENTO.HasValue
				}).ToList();
			}
		}

		public static void AggiornaStatoMisurazioni2021(List<decimal> idMisurazioni, decimal stato, string autoreUltimoAggiornamento)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elencoMisurazioniDaAggiornare = context.Misurazioni2021
					.Where(i => idMisurazioni.Contains(i.ID_MISURAZIONE)).ToList();

				foreach (Misurazione2021 m in elencoMisurazioniDaAggiornare)
				{
					m.VALIDATA = stato;
					m.ULTIMO_AGGIORNAMENTO = DateTime.Now;
					m.AUTORE_ULTIMO_AGGIORNAMENTO = autoreUltimoAggiornamento;
				}
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Inserisce, aggiorna o elimina una misurazione inserita tramite intervento
		/// </summary>
		/// <param name="dataMisurazione"></param>
		/// <param name="validata"></param>
		/// <param name="idGrandezzaStazione"></param>
		/// <param name="ultimoAggiornamento"></param>
		/// <param name="autoreUltimoAggiornamento"></param>
		/// <param name="idIntervento"></param>
		/// <param name="codiceIdentificativoSensore"></param>
		/// <param name="valore"></param>
		/// <param name="fonteArpal"></param>
		/// <param name="seNuovaMisurazione">indica se la misuraizone è sicuramente nuova, quindi non va cercata per aggiornarla</param>
		public static void InserisceAggiornaEliminaMisurazioneIntervento(
			DateTime dataMisurazione,
			decimal idGrandezzaStazione,
			DateTime ultimoAggiornamento,
			string autoreUltimoAggiornamento,
			decimal idIntervento,
			string codiceIdentificativoSensore,
			decimal? valore,
			bool fonteArpal,
			string codicePacchetto
			)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				Misurazione2021 m = null;

				// Cerca la misurazione
				if (String.IsNullOrWhiteSpace(codiceIdentificativoSensore))
				{
					logger.Debug($"Ricerca misurazione idGrandezzaStazione:{idGrandezzaStazione}, idIntervento:{idIntervento}");
					m = context.Misurazioni2021.Where
						(i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione && i.ID_INTERVENTO == idIntervento)
						.FirstOrDefault();
				}
				else
				{
					logger.Debug($"Ricerca misurazione idGrandezzaStazione:{idGrandezzaStazione}, idIntervento:{idIntervento}, codiceIdentificativoSensore:{codiceIdentificativoSensore}");
					m = context.Misurazioni2021.Where
						(i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione && i.ID_INTERVENTO == idIntervento && i.CODICE_IDENTIFICATIVO_SENSORE == codiceIdentificativoSensore)
						.FirstOrDefault();
				}
				if (m != null)
				{
					// Se la trova
					logger.Debug($"Misurazione trovata idMisurazione:{m.ID_MISURAZIONE}");
					// Se c'è il valore la aggiorna
					if (valore.HasValue)
					{
						decimal? valoreSensore = null;
						if (!String.IsNullOrWhiteSpace(codiceIdentificativoSensore))
							valoreSensore = valore.Value;
						logger.Debug($"E' stato specificato un valore - Aggiornamento misurazione {m.ID_MISURAZIONE}");
						m.DATA_MISURAZIONE = dataMisurazione;
						m.VALORE = valore.Value;
						m.ULTIMO_AGGIORNAMENTO = ultimoAggiornamento;
						m.AUTORE_ULTIMO_AGGIORNAMENTO = autoreUltimoAggiornamento;
						m.VALORE_SENSORE = valoreSensore;
						m.FONTE_ARPAL = fonteArpal;
						m.PACCHETTO_CODICE = codicePacchetto;
						context.SaveChanges();
					}
					else
					{
						// Se non c'è il valore la elimina
						logger.Debug($"Non è stato specificato un valore - Eliminazione misurazione {m.ID_MISURAZIONE}");
						context.Misurazioni2021.Remove(m);
						context.SaveChanges();
					}
				}
				else
				{
					logger.Debug($"Misurazione nuova");
					if (valore.HasValue)
					{
						// Se c'è il valore la inserisce
						logger.Debug($"E' stato specificato un valore");
						decimal? valoreSensore = null;
						if (!String.IsNullOrWhiteSpace(codiceIdentificativoSensore))
							valoreSensore = valore.Value;
						var nuovaMisurazione = new Misurazione2021
						{
							DATA_MISURAZIONE = dataMisurazione,
							VALORE = valore.Value,
							ID_GRANDEZZA_STAZIONE = idGrandezzaStazione,
							ULTIMO_AGGIORNAMENTO = ultimoAggiornamento,
							AUTORE_ULTIMO_AGGIORNAMENTO = autoreUltimoAggiornamento,
							ID_INTERVENTO = idIntervento,
							CODICE_IDENTIFICATIVO_SENSORE = codiceIdentificativoSensore,
							VALORE_SENSORE = valoreSensore,
							FONTE_ARPAL = fonteArpal,
							PACCHETTO_CODICE = codicePacchetto
						};
						context.Misurazioni2021.Add(nuovaMisurazione);
						context.SaveChanges();
						logger.Debug($"Inserita misurazione {nuovaMisurazione.ID_MISURAZIONE}");
					}
					else
					{
						// Altrimenti la ignora
						logger.Debug($"Non è stato specificato un valore - Misurazione non inserita.");
					}
				}
			}
		}


		/// <summary>
		/// Rimuove tutte le misurazioni relative ai codici pacchetti rimossi
		/// </summary>
		/// <param name="codiciPacchettiRimossi"></param>
		public static void EliminaMisurazioniPerPacchettiRimossi(List<string> codiciPacchettiRimossi)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var misurazioniDaRimuovere = context.Misurazioni2021
					.Where(m => codiciPacchettiRimossi.Contains(m.PACCHETTO_CODICE)).ToList();
				context.Misurazioni2021.RemoveRange(misurazioniDaRimuovere);
				context.SaveChanges();
				var parametriDiCampoDaRimuovere = context.ParametriCampoInterventi
					.Where(p => codiciPacchettiRimossi.Contains(p.PACCHETTO_CODICE)).ToList();
				context.ParametriCampoInterventi.RemoveRange(parametriDiCampoDaRimuovere);
				context.SaveChanges();
			}
		}

		#endregion
	}
}