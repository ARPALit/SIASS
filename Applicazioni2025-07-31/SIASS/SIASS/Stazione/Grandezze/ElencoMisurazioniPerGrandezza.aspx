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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoMisurazioniPerGrandezza.aspx.cs" Inherits="SIASS.ElencoMisurazioniPerGrandezza" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Misurazioni</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>
        <asp:Label ID="GrandezzaLabel" runat="server"></asp:Label></h1>
    <p>
        <asp:HyperLink ID="ElencoGrandezzeHyperLink" runat="server">&lt;&lt; Grandezze e misurazioni</asp:HyperLink>
    </p>
    <p>
        <asp:Label ID="DateMisurazioniLabel" runat="server"></asp:Label>
    </p>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <p>
        Visualizza dati
        <asp:Label ID="Label1" runat="server" Text="dal:" AssociatedControlID="DataInizioTextBox"></asp:Label>
        <asp:TextBox ID="DataInizioTextBox" runat="server" MaxLength="10" Width="10em" AutoCompleteType="Disabled"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="DataInizioTextBox" />
        <asp:Label ID="Label2" runat="server" Text="al:" AssociatedControlID="DataFineTextBox"></asp:Label>
        <asp:TextBox ID="DataFineTextBox" runat="server" MaxLength="10" Width="10em" AutoCompleteType="Disabled"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="DataFineTextBox" />
        <asp:Label ID="Label4" runat="server" Text="Stato validazione:" AssociatedControlID="StatoValidazioneDropDownList"></asp:Label>
        <asp:DropDownList ID="StatoValidazioneDropDownList" runat="server">
            <asp:ListItem Text="Tutti" Value=""></asp:ListItem>
            <asp:ListItem Text="Da validare" Value="0"></asp:ListItem>
            <asp:ListItem Text="Valida" Value="1"></asp:ListItem>
            <asp:ListItem Text="Non valida" Value="2"></asp:ListItem>
        </asp:DropDownList>
        <asp:CheckBox ID="VisualizzaGraficoCheckBox" runat="server" />
        <asp:Label ID="Label3" runat="server" Text="Mostra grafico" AssociatedControlID="VisualizzaGraficoCheckBox"></asp:Label>
    </p>
    <asp:UpdatePanel ID="OpzioniRicercaUpdatePanel" runat="server" RenderMode="Inline">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="EvidenziaSpikeCheckBox" />
            <asp:AsyncPostBackTrigger ControlID="EvidenziaPlateauCheckBox" />
        </Triggers>
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="Valore minimo:" AssociatedControlID="ValoreMinimoTextBox"></asp:Label>
                        <asp:TextBox ID="ValoreMinimoTextBox" runat="server" Width="10em" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                    </td>
                    <td style="padding-left: 3em;">
                        <asp:Label ID="Label10" runat="server" Text="Valore massimo:" AssociatedControlID="ValoreMassimoTextBox"></asp:Label>
                        <asp:TextBox ID="ValoreMassimoTextBox" runat="server" Width="10em" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="EvidenziaSpikeCheckBox" runat="server" AutoPostBack="True" OnCheckedChanged="EvidenziaSpikeCheckBox_CheckedChanged" />
                        <asp:Label ID="Label5" runat="server" Text="Evidenzia spike" AssociatedControlID="EvidenziaSpikeCheckBox"></asp:Label>
                    </td>
                    <td style="padding-left: 3em;">
                        <asp:Label ID="Label7" runat="server" Text="Moltiplicatore deviazione standard:" AssociatedControlID="MoltiplicatoreDeviazioneStandardTextBox"></asp:Label>
                        <asp:TextBox ID="MoltiplicatoreDeviazioneStandardTextBox" runat="server" Width="3em" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                        &nbsp;
                        <asp:Label ID="SogliaSpikeLabel" runat="server"></asp:Label>
                        &nbsp;
                        <asp:Label ID="ValoreMedioLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="EvidenziaPlateauCheckBox" runat="server" AutoPostBack="True" OnCheckedChanged="EvidenziaPlateauCheckBox_CheckedChanged" />
                        <asp:Label ID="Label6" runat="server" Text="Evidenzia plateau" AssociatedControlID="EvidenziaPlateauCheckBox"></asp:Label>
                    </td>
                    <td style="padding-left: 3em;">
                        <asp:Label ID="Label8" runat="server" Text="Dimensione plateau:" AssociatedControlID="DimensionePlateauTextBox"></asp:Label>
                        <asp:TextBox ID="DimensionePlateauTextBox" runat="server" Width="3em" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:RangeValidator ID="MoltiplicatoreDeviazioneStandardRangeValidator" runat="server" ErrorMessage="Moltiplicatore deviazione standard: Deve essere un numero" ControlToValidate="MoltiplicatoreDeviazioneStandardTextBox" MaximumValue="10" MinimumValue="0" Type="Double" Display="None" Enabled="false">Deve essere un numero intero</asp:RangeValidator>
            <asp:RequiredFieldValidator ID="MoltiplicatoreDeviazioneStandardRequiredFieldValidator" runat="server" ControlToValidate="MoltiplicatoreDeviazioneStandardTextBox" Display="None" ErrorMessage="Moltiplicatore deviazione standard obbligatorio">Moltiplicatore deviazione standard obbligatorio</asp:RequiredFieldValidator>
            <asp:RangeValidator ID="DimensionePlateauRangeValidator" runat="server" ErrorMessage="Dimensione plateau: Deve essere un numero intero" ControlToValidate="DimensionePlateauTextBox" MaximumValue="1000" MinimumValue="2" Type="Integer" Display="None" Enabled="false">Deve essere un numero</asp:RangeValidator>
            <asp:RequiredFieldValidator ID="DimensionePlateauRequiredFieldValidator" runat="server" ControlToValidate="DimensionePlateauTextBox" Display="None" ErrorMessage="Dimensione plateau obbligatoria">Dimensione plateau obbligatoria</asp:RequiredFieldValidator>
            <asp:RangeValidator ID="ValoreMinimoRangeValidator" runat="server" ErrorMessage="Valore minimo: Deve essere un numero" ControlToValidate="ValoreMinimoTextBox" MaximumValue="1000000000" MinimumValue="-1000000000" Type="Double" Display="None">Deve essere un numero</asp:RangeValidator>
            <asp:RangeValidator ID="ValoreMassimoRangeValidator" runat="server" ErrorMessage="Valore massimo: Deve essere un numero" ControlToValidate="ValoreMassimoTextBox" MaximumValue="1000000000" MinimumValue="-1000000000" Type="Double" Display="None">Deve essere un numero</asp:RangeValidator>
            <p>
                <a data-bs-toggle="collapse" href="#collapseIstruzioni" role="button" aria-expanded="false" aria-controls="collapseIstruzioni">Dettagli</a>
            </p>
            <div class="collapse mb-3" id="collapseIstruzioni">
                <div class="card card-body">
                    <asp:Label ID="IstruzioniLabel" runat="server"></asp:Label>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <p>
        <asp:Button ID="AggiornaButton" runat="server" Text="Visualizza" OnClick="AggiornaButton_Click" CssClass="btn btn-primary" />
    </p>
    <asp:RegularExpressionValidator ID="DataInizioRegularExpressionValidator" runat="server" ControlToValidate="DataInizioTextBox"
        ErrorMessage="Data iniziale non valida"
        ForeColor="" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Data iniziale non valida</asp:RegularExpressionValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="DataInizioTextBox" Display="None" ErrorMessage="Data iniziale obbligatoria">Data iniziale obbligatoria</asp:RequiredFieldValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="DataFineTextBox"
        ErrorMessage="Data finale non valida"
        ForeColor="" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Data finale non valida</asp:RegularExpressionValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="DataFineTextBox" Display="Dynamic" ErrorMessage="Data finale obbligatoria">Data finale obbligatoria</asp:RequiredFieldValidator>
    <p>
        <asp:Label ID="AvvisoLabel" runat="server"></asp:Label>
    </p>
    <div id="messaggioAttesa" style="display: block; padding: 20px 0 0 200px;">
        <img src="../../img/Loading.gif" />
    </div>
    <table id="tabellaRisultati" style="display: none;">
        <tr>
            <td style="vertical-align: top;">
                <asp:GridView ID="ElencoMisurazioniGridView" runat="server" EmptyDataText="Nessuna misurazione trovata per i criteri specificati" DataKeyNames="IdMisurazione" OnRowDataBound="ElencoMisurazioniGridView_RowDataBound" AutoGenerateColumns="False" CssClass="table table-bordered table-striped table-hover table-sm w-auto">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <script type="text/javascript">
                                    function SelezionaTutteNessuna() {
                                        var checkSpan = document.getElementsByClassName("checkTabellaMisurazioni");
                                        for (i = 0; i < checkSpan.length; i++) {
                                            checkSpan[i].getElementsByTagName("input")[0].checked = document.getElementById("SelezionaTutteChk").checked;
                                        }
                                    }
                                </script>
                                <input id="SelezionaTutteChk" name="SelezionaTutteChk" type="checkbox" onclick="SelezionaTutteNessuna()" title="Seleziona tutte/nessuna" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="SelezionaCheckBox" runat="server" CssClass="checkTabellaMisurazioni" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataMisurazione" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderText="Data" HtmlEncode="False" />
                        <asp:TemplateField HeaderText="Valore">
                            <ItemTemplate>
                                <asp:Label ID="ValoreLabel" runat="server" Text='<%# Eval("Valore", "{0:0.000}")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Stato">
                            <ItemTemplate>
                                <asp:Label ID="StatoLabel" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Spike/Plateau" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <strong>
                                    <asp:Label ID="SpikePlateauLabel" runat="server" ForeColor="Red"></asp:Label></strong>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Panel ID="OperazioniSuSelezionatePanel" runat="server">
                    <p>
                        Segna selezionate come 
                <asp:DropDownList ID="NuovoStatoDropDownList" runat="server">
                    <asp:ListItem Text="Da validare" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Valida" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Non valida" Value="2"></asp:ListItem>
                </asp:DropDownList>
                        <asp:Button ID="OkButton" runat="server" Text="Ok" OnClick="OkButton_Click" CssClass="btn btn-success" />
                    </p>
                </asp:Panel>
            </td>
            <td style="vertical-align: top; padding-left: 50px;">
                <asp:Panel ID="GraficoPanel" runat="server" Width="1200px" Height="640px">
                    <div class="chart-container" style="position: relative; height: 80vh; width: 90%">
                        <canvas id="Grafico"></canvas>
                    </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        document.getElementById("messaggioAttesa").style.display = "none";
        document.getElementById("tabellaRisultati").style.display = "block";
    </script>
</asp:Content>
