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
using SIASS.GSOServiceReference;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SIASS.BLL
{
    public static class GSOManager
    {
        private static readonly GsoWebServicesSoapClient ws = new GsoWebServicesSoapClient();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static List<UtenteApplicazione> UtentiPerApplicazione(string applicazione)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Richiesta GSO GetApplicationUsers:{applicazione}");
            DataSet ds = ws.GetApplicationUsers(applicazione);
            List<UtenteApplicazione> utentiApplicazione = new List<UtenteApplicazione>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UtenteApplicazione ua = new UtenteApplicazione
                {
                    IdUtente = (decimal)dr["ID_UTENTE"],
                    Nome = dr["NOME"].ToString(),
                    Cognome = dr["COGNOME"].ToString(),
                    DescrizioneProfilo = dr["DESCRIZIONE_PROFILO"].ToString()
                };

                utentiApplicazione.Add(ua);
            }
            return utentiApplicazione;
        }

        public class UtenteApplicazione
        {
            public decimal IdUtente { get; set; }
            public string Nome { get; set; }
            public string Cognome { get; set; }
            public string DescrizioneProfilo { get; set; }
            public string CognomeNome
            {
                get
                {
                    return $"{Cognome} {Nome}";
                }
            }
        }

    }
}