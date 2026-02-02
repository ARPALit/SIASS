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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;

namespace SIASS
{
    public class EsportazioneVerbali : IHttpHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void ProcessRequest(HttpContext context)
        {
            var risposta = new Risposta();

            // Imposta il tipo di contenuto della risposta a JSON
            context.Response.ContentType = "application/json";

            // Lettura del valore ApiKey dalla querystring
            string apiKey = context.Request.QueryString["ApiKey"];

            // Verifica della validità della chiave
            if (apiKey != ConfigurationManager.AppSettings["ApiKeyEsportazioneVerbaliInterventi"])
            {
                risposta.Success = false;
                risposta.ErrorMessage = "ApiKey non valida";
                logger.Warn($"EsportazioneVerbali - ApiKey non valida: {apiKey}");
            }
            else
            {
                try
                {
                    // Recupera l'elenco dei verbali da esportare
                    risposta.Data = EsportazioneVerbaleManager.ElencoVerbali();
                }
                catch (Exception ex)
                {
                    risposta.Success = false;
                    risposta.ErrorMessage = ex.Message;
                    risposta.ErrorDetails = ex.ToString();
                }
            }

            // Serializza l'oggetto in JSON
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            // Imposta un valore alto per la lunghezza del json
            jsSerializer.MaxJsonLength = Int32.MaxValue;
            string jsonResponse = jsSerializer.Serialize(risposta);
            context.Response.Write(jsonResponse);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private sealed class Risposta
        {
            public bool Success { get; set; } = true;
            public List<VerbalePerEsportazione> Data { get; set; } = null;
            public string ErrorMessage { get; set; } = null;
            public string ErrorDetails { get; set; } = null;
        }
    }
}