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

public partial class SIAS_CONTROLLI_ANOMALIE
{
	public decimal ID_CONTROLLO { get; set; }

	public string CODICE_FUNZIONE { get; set; }

	public string NOME_CONTROLLO { get; set; }

	public string DESCRIZIONE_CONTROLLO { get; set; }

	public string CONFIGURAZIONE_JSON { get; set; }

	public string RETE { get; set; }

	public decimal? ID_TIPO_STAZIONE { get; set; }

	public string GRANDEZZA { get; set; }

	public string NOME_UNITA_MISURA { get; set; }

	public bool ABILITATO { get; set; }

	public DateTime? DATA_ULTIMA_ESECUZIONE { get; set; }

	public string ESITO_ULTIMA_ESECUZIONE { get; set; }

	public virtual ICollection<SIAS_ANOMALIE> SIAS_ANOMALIE { get; set; } = new List<SIAS_ANOMALIE>();
}
