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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="NuovoCaratteristicheTecniche.aspx.cs" Inherits="SIASS.NuovoCaratteristicheTecniche" %>
<%@ Register src="../HeaderStazioneResponsive.ascx" tagname="HeaderStazioneResponsive" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Caratteristiche tecniche</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
    </asp:ScriptManager>
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Nuove caratteristiche tecniche</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label2" runat="server" Text="Profondità (metri):" AssociatedControlID="ProfonditaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="ProfonditaTextBox" runat="server" CssClass="form-control" MaxLength="6"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Profondità: deve essere un numero maggiore di 0" ControlToValidate="ProfonditaTextBox" CssClass="badge bg-danger" MinimumValue="0,01" Type="Double" Display="None" MaximumValue="1000">Deve essere un numero maggiore di 0</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label5" runat="server" Text="Diametro (cm):" AssociatedControlID="DiametroTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="DiametroTextBox" runat="server" CssClass="form-control"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Diametro: deve essere un numero maggiore di 0" ControlToValidate="DiametroTextBox" CssClass="badge bg-danger" MinimumValue="0,01" Type="Double" Display="None" MaximumValue="1000">Deve essere un numero maggiore di 0</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label6" runat="server" Text="Range di soggiacenza - Da (cm):" AssociatedControlID="RangeSoggiacenzaDaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="RangeSoggiacenzaDaTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator3" runat="server" ErrorMessage="Range di soggiacenza Da: deve essere un numero" ControlToValidate="RangeSoggiacenzaDaTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="100000" MinimumValue="-100000">Deve essere un numero</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label7" runat="server" Text="Range di soggiacenza - A (cm):" AssociatedControlID="RangeSoggiacenzaATextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="RangeSoggiacenzaATextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Range di soggiacenza A: deve essere un numero" ControlToValidate="RangeSoggiacenzaATextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="100000" MinimumValue="-100000">Deve essere un numero</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label8" runat="server" Text="Tipo di chiusura:" AssociatedControlID="TipiChiusuraDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiChiusuraDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneTipoChiusura" DataValueField="IdTipoChiusura">
        </asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label9" runat="server" Text="Quota boccapozzo rispetto al piano campagna (cm):" AssociatedControlID="QuotaBoccapozzoPcTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="QuotaBoccapozzoPcTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator5" runat="server" ErrorMessage="Quota boccapozzo rispetto al piano campagna: deve essere un numero" ControlToValidate="QuotaBoccapozzoPcTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="1000" MinimumValue="-1000">Deve essere un numero</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label3" runat="server" Text="Quota boccapozzo (m s.l.m.):" AssociatedControlID="QuotaBoccapozzoSlmTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="QuotaBoccapozzoSlmTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ErrorMessage="Quota boccapozzo (m s.l.m.): deve essere un numero a due decimali" ControlToValidate="QuotaBoccapozzoSlmTextBox" ValidationExpression="^\-?[0-9]+(?:\,[0-9]{1,2})?$" Display="None"></asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label4" runat="server" Text="Quota piano di riferimento (PR) (m s.l.m.):" AssociatedControlID="QuotaPianoRiferimentoSlmTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="QuotaPianoRiferimentoSlmTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Quota piano di riferimento (PR) (m s.l.m.): Obbligatorio" ControlToValidate="QuotaPianoRiferimentoSlmTextBox" Display="None"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="Quota piano di riferimento (PR) (m s.l.m.): deve essere un numero a due decimali" ControlToValidate="QuotaPianoRiferimentoSlmTextBox" ValidationExpression="^\-?[0-9]+(?:\,[0-9]{1,2})?$" Display="None"></asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label17" runat="server" Text="Differenza PR-PC (cm):" AssociatedControlID="DifferenzaPrpcTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="DifferenzaPrpcTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator10" runat="server" ErrorMessage="Differenza PR-PC (cm): deve essere un numero" ControlToValidate="DifferenzaPrpcTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="10000" MinimumValue="-10000">Deve essere un numero</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label10" runat="server" Text="Profondità emungimento rispetto al piano campagna (m):" AssociatedControlID="ProfonditaEmungimentoTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="ProfonditaEmungimentoTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator6" runat="server" ErrorMessage="Profondità emungimento rispetto al piano campagna: deve essere un numero" ControlToValidate="ProfonditaEmungimentoTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="1000" MinimumValue="-1000">Deve essere un numero</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label13" runat="server" Text="Portata massima di esercizio (l/s):" AssociatedControlID="PortataMassimaEsercizioTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="PortataMassimaEsercizioTextBox" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator7" runat="server" ErrorMessage="Portata massima di esercizio: deve essere un numero maggiore o uguale a 0" ControlToValidate="PortataMassimaEsercizioTextBox" CssClass="badge bg-danger" Display="None" Type="Double" MaximumValue="10000" MinimumValue="0">Deve essere un numero maggiore o uguale a 0</asp:RangeValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label14" runat="server" Text="Presenza foro per la sonda:" AssociatedControlID="PresenzaForoSondaCheckBox" CssClass="form-label"></asp:Label>
        <asp:CheckBox ID="PresenzaForoSondaCheckBox" runat="server" CssClass="form-check" />
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label15" runat="server" Text="Destinazione d'uso:" AssociatedControlID="TipiDestinazioneUsoDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiDestinazioneUsoDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneTipoDestinazioneUso" DataValueField="IdTipoDestinazioneUso">
        </asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label16" runat="server" Text="Frequenza di utilizzo:" AssociatedControlID="TipiFrequenzaUtilizzoDropDownList" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="TipiFrequenzaUtilizzoDropDownList" runat="server" CssClass="form-select" DataTextField="DescrizioneTipoFrequenzaUtilizzo" DataValueField="IdTipoFrequenzaUtilizzo">
        </asp:DropDownList>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label1" runat="server" Text="Captata:" AssociatedControlID="CaptataCheckBox" CssClass="form-label"></asp:Label>
        <asp:CheckBox ID="CaptataCheckBox" runat="server" CssClass="form-check" />
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label11" runat="server" Text="Inizio validità (gg/mm/aaaa):" AssociatedControlID="InizioValiditaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="InizioValiditaTextBox" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="InizioValiditaTextBox" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Inizio validità: Obbligatorio" ControlToValidate="InizioValiditaTextBox" CssClass="badge bg-danger" Display="None">Obbligatorio</asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="InizioValiditaTextBox" CssClass="badge bg-danger" ErrorMessage="Inizio validità: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
    </div>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label12" runat="server" Text="Fine validità (gg/mm/aaaa):" AssociatedControlID="FineValiditaTextBox" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="FineValiditaTextBox" runat="server" MaxLength="10" CssClass="form-control"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="FineValiditaTextBox" />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="FineValiditaTextBox" CssClass="badge bg-danger" ErrorMessage="Fine validità: Formato gg/mm/aaaa" ValidationExpression="^(((0?[1-9]|[12]\d|3[01])[\/](0?[13578]|1[02])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|[12]\d|30)[\/](0?[13456789]|1[012])[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|((0?[1-9]|1\d|2[0-8])[\/]0?2[\/]((1[6-9]|[2-9]\d)?\d{2}|\d))|(29[\/]0?2[\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00|[048])))$" Display="None">Formato gg/mm/aaaa</asp:RegularExpressionValidator>
    </div>
    <div class="my-lg-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" CssClass="btn btn-success" OnClick="SalvaButton_Click" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
    </div>
</asp:Content>
