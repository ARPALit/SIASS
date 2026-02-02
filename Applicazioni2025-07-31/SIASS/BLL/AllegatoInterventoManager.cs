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
using static SIASS.Model.InfoIntervento;

namespace SIASS.BLL
{
    class AllegatoInterventoManager
    {
        public static InfoAllegatoIntervento CaricaInfoAllegatoIntervento(decimal idAllegatoIntervento)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                InfoAllegatoIntervento infoAllegatoIntervento = (from allegatoIntervento in context.AllegatiIntervento
                                                                 where allegatoIntervento.IdAllegatoIntervento == idAllegatoIntervento
                                                                 select new InfoAllegatoIntervento
                                                                 {
                                                                     IdAllegatoIntervento = allegatoIntervento.IdAllegatoIntervento,
                                                                     IdIntervento = allegatoIntervento.IdIntervento,
                                                                     NomeFileAllegato = allegatoIntervento.NomeFileAllegato,
                                                                     DescrizioneAllegato = allegatoIntervento.DescrizioneAllegato,
                                                                     DataOraInserimento = allegatoIntervento.DataOraInserimento,
                                                                     IdStazione = allegatoIntervento.Intervento.ID_STAZIONE
                                                                 }
                                                                 ).FirstOrDefault();

                return infoAllegatoIntervento;
            }
        }

        public static List<InfoAllegatoIntervento> ElencoAllegatiIntervento(decimal idIntervento)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from a in context.AllegatiIntervento
                              where a.IdIntervento == idIntervento
                              orderby a.DataOraInserimento descending
                              select new InfoAllegatoIntervento
                              {
                                  IdAllegatoIntervento = a.IdAllegatoIntervento,
                                  IdIntervento = a.IdIntervento,
                                  NomeFileAllegato = a.NomeFileAllegato,
                                  DescrizioneAllegato = a.DescrizioneAllegato,
                                  DataOraInserimento = a.DataOraInserimento,
                                  IdStazione = a.Intervento.ID_STAZIONE
                              }).ToList();

                return elenco;
            }
        }
    }
}
