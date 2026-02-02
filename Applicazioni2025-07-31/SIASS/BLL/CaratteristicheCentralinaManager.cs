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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIASS.BLL
{
    static class CaratteristicheCentralinaManager
    {
        public static InfoCaratteristicheCentralina CaratteristicheCentralinaAttive(decimal idStazione, SIASSEntities context)
        {
            CaratteristicheCentralina c;

            // Cerca un intervallo comprendente la data corrente e senza data di fine
            c = context.CaratteristicheCentraline.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && i.FineValidita == null);

            // Cerca un intervallo comprendente la data corrente e con data di fine
            if (c == null)
                c = context.CaratteristicheCentraline.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && DateTime.Today <= i.FineValidita);

            if (c == null)
                return null;

            return new InfoCaratteristicheCentralina(c);
        }

        public static List<InfoCaratteristicheCentralina> ElencoCaratteristicheCentralina(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from c in context.CaratteristicheCentraline
                              where c.IdStazione == idStazione
                              orderby c.InizioValidita descending
                              select new InfoCaratteristicheCentralina
                              {
                                  IdCaratteristicheCentralina = c.IdCaratteristicheCentralina,
                                  IdStazione = c.IdStazione,
                                  NumeroSerieTube = c.NumeroSerieTube,
                                  TubeNumeroInventarioArpal = c.TubeNumeroInventarioArpal,
                                  ModelloTube = c.ModelloTube,
                                  NumeroSerieCavo = c.NumeroSerieCavo,
                                  CavoNumeroInventarioArpal = c.CavoNumeroInventarioArpal,
                                  LunghezzaCavo = c.LunghezzaCavo,
                                  NumeroSerieTroll = c.NumeroSerieTroll,
                                  TrollNumeroInventarioArpal = c.TrollNumeroInventarioArpal,
                                  ModelloTroll = c.ModelloTroll,
                                  NumeroSIM = c.NumeroSIM,
                                  CodiceEDI = c.CodiceEDI,
                                  InizioValidita = c.InizioValidita,
                                  FineValidita = c.FineValidita,
                                  MarcaTube = c.MarcaTube,
                                  NumeroMisurazioni = c.Misurazioni.Count()
                              }).ToList();

                return elenco;
            }
        }
    }
}
