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

public partial class SIASS_STAZIONI
{
	public decimal ID_STAZIONE { get; set; }

	public string CODICE_IDENTIFICATIVO { get; set; }

	public string DESCRIZIONE { get; set; }

	public bool? ESCLUSA_MONITORAGGIO { get; set; }

	public decimal ID_TIPO_STAZIONE { get; set; }

	public string ANNOTAZIONI { get; set; }

	public decimal? ID_ALLEGATO_FOTO_STAZIONE { get; set; }

	public decimal? ID_ALLEGATO_MAPPA { get; set; }

	public string ALLESTIMENTO { get; set; }

	public decimal? ID_SITO { get; set; }

	public bool? TELETRASMISSIONE { get; set; }

	public DateTime ULTIMO_AGGIORNAMENTO { get; set; }

	public string AUTORE_ULTIMO_AGGIORNAMENTO { get; set; }

	public bool? CONTROLLO_ANOMALIE { get; set; }

	public virtual ICollection<SIAS_ANOMALIE> SIAS_ANOMALIE { get; set; } = new List<SIAS_ANOMALIE>();

	public virtual ICollection<SIAS_GRANDEZZE_STAZIONE> SIAS_GRANDEZZE_STAZIONE { get; set; } = new List<SIAS_GRANDEZZE_STAZIONE>();

	public virtual ICollection<SIAS_STAZIONI_RETI> SIAS_STAZIONI_RETI { get; set; } = new List<SIAS_STAZIONI_RETI>();
}
