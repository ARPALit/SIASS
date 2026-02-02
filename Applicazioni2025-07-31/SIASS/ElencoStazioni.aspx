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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoStazioni.aspx.cs" Inherits="SIASS.ElencoStazioni" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Stazioni</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <h1>Stazioni</h1>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ProvinceDropDownList" />
            <asp:PostBackTrigger ControlID="CercaStazioneButton" />
            <asp:PostBackTrigger ControlID="RimuoviFiltriButton" />
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
                    <asp:Label ID="Label12" runat="server" Text="Gestore:" AssociatedControlID="GestoriDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="GestoriDropDownList" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <asp:Label ID="Label10" runat="server" Text="Includi fuori monitoraggio:" AssociatedControlID="EsclusaMonitoraggioCheckBox" CssClass="form-label"></asp:Label>
                    <asp:CheckBox ID="EsclusaMonitoraggioCheckBox" runat="server" CssClass="form-check" />
                </div>
                <div class="col-12">
                    <asp:Button ID="CercaStazioneButton" runat="server" OnClick="CercaStazioneButton_Click" Text="Cerca" CssClass="btn btn-primary" />
                    <asp:Button ID="RimuoviFiltriButton" runat="server" Text="Rimuovi filtri" CssClass="btn btn-secondary" OnClick="RimuoviFiltriButton_Click" CausesValidation="False" />
                    <asp:Button ID="NuovaStazioneButton" runat="server" CausesValidation="False" CssClass="btn btn-success" OnClick="NuovaStazioneButton_Click" Text="Nuova stazione" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="GestorePanel" runat="server">
        <div class="row g-3 mb-3 small">
            <div class="col-md-2">
                <asp:Label ID="Label13" runat="server" Text="Codice identificativo/descrizione:" AssociatedControlID="GestoreCodiceIdentificativoDescrizioneTextBox" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="GestoreCodiceIdentificativoDescrizioneTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-12">
                <asp:Button ID="GestoreCercaStazioneButton" runat="server" Text="Cerca" CssClass="btn btn-primary" OnClick="GestoreCercaStazioneButton_Click" />
            </div>
        </div>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="StazioniGridView" />
        </Triggers>
        <ContentTemplate>
            <asp:GridView ID="StazioniGridView" runat="server" AllowCustomPaging="True" AllowPaging="True" OnPageIndexChanging="StazioniGridView_PageIndexChanging" PageSize="20" CssClass="table table-bordered table-striped table-hover small w-auto" EmptyDataText="Nessuna stazione trovata" AutoGenerateColumns="False" OnRowDataBound="StazioniGridView_RowDataBound" DataKeyNames="IdStazione">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="ImmagineTipoStazioneImage" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Identificativo">
                        <ItemTemplate>
                            <asp:Label ID="CodiceIdentificativoLabel" runat="server" Text='<%# Bind("CodiceIdentificativo") %>'></asp:Label>
                            <asp:HyperLink ID="VisualizzaStazioneHyperLink" runat="server" NavigateUrl='<%# Eval("IdStazione", "~/Stazione/VisualizzaStazione.aspx?IdStazione={0}") %>' Text='<%# Bind("CodiceIdentificativo") %>'></asp:HyperLink>
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
                    <asp:TemplateField HeaderText="PDF stazione">
                        <ItemTemplate>
                            <asp:ImageButton ID="PDFStazioneImageButton" runat="server" Height="16px" ImageUrl="~/img/pdf_icon.png" OnClick="PDFStazioneImageButton_Click" Width="16px" CausesValidation="false" AlternateText="PDF stazione" ToolTip="PDF stazione" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Interventi">
                        <ItemTemplate>
                            <asp:HyperLink ID="InterventiHyperLink" runat="server" NavigateUrl='<%# Eval("IdStazione", "~/Stazione/Interventi/ElencoInterventi.aspx?IdStazione={0}") %>' Text="Interventi"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:HyperLinkField DataNavigateUrlFields="IdStazione" DataNavigateUrlFormatString="~/Stazione/Grandezze/ElencoGrandezze.aspx?IdStazione={0}" Text="Misurazioni" HeaderText="Misurazioni" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="MappaPanel" runat="server">
        <div id="map" style="<%=ImpostaVisibilitaMappa() %>"></div>
    </asp:Panel>
    <div>
        <small>
            <asp:Label ID="VersioneLabel" runat="server"></asp:Label>
            -
            <asp:HyperLink ID="PrivacyHyperLink" runat="server" Target="_blank">Privacy</asp:HyperLink></small>
    </div>
</asp:Content>
