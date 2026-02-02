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
using System.Configuration;
using System.Linq;
using System.Text;

namespace SIASS.BLL
{
    public class SitoManager
    {
        public static InfoSito CaricaSito(decimal idSito)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var sito = context.Siti.FirstOrDefault(i => i.ID_SITO == idSito);
                if (sito == null)
                    throw new ArgumentException($"CaricaSito - IdSito: {idSito} non trovato");

                var infoSito = new InfoSito()
                {
                    IdSito = sito.ID_SITO,
                    CodiceIdentificativo = sito.CODICE_IDENTIFICATIVO,
                    Descrizione = sito.DESCRIZIONE,
                    Longitudine = sito.LONGITUDINE,
                    Latitudine = sito.LATITUDINE,
                    LongitudineGaussBoaga = sito.LONGITUDINE_GAUSS_BOAGA,
                    LatitudineGaussBoaga = sito.LATITUDINE_GAUSS_BOAGA,
                    QuotaPianoCampagna = sito.QUOTA_PIANO_CAMPAGNA,
                    ComuneProvincia = $"{sito.Comune.DenominazioneComune} ({sito.Comune.Provincia.SiglaProvincia})",
                    Indirizzo = sito.INDIRIZZO,
                    Superficie = sito.SUPERFICIE,
                    CodiceIFFI = sito.CODICE_IFFI,
                    Stazioni = sito.Stazioni
                    .OrderBy(i => i.CodiceIdentificativo)
                    .Select(i => new InfoSito.InfoStazioneSito()
                    {
                        IdStazione = i.IdStazione,
                        CodiceIdentificativo = i.CodiceIdentificativo,
                        Descrizione = i.Descrizione,
                        DescrizioneTipoStazione = i.TipoStazione.DescrizioneTipoStazione
                    }).ToList()
                };

                return infoSito;
            }
        }
        public static List<InfoSitoPerElenco> ElencoSiti(string codiceIdentificativoDescrizione, string codiceComune, string codiceProvincia,
            int numeroPagina, int dimensionePagina, out int recordTrovati)
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.Siti.Select(i => i);
                if (!string.IsNullOrEmpty(codiceIdentificativoDescrizione))
                    elenco = elenco.Where(i => i.CODICE_IDENTIFICATIVO.ToLower().Contains(codiceIdentificativoDescrizione.ToLower()) ||
                    i.DESCRIZIONE.ToLower().Contains(codiceIdentificativoDescrizione.ToLower()));
                if (!string.IsNullOrEmpty(codiceComune))
                    elenco = elenco.Where(i => i.CODICE_COMUNE == codiceComune);
                if (!string.IsNullOrEmpty(codiceProvincia))
                    elenco = elenco.Where(i => i.Comune.CodiceProvincia == codiceProvincia);

                recordTrovati = elenco.Count();

                return elenco
                    .OrderBy(i => i.CODICE_IDENTIFICATIVO)
                    .Skip(dimensionePagina * (numeroPagina - 1))
                    .Take(dimensionePagina)
                    .Select(i => new InfoSitoPerElenco()
                    {
                        IdSito = i.ID_SITO,
                        CodiceIdentificativo = i.CODICE_IDENTIFICATIVO,
                        Descrizione = i.DESCRIZIONE,
                        Longitudine = i.LONGITUDINE,
                        Latitudine = i.LATITUDINE,
                        ComuneProvincia = i.Comune.DenominazioneComune + " (" + i.Comune.Provincia.SiglaProvincia + ")"
                    }).ToList();
            }
        }

        public static string GeneraScriptMappa(List<InfoSitoPerElenco> elencoSiti, bool seGestore)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type=\"text/javascript\">");

            // Inizializzaione mappa
            sb.AppendLine("function initMap() {");
            sb.AppendLine("var myLatLng = { lat: 44.25, lng: 8.76 }; ");
            sb.AppendLine("var map = new google.maps.Map(document.getElementById('map'), {");
            sb.AppendLine("center: myLatLng,");
            sb.AppendLine("scrollwheel: true,");
            sb.AppendLine("zoom: 9, ");
            sb.AppendLine("mapTypeId: google.maps.MapTypeId.ROADMAP ");
            sb.AppendLine("}); ");
            sb.AppendLine("var rndLatLng; ");

            // Aggiunge marker singolo sito
            string cartellaImmagini = $"{Utils.ApplicationUrlRoot()}/img/";
            foreach (var s in elencoSiti)
            {
                // Genera tooltip con codice e descrizione
                string tooltipSito = s.CodiceIdentificativo + " - " + s.Descrizione;
                // Escape di eventuali apici
                tooltipSito = tooltipSito.Replace("'", @"\'");

                //  costruisce il maker del sito
                sb.AppendLine("sitoLatLng = { lat: " + s.Latitudine.ToString().Replace(",", ".") + ", lng: " + s.Longitudine.ToString().Replace(",", ".") + " }; ");
                // Immagine
                sb.AppendLine("var image = { url: '" + cartellaImmagini + "Sito.png' }; ");
                // Marker
                sb.AppendLine("var marker" + s.IdSito.ToString() + " = new google.maps.Marker({ " +
                    "position: sitoLatLng, " +
                    "map: map, " +
                    "title: '" + tooltipSito + "', " +
                    "url: '/Sito/VisualizzaSito.aspx?IdSito=" + s.IdSito.ToString() + "', " +
                    "icon: image" +
                "}); ");

                // Aggiunge listener per evento di clic solo per utente gestore
                if (seGestore)
                    sb.AppendLine("google.maps.event.addListener(marker" + s.IdSito.ToString() + ", 'click', function () { window.location.href = marker" + s.IdSito.ToString() + ".url; }); ");
            }

            sb.AppendLine("}</script>");

            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["GoogleAPIKey"]))
                sb.AppendLine(@"<script async defer src='https://maps.googleapis.com/maps/api/js?callback=initMap'></script>");
            else
                sb.AppendLine(@"<script async defer src='https://maps.googleapis.com/maps/api/js?key=" + ConfigurationManager.AppSettings["GoogleAPIKey"] + "&callback=initMap'></script>");

            return sb.ToString();
        }

        public static List<InfoSitoPerSelezione> ElencoSitiPerSelezione()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elenco = context.Siti.Select(i => i);

                return elenco
                    .OrderBy(i => i.CODICE_IDENTIFICATIVO)
                    .Select(i => new InfoSitoPerSelezione()
                    {
                        IdSito = i.ID_SITO,
                        CodiceIdentificativoDescrizioneComuneProvincia = i.CODICE_IDENTIFICATIVO +
                            " - " + i.DESCRIZIONE +
                            " - " + i.Comune.DenominazioneComune +
                            " (" + i.Comune.Provincia.SiglaProvincia + ")"
                    }).ToList();
            }
        }

        public class InfoSitoPerSelezione
        {
            public decimal IdSito { get; set; }
            public string CodiceIdentificativoDescrizioneComuneProvincia { get; set; }
        }
    }
}