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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ConfAggiuntaGrandezzaCampo.aspx.cs" Inherits="SIASS.ConfAggiuntaGrandezzaCampo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Configurazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <h1>Aggiunta grandezza misurabile sul campo</h1>
    <asp:MultiView ID="ConfigurazioneMultiView" runat="server">
        <asp:View ID="SelezioneGrandezzaView" runat="server">
            <div class="row g-3 mb-3 small">
                <div class="col-2">
                    <asp:Label ID="Label12" runat="server" Text="Selezione grandezza:" AssociatedControlID="ElencoTipiGrandezzaDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="ElencoTipiGrandezzaDropDownList" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-2">
                    <asp:Label ID="Label13" runat="server" Text="Unità di misura:" AssociatedControlID="ElencoTipiUnitaMisuraDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="ElencoTipiUnitaMisuraDropDownList" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-2">
                    <asp:Label ID="Label14" runat="server" Text="Numero decimali:" AssociatedControlID="NumeroDecimaliTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="NumeroDecimaliTextBox" runat="server" MaxLength="2" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
            </div>
            <asp:Button ID="SelezioneStazioniButton" runat="server" Text="Selezione stazioni &rarr;" CssClass="btn btn-primary" OnClick="SelezioneStazioniButton_Click" />
        </asp:View>
        <asp:View ID="SelezioneStazioniView" runat="server">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ProvinceDropDownList" />
                    <asp:PostBackTrigger ControlID="CercaStazioneButton" />
                    <asp:PostBackTrigger ControlID="AggiungiButton" />
                </Triggers>
                <ContentTemplate>
                    <div class="row g-3 mb-3 small">
                        <div class="col-md-2">
                            <asp:Label ID="Label1" runat="server" Text="Codice identificativo/descrizione:" AssociatedControlID="CodiceIdentificativoDescrizioneTextBox" CssClass="form-label"></asp:Label>
                            <asp:TextBox ID="CodiceIdentificativoDescrizioneTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label3" runat="server" Text="Provincia:" AssociatedControlID="ProvinceDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="ProvinceDropDownList" runat="server" CssClass="form-select" DataTextField="DenominazioneProvincia" DataValueField="CodiceProvincia" AutoPostBack="True" OnSelectedIndexChanged="ProvinceDropDownList_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label2" runat="server" Text="Comune:" AssociatedControlID="ComuniDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="ComuniDropDownList" runat="server" CssClass="form-select" DataTextField="DenominazioneComune" DataValueField="CodiceComune"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label11" runat="server" Text="Sito:" AssociatedControlID="SitiDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="SitiDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE" DataValueField="ID_SITO"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label4" runat="server" Text="Rete di appartenenza:" AssociatedControlID="TipiReteAppartenenzaDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="TipiReteAppartenenzaDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE" DataValueField="DESCRIZIONE"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label6" runat="server" Text="Allestimento:" AssociatedControlID="TipiAllestimentoDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="TipiAllestimentoDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE" DataValueField="DESCRIZIONE"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label5" runat="server" Text="Bacino:" AssociatedControlID="BaciniDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="BaciniDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneBacino" DataValueField="IdBacino"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label7" runat="server" Text="Grandezza rilevata:" AssociatedControlID="TipiGrandezzaDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="TipiGrandezzaDropDownList" runat="server" CssClass="form-select" DataTextField="NOME_GRANDEZZA" DataValueField="NOME_GRANDEZZA"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label8" runat="server" Text="Corpo idrico:" AssociatedControlID="CorpiIdriciDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="CorpiIdriciDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneCorpoIdrico" DataValueField="IdCorpoIdrico"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label9" runat="server" Text="Tipo stazione:" AssociatedControlID="TipiStazioneDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="TipiStazioneDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneTipoStazione" DataValueField="IdTipoStazione"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label15" runat="server" Text="Gestore:" AssociatedControlID="GestoriDropDownList" CssClass="form-label"></asp:Label>
                            <asp:DropDownList ID="GestoriDropDownList" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="Label10" runat="server" Text="Includi fuori monitoraggio:" AssociatedControlID="EsclusaMonitoraggioCheckBox" CssClass="form-label"></asp:Label>
                            <asp:CheckBox ID="EsclusaMonitoraggioCheckBox" runat="server" CssClass="form-check" />
                        </div>
                        <div class="col-12">
                            <asp:Button ID="CercaStazioneButton" runat="server" OnClick="CercaStazioneButton_Click" Text="Cerca" CssClass="btn btn-primary" />
                            <asp:Button ID="AggiungiButton" runat="server" Text="Aggiungi grandezza" CssClass="btn btn-primary" CausesValidation="False" OnClick="AggiungiButton_Click" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="StazioniGridView" />
                </Triggers>
                <ContentTemplate>
                    <asp:GridView ID="StazioniGridView" runat="server" AllowCustomPaging="True" AllowPaging="True" OnPageIndexChanging="StazioniGridView_PageIndexChanging" PageSize="10000" CssClass="table table-bordered table-striped table-hover small w-auto" EmptyDataText="Nessuna stazione trovata" AutoGenerateColumns="False" OnRowDataBound="StazioniGridView_RowDataBound" DataKeyNames="IdStazione">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Image ID="ImmagineTipoStazioneImage" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Identificativo">
                                <ItemTemplate>
                                    <asp:Label ID="CodiceIdentificativoLabel" runat="server" Text='<%# Bind("CodiceIdentificativo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" />
                            <asp:BoundField DataField="DescrizioneSito" HeaderText="Sito" />
                            <asp:BoundField DataField="Comune" HeaderText="Comune" />
                            <asp:BoundField DataField="Provincia" HeaderText="Provincia" />
                            <asp:BoundField DataField="Bacino" HeaderText="Bacino" />
                            <asp:BoundField DataField="CorpoIdrico" HeaderText="Corpo idrico" />
                            <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                            <asp:BoundField DataField="ReteAppartenenza" HeaderText="Rete di appartenenza" />
                            <asp:TemplateField HeaderText="Fuori monitoraggio">
                                <ItemTemplate>
                                    <asp:Label ID="EsclusaMonitoraggioLabel" runat="server" Text="&#10004"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:View>
        <asp:View ID="ConfermaView" runat="server">
            <asp:Label ID="ConfermaLabel" runat="server"></asp:Label><br />
            <asp:Button ID="ConfermaButton" runat="server" Text="Aggiungi" CssClass="btn btn-success my-3" OnClick="ConfermaButton_Click" />
        </asp:View>
        <asp:View ID="EsitoView" runat="server">
            <asp:Label ID="EsitoLabel" runat="server"></asp:Label>
        </asp:View>
    </asp:MultiView>
</asp:Content>
