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
    public static class ImportazioneEDIManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void LetturaFile(string[] campiPrimaRigaCsv, TextFieldParser parserCsv, InformazioniFileImportazione informazioniFile)
        {
            logger.Info("Call LetturaFile");

            // Identificazione dei parametri presenti nell'intestazione
            List<ParametroFile> parametriFile = ParametriIntestazione(campiPrimaRigaCsv);

            int rigaCorrente = 1;

            // Lettura del file
            while (!parserCsv.EndOfData)
            {
                rigaCorrente++;

                logger.Trace("LetturaFile - Parsing riga: {0}", rigaCorrente);

                string[] campiCsv = parserCsv.ReadFields();

                // Lettura data e ora della misura; i campi data e ora sono concatenati per il parsing
                DateTime? dataMisura;
                string stringaDataOra = campiCsv[0] + " " + campiCsv[1];
                if (DateTime.TryParseExact(stringaDataOra, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                    dataMisura = d;
                else
                {
                    logger.Debug("LetturaFile - Errore parsing data e ora: {0} riga {1}", stringaDataOra, rigaCorrente);
                    informazioniFile.NumeriRigheNonValide.Add(rigaCorrente);
                    continue;
                }

                // Lettura singole misure
                foreach (var parametro in parametriFile)
                {
                    string stringaMisura = campiCsv[parametriFile.IndexOf(parametro) + 2];
                    if (decimal.TryParse(stringaMisura, NumberStyles.Any, new CultureInfo("it-IT"), out decimal v))
                        informazioniFile.AggiungiMisura(parametro.DescrizioneNelFile, parametro.IdTipoUnitaMisura, v, dataMisura.Value);
                    else
                    {
                        logger.Debug("LetturaFile - Errore parsing misura: {0} riga: {1}", stringaMisura, rigaCorrente);
                        if (!informazioniFile.NumeriRigheNonValide.Contains(rigaCorrente)) informazioniFile.NumeriRigheNonValide.Add(rigaCorrente);
                    }
                }
            }

            informazioniFile.NumeroRighe = rigaCorrente;

            return;
        }

        /// <summary>
        /// Estrazione dei parametri dall'intestazione del file
        /// </summary>
        /// <param name="campiPrimaRigaCsv"></param>
        /// <returns></returns>
        private static List<ParametroFile> ParametriIntestazione(string[] campiPrimaRigaCsv)
        {
            logger.Info("Call ParametriIntestazione");

            // Unità di misura EDI
            List<TipoUnitaMisura> elencoUnitaMisura = TipiUnitaMisura();

            List<ParametroFile> parametri = new List<ParametroFile>();

            // L'intestazione contiene almeno 3 colonne (verificato quando si identifica il file) 
            // e le prima due sono le stringhe "Data" e "Ora" da ignorare
            for (int i = 2; i < campiPrimaRigaCsv.Count(); i++)
            {
                // Aggiunge un parametro all'elenco assegnando l'idTipoUnitaMisura determianto in base alla configurazione SIASS
                var parametroFile = new ParametroFile() { IdTipoUnitaMisura = IdTipoUntaMisuraEDI(campiPrimaRigaCsv[i], elencoUnitaMisura) };
                // Se è trovata corrispondenza la descrizione del parametro è la sottostringa seguente al primo spazio, altrimenti l'intera intestazione del file
                if (parametroFile.IdTipoUnitaMisura.HasValue)
                    parametroFile.DescrizioneNelFile = campiPrimaRigaCsv[i].Substring(campiPrimaRigaCsv[i].IndexOf(' ') + 1);
                else
                    parametroFile.DescrizioneNelFile = campiPrimaRigaCsv[i];

                parametri.Add(parametroFile);
            }

            return parametri;
        }

        private class ParametroFile
        {
            /// <summary>
            /// Descrizione della parametro come riportato nel file
            /// </summary>
            public string DescrizioneNelFile { get; set; }
            /// <summary>
            /// Id del tipo unità di misura corrispondente in SIASS (se trovata)
            /// </summary>
            public decimal? IdTipoUnitaMisura { get; set; }
        }

        private class TipoUnitaMisura
        {
            public decimal IdTipoUnitaMisura { get; set; }
            public string GrandezzaEDI { get; set; }
            public string UnitaMisuraEDI { get; set; }
        }

        /// <summary>
        /// Elenco dei tipi unità di misura che hanno parametri EDI
        /// </summary>
        /// <returns></returns>
        private static List<TipoUnitaMisura> TipiUnitaMisura()
        {
            logger.Info("Call TipiUnitaMisura");

            using (SIASSEntities context = new SIASSEntities())
            {
                return context.TipiUnitaMisura
                    .Where(i => i.GrandezzaEDI != null && i.UnitaMisuraEDI != null)
                    .Select(i => new TipoUnitaMisura
                    {
                        IdTipoUnitaMisura = i.IdTipoUnitaMisura,
                        GrandezzaEDI = i.GrandezzaEDI,
                        UnitaMisuraEDI = i.UnitaMisuraEDI
                    }).ToList();
            }
        }

        /// <summary>
        /// Associazione della descrizione del parametro presente nell'intestazione del
        /// file con il tipo di unità di misura
        /// </summary>
        /// <param name="descrizioneParametroFile"></param>
        /// <param name="elencoUnitaMisura"></param>
        /// <returns></returns>
        private static decimal? IdTipoUntaMisuraEDI(string descrizioneParametroFile, List<TipoUnitaMisura> elencoUnitaMisura)
        {
            logger.Info("Call IdTipoUntaMisuraEDI - DescrizioneParametroFile:{0}", descrizioneParametroFile);

            // Per ogni unità di misura definita in SIASS è cerca la corrispondenza con il testo
            // che descrive il parametro comparando la grandezza (es. PRESSIONE) e l'unità di misura (es. [hPa])
            // presenti nel testo
            foreach (var unitaMisura in elencoUnitaMisura)
            {
                logger.Debug("IdTipoUntaMisuraEDI - GrandezzaEDI: {0} UnitaMisuraEDI: {1}", unitaMisura.GrandezzaEDI, unitaMisura.UnitaMisuraEDI);

                string grandezzaEDI = string.Format(" {0} ", unitaMisura.GrandezzaEDI);
                string unitaMisuraEDI = string.Format("[{0}]", unitaMisura.UnitaMisuraEDI);

                if (descrizioneParametroFile.Contains(grandezzaEDI) && descrizioneParametroFile.Contains(unitaMisuraEDI))
                {
                    logger.Debug("IdTipoUntaMisuraEDI - Corrispondenza trovata: {0}", unitaMisura.IdTipoUnitaMisura);
                    return unitaMisura.IdTipoUnitaMisura;
                }
            }

            logger.Debug("IdTipoUntaMisuraEDI - Corrispondenza non trovata");
            return null;
        }
    }
}