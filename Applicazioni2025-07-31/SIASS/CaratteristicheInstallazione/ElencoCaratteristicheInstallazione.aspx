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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="ElencoCaratteristicheInstallazione.aspx.cs" Inherits="SIASS.ElencoCaratteristicheInstallazione" %>
<%@ Register Src="~/Stazione/HeaderStazioneResponsive.ascx" TagPrefix="uc1" TagName="HeaderStazioneResponsive" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Caratteristiche dell'installazione</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive runat="server" ID="HeaderStazioneResponsive1" />
    <h1>Caratteristiche dell'installazione</h1>
    <p>
        <asp:HyperLink ID="NuovoHyperLink" runat="server">Nuove caratteristiche dell'installazione</asp:HyperLink>
    </p>
    <asp:GridView ID="ElencoCaratteristicheInstallazioneGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessuna caratteristica dell'installazione" CssClass="table table-bordered table-striped table-hover small w-auto" DataKeyNames="IdCaratteristicheInstallazione" OnRowDataBound="ElencoCaratteristicheInstallazioneGridView_RowDataBound">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="IdCaratteristicheInstallazione"
                    DataNavigateUrlFormatString="ModificaCaratteristicheInstallazione.aspx?IdCaratteristicheInstallazione={0}" Text="Modifica" />
                <asp:BoundField DataField="DescrizioneTipoFissaggioTrasmettitore" HeaderText="Tipo fissaggio trasmettitore" />
                <asp:TemplateField HeaderText="Cavo esterno in guaina">
                    <ItemTemplate>
                        &nbsp;<asp:Label ID="CavoEsternoInGuainaLabel" runat="server" Text="&#10004"></asp:Label>&nbsp;
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Cavo sottotraccia">
                    <ItemTemplate>
                        &nbsp;<asp:Label ID="CavoSottotracciaLabel" runat="server" Text="&#10004"></asp:Label>&nbsp;
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Protezione area">
                    <ItemTemplate>
                        &nbsp;<asp:Label ID="ProtezioneAreaLabel" runat="server" Text="&#10004"></asp:Label>&nbsp;
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:BoundField DataField="DescrizioneTipoAccesso" HeaderText="Tipo accesso" />
                <asp:BoundField DataField="InizioValidita" DataFormatString="{0:dd/MM/yyyy}"
                    HeaderText="Inizio validità">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="FineValidita" DataFormatString="{0:dd/MM/yyyy}"
                    HeaderText="Fine validità">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Osservazioni" HeaderText="Osservazioni">
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Sicurezza" HeaderText="Sicurezza">
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="ProfonditaSensore" HeaderText="Profondità sensore (m)">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>
            </Columns>
    </asp:GridView>
</asp:Content>
