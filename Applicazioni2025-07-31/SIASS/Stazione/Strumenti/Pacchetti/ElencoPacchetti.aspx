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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoPacchetti.aspx.cs" Inherits="SIASS.ElencoPacchetti" %>

<%@ Register Src="../../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Pacchetti strumento</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Pacchetti ALIMS</h1>
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
    <div class="mb-3">
        <asp:UpdatePanel ID="PacchettiUpdatePanel" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="AggiungiPacchettoButton" />
            </Triggers>
            <ContentTemplate>
                <div class="mb-3" style="max-width: 500px">
                    <asp:GridView ID="PacchettiStrumentoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="IdPacchetto" EmptyDataText="Nessun pacchetto assegnato a questo strumento" ShowHeader="False" CssClass="table table-bordered table-striped table-sm table-hover" OnSelectedIndexChanged="PacchettiStrumentoGridView_SelectedIndexChanged">
                        <Columns>
                            <asp:BoundField DataField="DescrizionePacchetto" />
                            <asp:CommandField SelectText="Rimuovi" ShowSelectButton="True">
                                <ItemStyle Width="1%" />
                            </asp:CommandField>
                        </Columns>
                    </asp:GridView>
                    <div class="row">
                        <div class="col">
                            <asp:DropDownList ID="TipiPacchettoDropDownList" runat="server" DataTextField="DescrizionePacchetto" DataValueField="IdPacchetto" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col">
                            <asp:Button ID="AggiungiPacchettoButton" runat="server" CssClass="btn btn-primary" Text="Aggiungi" CausesValidation="false" OnClick="AggiungiPacchettoButton_Click" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
    </div>
</asp:Content>
