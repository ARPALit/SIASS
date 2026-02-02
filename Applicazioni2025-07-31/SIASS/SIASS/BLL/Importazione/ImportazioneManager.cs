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
using System.Globalization;

namespace SIASS.BLL
{
    public static class ImportazioneManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Lettura del file
        /// </summary>
        /// <param name="idCaratteristicheCentralina">Identificativo centralina cui associare i dati</param>
        /// <param name="percorsoFile">Percorso completo del file system in cui si trova il file</param>
        /// <returns></returns>
        public static InformazioniFileImportazione LetturaFile(decimal idCaratteristicheCentralina, string percorsoFile)
        {
            logger.Info("Call LetturaFile - idCaratteristicheCentralina: {0} percorsoFile:{1}", idCaratteristicheCentralina, percorsoFile);

            InformazioniFileImportazione informazioniFile = new InformazioniFileImportazione();

            try
            {
                using (TextFieldParser parserCsv = new TextFieldParser(percorsoFile, System.Text.Encoding.Default))
                {
                    parserCsv.CommentTokens = new string[] { "#" };
                    parserCsv.SetDelimiters(new string[] { ";" });
                    parserCsv.HasFieldsEnclosedInQuotes = false;


                    // Verifica se il file è vuoto
                    if (parserCsv.EndOfData)
                    {
                        logger.Debug("LetturaFile - File vuoto");
                        informazioniFile.SeValido = false;
                        informazioniFile.MotivoNonValido = "File vuoto";
                        return informazioniFile;
                    }

                    // Lettura prima riga
                    string[] campiPrimaRigaCsv = parserCsv.ReadFields();

                    // Identifica il tipo di file
                    IdentificaTipoFile(campiPrimaRigaCsv, informazioniFile);

                    if (!informazioniFile.SeValido)
                    {
                        logger.Debug("LetturaFile - File non valido - Motivo: {0}", informazioniFile.MotivoNonValido);
                        return informazioniFile;
                    }

                    if (informazioniFile.TipoFile == "OTT") ImportazioneOTTManager.LetturaFile(idCaratteristicheCentralina, campiPrimaRigaCsv, parserCsv, informazioniFile);
                    if (informazioniFile.TipoFile == "EDI") ImportazioneEDIManager.LetturaFile(campiPrimaRigaCsv, parserCsv, informazioniFile);
                }
                return informazioniFile;
            }
            catch (Exception ex)
            {
                informazioniFile.SeValido = false;
                informazioniFile.MotivoNonValido = Utils.GetExceptionDetails(ex);
                logger.Debug("LetturaFile - Errore lettura file: {0}", informazioniFile.MotivoNonValido);
                return informazioniFile;
            }
        }

        /// <summary>
        /// Identifica il tipo di file OTT o EDI valorizzando la proprietà SeValido
        /// In caso di identificazione riuscita è valorizzato TipoFile altrimenti NonValidoMotivo
        /// </summary>
        /// <param name="parserCsv"></param>
        /// <param name="informazioniFile"></param>
        private static void IdentificaTipoFile(string[] campiPrimaRigaCsv, InformazioniFileImportazione informazioniFile)
        {
            logger.Info("Call IdentificaTipoFile");

            informazioniFile.TipoFile = null;
            informazioniFile.SeValido = false;
            informazioniFile.MotivoNonValido = "Impossibile identificare il formato del file in base alla prima riga";

            // In ogni caso devono essere presenti almeno 3 colonne
            if (campiPrimaRigaCsv.Length < 3)
            {
                informazioniFile.MotivoNonValido = "Il file deve contenere almeno 3 colonne";
                return;
            }

            // EDI: i primi due campi della prima riga contengono le stringhe "Data" e "Ora"
            if (campiPrimaRigaCsv[0].Equals("Data") && campiPrimaRigaCsv[1].Equals("Ora"))
            {
                informazioniFile.SeValido = true;
                informazioniFile.TipoFile = "EDI";
                informazioniFile.MotivoNonValido = null;
                return;
            }

            // OTT: almeno cinque colonne, la terza è una data dd/MM/yyyy e la quarta un'ora HH:mm:ss
            if (campiPrimaRigaCsv.Length == 5)
            {
                // I campi data e ora sono concatenati e ne è tentato il parsing
                string stringaDataOra = campiPrimaRigaCsv[2] + " " + campiPrimaRigaCsv[3];
                if (DateTime.TryParseExact(stringaDataOra, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    informazioniFile.SeValido = true;
                    informazioniFile.TipoFile = "OTT";
                    informazioniFile.MotivoNonValido = null;
                    return;
                }
            }
        }
    }
}