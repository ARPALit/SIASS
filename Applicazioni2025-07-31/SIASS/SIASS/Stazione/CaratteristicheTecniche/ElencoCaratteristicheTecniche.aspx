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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoCaratteristicheTecniche.aspx.cs" Inherits="SIASS.ElencoCaratteristicheTecniche" %>

<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Caratteristiche tecniche</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive runat="server" ID="HeaderStazioneResponsive1" />
    <h1>Caratteristiche tecniche</h1>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuove caratteristiche tecniche</asp:HyperLink>
    </p>
    <asp:GridView ID="ElencoCaratteristicheTecnicheGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessuna caratteristica tecnica" CssClass="table table-bordered table-striped table-hover small w-auto" DataKeyNames="IdCaratteristicheTecnichePozzo" OnRowDataBound="ElencoCaratteristicheTecnicheGridView_RowDataBound">
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="IdCaratteristicheTecnichePozzo" DataNavigateUrlFormatString="ModificaCaratteristicheTecniche.aspx?IdCaratteristicheTecniche={0}" Text="Modifica" />
            <asp:BoundField DataField="Profondita" HeaderText="Profondità" />
            <asp:BoundField DataField="Diametro" HeaderText="Diametro" />
            <asp:TemplateField HeaderText="Range di soggiacenza">
                <ItemTemplate>
                    <asp:Label ID="RangeSoggiacenzaDaLabel" runat="server" Text='<%# Bind("RangeSoggiacenzaDa") %>'></asp:Label>-<asp:Label ID="RangeSoggiacenzaALabel" runat="server" Text='<%# Bind("RangeSoggiacenzaA") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DescrizioneTipoChiusura" HeaderText="Tipo chiusura" />
            <asp:BoundField DataField="QuotaBoccapozzoPc" HeaderText="Quota boccapozzo piano campagna" />
            <asp:BoundField DataField="QuotaBoccapozzoSlm" HeaderText="Quota boccapozzo (m s.l.m.)" />
            <asp:BoundField DataField="QuotaPianoRiferimentoSlm" HeaderText="Quota piano di riferimento (PR) (m s.l.m.)" />
            <asp:BoundField DataField="DifferenzaPrpc" HeaderText="Differenza PR-PC (cm)" />
            <asp:BoundField DataField="ProfonditaEmungimento" HeaderText="Profondità emungimento" />
            <asp:BoundField DataField="PortataMassimaEsercizio" HeaderText="Portata massima" />
            <asp:TemplateField HeaderText="Presenza foro per la sonda">
                <ItemTemplate>
                    &nbsp;<asp:Label ID="PresenzaForoSondaLabel" runat="server" Text="&#10004"></asp:Label>&nbsp;
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:BoundField DataField="DescrizioneTipoDestinazioneUso" HeaderText="Destinazione d'uso" />
            <asp:BoundField DataField="DescrizioneTipoFrequenzaUtilizzo" HeaderText="Frequenza di utilizzo" />
            <asp:TemplateField HeaderText="Captata">
                <ItemTemplate>
                    &nbsp;<asp:Label ID="CaptataLabel" runat="server" Text="&#10004"></asp:Label>&nbsp;
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:BoundField DataField="InizioValidita" DataFormatString="{0:dd/MM/yyyy}"
                HeaderText="Inizio validità">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="FineValidita" DataFormatString="{0:dd/MM/yyyy}"
                HeaderText="Fine validità">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
</asp:Content>
