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
    static class CaratteristicheInstallazioneManager
    {
        /// <summary>
        /// Restituisce la caratteristica installazione più recente relativa alla stazione
        /// </summary>
        /// <param name="idStazione"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static InfoCaratteristicheInstallazione CaratteristicheInstallazioneAttive(decimal idStazione, SIASSEntities context)
        {
            CaratteristicheInstallazione c;

            // Cerca un intervallo comprendente la data corrente e senza data di fine
            c = context.CaratteristicheInstallazioni.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && i.FineValidita == null);

            // Cerca un intervallo comprendente la data corrente e con data di fine
            if (c == null)
                c = context.CaratteristicheInstallazioni.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && DateTime.Today <= i.FineValidita);

            if (c == null)
                return null;

            return new InfoCaratteristicheInstallazione(c);
        }

        public static InfoCaratteristicheInstallazione CaricaInfoCaratteristicheInstallazione(decimal idCaratteristicheInstallazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                CaratteristicheInstallazione c = context.CaratteristicheInstallazioni.FirstOrDefault(i => i.IdCaratteristicheInstallazione == idCaratteristicheInstallazione);
                if (c != null)
                    return new InfoCaratteristicheInstallazione(c);
                else
                    return null;
            }
        }

        public static List<InfoCaratteristicheInstallazione> ElencoCaratteristicheInstallazione(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from c in context.CaratteristicheInstallazioni
                              where c.IdStazione == idStazione
                              orderby c.InizioValidita descending
                              select new InfoCaratteristicheInstallazione
                              {
                                  IdCaratteristicheInstallazione = c.IdCaratteristicheInstallazione,
                                  IdStazione = c.IdStazione,
                                  IdTipoFissaggioTrasmettitore = c.IdTipoFissaggioTrasmettitore,
                                  DescrizioneTipoFissaggioTrasmettitore = c.TipoFissaggioTrasmettitore != null ? c.TipoFissaggioTrasmettitore.DescrizioneTipoFissaggioTrasmettitore : null,
                                  CavoEsternoInGuaina = c.CavoEsternoInGuaina,
                                  CavoSottotraccia = c.CavoSottotraccia,
                                  ProtezioneArea = c.ProtezioneArea,
                                  IdTipoAccesso = c.IdTipoAccesso,
                                  DescrizioneTipoAccesso = c.TipoAccesso != null ? c.TipoAccesso.DescrizioneTipoAccesso : null,
                                  InizioValidita = c.InizioValidita,
                                  FineValidita = c.FineValidita,
                                  Osservazioni = c.Osservazioni,
                                  Sicurezza = c.Sicurezza,
                                  ProfonditaSensore=c.PROFONDITA_SENSORE
                              }).ToList();

                return elenco;
            }
        }

        public static List<TipoFissaggioTrasmettitore> TipiFissaggioTrasmettitore()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiFissaggioTrasmettitore.OrderBy(i => i.DescrizioneTipoFissaggioTrasmettitore);
                return elenco.ToList();
            }
        }
        
        public static List<TipoAccesso> TipiAccesso()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiAccesso.OrderBy(i => i.DescrizioneTipoAccesso);
                return elenco.ToList();
            }
        }
        
        public static bool Valida(CaratteristicheInstallazione c, out string errore)
        {
            bool valida = false;
            errore = null;

            // Coerenza date inizio e fine validità
            if (c.FineValidita.HasValue && (DateTime.Compare(c.FineValidita.Value, c.InizioValidita) < 0))
            {
                errore = "La data di fine validità non può essere antecedente a quella di inizio validità.";
                return valida;
            }

            // Elenco delle altre caratteristiche Installazione realtive alla stazione
            List<CaratteristicheInstallazione> altreCaratteristicheInstallazione = null;
            using (SIASSEntities context = new SIASSEntities())
            {
                altreCaratteristicheInstallazione = context.CaratteristicheInstallazioni.Where(i => i.IdStazione == c.IdStazione && i.IdCaratteristicheInstallazione != c.IdCaratteristicheInstallazione).ToList();
            }
            // Esiste già un intervallo aperto
            if (altreCaratteristicheInstallazione.Any(i => i.FineValidita == null) && !c.FineValidita.HasValue)
            {
                errore = "Esiste già un intervallo di validità aperto.";
                return valida;
            }
            // Verifica che non ci sia sovrapposizione degli intervalli di validità
            if (altreCaratteristicheInstallazione.Any(i => c.InizioValidita >= i.InizioValidita && c.InizioValidita <= i.FineValidita))
            {
                errore = "Gli intervalli di validità sono sovrapposti.";
                return valida;
            }
            else if (c.FineValidita.HasValue)
            {
                if (altreCaratteristicheInstallazione.Any(i => c.FineValidita >= i.InizioValidita && c.FineValidita <= i.FineValidita))
                {
                    errore = "Gli intervalli di validità sono sovrapposti.";
                    return valida;
                }
            }
            return true;
        }
    }
}
