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
    public static class SensoreManager
    {
        public static List<InfoSensore> ElencoSensoriStrumento(decimal idStrumento)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                return context.Sensori2021
                    .Where(i => i.Strumento.ID_STRUMENTO_STAZIONE == idStrumento)
                    .Select(i => new InfoSensore()
                    {
                        CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
                        Grandezza = i.Grandezza.GRANDEZZA,
                        IdGrandezza = i.ID_GRANDEZZA_STAZIONE,
                        UnitaMisura = i.Grandezza.UNITA_MISURA,
                        UnitaMisuraSensore = i.UNITA_MISURA,
                        EspressioneRisultato = i.ID_TIPO_ESPRESS_RISULTATO == null ? null : i.EspressioneRisultato.DESCR_TIPO_ESPRESS_RISULTATO,
                        FrequenzaAcquisizione = i.FREQUENZA_ACQUISIZIONE,
                        CodicePMC = i.CODICE_PMC,
                        Metodo = i.ID_TIPO_METODO == null ? null : i.Metodo.DESCRIZIONE_METODO,
                        CoefficienteConversioneUnitaMisura = i.COEFF_CONVER_UNITA_MISURA,
                        NumeroDecimali = i.Grandezza.NUMERO_DECIMALI
                    })
                    .OrderBy(i => i.Grandezza)
                    .ToList();
            }
        }

        public static InfoSensore CaricaInfoSensore(string codiceIdentificativo)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var sensore = context.Sensori2021.FirstOrDefault(i => i.CODICE_IDENTIFICATIVO.ToUpper() == codiceIdentificativo.ToUpper());
                if (sensore == null)
                    throw new ApplicationException($"CaricaInfoSensore - CODICE_IDENTIFICATIVO {codiceIdentificativo} non trovato");

                return new InfoSensore()
                {
                    CodiceIdentificativo = sensore.CODICE_IDENTIFICATIVO,
                    IdStrumentoStazione = sensore.ID_STRUMENTO_STAZIONE,
                    Grandezza = sensore.Grandezza.GRANDEZZA,
                    IdGrandezza = sensore.ID_GRANDEZZA_STAZIONE,
                    UnitaMisura = sensore.Grandezza.UNITA_MISURA,
                    UnitaMisuraSensore = sensore.UNITA_MISURA,
                    IdTipoEspressioneRisultato = sensore.ID_TIPO_ESPRESS_RISULTATO,
                    EspressioneRisultato = sensore.ID_TIPO_ESPRESS_RISULTATO == null ? null : sensore.EspressioneRisultato.DESCR_TIPO_ESPRESS_RISULTATO,
                    FrequenzaAcquisizione = sensore.FREQUENZA_ACQUISIZIONE,
                    CodicePMC = sensore.CODICE_PMC,
                    IdTipoMetodo = sensore.ID_TIPO_METODO,
                    Metodo = sensore.ID_TIPO_METODO == null ? null : sensore.Metodo.DESCRIZIONE_METODO,
                    CoefficienteConversioneUnitaMisura = sensore.COEFF_CONVER_UNITA_MISURA,
                    NumeroDecimali = sensore.Grandezza.NUMERO_DECIMALI
                };
            }
        }

        public static List<GrandezzaStazione> ElencoGrandezzeStazione(decimal idStazione)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.GrandezzeStazione.Where(i => i.ID_STAZIONE == idStazione).OrderBy(i => i.GRANDEZZA);
                return elenco.ToList();
            }
        }

        public static List<TipoEspressioneRisultato> TipiEspresssioneRisultato()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiEspressioneRisultato.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCR_TIPO_ESPRESS_RISULTATO);
                return elenco.ToList();
            }
        }

        public static List<TipoMetodo> TipiMetodo()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiMetodo.OrderBy(i => i.ORDINE).ThenBy(i => i.DESCRIZIONE_METODO);
                return elenco.ToList();
            }
        }

        public static List<TipoUnitaMisura2021> TipiUnitaMisura()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.TipiUnitaMisura2021.OrderBy(i => i.ORDINE).ThenBy(i => i.NOME_UNITA_MISURA);
                return elenco.ToList();
            }
        }
    }
}