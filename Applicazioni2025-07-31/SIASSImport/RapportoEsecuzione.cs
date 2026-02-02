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
using System.IO;
using System.Linq;
using System.Text;

namespace SIASSImport
{
    /// <summary>
    /// Informazioni sull'elaborazione di un singolo file
    /// </summary>
    public class RapportoElaborazioneFile
    {
        public RapportoElaborazioneFile(string nomeFile)
        {
            OraInizio = DateTime.Now;
            NomeFile = nomeFile;
            SeErroreParsingDatiCVS = false;
            SeErroreInserimentoDatiDB = false;
        }

        public string NomeFile { get; }
        public int NumeroMisurazioniInserite { get; set; }
        public int NumeroMisurazioniLette { get; set; }
        public int NumeroRigheFile { get; set; }
        public DateTime? OraFine { get; set; }
        public DateTime OraInizio { get; }
        public bool SeErroreInserimentoDatiDB { get; set; }
        public bool SeErroreParsingDatiCVS { get; set; }
        public bool SeErrori
        {
            get
            {
                return SeErroreParsingDatiCVS || SeErroreInserimentoDatiDB;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if(Properties.Settings.Default.SoloVerificaDati)
                sb.AppendLine($"L'esecuzione non ha inserito i dati nel DB perché è attiva l'opzione di configurazione 'SoloVerificaDati'");
            sb.AppendLine($"File: {NomeFile}");
            sb.AppendLine($"Ora inizio: {OraInizio:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Ora fine: {OraFine:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"N° righe del file: {NumeroRigheFile}");
            sb.AppendLine($"N° misurazioni lette: {NumeroMisurazioniLette}");
            sb.AppendLine($"N° misurazioni inserite: {NumeroMisurazioniInserite}");
            if (SeErroreParsingDatiCVS)
                sb.AppendLine($"*** Sono presenti uno o più errori nel parsing, vedere i messaggi di avviso nel log ***");
            if (SeErroreInserimentoDatiDB)
                sb.AppendLine($"*** Sono presenti uno o più errori nell'inserimento nel DB delle misurazioni, vedere i messaggi di avviso nel log ***");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Informazioni sull'intera esecuzione dell'applicazione
    /// </summary>
    public class RapportoEsecuzione
    {
        public RapportoEsecuzione()
        {
            OraInizio = DateTime.Now;
            FileElaborati = new List<RapportoElaborazioneFile>();
            Errori = new List<string>();
        }

        public List<string> Errori { get; set; }
        public List<RapportoElaborazioneFile> FileElaborati { get; set; }
        public DateTime? OraFine { get; set; }
        public DateTime OraInizio { get; }
        public bool SeErrori
        {
            get
            {
                return Errori.Count > 0 || FileElaborati.Any(i => i.SeErroreInserimentoDatiDB || i.SeErroreParsingDatiCVS);
            }
        }

        public string Titolo
        {
            get
            {
                return $"SIASS Importazione {OraInizio:yyyy-MM-dd HH-mm-ss} completata{(SeErrori ? " CON ERRORI" : null)}";
            }
        }

        public void SalvaSuFile()
        {
            string nomeFileRapporto = $"{OraInizio:yyyy-MM-dd HH-mm-ss}{(SeErrori ? " CON ERRORI" : null)}.txt";
            string cartellaRapporti = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Log\Rapporti");
            Directory.CreateDirectory(cartellaRapporti);
            File.WriteAllText(Path.Combine(cartellaRapporti, nomeFileRapporto), this.ToString());
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Ora inizio: {OraInizio:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Ora fine: {OraFine:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Cartella importazone: {Properties.Settings.Default.CartellaImportazione}");
            sb.AppendLine($"N° file elaborati: {FileElaborati.Count}");
            if (Errori.Count != 0)
            {
                sb.AppendLine($"Errori di elaborazione ({Errori.Count}):");
                foreach (var errore in Errori)
                    sb.AppendLine(errore);
            }
            sb.AppendLine();
            foreach (var fileElaborato in FileElaborati)
                sb.AppendLine(fileElaborato.ToString());

            return sb.ToString();
        }
    }
}