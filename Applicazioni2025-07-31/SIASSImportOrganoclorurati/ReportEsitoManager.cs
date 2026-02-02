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

using System;
using System.Collections.Generic;
using System.Text;

namespace SIASSImportOrganoclorurati
{
    internal class ReportEsitoManager
    {
        internal static string ImportazioneRiuscita(Importazione importazione, EsitoInserimento esitoInserimento)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(InformazioniImportazione(importazione));
            sb.AppendLine($"Esito importazione: RIUSCITA");
            sb.AppendLine($"Misurazioni acquisite: {esitoInserimento.NumeroMisurazioniImportate}");
            sb.AppendLine($"Misurazioni aggiornate perché già presenti: {esitoInserimento.MisurazioniAggiornate.Count}");
            if (esitoInserimento.MisurazioniAggiornate.Count > 0)
            {
                sb.AppendLine($"Dettaglio misurazioni aggiornate:");
                foreach (var misurazione in esitoInserimento.MisurazioniAggiornate)
                    sb.AppendLine($"Sito: {misurazione.Sito} Piezometro: {misurazione.Piezometro} Data campionamento: {misurazione.DataCampionamento:d} Parametro: {misurazione.Parametro} Valore: {misurazione.ConcVal}");
            }
            return sb.ToString();
        }

        internal static string InserimentoMisurazioniFallito(Importazione importazione)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(InformazioniImportazione(importazione));
            sb.AppendLine($"Esito importazione: FALLITA");
            sb.AppendLine($"Si è verificato un errore di elaborazione; segnalare il problema all'Agenzia riportando le informazioni di questo documento");
            return sb.ToString();
        }

        internal static string ParsingFallito(Importazione importazione, EsitoParsingFile esitoParsing)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(InformazioniImportazione(importazione));
            sb.AppendLine($"Esito importazione: FALLITA");
            sb.AppendLine($"Sono stati rilevati uno o più errori durante l'analisi del file inviato. Nessun dato è stato acquisito.");
            sb.AppendLine($"Correggere gli errori riportato di seguito e inviare nuovamente i dati.");
            sb.AppendLine();
            if (esitoParsing.ErroreFile != null)
                sb.AppendLine(esitoParsing.ErroreFile);
            else
            {
                sb.AppendLine($"Numero di errori: {esitoParsing.ErroriParsing.Count} (sono riportati solo i primi 100)");
                int contatore = 0;
                foreach (var erroreParsing in esitoParsing.ErroriParsing)
                {
                    if (++contatore > 100) break;
                    sb.AppendLine(erroreParsing);
                }
            }

            return sb.ToString();
        }

        internal static string VerificaPresenzaStazioniFallita(Importazione importazione, List<string> stazioniMancanti)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(InformazioniImportazione(importazione));
            sb.AppendLine($"Esito importazione: FALLITA");
            sb.AppendLine($"Sono stati rilevati uno o più dati per i quali nella base dati non è presente una stazione in base alla seguente corrispondenza:");
            sb.AppendLine($"\tCodice SIASS identificativo del sito = colonna 'sito' del file d'importazione");
            sb.AppendLine($"\tCodice SIASS identificativo della stazione = colonna 'piezometro' del file d'importazione");
            sb.AppendLine($"Nessun dato è stato acquisito.");
            
            sb.AppendLine($"Stazioni non trovate ({stazioniMancanti.Count}):");
            foreach (var stazione in stazioniMancanti)
                sb.AppendLine($"\t{stazione}");

            return sb.ToString();
        }

        private static string InformazioniImportazione(Importazione importazione)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Id importazione: {importazione.IdImportazione}");
            sb.AppendLine($"Ricezione file: {importazione.DataRicezioneFile}");
            sb.AppendLine($"Operatore: {importazione.Operatore}");
            sb.AppendLine($"File: {importazione.NomeFile}");
            sb.AppendLine($"Importazione file: {DateTime.Now}");
            sb.AppendLine("------------------------------------------------------------");
            return sb.ToString();
        }
    }
}
