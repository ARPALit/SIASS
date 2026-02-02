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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIASSImport
{
    public class MisurazioneDaImportare
    {
        public string CodiceIdentificativoSensore { get; set; }
        public DateTime DataOraMisurazione { get; set; }
        /// <summary>
        /// Valore della misurazione nell'unità di misura della grandezza associata al sensore.
        /// Se previsto è ottenuta applicando il coefficiente di conversione al valore letto dal sensore.
        /// </summary>
        public decimal ValoreMisurazioneUnitaMisuraGrandezza { get; set; }
        public decimal ValoreMisurazioneUnitaMisuraSensore { get; set; }
        public decimal? IdGrandezzaStazione { get; set; }
    }
}
