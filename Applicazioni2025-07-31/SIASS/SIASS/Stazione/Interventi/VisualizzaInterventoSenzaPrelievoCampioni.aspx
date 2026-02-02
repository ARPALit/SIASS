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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="VisualizzaInterventoSenzaPrelievoCampioni.aspx.cs" Inherits="SIASS.VisualizzaInterventoSenzaPrelievoCampioni" %>

<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Visualizza intervento senza prelievo di campioni</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive runat="server" ID="HeaderStazioneResponsive1" />
    <h1>Visualizza intervento</h1>
    <div class="mb-3 p-2 border border-1" style="background-color: #f0f0f0">
        <div class="row">
            <div class="col-6">
                Tipo:
                    <asp:Label ID="TipoInterventoLabel" runat="server"></asp:Label>
            </div>
            <div class="col-3">
                Sigla verbale:
                    <asp:Label ID="SiglaVerbaleLabel" runat="server"></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-6">
                Argomento:
                    <asp:Label ID="ArgomentoLabel" runat="server"></asp:Label>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-3">
            <asp:Label ID="Label2" runat="server" Text="Richiedente:"></asp:Label>
            <asp:Label ID="RichiedenteLabel" runat="server"></asp:Label>
        </div>
        <div class="col-3">
            <asp:Label ID="Label5" runat="server" Text="Data:"></asp:Label>
            <asp:Label ID="DataInterventoLabel" runat="server"></asp:Label>
        </div>
        <div class="col-2">
            <asp:Label ID="Label16" runat="server" Text="Ora inizio:"></asp:Label>
            <asp:Label ID="OraInterventoLabel" runat="server"></asp:Label>
        </div>
        <div class="col-2">
            <asp:Label ID="Label6" runat="server" Text="Durata (minuti):"></asp:Label>
            <asp:Label ID="DurataInterventoLabel" runat="server"></asp:Label>
        </div>
        <div class="col-2">
            <asp:Label ID="Label24" runat="server" Text="Ora fine:"></asp:Label>
            <asp:Label ID="OraFineInterventoLabel" runat="server"></asp:Label>
        </div>
    </div>

    <div class="row">
        <div class="col-1">
            <asp:Label ID="Label7" runat="server" Text="Codice campagna:"></asp:Label>
            <asp:Label ID="CodiceCampagnaLabel" runat="server"></asp:Label>
        </div>
        <div class="col-1">
            <asp:Label ID="Label25" runat="server" Text="Quota campione:"></asp:Label>
            <asp:Label ID="QuotaCampioneLabel" runat="server"></asp:Label>
        </div>
    </div>

    <div class="row mb-3">
        <asp:Panel ID="FileDatiPanel" runat="server" CssClass="col-3">
            <asp:Label ID="Label8" runat="server" Text="File dati:"></asp:Label>
            <asp:Label ID="FileDatiLabel" runat="server"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="TipiStrumentoInterventoPanel" runat="server" CssClass="col-3">
            <asp:Label ID="Label3" runat="server" Text="Strumento usato:"></asp:Label>
            <asp:Label ID="StrumentoInterventoLabel" runat="server"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="FileAngoliPanel" runat="server" CssClass="col-3">
            <asp:Label ID="Label14" runat="server" Text="File angoli:"></asp:Label>
            <asp:Label ID="FileAngoliLabel" runat="server"></asp:Label>
        </asp:Panel>
    </div>

    <div class="mb-3 p-2 border border-1">
        <h2>Tecnico di parte</h2>
        <div class="row mb-3">
            <div class="col">
                <asp:Label ID="Label20" runat="server" Text="Nome del tecnico:"></asp:Label>
                <asp:Label ID="ParteNomeTecnicoLabel" runat="server"></asp:Label>
            </div>
            <div class="col">
                <asp:Label ID="Label21" runat="server" Text="Azienda del tecnico:"></asp:Label>
                <asp:Label ID="ParteAziendaTecnicoLabel" runat="server"></asp:Label>
            </div>
        </div>
        <div class="row mb-3">
            <div class="col">
                <asp:Label ID="Label22" runat="server" Text="Ruolo del tecnico:"></asp:Label>
                <asp:Label ID="ParteRuoloTecnicoLabel" runat="server"></asp:Label>
            </div>
            <div class="col">
                <asp:Label ID="Label23" runat="server" Text="Contatti:"></asp:Label>
                <asp:Label ID="ParteContattiLabel" runat="server"></asp:Label>
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <div class="col mx-2 border border-1">
            <div class="p-2">
                <h2>Operatori</h2>
                <asp:GridView ID="OperatoriInterventoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="IdOperatore" EmptyDataText="Nessun operatore assegnato a questo intervento" ShowHeader="False" CssClass="table table-bordered table-striped table-hover table-sm w-auto">
                    <Columns>
                        <asp:BoundField DataField="DescrizioneOperatore" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="col mx-2 border border-1">
            <div class="p-2">
                <h2>Operatori a supporto</h2>
                <asp:GridView ID="OperatoriSupportoInterventoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="IdOperatore" EmptyDataText="Nessun operatore a supporto assegnato a questo intervento" ShowHeader="False" CssClass="table table-bordered table-striped table-hover table-sm w-auto">
                    <Columns>
                        <asp:BoundField DataField="DescrizioneOperatore" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <div class="col my-2 border border-1">
        <div class="p-2">
            <h2>Misurazioni</h2>
            <asp:Repeater ID="MisurazioniRepeater" runat="server" OnItemDataBound="MisurazioniRepeater_ItemDataBound">
                <HeaderTemplate>
                    <table>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="GrandezzaEUnitaMisuraLabel" runat="server" Text='<%# Eval("GrandezzaEUnitaMisura") + ":" %>'></asp:Label></td>
                        <td>
                            <asp:Label ID="ValoreMisurazioneLabel" runat="server"></asp:Label>
                            <asp:Label ID="ValoreMisurazioneBooleanaLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fonte ARPAL:"></asp:Label>
                            <asp:Label ID="FonteArpalLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td>
                            <small>
                                <asp:Label ID="defaultItem" runat="server" Visible='<%# MisurazioniRepeater.Items.Count == 0 %>' Text="Nessuna misurazione." /></small>
                        </td>
                    </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <div class="row mb-3">
        <div class="col">
            <asp:Label ID="Label18" runat="server" Text="Annotazioni:"></asp:Label>
            <asp:Label ID="AnnotazioniLabel" runat="server" Text="Annotazioni:"></asp:Label>
        </div>
    </div>

    <div class="my-lg-3">
        <asp:Button ID="ChiudiButton" runat="server" Text="Chiudi" CssClass="btn btn-primary" CausesValidation="false" OnClick="ChiudiButton_Click" />
    </div>

</asp:Content>
