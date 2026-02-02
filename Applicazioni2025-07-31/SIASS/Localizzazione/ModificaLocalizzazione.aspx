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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ModificaLocalizzazione.aspx.cs" Inherits="SIASS.ModificaLocalizzazione" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Localizzazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Modifica localizzazione</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label2" runat="server" Text="Comune:" AssociatedControlID="ComuniDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="ComuniDropDownList" runat="server" DataTextField="DenominazioneComune" DataValueField="CodiceComune" CssClass="form-select">
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Comune: Obbligatorio" ControlToValidate="ComuniDropDownList" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label5" runat="server" Text="Località:" AssociatedControlID="LocalitaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="LocalitaTextBox" runat="server" CssClass="form-control" MaxLength="300" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label1" runat="server" Text="Bacino:" AssociatedControlID="BaciniDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="BaciniDropDownList" runat="server" DataTextField="DescrizioneBacino" DataValueField="IdBacino" CssClass="form-select">
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Bacino: Obbligatorio" ControlToValidate="BaciniDropDownList" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label3" runat="server" Text="Corpo idrico:" AssociatedControlID="CorpiIdriciDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="CorpiIdriciDropDownList" runat="server" DataTextField="DescrizioneCorpoIdrico" DataValueField="IdCorpoIdrico" CssClass="form-select">
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Corpo idrico: Obbligatorio" ControlToValidate="CorpiIdriciDropDownList" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label4" runat="server" Text="CTR 1:10000:" AssociatedControlID="CTRTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="CTRTextBox" runat="server" CssClass="form-control" MaxLength="300" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label6" runat="server" Text="Latitudine (gradi decimali):" AssociatedControlID="LatitudineTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="LatitudineTextBox" runat="server" CssClass="form-control" MaxLength="20" AutoCompleteType="Disabled"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Latitudine (gradi decimali): Obbligatorio" ControlToValidate="LatitudineTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        <asp:RangeValidator ID="RangeValidator7" runat="server" ErrorMessage="Latitudine (gradi decimali): deve essere compreso tra 43,5 e 44,7" ControlToValidate="LatitudineTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="44,7" MinimumValue="43,5">Deve essere compreso tra 43,5 e 44,7</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label7" runat="server" Text="Longitudine (gradi decimali):" AssociatedControlID="LongitudineTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="LongitudineTextBox" runat="server" CssClass="form-control" MaxLength="20" AutoCompleteType="Disabled"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Longitudine (gradi decimali): Obbligatorio" ControlToValidate="LongitudineTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        <asp:RangeValidator ID="RangeValidator8" runat="server" ErrorMessage="Longitudine (gradi decimali): deve essere compreso tra 7,4 e 10,1" ControlToValidate="LongitudineTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="10,1" MinimumValue="7,4">Deve essere compreso tra 7,4 e 10,1</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label8" runat="server" Text="Latitudine Gauss-Boaga (m):" AssociatedControlID="LatitudineGaussBoagaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="LatitudineGaussBoagaTextBox" runat="server" CssClass="form-control" MaxLength="20" AutoCompleteType="Disabled"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Latitudine Gauss-Boaga (m): Obbligatorio" ControlToValidate="LatitudineGaussBoagaTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Latitudine Gauss-Boaga (m): deve essere compreso tra 4800000 e 5000000" ControlToValidate="LatitudineGaussBoagaTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="5000000" MinimumValue="4800000">Deve essere compreso tra 4800000 e 5000000</asp:RangeValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ErrorMessage="Latitudine Gauss-Boaga (m): deve essere un numero a un decimale" ControlToValidate="LatitudineGaussBoagaTextBox" ValidationExpression="^\-?[0-9]+(?:\,[0-9]{1,1})?$" Display="None"></asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label9" runat="server" Text="Longitudine Gauss-Boaga (m):" AssociatedControlID="LongitudineGaussBoagaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="LongitudineGaussBoagaTextBox" runat="server" CssClass="form-control" MaxLength="20" AutoCompleteType="Disabled"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Longitudine Gauss-Boaga (m): Obbligatorio" ControlToValidate="LongitudineGaussBoagaTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Longitudine Gauss-Boaga (m): deve essere compreso tra 1370000 e 1590000" ControlToValidate="LongitudineGaussBoagaTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="1590000" MinimumValue="1370000">Deve essere compreso tra 1370000 e 1590000</asp:RangeValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ErrorMessage="Longitudine Gauss-Boaga (m): deve essere un numero a un decimale" ControlToValidate="LongitudineGaussBoagaTextBox" ValidationExpression="^\-?[0-9]+(?:\,[0-9]{1,1})?$" Display="None"></asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label10" runat="server" Text="Quota piano campagna (PC) (m s.l.m.):" AssociatedControlID="QuotaPianoCampagnaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="QuotaPianoCampagnaTextBox" runat="server" CssClass="form-control" MaxLength="20" AutoCompleteType="Disabled"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="Quota piano campagna (PC) (m s.l.m.): Obbligatorio" ControlToValidate="QuotaPianoCampagnaTextBox" Display="None"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="Quota piano campagna (PC) (m s.l.m.): deve essere un numero a due decimali" ControlToValidate="QuotaPianoCampagnaTextBox" ValidationExpression="^\-?[0-9]+(?:\,[0-9]{1,2})?$" Display="None"></asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label11" runat="server" Text="Codice SIRAL:" AssociatedControlID="CodiceSIRALTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="CodiceSIRALTextBox" runat="server" CssClass="form-control" MaxLength="50" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label12" runat="server" Text="Inizio validità (gg/mm/aaaa):" AssociatedControlID="InizioValiditaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="InizioValiditaTextBox" runat="server" MaxLength="10" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="InizioValiditaTextBox" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Inizio validità: Obbligatorio" ControlToValidate="InizioValiditaTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="InizioValiditaTextBox" CssClass="badge bg-danger" ErrorMessage="Inizio validità: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label13" runat="server" Text="Fine validità (gg/mm/aaaa):" AssociatedControlID="FineValiditaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="FineValiditaTextBox" runat="server" MaxLength="10" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="FineValiditaTextBox" />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="FineValiditaTextBox" CssClass="badge bg-danger" ErrorMessage="Fine validità: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
        <button class="btn btn-danger" data-bs-target="#confermaEliminazioneModal" data-bs-toggle="modal" type="button">Elimina</button>
    </div>
    <div id="confermaEliminazioneModal" aria-hidden="true" aria-labelledby="confermaEliminazioneModal" class="modal fade" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Elimina localizzazione</h5>
                </div>
                <div class="modal-body">
                    Eliminare questa localizzazione?
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                    <asp:Button ID="EliminaButton" runat="server" Text="Elimina" CssClass="btn btn-danger" CausesValidation="false" OnClick="EliminaButton_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
