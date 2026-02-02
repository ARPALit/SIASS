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

using MailKit.Security;
using MimeKit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;

namespace SIASSImport
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            try
            {
                logger.Info("Inizio elaborazione");
                var rapporto = new RapportoEsecuzione();
                var elencoFile = CSVManager.ElencoFileCVSDaImportare();
                if (elencoFile.Count > 0)
                {
                    logger.Info($"Trovati {elencoFile.Count} file da importare");
                    var archivioInformazioniSensori = new ArchivioInformazioniSensori();
                    foreach (var nomeFile in elencoFile)
                    {
                        InserimentoMisurazioni(CSVManager.ImportazioneFileCSV(nomeFile, rapporto),
                            nomeFile, rapporto, archivioInformazioniSensori);
                        ArchiviazioneFile(nomeFile, rapporto);
                    }
                }
                else
                    logger.Info("Nessun file da elaborare");

                rapporto.OraFine = DateTime.Now;
                rapporto.SalvaSuFile();
                InvioNotificaEmail(rapporto);
                logger.Info("Elaborazione completata");
            }
            catch (Exception ex)
            {
                logger.Error(Utils.GetExceptionDetails(ex));
                Environment.ExitCode = -1;
            }
        }

        /// <summary>
        /// Il file è archiviato in una sottocartella dell'archivio definita dall'ora di inizio elaborazione.
        /// In caso di errore al nome del file è aggiunto il prefisso "ERRORE"
        /// </summary>
        /// <param name="nomeFile"></param>
        /// <param name="rapporto"></param>
        private static void ArchiviazioneFile(string nomeFile, RapportoEsecuzione rapporto)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - File: {nomeFile}");

            string cartellaArchivio = Path.Combine(Properties.Settings.Default.CartellaArchivio, rapporto.OraInizio.ToString("yyyy-MM-dd HH-mm-ss"));
            Directory.CreateDirectory(cartellaArchivio);

            string nomeFileDestinazione = nomeFile;

            if (rapporto.FileElaborati.FirstOrDefault(i => i.NomeFile == nomeFile).SeErrori)
                nomeFileDestinazione = $"ERRORE_{nomeFileDestinazione}";

            string fileCSVOrigine = Path.Combine(Properties.Settings.Default.CartellaImportazione, nomeFile);

            File.Move(fileCSVOrigine, Path.Combine(cartellaArchivio, nomeFileDestinazione));

            logger.Debug($"File {fileCSVOrigine} archiviato in {cartellaArchivio}");
        }

        private static void InserimentoMisurazioni(List<MisurazioneDaImportare> misurazioni, string nomeFile,
            RapportoEsecuzione rapporto, ArchivioInformazioniSensori archivioInformazioniSensori)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name} - File: {nomeFile}");

            // Configurazione che indica di limitarsi alla verifica dati senza inserimento nel DB
            var soloVerificaDati = Properties.Settings.Default.SoloVerificaDati;

            var rapportoFile = rapporto.FileElaborati.FirstOrDefault(i => i.NomeFile == nomeFile);

            foreach (var misurazione in misurazioni)
            {
                logger.Trace($"Elaborazione misurazione: {misurazione.CodiceIdentificativoSensore} {misurazione.DataOraMisurazione} {misurazione.ValoreMisurazioneUnitaMisuraSensore}");
                // Ricerca informazioni sul sensore associato alla misurazione
                var infoSensore = archivioInformazioniSensori.LetturaInformazioniSensore(misurazione.CodiceIdentificativoSensore);
                if (infoSensore == null)
                {
                    rapportoFile.SeErroreInserimentoDatiDB = true;
                    logger.Warn($"Codice sensore {misurazione.CodiceIdentificativoSensore} non trovato; misurazione non inserita");
                    continue;
                }
                // Assegnazione alla misurazione dell'id grandezza corrispondente in base al valore del sensore
                misurazione.IdGrandezzaStazione = infoSensore.IdGrandezzaStazione;
                // Eventuale conversione del valore
                if (infoSensore.CoefficienteConversioneUnitaMisura.HasValue)
                {                    
                    misurazione.ValoreMisurazioneUnitaMisuraGrandezza = infoSensore.CoefficienteConversioneUnitaMisura.Value * misurazione.ValoreMisurazioneUnitaMisuraSensore;
                    logger.Trace($"Applicata conversione unità di misura: {misurazione.ValoreMisurazioneUnitaMisuraSensore} -> {misurazione.ValoreMisurazioneUnitaMisuraGrandezza}");
                }
                else
                    misurazione.ValoreMisurazioneUnitaMisuraGrandezza = misurazione.ValoreMisurazioneUnitaMisuraSensore;

                // Se la misurazione è già presente su DB viene aggiornata a meno che non sia già stata validata (campo validata != 0)
                var misurazioneSuDB = DAL.LetturaMisurazioneDaDB(misurazione);
                if (misurazioneSuDB != null)
                {
                    if (misurazioneSuDB.Validata != 0)
                    {
                        logger.Trace("Misurazione non inserita perché presente e già validata");
                        continue;
                    }
                    if (!soloVerificaDati) DAL.AggiornamentoMisurazione(misurazione, misurazioneSuDB, rapportoFile);
                }
                else
                    if (!soloVerificaDati) DAL.InserimentoMisurazione(misurazione, rapportoFile);

                rapportoFile.NumeroMisurazioniInserite++;
            }

            rapportoFile.OraFine = DateTime.Now;
            logger.Info($"File: {rapportoFile.NomeFile} - Misurazioni inserite: {rapportoFile.NumeroMisurazioniInserite}");
        }

        /// <summary>
        /// La notifica consiste nell'invio del rapporto se è configurato l'invio
        /// oppure se si è verificato un errore ed è configurato l'invio
        /// della notifica in caso di errore
        /// </summary>
        /// <param name="rapporto"></param>
        private static void InvioNotificaEmail(RapportoEsecuzione rapporto)
        {
            logger.Info($"{MethodBase.GetCurrentMethod().Name}");

            if (Properties.Settings.Default.SeInviareEmailFineEsportazione ||
                (Properties.Settings.Default.SeInviareEmailErrore && rapporto.SeErrori))
            {
                logger.Debug("Invio email...");

                // Destinatari
                List<string> indirizziDestinatari = Properties.Settings.Default.DestinatariEmail.Split(';').ToList();
                if (indirizziDestinatari.Count == 0)
                {
                    logger.Warn("Nessun destinatario email trovato nel file di configurazione.");
                    return;
                }
                logger.Debug($"Destinatari email:{string.Join("; ", indirizziDestinatari.ToArray())}");
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(MailboxAddress.Parse(Properties.Settings.Default.IndirizzoMittenteEmail));
                foreach (var indirizzo in indirizziDestinatari) mimeMessage.To.Add(MailboxAddress.Parse(indirizzo));

                // Titolo
                mimeMessage.Subject = rapporto.Titolo;

                // Corpo
                var builder = new BodyBuilder
                {
                    TextBody = rapporto.ToString()
                };
                mimeMessage.Body = builder.ToMessageBody();

                // Invio
                logger.Debug("Invio messaggio al server SMTP...");
                using (var client = new SmtpClientDestinatarioNonAccettato())
                {
                    client.SslProtocols = SslProtocols.Default;
                    SecureSocketOptions secureSocketOptions = SecureSocketOptions.Auto;
                    if (Properties.Settings.Default.SeAbilitareSSLPerSMTPServer)
                        secureSocketOptions = SecureSocketOptions.SslOnConnect;
                    client.Connect(Properties.Settings.Default.ServerSMTP, Properties.Settings.Default.PortaServerSMTP, secureSocketOptions);
                    // Usa l'autenticazione solo se la password è presente
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.PasswordUtenteSMTP))
                        client.Authenticate(Properties.Settings.Default.NomeUtenteSMTP, Properties.Settings.Default.PasswordUtenteSMTP);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
                logger.Info("Email inviata");
            }
            else
                logger.Info("Invio email non richiesto");
        }
    }
}