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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="NuovoAllegatoStazione.aspx.cs" Inherits="SIASS.NuovoAllegatoStazione" %>
<%@ Register src="../HeaderStazioneResponsive.ascx" tagname="HeaderStazioneResponsive" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Allegato stazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Nuovo allegato</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowModelStateErrors="true" CssClass="alert alert-danger"></asp:ValidationSummary>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label1" runat="server" Text="File:"></asp:Label>
        <asp:FileUpload ID="NomeFileAllegatoFileUpload" runat="server" CssClass="form-control" />
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label5" runat="server" Text="Descrizione:"></asp:Label>
        <asp:TextBox ID="DescrizioneAllegatoTextBox" runat="server" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <div class="mb-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" OnClick="SalvaButton_Click" CssClass="btn btn-success" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CausesValidation="False" CssClass="btn btn-primary" OnClick="AnnullaButton_Click" />
    </div>
</asp:Content>
