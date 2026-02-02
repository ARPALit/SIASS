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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="VisualizzaSito.aspx.cs" Inherits="SIASS.VisualizzaSito" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Sito</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Sito</h1>
    <div class="row mb-3">
        <div class="col mx-2 border border-1 p-2">
            <h5>Codice identificativo - Descrizione</h5>
            <asp:Label ID="CodiceIdentificativoLabel" runat="server"></asp:Label>
            -
            <asp:Label ID="DescrizioneLabel" runat="server"></asp:Label>
        </div>
        <div class="col mx-2 border border-1 p-2">
            <h5>Comune</h5>
            <asp:Label ID="ComuneProvinciaLabel" runat="server"></asp:Label>
        </div>
        <div class="col mx-2 border border-1 p-2">
            <h5>Indirizzo</h5>
            <asp:Label ID="IndirizzoLabel" runat="server"></asp:Label>
        </div>
        <div class="col mx-2 border border-1 p-2">
            <h5>Codice IFFI</h5>
            <asp:Label ID="CodiceIFFILabel" runat="server"></asp:Label>
        </div>
    </div>
    <div class="row mb-3">
        <div class="col mx-2 border border-1 p-2">
            <h5>Coordinate</h5>
            Latitudine:
                        <asp:Label ID="LatitudineLabel" runat="server"></asp:Label><br />
            Longitudine:
                        <asp:Label ID="LongitudineLabel" runat="server"></asp:Label>
        </div>
        <div class="col mx-2 border border-1 p-2">
            <h5>Coordinate Gauss-Boaga</h5>
            Latitudine:
                        <asp:Label ID="LatitudineGaussBoagaLabel" runat="server"></asp:Label><br />
            Longitudine:
                        <asp:Label ID="LongitudineGaussBoagaLabel" runat="server"></asp:Label>
        </div>
        <div class="col mx-2 border border-1 p-2">
            <h5>Quota piano campagna</h5>
            <asp:Label ID="QuotaPianoCampagnaLabel" runat="server"></asp:Label>
        </div>
        <div class="col mx-2 border border-1 p-2">
            <h5>Superficie</h5>
            <asp:Label ID="SuperficieLabel" runat="server"></asp:Label>
        </div>
    </div>
    <h2 class="head">Stazioni</h2>
    <asp:GridView ID="StazioniGridView" runat="server" EmptyDataText="Nessuna stazione" CssClass="table table-bordered table-striped small w-auto" DataKeyNames="IdStazione" AutoGenerateColumns="False">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdStazione" DataNavigateUrlFormatString="~/Stazione/VisualizzaStazione.aspx?IdStazione={0}" DataTextField="CodiceIdentificativo" HeaderText="Codice" />
            <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" />
            <asp:BoundField DataField="DescrizioneTipoStazione" HeaderText="Tipo" />
        </Columns>
    </asp:GridView>
</asp:Content>
