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
using Oracle.ManagedDataAccess.Client;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace SIASS.BLL
{
	class StazioneManager
	{
		/// <summary>
		/// Carica info stazione complete delle versioni attive delle sezioni correlati
		/// </summary>
		/// <param name="idStazione"></param>
		/// <returns></returns>
		internal static InfoStazione CaricaInfoStazione(decimal idStazione)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var infoStazione = (from s in context.Stazioni
									where s.IdStazione == idStazione
									select new InfoStazione
									{
										IdStazione = s.IdStazione,
										CodiceIdentificativo = s.CodiceIdentificativo,
										Descrizione = s.Descrizione,
										EsclusaMonitoraggio = s.EsclusaMonitoraggio,
										IdTipoStazione = s.IdTipoStazione,
										DescrizioneTipoStazione = s.TipoStazione.DescrizioneTipoStazione,
										IdAllegatoFotoStazione = s.IdAllegatoFotoStazione,
										IdAllegatoMappa = s.IdAllegatoMappa,
										NomeFileAllegatoFotoStazione = s.IdAllegatoFotoStazione != null ? s.AllegatoFotoStazione.NomeFileAllegato : null,
										NomeFileAllegatoMappa = s.IdAllegatoMappa != null ? s.AllegatoMappa.NomeFileAllegato : null,
										Annotazioni = s.Annotazioni,
										Categoria = s.TipoStazione.Categoria,
										Allestimento = s.Allestimento,
										IdSito = s.IdSito,
										CodiceIdentificativoSito = s.IdSito != null ? s.Sito.CODICE_IDENTIFICATIVO : null,
										DescrizioneSito = s.IdSito != null ? s.Sito.DESCRIZIONE : null,
										ComuneSito = s.IdSito != null ? s.Sito.Comune.DenominazioneComune : null,
										ProvinciaSito = s.IdSito != null ? s.Sito.Comune.Provincia.SiglaProvincia : null,
										Teletrasmissione = s.Teletrasmissione,
										IndirizzoSito = s.Sito.INDIRIZZO,
										PuntoConformita = s.PUNTO_CONFORMITA,
										ControlloAnomalie = s.CONTROLLO_ANOMALIE
									}).FirstOrDefault();

				if (infoStazione != null)
					infoStazione.Finalita = FinalitaStazione(idStazione);

				// Aggiunta info sezioni correlate
				if (infoStazione != null)
				{
					infoStazione.Localizzazione = LocalizzazioneManager.LocalizzazioneAttiva(infoStazione.IdStazione, context);
					infoStazione.CaratteristicheTecnichePozzo = CaratteristicheTecnichePozzoManager.CaratteristicheTecnichePozzoAttive(infoStazione.IdStazione, context);
					infoStazione.CaratteristicheInstallazione = CaratteristicheInstallazioneManager.CaratteristicheInstallazioneAttive(infoStazione.IdStazione, context);
					infoStazione.CaratteristicheCentralina = CaratteristicheCentralinaManager.CaratteristicheCentralinaAttive(infoStazione.IdStazione, context);
					infoStazione.DatiAmministrativi = DatiAmministrativiManager.DatiAmministrativAttivi(infoStazione.IdStazione, context);
					infoStazione.UltimoIntervento = InterventoManager.UltimoIntervento(infoStazione.IdStazione, context);
					infoStazione.RetiAppartenenza = context.StazioniReti.Where(i => i.ID_STAZIONE == infoStazione.IdStazione).Select(i => i.RETE).OrderBy(i => i).ToList();
				}

				return infoStazione;
			}
		}

		/// <summary>
		/// Report scheda stazione
		/// </summary>
		/// <param name="rv"></param>
		/// <param name="idStazione"></param>
		public static void GeneraReportStazione(ReportViewer rv, decimal idStazione)
		{
			InfoStazione infoStazione = StazioneManager.CaricaInfoStazione(idStazione);

			if (infoStazione != null)
			{
				rv.Reset();

				// Impostazione report
				rv.LocalReport.ReportPath = @"rdlc\SchedaStazione.rdlc";
				rv.LocalReport.DisplayName = "Stazione" + infoStazione.CodiceIdentificativo;
				// Abilita l'uso di immagini esterne con url
				rv.LocalReport.EnableExternalImages = true;

				// Parametri

				// Stazione
				string esclusaMonitoraggio = "No";
				if (infoStazione.EsclusaMonitoraggio) esclusaMonitoraggio = "Sì";
				string teletrasmissione = "No";
				if (infoStazione.Teletrasmissione) teletrasmissione = "Sì";
				string puntoConformita = "No";
				if (infoStazione.PuntoConformita) puntoConformita = "Sì";

				// Immagini
				string ImmagineStazione = string.Empty;
				string ImmagineMappa = string.Empty;
				if (infoStazione.IdAllegatoFotoStazione.HasValue)
					ImmagineStazione = new Uri(HttpContext.Current.Server.MapPath("~/File/Allegati/Stazione" + infoStazione.IdStazione.ToString() + "/" + infoStazione.NomeFileAllegatoFotoStazione)).AbsoluteUri;
				if (infoStazione.IdAllegatoMappa.HasValue)
					ImmagineMappa = new Uri(HttpContext.Current.Server.MapPath("~/File/Allegati/Stazione" + infoStazione.IdStazione.ToString() + "/" + infoStazione.NomeFileAllegatoMappa)).AbsoluteUri;

				// Localizzazione
				string DenominazioneComune = null;
				string DenominazioneProvincia = null;
				string Localita = null;
				string DescrizioneBacino = null;
				string DescrizioneCorpoIdrico = null;
				string CTR = null;
				string Latitudine = null;
				string Longitudine = null;
				string LatitudineGaussBoaga = null;
				string LongitudineGaussBoaga = null;
				string QuotaPianoCampagna = null;
				string CodiceSIRAL = null;
				if (infoStazione.Localizzazione != null)
				{
					DenominazioneComune = infoStazione.Localizzazione.DenominazioneComune;
					DenominazioneProvincia = infoStazione.Localizzazione.DenominazioneProvincia;
					Localita = infoStazione.Localizzazione.Localita;
					DescrizioneBacino = infoStazione.Localizzazione.DescrizioneBacino;
					DescrizioneCorpoIdrico = infoStazione.Localizzazione.DescrizioneCorpoIdrico;
					CTR = infoStazione.Localizzazione.CTR;
					Latitudine = infoStazione.Localizzazione.Latitudine.ToString();
					Longitudine = infoStazione.Localizzazione.Longitudine.ToString();
					LatitudineGaussBoaga = infoStazione.Localizzazione.LatitudineGaussBoaga.ToString();
					LongitudineGaussBoaga = infoStazione.Localizzazione.LongitudineGaussBoaga.ToString();
					if (infoStazione.Localizzazione.QuotaPianoCampagna.HasValue)
						QuotaPianoCampagna = infoStazione.Localizzazione.QuotaPianoCampagna.Value.ToString();
					CodiceSIRAL = infoStazione.Localizzazione.CodiceSIRAL;
				}

				// Caratteristiche tecniche
				string Profondita = null;
				string Diametro = null;
				string RangeSoggiacenzaDa = null;
				string RangeSoggiacenzaA = null;
				string DescrizioneTipoChiusura = null;
				string QuotaBoccapozzoPc = null;
				string QuotaBoccapozzoSlm = null;
				string QuotaPianoRiferimentoSlm = null;
				string DifferenzaPrpc = null;
				string ProfonditaEmungimento = null;
				string PortataMassimaEsercizio = null;
				string PresenzaForoSonda = "No";
				string DescrizioneTipoDestinazioneuso = null;
				string DescrizioneTipoFrequenzaUtilizzo = null;
				string Captata = "No";
				if (infoStazione.CaratteristicheTecnichePozzo != null)
				{
					if (infoStazione.CaratteristicheTecnichePozzo.Profondita.HasValue)
						Profondita = infoStazione.CaratteristicheTecnichePozzo.Profondita.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.Diametro.HasValue)
						Diametro = infoStazione.CaratteristicheTecnichePozzo.Diametro.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaDa.HasValue)
						RangeSoggiacenzaDa = infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaDa.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaA.HasValue)
						RangeSoggiacenzaA = infoStazione.CaratteristicheTecnichePozzo.RangeSoggiacenzaA.Value.ToString();
					DescrizioneTipoChiusura = infoStazione.CaratteristicheTecnichePozzo.DescrizioneTipoChiusura;
					if (infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoPc.HasValue)
						QuotaBoccapozzoPc = infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoPc.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoSlm.HasValue)
						QuotaBoccapozzoSlm = infoStazione.CaratteristicheTecnichePozzo.QuotaBoccapozzoSlm.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm.HasValue)
						QuotaPianoRiferimentoSlm = infoStazione.CaratteristicheTecnichePozzo.QuotaPianoRiferimentoSlm.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.DifferenzaPrpc.HasValue)
						DifferenzaPrpc = infoStazione.CaratteristicheTecnichePozzo.DifferenzaPrpc.Value.ToString(); if (infoStazione.CaratteristicheTecnichePozzo.ProfonditaEmungimento.HasValue)
						ProfonditaEmungimento = infoStazione.CaratteristicheTecnichePozzo.ProfonditaEmungimento.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.PortataMassimaEsercizio.HasValue)
						PortataMassimaEsercizio = infoStazione.CaratteristicheTecnichePozzo.PortataMassimaEsercizio.Value.ToString();
					if (infoStazione.CaratteristicheTecnichePozzo.PresenzaForoSonda)
						PresenzaForoSonda = "Sì";
					DescrizioneTipoDestinazioneuso = infoStazione.CaratteristicheTecnichePozzo.DescrizioneTipoDestinazioneuso;
					DescrizioneTipoFrequenzaUtilizzo = infoStazione.CaratteristicheTecnichePozzo.DescrizioneTipoFrequenzaUtilizzo;
					if (infoStazione.CaratteristicheTecnichePozzo.Captata)
						Captata = "Sì";
				}

				// Caratteristiche installazione
				string DescrizioneTipoFissaggioTrasmettitore = null;
				string CavoEsternoInGuaina = "No";
				string CavoSottotraccia = "No";
				string ProtezioneArea = "No";
				string DescrizioneTipoAccesso = null;
				string Osservazioni = null;
				string Sicurezza = null;
				string ProfonditaSensore = null;
				if (infoStazione.CaratteristicheInstallazione != null)
				{
					DescrizioneTipoFissaggioTrasmettitore = infoStazione.CaratteristicheInstallazione.DescrizioneTipoFissaggioTrasmettitore;
					if (infoStazione.CaratteristicheInstallazione.CavoEsternoInGuaina)
						CavoEsternoInGuaina = "Sì";
					if (infoStazione.CaratteristicheInstallazione.CavoSottotraccia)
						CavoSottotraccia = "Sì";
					if (infoStazione.CaratteristicheInstallazione.ProtezioneArea)
						ProtezioneArea = "Sì";
					DescrizioneTipoAccesso = infoStazione.CaratteristicheInstallazione.DescrizioneTipoAccesso;
					Osservazioni = infoStazione.CaratteristicheInstallazione.Osservazioni;
					Sicurezza = infoStazione.CaratteristicheInstallazione.Sicurezza;
					if (infoStazione.CaratteristicheInstallazione.ProfonditaSensore.HasValue)
						ProfonditaSensore = infoStazione.CaratteristicheInstallazione.ProfonditaSensore.ToString();
				}

				// Dati amministrativi
				string Gestore = null;
				string IndirizzoGestore = null;
				string TelefonoGestore = null;
				string RiferimentoGestore = null;
				string PartitaIVAGestore = null;
				if (infoStazione.DatiAmministrativi != null)
				{
					Gestore = infoStazione.DatiAmministrativi.Gestore;
					IndirizzoGestore = infoStazione.DatiAmministrativi.IndirizzoGestore;
					TelefonoGestore = infoStazione.DatiAmministrativi.TelefonoGestore;
					RiferimentoGestore = infoStazione.DatiAmministrativi.RiferimentoGestore;
					PartitaIVAGestore = infoStazione.DatiAmministrativi.PartitaIVAGestore;
				}

				// Ultimo intervento
				string DataIntervento = null;
				string DescrizioneTipoIntervento = null;
				string DescrizioneOperatori = null;
				string DescrizioneOperatoriSupporto = null;

				if (infoStazione.UltimoIntervento != null)
				{
					DataIntervento = infoStazione.UltimoIntervento.DataIntervento.ToString(CostantiGenerali.FORMATO_DATA);
					DescrizioneTipoIntervento = infoStazione.UltimoIntervento.DescrizioneTipoIntervento;
					DescrizioneOperatori = infoStazione.UltimoIntervento.DescrizioneOperatoriIntervento;
					DescrizioneOperatoriSupporto = infoStazione.UltimoIntervento.DescrizioneOperatoriSupportoIntervento;
				}

				ReportParameter[] parametri = new ReportParameter[] {
						new ReportParameter("IdStazione", infoStazione.IdStazione.ToString()),
						new ReportParameter("CodiceIdentificativo", infoStazione.CodiceIdentificativo),
						new ReportParameter("Descrizione", infoStazione.Descrizione),
						new ReportParameter("EsclusaMonitoraggio", esclusaMonitoraggio),
						new ReportParameter("DescrizioneTipoStazione", infoStazione.DescrizioneTipoStazione),
						new ReportParameter("Annotazioni", infoStazione.Annotazioni),
						new ReportParameter("ReteAppartenenza", infoStazione.ReteAppartenenza),
						new ReportParameter("Allestimento", infoStazione.Allestimento),
						new ReportParameter("CodiceIdentificativoDescrizioneComuneProvinciaSito", infoStazione.CodiceIdentificativoDescrizioneComuneProvinciaSito),
						new ReportParameter("Teletrasmissione", teletrasmissione),
						new ReportParameter("PuntoConformita", puntoConformita),
						new ReportParameter("Finalita", infoStazione.StringaFinalita),

						new ReportParameter("DenominazioneComune", DenominazioneComune),
						new ReportParameter("DenominazioneProvincia", DenominazioneProvincia),
						new ReportParameter("Localita", Localita),
						new ReportParameter("DescrizioneBacino", DescrizioneBacino),
						new ReportParameter("DescrizioneCorpoIdrico", DescrizioneCorpoIdrico),
						new ReportParameter("CTR", CTR),
						new ReportParameter("Latitudine", Latitudine),
						new ReportParameter("Longitudine", Longitudine),
						new ReportParameter("LatitudineGaussBoaga", LatitudineGaussBoaga),
						new ReportParameter("LongitudineGaussBoaga", LongitudineGaussBoaga),
						new ReportParameter("QuotaPianoCampagna", QuotaPianoCampagna),
						new ReportParameter("CodiceSIRAL", CodiceSIRAL),

						new ReportParameter("Profondita", Profondita),
						new ReportParameter("Diametro", Diametro),
						new ReportParameter("RangeSoggiacenzaDa", RangeSoggiacenzaDa),
						new ReportParameter("RangeSoggiacenzaA", RangeSoggiacenzaA),
						new ReportParameter("DescrizioneTipoChiusura", DescrizioneTipoChiusura),
						new ReportParameter("QuotaBoccapozzoPc", QuotaBoccapozzoPc),
						new ReportParameter("QuotaBoccapozzoSlm", QuotaBoccapozzoSlm),
						new ReportParameter("QuotaPianoRiferimentoSlm", QuotaPianoRiferimentoSlm),
						new ReportParameter("DifferenzaPrpc", DifferenzaPrpc),
						new ReportParameter("ProfonditaEmungimento", ProfonditaEmungimento),
						new ReportParameter("PortataMassimaEsercizio", PortataMassimaEsercizio),
						new ReportParameter("PresenzaForoSonda", PresenzaForoSonda),
						new ReportParameter("DescrizioneTipoDestinazioneuso", DescrizioneTipoDestinazioneuso),
						new ReportParameter("DescrizioneTipoFrequenzaUtilizzo", DescrizioneTipoFrequenzaUtilizzo),
						new ReportParameter("Captata", Captata),

						new ReportParameter("DescrizioneTipoFissaggioTrasmettitore", DescrizioneTipoFissaggioTrasmettitore),
						new ReportParameter("CavoEsternoInGuaina", CavoEsternoInGuaina),
						new ReportParameter("CavoSottotraccia", CavoSottotraccia),
						new ReportParameter("ProtezioneArea", ProtezioneArea),
						new ReportParameter("DescrizioneTipoAccesso", DescrizioneTipoAccesso),
						new ReportParameter("Osservazioni", Osservazioni),
						new ReportParameter("Sicurezza", Sicurezza),
						new ReportParameter("ProfonditaSensore", ProfonditaSensore),

						new ReportParameter("Gestore", Gestore),
						new ReportParameter("IndirizzoGestore", IndirizzoGestore),
						new ReportParameter("TelefonoGestore", TelefonoGestore),
						new ReportParameter("RiferimentoGestore", RiferimentoGestore),
						new ReportParameter("PartitaIVAGestore", PartitaIVAGestore),

						new ReportParameter("DataIntervento", DataIntervento),
						new ReportParameter("DescrizioneTipoIntervento", DescrizioneTipoIntervento),
						new ReportParameter("DescrizioneOperatori", DescrizioneOperatori),
						new ReportParameter("DescrizioneOperatoriSupporto", DescrizioneOperatoriSupporto),
						new ReportParameter("ImmagineStazione", ImmagineStazione),
						new ReportParameter("ImmagineMappa", ImmagineMappa)
					};

				rv.LocalReport.SetParameters(parametri);

				var strumentiStazione = StrumentoManager.ElencoStrumenti(infoStazione.IdStazione)
					.Where(i => i.FineValidita == null || i.FineValidita >= DateTime.Now.Date)
					.OrderBy(i => i.Marca)
					.ThenBy(i => i.Modello)
					.ToList();

				rv.LocalReport.DataSources.Add(new ReportDataSource("StrumentiDataSet", strumentiStazione));

				rv.LocalReport.SetParameters(new ReportParameter[] {
					new ReportParameter("DataStampa",
						"Stampato il " + DateTime.Now.ToString(CostantiGenerali.FORMATO_DATA_ORA_ALLE))
				});

			}
		}

		/// <summary>
		/// Parametri per la ricerca stazione
		/// </summary>
		public class RicercaStazione
		{
			public string CodiceIdentificativoDescrizione { get; set; }
			public string CodiceIdentificativoDescrizioneSito { get; set; }
			public string CodiceComune { get; set; }
			public string CodiceProvincia { get; set; }
			public decimal? IdBacino { get; set; }
			public decimal? IdCorpoIdrico { get; set; }
			public decimal? IdTipoStazione { get; set; }
			public bool? EsclusaMonitoraggio { get; set; }
			public string Allestimento { get; set; }
			public string ReteAppartenenza { get; set; }
			public string GrandezzaRilevata { get; set; }
			public decimal? IdSito { get; set; }
			public string Gestore { get; set; }
			public string PartitaIvaGestore { get; set; }
			public int NumeroPagina { get; set; }
			public int DimensionePagina { get; set; }
		}

		public static List<string> FinalitaStazione(decimal idStazione)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.FinalitaStazioni
					.Where(i => i.ID_STAZIONE == idStazione)
					.OrderBy(i => i.SIAS_TIPI_FINALITA_STAZ.ORDINE)
					.ThenBy(i => i.SIAS_TIPI_FINALITA_STAZ.DESCRIZIONE)
					.Select(i => i.FINALITA);
				return elenco.ToList();
			}
		}

		public static List<InfoStazionePerElenco> ElencoStazioni(RicercaStazione parametriRicerca, out int recordTrovati)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.V_ELENCO_STAZIONI.Select(i => i);
				if (!string.IsNullOrEmpty(parametriRicerca.CodiceIdentificativoDescrizione))
					elenco = elenco.Where(i => i.CODICE_IDENTIFICATIVO.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizione.ToLower()) ||
					i.DESCRIZIONE.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizione.ToLower()));
				if (!string.IsNullOrEmpty(parametriRicerca.CodiceIdentificativoDescrizioneSito))
					elenco = elenco.Where(i => i.CODICE_IDENTIFICATIVO_SITO.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizioneSito.ToLower()) ||
					i.DESCRIZIONE_SITO.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizioneSito.ToLower()));
				if (!string.IsNullOrEmpty(parametriRicerca.CodiceComune))
					elenco = elenco.Where(i => i.CODICE_COMUNE == parametriRicerca.CodiceComune);
				if (!string.IsNullOrEmpty(parametriRicerca.CodiceProvincia))
					elenco = elenco.Where(i => i.CODICE_PROVINCIA == parametriRicerca.CodiceProvincia);
				if (parametriRicerca.IdBacino.HasValue)
					elenco = elenco.Where(i => i.ID_BACINO == parametriRicerca.IdBacino);
				if (parametriRicerca.IdCorpoIdrico.HasValue)
					elenco = elenco.Where(i => i.ID_CORPO_IDRICO == parametriRicerca.IdCorpoIdrico);
				if (parametriRicerca.IdTipoStazione.HasValue)
					elenco = elenco.Where(i => i.ID_TIPO_STAZIONE == parametriRicerca.IdTipoStazione);
				if (!string.IsNullOrEmpty(parametriRicerca.Gestore))
					elenco = elenco.Where(i => i.GESTORE.ToLower() == parametriRicerca.Gestore.ToLower());
				if (parametriRicerca.EsclusaMonitoraggio.HasValue && !parametriRicerca.EsclusaMonitoraggio.Value)
					elenco = elenco.Where(i => i.ESCLUSA_MONITORAGGIO == false);
				if (!string.IsNullOrEmpty(parametriRicerca.Allestimento))
					elenco = elenco.Where(i => i.ALLESTIMENTO == parametriRicerca.Allestimento);

				// Se è specificato il filtro per rete, filtra l'elenco prendendo solo le stazioni con id contenuti in stazioni reti per quella rete
				if (!string.IsNullOrEmpty(parametriRicerca.ReteAppartenenza))
				{
					var idStazioniRete = context.StazioniReti.Where(i => i.RETE == parametriRicerca.ReteAppartenenza).Select(i => i.ID_STAZIONE).ToList();
					elenco = elenco.Where(i => idStazioniRete.Contains(i.ID_STAZIONE));
				}


				if (!string.IsNullOrEmpty(parametriRicerca.GrandezzaRilevata))
					elenco = elenco.Where(i => context.GrandezzeStazione.Any(g => g.ID_STAZIONE == i.ID_STAZIONE && g.GRANDEZZA == parametriRicerca.GrandezzaRilevata));
				if (parametriRicerca.IdSito.HasValue)
					elenco = elenco.Where(i => i.ID_SITO == parametriRicerca.IdSito);

				recordTrovati = elenco.Count();

				return elenco
					.OrderBy(i => i.CODICE_IDENTIFICATIVO)
					.Skip(parametriRicerca.DimensionePagina * (parametriRicerca.NumeroPagina - 1))
					.Take(parametriRicerca.DimensionePagina)
					.Select(i => new InfoStazionePerElenco()
					{
						IdStazione = i.ID_STAZIONE,
						CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
						Descrizione = i.DESCRIZIONE,
						Tipo = i.DESCRIZIONE_TIPO_STAZIONE,
						IdTipo = i.ID_TIPO_STAZIONE,
						Comune = i.DENOMINAZIONE_COMUNE,
						Provincia = i.SIGLA_PROVINCIA,
						Bacino = i.DESCRIZIONE_BACINO,
						CorpoIdrico = i.DESCRIZIONE_CORPO_IDRICO,
						EsclusaMonitoraggio = i.ESCLUSA_MONITORAGGIO,
						ReteAppartenenza = i.RETE_APPARTENENZA,
						Longitudine = i.LONGITUDINE.Value,
						Latitudine = i.LATITUDINE.Value,
						IdSito = i.ID_SITO,
						CodiceIdentificativoSito = i.CODICE_IDENTIFICATIVO_SITO,
						DescrizioneSito = i.DESCRIZIONE_SITO,
						LatitudineSito = i.LATITUDINE_SITO,
						LongitudineSito = i.LONGITUDINE_SITO
					})
					.Distinct() // La distinct serve a evitare duplicati nel caso di stazioni con più gestori
					.OrderBy(i => i.CodiceIdentificativo)
					.ToList();
			}
		}

		public static List<InfoStazionePerElencoGestore> ElencoStazioniGestore(RicercaStazione parametriRicerca, out int recordTrovati)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.V_ELENCO_STAZIONI_GESTORE.Select(i => i);
				if (!string.IsNullOrEmpty(parametriRicerca.CodiceIdentificativoDescrizione))
					elenco = elenco.Where(i => i.CODICE_IDENTIFICATIVO.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizione.ToLower()) ||
					i.DESCRIZIONE.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizione.ToLower()));
				if (!string.IsNullOrEmpty(parametriRicerca.CodiceIdentificativoDescrizioneSito))
					elenco = elenco.Where(i => i.CODICE_IDENTIFICATIVO_SITO.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizioneSito.ToLower()) ||
					i.DESCRIZIONE_SITO.ToLower().Contains(parametriRicerca.CodiceIdentificativoDescrizioneSito.ToLower()));
				if (!string.IsNullOrEmpty(parametriRicerca.PartitaIvaGestore))
					elenco = elenco.Where(i => i.PIVA_GESTORE == parametriRicerca.PartitaIvaGestore);

				// Se è specificato il filtro per rete, filtra l'elenco prendendo solo le stazioni con id contenuti in stazioni reti per quella rete
				if (!string.IsNullOrEmpty(parametriRicerca.ReteAppartenenza))
				{
					var idStazioniRete = context.StazioniReti.Where(i => i.RETE == parametriRicerca.ReteAppartenenza).Select(i => i.ID_STAZIONE).ToList();
					elenco = elenco.Where(i => idStazioniRete.Contains(i.ID_STAZIONE));
				}


				if (!string.IsNullOrEmpty(parametriRicerca.GrandezzaRilevata))
					elenco = elenco.Where(i => context.GrandezzeStazione.Any(g => g.ID_STAZIONE == i.ID_STAZIONE && g.GRANDEZZA == parametriRicerca.GrandezzaRilevata));
				if (parametriRicerca.IdSito.HasValue)
					elenco = elenco.Where(i => i.ID_SITO == parametriRicerca.IdSito);

				recordTrovati = elenco.Count();

				return elenco
					.OrderBy(i => i.CODICE_IDENTIFICATIVO)
					.Skip(parametriRicerca.DimensionePagina * (parametriRicerca.NumeroPagina - 1))
					.Take(parametriRicerca.DimensionePagina)
					.Select(i => new InfoStazionePerElencoGestore()
					{
						IdStazione = i.ID_STAZIONE,
						CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
						Descrizione = i.DESCRIZIONE,
						Tipo = i.DESCRIZIONE_TIPO_STAZIONE,
						IdTipo = i.ID_TIPO_STAZIONE,
						Comune = i.DENOMINAZIONE_COMUNE,
						Provincia = i.SIGLA_PROVINCIA,
						Bacino = i.DESCRIZIONE_BACINO,
						CorpoIdrico = i.DESCRIZIONE_CORPO_IDRICO,
						EsclusaMonitoraggio = i.ESCLUSA_MONITORAGGIO,
						ReteAppartenenza = i.RETE_APPARTENENZA,
						Longitudine = i.LONGITUDINE.Value,
						Latitudine = i.LATITUDINE.Value,
						IdSito = i.ID_SITO,
						CodiceIdentificativoSito = i.CODICE_IDENTIFICATIVO_SITO,
						DescrizioneSito = i.DESCRIZIONE_SITO,
						LatitudineSito = i.LATITUDINE_SITO,
						LongitudineSito = i.LONGITUDINE_SITO,
						Gestore = i.GESTORE,
						PartitaIvaGestore = i.PIVA_GESTORE
					})
					.OrderBy(i => i.CodiceIdentificativo)
					.ToList();
			}
		}

		public static string GeneraScriptMappa(List<InfoStazionePerElenco> elencoStazioni, bool seGestore)
		{
			string cartellaImmaginiTipiStazione = $"{Utils.ApplicationUrlRoot()}/img/TipiStazione/";

			StringBuilder sb = new StringBuilder();

			sb.AppendLine("<script type=\"text/javascript\">");

			// Inizializzaione mappa
			sb.AppendLine("function initMap() {");
			sb.AppendLine("var myLatLng = { lat: 44.25, lng: 8.76 }; ");
			sb.AppendLine("var map = new google.maps.Map(document.getElementById('map'), {");
			sb.AppendLine("center: myLatLng,");
			sb.AppendLine("scrollwheel: true,");
			sb.AppendLine("zoom: 9, ");
			sb.AppendLine("mapTypeId: google.maps.MapTypeId.ROADMAP ");
			sb.AppendLine("}); ");
			sb.AppendLine("var rndLatLng; ");

			// Aggiunge marker singola stazione
			foreach (var s in elencoStazioni)
			{
				if (s.Latitudine != null)
				{
					// Genera tooltip con codice e descrizione stazione
					string tooltipStazione = s.CodiceIdentificativo + " - " + s.Descrizione;
					// Escape di eventuali apici
					tooltipStazione = tooltipStazione.Replace("'", @"\'");

					//  costruisce il maker della stazione
					sb.AppendLine("stazioneLatLng = { lat: " + s.Latitudine.ToString().Replace(",", ".") + ", lng: " + s.Longitudine.ToString().Replace(",", ".") + " }; ");
					// Immagine
					sb.AppendLine("var image = { url: '" + cartellaImmaginiTipiStazione + s.IdTipo + ".png' }; ");
					// Marker
					sb.AppendLine("var marker" + s.IdStazione.ToString() + " = new google.maps.Marker({ " +
						"position: stazioneLatLng, " +
						"map: map, " +
						"title: '" + tooltipStazione + "', " +
						"url: '/Stazione/VisualizzaStazione.aspx?IdStazione=" + s.IdStazione.ToString() + "', " +
						"icon: image" +
					"}); ");

					// Aggiunge listener per evento di clic solo per utente gestore
					if (seGestore)
						sb.AppendLine("google.maps.event.addListener(marker" + s.IdStazione.ToString() + ", 'click', function () { window.location.href = marker" + s.IdStazione.ToString() + ".url; }); ");
				}
			}

			// Siti
			var elencoSiti = elencoStazioni
				.Where(i => i.IdSito.HasValue && i.LatitudineSito.HasValue && i.LongitudineSito.HasValue)
				.Select(s => new { s.IdSito, s.LatitudineSito, s.LongitudineSito, s.CodiceIdentificativoSito, s.DescrizioneSito })
				.Distinct();
			// Aggiunge marker singolo sito
			string cartellaImmagini = $"{Utils.ApplicationUrlRoot()}/img/";
			foreach (var s in elencoSiti)
			{
				// Genera tooltip con codice e descrizione sito
				string tooltipSito = s.CodiceIdentificativoSito + " - " + s.DescrizioneSito;
				// Escape di eventuali apici
				tooltipSito = tooltipSito.Replace("'", @"\'");

				//  costruisce il maker del sito
				sb.AppendLine("sitoLatLng = { lat: " + s.LatitudineSito.ToString().Replace(",", ".") + ", lng: " + s.LongitudineSito.ToString().Replace(",", ".") + " }; ");
				// Immagine
				sb.AppendLine("var image = { url: '" + cartellaImmagini + "Sito.png' }; ");
				// Marker
				sb.AppendLine("var marker" + s.IdSito.ToString() + " = new google.maps.Marker({ " +
					"position: sitoLatLng, " +
					"map: map, " +
					"title: '" + tooltipSito + "', " +
					"url: '/Sito/VisualizzaSito.aspx?IdSito=" + s.IdSito.ToString() + "', " +
					"icon: image" +
				"}); ");

				// Aggiunge listener per evento di clic solo per utente gestore
				if (seGestore)
					sb.AppendLine("google.maps.event.addListener(marker" + s.IdSito.ToString() + ", 'click', function () { window.location.href = marker" + s.IdSito.ToString() + ".url; }); ");
			}

			sb.AppendLine("}</script>");

			if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleAPIKey"]))
				sb.AppendLine(@"<script async defer src='https://maps.googleapis.com/maps/api/js?callback=initMap'></script>");
			else
				sb.AppendLine(@"<script async defer src='https://maps.googleapis.com/maps/api/js?key=" + ConfigurationManager.AppSettings["GoogleAPIKey"] + "&callback=initMap'></script>");

			return sb.ToString();
		}

		#region Ultima misurazione e intervento

		/// <summary>
		/// Parametri per la ricerca ultima misurazione e intervento 
		/// </summary>
		public class RicercaUltimaMisurazioneEIntervento
		{
			public string CodiceProvincia { get; set; }
			public decimal? IdTipoStazione { get; set; }
			public bool? EsclusaMonitoraggio { get; set; }
			/// <summary>
			/// Proprietà per cui ordinare i risultati (uno tra i valori forniti da CampiOrdinamentoRicercaUltimaMisurazioneEIntervento)
			/// </summary>
			public string Ordinamento { get; set; }
			public bool SeUltimaMisurazione { get; set; }
			public string ReteAppartenenza { get; set; }
		}

		/// <summary>
		/// Verifica se per una stazione esiste una misurazione e/o un intervento
		/// </summary>
		/// <param name="idStazione"></param>
		/// <returns></returns>
		public static bool EsisteUltimaMisurazioneOIntervento(decimal idStazione)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				return context.Database.SqlQuery<bool>(
					"select count(1) from v_ultima_misuraz_e_intervento where id_stazione = :id_stazione and rownum = 1 and (data_ultimo_intervento is not null or data_ultima_misurazione is not null)",
					new OracleParameter("@id_stazione", idStazione)).FirstOrDefault();
			}
		}

		public static List<string> CampiOrdinamentoRicercaUltimaMisurazioneEIntervento()
		{
			return new List<string>()
			{
				"Descrizione stazione (A-Z)",
				"Descrizione stazione (Z-A)",
				"Codice identificativo (A-Z)",
				"Codice identificativo (Z-A)",
				"Tipo (A-Z)",
				"Tipo (Z-A)",
				"Comune (A-Z)",
				"Comune (Z-A)",
				"Provincia (A-Z)",
				"Provincia (Z-A)",
				"Data ultimo intervento (meno recente prima)",
				"Data ultimo intervento (più recente prima)",
				"Data ultima misurazione (meno recente prima)",
				"Data ultima misurazione (più recente prima)"
			};
		}

		public static List<InfoUltimaMisurazioneEIntervento> UltimaMisurazioneEIntervento(RicercaUltimaMisurazioneEIntervento parametriRicerca)
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				IEnumerable<InfoUltimaMisurazioneEIntervento> risultato;
				if (parametriRicerca.SeUltimaMisurazione)
					risultato = context.V_ULTIMA_MISURAZ_E_INTERVENTO
						.Select(i => new InfoUltimaMisurazioneEIntervento()
						{
							IdStazione = i.ID_STAZIONE,
							CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
							Descrizione = i.DESCRIZIONE,
							Tipo = i.DESCRIZIONE_TIPO_STAZIONE,
							IdTipo = i.ID_TIPO_STAZIONE,
							Comune = i.DENOMINAZIONE_COMUNE,
							CodiceProvincia = i.CODICE_PROVINCIA,
							Provincia = i.DENOMINAZIONE_PROVINCIA,
							EsclusaMonitoraggio = i.ESCLUSA_MONITORAGGIO,
							DataUltimoIntervento = i.DATA_ULTIMO_INTERVENTO,
							DurataUltimoIntervento = i.DURATA_ULTIMO_INTERVENTO,
							DataUltimaMisurazione = i.DATA_ULTIMA_MISURAZIONE,
							UltimaMisurazioneValidata = i.VALIDATA,
							CodiceSiral = i.CODICE_SIRAL,
							ReteAppartenenza = i.RETE_APPARTENENZA,
							Grandezza = i.GRANDEZZA
						}).AsEnumerable();
				else
					risultato = context.V_ULTIMO_INTERVENTO
						.Select(i => new InfoUltimaMisurazioneEIntervento()
						{
							IdStazione = i.ID_STAZIONE,
							CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
							Descrizione = i.DESCRIZIONE,
							Tipo = i.DESCRIZIONE_TIPO_STAZIONE,
							IdTipo = i.ID_TIPO_STAZIONE,
							Comune = i.DENOMINAZIONE_COMUNE,
							CodiceProvincia = i.CODICE_PROVINCIA,
							Provincia = i.DENOMINAZIONE_PROVINCIA,
							EsclusaMonitoraggio = i.ESCLUSA_MONITORAGGIO,
							DataUltimoIntervento = i.DATA_ULTIMO_INTERVENTO,
							DurataUltimoIntervento = i.DURATA_ULTIMO_INTERVENTO,
							CodiceSiral = i.CODICE_SIRAL,
							ReteAppartenenza = i.RETE_APPARTENENZA
						}).AsEnumerable();

				if (!string.IsNullOrEmpty(parametriRicerca.CodiceProvincia))
					risultato = risultato.Where(i => i.CodiceProvincia == parametriRicerca.CodiceProvincia);
				if (parametriRicerca.IdTipoStazione.HasValue)
					risultato = risultato.Where(i => i.IdTipo == parametriRicerca.IdTipoStazione);
				if (parametriRicerca.EsclusaMonitoraggio.HasValue && !parametriRicerca.EsclusaMonitoraggio.Value)
					risultato = risultato.Where(i => i.EsclusaMonitoraggio == false);

				// Modificato per cercare sottostringhe (Contains) e trovare stazioni appartenenti a più reti con il filtro per una sola
				if (!string.IsNullOrEmpty(parametriRicerca.ReteAppartenenza))
					risultato = risultato.Where(i => i.ReteAppartenenza != null && i.ReteAppartenenza.ToLower().Contains(parametriRicerca.ReteAppartenenza.ToLower()));

				switch (parametriRicerca.Ordinamento)
				{
					case "Descrizione stazione (A - Z)":
						return risultato.OrderBy(i => i.Descrizione).ToList();
					case "Descrizione stazione (Z-A)":
						return risultato.OrderByDescending(i => i.Descrizione).ToList();
					case "Codice identificativo (A-Z)":
						return risultato.OrderBy(i => i.CodiceIdentificativo).ToList();
					case "Codice identificativo (Z-A)":
						return risultato.OrderByDescending(i => i.CodiceIdentificativo).ToList();
					case "Tipo (A-Z)":
						return risultato.OrderBy(i => i.Tipo).ToList();
					case "Tipo (Z-A)":
						return risultato.OrderByDescending(i => i.Tipo).ToList();
					case "Provincia (A-Z)":
						return risultato.OrderBy(i => i.Provincia).ToList();
					case "Provincia (Z-A)":
						return risultato.OrderByDescending(i => i.Provincia).ToList();
					case "Comune (A-Z)":
						return risultato.OrderBy(i => i.Comune).ToList();
					case "Comune (Z-A)":
						return risultato.OrderByDescending(i => i.Comune).ToList();
					case "Data ultimo intervento (meno recente prima)":
						return risultato.OrderBy(i => i.DataUltimoIntervento).ToList();
					case "Data ultimo intervento (più recente prima)":
						return risultato.OrderByDescending(i => i.DataUltimoIntervento).ToList();
					case "Data ultima misurazione (meno recente prima)":
						return risultato.OrderBy(i => i.DataUltimaMisurazione).ToList();
					case "Data ultima misurazione (più recente prima)":
						return risultato.OrderByDescending(i => i.DataUltimaMisurazione).ToList();
					default:
						return risultato.OrderBy(i => i.Descrizione).ToList();
				}

			}
		}
		#endregion Ultima misurazione e intervento

		public static List<TipoStazione> TipiStazione()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiStazione.OrderBy(i => i.DescrizioneTipoStazione);
				return elenco.ToList();
			}
		}

		public static List<string> CategorieStazione()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiStazione.Select(i => i.Categoria).Distinct().OrderBy(i => i);
				return elenco.ToList();
			}
		}

		public static List<TipoReteAppartenenza> TipiReteAppartenenza()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiReteAppartenenza.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
				return elenco.ToList();
			}
		}

		public static List<TipoAllestimento> TipiAllestimento()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiAllestimento.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
				return elenco.ToList();
			}
		}

		public static List<TipoFinalitaStazione> TipiFinalitaStazioni()
		{
			using (SIASSEntities context = new SIASSEntities())
			{
				var elenco = context.TipiFinalitaStazione.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE);
				return elenco.ToList();
			}
		}
	}
}
