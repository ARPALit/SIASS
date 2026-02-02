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

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="NuovoIntervento.aspx.cs" Inherits="SIASS.NuovoIntervento" %>

<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Nuovo intervento con prelievo di campioni</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
    <uc1:HeaderStazioneResponsive runat="server" ID="HeaderStazioneResponsive1" />
    <h1>Nuovo intervento</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <div class="row mb-3">
        <div class="col">
            <asp:Label ID="Label1" runat="server" Text="Tipo:" AssociatedControlID="TipiInterventoDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiInterventoDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE_TIPO_INTERVENTO" DataValueField="ID_TIPO_INTERVENTO"></asp:DropDownList>
        </div>
        <div class="col">
            <asp:Label ID="Label4" runat="server" Text="Matrice:" AssociatedControlID="TipiMatriceDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiMatriceDropDownList" runat="server" CssClass="form-select" DataTextField="Descrizione" DataValueField="Codice" AutoPostBack="True" OnSelectedIndexChanged="TipiMatriceDropDownList_SelectedIndexChanged"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Matrice: Obbligatorio" ControlToValidate="TipiMatriceDropDownList" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        </div>
    </div>
    <div class="row mb-3">
        <div class="col">
            <asp:Label ID="Label3" runat="server" Text="Argomento:" AssociatedControlID="TipiArgomentoDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiArgomentoDropDownList" runat="server" CssClass="form-select" DataTextField="Descrizione" DataValueField="Codice" AutoPostBack="True" OnSelectedIndexChanged="TipiArgomentoDropDownList_SelectedIndexChanged"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Argomento: Obbligatorio" ControlToValidate="TipiArgomentoDropDownList" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        </div>
        <div class="col-2">
            <asp:Label ID="Label2" runat="server" Text="Sede di accettazione:" AssociatedControlID="TipiSediDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiSediDropDownList" runat="server" CssClass="form-select" AutoPostBack="True" DataTextField="DENOMINAZIONE_SEDE" DataValueField="CODICE_SEDE" OnSelectedIndexChanged="TipiSediDropDownList_SelectedIndexChanged"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Sede: Obbligatorio" ControlToValidate="TipiSediDropDownList" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        </div>
    </div>
    <div class="row mb-3">
        <div class="col-2">
            <asp:Label ID="Label5" runat="server" Text="Data (gg/mm/aaaa):" AssociatedControlID="DataInterventoTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="DataInterventoTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="DataInterventoTextBox" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Data: Obbligatorio" ControlToValidate="DataInterventoTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="DataInterventoTextBox" CssClass="badge bg-danger" ErrorMessage="Data: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
        </div>
        <div class="col-2">
            <asp:Label ID="Label16" runat="server" Text="Ora inizio (hh:mm):" AssociatedControlID="OraInterventoTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="OraInterventoTextBox" runat="server" CssClass="form-control" MaxLength="5" AutoCompleteType="Disabled" type="time"></asp:TextBox>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="OraInterventoTextBox" CssClass="badge bg-danger" ErrorMessage="Ora: Formato hh:mm" ValidationExpression="^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$" Display="None">Formato hh:mm</asp:RegularExpressionValidator>
        </div>
        <div class="col-3">
            <asp:Label ID="Label6" runat="server" Text="Richiedente:" AssociatedControlID="TipiRichiedenteDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiRichiedenteDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE_TIPO_RICHIEDENTE" DataValueField="ID_TIPO_RICHIEDENTE"></asp:DropDownList>
        </div>
        <div class="col-1">
            <asp:Label ID="Label7" runat="server" Text="Codice campagna:" AssociatedControlID="CodiceCampagnaTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="CodiceCampagnaTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
        </div>
    </div>
    <h2>Pacchetti</h2>
    <asp:GridView ID="PacchettiGridView" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered small w-auto" DataKeyNames="PackIdentity">
        <Columns>
            <asp:TemplateField>
                <HeaderTemplate>
                    <script type="text/javascript">
                        function SelezionaTutteNessuna() {
                            var checkSpan = document.getElementsByClassName("checkTabellaPacchetti");
                            for (i = 0; i < checkSpan.length; i++) {
                                checkSpan[i].getElementsByTagName("input")[0].checked = document.getElementById("SelezionaTutteChk").checked;
                            }
                        }
                    </script>
                    <input id="SelezionaTutteChk" name="SelezionaTutteChk" type="checkbox" onclick="SelezionaTutteNessuna()" title="Seleziona tutti/nessuno" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="SelezionatoCheckBox" runat="server" CssClass="checkTabellaPacchetti" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PackIdentity" HeaderText="Codice" />
            <asp:BoundField DataField="PackName" HeaderText="Descrizione" />
        </Columns>
    </asp:GridView>
    <div class="row mb-3">
        <div class="col-3 mx-2 border border-1">
            <asp:UpdatePanel ID="OperatoriUpdatePanel" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AggiungiOperatoreButton" />
                </Triggers>
                <ContentTemplate>
                    <div class="p-2">
                        <h2>Operatori</h2>
                        <asp:GridView ID="OperatoriInterventoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="IdOperatore" EmptyDataText="Nessun operatore assegnato a questo intervento" ShowHeader="False" CssClass="table table-bordered table-striped table-hover table-sm w-auto" OnSelectedIndexChanged="OperatoriInterventoGridView_SelectedIndexChanged">
                            <Columns>
                                <asp:BoundField DataField="DescrizioneOperatore" />
                                <asp:CommandField SelectText="Rimuovi" ShowSelectButton="True" ControlStyle-CssClass="link-danger">
                                    <ItemStyle Width="1%" />
                                </asp:CommandField>
                            </Columns>
                        </asp:GridView>
                        <div class="row">
                            <div class="col">
                                <asp:DropDownList ID="OperatoriPerInterventoDropDownList" runat="server" DataTextField="DescrizioneOperatore" DataValueField="IdOperatore" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col">
                                <asp:Button ID="AggiungiOperatoreButton" runat="server" CssClass="btn btn-primary" Text="Aggiungi" CausesValidation="false" OnClick="AggiungiOperatoreButton_Click" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Crea intervento" CssClass="btn btn-success" OnClientClick="this.disabled=true; this.value='Attendere...';" UseSubmitBehavior="False" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
    </div>
</asp:Content>
