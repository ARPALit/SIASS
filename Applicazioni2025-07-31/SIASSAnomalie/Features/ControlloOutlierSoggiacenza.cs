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
	public class ControlloOutlierSoggiacenza()
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
				logger.Debug($"Controllo 'Outlier Soggiacenza' per idGrandezza:{richiesta.IdGrandezzaStazione} configurazioneJSON:{richiesta.ConfigurazioneJSON}");

				var configurazioneControllo = JsonSerializer.Deserialize<ConfigurazioneControllo>(richiesta.ConfigurazioneJSON);

				var dataCorrente = DateTime.Today;
				var inizioFinestraControllo = dataCorrente.AddHours(-configurazioneControllo.AmpiezzaFinestraControllo.Value);
				var inizioPeriodoVerifica = dataCorrente.AddHours(-configurazioneControllo.AmpiezzaPeriodoVerifica.Value);

				var elencoMisurazioniFinestraControllo = context.SIAS_MISURAZIONI
					.Where(
					i => i.ID_GRANDEZZA_STAZIONE == richiesta.IdGrandezzaStazione
					&&
					i.DATA_MISURAZIONE >= inizioFinestraControllo
					&&
					i.DATA_MISURAZIONE < inizioPeriodoVerifica
					)
					.ToList();

				if (elencoMisurazioniFinestraControllo.Count() > 0)
				{
					// Calcolo della media dei valori della finestra di controllo
					double media = (double)elencoMisurazioniFinestraControllo.Average(m => m.VALORE);
					logger.Debug($"Media: {media}");

					// Calcolo della somma dei quadrati delle differenze dalla media
					double sommaQuadratiDifferenze = elencoMisurazioniFinestraControllo.Sum(m => Math.Pow((double)m.VALORE - media, 2));
					logger.Debug($"Somma Quadrati Differenze: {sommaQuadratiDifferenze}");

					// Calcolo della varianza
					double varianza = sommaQuadratiDifferenze / elencoMisurazioniFinestraControllo.Count;
					logger.Debug($"Varianza: {varianza}");

					// Calcolo della deviazione standard
					double deviazioneStandard = Math.Sqrt(varianza);
					logger.Debug($"Deviazione Standard: {deviazioneStandard}");

					// Valori minimo e massimo della finestra di controllo
					var valoreMinimo = media - deviazioneStandard;
					logger.Debug($"Valore minimo: {valoreMinimo}");
					var valoreMassimo = media + deviazioneStandard;
					logger.Debug($"Valore massimo: {valoreMassimo}");

					// Cerca una misurazione nel periodo di verifica esterna all'intervallo minimo/massimo della finestra di controllo
					var misurazioneFuoriIntervallo = context.SIAS_MISURAZIONI
						.Where(
						i => i.ID_GRANDEZZA_STAZIONE == richiesta.IdGrandezzaStazione
						&&
						i.DATA_MISURAZIONE >= inizioPeriodoVerifica
						&&
						(i.VALORE < (decimal)valoreMinimo || i.VALORE > (decimal)(valoreMassimo))
						)
						.OrderBy(i => i.DATA_MISURAZIONE)
						.FirstOrDefault();

					if (misurazioneFuoriIntervallo != null)
					{
						logger.Debug($"Trovate misurazioni fuori intervallo");
						risposta.Valore = misurazioneFuoriIntervallo.VALORE;
						risposta.DataValore = misurazioneFuoriIntervallo.DATA_MISURAZIONE;
						logger.Debug($"Anomalia Valore: {risposta.Valore} - Data: {risposta.DataValore}");
						risposta.PresenzaAnomalia = true;
					}
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
			public int? AmpiezzaPeriodoVerifica { get; set; } // Dati da verificare
			public int? AmpiezzaFinestraControllo { get; set; } // Finestra di controllo
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
