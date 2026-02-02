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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SIASS.Mobile.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Stazioni</h1>
    <div class="mb-3">
        <asp:Label ID="Label2" runat="server" Text="Ricerca per codice o descrizione:" CssClass="form-label"></asp:Label>
    </div>
    <div class="row mb-3" style="max-width: 500px;">
        <div class="col">
            <asp:TextBox ID="RicercaTextBox" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="col">
            <asp:Button ID="CercaButton" runat="server" Text="Cerca" OnClick="CercaButton_Click" CssClass="btn btn-primary" />
        </div>
    </div>
    <asp:GridView ID="ElencoStazioniGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessun risultato" CssClass="table table-bordered table-striped small" OnRowDataBound="ElencoStazioniGridView_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Stazioni">
                <ItemTemplate>
                    <asp:HyperLink ID="StazioneHyperLink" runat="server" NavigateUrl='<%# Eval("IdStazione", "../Stazione/Interventi/ElencoInterventi.aspx?IdStazione={0}") %>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
