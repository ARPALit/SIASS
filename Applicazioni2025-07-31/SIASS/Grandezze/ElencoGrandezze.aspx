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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoGrandezze.aspx.cs" Inherits="SIASS.ElencoGrandezze" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Grandezze e misurazioni</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <div class="row">
        <asp:Panel ID="GrandezzeStazionePanel" runat="server" CssClass="col-5">
            <h1>Grandezze stazione</h1>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
            <p>
                <asp:HyperLink ID="NuovaGrandezzaHyperLink" runat="server">Nuova grandezza</asp:HyperLink>
            </p>
            <div class="mb-3">
                <asp:GridView ID="GrandezzeStazioneGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="ID_GRANDEZZA_STAZIONE" EmptyDataText="Nessuna grandezza assegnata a questa stazione" CssClass="table table-bordered table-striped table-hover table-sm w-auto" OnSelectedIndexChanged="GrandezzeStazioneGridView_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="GRANDEZZA" HeaderText="Grandezza" />
                        <asp:BoundField DataField="UNITA_MISURA" HeaderText="Unità di misura" />
                        <asp:BoundField DataField="NUMERO_DECIMALI" HeaderText="Numero decimali" />
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="Rimuovi" OnClientClick="return confirm('Rimuovere questa grandezza dalla stazione?');"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
        <div class="col-5">
            <h1>Misurazioni</h1>
            <asp:Button ID="MostraDatiMisurazioniButton" runat="server" Text="Mostra informazioni validazione" CssClass="btn btn-primary mb-3" OnClick="MostraDatiMisurazioniButton_Click" />
            <asp:Label ID="MostraDatiMisurazioniLabel" runat="server" Text="(può richiedere alcuni minuti)" CssClass="mb-3"></asp:Label>
            <asp:GridView ID="GrandezzeMisurazioniGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="ID_GRANDEZZA_STAZIONE" EmptyDataText="Nessuna grandezza assegnata a questa stazione" CssClass="table table-bordered table-striped table-hover table-sm w-auto" OnSelectedIndexChanged="GrandezzeStazioneGridView_SelectedIndexChanged" Visible="True" OnRowDataBound="GrandezzeMisurazioniGridView_RowDataBound">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="ID_GRANDEZZA_STAZIONE" DataNavigateUrlFormatString="ElencoMisurazioniPerGrandezza.aspx?IdGrandezzaStazione={0}" DataTextField="GRANDEZZA" HeaderText="Grandezza" />
                    <asp:BoundField DataField="UNITA_MISURA" HeaderText="Unità di misura" />
                    <asp:BoundField DataField="DATA_MISURAZIONE" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Ultima misurazione" HtmlEncode="false" />
                    <asp:TemplateField HeaderText="Validazione ultima misurazione">
                        <ItemTemplate>
                            <asp:Label ID="ValidataLabel" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
