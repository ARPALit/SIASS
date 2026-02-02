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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoAllegatiStazione.aspx.cs" Inherits="SIASS.ElencoAllegatiStazione" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Allegati stazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Allegati stazione</h1>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuovo allegato</asp:HyperLink>
    </p>
    <asp:GridView ID="ElencoAllegatiStazioneGridView" runat="server"
        CssClass="table table-bordered table-striped table-hover small w-auto"
        EmptyDataText="Nessun allegato associato alla stazione."
        AutoGenerateColumns="False">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdAllegatoStazione"
                DataNavigateUrlFormatString="ModificaAllegatoStazione.aspx?IdAllegatoStazione={0}" Text="Modifica" />
            <asp:HyperLinkField DataNavigateUrlFields="IdStazione,NomeFileAllegato" DataNavigateUrlFormatString="~/File/Allegati/Stazione{0}/{1}" DataTextField="NomeFileAllegato" HeaderText="File" Target="_blank" />
            <asp:BoundField DataField="DescrizioneAllegato" HeaderText="Descrizione" />
            <asp:BoundField DataField="DataOraInserimento" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                HeaderText="Data inserimento" />
        </Columns>
    </asp:GridView>
</asp:Content>
