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
    public static class StrumentoManager
    {
        /// <summary>
        /// Elenco strumenti per stazione
        /// </summary>
        /// <param name="idStazione"></param>
        /// <returns></returns>
        public static List<InfoStrumento> ElencoStrumenti(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                return context.StrumentiStazione
                    .Where(i => i.ID_STAZIONE == idStazione)
                    .OrderBy(i => i.MARCA)
                    .ThenBy(i => i.MODELLO)
                    .Select(i => new InfoStrumento()
                    {
                        IdStrumento = i.ID_STRUMENTO_STAZIONE,
                        IdTipoStrumento = i.ID_TIPO_STRUMENTO,
                        DescrizioneTipoStrumento = i.Tipo.DESCRIZIONE_TIPO_STRUMENTO,
                        NumeroDiSerie = i.NUMERO_DI_SERIE,
                        Marca = i.MARCA,
                        Modello = i.MODELLO,
                        NumeroInventarioArpal = i.NUMERO_INVENTARIO_ARPAL,
                        Caratteristiche = i.CARATTERISTICHE,
                        CodiceSistemaGestionale = i.CODICE_SISTEMA_GESTIONALE,
                        InizioValidita = i.INIZIO_VALIDITA,
                        FineValidita = i.FINE_VALIDITA,
                        IdStazione = i.ID_STAZIONE,
                        CodiceIdentificativoStazione = i.Stazione.CodiceIdentificativo,
                        DescrizioneStazione = i.Stazione.Descrizione,
                        VisibileIntervento = i.Tipo.VISIBILE_INTERVENTO
                    }).ToList();
            }
        }

        public static InfoStrumento CaricaInfoStrumento(decimal idStrumento)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var strumento = context.StrumentiStazione.FirstOrDefault(i => i.ID_STRUMENTO_STAZIONE == idStrumento);
                if (strumento == null)
                    throw new ApplicationException($"CaricaInfoStrumento - ID_STRUMENTO_STAZIONE {idStrumento} non trovato");

                return new InfoStrumento()
                {
                    IdStrumento = strumento.ID_STRUMENTO_STAZIONE,
                    IdTipoStrumento = strumento.ID_TIPO_STRUMENTO,
                    DescrizioneTipoStrumento = strumento.Tipo.DESCRIZIONE_TIPO_STRUMENTO,
                    NumeroDiSerie = strumento.NUMERO_DI_SERIE,
                    Marca = strumento.MARCA,
                    Modello = strumento.MODELLO,
                    NumeroInventarioArpal = strumento.NUMERO_INVENTARIO_ARPAL,
                    Caratteristiche = strumento.CARATTERISTICHE,
                    CodiceSistemaGestionale = strumento.CODICE_SISTEMA_GESTIONALE,
                    InizioValidita = strumento.INIZIO_VALIDITA,
                    FineValidita = strumento.FINE_VALIDITA,
                    IdStazione = strumento.ID_STAZIONE,
                    CodiceIdentificativoStazione = strumento.Stazione.CodiceIdentificativo,
                    DescrizioneStazione = strumento.Stazione.Descrizione,
                    VisibileIntervento = strumento.Tipo.VISIBILE_INTERVENTO
                };
            }
        }

        public static List<TipoStrumento> TipiStrumento()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiStrumento.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE_TIPO_STRUMENTO);
                return elenco.ToList();
            }
        }

        public static List<InfoStrumento.InfoPacchettoStrumento> ElencoPacchettiStrumento(decimal idStrumento)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                return context.PacchettiStrumenti
                    .Where(i => i.ID_STRUMENTO_STAZIONE == idStrumento && (i.TipoPacchetto.DATA_FINE_VALIDITA == null || i.TipoPacchetto.DATA_FINE_VALIDITA >= DateTime.Now))
                    .OrderBy(i => i.TipoPacchetto.ORDINE)
                    .ThenBy(i => i.TipoPacchetto.DESCRIZIONE_PACCHETTO)
                    .Select(i => new InfoStrumento.InfoPacchettoStrumento()
                    {
                        IdPacchettoStrumento = i.ID_PACCHETTO_STRUMENTO,
                        IdPacchetto = i.ID_PACCHETTO,
                        IdStrumento = i.ID_STRUMENTO_STAZIONE,
                        DescrizionePacchetto = i.TipoPacchetto.DESCRIZIONE_PACCHETTO,
                        CodiceAlims = i.TipoPacchetto.CODICE_ALIMS
                    }).ToList();
            }
        }

        public static List<InfoPacchetto> TipiPacchetto()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = from tipoPacchetto in context.TipiPacchetto
                             where tipoPacchetto.DATA_FINE_VALIDITA == null || tipoPacchetto.DATA_FINE_VALIDITA >= DateTime.Now
                             orderby tipoPacchetto.ORDINE, tipoPacchetto.DESCRIZIONE_PACCHETTO
                             select new InfoPacchetto
                             {
                                 IdPacchetto = tipoPacchetto.ID_PACCHETTO,
                                 DescrizionePacchetto = tipoPacchetto.DESCRIZIONE_PACCHETTO,
                                 CodiceAlims = tipoPacchetto.CODICE_ALIMS
                             };
                return elenco.ToList();
            }
        }
    }
}