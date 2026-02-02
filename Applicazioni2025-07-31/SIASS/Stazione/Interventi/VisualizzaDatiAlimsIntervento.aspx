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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="VisualizzaDatiAlimsIntervento.aspx.cs" Inherits="SIASS.VisualizzaDatiAlimsIntervento" %>

<%@ Register Src="HeaderInterventoResponsive.ascx" TagName="HeaderInterventoResponsive" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Dati ALIMS</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc2:HeaderInterventoResponsive ID="HeaderInterventoResponsive1" runat="server" />
    <h1>Dati ALIMS</h1>
    <asp:GridView ID="DatiAlimsGridView" runat="server" EmptyDataText="Nessun dato disponibile" CssClass="table table-bordered table-striped small w-auto" AutoGenerateColumns="false" OnRowDataBound="DatiAlimsGridView_RowDataBound"></asp:GridView>
    <p>
        <asp:HyperLink ID="InterventiStazioneHyperLink" runat="server">Torna all'elenco degli interventi</asp:HyperLink>
    </p>
</asp:Content>
