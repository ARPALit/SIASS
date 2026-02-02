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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoSensori.aspx.cs" Inherits="SIASS.ElencoSensori" %>

<%@ Register Src="../../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Sensori</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Sensori</h1>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label3" runat="server" Text="Tipo strumento:" CssClass="form-label"></asp:Label>
        <asp:HyperLink ID="DescrizioneTipoStrumentoHyperLink" runat="server"></asp:HyperLink>
    </div>
    <asp:Panel ID="DatiStrumentoNonVisibileInterventoPanel" runat="server">
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label1" runat="server" Text="Numero di serie strumento:" CssClass="form-label"></asp:Label>
            <asp:Label ID="NumeroDiSerieLabel" runat="server"></asp:Label>
        </div>
    </asp:Panel>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuovo sensore</asp:HyperLink>
    </p>
    <div class="mb-3">
        <asp:GridView ID="SensoriStrumentoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="CodiceIdentificativo" EmptyDataText="Nessun sensore assegnato a questo strumento" CssClass="table table-bordered table-striped table-sm table-hover w-auto">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="CodiceIdentificativo" DataNavigateUrlFormatString="~/Stazione/Strumenti/Sensori/ModificaSensore.aspx?CodiceIdentificativo={0}" DataTextField="CodiceIdentificativo" HeaderText="Codice identificativo" />
                <asp:BoundField DataField="GrandezzaEUnitaMisura" HeaderText="Grandezza e unità di misura della stazione" />
                <asp:BoundField DataField="CodicePMC" HeaderText="Codice PMC" />
                <asp:BoundField DataField="NumeroDecimali" HeaderText="Numero decimali" />
                <asp:BoundField DataField="EspressioneRisultato" HeaderText="Espressione risultato" />
                <asp:BoundField DataField="FrequenzaAcquisizione" HeaderText="Frequenza acquisizione" />
                <asp:BoundField DataField="Metodo" HeaderText="Metodo" />
                <asp:BoundField DataField="UnitaMisuraSensore" HeaderText="Unità di misura del sensore" />
                <asp:BoundField DataField="CoefficienteConversioneUnitaMisura" HeaderText="Coefficiente conversione unità di misura" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
