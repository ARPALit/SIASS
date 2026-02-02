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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="VisualizzaDatiAlimsStazione.aspx.cs" Inherits="SIASS.VisualizzaDatiAlimsStazione" %>

<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Dati ALIMS</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Dati ALIMS stazione</h1>
    <asp:Label ID="NessunDatoDisponibileLabel" runat="server" Text="Nessun dato disponibile"></asp:Label>
    <asp:Panel ID="DatiPanel" runat="server">
        <div class="col-1 my-3">
            <asp:Label ID="Label1" runat="server" Text="Anno:" AssociatedControlID="AnniDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="AnniDropDownList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="AnniDropDownList_SelectedIndexChanged" DataTextField="ANNO" DataValueField="ANNO" CssClass="form-select"></asp:DropDownList>
        </div>
        <asp:GridView ID="DatiAlimsGridView" runat="server" EmptyDataText="Nessun dato disponibile" CssClass="table table-bordered table-striped small w-auto" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="DATA" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data" HtmlEncode="False" />
                <asp:BoundField DataField="PARAMETRO" HeaderText="Parametro" />
                <asp:BoundField DataField="VALORE" HeaderText="Valore" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <p>
        <asp:HyperLink ID="InterventiStazioneHyperLink" runat="server">Torna all'elenco degli interventi</asp:HyperLink>
    </p>
</asp:Content>
