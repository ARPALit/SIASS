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

using NLog;
using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SIASS.BLL
{
	public static class OperatoreSiassManager
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		const string VariabileSessioneOperatore = "OperatoreCorrente";

		/// <summary>
		/// Ritorna l'operatore attivo
		/// </summary>
		/// <returns>Oggetto Operatore prelevato dalla sessione corrente o null</returns>		
		public static ConfigurazioneOperatoreSiass OperatoreCorrente()
		{
			try
			{
				return (ConfigurazioneOperatoreSiass)HttpContext.Current.Session[VariabileSessioneOperatore];
			}
			catch (Exception ex)
			{
				logger.Error("OperatoreCorrente() - Errore {0}", ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Salva in sessione l'oggetto operatore
		/// </summary>
		/// <param name="oper"></param>
		public static void SalvaOperatoreInSessione(ConfigurazioneOperatoreSiass oper)
		{
			try
			{
				HttpContext.Current.Session[VariabileSessioneOperatore] = oper;
			}
			catch (Exception ex)
			{
				logger.Error("SalvaOperatoreInSessione() - Errore {0}", ex.Message);
			}
		}
	}
}