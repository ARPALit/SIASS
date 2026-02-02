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
    static class CaratteristicheTecnichePozzoManager
    {
        /// <summary>
        /// Restituisce la caratteristica tecnica pozzo più recente relativa alla stazione
        /// </summary>
        /// <param name="idStazione"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static InfoCaratteristicheTecnichePozzo CaratteristicheTecnichePozzoAttive(decimal idStazione, SIASSEntities context)
        {
            CaratteristicheTecnichePozzo c;

            // Cerca un intervallo comprendente la data corrente e senza data di fine
            c = context.CaratteristicheTecnichePozzi.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && i.FineValidita == null);

            // Cerca un intervallo comprendente la data corrente e con data di fine
            if (c == null)
                c = context.CaratteristicheTecnichePozzi.FirstOrDefault(i => i.IdStazione == idStazione && DateTime.Today >= i.InizioValidita && DateTime.Today <= i.FineValidita);

            if (c == null)
                return null;

            return new InfoCaratteristicheTecnichePozzo(c);
        }

        public static InfoCaratteristicheTecnichePozzo CaricaInfoCaratteristicheTecnichePozzo(decimal idCaratteristicheTecnichePozzo)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                CaratteristicheTecnichePozzo c = context.CaratteristicheTecnichePozzi.FirstOrDefault(i => i.IdCaratteristicheTecnichePozzo == idCaratteristicheTecnichePozzo);
                if (c != null)
                    return new InfoCaratteristicheTecnichePozzo(c);
                else
                    return null;
            }
        }

        public static List<InfoCaratteristicheTecnichePozzo> ElencoCaratteristicheTecnichePozzo(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = (from c in context.CaratteristicheTecnichePozzi
                              where c.IdStazione == idStazione
                              orderby c.InizioValidita descending
                              select new InfoCaratteristicheTecnichePozzo
                              {
                                  IdCaratteristicheTecnichePozzo = c.IdCaratteristicheTecnichePozzo,
                                  IdStazione = c.IdStazione,
                                  Profondita = c.Profondita,
                                  Diametro = c.Diametro,
                                  RangeSoggiacenzaDa = c.RangeSoggiacenzaDa,
                                  RangeSoggiacenzaA = c.RangeSoggiacenzaA,
                                  IdTipoChiusura = c.IdTipoChiusura,
                                  DescrizioneTipoChiusura = c.TipoChiusura != null ? c.TipoChiusura.DescrizioneTipoChiusura : null,
                                  QuotaBoccapozzoPc = c.QuotaBoccapozzoPc,
                                  QuotaBoccapozzoSlm = c.QuotaBoccapozzoSlm,
                                  QuotaPianoRiferimentoSlm = c.QuotaPianoRiferimentoSlm,
                                  DifferenzaPrpc = c.DifferenzaPrpc,
                                  ProfonditaEmungimento = c.ProfonditaEmungimento,
                                  PortataMassimaEsercizio = c.PortataMassimaEsercizio,
                                  PresenzaForoSonda = c.PresenzaForoSonda,
                                  IdTipoDestinazioneUso = c.IdTipoDestinazioneUso,
                                  DescrizioneTipoDestinazioneuso = c.TipoDestinazioneUso != null ? c.TipoDestinazioneUso.DescrizioneTipoDestinazioneUso : null,
                                  IdTipoFrequenzaUtilizzo = c.IdTipoFrequenzaUtilizzo,
                                  DescrizioneTipoFrequenzaUtilizzo = c.TipoFrequenzaUtilizzo != null ? c.TipoFrequenzaUtilizzo.DescrizioneTipoFrequenzaUtilizzo : null,
                                  Captata = c.Captata,
                                  InizioValidita = c.InizioValidita,
                                  FineValidita = c.FineValidita
                              }).ToList();

                return elenco;
            }
        }

        public static List<TipoChiusura> TipiChiusura()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiChiusura.OrderBy(i => i.DescrizioneTipoChiusura);
                return elenco.ToList();
            }
        }

        public static List<TipoDestinazioneUso> TipiDestinazioneUso()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiDestinazioneUso.OrderBy(i => i.DescrizioneTipoDestinazioneUso);
                return elenco.ToList();
            }
        }

        public static List<TipoFrequenzaUtilizzo> TipiFrequenzaUtilizzo()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiFrequenzaUtilizzo.OrderBy(i => i.DescrizioneTipoFrequenzaUtilizzo);
                return elenco.ToList();
            }
        }

        public static bool Valida(CaratteristicheTecnichePozzo c, out string errore)
        {
            bool valida = false;
            errore = null;

            // Coerenza date inizio e fine validità
            if (c.FineValidita.HasValue && (DateTime.Compare(c.FineValidita.Value, c.InizioValidita) < 0))
            {
                errore = "La data di fine validità non può essere antecedente a quella di inizio validità.";
                return valida;
            }

            // Elenco delle altre caratteristiche tecniche pozzo realtive alla stazione
            List<CaratteristicheTecnichePozzo> altreCaratteristicheTecnichePozzo = null;
            using (SIASSEntities context = new SIASSEntities())
            {
                altreCaratteristicheTecnichePozzo = context.CaratteristicheTecnichePozzi.Where(i => i.IdStazione == c.IdStazione && i.IdCaratteristicheTecnichePozzo != c.IdCaratteristicheTecnichePozzo).ToList();
            }
            // Esiste già un intervallo aperto
            if (altreCaratteristicheTecnichePozzo.Any(i => i.FineValidita == null) && !c.FineValidita.HasValue)
            {
                errore = "Esiste già un intervallo di validità aperto.";
                return valida;
            }
            // Verifica che non ci sia sovrapposizione degli intervalli di validità
            if (altreCaratteristicheTecnichePozzo.Any(i => c.InizioValidita >= i.InizioValidita && c.InizioValidita <= i.FineValidita))
            {
                errore = "Gli intervalli di validità sono sovrapposti.";
                return valida;
            }
            else if (c.FineValidita.HasValue)
            {
                if (altreCaratteristicheTecnichePozzo.Any(i => c.FineValidita >= i.InizioValidita && c.FineValidita <= i.FineValidita))
                {
                    errore = "Gli intervalli di validità sono sovrapposti.";
                    return valida;
                }
            }
            return true;
        }

    }
}
