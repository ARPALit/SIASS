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

using Edeicos;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SIASS
{
    internal static class CostruzioneElenchiDataset
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public class Result
        {
            public string URLDatasetReteMonitoraggio { get; set; }
            public List<LinkAURL> ElencoStoricoMisurazioni { get; set; }
            public List<LinkAURL> ElencoMisurazioniAnnoCorrente { get; set; }
        }

        internal static ResultDto<Result> Run()
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name}");

            try
            {
                string urlDatasetReteMonitoraggio = ConfigurationManager.AppSettings["URLDatasetReteMonitoraggio"];
                string urlStoricoMisurazioni = ConfigurationManager.AppSettings["URLDatasetStoricoMisurazioni"];
                string urlMisurazioniAnnoCorrente = urlStoricoMisurazioni.Replace("&amp;", "&");

                var elencoStazioni = ElencoStazioniDaJson(LetturaDatasetReteMonitoraggio(urlDatasetReteMonitoraggio));

                var elencoStoricoMisurazioni = new List<LinkAURL>();
                var elencoMisurazioniAnnoCorrente = new List<LinkAURL>();
                string annoCorrente = DateTime.Today.Year.ToString();
                foreach (var stazione in elencoStazioni)
                {
                    elencoStoricoMisurazioni.Add(new LinkAURL(stazione.DescrizioneEstesa, urlStoricoMisurazioni
                        .Replace("__CODICE_STAZIONE__", stazione.CodiceIdentificativo)
                            .Replace("__ANNO__", "")
                            .Replace("__MESE__", "")
                        ));
                    elencoMisurazioniAnnoCorrente.Add(new LinkAURL(stazione.DescrizioneEstesa, urlMisurazioniAnnoCorrente
                        .Replace("__CODICE_STAZIONE__", stazione.CodiceIdentificativo)
                        .Replace("__ANNO__", annoCorrente)
                        .Replace("__MESE__", "")))
                        ;
                }

                var result = new Result
                {
                    URLDatasetReteMonitoraggio = urlDatasetReteMonitoraggio,
                    ElencoStoricoMisurazioni = elencoStoricoMisurazioni,
                    ElencoMisurazioniAnnoCorrente = elencoMisurazioniAnnoCorrente
                };

                return new ResultDto<Result>(result);
            }
            catch (Exception ex)
            {
                string errorDetails = Utils.GetExceptionDetails(ex);
                logger.Error(errorDetails);
                return new ResultDto<Result>(new Error($"Errore. Contattare l'amministratore di sistema. ({logger.Name}.{MethodBase.GetCurrentMethod().Name})", errorDetails));
            }
        }

        public class LinkAURL
        {
            public LinkAURL(string descrizione, string url)
            {
                Descrizione = descrizione;
                URLFile = url;
            }
            public string Descrizione { get; set; }
            public string URLFile { get; set; }
        }

        /// <summary>
        /// Elenco dei punti di monitoraggio costruito sulla base della stringa json ricevuta da AWS
        /// Ordinato per codice punto e comprensivo della descrizione estesa con codice, descrizione, comune e provincia
        /// </summary>
        /// <param name="stringaJson"></param>
        /// <returns></returns>

        private static List<Stazione> ElencoStazioniDaJson(string stringaJson)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

			var stazioni = JsonSerializer.Deserialize<List<RispostaStrutturaReteMonitoraggio.StazioneDaJson>>(stringaJson, options);

			var risposta = new RispostaStrutturaReteMonitoraggio
			{
				ElencoStazioniDaJson = stazioni
			};

			// Per ogni punto è scelto il record con l'anno più recente
			var elenco = risposta.ElencoStazioniDaJson
                .GroupBy(i => i.CodiceIdentificativo)
                .Select(group => group.OrderByDescending(p => p.CodiceIdentificativo).FirstOrDefault())
                .Select(i => new Stazione()
                {
                    CodiceIdentificativo = i.CodiceIdentificativo,
                    Descrizione = i.Descrizione,
                    SiglaProvincia = i.SiglaProvincia,
                    Comune = i.Comune
                })
                .OrderBy(i => i.CodiceIdentificativo)
                .ToList();

            logger.Info($"{MethodBase.GetCurrentMethod().Name} - Trovati {elenco.Count} punti di monitoraggio");

            return elenco;
        }
        /// <summary>
        /// Chiamata endpoint AWS per ottenere il dataset json della rete di monitoraggio
        /// </summary>
        /// <param name="urlDatasetReteMonitoraggio"></param>
        /// <returns></returns>

        private static string LetturaDatasetReteMonitoraggio(string urlDatasetReteMonitoraggio)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name}");

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(urlDatasetReteMonitoraggio)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            logger.Debug($"UrlDatasetReteMonitoraggio:{urlDatasetReteMonitoraggio}");
            logger.Debug("Chiamata API");
            var testoRisposta = client.GetStringAsync(string.Empty).GetAwaiter().GetResult();

            logger.Debug($"Lunghezza testo risposta:{testoRisposta.Length}");
            logger.Trace($"Testo risposta:{Environment.NewLine}{testoRisposta}");
            return testoRisposta;
        }

        private class RispostaStrutturaReteMonitoraggio
        {
            public List<StazioneDaJson> ElencoStazioniDaJson { get; set; }
            internal class StazioneDaJson
            {
                [JsonPropertyName("codice_identificativo")]
                public string CodiceIdentificativo { get; set; }
                [JsonPropertyName("sigla_provincia")]
                public string SiglaProvincia { get; set; }
                [JsonPropertyName("denominazione_comune")]
                public string Comune { get; set; }
                [JsonPropertyName("descrizione")]
                public string Descrizione { get; set; }
            }
        }

        private class Stazione
        {
            public string CodiceIdentificativo { get; set; }
            public string Provincia { get; set; }
            public string SiglaProvincia { get; set; }
            public string Comune { get; set; }
            public string Descrizione { get; set; }
            /// <summary>
            /// Tutte le informazioni del punto
            /// </summary>
            public string DescrizioneEstesa
            {
                get
                {
                    return $"{CodiceIdentificativo} - {Descrizione} - {Comune} ({SiglaProvincia})";
                }
            }
            public string UltimoAnno { get; set; }
        }
    }
}
