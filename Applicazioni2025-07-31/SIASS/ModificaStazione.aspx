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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ModificaStazione.aspx.cs" Inherits="SIASS.ModificaStazione" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Modifica stazione</title>
    <style type="text/css">
        .checkboxlistformat label {
            margin-left: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <h1>Modifica stazione</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <div class="mb-3">
        Codice identificativo:
        <asp:Label ID="CodiceIdentificativoLabel" runat="server"></asp:Label>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label16" runat="server" Text="Descrizione:" AssociatedControlID="DescrizioneTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="DescrizioneTextBox" runat="server" CssClass="form-control" MaxLength="200" AutoCompleteType="Disabled"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Descrizione: Obbligatorio" ControlToValidate="DescrizioneTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
    </div>
    <div style="max-width: 200px">
        <asp:Label ID="Label10" runat="server" Text="Esclusa dal monitoraggio:" AssociatedControlID="EsclusaMonitoraggioCheckBox"></asp:Label>
        <asp:CheckBox ID="EsclusaMonitoraggioCheckBox" runat="server" CssClass="form-check" />
    </div>
    <div style="max-width: 200px">
        <asp:Label ID="Label8" runat="server" Text="Controllo anomalie:" AssociatedControlID="ControlloAnomalieCheckBox"></asp:Label>
        <asp:CheckBox ID="ControlloAnomalieCheckBox" runat="server" CssClass="form-check" />
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label4" runat="server" Text="Tipo:" AssociatedControlID="TipiStazioneDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiStazioneDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneTipoStazione" DataValueField="IdTipoStazione"></asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label1" runat="server" Text="Immagine stazione:" AssociatedControlID="AllegatiFotoStazioneDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="AllegatiFotoStazioneDropDownList" runat="server" CssClass="form-select" DataTextField="NomeFileDescrizioneAllegato" DataValueField="IdAllegatoStazione"></asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label2" runat="server" Text="Immagine mappa:" AssociatedControlID="AllegatiMappaDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="AllegatiMappaDropDownList" runat="server" CssClass="form-select" DataTextField="NomeFileDescrizioneAllegato" DataValueField="IdAllegatoStazione"></asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label5" runat="server" Text="Allestimento:" AssociatedControlID="TipiAllestimentoDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiAllestimentoDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE" DataValueField="DESCRIZIONE"></asp:DropDownList>
    </div>
    <div style="max-width: 200px">
        <asp:Label ID="Label6" runat="server" Text="Teletrasmissione:" AssociatedControlID="TeletrasmissioneCheckBox"></asp:Label>
        <asp:CheckBox ID="TeletrasmissioneCheckBox" runat="server" CssClass="form-check" />
    </div>
    <div style="max-width: 200px">
        <asp:Label ID="Label3" runat="server" Text="Punto di conformità:" AssociatedControlID="PuntoConformitaCheckBox"></asp:Label>
        <asp:CheckBox ID="PuntoConformitaCheckBox" runat="server" CssClass="form-check" />
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label7" runat="server" Text="Sito:" AssociatedControlID="SitiDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="SitiDropDownList" runat="server" CssClass="form-select" DataTextField="CodiceIdentificativoDescrizioneComuneProvincia" DataValueField="IdSito"></asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 500px;">
        <asp:Label ID="Label18" runat="server" Text="Annotazioni:" AssociatedControlID="AnnotazioniTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="AnnotazioniTextBox" runat="server" CssClass="form-control" MaxLength="1000" TextMode="MultiLine" Height="100px" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <h2>Rete di appartenenza</h2>
    <div class="mb-3" style="max-width: 500px;">
        <asp:CheckBoxList ID="ReteAppartenenzaCheckBoxList" runat="server" DataTextField="DESCRIZIONE" DataValueField="DESCRIZIONE" CssClass="checkboxlistformat"></asp:CheckBoxList>
    </div>
    <h2>Finalità</h2>
    <div class="mb-3" style="max-width: 500px;">
        <asp:CheckBoxList ID="FinalitaCheckBoxList" runat="server" DataTextField="DESCRIZIONE" DataValueField="DESCRIZIONE" CssClass="checkboxlistformat"></asp:CheckBoxList>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
        <button class="btn btn-danger" data-bs-target="#confermaEliminazioneModal" data-bs-toggle="modal" type="button">Elimina</button>
    </div>
    <div id="confermaEliminazioneModal" aria-hidden="true" aria-labelledby="confermaEliminazioneModal" class="modal fade" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Elimina stazione</h5>
                </div>
                <div class="modal-body">
                    Eliminare questa stazione?
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                    <asp:Button ID="EliminaButton" runat="server" Text="Elimina" CssClass="btn btn-danger" CausesValidation="false" OnClick="EliminaButton_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
