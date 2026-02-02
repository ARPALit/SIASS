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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderInterventoResponsive.ascx.cs" Inherits="SIASS.HeaderInterventoResponsive" %>
<div class="navbar navbar-expand-lg navbar-light bg-light">
    <asp:Panel ID="NavigazioneGestionePanel" runat="server">
        Stazione
    <asp:HyperLink ID="StazioneHyperLink" runat="server" CssClass="link-primary mx-2"></asp:HyperLink>
    </asp:Panel>
    <asp:HyperLink ID="ElencoInterventiHyperLink" runat="server" CssClass="link-primary mx-2">Interventi</asp:HyperLink>
</div>
