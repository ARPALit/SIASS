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

namespace SIASS
{
    public class CostantiGenerali
    {
        // Formato data da utilizzare nelle formattazioni in stringa via codice
        // nel databind dichiarativo il formato è specificato direttamente e non
        // viene quindi influenzato da questa impostazione
        public const string FORMATO_DATA = "dd/MM/yyyy";
        public const string FORMATO_DATA_ORA = "dd/MM/yyyy HH.mm";
        public const string FORMATO_DATA_ORA_ALLE = "dd/MM/yyyy alle HH.mm";
    }
}