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

using System;

namespace SIASSImportOrganoclorurati
{
    /// <summary>
    /// Informazioni sulla misurazione lette dal file d'importazione
    /// </summary>
    internal class MisurazioneOrganoclorurati
    {
        public string Sito { get; set; } // Obbligatorio
        public string Piezometro { get; set; } // Obbligatorio
        public DateTime DataCampionamento { get; set; } // Obbligatorio
        public string Fonte { get; set; }
        public string Parametro { get; set; } // Obbligatorio
        public string ConcSign { get; set; }
        public decimal ConcVal { get; set; } // Obbligatorio
        public decimal? Loq { get; set; }
        public decimal? Incertezza { get; set; }
        public string ConcUdm { get; set; }
        /// <summary>
        /// Numero di riga del file d'importazione corrispondente alla misurazione
        /// </summary>
        public int NumeroRigaFile { get; set; }
    }
}
