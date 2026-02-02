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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ModificaAllegatoStazione.aspx.cs" Inherits="SIASS.ModificaAllegatoStazione" %>

<%@ Register Src="../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Allegato stazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Modifica allegato</h1>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowModelStateErrors="true"
        CssClass="alert alert-danger"></asp:ValidationSummary>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label2" runat="server" Text="File:"></asp:Label>
        <asp:HyperLink ID="NomeFileAllegatoHyperLink" runat="server" Target="_blank"></asp:HyperLink>
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label5" runat="server" Text="Descrizione:"></asp:Label>
        <asp:TextBox ID="DescrizioneAllegatoTextBox" runat="server" Width="30em" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
    </div>
    <div class="mb-3" style="max-width: 300px">
        <asp:Label ID="Label1" runat="server" Text="Data inserimento:"></asp:Label>
        <asp:Label ID="DataOraInserimentoLabel" runat="server"></asp:Label>
    </div>
    <div class="mb-3">
        <asp:Button ID="SalvaButton" runat="server" Text="Salva" OnClick="SalvaButton_Click" CssClass="btn btn-success" />
        <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CausesValidation="False" OnClick="AnnullaButton_Click" CssClass="btn btn-primary" />
        <button class="btn btn-danger" data-bs-target="#confermaEliminazioneModal" data-bs-toggle="modal" type="button">Elimina</button>
    </div>
    <div id="confermaEliminazioneModal" aria-hidden="true" aria-labelledby="confermaEliminazioneModal" class="modal fade" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Elimina allegato</h5>
                </div>
                <div class="modal-body">
                    Eliminare questo allegato?
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-bs-dismiss="modal" type="button">Annulla</button>
                    <asp:Button ID="EliminaButton" runat="server" Text="Elimina" CausesValidation="False" OnClick="EliminaButton_Click" CssClass="btn btn-danger" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
