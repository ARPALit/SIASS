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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoInterventi.aspx.cs" Inherits="SIASS.ElencoInterventi" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Interventi stazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Interventi stazione</h1>
    <asp:MultiView ID="InterventiMultiView" runat="server">
        <asp:View ID="InterventiView" runat="server">
            <asp:Label ID="AvvisoLabel" runat="server" CssClass="text-danger"></asp:Label>
            <div class="btn-toolbar" role="toolbar">
                <div class="btn-group me-2" role="group">
                    <button type="button" class="btn btn-outline-primary dropdown-toggle " data-bs-toggle="dropdown" aria-expanded="false">
                        Nuovo intervento
                    </button>
                    <ul class="dropdown-menu">
                        <li>
                            <asp:HyperLink ID="NuovoInterventoConPrelievoCampioniHyperLink" runat="server" CssClass="link-primary dropdown-item">Con prelievo di campioni</asp:HyperLink></li>
                        <li>
                            <asp:HyperLink ID="NuovoInterventoSenzaPrelievoCampioniHyperLink" runat="server" CssClass="link-primary dropdown-item">Senza prelievo di campioni</asp:HyperLink></li>
                    </ul>
                </div>
                <div class="btn-group me-2" role="group">
                    <asp:HyperLink ID="VisualizzaDatiAlimsStazioneHyperLink" runat="server" CssClass="btn btn-outline-primary">Dati ALIMS della stazione</asp:HyperLink>
                </div>
            </div>
            <p>
                <asp:Label ID="EsitoInvioVerbaleLabel" runat="server"></asp:Label>
            </p>
            <asp:GridView ID="ElencoInterventiGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessun intervento" CssClass="table table-bordered small w-auto" DataKeyNames="IdIntervento" OnRowCommand="ElencoInterventiGridView_RowCommand" OnRowDataBound="ElencoInterventiGridView_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Data">
                        <ItemTemplate>
                            <asp:HyperLink ID="DataInterventoHyperLink" runat="server" Text='<%# Eval("DataIntervento", "{0:dd/MM/yyyy}") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="OrganizzazioneCreazione" HeaderText="OrganizzazioneCreazione" Visible="False" />
                    <asp:BoundField DataField="DescrizioneTipoIntervento" HeaderText="Tipo" />
                    <asp:BoundField DataField="DurataIntervento" HeaderText="Durata" />
                    <asp:BoundField DataField="DescrizioneOperatoriIntervento" HeaderText="Operatori" />
                    <asp:BoundField DataField="DescrizioneOperatoriSupportoIntervento" HeaderText="Operatori a supporto" />
                    <asp:BoundField DataField="ElencoCodiceCampioneRDP" HeaderText="Codice campione/RDP" HtmlEncode="false">
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Verbale">
                        <ItemTemplate>
                            <asp:LinkButton ID="GeneraVerbaleLinkButton" runat="server" CausesValidation="False" CommandArgument='<%#Eval("IdIntervento")%>' CommandName="Genera" Text="Genera"></asp:LinkButton>
                            <asp:LinkButton ID="ScaricaVerbaleLinkButton" runat="server" CausesValidation="False" CommandArgument='<%#Eval("IdIntervento")%>' CommandName="Scarica" Text='<%#Eval("SiglaVerbale")%>'></asp:LinkButton>
                            <asp:Label ID="InviatoLabel" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFields="IdIntervento" DataNavigateUrlFormatString="VisualizzaDatiAlimsIntervento.aspx?IdIntervento={0}" HeaderText="Dati ALIMS intervento" Text="Visualizza dati ALIMS" />
                    <asp:HyperLinkField DataNavigateUrlFields="IdIntervento" DataNavigateUrlFormatString="AllegatiIntervento/ElencoAllegatiIntervento.aspx?IdIntervento={0}" Text="Allegati" HeaderText="Allegati" />
                </Columns>
            </asp:GridView>
        </asp:View>
        <asp:View ID="GeneraVerbaleView" runat="server" OnActivate="GeneraVerbaleView_Activate">
            <div id="LegendaTipiVerbale" runat="server" class="my-3"></div>
            <asp:Label ID="SelezioneModelloVerbaleLabel" runat="server" AssociatedControlID="ModelliVerbaleDropDownList" CssClass="form-label" Text="Selezionare il modello di verbale:"></asp:Label>
            <div class="row my-3">
                <div class="col-2">
                    <asp:DropDownList ID="ModelliVerbaleDropDownList" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </div>
                <div class="col-1">
                    <button class="btn btn-primary" data-bs-target="#confermaGenerazioneVerbaleModal" data-bs-toggle="modal" type="button">Genera verbale</button>
                </div>
            </div>
            <asp:Button ID="TornaElencoInterventiButton" runat="server" OnClick="TornaElencoInterventiButton_Click" CssClass="btn btn-primary" Text="Torna all'elenco interventi" />
            <div id="confermaGenerazioneVerbaleModal" aria-hidden="true" aria-labelledby="confermaGenerazioneVerbaleModal" class="modal fade" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Generazione verbale</h5>
                        </div>
                        <div class="modal-body">
                            Dopo la generazione del verbale non sar&agrave; pi&ugrave; possibile modificare l'intervento o selezionare un differente modello. L'intervento sar&agrave; disponibile ad ALIMS in caso di presenza di campioni.
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                            <asp:Button ID="GeneraVerbaleDaModelloButton" runat="server" Text="Genera" CssClass="btn btn-success" OnClick="GeneraVerbaleDaModelloButton_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:View>
        <asp:View ID="NonPossibileGenerareVerbaleView" runat="server">
            <p>
                <asp:Label ID="NonPossibileGenerareVerbaleLabel" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Button ID="TornaElencoInterventi2Button" runat="server" OnClick="TornaElencoInterventiButton_Click" CssClass="btn btn-primary" Text="Torna all'elenco interventi" />
            </p>
        </asp:View>
    </asp:MultiView>
</asp:Content>
