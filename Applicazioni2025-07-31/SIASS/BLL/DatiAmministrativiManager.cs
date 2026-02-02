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
    static class DatiAmministrativiManager
    {
        /// <summary>
        /// Restituisce i dati amministrativi più recenti relativi alla stazione
        /// </summary>
        /// <param name="idStazione"></param>
        /// <returns></returns>
        public static InfoDatiAmministrativi DatiAmministrativAttivi(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                return DatiAmministrativAttivi(idStazione, context);
            }
        }

        public static InfoDatiAmministrativi DatiAmministrativAttivi(decimal idStazione, SIASSEntities context)
        {
            DatiAmministrativi d;
            // Cerca un intervallo comprendente la data corrente e senza data di fine
            d = context.DatiAmministrativi.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && i.FineValidita == null);

            // Cerca un intervallo comprendente la data corrente e con data di fine
            if (d == null)
                d = context.DatiAmministrativi.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && DateTime.Today <= i.FineValidita);

            if (d == null)
                return null;

            return new InfoDatiAmministrativi(d);
        }

        public static InfoDatiAmministrativi CaricaInfoDatiAmministrativi(decimal idDatiAmministrativi)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                DatiAmministrativi info = context.DatiAmministrativi.FirstOrDefault(i => i.IdDatiAmministrativi == idDatiAmministrativi);
                if (info != null)
                    return new InfoDatiAmministrativi(info);
                else
                    return null;
            }
        }

        public static List<InfoDatiAmministrativi> ElencoDatiAmministrativi(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from d in context.DatiAmministrativi
                              where d.IdStazione == idStazione
                              orderby d.InizioValidita descending
                              select new InfoDatiAmministrativi
                              {
                                  IdDatiAmministrativi = d.IdDatiAmministrativi,
                                  IdStazione = d.IdStazione,
                                  Gestore = d.Gestore,
                                  IndirizzoGestore = d.IndirizzoGestore,
                                  TelefonoGestore = d.TelefonoGestore,
                                  PartitaIVAGestore = d.PIVA_GESTORE,
								  RiferimentoGestore = d.RiferimentoGestore,
                                  InizioValidita = d.InizioValidita,
                                  FineValidita = d.FineValidita
                              }).ToList();

                return elenco;
            }
        }
    }
}
