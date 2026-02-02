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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoMisurazioniOrganoclorurati.aspx.cs" Inherits="SIASS.ElencoMisurazioniOrganoclorurati" %>

<%@ Register Src="HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Misurazioni organoclorurati</h1>
    <asp:Panel ID="MisurazioniPanel" runat="server">
        <div class="my-3">
            <asp:Label ID="Label1" runat="server" Text="Data campionamento:"></asp:Label>
            <asp:DropDownList ID="DateDropDownList" runat="server" DataTextFormatString="{0:dd/MM/yyyy}"></asp:DropDownList>
            <asp:Button ID="VisualizzaButton" runat="server" Text="Visualizza" OnClick="VisualizzaButton_Click" />
        </div>
        <asp:GridView ID="ElencoMisurazioniGridView" runat="server" CssClass="table table-bordered table-striped table-hover table-sm w-auto" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="DATA_CAMPIONAMENTO" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data campionamento" HtmlEncode="False" />
                <asp:BoundField DataField="FONTE" HeaderText="Fonte" />
                <asp:BoundField DataField="PARAMETRO" HeaderText="Parametro" />
                <asp:BoundField DataField="FAMIGLIA" HeaderText="Famiglia" />
                <asp:BoundField DataField="CONC_SIGN" HeaderText="Segno" />
                <asp:BoundField DataField="CONC_VAL" HeaderText="Valore" />
                <asp:BoundField DataField="LOQ" HeaderText="LOQ" />
                <asp:BoundField DataField="INCERTEZZA" HeaderText="Incertezza" />
                <asp:BoundField DataField="CONC_CSC" HeaderText="CSC" />
                <asp:BoundField DataField="CONC_UDM" HeaderText="UDM" />
                <asp:BoundField DataField="DATA_AGGIORNAMENTO" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data aggiornamento" HtmlEncode="False" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <asp:Label ID="AvvisoLabel" runat="server" Text="Nessuna misurazione di organoclorurati per questa stazione."></asp:Label>
</asp:Content>
