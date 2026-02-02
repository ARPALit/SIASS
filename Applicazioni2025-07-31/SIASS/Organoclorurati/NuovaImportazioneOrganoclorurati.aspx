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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="NuovaImportazioneOrganoclorurati.aspx.cs" Inherits="SIASS.NuovaImportazioneOrganoclorurati" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Importazione organoclorurati</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Importazione dati organoclorurati</h1>
    <asp:MultiView ID="CaricaNuovoFileMultiView" runat="server">
        <asp:View ID="CaricaNuovoFileView" runat="server">
            <div class="my-3">
                Selezionare un file in formato xlsx.
                <asp:FileUpload ID="FileUpload" runat="server" CssClass="form-control" />
            </div>
            <div class="my-3">
                <asp:Button ID="CaricaButton" runat="server" Text="Carica" CssClass="btn btn-success" OnClientClick="this.disabled=true; this.value='Attendere, prego...';" UseSubmitBehavior="False" OnClick="CaricaButton_Click" />
            </div>
        </asp:View>
        <asp:View ID="EsitoView" runat="server">
            <asp:Label ID="EsitoLabel" runat="server"></asp:Label>
            <div class="my-3">
                <asp:HyperLink ID="CaricaNuovoFileHyperLink" runat="server" CssClass="btn btn-primary">Carica nuovo file</asp:HyperLink>
                <asp:HyperLink ID="ElencoImportazioniHyperLink" runat="server" CssClass="btn btn-primary">Elenco importazioni</asp:HyperLink>
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
