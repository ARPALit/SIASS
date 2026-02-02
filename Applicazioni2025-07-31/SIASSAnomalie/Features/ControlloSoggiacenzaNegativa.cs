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
using SIASSAnomalie.Model;
using System.Reflection;
using System.Text.Json;

namespace SIASSAnomalie.Features
{
	public class ControlloSoggiacenzaNegativa()
	{
		readonly Logger logger = LogManager.GetCurrentClassLogger();

		public Risposta EsegueControllo(Richiesta richiesta)
		{
			var risposta = new Risposta();
			try
			{
				logger.Debug(MethodBase.GetCurrentMethod().Name);

				risposta.EsecuzioneRiuscita = false;
				risposta.PresenzaAnomalia = false;
				risposta.Valore = null;
				risposta.DataValore = null;

				using var context = new SIASSContext(richiesta.StringaConnessioneDB);
				logger.Debug($"Controllo 'Soggiacenza Negativa' per idGrandezza:{richiesta.IdGrandezzaStazione} configurazioneJSON:{richiesta.ConfigurazioneJSON}");

				var configurazioneControllo = JsonSerializer.Deserialize<ConfigurazioneControllo>(richiesta.ConfigurazioneJSON);

				var dataCorrente = DateTime.Today;
				var inizioIntervallo = dataCorrente.AddHours(-configurazioneControllo.AmpiezzaPeriodoVerifica.Value);
				var fineIntervallo = dataCorrente;

				var elencoMisurazioniAnomale = context.SIAS_MISURAZIONI
					.Where(
					i => i.ID_GRANDEZZA_STAZIONE == richiesta.IdGrandezzaStazione
					&&
					i.VALORE < configurazioneControllo.ValoreMinimoSoggiacenza.Value
					&&
					i.DATA_MISURAZIONE < fineIntervallo
					&&
					i.DATA_MISURAZIONE >= inizioIntervallo
					)
					.ToList();

				if (elencoMisurazioniAnomale.Count() > 0)
				{
					risposta.Valore = elencoMisurazioniAnomale.OrderByDescending(i => i.DATA_MISURAZIONE).Select(i => i.VALORE).FirstOrDefault();
					risposta.DataValore = elencoMisurazioniAnomale.OrderByDescending(i => i.DATA_MISURAZIONE).Select(i => i.DATA_MISURAZIONE).FirstOrDefault();
					logger.Debug($"Anomalia Valore: {risposta.Valore} - Data: {risposta.DataValore}");
					risposta.PresenzaAnomalia = true;
				}
				risposta.EsecuzioneRiuscita = true;
				risposta.DescrizioneEsito = "Esecuzione riuscita";
				return risposta;
			}
			catch (Exception exception)
			{
				logger.Error(Utils.GetExceptionDetails(exception));
				risposta.EsecuzioneRiuscita = false;
				risposta.DescrizioneEsito = $"Errore: {exception.Message.Substring(0, 90)}";
				return risposta;
			}
		}

		private class ConfigurazioneControllo
		{
			public int? ValoreMinimoSoggiacenza { get; set; }
			public int? AmpiezzaPeriodoVerifica { get; set; }
		}

		public class Richiesta
		{
			public decimal IdGrandezzaStazione { get; set; }
			public string ConfigurazioneJSON { get; set; }
			public string StringaConnessioneDB { get; set; }
		}

		public class Risposta
		{
			public bool EsecuzioneRiuscita { get; set; }
			public bool PresenzaAnomalia { get; set; }
			public string DescrizioneEsito { get; set; }
			public decimal? Valore { get; set; }
			public DateTime? DataValore { get; set; }
		}

	}
}
