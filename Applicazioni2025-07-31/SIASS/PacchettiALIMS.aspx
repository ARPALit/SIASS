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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="PacchettiALIMS.aspx.cs" Inherits="SIASS.PacchettiALIMS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Pacchetti ALIMS</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Pacchetti ALIMS</h1>
    <div class="row mb-2">
        <div class="col">
            <asp:Label ID="Label4" runat="server" Text="Matrice:" AssociatedControlID="TipiMatriceDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiMatriceDropDownList" runat="server" CssClass="form-select" DataTextField="Descrizione" DataValueField="Codice" AutoPostBack="True" OnSelectedIndexChanged="TipiMatriceDropDownList_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class="col">
            <asp:Label ID="Label3" runat="server" Text="Argomento:" AssociatedControlID="TipiArgomentoDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiArgomentoDropDownList" runat="server" CssClass="form-select" DataTextField="Descrizione" DataValueField="Codice" AutoPostBack="True"></asp:DropDownList>
        </div>
        <div class="col">
            <asp:Label ID="Label2" runat="server" Text="Sede di accettazione:" AssociatedControlID="TipiSediDropDownList" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="TipiSediDropDownList" runat="server" CssClass="form-select" AutoPostBack="True" DataTextField="DENOMINAZIONE_SEDE" DataValueField="CODICE_SEDE"></asp:DropDownList>
        </div>
    </div>

    <asp:Button ID="CercaPacchettiButton" runat="server" Text="Cerca pacchetti" OnClick="CercaPacchettiButton_Click" CssClass="btn btn-primary mb-2" />

    <asp:Panel ID="PacchettiPanel" runat="server">
        <h2>Pacchetti</h2>
        <asp:DropDownList ID="PacchettiDropDownList" runat="server" CssClass="form-select" AutoPostBack="True" DataTextField="PackName" DataValueField="PackIdentity" OnSelectedIndexChanged="PacchettiDropDownList_SelectedIndexChanged"></asp:DropDownList>
        <h2>Analiti</h2>
        <asp:GridView ID="AnalitiGridView" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered small w-auto" DataKeyNames="Codice">
            <Columns>
                <asp:BoundField DataField="CodiceEDescrizione" HeaderText="Analita" />
                <asp:BoundField DataField="CodiceEDescrizioneMetodo" HeaderText="Metodo" />
                <asp:BoundField DataField="CodiceEDescrizionePacchetto" HeaderText="Pacchetto" />
                <asp:BoundField DataField="UnitaMisura" HeaderText="Unità di misura" />
                <asp:BoundField DataField="ValoreLimite" HeaderText="Valore limite" Visible="False" />
                <asp:BoundField DataField="LDQ" HeaderText="LDQ" />
                <asp:BoundField DataField="LimiteIndic" HeaderText="Limite indic." />
                <asp:BoundField DataField="LimiteMinimo" HeaderText="Limite minimo" />
                <asp:BoundField DataField="LimiteMassimo" HeaderText="Limite massimo" />
                <asp:BoundField DataField="LaboratorioAnalisi" HeaderText="Laboratorio analisi" />
            </Columns>
        </asp:GridView>
        <h2>Contenitori</h2>
        <asp:GridView ID="ContenitoriGridView" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered small w-auto">
            <Columns>
                <asp:BoundField DataField="Descrizione" HeaderText="Contenitore" />
                <asp:BoundField DataField="Quantita" HeaderText="Quantità" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
