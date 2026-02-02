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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoInterventiPeriodo.aspx.cs" Inherits="SIASS.ElencoInterventiPeriodo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Interventi</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <h1>Interventi</h1>
    <div class="row g-3 mb-3 small">
        <div class="col-md-2">
            <asp:Label ID="Label11" runat="server" Text="Inizio periodo (gg/mm/aaaa):" AssociatedControlID="DataInizioPeriodoTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="DataInizioPeriodoTextBox" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="DataInizioPeriodoTextBox" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Inizio periodo: Obbligatorio" ControlToValidate="DataInizioPeriodoTextBox" CssClass="badge bg-danger" Display="Dynamic">Obbligatorio</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="DataInizioPeriodoTextBox" CssClass="badge bg-danger" ErrorMessage="Inizio periodo: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="Dynamic">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
        </div>
        <div class="col-md-2">
            <asp:Label ID="Label12" runat="server" Text="Fine periodo (gg/mm/aaaa):" AssociatedControlID="DataFinePeriodoTextBox" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="DataFinePeriodoTextBox" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="DataFinePeriodoTextBox" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Fine periodo: Obbligatorio" ControlToValidate="DataFinePeriodoTextBox" CssClass="badge bg-danger" Display="Dynamic">Obbligatorio</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="DataFinePeriodoTextBox" CssClass="badge bg-danger" ErrorMessage="Fine periodo: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="Dynamic">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
        </div>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="VisualizzaButton" runat="server" Text="Visualizza" CssClass="btn btn-primary" OnClick="VisualizzaButton_Click" />
    </div>
    <asp:GridView ID="ElencoInterventiPeriodoGridView" runat="server" CssClass="table table-bordered table-striped table-hover small w-auto" EmptyDataText="Nessun intervento nel periodo selezionato" AutoGenerateColumns="False">
        <Columns>
                    <asp:TemplateField HeaderText="Identificativo stazione">
                        <ItemTemplate>
                            <asp:HyperLink ID="VisualizzaStazioneHyperLink" runat="server" NavigateUrl='<%# Eval("IdStazione", "~/Stazione/VisualizzaStazione.aspx?IdStazione={0}") %>' Text='<%# Bind("CodiceIdentificativoStazione") %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DescrizioneStazione" HeaderText="Descrizione" />
                    <asp:BoundField DataField="DescrizioneTipoIntervento" HeaderText="Tipo intervento" />
            <asp:HyperLinkField DataNavigateUrlFields="IdIntervento" DataNavigateUrlFormatString="~/Stazione/Interventi/ModificaIntervento.aspx?IdIntervento={0}" DataTextField="DataIntervento" DataTextFormatString="{0:dd/MM/yyyy}" HeaderText="Data" />
                    <asp:BoundField DataField="OraIntervento" HeaderText="Ora" />
                    <asp:BoundField DataField="DurataIntervento" HeaderText="Durata" />
        </Columns>
    </asp:GridView>
</asp:Content>
