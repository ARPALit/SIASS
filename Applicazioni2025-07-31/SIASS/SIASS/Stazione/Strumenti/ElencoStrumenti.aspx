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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoStrumenti.aspx.cs" Inherits="SIASS.ElencoStrumenti" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Strumenti</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Strumenti</h1>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuovo strumento</asp:HyperLink>
    </p>
    <asp:GridView ID="ElencoStrumentiGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessuno strumento definito per questa stazione" CssClass="table table-bordered table-striped table-hover small w-auto" DataKeyNames="IdStrumento">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdStrumento" DataNavigateUrlFormatString="VisualizzaStrumento.aspx?IdStrumento={0}" DataTextField="DescrizioneTipoStrumento" HeaderText="Tipo" />
            <asp:BoundField DataField="NumeroDiSerie" HeaderText="Numero di serie" />
            <asp:BoundField DataField="Marca" HeaderText="Marca" />
            <asp:BoundField DataField="Modello" HeaderText="Modello" />
            <asp:BoundField DataField="NumeroInventarioArpal" HeaderText="Numero inventario Arpal" />
            <asp:BoundField DataField="Caratteristiche" HeaderText="Caratteristiche" />
            <asp:BoundField DataField="CodiceSistemaGestionale" HeaderText="Codice sistema gestionale" />
            <asp:BoundField DataField="InizioValidita" DataFormatString="{0:dd/MM/yyyy}"
                HeaderText="Inizio validità">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="FineValidita" DataFormatString="{0:dd/MM/yyyy}"
                HeaderText="Fine validità">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
</asp:Content>
