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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ImportazioniOrganoclorurati.aspx.cs" Inherits="SIASS.ImportazioniOrganoclorurati" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Organoclorurati</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Importazione dati organoclorurati</h1>
    <div class="mb-3">
        Questa funzione consente l'importazione dei dati sugli organoclorurati. E' disponibile un <a href="Template.xlsx" target="_blank">esempio di file di importazione</a>.
    </div>
    <div class="mb-3">
        <asp:Button ID="NuovaImportazioneButton" runat="server" CausesValidation="False" CssClass="btn btn-success" Text="Nuova importazione" OnClick="NuovaImportazioneButton_Click" />
    </div>
    <asp:Panel ID="ElencoImportazioniPanel" runat="server" CssClass="mb-3">
        <div class="row g-3 mb-3 small">
            <div class="col-md-3">
                <asp:Label ID="Label1" runat="server" Text="Visualizza importazioni nello stato:" AssociatedControlID="StatiDropDownList" CssClass="form-label"></asp:Label>
                <asp:DropDownList ID="StatiDropDownList" runat="server" CssClass="form-select" DataValueField="STATO" DataTextField="NUMERO_PER_STATO"></asp:DropDownList>
            </div>
        </div>
        <asp:Button ID="VisualizzaButton" runat="server" Text="Visualizza" CssClass="btn btn-primary" OnClick="VisualizzaButton_Click" />
    </asp:Panel>
    <asp:GridView ID="ImportazioniGridView" runat="server" CssClass="table table-bordered table-striped table-hover small w-auto" AutoGenerateColumns="False" AllowPaging="True" EmptyDataText="Nessuna importazione trovata" OnPageIndexChanging="ImportazioniGridView_PageIndexChanging" PageSize="20" OnRowDataBound="ImportazioniGridView_RowDataBound">
        <Columns>
            <asp:BoundField DataField="NOME_FILE" HeaderText="File" />
            <asp:BoundField DataField="STATO" HeaderText="Stato" />
            <asp:BoundField DataField="OPERATORE" HeaderText="Operatore" />
            <asp:BoundField DataField="DATA_RICEZIONE_FILE" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderText="Data ricezione file" HtmlEncode="False" />
            <asp:TemplateField HeaderText="Rapporto">
                <ItemTemplate>
                    <asp:HyperLink ID="RapportoHyperLink" runat="server" Text="Visualizza" Target="_blank"></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
