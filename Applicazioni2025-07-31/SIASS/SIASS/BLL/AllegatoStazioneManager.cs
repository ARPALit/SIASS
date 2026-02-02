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

using SIASS.Model;
using System.Collections.Generic;
using System.Linq;

namespace SIASS.BLL
{
    public static class AllegatoStazioneManager
    {
        public static InfoAllegatoStazione CaricaInfoAllegatoStazione(decimal idAllegatoStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                AllegatoStazione a = context.AllegatiStazione.FirstOrDefault(i => i.IdAllegatoStazione == idAllegatoStazione);
                if (a != null)
                    return new InfoAllegatoStazione(a);
                else
                    return null;
            }
        }

        public static List<InfoAllegatoStazione> ElencoAllegatiStazione(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from a in context.AllegatiStazione
                              where a.IdStazione == idStazione
                              orderby a.DataOraInserimento descending
                              select new InfoAllegatoStazione
                              {
                                  IdAllegatoStazione = a.IdAllegatoStazione,
                                  IdStazione = a.IdStazione,
                                  NomeFileAllegato = a.NomeFileAllegato,
                                  DescrizioneAllegato = a.DescrizioneAllegato,
                                  DataOraInserimento = a.DataOraInserimento
                              }).ToList();

                return elenco;
            }
        }
    }
}
