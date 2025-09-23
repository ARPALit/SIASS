SIASS

Sommario

[1\. Dipendenze 3](#_Toc204850474)

[2\. Installazione ambiente di sviluppo 3](#_Toc204850475)

[3\. Compilazione software 3](#_Toc204850476)

[4\. Installazione software 4](#_Toc204850477)

[Creazione database 4](#_Toc204850478)

[Applicazione Web 4](#_Toc204850479)

[Applicazioni background 4](#_Toc204850480)

[Descrizione file di configurazione 4](#_Toc204850481)

[Procedura di verifica dell’installazione 6](#_Toc204850482)

[5\. Descrizione architettura 7](#_Toc204850483)

[Applicazione Web 7](#_Toc204850484)

[Applicazione background per l’importazione di dati di misurazioni 7](#_Toc204850485)

[Applicazione background per l’importazione di dati sugli organoclorurati 8](#_Toc204850486)

[Applicazione background anomalie 8](#_Toc204850487)

[Base dati Oracle 8](#_Toc204850488)

[6\. Modello dati 8](#_Toc204850489)

[ALIMS_DATI 8](#_Toc204850490)

[ALIMS_PACCHETTI_CONTENITORI 8](#_Toc204850491)

[MV_ULTIMA_MISURAZIONE 9](#_Toc204850492)

[SIAS_ALLEGATI_INTERVENTI 9](#_Toc204850493)

[SIAS_ANALITI_INTERVENTI 9](#_Toc204850494)

[SIAS_ANOMALIE 10](#_Toc204850495)

[SIAS_CONTROLLI_ANOMALIE 10](#_Toc204850496)

[SIAS_FINALITA_STAZIONI 10](#_Toc204850497)

[SIAS_GRANDEZZE_STAZIONE 10](#_Toc204850498)

[SIAS_IMPORT_ORGANOCLORURATI 10](#_Toc204850499)

[SIAS_INTERVENTI 11](#_Toc204850500)

[SIAS_MISURAZIONI 12](#_Toc204850501)

[SIAS_OPERATORI_INTERV_SUP 12](#_Toc204850502)

[SIAS_OPERATORI_INTERVENTI 12](#_Toc204850503)

[SIAS_ORGANOCLORURATI 12](#_Toc204850504)

[SIAS_PACCHETTI_INTERVENTI 13](#_Toc204850505)

[SIAS_PACCHETTI_INTERVENTI_V1 13](#_Toc204850506)

[SIAS_PACCHETTI_STRUMENTI 13](#_Toc204850507)

[SIAS_PAR_CAMPO_INTERVENTI 13](#_Toc204850508)

[SIAS_PROFILI 14](#_Toc204850509)

[SIAS_SENSORI 14](#_Toc204850510)

[SIAS_SITI 14](#_Toc204850511)

[SIAS_STAZIONI_RETI 14](#_Toc204850512)

[SIAS_STRUMENTI_STAZIONI 15](#_Toc204850513)

[SIAS_TIPI_ALLESTIMENTO 15](#_Toc204850514)

[SIAS_TIPI_ARGOMENTO 15](#_Toc204850515)

[SIAS_TIPI_ESPRES_RISULTATO 15](#_Toc204850516)

[SIAS_TIPI_FINALITA_STAZ 15](#_Toc204850517)

[SIAS_TIPI_GRANDEZZA 16](#_Toc204850518)

[SIAS_TIPI_INTERVENTO 16](#_Toc204850519)

[SIAS_TIPI_METODO 16](#_Toc204850520)

[SIAS_TIPI_PACCHETTO 16](#_Toc204850521)

[SIAS_TIPI_RETE_APPARTENENZA 16](#_Toc204850522)

[SIAS_TIPI_RICHIEDENTE 17](#_Toc204850523)

[SIAS_TIPI_SEDE_ACCETTAZIONE 17](#_Toc204850524)

[SIAS_TIPI_STRUMENTO 17](#_Toc204850525)

[SIAS_TIPI_STRUMENTO_INTERVENTO 17](#_Toc204850526)

[SIAS_TIPI_UNITA_MISURA 17](#_Toc204850527)

[SIASS_ALLEGATI_STAZIONI 17](#_Toc204850528)

[SIASS_CARATT_INSTALLAZIONI 18](#_Toc204850529)

[SIASS_CARATT_TECNICHE_POZZI 18](#_Toc204850530)

[SIASS_COMUNI 19](#_Toc204850531)

[SIASS_CORPI_IDRICI 19](#_Toc204850532)

[SIASS_DATI_AMMINISTRATIVI 19](#_Toc204850533)

[SIASS_LOCALIZZAZIONI 19](#_Toc204850534)

[SIASS_PROVINCE 20](#_Toc204850535)

[SIASS_STAZIONI 20](#_Toc204850536)

[SIASS_TIPI_ACCESSO 20](#_Toc204850537)

[SIASS_TIPI_CHIUSURA 20](#_Toc204850538)

[SIASS_TIPI_DESTINAZIONE_USO 21](#_Toc204850539)

[SIASS_TIPI_FISSAGGIO_TRASM 21](#_Toc204850540)

[SIASS_TIPI_FREQUENZA_UTILIZZO 21](#_Toc204850541)

[SIASS_TIPI_STAZIONE 21](#_Toc204850542)

[7\. Descrizione API applicazione Web 21](#_Toc204850543)

[8\. Dati da servizi esterni 22](#_Toc204850544)

[Servizio anagrafiche esterne 23](#_Toc204850545)

[Web service esterni 25](#_Toc204850546)

## 1\. Dipendenze

L’applicazione è composta da più componenti, che richiedono:

Server:

- Sistema operativo Microsoft Windows Server 2012 R2 o successivo
- Microsoft IIS versione 8.5 o successiva
- Microsoft .NET 8, ASP.NET 4.8
- Oracle Database v.11g R2 o successiva
- API per autenticazione e dati di servizio

Client:

- Browser Web
- Accesso con reverse proxy con autenticazione

## 2\. Installazione ambiente di sviluppo

L'ambiente di sviluppo è Microsoft Visual Studio 2022, da installare con strumenti di sviluppo C# e ASP.NET.

<https://visualstudio.microsoft.com/it/>

E' possibile utilizzare anche Microsoft Visual Studio Code

<https://code.visualstudio.com/>

E' richiesta l'installazione di Oracle Developer Tools per Visual Studio

<https://www.oracle.com/database/technologies/developer-tools/visual-studio/>

## 3\. Compilazione software

Il software comprende tre progetti di Visual Studio:

- applicazione Web per gli operatori
- applicazione background per l’importazione di dati di misurazioni
- applicazione background per l’importazione di dati sugli organoclorurati, da file caricati tramite applicazione Web
- applicazione background anomalie

I progetti sono compilabili per il rilascio mediante la funzione di Visual Studio (Menu Compilazione->Pubblica).

## 4\. Installazione software

### Creazione database

Creare uno schema Oracle ed eseguire gli script forniti.

### Applicazione Web

Copiare la versione compilata in una cartella sul server Web.

Da interfaccia di IIS creare un application pool con .NET 4.0.

Creare un sito Web che utilizza l'application pool al punto precedente e che punta alla relativa cartella sul server Web.

Configurare il sito per utilizzare una connessione protetta in https con certificato SSL.

### Applicazioni background

Copiare la versione compilata di ogni applicazione in una cartella sul server Web.

Schedulare i processi tramite task scheduler di Windows.

L’account che esegue l’applicazione deve poter accedere in scrittura alle cartella “Log” e alla cartella “File” di importazione file, allegati e verbali interventi.

### Descrizione file di configurazione

#### Applicazione Web

La configurazione è contenuta nel file Web.Config

| **Impostazione** | **Descrizione** |
| --- | --- |
| SIASSEntities | Nella sezione connectionStrings; contiene la stringa di connessione alla base dati Oracle. |
| Endpoint GsoWebServicesSoap | Contiene l’URL a cui risponde il Web Service di autenticazione |
| Endpoint AnsoloWebServiceSoap | Contiene l’URL a cui risponde il Web Service di anagrafiche |
| MostraErroreDettagliato | Indica se visualizzare in interfaccia i dettagli di eventuali errori dell’applicazione |
| IdentificativoApplicativo | Nome dell’applicazione configurato sul servizio di autenticazione |
| URLUscita | URL di uscita dall’applicazione |
| RichiestaCredenziali | Consente di simulare accedere come un operatore indipendentemente dall’autenticazione (utilizzato solo per sviluppo) |
| DimensioneMassimaFileDaCaricare | Dimensione massima in byte di file di cui viene fatto l’upload tramite interfaccia (allegati, file di importazione) |
| GoogleAPIKey | API Key di Google utilizzata per disegnare la mappa delle stazioni |
| GiorniDataInizioFiltroMisurazioni | Numero di giorni per l'intervallo di default dei filtri delle date delle misurazioni nella funzione di visualizzazione |
| NumeroMassimoMisurazioni | Numero massimo di misurazioni da mostrare nella funzione di visualizzazione |
| SeparatoreCampiCSV | SeparatoreCampiCSV utilizzato nell'esportazione delle misurazioni |
| SeparatoreDecimale | SeparatoreDecimale utilizzato nell'esportazione delle misurazioni |
| ProfiliGSOOperatoriInterventi | Profili del servizio di autenticazione per operatori interventi (separati da ";"): se l'operatore ha uno di questi profili può essere selezionato come operatore di un intervento |
| IdTipoStrumentoLetturaSulCampo | Id del tipo strumento (tabella SIAS_TIPI_STRUMENTO) che viene utilizzato negli interventi come "Lettura sul campo" |
| NomeReteOrganoclorurati | Nome rete organoclorurati usato per generare il verbale specifico; deve coincidere con il valore della tabella SIAS_TIPI_RETE_APPARTENENZA e con il nome del file del report nella cartella rdlc\\VerbaliIntervento |
| NomeGrandezzaSpurgo | Nome grandezza spurgo usato per generare il dato di presenza spurgo per il verbale organoclorurati; deve coincidere con il valore della tabella SIAS_TIPI_GRANDEZZA |
| TemplateURLAPIAutenticazione | Template URL API Autenticazione |
| NomiHeaderCodiceFiscale | Nome header che contiene il codice fiscale |
| URLDatasetReteMonitoraggio | URL Datset struttura della rete di monitoraggio |
| URLDatasetStoricoMisurazioni | URL Dataset dello storico misurazioni per stazione |
| URLPrivacy | URL della pagina Web esterna contenente eventuali informazioni sulla privacy (opzionale) |
| CartellaVerbaliInterventi | Cartella che contiene i file dei verbali interventi |
| ApiKeyEsportazioneVerbaliInterventi | API key endpoint verbali interventi |
| ApiAnagraficheBaseUrl | URL di base delle API anagrafiche |
| ApiAnagraficheApiKey | API key delle API anagrafiche |
| ApiAnagraficheConfigurazioneOperatore | Endpoint configurazione operatore delle API anagrafiche |
| ApiAnagraficheAnaliti | Endpoint analiti delle API anagrafiche |
| ApiAnagrafichePacchetti | Endpoint pacchetti delle API anagrafiche |
| ApiAnagraficheContenitori | Endpoint contenitori delle API anagrafiche |

#### Applicazione background per l’importazione di dati di misurazioni (SIASSImport)

La configurazione è contenuta nel file app.config

| **Impostazione** | **Descrizione** |
| --- | --- |
| CartellaImportazione | Cartella contenente i file CSV di misurazioni da importare |
| CartellaArchivio | Cartella in cui vengono archiviati i file importati |
| SeIntestazioneCSV | Indica se i file CSV contengono una riga di intestazione colonne |
| CarattereDelimitatoreCampiCSV |     |
| CarattereCommentiCSV |     |
| SeparatoreDecimaliMisurazioneCSV |     |
| SeInviareEmailFineEsportazione | Indica se viene inviato un messaggio di posta elettronica contenente i risultati della singola importazione |
| SeInviareEmailErrore | Come sopra, ma solo in caso di errore |
| DestinatariEmail | Destinatari del messaggio |
| ServerSMTP |     |
| PortaServerSMTP |     |
| SeAbilitareSSLPerSMTPServer |     |
| NomeUtenteSMTP |     |
| PasswordUtenteSMTP |     |
| FormatoDataCSV | Formato delle date nel file CSV |
| StringaConnessioneDB | Stringa di connessione alla base dati Oracle |
| IndirizzoMittenteEmail |     |
| SeCampiCSVRacchiusiTraVirgolette |     |
| SoloVerificaDati | Indica se effettuare l’importazione oppure limitarsi a verificare i dati del file da importare |

### Procedura di verifica dell’installazione

Per testare l’installazione, inserire nell’ordine:

- un sito (direttamente su base dati nella tabella SIAS_SITI)
- una stazione
- dati relativi alla stazione (dati amministrativi, caratteristiche tecniche, localizzazione, caratteristiche dell’installazione, strumenti, grandezze e misurazioni)
- un intervento

Separatamente verificare che:

- l’applicazione Web sia raggiungibile tramite browser;
- la base dati sia raggiungibile dall’applicazione Web e dalle applicazioni background;
- I servizi esterni (autenticazione, anagrafiche) siano raggiungibili dall’applicazione Web.

I log degli errori delle applicazioni sono archiviati nella sottocartella Log. Il servizio usato per la scrittura e archiviazione dei log è NLog le cui impostazioni sono contenute nei file di configurazione delle singole applicazioni.

#### Scenari di test

Utilizzare l’applicazione Web.

##### 1\. Login con autenticazione Windows

- Accedere alla pagina /Login/Login.aspx
- Effettuare il login con autenticazione integrata Windows.
- Selezionare un profilo (in caso l’operatore disponga di più profili)
- Verificare che si venga reindirizzati alla pagina principale dell’applicazione contenente l’elenco delle stazioni e che in alto a destra compaia il nominativo dell’operatore.

##### 2\. Login con autenticazione esterna

- Accedere alla pagina /AutenticazionePortale.aspx
- Verificare che il sistema esterno autentichi l’operatore.
- Selezionare un profilo (in caso l’operatore disponga di più profili).
- Verificare che si venga reindirizzati alla pagina principale dell’applicazione contenente l’elenco delle stazioni e che in alto a destra compaia il nominativo dell’operatore.

##### 3\. Elenco stazioni

- Fare clic su “Cerca” e verificare che venga mostrato l’elenco dei risultati; se non sono state inserite stazioni l’elenco è vuoto.

##### 4\. Nuova stazione

- Inserire un nuovo sito (direttamente su database).
- Inserire una nuova stazione.
- Dopo l’inserimento verificare che venga visualizzata la pagina di riepilogo della stazione.
- Accedere alle sezioni “Dati amministrativi”, “Caratteristiche tecniche”, “Localizzazione”, “Caratteristiche dell’installazione” ed effettuare un inserimento per ciascuna.

##### 5\. Strumenti e sensori

Inserire la configurazione di strumenti e sensori per una stazione.

##### 6\. Inserimento di un intervento

Inserire e modificare un intervento, con e senza prelievo di campioni, e relativi analiti e misurazioni.

##### 7\. Importazione misurazioni

- Importare tramite l’applicazione background i dati di misurazioni da un file relativamente alla configurazione di strumenti e sensori impostata per la stazione.
- Visualizzare le misurazioni importate.

##### 8\. Importazione organoclorurati

- Importare un file tramite applicazione Web e quindi attendere che l’applicazione in background schedulata lo elabori.
- Visualizzare il risultato dell’importazione.

## 5\. Descrizione architettura

L'architettura del sistema si basa su una struttura che integra diverse componenti front-end, back-end e di elaborazione in background per gestire stazioni e relativi interventi e misurazioni, la generazione dei PDF dei verbali di intervento, la sincronizzazione dei dati e l'integrazione con sistemi esterni.

### Applicazione Web

Interfaccia utente per l'inserimento, modifica, completamento e consultazione di stazioni, interventi e misurazioni.

L'applicazione ottiene il profilo utente con le relative autorizzazioni tramite API.

E’ accessibile agli operatori con un browser.

Comunicazione: espone tramite API i dati degli interventi e relative misurazioni.

### Applicazione background per l’importazione di dati di misurazioni

Processo schedulato che si occupa dell’inserimento di dati delle misurazioni senza intervento manuale, leggendo i dati da file CSV presenti in una cartella definita in configurazione e importandoli su base dati in base alla configurazione di strumenti e sensori di una stazione.

### Applicazione background per l’importazione di dati sugli organoclorurati

Processo schedulato che si occupa dell’inserimento di dati degli organoclorurati senza intervento manuale, leggendo i dati da file Excel presenti in una cartella definita in configurazione che possono essere caricati dagli operatori utilizzando l’applicazione Web.

### Applicazione background anomalie

Processo schedulato che si occupa dell’inserimento di segnalazione di anomalie nelle misurazioni (come assenza di dati in un periodo, picchi, ecc.) in una tabella della base dati in base ai controlli implementati e alla configurazione specificata.

Base dati Oracle

Archivio centrale per i dati strutturati di stazioni e misurazioni.

Servizio anagrafiche esterne

Sorgente di dati esterna all’applicazione utilizzata dall’applicazione tramite API RESTful. Fornisce endpoint per: dati anagrafici per compilazione (matrici, analiti, ecc.), profili operatori.

6\. Modello dati

Di seguito le principali tabelle contenute nella base dati.

### ALIMS_DATI

Può essere realizzata come vista su un sistema esterno.

| **Campo** | **Descrizione** |
| --- | --- |
| DATA_CAMPIONAMENTO |     |
| STAZIONE | Codice della stazione |
| PARAMETRO |     |
| UNITA_MISURA |     |
| VALORE |     |

### ALIMS_PACCHETTI_CONTENITORI

Può essere realizzata come vista su un sistema esterno.

| **Campo** | **Descrizione** |
| --- | --- |
| SIGLA_PACCHETTO |     |
| NOME_PACCHETTO |     |
| PARAMETRO |     |
| TIPO_CONTENITORE |     |
| NUMERO_CONTENITORI |     |
| METODO |     |
| UDM |     |
| LDQ |     |
| MATRICE |     |
| LABORATORIO_ANALISI |     |
| REPARTO_LINEA |     |
| FRAZIONE |     |
| LIMITE_INDIC |     |
| LEGGE_INDIC |     |
| LIMITE_MINIMO |     |
| LIMITE_MASSIMO |     |
| PMC |     |

### MV_ULTIMA_MISURAZIONE

Generata dall’omonima vista materializzata.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_STAZIONE |     |
| ID_GRANDEZZA_STAZIONE | Identificativo della grandezza e unità di misura configurate per la stazione |
| ID_MISURAZIONE |     |
| DATA_MISURAZIONE |     |
| VALIDATA |     |
| GRANDEZZA |     |

### SIAS_ALLEGATI_INTERVENTI

Dati dei file allegati agli interventi.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_ALLEGATO_INTERVENTO |     |
| ID_INTERVENTO |     |
| NOME_FILE_ALLEGATO |     |
| DESCRIZIONE_ALLEGATO |     |
| DATA_ORA_INSERIMENTO |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |

### SIAS_ANALITI_INTERVENTI

Analiti specificati per intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_ANALITA_INTERVENTO |     |
| ID_INTERVENTO |     |
| CODICE |     |
| DESCRIZIONE |     |
| METODO_CODICE |     |
| METODO_DESCRIZIONE |     |
| PACCHETTO_CODICE |     |
| PACCHETTO_DESCRIZIONE |     |
| RIMOSSO_DA_OPER | Indica se è stato rimosso dall’operatore in fase di inserimento |
| AGGIUNTO_DA_OPER | Indica se è stato aggiunto dall’operatore in fase di inserimento |
| VALORE_LIMITE |     |
| UNITA_MISURA |     |
| LINEA_LAV |     |

### SIAS_ANOMALIE

Anomalie rilevate tramite applicazione background.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_ANOMALIA |     |
| ID_STAZIONE |     |
| DATA_CONTROLLO |     |
| VALORE |     |
| DATA_VALORE |     |
| ID_CONTROLLO |     |
| VISUALIZZATO | Utilizzato dall’interfaccia esterna di visualizzazione |

### SIAS_CONTROLLI_ANOMALIE

Configurazione dei controlli per il rilevamento di anomalie nelle misurazioni tramite applicazione background. Vedere per i dettagli il documento descrittivo dell’applicazione ControlliAnomalie.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_CONTROLLO |     |
| CODICE_FUNZIONE |     |
| NOME_CONTROLLO |     |
| DESCRIZIONE_CONTROLLO |     |
| CONFIGURAZIONE_JSON |     |
| RETE |     |
| ID_TIPO_STAZIONE |     |
| GRANDEZZA |     |
| NOME_UNITA_MISURA |     |
| ABILITATO |     |
| DATA_ULTIMA_ESECUZIONE |     |
| ESITO_ULTIMA_ESECUZIONE |     |

### SIAS_FINALITA_STAZIONI

Finalità di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_FINALITA_STAZIONE |     |
| ID_STAZIONE |     |
| FINALITA |     |

### SIAS_GRANDEZZE_STAZIONE

Grandezze e unità di misura rilevate da una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_GRANDEZZA_STAZIONE |     |
| ID_STAZIONE |     |
| GRANDEZZA | Grandezza da tipi grandezze |
| UNITA_MISURA | Unità di misura da tipi unità misura |
| NUMERO_DECIMALI |     |

### SIAS_IMPORT_ORGANOCLORURATI

Dati relativi ai file utilizzati per l’importazione degli organoclorurati.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_IMPORTAZIONE |     |
| OPERATORE |     |
| NOME_FILE |     |
| DATA_IMPORTAZIONE |     |
| STATO |     |
| DATA_RICEZIONE_FILE |     |
| PIVA_OPERATORE | Partita IVA dell’operatore che ha effettuato l’upload del file da interfaccia |

### SIAS_INTERVENTI

Interventi.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_INTERVENTO |     |
| ID_STAZIONE |     |
| ID_TIPO_INTERVENTO |     |
| ID_TIPO_RICHIEDENTE |     |
| DATA_INTERVENTO |     |
| DURATA_INTERVENTO | Durata dell’intervento in minuti |
| CODICE_CAMPAGNA |     |
| FILE_DATI |     |
| PORTATA_SORGENTE |     |
| SPURGO |     |
| VOLUME_COLONNA_ACQUA |     |
| PORTATA_SPURGO |     |
| DURATA_SPURGO |     |
| FILE_ANGOLI |     |
| ID_STRUMENTO |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| ORA_INTERVENTO |     |
| NUMERO_CAMPIONI |     |
| ANNOTAZIONI |     |
| PARTE_NOME_TECNICO |     |
| PARTE_AZIENDA_TECNICO |     |
| PARTE_RUOLO_TECNICO |     |
| PARTE_CONTATTI |     |
| ORA_FINE_INTERVENTO |     |
| QUOTA_CAMPIONE |     |
| ANNOTAZIONI_PACCHETTI |     |
| CAMPIONE_BIANCO |     |
| MOD_VERBALE_SELEZIONATO |     |
| SIGLA_VERBALE |     |
| CODICE_SEDE_ACCETTAZIONE |     |
| PRELIEVO_CAMPIONI |     |
| CREAZIONE_VERBALE |     |
| DATI_CAMPIONE_BIANCO |     |
| CODICE_MATRICE |     |
| DESCRIZIONE_MATRICE |     |
| CODICE_ARGOMENTO |     |
| DESCRIZIONE_ARGOMENTO |     |
| ORGANIZZAZIONE_CREAZIONE |     |
| STATO_INVIO_VERBALE_V1 | Compatibilità con versione precedente dell’applicazione |
| VERSIONE | Versione del modello dati dell’intervento |

### SIAS_MISURAZIONI

Misurazioni.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_MISURAZIONE |     |
| DATA_MISURAZIONE |     |
| VALORE |     |
| VALIDATA |     |
| ID_GRANDEZZA_STAZIONE |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| ID_INTERVENTO |     |
| CODICE_IDENTIFICATIVO_SENSORE |     |
| VALORE_SENSORE | Valore originario misurato dal sensore |
| FONTE_ARPAL | Indica se la misurazione è stata effettuata da ARPAL |
| PACCHETTO_CODICE |     |

### SIAS_OPERATORI_INTERV_SUP

Operatori a supporto di un intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_OPERATORE_INTERV_SUP |     |
| ID_OPERATORE | Identificativo dell’operatore su sistema esterno |
| ID_INTERVENTO |     |
| DESCRIZIONE_OPERATORE |     |

### SIAS_OPERATORI_INTERVENTI

Operatori di un intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_OPERATORE_INTERVENTO |     |
| ID_OPERATORE | Identificativo dell’operatore su sistema esterno |
| ID_INTERVENTO |     |
| DESCRIZIONE_OPERATORE |     |

### SIAS_ORGANOCLORURATI

Misurazioni di organoclorurati.

| **Campo** | **Descrizione** |
| --- | --- |
| SITO |     |
| PIEZOMETRO |     |
| DATA_CAMPIONAMENTO |     |
| FONTE |     |
| PARAMETRO |     |
| FAMIGLIA |     |
| CONC_SIGN |     |
| CONC_VAL |     |
| LOQ |     |
| INCERTEZZA |     |
| CONC_CSC |     |
| CONC_UDM |     |
| ID_IMPORTAZIONE |     |
| DATA_AGGIORNAMENTO |     |
| ID_MISURAZIONE |     |

### SIAS_PACCHETTI_INTERVENTI

Pacchetti per intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_PACCHETTO_INTERVENTI |     |
| ID_INTERVENTO |     |
| COD_ARGOMENTO |     |
| CODICE |     |
| DESCRIZIONE |     |
| SEDE |     |

### SIAS_PACCHETTI_INTERVENTI_V1

Pacchetti per intervento mantenuto per compatibilità con versione precedente dell’applicazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_PACCHETTO_INTERVENTO |     |
| ID_PACCHETTO |     |
| ID_INTERVENTO |     |

### SIAS_PACCHETTI_STRUMENTI

Pacchetti per strumento della stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_PACCHETTO_STRUMENTO |     |
| ID_PACCHETTO |     |
| ID_STRUMENTO_STAZIONE |     |

### SIAS_PAR_CAMPO_INTERVENTI

Parametri da campo per intervento, indipendentemente dalle misurazioni effettuate.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_PAR_CAMPO_INTERVENTO |     |
| ID_INTERVENTO |     |
| ID_GRANDEZZA_STAZIONE |     |
| CODICE |     |
| DESCRIZIONE |     |
| METODO_CODICE |     |
| PACCHETTO_CODICE |     |

### SIAS_PROFILI

Definizione dei profili operatore.

| **Campo** | **Descrizione** |
| --- | --- |
| VISUALIZZAZIONE |     |
| GESTIONE |     |
| GESTIONE_RETE_ORGANOCLORURATI |     |
| GESTIONE_RETE_FREATIMETRICA |     |
| AMMINISTRATORE |     |
| GESTORE_DITTA |     |
| CONSULTAZIONEDATASET |     |

### SIAS_SENSORI

Sensori di uno strumento.

| **Campo** | **Descrizione** |
| --- | --- |
| CODICE_IDENTIFICATIVO |     |
| ID_STRUMENTO_STAZIONE |     |
| ID_GRANDEZZA_STAZIONE |     |
| UNITA_MISURA |     |
| ID_TIPO_ESPRESS_RISULTATO |     |
| FREQUENZA_ACQUISIZIONE |     |
| CODICE_PMC |     |
| ID_TIPO_METODO |     |
| COEFF_CONVER_UNITA_MISURA |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| CODICE_IDENTIFICATIVO_CREAZ | Codice identificativo originale al primo inserimento |

### SIAS_SITI

Siti a cui appartengono le stazioni.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_SITO |     |
| CODICE_IDENTIFICATIVO |     |
| DESCRIZIONE |     |
| LONGITUDINE |     |
| LATITUDINE |     |
| LONGITUDINE_GAUSS_BOAGA |     |
| LATITUDINE_GAUSS_BOAGA |     |
| QUOTA_PIANO_CAMPAGNA |     |
| CODICE_COMUNE | Codice ISTAT del comune |
| SUPERFICIE |     |
| CODICE_IFFI |     |
| INDIRIZZO |     |

### SIAS_STAZIONI_RETI

Reti a cui appartengono le stazioni.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_STAZIONE_RETE |     |
| ID_STAZIONE |     |
| RETE |     |

### SIAS_STRUMENTI_STAZIONI

Strumenti di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_STRUMENTO_STAZIONE |     |
| ID_TIPO_STRUMENTO |     |
| NUMERO_DI_SERIE |     |
| MARCA |     |
| MODELLO |     |
| NUMERO_INVENTARIO_ARPAL |     |
| CARATTERISTICHE |     |
| CODICE_SISTEMA_GESTIONALE |     |
| INIZIO_VALIDITA |     |
| FINE_VALIDITA |     |
| ID_STAZIONE |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |

### SIAS_TIPI_ALLESTIMENTO

Tipi di allestimento di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| DESCRIZIONE |     |
| ORDINE | Ordine di visualizzazione in interfaccia |

### SIAS_TIPI_ARGOMENTO

Argomento di un intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_ARGOMENTO |     |
| DESCRIZIONE_TIPO_ARGOMENTO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| CODICE_ALIMS |     |

### SIAS_TIPI_ESPRES_RISULTATO

Tipi di espressione di risultato di un sensore.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_ESPRESS_RISULTATO |     |
| DESCR_TIPO_ESPRESS_RISULTATO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |

### SIAS_TIPI_FINALITA_STAZ

Tipi di finalità di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| DESCRIZIONE |     |
| ORDINE | Ordine di visualizzazione in interfaccia |

### SIAS_TIPI_GRANDEZZA

Tipi di grandezza.

| **Campo** | **Descrizione** |
| --- | --- |
| NOME_GRANDEZZA |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| PMC_USUALE |     |
| BOOLEANA | Indica se la grandezza è di tipo booleano |
| ANALITA_IDENTITY | Codice sistema esterno |

### SIAS_TIPI_INTERVENTO

Tipi di intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_INTERVENTO |     |
| DESCRIZIONE_TIPO_INTERVENTO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| CON_PRELIEVO_CAMPIONI | Indica se il tipo è utilizzabile da un intervento con prelievo di campioni |
| SENZA_PRELIEVO_CAMPIONI | Indica se il tipo è utilizzabile da un intervento senza prelievo di campioni |

### SIAS_TIPI_METODO

Tipi di metodo di un sensore.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_METODO |     |
| DESCRIZIONE_METODO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |

### SIAS_TIPI_PACCHETTO

Tipi di pacchetto per uno strumento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_PACCHETTO |     |
| DESCRIZIONE_PACCHETTO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| CODICE_ALIMS | Codice sistema esterno |
| NOTE_PACCHETTO |     |
| DATA_FINE_VALIDITA |     |

### SIAS_TIPI_RETE_APPARTENENZA

Tipi di rete di appartenenza di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| DESCRIZIONE |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| SIPRA | Codice sistema esterno |

### SIAS_TIPI_RICHIEDENTE

Tipi di richiedente di un intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_RICHIEDENTE |     |
| DESCRIZIONE_TIPO_RICHIEDENTE |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| CODICE_ALIMS | Codice sistema esterno |

### SIAS_TIPI_SEDE_ACCETTAZIONE

Sedi di accettazione degli interventi.

| **Campo** | **Descrizione** |
| --- | --- |
| CODICE_SEDE |     |
| DENOMINAZIONE_SEDE |     |

### SIAS_TIPI_STRUMENTO

Tipi di strumento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_STRUMENTO |     |
| DESCRIZIONE_TIPO_STRUMENTO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| VISIBILE_INTERVENTO | Indica se la configurazione dello strumento viene utilizzata per le letture sul campo dell’intervento |

### SIAS_TIPI_STRUMENTO_INTERVENTO

Tipi di strumento per intervento.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_STRUMENTO |     |
| DESCRIZIONE_STRUMENTO |     |
| ORDINE | Ordine di visualizzazione in interfaccia |

### SIAS_TIPI_UNITA_MISURA

Tipi di unità di misura.

| **Campo** | **Descrizione** |
| --- | --- |
| NOME_UNITA_MISURA |     |
| ORDINE | Ordine di visualizzazione in interfaccia |
| SE_BOOLEANO | Indica se l’unità di misura è di tipo booleano (esempio: “presenza”) |

### SIASS_ALLEGATI_STAZIONI

Dati dei file allegati alle stazioni.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_ALLEGATO_STAZIONE |     |
| ID_STAZIONE |     |
| NOME_FILE_ALLEGATO |     |
| DESCRIZIONE_ALLEGATO |     |
| DATA_ORA_INSERIMENTO |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |

### SIASS_CARATT_INSTALLAZIONI

Caratteristiche dell’installazione di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_CARATT_INSTALLAZIONE |     |
| ID_STAZIONE |     |
| ID_TIPO_FISSAGGIO_TRASM |     |
| CAVO_ESTERNO_IN_GUAINA |     |
| CAVO_SOTTOTRACCIA |     |
| PROTEZIONE_AREA |     |
| ID_TIPO_ACCESSO |     |
| INZIO_VALIDITA |     |
| FINE_VALIDITA |     |
| OSSERVAZIONI |     |
| SICUREZZA |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| PROFONDITA_SENSORE |     |

### SIASS_CARATT_TECNICHE_POZZI

Caratteristiche tecniche dei pozzi di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_CARATT_TECNICHE_POZZO |     |
| ID_STAZIONE |     |
| PROFONDITA |     |
| DIAMETRO |     |
| RANGE_SOGGIACENZA_DA |     |
| RANGE_SOGGIACENZA_A |     |
| ID_TIPO_CHIUSURA |     |
| QUOTA_BOCCAPOZZO_PC |     |
| PROFONDITA_EMUNGIMENTO |     |
| PORTATA_MASSIMA_ESERCIZIO |     |
| PRESENZA_FORO_SONDA |     |
| ID_TIPO_DESTINAZIONE_USO |     |
| ID_TIPO_FREQUENZA_UTILIZZO |     |
| INIZIO_VALIDITA |     |
| FINE_VALIDITA |     |
| CAPTATA |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| QUOTA_BOCCAPOZZO_SLM |     |
| QUOTA_PIANO_RIFERIMENTO_SLM |     |
| DIFFERENZA_PRPC |     |

### SIASS_COMUNI

Comuni.

| **Campo** | **Descrizione** |
| --- | --- |
| CODICE_COMUNE |     |
| DENOMINAZIONE_COMUNE |     |
| CODICE_PROVINCIA |     |

### SIASS_CORPI_IDRICI

Corpi idrici di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_CORPO_IDRICO |     |
| DESCRIZIONE_CORPO_IDRICO |     |
| CODICE_EUROPEO |     |
| CODICE_REGIONALE |     |

### SIASS_DATI_AMMINISTRATIVI

Dati amministrativi di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_DATI_AMMINISTRATIVI |     |
| ID_STAZIONE |     |
| GESTORE |     |
| INDIRIZZO_GESTORE |     |
| TELEFONO_GESTORE |     |
| RIFERIMENTO_GESTORE |     |
| INIZIO_VALIDITA |     |
| FINE_VALIDITA |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| PIVA_GESTORE | Partita IVA del gestore che determina la possibilità di accesso da parte di operatori appartenenti ad azienda con medesima partita IVA |

### SIASS_LOCALIZZAZIONI

Localizzazioni di una stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_LOCALIZZAZIONE |     |
| ID_STAZIONE |     |
| CODICE_COMUNE |     |
| LOCALITA |     |
| ID_BACINO |     |
| ID_CORPO_IDRICO |     |
| CTR |     |
| LONGITUDINE | Utilizzata per visualizzazione su mappa in interfaccia |
| LATITUDINE | Utilizzata per visualizzazione su mappa in interfaccia |
| LONGITUDINE_GAUS_BOAGA |     |
| LATITUDINE_GAUS_BOAGA |     |
| QUOTA_PIANO_CAMPAGNA |     |
| INIZIO_VALIDITA |     |
| FINE_VALIDITA |     |
| CODICE_SIRAL |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |

### SIASS_PROVINCE

Province.

| **Campo** | **Descrizione** |
| --- | --- |
| CODICE_PROVINCIA |     |
| DENOMINAZIONE_PROVINCIA |     |
| SIGLA_PROVINCIA |     |

### SIASS_STAZIONI

Stazioni.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_STAZIONE |     |
| CODICE_IDENTIFICATIVO |     |
| DESCRIZIONE |     |
| ESCLUSA_MONITORAGGIO |     |
| ID_TIPO_STAZIONE |     |
| ANNOTAZIONI |     |
| ID_ALLEGATO_FOTO_STAZIONE | Id dell’allegato stazione. Deve essere una immagine, che viene utilizzata nella produzione del PDF della stazione. |
| ID_ALLEGATO_MAPPA | Id dell’allegato stazione. Deve essere una immagine, che viene utilizzata nella produzione del PDF della stazione. |
| ALLESTIMENTO |     |
| ID_SITO |     |
| TELETRASMISSIONE |     |
| ULTIMO_AGGIORNAMENTO |     |
| AUTORE_ULTIMO_AGGIORNAMENTO |     |
| CONTROLLO_ANOMALIE | Indica se la stazione deve essere monitorata dall’applicazione Controllo anomalie |
| PUNTO_CONFORMITA |     |

### SIASS_TIPI_ACCESSO

Tipi di accesso di una installazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_ACCESSO |     |
| DESCRIZIONE_TIPO_ACCESSO |     |

### SIASS_TIPI_CHIUSURA

Tipi di chiusura di un pozzo.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_CHIUSURA |     |
| DESCRIZIONE_TIPO_CHIUSURA |     |

### SIASS_TIPI_DESTINAZIONE_USO

Tipi di destinazione d’uso di un pozzo.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_DESTINAZIONE_USO |     |
| DESCR_TIPO_DESTINAZIONE_USO |     |

### SIASS_TIPI_FISSAGGIO_TRASM

Tipi di fissaggio di un trasmettitore.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_FISSAGGIO_TRASM |     |
| DESCR_TIPO_FISSAGGIO_TRASM |     |

### SIASS_TIPI_FREQUENZA_UTILIZZO

Frequenza di utilizzo di un pozzo.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_FREQUENZA_UTILIZZO |     |
| DESCR_TIPO_FREQUENZA_UTILIZZO |     |

### SIASS_TIPI_STAZIONE

Tipi di stazione.

| **Campo** | **Descrizione** |
| --- | --- |
| ID_TIPO_STAZIONE |     |
| DESCRIZIONE_TIPO_STAZIONE |     |
| CATEGORIA |     |

7\. Descrizione API applicazione Web

L’applicazione Web espone una API con i verbali degli interventi con prelievo di campioni. L’API è raggiungibile alla URL \[percorso\]/EsportazioneVerbali.ashx?apikey=\[API Key definita in configurazione\].

Di seguito un JSON di esempio di risposta in cui viene omesso per brevità il PDF del verbale:

{

"Data": \[

{

"Analiti": \[

{

"Codice": "ALYT000137",

"CodiceMetodo": "C237",

"CodicePacchetto": "SP_PK343761",

"Descrizione": "2,4,5 T (Ac. 2,4,5-Triclorofenossiacetico)"

},

{

"Codice": "ALYT1999",

"CodiceMetodo": "C626",

"CodicePacchetto": "SP_PK343761",

"Descrizione": "Fosetil alluminio"

},

{

"Codice": "ANALYT999",

"CodiceMetodo": "C237",

"CodicePacchetto": "SP_PK343761",

"Descrizione": "Bifenox"

}

\],

"Annotazioni": null,

"CampioneBianco": false,

"CodiceArgomento": "07MOSTQ - 07P\*06",

"CodiceRichiedente": null,

"CodiceStazione": "GEB012",

"CodiciPacchetto": \[

"GE_PK405050",

"SP_PK343761"

\],

"DataOraIntervento": "2025-02-19T00:00:00+01:00",

"DataOraVerbale": "2025-02-19T00:00:00+01:00",

"DatiCampioneBianco": null,

"\[OMISSIS: FILE PDF DEL VERBALE\]",

"Matrice": "MTX0000101",

"Misurazioni": \[

{

"CodiceAnalita": "ALYT000400",

"CodiceMetodo": "C071",

"CodicePacchetto": "GE_PK352842",

"Valore": 300

},

{

"CodiceAnalita": "ALYT000469",

"CodiceMetodo": "T045",

"CodicePacchetto": "GE_PK352842",

"Valore": 100

},

{

"CodiceAnalita": "ALYT000750",

"CodiceMetodo": "T016",

"CodicePacchetto": "GE_PK352842",

"Valore": 4.3

},

{

"CodiceAnalita": "ALYT000821",

"CodiceMetodo": "C074",

"CodicePacchetto": "GE_PK352842",

"Valore": 7.7

},

{

"CodiceAnalita": "ALYT001064",

"CodiceMetodo": "T032",

"CodicePacchetto": "GE_PK352842",

"Valore": 25

}

\],

"NomeFileVerbale": "SIASS_20250704_XYZ007_ID16222.pdf",

"NumeroCampioniRilevati": 1,

"Prelevatore": "123456",

"QuotaCampione": null,

"SiglaVerbale": "SIASS_20250704_XYZ007_ID16222"

}

\],

"ErrorDetails": null,

"ErrorMessage": null,

"Success": true

}

## 8\. Dati da servizi esterni

L’applicazione utilizza API esterne per la configurazione operatore e le anagrafiche di analiti, pacchetti e contenitori. Le API devono richiedere una autenticazione mediante API Key.

Per l’autenticazione integrata con Windows l’applicazione chiama l’API definita in configurazione passando il nome dell’utente collegato e ricavandone il profilo di autorizzazione.

Per l’autenticazione tramite reverse proxy l’applicazione chiama l’API definita in configurazione passando il codice fiscale trovato negli header di autenticazione e ricavandone il profilo.

L’applicazione utilizza inoltre un Web service per ottenere la matricola dell’operatore fornendone l’identificativo e uno per ottenere gli operatori utilizzabili negli interventi.

### Servizio anagrafiche esterne

#### /ANAccessoUtenti

Parametri in ingresso:

| **Parametro** | **Descrizione** |
| --- | --- |
| Risorsa | Nome della risorsa |
| CF  | Codice fiscale |
| Username | Username |
| ApiKey | Chiave di autenticazione |

Output:

Lista di oggetti contenenti i seguenti attributi:

| **Attributo** | **Descrizione** |
| --- | --- |
| ARGOMENTO | Argomento a cui ha accesso l’operatore |
| CF  | Codice fiscale |
| COD_ANA_ENTE | Codice ente |
| COD_ARGOMENTO | Codice argomento |
| COGNOME | Cognome |
| ID  | Identificativo operatore |
| LINK | Link verso lo strumento di reportistica |
| MATRICE_IDENTITY | Codice matrice |
| MATRICE_NAME | Descrizione matrice |
| NOME | Nome |
| PIVA | Partita IVA |
| PROFILO | AMMINISTRATORE, GESTIONE, VISUALIZZAZIONE |
| RAG_SOC | Ragione sociale |
| RISORSA | Identificativo dell’applicazione |
| RUOLO | Ruolo nel sistema di autorizzazione |
| SEDE | Sede |
| USERNAME | Identificativo dell’account Windows |

#### /ANMatriciAnaliti

Parametri in ingresso:

| **Parametro** | **Descrizione** |
| --- | --- |
| matrice | Codice matrice |
| cod_argomento | Codice argomento |
| cod_pack | Codice pacchetto |
| analita_name | Parte del nome dell’analita |
| linea | “Laboratorio” o “Territorio” |
| ApiKey | Chiave di autenticazione |

Output:

Lista di oggetti contenenti i seguenti attributi:

| **Attributo** | **Descrizione** |
| --- | --- |
| PACK_IDENTITY | Codice pacchetto |
| PACK_NAME | Descrizione pacchetto |
| PACK_TYPE |     |
| MATRICE_IDENTITY | Codice matrice |
| MATRICE_NAME | Descrizione matrice |
| METODO_IDENTITY | Codice metodo |
| METODO_NAME | Descrizione metodo |
| ANALITA_IDENTITY | Codice analita |
| ANALITA_NAME | Descrizione analita |
| PARAM_UNITS | Unità di misura |
| FRAZIONE |     |
| REPARTO |     |
| LINEA |     |
| COD_ARGOMENTO | Codice argomento |
| ARGOMENTO | Descrizione argomento |
| VALORE_LIMITE |     |
| ORDER_NUM | Ordine |
| LDQ |     |
| LIMITE_INDIC |     |
| LIMITE_MINIMO |     |
| LIMITE_MASSIMO |     |
| LABORATORIO_ANALISI |     |
| LINEA_LAV | “Laboratorio” o “Territorio” |

#### /ANPacchetti

Parametri in ingresso:

| **Parametro** | **Descrizione** |
| --- | --- |
| matrice | Codice matrice |
| cod_argomento | Codice argomento |
| sede | Codice della sede |
| linea | “Laboratorio” o “Territorio” |
| ApiKey | Chiave di autenticazione |

Output:

Lista di oggetti contenenti i seguenti attributi:

| **Attributo** | **Descrizione** |
| --- | --- |
| PACK_IDENTITY | Codice pacchetto |
| PACK_NAME | Descrizione pacchetto |
| COD_ARGOMENTO | Codice argomento |
| SEDE | Codice sede |
| LINEA_LAV | “Laboratorio” o “Territorio” |

#### /ANContenitori

Parametri in ingresso:

| **Parametro** | **Descrizione** |
| --- | --- |
| pack_identity | Codice pacchetto |
| ApiKey | Chiave di autenticazione |

Output:

Lista di oggetti contenenti i seguenti attributi:

| **Attributo** | **Descrizione** |
| --- | --- |
| IDENTITY | Codice contenitore |
| BOTTLE_TYPE | Tipo |
| ORDER_NUM | Ordine |
| QUANTITY | Quantità |

### Web service esterni

#### Web service IdAnsoloPerUtenteGSO

Dato un id utente del servizio di autenticazione ne ritorna il corrispondente del servizio di gestione anagrafiche.

Richiesta POST:

IDUtenteGSO=string

XML di risposta:

&lt;int xmlns="<http://www.arpal.org/ansolo"&gt;123&lt;/int>&gt;

#### Web service GetApplicationUsers

Data una applicazione, ne ritorna gli utenti.

Richiesta POST :

sApplication=string

XML di risposta:

<DataSet

xmlns="<http://tempuri.org/">>

<xs:schema

xmlns=""

xmlns:xs="<http://www.w3.org/2001/XMLSchema>"

xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"

xmlns:msprop="urn:schemas-microsoft-com:xml-msprop" id="NewDataSet">

&lt;xs:element name="NewDataSet" msdata:IsDataSet="true" msdata:UseCurrentLocale="true"&gt;

&lt;xs:complexType&gt;

&lt;xs:choice minOccurs="0" maxOccurs="unbounded"&gt;

&lt;xs:element name="Table1" msprop:SqlHash="-2120557311" msprop:BaseTable.0="GSOV_GET_APP_USER_PROF" msprop:BaseTable.1="GSO_UTENTI"&gt;

&lt;xs:complexType&gt;

&lt;xs:sequence&gt;

&lt;xs:element name="ID_UTENTE" msprop:BaseColumn="ID_UTENTE" msprop:OraDbType="113" type="xs:decimal" minOccurs="0"/&gt;

&lt;xs:element name="NOME" msprop:BaseColumn="NOME" msprop:OraDbType="126" type="xs:string" minOccurs="0"/&gt;

&lt;xs:element name="COGNOME" msprop:BaseColumn="COGNOME" msprop:OraDbType="126" type="xs:string" minOccurs="0"/&gt;

&lt;xs:element name="DESCRIZIONE_PROFILO" msprop:BaseColumn="DESCRIZIONE" msprop:OraDbType="126" type="xs:string" minOccurs="0"/&gt;

&lt;/xs:sequence&gt;

&lt;/xs:complexType&gt;

&lt;/xs:element&gt;

&lt;/xs:choice&gt;

&lt;/xs:complexType&gt;

&lt;/xs:element&gt;

&lt;/xs:schema&gt;

<diffgr:diffgram

xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"

xmlns:diffgr="urn:schemas-microsoft-com:xml-diffgram-v1">

<NewDataSet

xmlns="">

&lt;Table1 diffgr:id="Table11" msdata:rowOrder="0"&gt;

&lt;ID_UTENTE&gt;123&lt;/ID_UTENTE&gt;

&lt;NOME&gt;Mario&lt;/NOME&gt;

&lt;COGNOME&gt;Rossi&lt;/COGNOME&gt;

&lt;DESCRIZIONE_PROFILO&gt;GESTIONE&lt;/DESCRIZIONE_PROFILO&gt;

&lt;/Table1&gt;

&lt;/NewDataSet&gt;

&lt;/diffgr:diffgram&gt;

&lt;/DataSet&gt;
