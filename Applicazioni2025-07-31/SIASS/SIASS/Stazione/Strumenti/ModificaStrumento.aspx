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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ModificaStrumento.aspx.cs" Inherits="SIASS.ModificaStrumento" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Strumento</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Modifica strumento</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label7" runat="server" Text="Tipo:" CssClass="form-label"></asp:Label>
        <asp:Label ID="DescrizioneTipoStrumentoLabel" runat="server" CssClass="form-label"></asp:Label>
    </div>
    <asp:Panel ID="DatiStrumentoNonVisibileInterventoPanel" runat="server">
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label11" runat="server" Text="Numero di serie:" AssociatedControlID="NumeroDiSerieTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="NumeroDiSerieTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label1" runat="server" Text="Marca:" AssociatedControlID="MarcaTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="MarcaTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label3" runat="server" Text="Modello:" AssociatedControlID="ModelloTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="ModelloTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label4" runat="server" Text="Numero inventario Arpal:" AssociatedControlID="NumeroInventarioArpalTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="NumeroInventarioArpalTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label5" runat="server" Text="Caratteristiche:" AssociatedControlID="CaratteristicheTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="CaratteristicheTextBox" runat="server" CssClass="form-control" MaxLength="2000" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label6" runat="server" Text="Codice sistema gestionale:" AssociatedControlID="CodiceSistemaGestionaleTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="CodiceSistemaGestionaleTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label12" runat="server" Text="Inizio validità (gg/mm/aaaa):" AssociatedControlID="InizioValiditaTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="InizioValiditaTextBox" runat="server" MaxLength="10" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="InizioValiditaTextBox" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Inizio validità: Obbligatorio" ControlToValidate="InizioValiditaTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="InizioValiditaTextBox" CssClass="badge bg-danger" ErrorMessage="Inizio validità: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
        </div>
        <div class="mb-3" style="max-width: 500px">
            <asp:Label ID="Label13" runat="server" Text="Fine validità (gg/mm/aaaa):" AssociatedControlID="FineValiditaTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="FineValiditaTextBox" runat="server" MaxLength="10" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="FineValiditaTextBox" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="FineValiditaTextBox" CssClass="badge bg-danger" ErrorMessage="Fine validità: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
        </div>
    </asp:Panel>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
    </div>
</asp:Content>
