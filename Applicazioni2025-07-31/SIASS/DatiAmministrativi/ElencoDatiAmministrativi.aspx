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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoDatiAmministrativi.aspx.cs" Inherits="SIASS.ElencoDatiAmministrativi" %>

<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Dati amministrativi</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive runat="server" ID="HeaderStazioneResponsive1" />
    <h1>Dati amministrativi</h1>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuovi dati ammministrativi</asp:HyperLink>
    </p>
    <asp:GridView ID="ElencoDatiAmministrativiGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessun dato amministrativo" CssClass="table table-bordered table-striped table-hover small w-auto" DataKeyNames="IdDatiAmministrativi">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdDatiAmministrativi"
                DataNavigateUrlFormatString="ModificaDatiAmministrativi.aspx?IdDatiAmministrativi={0}" Text="Modifica" />
            <asp:BoundField DataField="Gestore" HeaderText="Gestore" />
            <asp:BoundField DataField="IndirizzoGestore" HeaderText="Indirizzo gestore" />
            <asp:BoundField DataField="TelefonoGestore" HeaderText="Telefono" />
            <asp:BoundField DataField="PartitaIVAGestore" HeaderText="Partita IVA" />
            <asp:BoundField DataField="RiferimentoGestore" HeaderText="Riferimento" />
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
