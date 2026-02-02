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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="Configurazione.aspx.cs" Inherits="SIASS.Configurazione" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Configurazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Configurazione</h1>
    <div class="list-group">
        <asp:HyperLink ID="AggiuntaGrandezzaCampoHyperLink" runat="server" CssClass="list-group-item list-group-item-action link-primary" NavigateUrl="ConfAggiuntaGrandezzaCampo.aspx">Aggiunta grandezza misurabile sul campo</asp:HyperLink>
        <asp:HyperLink ID="RimozioneGrandezzaCampoHyperLink" runat="server" CssClass="list-group-item list-group-item-action link-primary" NavigateUrl="ConfRimozioneGrandezzaCampo.aspx">Rimozione grandezza misurabile sul campo</asp:HyperLink>
        <asp:HyperLink ID="AggiuntaPacchettoInterventoHyperLink" runat="server" CssClass="list-group-item list-group-item-action link-primary" NavigateUrl="ConfAggiuntaPacchettoIntervento.aspx">Aggiunta pacchetto per intervento</asp:HyperLink>
        <asp:HyperLink ID="RimozionePacchettoInterventoHyperLink" runat="server" CssClass="list-group-item list-group-item-action link-primary" NavigateUrl="ConfRimozionePacchettoIntervento.aspx">Rimozione pacchetto per intervento</asp:HyperLink>
    </div>
</asp:Content>
