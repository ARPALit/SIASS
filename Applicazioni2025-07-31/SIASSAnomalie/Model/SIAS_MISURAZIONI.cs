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

namespace SIASSAnomalie.Model;

public partial class SIAS_MISURAZIONI
{
	public decimal ID_MISURAZIONE { get; set; }

	public DateTime DATA_MISURAZIONE { get; set; }

	public decimal VALORE { get; set; }

	public decimal VALIDATA { get; set; }

	public decimal ID_GRANDEZZA_STAZIONE { get; set; }

	public DateTime ULTIMO_AGGIORNAMENTO { get; set; }

	public string AUTORE_ULTIMO_AGGIORNAMENTO { get; set; }

	public decimal? ID_INTERVENTO { get; set; }

	public string CODICE_IDENTIFICATIVO_SENSORE { get; set; }

	public decimal? VALORE_SENSORE { get; set; }

	public bool? FONTE_ARPAL { get; set; }

	public virtual SIAS_GRANDEZZE_STAZIONE ID_GRANDEZZA_STAZIONENavigation { get; set; }
}
