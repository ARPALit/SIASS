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
using SIASS.ANSOLOServiceReference;
using System;
using System.Data;
using System.Reflection;

namespace SIASS.BLL
{
    public static class ANSOLOManager
    {
        private static readonly AnsoloWebServiceSoapClient ws = new AnsoloWebServiceSoapClient();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static bool MatricolaAnsolo(int idUtenteGSO, out string matricola)
        {
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Richiesta ANSOLO IdAnsoloPerIdUtenteGSO per idUtenteGSO:{idUtenteGSO}");
            matricola = null;
            int idAnsolo = ws.IdAnsoloPerIdUtenteGSO((int)idUtenteGSO);
            // Soggetto non trovato in ansolo
            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - IdAnsolo:{idAnsolo}");
            if (idAnsolo == 0)
            {
                logger.Error($"Utente GSO id:{idUtenteGSO} non trovato in Ansolo");
                return false;
            }

            logger.Debug($"{MethodBase.GetCurrentMethod().Name} - Richiesta ANSOLO DettagliSoggetto per idAnsolo:{idAnsolo}");
            DataSet ds = ws.DettagliSoggetto(idAnsolo);
            if (ds.Tables.Count == 0)
            {
                logger.Error($"Soggetto idAnsolo {idAnsolo} non trovato in Ansolo");
                return false;
            }

            // selezione prima riga
            if (ds.Tables[0].Rows.Count == 0)
            {
                logger.Error($"Soggetto idAnsolo {idAnsolo} non trovato in Ansolo");
                return false;
            }

            DataRow riga = ds.Tables[0].Rows[0];
            if (riga["MATRICOLA"] == DBNull.Value)
            {
                logger.Error($"Matricola per idAnsolo {idAnsolo} non trovata in Ansolo");
                return false;
            }

            matricola = Convert.ToString(riga["MATRICOLA"]);
            logger.Debug($"Matricola trovato su Ansolo: {matricola}");
            return true;
        }
    }
}