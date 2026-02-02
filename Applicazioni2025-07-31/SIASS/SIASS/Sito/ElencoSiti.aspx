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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoSiti.aspx.cs" Inherits="SIASS.ElencoSiti" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Elenco siti</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <h1>Siti</h1>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ProvinceDropDownList" />
            <asp:PostBackTrigger ControlID="CercaSitoButton" />
            <asp:PostBackTrigger ControlID="RimuoviFiltriButton" />
        </Triggers>
        <ContentTemplate>
            <div class="row g-3 mb-3 small">
                <div class="col-md-4">
                    <asp:Label ID="Label1" runat="server" Text="Codice identificativo/descrizione:" AssociatedControlID="CodiceIdentificativoDescrizioneTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="CodiceIdentificativoDescrizioneTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:Label ID="Label3" runat="server" Text="Provincia:" AssociatedControlID="ProvinceDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="ProvinceDropDownList" runat="server" CssClass="form-select" DataTextField="DenominazioneProvincia" DataValueField="CodiceProvincia" AutoPostBack="True" OnSelectedIndexChanged="ProvinceDropDownList_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <asp:Label ID="Label2" runat="server" Text="Comune:" AssociatedControlID="ComuniDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="ComuniDropDownList" runat="server" CssClass="form-select" DataTextField="DenominazioneComune" DataValueField="CodiceComune"></asp:DropDownList>
                </div>
                <div class="col-12">
                    <asp:Button ID="CercaSitoButton" runat="server" OnClick="CercaSitoButton_Click" Text="Cerca" CssClass="btn btn-primary" />
                    <asp:Button ID="RimuoviFiltriButton" runat="server" Text="Rimuovi filtri" CssClass="btn btn-secondary" OnClick="RimuoviFiltriButton_Click" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:GridView ID="SitiGridView" runat="server" AllowCustomPaging="True" AllowPaging="True" OnPageIndexChanging="SitiGridView_PageIndexChanging" PageSize="20" EmptyDataText="Nessun sito trovato" CssClass="table table-bordered table-striped small w-auto" DataKeyNames="IdSito" AutoGenerateColumns="False">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdSito" DataNavigateUrlFormatString="~/Sito/VisualizzaSito.aspx?IdSito={0}" DataTextField="CodiceIdentificativo" HeaderText="Identificativo" />
            <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" />
            <asp:BoundField DataField="ComuneProvincia" HeaderText="Comune" />
        </Columns>
    </asp:GridView>
    <div id="map" style="<%=ImpostaVisibilitaMappa() %>"></div>
</asp:Content>
