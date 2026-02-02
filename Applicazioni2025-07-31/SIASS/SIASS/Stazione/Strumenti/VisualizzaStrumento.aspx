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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="VisualizzaStrumento.aspx.cs" Inherits="SIASS.VisualizzaStrumento" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Strumento</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Strumento</h1>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label3" runat="server" Text="Tipo strumento:" CssClass="form-label"></asp:Label>
        <asp:Label ID="DescrizioneTipoStrumentoLabel" runat="server"></asp:Label>
    </div>
    <asp:Panel ID="DatiStrumentoNonVisibileInterventoPanel" runat="server">
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label1" runat="server" Text="Numero di serie:" CssClass="form-label"></asp:Label>
            <asp:Label ID="NumeroDiSerieLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label2" runat="server" Text="Marca:" CssClass="form-label"></asp:Label>
            <asp:Label ID="MarcaLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label4" runat="server" Text="Modello:" CssClass="form-label"></asp:Label>
            <asp:Label ID="ModelloLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label6" runat="server" Text="Numero inventario Arpal:" CssClass="form-label"></asp:Label>
            <asp:Label ID="NumeroInventarioArpalLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label8" runat="server" Text="Caratteristiche:" CssClass="form-label"></asp:Label>
            <asp:Label ID="CaratteristicheLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label10" runat="server" Text="Codice sistema gestionale:" CssClass="form-label"></asp:Label>
            <asp:Label ID="CodiceSistemaGestionaleLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label12" runat="server" Text="Inizio validità:" CssClass="form-label"></asp:Label>
            <asp:Label ID="InizioValiditaLabel" runat="server"></asp:Label>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label14" runat="server" Text="Fine validità:" CssClass="form-label"></asp:Label>
            <asp:Label ID="FineValiditaLabel" runat="server"></asp:Label>
        </div>
    </asp:Panel>
    <div class="my-lg-3">
        <asp:Button ID="ModificaStrumentoButton" runat="server" Text="Modifica" CssClass="btn btn-primary" CausesValidation="false" OnClick="ModificaStrumentoButton_Click" />
        <button class="btn btn-danger" data-bs-target="#confermaEliminazioneModal" data-bs-toggle="modal" type="button" runat="server" id="EliminaConfermaButton" clientidmode="static">Elimina</button>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <h2>Pacchetti ALIMS</h2>
        <asp:GridView ID="PacchettiStrumentoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="IdPacchetto" EmptyDataText="Nessun pacchetto assegnato a questo strumento" ShowHeader="True" CssClass="table table-bordered table-striped table-sm w-auto">
            <Columns>
                <asp:BoundField DataField="DescrizionePacchetto" HeaderText="Pacchetto" />
                <asp:BoundField DataField="CodiceAlims" HeaderText="Codice ALIMS" />
            </Columns>
        </asp:GridView>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="ModificaPacchettiButton" runat="server" Text="Modifica" CssClass="btn btn-primary" CausesValidation="false" OnClick="ModificaPacchettiButton_Click" Visible="False" />
    </div>
    <div class="mb-3">
        <h2>Sensori</h2>
        <asp:GridView ID="SensoriStrumentoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="CodiceIdentificativo" EmptyDataText="Nessun sensore assegnato a questo strumento" ShowHeader="True" CssClass="table table-bordered table-striped table-sm table-hover w-auto">
            <Columns>
                <asp:BoundField DataField="CodiceIdentificativo" HeaderText="Codice identificativo" />
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
    <div class="my-lg-3">
        <asp:Button ID="ModificaSensoriButton" runat="server" Text="Modifica" CssClass="btn btn-primary" CausesValidation="false" OnClick="ModificaSensoriButton_Click" />
        <asp:Button ID="RiassegnaSensoriButton" runat="server" Text="Riassegna sensori ad altro strumento" CssClass="btn btn-primary" CausesValidation="false" OnClick="RiassegnaSensoriButton_Click" />
    </div>
    <p>
        <asp:HyperLink ID="ElencoStrumentiHyperLink" runat="server">&lt;&lt; Strumenti</asp:HyperLink>
    </p>
    <div id="confermaEliminazioneModal" aria-hidden="true" aria-labelledby="confermaEliminazioneModal" class="modal fade" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Elimina strumento</h5>
                </div>
                <div class="modal-body">
                    <asp:Label ID="ConfermaEliminazioneLabel" runat="server" Text="Eliminare questo strumento?"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                    <asp:Button ID="EliminaButton" runat="server" Text="Elimina" CssClass="btn btn-danger" CausesValidation="false" OnClick="EliminaButton_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
