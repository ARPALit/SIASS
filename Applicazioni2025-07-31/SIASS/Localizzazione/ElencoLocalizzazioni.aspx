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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoLocalizzazioni.aspx.cs" Inherits="SIASS.ElencoLocalizzazioni" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Localizzazioni</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Localizzazioni</h1>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuova localizzazione</asp:HyperLink>
    </p>
    <asp:GridView ID="ElencoLocalizzazioniGridView" runat="server"
        CssClass="table table-bordered table-striped table-hover small"
        EmptyDataText="Nessuna localizzazione associata alla stazione."
        AutoGenerateColumns="False">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdLocalizzazione"
                DataNavigateUrlFormatString="ModificaLocalizzazione.aspx?IdLocalizzazione={0}" Text="Modifica" />
            <asp:BoundField DataField="DenominazioneComune" HeaderText="Comune" />
            <asp:BoundField DataField="SiglaProvincia" HeaderText="Provincia" />
            <asp:BoundField DataField="DescrizioneBacino" HeaderText="Bacino" />
            <asp:BoundField DataField="DescrizioneCorpoIdrico" HeaderText="Corpo idrico" />
            <asp:BoundField DataField="CTR" HeaderText="CTR 1:10000" />
            <asp:BoundField DataField="Latitudine" HeaderText="Latitudine" />
            <asp:BoundField DataField="Longitudine" HeaderText="Longitudine" />
            <asp:BoundField DataField="LatitudineGaussBoaga" HeaderText="Latitudine Gauss-Boaga" />
            <asp:BoundField DataField="LongitudineGaussBoaga" HeaderText="Longitudine Gauss-Boaga" />
            <asp:BoundField DataField="QuotaPianoCampagna" HeaderText="Quota piano campagna (PC) (m s.l.m.)" />
            <asp:BoundField DataField="CodiceSIRAL" HeaderText="Codice SIRAL" />
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
