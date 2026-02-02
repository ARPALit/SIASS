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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SIASS.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ARPAL Dataset</title>
    <style type="text/css">
        @import url('https://fonts.googleapis.com/css2?family=Asap:ital,wght@0,400;0,500;0,600;0,700;1,400;1,500;1,600;1,700&family=Bitter:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800&display=swap');

        html, body, div, form, fieldset, legend, label {
            font-family: Arial, Helvetica, sans-serif;
            margin: 0;
            padding: 0;
            font-family: 'Asap', sans-serif;
        }

        html {
            height: 100vh;
        }

        body {
            background-image: linear-gradient(to right, #bfcedc, #70879d);
            background-attachment: fixed;
            font-size: 15px;
            height: 100vh;
            color: #333;
        }

        table {
            border-collapse: collapse;
            border-spacing: 0;
        }

        th, td {
            text-align: left;
            vertical-align: top;
        }

        h1, h2, h3, h4, h5, h6, th, td, caption {
            font-weight: normal;
            font-family: 'Bitter', serif;
            margin: 0px;
        }

        img {
            border: 0;
        }

        section {
            max-width: 1000px;
            margin: auto;
            background-color: #728598;
            min-height: 100vh;
            box-shadow: 0px 0px 5px 0px #688096;
        }

        header {
            padding: 24px;
            background-color: #fff;
        }

            header h1 {
                color: #F70011;
                font-size: 28px;
                padding: 0px;
                font-style: italic;
                display: inline-block;
                font-weight: 500;
                padding-top: 10px;
            }

            header img {
                display: inline-block;
                vertical-align: bottom;
                margin: 0px 24px 0px 0px;
            }

        header, article {
            clear: both;
        }

        article {
            background-color: #edf4fa;
            padding: 24px;
        }

        footer {
            color: #EEE;
            background-color: #728598;
            padding: 24px;
            font-style: italic;
            font-size: 14px;
        }

        h2 {
            color: #0053a6;
            font-weight: 500;
            font-size: 25px;
            padding-bottom: 5px;
        }

        a {
            color: #0053a6;
        }

            a:hover {
                text-decoration: none;
            }

        h4 {
            color: #618db4;
            font-weight: 500;
            font-weight: 500;
            font-size: 20px;
            padding-bottom: 12px;
            font-style: italic;
        }

        h6 {
            color: #99444a;
            font-weight: 500;
            font-weight: 500;
            font-size: 17px;
            padding-bottom: 24px;
            font-style: italic;
        }

        hr {
            margin: 24px 0px;
            border: none;
            border-top: 1px dashed #919da7;
        }

        p {
            line-height: 22px;
            margin: 0px 0px 16px 0px;
        }

        li {
            margin: 0px 0px 8px 24px;
            padding: 0px;
            line-height: 22px;
        }

        ul {
            padding: 0px 0px 16px 0px;
            margin: 0px;
        }

        strong {
            font-weight: 600;
        }

        table {
            font-size: 13px;
            border-collapse: collapse;
            border-spacing: 0;
            width: 100%;
            background-color: #fff;
        }

            table th {
                font-weight: 600;
            }

            table th, table td {
                text-align: left;
                padding: 5px;
            }

            table tbody tr:nth-child(even) {
                background-color: #edf4fa
            }

        .tabcontent {
            display: none;
        }

        .tab {
            margin-bottom: 18px;
            border-bottom: 1px solid #97b1c9;
        }

        .tablinks {
            background-color: inherit;
            display: inline-block;
            border: none;
            outline: none;
            cursor: pointer;
            padding: 10px 12px 8px 12px;
            transition: 0.33s;
            font-size: 15px;
            color: #0053a6;
            text-decoration: underline;
        }

            .tablinks:hover,
            .tablinks.active {
                background-color: #97b1c9;
                color: #FFF;
                text-decoration: none;
                border-top-left-radius: 8px;
                border-top-right-radius: 8px;
            }
    </style>
    <script type="text/javascript">
        function openTab(evt, tabName) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }
            document.getElementById(tabName).style.display = "block";
            evt.currentTarget.className += " active";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <section>

            <header>
                <img src="../img/Logo_Arpal.png" width="300" title="Arpa Liguria Logo" />
                <h1>ARPAL - Dataset SIASS</h1>
            </header>

            <article>

                <div id="ContenutoIntroduzione" runat="server"></div>

                <hr />

                <h2>Dataset "Struttura rete di monitoraggio"</h2>
                <h6>Rende un oggetto Json che descrive la struttura della rete di monitoraggio.</h6>
                <p>
                    La URL di servizio è:
                    <asp:HyperLink ID="ReteMonitoraggioHyperLink" runat="server" Target="_blank"></asp:HyperLink>
                </p>
                <p>
                    Di default viene resa tutta la struttura della rete regionale per tutti gli anni. Per visualizzare la consistenza della rete in uno specifico anno l'URL è <em>
                        <asp:Label ID="ReteMonitoraggioAnnoLabel" runat="server"></asp:Label></em> dove <em>MM</em> indica il mese, <em>AAAA</em> indica l'anno, <em>CODICE_STAZIONE</em> indica il codice identificativo della stazione.
                </p>

                <hr />

                <h2>Dataset relativi alle stazioni</h2>
                <h6>Seleziona il tipo di dati di tuo interesse.</h6>
                <div class="tab">
                    <button class="tablinks active" onclick="openTab(event, 'StoricoMisurazioni'); return false;">Storico misurazioni</button>
                    <button class="tablinks" onclick="openTab(event, 'MisurazioniAnnoCorrente'); return false;">Misurazioni anno corrente</button>
                </div>
                <div id="StoricoMisurazioni" class="tabcontent" style="display: block;">
                    <h4>Dataset &quot;Storico misurazioni&quot;</h4>
                    <asp:Repeater ID="StoricoMisurazioniRepeater" runat="server">
                        <HeaderTemplate>
                            <table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:HyperLink ID="JsonHyperLink" runat="server" NavigateUrl='<%# Eval("URLFile") %>' Target="_blank"><%# Eval("Descrizione") %></asp:HyperLink>
                                </td>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <td>
                                <asp:HyperLink ID="JsonHyperLink" runat="server" NavigateUrl='<%# Eval("URLFile") %>' Target="_blank"><%# Eval("Descrizione") %></asp:HyperLink>
                            </td>
                            </tr>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div id="MisurazioniAnnoCorrente" class="tabcontent">
                    <h4>Dataset &quot;Misurazioni anno corrente&quot;</h4>
                    <asp:Repeater ID="MisurazioniAnnoCorrenteRepeater" runat="server">
                        <HeaderTemplate>
                            <table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:HyperLink ID="JsonHyperLink" runat="server" NavigateUrl='<%# Eval("URLFile") %>' Target="_blank"><%# Eval("Descrizione") %></asp:HyperLink>
                                </td>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <td>
                                <asp:HyperLink ID="JsonHyperLink" runat="server" NavigateUrl='<%# Eval("URLFile") %>' Target="_blank"><%# Eval("Descrizione") %></asp:HyperLink>
                            </td>
                            </tr>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </article>

            <footer>&copy; ARPAL 2023 | Via Bombrini 8 - 16149 Genova - tel +39 010 64371 | C.F. e P.IVA 01305930107</footer>

        </section>
    </form>
</body>
</html>
