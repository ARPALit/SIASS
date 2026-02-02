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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SIASS.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <title>SIASS - Login</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <asp:Panel ID="CredenzialiPanel" runat="server" CssClass="mb-3">
                <div class="alert alert-warning">
                    <asp:Label ID="AvvisoOpzioneRichiestaCredenzialiLabel" runat="server" Text="E' attiva l'opzione di richiesta credenziali (parametro RichiestaCredenziali del web.config)"></asp:Label>
                </div>
                <div class="mb-3">
                    <label for="UsernameTextBox" class="form-label">Login:</label>
                    <asp:TextBox ID="UsernameTextBox" runat="server" MaxLength="100" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="mb-3">
                    <label for="PasswordTextBox" class="form-label">Password</label>
                    <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" MaxLength="50" CssClass="form-control"></asp:TextBox>
                </div>
                <asp:Button ID="OKButton" runat="server" Text="OK" OnClick="OKButton_Click" CssClass="btn btn-primary" />
            </asp:Panel>
            <div class="alert alert-danger">
                <asp:Label ID="MessaggioErroreLabel" runat="server"></asp:Label>
            </div>
            <p>
                <asp:HyperLink ID="IntranetHyperLink" runat="server">Torna alla home page Intranet ARPAL</asp:HyperLink>
            </p>
        </div>
    </form>
</body>
</html>
