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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageResponsive.Master" AutoEventWireup="true" CodeBehind="RiassegnaSensoriStrumento.aspx.cs" Inherits="SIASS.RiassegnaSensoriStrumento" %>

<%@ Register Src="../../HeaderStazioneResponsive.ascx" TagName="HeaderStazioneResponsive" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>SIASS - Riassegna sensori</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:HeaderStazioneResponsive ID="HeaderStazioneResponsive1" runat="server" />
    <h1>Riassegna sensori ad altro strumento</h1>
    <div class="mb-3" style="max-width: 500px">
        <asp:Label ID="Label3" runat="server" Text="Tipo strumento:" CssClass="form-label"></asp:Label>
        <asp:HyperLink ID="DescrizioneTipoStrumentoHyperLink" runat="server"></asp:HyperLink>
        &nbsp;
        <asp:Label ID="Label1" runat="server" Text="Numero di serie strumento:" CssClass="form-label"></asp:Label>
        <asp:Label ID="NumeroDiSerieLabel" runat="server"></asp:Label>
    </div>
    <asp:MultiView ID="RiassegnaSensoriMultiView" runat="server">
        <asp:View ID="SelezioneSensoriView" runat="server">
            <div class="mb-3">
                <p>
                    1) Selezionare i sensori da riassegnare.
                </p>
                <asp:GridView ID="SensoriStrumentoGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="CodiceIdentificativo" EmptyDataText="Nessun sensore assegnato a questo strumento" CssClass="table table-bordered table-striped table-sm table-hover w-auto">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <script type="text/javascript">
                                    function SelezionaTutteNessuna() {
                                        var checkSpan = document.getElementsByClassName("checkTabellaSensori");
                                        for (i = 0; i < checkSpan.length; i++) {
                                            checkSpan[i].getElementsByTagName("input")[0].checked = document.getElementById("SelezionaTutteChk").checked;
                                        }
                                    }
                                </script>
                                <input id="SelezionaTutteChk" name="SelezionaTutteChk" type="checkbox" onclick="SelezionaTutteNessuna()" title="Seleziona tutte/nessuna" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="SelezionaCheckBox" runat="server" CssClass="checkTabellaSensori" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodiceIdentificativo" HeaderText="Codice identificativo" />
                        <asp:BoundField DataField="GrandezzaEUnitaMisura" HeaderText="Grandezza e unità di misura della stazione" />
                        <asp:BoundField DataField="CodicePMC" HeaderText="Codice PMC" />
                        <asp:BoundField DataField="NumeroDecimali" HeaderText="Numero decimali" />
                        <asp:BoundField DataField="EspressioneRisultato" HeaderText="Espressione risultato" />
                        <asp:BoundField DataField="FrequenzaAcquisizione" HeaderText="Frequenza acquisizione" />
                        <asp:BoundField DataField="Metodo" HeaderText="Metodo" />
                        <asp:BoundField DataField="UnitaMisuraSensore" HeaderText="Unità di misura del sensore" />
                        <asp:BoundField DataField="CoefficienteConversioneUnitaMisura" HeaderText="Coefficiente conversione unità di misura" />
                    </Columns>
                </asp:GridView>
                <p>
                    2) Selezionare lo strumento a cui riassegnare i sensori.
                </p>
                <asp:GridView ID="ElencoStrumentiGridView" runat="server" AutoGenerateColumns="False" EmptyDataText="Nessuno strumento disponibile a cui riassegnare i sensori" CssClass="table table-bordered table-striped table-hover small w-auto" DataKeyNames="IdStrumento" OnSelectedIndexChanged="ElencoStrumentiGridView_SelectedIndexChanged">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="DescrizioneTipoStrumento" HeaderText="Tipo" />
                        <asp:BoundField DataField="NumeroDiSerie" HeaderText="Numero di serie" />
                        <asp:BoundField DataField="Marca" HeaderText="Marca" />
                        <asp:BoundField DataField="Modello" HeaderText="Modello" />
                        <asp:BoundField DataField="NumeroInventarioArpal" HeaderText="Numero inventario Arpal" />
                        <asp:BoundField DataField="Caratteristiche" HeaderText="Caratteristiche" />
                        <asp:BoundField DataField="CodiceSistemaGestionale" HeaderText="Codice sistema gestionale" />
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
            </div>
        </asp:View>
        <asp:View ID="ConfermaView" runat="server">
            <p>
                <asp:Label ID="ConfermaLabel" runat="server"></asp:Label>
            </p>
            <div class="my-lg-3">
                <asp:Button ID="RiassegnaButton" runat="server" Text="Riassegna" CssClass="btn btn-success" OnClick="RiassegnaButton_Click" />
                <asp:Button ID="AnnullaButton" runat="server" Text="Annulla" CssClass="btn btn-primary" CausesValidation="false" OnClick="AnnullaButton_Click" />
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
