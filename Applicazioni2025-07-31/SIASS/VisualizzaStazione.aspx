<%-- 
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
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="VisualizzaStazione.aspx.cs" Inherits="SIASS.VisualizzaStazione" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Stazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="map" style="width: 1200px; height: 250px; border: 1px solid #ccc; margin-top: 1em; margin-bottom: 1em;">
        <asp:Panel ID="NoLocalizzazionePanel" runat="server">
            <div style="background-color: #ddd; color: #000; height: 250px; padding: 110px; text-align: center;">Nessuna informazione di localizzazione per questa stazione.</div>
        </asp:Panel>
    </div>
    <div class="small">
        <div class="row mb-3">
            <div class="col-3">
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="ModificaStazioneHyperLink" runat="server" CssClass="text-decoration-none"></asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Tipo:
                    <asp:Label ID="DescrizioneTipoStazioneLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Fuori monitoraggio:
                    <asp:Label ID="EsclusaMonitoraggioLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Controllo anomalie:
                    <asp:Label ID="ControlloAnomalieLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Rete di appartenenza:
                    <asp:Label ID="ReteAppartenenzaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Allestimento:
                    <asp:Label ID="AllestimentoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Teletrasmissione:
                    <asp:Label ID="TeletrasmissioneLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Punto di conformità:
                    <asp:Label ID="PuntoConformitaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Sito:
                    <asp:Label ID="CodiceIdentificativoDescrizioneComuneProvinciaSitoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Annotazioni:
                    <asp:Label ID="AnnotazioniLabel" runat="server"></asp:Label></li>
                    </ul>
                </div>
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="ModificaDatiAmministrativiHyperLink" runat="server" CssClass="text-decoration-none">Dati amministrativi &rarr;</asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Gestore:
                    <asp:Label ID="GestoreLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Indirizzo Gestore:
                    <asp:Label ID="IndirizzoGestoreLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Telefono:
                    <asp:Label ID="TelefonoGestoreLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Partita IVA:
                    <asp:Label ID="PartitaIVAGestoreLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Riferimento:
                    <asp:Label ID="RiferimentoGestoreLabel" runat="server"></asp:Label></li>
                    </ul>
                </div>
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="ModificaCaratteristicheTecnichePozzoHyperLink" runat="server" CssClass="text-decoration-none">Caratteristiche tecniche &rarr;</asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Profondità (m):
                    <asp:Label ID="ProfonditaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Diametro (cm):
                    <asp:Label ID="DiametroLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Range di soggiacenza (cm):
                    <asp:Label ID="RangeSoggiacenzaDaLabel" runat="server"></asp:Label>-<asp:Label ID="RangeSoggiacenzaALabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Tipo chiusura:
                    <asp:Label ID="DescrizioneTipoChiusuraLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Quota boccapozzo rispetto al piano campagna (cm):
                    <asp:Label ID="QuotaBoccapozzoPcLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Quota boccapozzo (m s.l.m.):
                    <asp:Label ID="QuotaBoccapozzoSlmLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Quota piano di riferimento (PR) (m s.l.m.):
                    <asp:Label ID="QuotaPianoRiferimentoSlmLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Differenza PR-PC (cm):
                    <asp:Label ID="DifferenzaPrpcLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Profondità emungimento rispetto piano campagna (m):
                    <asp:Label ID="ProfonditaEmungimentoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Portata massima di esercizio (l/s):
                    <asp:Label ID="PortataMassimaEsercizioLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Presenza foro per la sonda:
                    <asp:Label ID="PresenzaForoSondaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Destinazione d'uso:
                    <asp:Label ID="DescrizioneTipoDestinazioneusoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Frequenza di utilizzo:
                    <asp:Label ID="DescrizioneTipoFrequenzaUtilizzoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Captata:
                    <asp:Label ID="CaptataLabel" runat="server"></asp:Label></li>
                    </ul>
                </div>
            </div>
            <div class="col-3">
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="ModificaLocalizzazioneHyperLink" runat="server" CssClass="text-decoration-none">Localizzazione &rarr;</asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Comune:
                    <asp:Label ID="DenominazioneComuneProvinciaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Località:
                    <asp:Label ID="LocalitaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Bacino:
                    <asp:Label ID="DescrizioneBacinoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Corpo idrico:
                    <asp:Label ID="DescrizioneCorpoIdricoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">CTR 1:10000:
                    <asp:Label ID="CTRLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Latitudine:
                    <asp:Label ID="LatitudineLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Longitudine:
                    <asp:Label ID="LongitudineLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Latitudine Gauss-Boaga (m):
                    <asp:Label ID="LatitudineGaussBoagaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Longitudine Gauss-Boaga (m):
                    <asp:Label ID="LongitudineGaussBoagaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Quota piano campagna (PC) (m s.l.m.):
                    <asp:Label ID="QuotaPianoCampagnaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Codice SIRAL:
                    <asp:Label ID="CodiceSIRALLabel" runat="server"></asp:Label></li>
                    </ul>
                </div>
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="ModificaCaratteristicheInstallazioneHyperLink" runat="server" CssClass="text-decoration-none">Caratteristiche dell'installazione &rarr;</asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Tipo fissaggio trasmettitore:
                    <asp:Label ID="DescrizioneTipoFissaggioTrasmettitoreLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Cavo esterno in guaina:
                    <asp:Label ID="CavoEsternoInGuainaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Cavo sottotraccia:
                    <asp:Label ID="CavoSottotracciaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Protezione area:
                    <asp:Label ID="ProtezioneAreaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Accesso:
                    <asp:Label ID="DescrizioneTipoAccessoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Osservazioni:
                    <asp:Label ID="OsservazioniLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Sicurezza:
                    <asp:Label ID="SicurezzaLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Profondità sensore (m):
                    <asp:Label ID="ProfonditaSensoreLabel" runat="server"></asp:Label></li>
                    </ul>
                </div>
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="ModificaStrumentiHyperLink" runat="server" CssClass="text-decoration-none">Strumenti &rarr;</asp:HyperLink></h5>
                    </div>
                    <asp:Repeater ID="StrumentiStazioneRepeater" runat="server">
                        <HeaderTemplate>
                            <ul class="list-group list-group-flush">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item">
                                <asp:Label ID="DescrizioneTipoStrumentoLabel" runat="server" Text='<%# HttpUtility.HtmlEncode(Eval("DescrizioneTipoStrumento")) %>'></asp:Label>
                                <asp:Label ID="MarcaLabel" runat="server" Text='<%# HttpUtility.HtmlEncode(Eval("Marca")) %>'></asp:Label>
                                <asp:Label ID="ModelloLabel" runat="server" Text='<%# HttpUtility.HtmlEncode(Eval("Modello")) %>'></asp:Label>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <div class="col-3">
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="InterventiHyperLink" runat="server" CssClass="text-decoration-none">Interventi &rarr;</asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Data ultimo intervento:
                    <asp:Label ID="DataInterventoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Tipo:
                    <asp:Label ID="TipoInterventoLabel" runat="server"></asp:Label></li>
                        <li class="list-group-item">Operatori:
                    <asp:Label ID="OperatoriLabel" runat="server"></asp:Label></li>
                    </ul>
                </div>
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="GrandezzeMisurazioniHyperLink" runat="server" CssClass="text-decoration-none">Grandezze e misurazioni &rarr;</asp:HyperLink></h5>
                    </div>
                    <asp:Repeater ID="GrandezzeStazioneRepeater" runat="server">
                        <HeaderTemplate>
                            <ul class="list-group list-group-flush">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item">
                                <asp:Label ID="GrandezzaLabel" runat="server" Text='<%# Eval("GRANDEZZA") %>'></asp:Label>
                                (<asp:Label ID="MarcaLabel" runat="server" Text='<%# Eval("UNITA_MISURA") %>'></asp:Label>)
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="card mb-3">
                    <div class="card-header">
                        <h5>
                            <asp:HyperLink ID="MisurazioniOrganocloruratiHyperLink" runat="server" CssClass="text-decoration-none">Misurazioni organoclorurati &rarr;</asp:HyperLink></h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Elenco delle misurazioni degli organoclorurati
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <p>
        <asp:LinkButton ID="PDFStazioneLinkButton" runat="server" CssClass="btn btn-primary" OnClick="PDFStazioneLinkButton_Click">PDF Stazione</asp:LinkButton>
        <asp:LinkButton ID="PDFInterventiLinkButton" runat="server" CssClass="btn btn-primary" OnClick="PDFInterventiLinkButton_Click">PDF Interventi</asp:LinkButton>
        <asp:HyperLink ID="AllegatiStazioneHyperLink" runat="server" CssClass="btn btn-primary">Allegati</asp:HyperLink>
    </p>
</asp:Content>
