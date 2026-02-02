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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoStazioneControl.ascx.cs"
    Inherits="SIASS.InfoStazioneControl" %>
<asp:FormView runat="server" ItemType="SIASS.Model.InfoStazione"
    DefaultMode="ReadOnly"
    ID="InfoStazioneFormView" SelectMethod="CaricaInfoStazione"
    DataKeyNames="IdStazione">
    <ItemTemplate>
        <h1>
            <asp:HyperLink
                ID="HyperLink1" runat="server" NavigateUrl='<%#"~/Stazione/VisualizzaStazione.aspx?IdStazione=" + Item.IdStazione%>'><%# Item.CodiceIdentificativo %>&nbsp;-&nbsp;<%# Item.Descrizione %></asp:HyperLink>
        </h1>
    </ItemTemplate>
</asp:FormView>
