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
    static class LocalizzazioneManager
    {
        /// <summary>
        /// Restituisce la localizzazione più recente relativa alla stazione
        /// </summary>
        /// <param name="idStazione"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static InfoLocalizzazione LocalizzazioneAttiva(decimal idStazione, SIASSEntities context)
        {
            Localizzazione l;
            // Cerca un intervallo comprendente la data corrente e senza data di fine
            l = context.Localizzazioni.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && i.FineValidita == null);

            // Cerca un intervallo comprendente la data corrente e con data di fine
            if (l == null)
                l = context.Localizzazioni.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && DateTime.Today <= i.FineValidita);

            if (l == null)
                return null;

            return new InfoLocalizzazione(l);
        }

        public static InfoLocalizzazione CaricaInfoLocalizzazione(decimal idLocalizzazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                Localizzazione info = context.Localizzazioni.FirstOrDefault(i => i.IdLocalizzazione == idLocalizzazione);
                if (info != null)
                    return new InfoLocalizzazione(info);
                else
                    return null;
            }
        }

        public static List<InfoLocalizzazione> ElencoLocalizzazioni(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from l in context.Localizzazioni
                              where l.IdStazione == idStazione
                              orderby l.InizioValidita descending
                              select new InfoLocalizzazione
                              {
                                  IdLocalizzazione = l.IdLocalizzazione,
                                  IdStazione = l.IdStazione,
                                  CodiceComune = l.CodiceComune,
                                  DenominazioneComune = l.Comune.DenominazioneComune,
                                  CodiceProvincia = l.Comune.CodiceProvincia,
                                  DenominazioneProvincia = l.Comune.Provincia.DenominazioneProvincia,
                                  SiglaProvincia = l.Comune.Provincia.SiglaProvincia,
                                  Localita = l.Localita,
                                  IdBacino = l.IdBacino,
                                  DescrizioneBacino = l.Bacino.DescrizioneBacino,
                                  IdCorpoIdrico = l.IdCorpoIdrico,
                                  DescrizioneCorpoIdrico = l.CorpoIdrico.DescrizioneCorpoIdrico,
                                  CTR = l.CTR,
                                  Longitudine = l.Longitudine,
                                  Latitudine = l.Latitudine,
                                  LongitudineGaussBoaga = l.LongitudineGaussBoaga,
                                  LatitudineGaussBoaga = l.LatitudineGaussBoaga,
                                  QuotaPianoCampagna = l.QuotaPianoCampagna,
                                  InizioValidita = l.InizioValidita,
                                  FineValidita = l.FineValidita,
                                  CodiceSIRAL = l.CodiceSIRAL
                              }).ToList();

                return elenco;
            }
        }

        public static bool Valida(Localizzazione loc, out string errore)
        {
            bool valida = false;
            errore = null;

            // Coerenza date inizio e fine validità
            if (loc.FineValidita.HasValue && (DateTime.Compare(loc.FineValidita.Value, loc.InizioValidita) < 0))
            {
                errore = "La data di fine validità non può essere antecedente a quella di inizio validità.";
                return valida;
            }

            // Elenco delle altre localizzazioni
            List<Localizzazione> altreLocalizzazioni = null;
            using (SIASSEntities context = new SIASSEntities())
            {
                altreLocalizzazioni = context.Localizzazioni.Where(i => i.IdStazione == loc.IdStazione && i.IdLocalizzazione != loc.IdLocalizzazione).ToList();
            }
            // Esiste già un intervallo aperto
            if (altreLocalizzazioni.Any(i => i.FineValidita == null) && !loc.FineValidita.HasValue)
            {
                errore = "Esiste già un intervallo di validità aperto.";
                return valida;
            }
            // Verifica che non ci sia sovrapposizione degli intervalli di validità
            if (altreLocalizzazioni.Any(i => loc.InizioValidita >= i.InizioValidita && loc.InizioValidita <= i.FineValidita))
            {
                errore = "Gli intervalli di validità sono sovrapposti.";
                return valida;
            }
            else if (loc.FineValidita.HasValue)
            {
                if (altreLocalizzazioni.Any(i => loc.FineValidita >= i.InizioValidita && loc.FineValidita <= i.FineValidita))
                {
                    errore = "Gli intervalli di validità sono sovrapposti.";
                    return valida;
                }
            }
            return true;
        }
    }
}
