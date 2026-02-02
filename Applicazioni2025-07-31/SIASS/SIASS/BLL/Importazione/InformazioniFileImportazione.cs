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

using SIASS.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIASS.BLL
{
    [Serializable]
    public class InformazioniFileImportazione
    {
        /// <summary>
        /// Tipo del file: EDI o OTT
        /// </summary>
        public string TipoFile { get; set; }
        /// <summary>
        /// Indica se il file è valido per l'importazione
        /// </summary>
        public bool SeValido { get; set; }
        /// <summary>
        /// Descrizione del motivo per cui il file non è valido
        /// </summary>
        public string MotivoNonValido { get; set; }
        /// <summary>
        /// Numero di righe del file (per il tipo OTT coincide col numero di campioni)
        /// </summary>
        public int NumeroRighe { get; set; }
        /// <summary>
        /// Misure valide lette dal file
        /// </summary>
        private List<Misura> Misure { get; set; }
        /// <summary>
        /// Numeri delle righe non valide, per EDI potrebbe non essere valido un solo valore
        /// </summary>
        public List<int> NumeriRigheNonValide { get; set; }
        public InformazioniFileImportazione()
        {
            Misure = new List<Misura>();
            NumeriRigheNonValide = new List<int>();
        }

        public void AggiungiMisura(string descrizioneNelFile, decimal? idTipoUnitaMisura, decimal valore, DateTime data)
        {
            Misure.Add(new InformazioniFileImportazione.Misura()
            {
                DescrizioneNelFile = descrizioneNelFile,
                Valore = valore,
                IdTipoUnitaMisura = idTipoUnitaMisura,
                Data = data
            });
        }

        /// <summary>
        /// Informazioni sui parametri
        /// </summary>
        public List<Parametro> Parametri
        {
            get
            {
                // Informazioni sulle unità di misura presenti in SISS
                using (SIASSEntities context = new SIASSEntities())
                {
                    // Elenco dei tipi di unità di misura usato per comporre la descrizione presente in SIASS
                    var tipiUnitaMisura = context.TipiUnitaMisura.ToList();

                    var elenco = Misure.GroupBy(x => new { x.DescrizioneNelFile, x.IdTipoUnitaMisura })
                    .Select(p =>
                    {
                        string descrizioneSIASS = null;
                        // Se esiste il tipo unità di misura coorispondente all'id è composta la descrizione
                        if (p.Key.IdTipoUnitaMisura.HasValue)
                        {
                            var tipoUnitaMisura = tipiUnitaMisura.FirstOrDefault(u => u.IdTipoUnitaMisura == p.Key.IdTipoUnitaMisura.Value);
                            if (tipiUnitaMisura != null)
                                descrizioneSIASS = string.Format("{0} [{1}]", tipoUnitaMisura.Grandezza, tipoUnitaMisura.DescrizioneTipoUnitaMisura);
                        }
                        return new Parametro
                        {
                            DescrizioneNelFile = p.Key.DescrizioneNelFile,
                            DescrizioneSIASS = descrizioneSIASS,
                            NumeroMisure = p.Count()
                        };
                    });

                    return elenco.ToList();

                }
            }
        }

        /// <summary>
        /// Misure importabili in SIASS perché associate a un tipo unità di misura esistente
        /// </summary>
        public List<Misura> MisureImportabili
        {
            get
            {
                return Misure.Where(i => i.IdTipoUnitaMisura != null).ToList();
            }
        }

        /// <summary>
        /// Singola misura letta
        /// </summary>
        [Serializable]
        public class Misura
        {
            /// <summary>
            /// Descrizione della misura come riportata nel file
            /// Viene usata per popolare il codice sensore nel caso di importazione da file OTT
            /// </summary>
            public string DescrizioneNelFile { get; set; }
            /// <summary>
            /// Id del tipo unità di misura corrispondente in SIASS (se trovata)
            /// </summary>
            public decimal? IdTipoUnitaMisura { get; set; }
            /// <summary>
            /// Valore della misura
            /// </summary>
            public decimal Valore { get; set; }
            /// <summary>
            /// Data della misura
            /// </summary>
            public DateTime Data { get; set; }
        }

        /// <summary>
        /// Informazioni sul singolo paraemtro letto dal file
        /// </summary>
        public class Parametro
        {
            /// <summary>
            /// Descrizione della misura come riportata nel file
            /// </summary>
            public string DescrizioneNelFile { get; set; }
            /// <summary>
            /// Descrizione SIASS della misura (se esiste)
            /// </summary>
            public string DescrizioneSIASS { get; set; }
            /// <summary>
            /// Numero di misure lette per il parametro
            /// </summary>
            public int NumeroMisure { get; set; }
        }
    }
}