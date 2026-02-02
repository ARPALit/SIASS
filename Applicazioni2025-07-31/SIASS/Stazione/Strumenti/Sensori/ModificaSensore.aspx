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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ModificaSensore.aspx.cs" Inherits="SIASS.ModificaSensore" %>

<%@ Register Src="../../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Sensore</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Modifica sensore</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
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
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label2" runat="server" Text="Codice identificativo:" CssClass="form-label"></asp:Label>
        <asp:Label ID="CodiceIdentificativoLabel" runat="server"></asp:Label>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label4" runat="server" Text="Grandezza:" AssociatedControlID="GrandezzeStazioneDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="GrandezzeStazioneDropDownList" runat="server" CssClass="form-select" DataTextField="GrandezzaEUnitaMisura" DataValueField="IdGrandezzaStazione"></asp:DropDownList>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="Grandezza: Obbligatorio" ControlToValidate="GrandezzeStazioneDropDownList" CssClass="badge bg-danger" Display="Dynamic">Obbligatorio</asp:RequiredFieldValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label5" runat="server" Text="Codice PMC:" AssociatedControlID="CodicePMCTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="CodicePMCTextBox" runat="server" CssClass="form-control" MaxLength="50" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <asp:Panel ID="DatiSensoreNonVisibileInterventoPanel" runat="server">
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label6" runat="server" Text="Espressione risultato:" AssociatedControlID="TipiEspressioneRisultatoDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiEspressioneRisultatoDropDownList" runat="server" CssClass="form-select" DataTextField="DESCR_TIPO_ESPRESS_RISULTATO" DataValueField="ID_TIPO_ESPRESS_RISULTATO"></asp:DropDownList>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label7" runat="server" Text="Frequenza acquisizione:" AssociatedControlID="FrequenzaAcquisizioneTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="FrequenzaAcquisizioneTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label8" runat="server" Text="Metodo:" AssociatedControlID="TipiMetodoDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiMetodoDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE_METODO" DataValueField="ID_TIPO_METODO"></asp:DropDownList>
        </div>
        <div class="mb-3" style="max-width: 500px">
            Indicare l'unità di misura solo se differente da quella della grandezza e specificare il coefficiente moltiplicativo di conversione unità di misura.
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label10" runat="server" Text="Unità di misura del sensore:" AssociatedControlID="TipiUnitaMisuraDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiUnitaMisuraDropDownList" runat="server" CssClass="form-select" DataTextField="NOME_UNITA_MISURA" DataValueField="NOME_UNITA_MISURA"></asp:DropDownList>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label9" runat="server" Text="Coefficiente conversione unità di misura del sensore:" AssociatedControlID="CoefficienteConversioneUnitaMisuraTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="CoefficienteConversioneUnitaMisuraTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="CoefficienteConversioneUnitaMisuraTextBox" CssClass="badge bg-danger" Display="Dynamic" ErrorMessage="Coefficiente conversione unità di misura: Deve essere un numero" MaximumValue="9999999" MinimumValue="0" Type="Double">Deve essere un numero</asp:RangeValidator>
        </div>
    </asp:Panel>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
        <button class="btn btn-danger" data-bs-target="#confermaEliminazioneModal" data-bs-toggle="modal" type="button">Elimina</button>
    </div>
    <div id="confermaEliminazioneModal" aria-hidden="true" aria-labelledby="confermaEliminazioneModal" class="modal fade" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Elimina sensore</h5>
                </div>
                <div class="modal-body">
                    Eliminare questo sensore?
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                    <asp:Button ID="EliminaButton" runat="server" Text="Elimina" CssClass="btn btn-danger" CausesValidation="false" OnClick="EliminaButton_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
