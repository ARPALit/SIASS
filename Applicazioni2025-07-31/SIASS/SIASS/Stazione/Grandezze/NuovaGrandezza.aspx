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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="NuovaGrandezza.aspx.cs" Inherits="SIASS.NuovaGrandezza" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Grandezza stazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Nuova grandezza</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label2" runat="server" Text="Tipo:" AssociatedControlID="TipiGrandezzaDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiGrandezzaDropDownList" runat="server" DataTextField="NOME_GRANDEZZA" DataValueField="NOME_GRANDEZZA" CssClass="form-select">
        </asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label1" runat="server" Text="Unità di misura:" AssociatedControlID="TipiUnitaMisuraDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiUnitaMisuraDropDownList" runat="server" DataTextField="NOME_UNITA_MISURA" DataValueField="NOME_UNITA_MISURA" CssClass="form-select">
        </asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label3" runat="server" Text="Numero decimali da utilizzare nell'inserimento dell'intervento:" AssociatedControlID="NumeroDecimaliDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="NumeroDecimaliDropDownList" runat="server" CssClass="form-select">
        </asp:DropDownList>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
    </div>
</asp:Content>
