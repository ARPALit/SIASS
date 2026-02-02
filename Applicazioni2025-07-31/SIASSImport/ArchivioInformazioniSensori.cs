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

using System.Collections.Generic;
using System.Linq;

namespace SIASSImport
{
    public class ArchivioInformazioniSensori
    {
        private readonly List<InfoSensore> elencoSensori;

        public ArchivioInformazioniSensori()
        {
            elencoSensori = new List<InfoSensore>();
        }

        /// <summary>
        /// Carica le informazioni sul sensore. Se un sensore è già stato richiesto le informazioni
        /// sono memorizzate nella lista interna
        /// </summary>
        /// <param name="codiceIdentificativoSensore"></param>
        /// <returns></returns>
        public InfoSensore LetturaInformazioniSensore(string codiceIdentificativoSensore)
        {
            var infoSensore = elencoSensori.FirstOrDefault(i => i.CodiceIdentificativoSensore == codiceIdentificativoSensore);
            if (infoSensore != null)
                return infoSensore;
            else
            {
                infoSensore = DAL.LetturaDatiSensore(codiceIdentificativoSensore);
                if (infoSensore != null)
                    elencoSensori.Add(infoSensore);
                return infoSensore;
            }
        }
    }

    public class InfoSensore
    {
        public InfoSensore(string codiceIdentificativoSensore, decimal idGrandezzaStazione, decimal? coefficienteConversioneUnitaMisura)
        {
            CodiceIdentificativoSensore = codiceIdentificativoSensore;
            IdGrandezzaStazione = idGrandezzaStazione;
            CoefficienteConversioneUnitaMisura = coefficienteConversioneUnitaMisura; ;
        }

        public string CodiceIdentificativoSensore { get; set; }
        public decimal? CoefficienteConversioneUnitaMisura { get; set; }
        public decimal IdGrandezzaStazione { get; set; }
    }
}