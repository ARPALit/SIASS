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
using System.Configuration;
using System.Linq;

namespace SIASS.BLL
{
    public static class ConfigurazioneManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region GrandezzeStazione
        public static List<string> ElencoTipiUnitaMisura()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select nome_unita_misura " +
                    "from sias_tipi_unita_misura " +
                    "order by ordine, nome_unita_misura";
                return context.Database.SqlQuery<string>(query).ToList();
            }
        }

        public static List<string> ElencoTipiGrandezza()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select nome_grandezza " +
                    "from sias_tipi_grandezza " +
                    "order by ordine, nome_grandezza";
                return context.Database.SqlQuery<string>(query).ToList();
            }
        }

        public static bool AggiungiGrandezzaStazione(decimal idStazione, string grandezza, string unitaMisura, decimal numeroDecimali)
        {
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();

            using (SIASSEntities context = new SIASSEntities())
            {
                logger.Debug($"Aggiunta grandezza stazione IdStazione {idStazione}; grandezza: {grandezza}; unità di misura: {unitaMisura}; numero decimali: {numeroDecimali}");

                decimal idGrandezzaStazione;

                var verificaGrandezza = context.GrandezzeStazione.Where(i => i.ID_STAZIONE == idStazione && i.GRANDEZZA == grandezza && i.UNITA_MISURA == unitaMisura).FirstOrDefault();

                if (verificaGrandezza != null)
                {
                    // Grandezza già assegnata a stazione
                    logger.Debug("Grandezza già assegnata");
                    // Legge id della grandezza da usare per l'associazione al sensore
                    idGrandezzaStazione = verificaGrandezza.ID_GRANDEZZA_STAZIONE;
                }
                else
                {

                    GrandezzaStazione g = new GrandezzaStazione
                    {
                        ID_STAZIONE = idStazione,
                        GRANDEZZA = grandezza,
                        UNITA_MISURA = unitaMisura,
                        NUMERO_DECIMALI = numeroDecimali
                    };

                    context.GrandezzeStazione.Add(g);
                    context.SaveChanges();
                    logger.Debug("Grandezza aggiunta");
                    // Legge id della grandezza da usare per l'associazione al sensore
                    idGrandezzaStazione = g.ID_GRANDEZZA_STAZIONE;
                }

                // Se non presente aggiunge lo strumento lettura sul campo
                decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

                decimal idStrumentoStazione;
                var strumentoLetturaSulCampo = context.StrumentiStazione.Where(i => i.ID_STAZIONE == idStazione && i.ID_TIPO_STRUMENTO == idTipoStrumentoLetturaSulCampo).FirstOrDefault();
                if (strumentoLetturaSulCampo == null)
                {
                    logger.Debug("Strumento lettura sul campo non presente");
                    // Lo strumento non esiste, lo inserisce
                    StrumentoStazione strumento = new StrumentoStazione
                    {
                        ID_STAZIONE = idStazione,
                        ID_TIPO_STRUMENTO = idTipoStrumentoLetturaSulCampo,
                        INIZIO_VALIDITA = DateTime.Now,
                        ULTIMO_AGGIORNAMENTO = DateTime.Now,
                        AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}"
                    };
                    context.StrumentiStazione.Add(strumento);
                    context.SaveChanges();
                    idStrumentoStazione = strumento.ID_STRUMENTO_STAZIONE;
                    logger.Debug($"Strumento lettura sul campo aggiunto: idStrumentoStazione {idStrumentoStazione}");
                }
                else
                {
                    logger.Debug("Strumento lettura sul campo presente");
                    // Lo strumento esiste, legge id
                    idStrumentoStazione = strumentoLetturaSulCampo.ID_STRUMENTO_STAZIONE;
                    logger.Debug($"idStrumentoStazione {idStrumentoStazione}");
                }

                // Se non presente crea il sensore per la nuova grandezza
                var sensoreStrumentoLetturaSulCampo = context.Sensori2021.Where(i => i.ID_STRUMENTO_STAZIONE == idStrumentoStazione && i.ID_GRANDEZZA_STAZIONE == idGrandezzaStazione).FirstOrDefault();
                if (sensoreStrumentoLetturaSulCampo == null)
                {
                    logger.Debug($"Sensore non presente");
                    // Il sensore non esiste, lo crea
                    Sensore2021 sensore = new Sensore2021
                    {
                        CODICE_IDENTIFICATIVO = ProssimoCodiceIdentificativoSensore(),
                        ID_STRUMENTO_STAZIONE = idStrumentoStazione,
                        ID_GRANDEZZA_STAZIONE = idGrandezzaStazione,
                        ULTIMO_AGGIORNAMENTO = DateTime.Now,
                        AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}"
                    };
                    context.Sensori2021.Add(sensore);
                    context.SaveChanges();
                    logger.Debug($"Sensore aggiunto: {sensore.CODICE_IDENTIFICATIVO}");
                }

                return true;

            }
        }

        public static string ProssimoCodiceIdentificativoSensore()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select " +
                    "max(to_number(substr(codice_identificativo,2))) ultimo_numero_codice " +
                    "from sias_sensori where codice_identificativo like 'M%' " +
                    "and " +
                    "trim(TRANSLATE(substr(codice_identificativo, 2), '0123456789', ' ')) is null";
                var risultato = context.Database.SqlQuery<decimal>(query).ToList();

                if (risultato == null)
                    return "M1";
                else
                    return $"M{risultato.FirstOrDefault() + 1}";
            }
        }

        public static List<string> ElencoGrandezzeStazioneInUso()
        {
            decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select distinct grandezza from sias_grandezze_stazione where id_grandezza_stazione in ( " +
                    "select id_grandezza_stazione from sias_sensori where id_strumento_stazione in ( " +
                    $"select id_strumento_stazione from sias_strumenti_stazione where id_tipo_strumento = {idTipoStrumentoLetturaSulCampo} " +
                    ") " +
                    ") " +
                    "order by grandezza";
                return context.Database.SqlQuery<string>(query).ToList();
            }
        }

        public static List<string> ElencoUnitaMisuraStazioneInUso()
        {
            decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "select distinct unita_misura from sias_grandezze_stazione where id_grandezza_stazione in ( " +
                    "select id_grandezza_stazione from sias_sensori where id_strumento_stazione in ( " +
                    $"select id_strumento_stazione from sias_strumenti_stazione where id_tipo_strumento = {idTipoStrumentoLetturaSulCampo} " +
                    ") " +
                    ") " +
                    "order by unita_misura";
                return context.Database.SqlQuery<string>(query).ToList();
            }
        }

        public static bool RimuoviGrandezzaStazione(decimal idStazione, string grandezza, string unitaMisura)
        {
            decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

            using (SIASSEntities context = new SIASSEntities())
            {
                logger.Debug($"Rimozione grandezza stazione grandezza {grandezza}, unitaMisura {unitaMisura} da idStazione {idStazione}");

                // Riceva idgrandezzastazione da rimuovere
                string query = "select  " +
                        "id_grandezza_stazione " +
                        "from sias_grandezze_stazione " +
                        $"where id_stazione = {idStazione} and grandezza = '{grandezza}' and unita_misura = '{unitaMisura}'";
                decimal idGrandezzaStazioneDaRimuovere = context.Database.SqlQuery<decimal>(query).ToList().FirstOrDefault();

                // Se in uso non può essere eliminata
                query = "select distinct " +
                        "id_grandezza_stazione " +
                        "from sias_misurazioni " +
                        $"where id_grandezza_stazione = {idGrandezzaStazioneDaRimuovere}";
                bool inUso = context.Database.SqlQuery<decimal>(query).Any();

                if (inUso)
                {
                    // Esistono misurazioni per la grandezza
                    logger.Debug("Esistono misurazioni per la grandezza");
                    return false;
                }

                // Se la grandezza è usata da altri sensori che non sono per la misurazione sul campo va lasciata e va eliminata solo il sensore per la misruazione sul campo
                query = $"select id_grandezza_stazione from sias_sensori where id_grandezza_stazione = {idGrandezzaStazioneDaRimuovere} and id_strumento_stazione in ( " +
                    $"select id_strumento_stazione from sias_strumenti_stazione where id_tipo_strumento <> {idGrandezzaStazioneDaRimuovere} " +
                    ")";
                bool inUsoDaAltriSensori = context.Database.SqlQuery<decimal>(query).Any();

                // Elimina eventuali sensori per misurazioni sul campo che usano la grandezza
                logger.Debug($"Eliminazione sensori per lettura sul campoche che usano idGrandezzaStazione = {idGrandezzaStazioneDaRimuovere}");
                context.Database.ExecuteSqlCommand($"delete from sias_sensori where id_grandezza_stazione = {idGrandezzaStazioneDaRimuovere} " +
                    "and id_strumento_stazione in ( " +
                    $"select id_strumento_stazione from sias_strumenti_stazione where id_tipo_strumento = {idTipoStrumentoLetturaSulCampo} " +
                    ")");
                context.SaveChanges();

                // Elimina la grandezza
                if (inUsoDaAltriSensori)
                {
                    logger.Debug("La grandezza non può essere eliminata perché in uso da altri sensori oltre alla misurazione sul campo");
                }
                else
                {
                    logger.Debug("La grandezza può essere eliminata perché non in uso da altri sensori oltre alla misurazione sul campo");
                    logger.Debug($"Eliminazione grandezza stazione idGrandezzaStazione = {idGrandezzaStazioneDaRimuovere}");
                    context.Database.ExecuteSqlCommand($"delete from sias_grandezze_stazione where id_grandezza_stazione = {idGrandezzaStazioneDaRimuovere}");
                    context.SaveChanges();
                }

                return true;

            }
        }

        #endregion

        #region PacchettiInterventi

        public static List<TipoPacchetto> ElencoTipiPacchetto()
        {
            using (SIASSEntities context = new SIASSEntities())
            {
                var elencoPacchetti = context.TipiPacchetto.
                    Where(i => i.DATA_FINE_VALIDITA == null || i.DATA_FINE_VALIDITA >= DateTime.Now).
                    OrderBy(i => i.ORDINE).
                    ThenBy(i => i.DESCRIZIONE_PACCHETTO);
                return elencoPacchetti.ToList();
            }
        }

        public static bool AggiungiPacchettoIntervento(decimal idStazione, decimal idPacchetto)
        {
			ConfigurazioneOperatoreSiass oper = OperatoreSiassManager.OperatoreCorrente();
            decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

            using (SIASSEntities context = new SIASSEntities())
            {
                logger.Debug($"Aggiunta pacchetto {idPacchetto} a IdStazione {idStazione}");

                // Verifica che il pacchetto non sia già assegnato al tipo strumento lettura sul campo
                decimal idStrumentoStazione;
                var strumentoLetturaSulCampo = context.StrumentiStazione.Where(i => i.ID_STAZIONE == idStazione && i.ID_TIPO_STRUMENTO == idTipoStrumentoLetturaSulCampo).FirstOrDefault();

                if (strumentoLetturaSulCampo == null)
                {
                    logger.Debug("Strumento lettura sul campo non presente");
                    // Lo strumento non esiste, lo inserisce
                    StrumentoStazione strumento = new StrumentoStazione
                    {
                        ID_STAZIONE = idStazione,
                        ID_TIPO_STRUMENTO = idTipoStrumentoLetturaSulCampo,
                        INIZIO_VALIDITA = DateTime.Now,
                        ULTIMO_AGGIORNAMENTO = DateTime.Now,
                        AUTORE_ULTIMO_AGGIORNAMENTO = $"{oper.Nome} {oper.Cognome}"
                    };
                    context.StrumentiStazione.Add(strumento);
                    context.SaveChanges();
                    idStrumentoStazione = strumento.ID_STRUMENTO_STAZIONE;
                    logger.Debug($"Strumento lettura sul campo aggiunto: idStrumentoStazione {idStrumentoStazione}");
                }
                else
                {
                    logger.Debug("Strumento lettura sul campo presente");
                    // Lo strumento esiste, legge id
                    idStrumentoStazione = strumentoLetturaSulCampo.ID_STRUMENTO_STAZIONE;
                    logger.Debug($"idStrumentoStazione {idStrumentoStazione}");
                }

                var pacchetto = context.PacchettiStrumenti.Where(i => i.ID_PACCHETTO == idPacchetto && i.ID_STRUMENTO_STAZIONE == idStrumentoStazione).FirstOrDefault();
                if (pacchetto != null)
                {
                    logger.Debug("Pacchetto già assegnato");
                    return false;
                }

                PacchettoStrumento p = new PacchettoStrumento
                {
                    ID_PACCHETTO = idPacchetto,
                    ID_STRUMENTO_STAZIONE = idStrumentoStazione
                };
                context.PacchettiStrumenti.Add(p);
                context.SaveChanges();
                logger.Debug("Pacchetto aggiunto");

                return true;

            }
        }

        public static List<TipoPacchetto> ElencoTipiPacchettoInUsoDaStrumentiLetturaSulCampo()
        {
            decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

            using (SIASSEntities context = new SIASSEntities())
            {
                string query = "SELECT " +
                    "id_pacchetto, " +
                    "descrizione_pacchetto, " +
                    "ordine, " +
                    "codice_alims, " +
                    "note_pacchetto, " +
                    "data_fine_validita " +
                    "FROM " +
                    "sias_tipi_pacchetto " +
                    "WHERE " +
                    "id_pacchetto in " +
                    "( " +
                    "select distinct id_pacchetto from sias_pacchetti_strumenti " +
                    "where ID_STRUMENTO_STAZIONE in " +
                    "( " +
                    $"select distinct id_strumento_stazione from SIAS_STRUMENTI_STAZIONE where ID_TIPO_STRUMENTO = {idTipoStrumentoLetturaSulCampo} " +
                    ") " +
                    ") " +
                    "ORDER BY " +
                    "ordine, " +
                    "descrizione_pacchetto";
                return context.Database.SqlQuery<TipoPacchetto>(query).ToList();
            }
        }

        public static bool RimuoviPacchettoIntervento(decimal idStazione, decimal idPacchetto)
        {
            decimal idTipoStrumentoLetturaSulCampo = decimal.Parse(ConfigurationManager.AppSettings["IdTipoStrumentoLetturaSulCampo"]);

            using (SIASSEntities context = new SIASSEntities())
            {
                logger.Debug($"Rimozione pacchetto intervento {idPacchetto} da idStazione {idStazione}");

                // Se in uso in interventi non può essere eliminato
                string query = $"select id_pacchetto from sias_pacchetti_interventi where id_pacchetto = {idPacchetto} and id_intervento in " +
                    "( " +
                    $"select id_intervento from sias_interventi where id_stazione = {idStazione} " +
                    ")";
                bool inUso = context.Database.SqlQuery<decimal>(query).Any();

                if (inUso)
                {
                    // Esistono misurazioni per la grandezza
                    logger.Debug("Esistono interventi che usano il pacchetto");
                    return false;
                }

                context.Database.ExecuteSqlCommand($"delete from sias_pacchetti_strumenti where id_pacchetto = {idPacchetto} and id_strumento_stazione in " +
                    "( " +
                    $"select id_strumento_stazione from sias_strumenti_stazione where id_stazione = {idStazione} and id_tipo_strumento = {idTipoStrumentoLetturaSulCampo} " +
                    ")");
                context.SaveChanges();

                return true;

            }
        }

        #endregion
    }
}
