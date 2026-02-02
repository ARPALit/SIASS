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

using Microsoft.VisualBasic.FileIO;
using NLog;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SIASS.BLL
{
    public static class ImportazioneOTTManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void LetturaFile(decimal idCaratteristicheCentralina, string[] campiPrimaRigaCsv, TextFieldParser parserCsv, InformazioniFileImportazione informazioniFile)
        {
            logger.Info("Call LetturaFile");

            List<InfoSensore> sensori = SensoriPerCentralina(idCaratteristicheCentralina);

            int rigaCorrente = 1;


            // Parsing prima riga            
            if (LetturaRigaFile(campiPrimaRigaCsv, out string codiceSensore, out DateTime? dataMisura, out decimal? valore))
                informazioniFile.AggiungiMisura(codiceSensore, IdTipoUnitaMisuraPerCodiceSensore(codiceSensore, sensori), valore.Value, dataMisura.Value);
            else
                informazioniFile.NumeriRigheNonValide.Add(rigaCorrente);

            // Lettura del resto del file
            while (!parserCsv.EndOfData)
            {
                rigaCorrente++;
                logger.Trace("LetturaFile - Parsing riga: {0}", rigaCorrente);

                if (LetturaRigaFile(parserCsv.ReadFields(), out codiceSensore, out dataMisura, out valore))
                    informazioniFile.AggiungiMisura(codiceSensore, IdTipoUnitaMisuraPerCodiceSensore(codiceSensore, sensori), valore.Value, dataMisura.Value);
                else
                    informazioniFile.NumeriRigheNonValide.Add(rigaCorrente);
            }

            informazioniFile.NumeroRighe = rigaCorrente;
        }

        private static bool LetturaRigaFile(string[] campiCsv, out string codiceSensore, out DateTime? dataMisura, out decimal? valore)
        {
            codiceSensore = null;
            dataMisura = null;
            valore = null;
            try
            {
                if (string.IsNullOrWhiteSpace(campiCsv[1]))
                {
                    logger.Debug("LetturaRigaFile - Codice sensore non presente");
                    return false;
                }
                else
                    codiceSensore = campiCsv[1];

                // I campi data e ora sono concatenati per il parsing
                string stringaDataOra = campiCsv[2] + " " + campiCsv[3];
                if (DateTime.TryParseExact(stringaDataOra, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                {
                    dataMisura = d;
                }
                else
                {
                    logger.Debug("LetturaRigaFile - Errore parsing data e ora: {0}", stringaDataOra);
                    return false;
                }

                if (decimal.TryParse(campiCsv[4], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal v))
                {
                    valore = v;
                }
                else
                {
                    logger.Debug("LetturaRigaFile - Errore parsing misura: {0}", campiCsv[4]);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("LetturaRigaFile - Errore parsing: {0}", ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Informazioni sensori associati alla centralina
        /// </summary>
        /// <param name="idCaratteristicheCentralina"></param>
        /// <returns></returns>
        private static List<InfoSensore> SensoriPerCentralina(decimal idCaratteristicheCentralina)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                return context.Sensori
                    .Where(i => i.IdCaratteristicheCentralina == idCaratteristicheCentralina && i.Abilitato)
                    .Select(i => new InfoSensore
                    {
                        CodiceSensore = i.CodiceSensore,
                        IdTipoUnitaMisura = i.IdTipoUnitaMisura,
                        Grandezza = i.UnitaMisura.Grandezza,
                        UnitaDiMisura = i.UnitaMisura.DescrizioneTipoUnitaMisura
                    }
                    ).ToList();
            }
        }

        private class InfoSensore
        {
            public string CodiceSensore { get; set; }
            public decimal IdTipoUnitaMisura { get; set; }
            public string Grandezza { get; set; }
            public string UnitaDiMisura { get; set; }
        }

        /// <summary>
        /// Determina l'id del tipo unità di misura associato al codice sensore, 
        /// se non è trovatacorrsipondenza è restituito null
        /// </summary>
        /// <param name="codiceSensore"></param>
        /// <param name="sensori"></param>
        /// <returns></returns>
        private static decimal? IdTipoUnitaMisuraPerCodiceSensore(string codiceSensore, List<InfoSensore> sensori)
        {
            var sensore = sensori.FirstOrDefault(i => i.CodiceSensore == codiceSensore);
            if (sensore != null)
                return sensore.IdTipoUnitaMisura;
            else
                return null;
        }
    }
}