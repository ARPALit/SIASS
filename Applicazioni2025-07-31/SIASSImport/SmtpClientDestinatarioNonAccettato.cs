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

using MailKit.Net.Smtp;
using MimeKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIASSImport
{
    /// <summary>
    /// Ovverride del client MailKit per gestire il caso in cui destinatari siano rifiutati
    /// senza generare un'eccezione che interrompe l'esecuzione
    /// </summary>
    public class SmtpClientDestinatarioNonAccettato : SmtpClient
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected override void OnRecipientNotAccepted(MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
        {
            try
            {
                base.OnRecipientNotAccepted(message, mailbox, response);
            }
            catch (SmtpCommandException ex)
            {
                logger.Warn("Address not accepted: {0}", mailbox.Address, ex.Message);
            }
        }

        protected override void OnNoRecipientsAccepted(MimeMessage message)
        {
            logger.Warn("No recipients accepted - Message: {0}", message.Subject);
        }
    }
}
