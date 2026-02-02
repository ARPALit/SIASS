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

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ModificaInterventoSenzaPrelievoCampioni.aspx.cs" Inherits="SIASS.ModificaInterventoSenzaPrelievoCampioni" %>

<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Modifica intervento senza prelievo di campioni</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"></asp:ScriptManager>
    <uc1:HeaderStazioneResponsive runat="server" ID="HeaderStazioneResponsive1" />
    <h1>Modifica intervento</h1>
    <asp:MultiView ID="ModificaInterventoMultiView" runat="server">
        <asp:View ID="InterventoView" runat="server">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
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
                    <asp:Label ID="Label2" runat="server" Text="Richiedente:" AssociatedControlID="TipiRichiedenteDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="TipiRichiedenteDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE_TIPO_RICHIEDENTE" DataValueField="ID_TIPO_RICHIEDENTE"></asp:DropDownList>
                </div>
                <div class="col-3">
                    <asp:Label ID="Label5" runat="server" Text="Data:" AssociatedControlID="DataInterventoTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="DataInterventoTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="DataInterventoTextBox" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Data: Obbligatorio" ControlToValidate="DataInterventoTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="DataInterventoTextBox" CssClass="badge bg-danger" ErrorMessage="Data: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
                </div>
                <div class="col-2">
                    <asp:Label ID="Label16" runat="server" Text="Ora inizio:" AssociatedControlID="OraInterventoTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="OraInterventoTextBox" runat="server" CssClass="form-control" MaxLength="5" AutoCompleteType="Disabled" type="time"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="OraInterventoTextBox" CssClass="badge bg-danger" ErrorMessage="Ora: Formato hh:mm" ValidationExpression="^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$" Display="None">Formato hh:mm</asp:RegularExpressionValidator>
                </div>
                <div class="col-2">
                    <asp:Label ID="Label6" runat="server" Text="Durata (minuti):" AssociatedControlID="DurataInterventoTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="DurataInterventoTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Durata: Deve essere un numero intero" ControlToValidate="DurataInterventoTextBox" CssClass="badge bg-danger" MaximumValue="1440" MinimumValue="0" Type="Integer" Display="None">Deve essere un numero intero</asp:RangeValidator>
                </div>
                <div class="col-2">
                    <asp:Label ID="Label24" runat="server" Text="Ora fine:" AssociatedControlID="OraFineInterventoTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="OraFineInterventoTextBox" runat="server" CssClass="form-control" MaxLength="5" AutoCompleteType="Disabled" type="time"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="OraFineInterventoTextBox" CssClass="badge bg-danger" ErrorMessage="Ora fine: Formato hh:mm" ValidationExpression="^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$" Display="None">Formato hh:mm</asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="row">
                <div class="col-1">
                    <asp:Label ID="Label7" runat="server" Text="Codice campagna:" AssociatedControlID="CodiceCampagnaTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="CodiceCampagnaTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
                <div class="col-2">
                    <asp:Label ID="Label25" runat="server" Text="Quota campione:" AssociatedControlID="QuotaCampioneTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="QuotaCampioneTextBox" runat="server" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                    <asp:RangeValidator ID="RangeValidator7" runat="server" ErrorMessage="Quota campione: Deve essere un numero" ControlToValidate="QuotaCampioneTextBox" CssClass="badge bg-danger" MaximumValue="10000" MinimumValue="-10000" Type="Double" Display="None">Deve essere un numero</asp:RangeValidator>
                </div>
            </div>

            <div class="row mb-3">
                <asp:Panel ID="FileDatiPanel" runat="server" CssClass="col-3">
                    <asp:Label ID="Label8" runat="server" Text="File dati:" AssociatedControlID="FileDatiTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="FileDatiTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                </asp:Panel>
                <asp:Panel ID="TipiStrumentoInterventoPanel" runat="server" CssClass="col-3">
                    <asp:Label ID="Label15" runat="server" Text="Strumento usato:" AssociatedControlID="TipiStrumentoInterventoDropDownList" CssClass="form-label"></asp:Label>
                    <asp:DropDownList ID="TipiStrumentoInterventoDropDownList" runat="server" CssClass="form-select" DataTextField="DESCRIZIONE_STRUMENTO" DataValueField="ID_STRUMENTO"></asp:DropDownList>
                </asp:Panel>
                <asp:Panel ID="FileAngoliPanel" runat="server" CssClass="col-3">
                    <asp:Label ID="Label14" runat="server" Text="File angoli:" AssociatedControlID="FileAngoliTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="FileAngoliTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                </asp:Panel>
            </div>

            <div class="mb-3 p-2 border border-1">
                <h2>Tecnico di parte</h2>
                <div class="row mb-3">
                    <div class="col">
                        <asp:Label ID="Label20" runat="server" Text="Nome del tecnico:" AssociatedControlID="ParteNomeTecnicoTextBox" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="ParteNomeTecnicoTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                    </div>
                    <div class="col">
                        <asp:Label ID="Label21" runat="server" Text="Azienda del tecnico:" AssociatedControlID="ParteAziendaTecnicoTextBox" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="ParteAziendaTecnicoTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col">
                        <asp:Label ID="Label22" runat="server" Text="Ruolo del tecnico:" AssociatedControlID="ParteRuoloTecnicoTextBox" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="ParteRuoloTecnicoTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                    </div>
                    <div class="col">
                        <asp:Label ID="Label23" runat="server" Text="Contatti:" AssociatedControlID="ParteContattiTextBox" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="ParteContattiTextBox" runat="server" CssClass="form-control" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col mx-2 border border-1">
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
                <div class="col mx-2 border border-1">
                    <asp:UpdatePanel ID="OperatoriSupportoUpdatePanel" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="AggiungiOperatoreSupportoButton" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="p-2">
                                <h2>Operatori a supporto</h2>
                                <asp:GridView ID="OperatoriSupportoInterventoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="IdOperatore" EmptyDataText="Nessun operatore a supporto assegnato a questo intervento" ShowHeader="False" CssClass="table table-bordered table-striped table-hover table-sm w-auto" OnSelectedIndexChanged="OperatoriSupportoInterventoGridView_SelectedIndexChanged">
                                    <Columns>
                                        <asp:BoundField DataField="DescrizioneOperatore" />
                                        <asp:CommandField SelectText="Rimuovi" ShowSelectButton="True" ControlStyle-CssClass="link-danger">
                                            <ItemStyle Width="1%" />
                                        </asp:CommandField>
                                    </Columns>
                                </asp:GridView>
                                <div class="row">
                                    <div class="col">
                                        <asp:DropDownList ID="OperatoriSupportoPerInterventoDropDownList" runat="server" DataTextField="DescrizioneOperatore" DataValueField="IdOperatore" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col">
                                        <asp:Button ID="AggiungiOperatoreSupportoButton" runat="server" CssClass="btn btn-primary" Text="Aggiungi" CausesValidation="false" OnClick="AggiungiOperatoreSupportoButton_Click" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
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
                                    <asp:TextBox ID="ValoreMisurazioneTextBox" runat="server" CssClass="form-control" MaxLength="10" Width="7em" ToolTip='<%# Eval("NumeroDecimali").ToString() + " decimali" %>' AutoCompleteType="Disabled"></asp:TextBox>
                                    <asp:DropDownList ID="ValoreMisurazioneBooleanaDropDownList" runat="server" CssClass="form-select"></asp:DropDownList>
                                    <asp:RangeValidator ID="ValoreMisurazioneRangeValidator" runat="server" ErrorMessage='<%# "Misurazione " + Eval("GrandezzaEUnitaMisura") + ": deve essere un numero" %>' ControlToValidate="ValoreMisurazioneTextBox" CssClass="badge bg-danger" MaximumValue="9999999" MinimumValue="-999999" Type="Double" Display="None">Deve essere un numero</asp:RangeValidator>
                                    <span style="display: none;">
                                        <asp:TextBox ID="IdGrandezzaTextBox" runat="server" Text='<%# Eval("IdGrandezza") %>'></asp:TextBox>
                                        <asp:TextBox ID="CodiceIdentificativoTextBox" runat="server" Text='<%# Eval("CodiceIdentificativo") %>'></asp:TextBox>
                                    </span>
                                </td>
                                <td>
                                    <div class="form-check">
                                        <asp:CheckBox ID="FonteArpalCheckBox" runat="server" />
                                        <asp:Label ID="Label19" runat="server" Text="Fonte ARPAL" CssClass="form-check-label" AssociatedControlID="FonteArpalCheckBox"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col">
                    <asp:Label ID="Label18" runat="server" Text="Annotazioni:" AssociatedControlID="AnnotazioniTextBox" CssClass="form-label"></asp:Label>
                    <asp:TextBox ID="AnnotazioniTextBox" runat="server" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Height="100px" AutoCompleteType="Disabled"></asp:TextBox>
                </div>
            </div>

            <div class="my-lg-3">
                <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" OnClientClick="this.disabled=true; this.value='Attendere...';" UseSubmitBehavior="False" />
                <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
                <button class="btn btn-danger" data-bs-target="#confermaEliminazioneModal" data-bs-toggle="modal" type="button">Elimina</button>
            </div>

            <div id="confermaEliminazioneModal" aria-hidden="true" aria-labelledby="confermaEliminazioneModal" class="modal fade" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Elimina intervento</h5>
                        </div>
                        <div class="modal-body">
                            Eliminare questo intervento?
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                            <asp:Button ID="EliminaButton" runat="server" Text="Elimina" CssClass="btn btn-danger" CausesValidation="false" OnClick="EliminaButton_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
