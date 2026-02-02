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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="SelezioneProfilo.aspx.cs" Inherits="SIASS.SelezioneProfilo" %>

<%@ MasterType VirtualPath="~/MasterPageResponsive.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Selezione profilo</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <h1>Selezione profilo</h1>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label4" runat="server" Text="Profilo attivo:"></asp:Label>
        <asp:Label ID="ProfiloAttivoLabel" runat="server"></asp:Label>
    </div>
    <div class="row mb-3">
        <div class="col-3">
            <asp:Label ID="Label1" runat="server" AssociatedControlID="ElencoOrganizzazioniDropDownList" CssClass="form-label">Organizzazione:</asp:Label>
            <asp:DropDownList ID="ElencoOrganizzazioniDropDownList" runat="server" DataTextField="RagioneSociale" DataValueField="Codice" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="ElencoOrganizzazioniDropDownList_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
    </div>
    <div class="row mb-3">
        <div class="col-3">
            <asp:Label ID="Label2" runat="server" AssociatedControlID="ElencoProfiliDropDownList" CssClass="form-label">Profilo:</asp:Label>
            <asp:DropDownList ID="ElencoProfiliDropDownList" runat="server" DataTextField="Descrizione" DataValueField="Codice" CssClass="form-select">
            </asp:DropDownList>
        </div>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Button ID="SelezioneProfiloButton" runat="server" CssClass="btn btn-success" Text="Imposta profilo" OnClick="SelezioneProfiloButton_Click" />
    </div>
</asp:Content>
