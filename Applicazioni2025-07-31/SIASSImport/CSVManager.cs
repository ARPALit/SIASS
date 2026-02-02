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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SIASSImport
{
    public static class CSVManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Posizioni dati nel file CSV

        private const int CSV_DATAORA = 1;
        private const int CSV_IDSENSORE = 0;
        private const int CSV_VALORE = 2;

        #endregion Posizioni dati nel file CSV

        public static List<string> ElencoFileCVSDaImportare()
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - Cartella importazione: {Properties.Settings.Default.CartellaImportazione}");
            return Directory.EnumerateFiles(Properties.Settings.Default.CartellaImportazione, "*.csv", System.IO.SearchOption.TopDirectoryOnly)
                .Select(f => Path.GetFileName(f)).ToList();
        }

        public static List<MisurazioneDaImportare> ImportazioneFileCSV(string nomeFile, RapportoEsecuzione rapporto)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - Importazione file: {nomeFile}");
            var rapportoFile = new RapportoElaborazioneFile(nomeFile);
            var misurazioni = new List<MisurazioneDaImportare>();

            int contatoreRigheLette = 0;
            using (TextFieldParser parserCSV = new TextFieldParser(Path.Combine(Properties.Settings.Default.CartellaImportazione, nomeFile)))
            {
                parserCSV.CommentTokens = new string[] { Properties.Settings.Default.CarattereCommentiCSV };
                parserCSV.SetDelimiters(new string[] { Properties.Settings.Default.CarattereDelimitatoreCampiCSV });
                parserCSV.HasFieldsEnclosedInQuotes = Properties.Settings.Default.SeCampiCSVRacchiusiTraVirgolette;

                // Salta la prima riga contenente i nomi di colonna
                if (Properties.Settings.Default.SeIntestazioneCSV)
                    parserCSV.ReadLine();

                while (!parserCSV.EndOfData)
                {
                    var misurazione = ParsingRigaCSV(parserCSV.ReadFields(), rapportoFile, ++contatoreRigheLette);
                    if (misurazione != null)
                        misurazioni.Add(misurazione);
                }
            }
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - File: {nomeFile} - Righe presenti: {contatoreRigheLette} - Righe estratte: {misurazioni.Count}");

            rapportoFile.NumeroMisurazioniLette = misurazioni.Count;
            rapportoFile.NumeroRigheFile = contatoreRigheLette;
            rapportoFile.OraFine = DateTime.Now;
            rapporto.FileElaborati.Add(rapportoFile);
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - Fine importazione file: {nomeFile}");
            return misurazioni;
        }

        /// <summary>
        /// Conversione dell'array di valori estratto dalla riga del file
        /// </summary>
        /// <param name="campiCSV"></param>
        /// <param name="rapportoFile"></param>
        /// <returns></returns>
        private static MisurazioneDaImportare ParsingRigaCSV(string[] campiCSV, RapportoElaborazioneFile rapportoFile, int contatoreRigheLette)
        {
            logger.Trace($"{MethodBase.GetCurrentMethod().Name} - Riga {contatoreRigheLette}: {String.Join(",", campiCSV.Select(p => p).ToArray())}");

            bool datiValidi = true;

            #region Presenza dati

            if (campiCSV.Length != 4)
            {
                logger.Warn($"Numero di campi dela riga non valido: {campiCSV.Length} (attesi 4)");
                datiValidi = false;
            }

            if (string.IsNullOrEmpty(campiCSV[CSV_IDSENSORE]))
            {
                logger.Warn($"Campo IdSensore (colonna {CSV_IDSENSORE + 1}) vuoto.");
                datiValidi = false;
            }

            if (string.IsNullOrEmpty(campiCSV[CSV_DATAORA]))
            {
                logger.Warn($"Campo DataOra (colonna {CSV_DATAORA + 1}) vuoto.");
                datiValidi = false;
            }

            if (string.IsNullOrEmpty(campiCSV[CSV_VALORE]))
            {
                logger.Warn($"Campo Valore (colonna {CSV_VALORE + 1}) vuoto.");
                datiValidi = false;
            }

            #endregion Presenza dati

            // Parsing data
            if (!DateTime.TryParseExact(campiCSV[CSV_DATAORA], Properties.Settings.Default.FormatoDataCSV, null, DateTimeStyles.None, out DateTime dataOraMisurazione))
            {
                logger.Warn($"Conversione non riuscita della stringa {campiCSV[CSV_DATAORA]} in data utilizzando il formato {Properties.Settings.Default.FormatoDataCSV}");
                datiValidi = false;
            }

            // Parsing valore
            var culturaConVirgola = new CultureInfo("it")
            {
                NumberFormat =
                {
                    NumberDecimalSeparator = Properties.Settings.Default.SeparatoreDecimaliMisurazioneCSV
                }
            };
            // Sono accettati numeri con separatore decimale e preceduti dal segno + o -
            if (!Decimal.TryParse(campiCSV[CSV_VALORE], NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, culturaConVirgola, out decimal valore))
            {
                logger.Warn($"Conversione non riuscita della stringa {campiCSV[CSV_VALORE]} in numero decimale");
                datiValidi = false;
            }

            if (datiValidi)
            {
                return new MisurazioneDaImportare()
                {
                    CodiceIdentificativoSensore = campiCSV[CSV_IDSENSORE].Trim(),
                    DataOraMisurazione = dataOraMisurazione,
                    ValoreMisurazioneUnitaMisuraSensore = valore
                };
            }
            else
            {
                rapportoFile.SeErroreParsingDatiCVS = true;
                return null;
            }
        }
    }
}