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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="UltimoInterventoUltimaMisurazione.aspx.cs" Inherits="SIASS.UltimoInterventoUltimaMisurazione" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Ultimo intervento/Ultima misurazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Ultimo intervento/Ultima misurazione</h1>
    <div class="row g-3 mb-3 small">
        <div class="col-md-2">
            <asp:Label ID="Label3" runat="server" Text="Provincia:" AssociatedControlID="ProvinceDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="ProvinceDropDownList" runat="server" CssClass="form-select" DataTextField="DenominazioneProvincia" DataValueField="CodiceProvincia"></asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:Label ID="Label9" runat="server" Text="Tipo stazione:" AssociatedControlID="TipiStazioneDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiStazioneDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneTipoStazione" DataValueField="IdTipoStazione"></asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:Label ID="Label4" runat="server" Text="Rete di appartenenza:" AssociatedControlID="TipiReteAppartenenzaDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiReteAppartenenzaDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE" DataValueField="DESCRIZIONE"></asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:Label ID="Label1" runat="server" Text="Ordina per:" AssociatedControlID="OrdinamentoDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="OrdinamentoDropDownList" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:Label ID="Label10" runat="server" Text="Includi fuori monitoraggio:" AssociatedControlID="EsclusaMonitoraggioCheckBox" CssClass="form-label"></asp:Label>
            <asp:CheckBox ID="EsclusaMonitoraggioCheckBox" runat="server" CssClass="form-check" />
        </div>
        <div class="col-md-2">
            <asp:Label ID="Label2" runat="server" Text="Includi ultima misurazione:" AssociatedControlID="SeUltimaMisurazioneCheckBox" CssClass="form-label"></asp:Label>
            <asp:CheckBox ID="SeUltimaMisurazioneCheckBox" runat="server" CssClass="form-check" />
        </div>
        <div class="col-12">
            <asp:Button ID="CercaStazioneButton" runat="server" Text="Cerca" CssClass="btn btn-primary" OnClick="CercaStazioneButton_Click" />
            <asp:Button ID="RimuoviFiltriButton" runat="server" Text="Rimuovi filtri" CssClass="btn btn-secondary" OnClick="RimuoviFiltriButton_Click" CausesValidation="False" />
        </div>
    </div>
    <asp:GridView ID="StazioniGridView" runat="server" CssClass="table table-sm table-bordered table-striped small w-auto" EmptyDataText="Nessuna stazione trovata" AutoGenerateColumns="False" DataKeyNames="IdStazione" OnRowDataBound="StazioniGridView_RowDataBound">
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
            <asp:BoundField DataField="Comune" HeaderText="Comune" />
            <asp:BoundField DataField="Provincia" HeaderText="Provincia" />
            <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
            <asp:BoundField DataField="ReteAppartenenza" HeaderText="Rete di appartenenza" />
            <asp:BoundField DataField="CodiceSiral" HeaderText="Codice Siral" />
            <asp:BoundField DataField="DataUltimoIntervento" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Ultimo intervento" />
            <asp:BoundField DataField="DurataUltimoIntervento" HeaderText="Durata intervento" />
            <asp:BoundField DataField="DataUltimaMisurazione" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Ultima misurazione" />
            <asp:BoundField DataField="Grandezza" HeaderText="Grandezza" />
            <asp:TemplateField HeaderText="Validazione ultima misurazione">
                <ItemTemplate>
                    <asp:HyperLink ID="VisualizzaMisurazioniHyperLink" runat="server" NavigateUrl='<%# Eval("IdStazione", "~/Stazione/Grandezze/ElencoGrandezze.aspx?IdStazione={0}") %>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fuori monitoraggio">
                <ItemTemplate>
                    <asp:Label ID="EsclusaMonitoraggioLabel" runat="server" Text="&#10004"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
