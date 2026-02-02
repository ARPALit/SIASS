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
using SIASSAnomalie.Features;
using SIASSAnomalie.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIASSAnomalie
{
	public class TEST
	{
		readonly Logger logger = LogManager.GetCurrentClassLogger();
		public void PROVA()
		{
			string stringaConnessioneDB = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=10.20.4.96)(PORT=1521))(CONNECT_DATA=(SID=arpal)));User Id=SIASS;Password=siass;";
			try
			{
				logger.Debug(MethodBase.GetCurrentMethod().Name);
				ControlloSpikeIstantaneo.Risposta risposta = new ControlloSpikeIstantaneo.Risposta();

				risposta.EsecuzioneRiuscita = false;
				risposta.PresenzaAnomalia = false;
				risposta.Valore = null;
				risposta.DataValore = null;

				using var context = new SIASSContext(stringaConnessioneDB);
				//logger.Debug($"Controllo 'Spike istantaneo' per idGrandezza:{richiesta.IdGrandezzaStazione} configurazioneJSON:{richiesta.ConfigurazioneJSON}");

				int ampiezzaPeriodoVerifica = 24;
				decimal idGrandezzaStazione = 20023;
				decimal valoreVariazione = 30;

				//var dataCorrente = DateTime.Today;
				var dataCorrente = new DateTime(2024, 08, 11);
				var inizioIntervallo = dataCorrente.AddHours(-ampiezzaPeriodoVerifica);

				var elencoMisurazioni = context.SIAS_MISURAZIONI
					.Where(
					i => i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione
					&&
					i.DATA_MISURAZIONE >= inizioIntervallo
					&&
					i.DATA_MISURAZIONE <= dataCorrente
					)
					.ToList();

				// Ricava una misurazione per ora
				var elencoMisurazioniPerOra = elencoMisurazioni
					.GroupBy(i => new
					{
						i.DATA_MISURAZIONE.Year,
						i.DATA_MISURAZIONE.Month,
						i.DATA_MISURAZIONE.Day,
						i.DATA_MISURAZIONE.Hour
					}
					)
					.Select(i => i.OrderBy(i => i.DATA_MISURAZIONE).First())
					.ToList();

				// Confronta le misurazioni consecutive per vedere se il valore è variato oltre soglia
				if (elencoMisurazioniPerOra.Count >= 2)
				{
					for (int i = 0; i < elencoMisurazioniPerOra.Count - 1; i++)
					{
						// Misurazione di un'ora
						var primaMisurazione = elencoMisurazioni.ElementAt(i);
						// Misurazione ora successiva
						var secondaMisurazione = elencoMisurazioni.ElementAt(i + 1);
						// Se la differenza è maggiore della soglia segnala anomalia
						if (Math.Abs(primaMisurazione.VALORE - secondaMisurazione.VALORE) > valoreVariazione)
						{
							risposta.Valore = secondaMisurazione.VALORE;
							risposta.DataValore = secondaMisurazione.DATA_MISURAZIONE;
							logger.Debug($"Anomalia Valore: {risposta.Valore} - Data: {risposta.DataValore}");
							risposta.PresenzaAnomalia = true;
							break;
						}
					}
				}

				risposta.EsecuzioneRiuscita = true;
				risposta.DescrizioneEsito = "Esecuzione riuscita";
			}
			catch (Exception exception)
			{
				logger.Error(Utils.GetExceptionDetails(exception));
			}
		}
	}
}
