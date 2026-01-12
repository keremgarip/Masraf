<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Masraf.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta http-equiv="x-ua-compatible" content="ie=edge" />
    <title>Masraf Giriş</title>
    <link rel="stylesheet" href="../vendor/bootstrap4/css/bootstrap.min.css" />
    <link rel="stylesheet" href="../vendor/themify-icons/themify-icons.css" />
    <link rel="stylesheet" href="../vendor/font-awesome/css/font-awesome.min.css" />
    <link rel="stylesheet" href="css/core.css" />
    <!-- HTML5 Shiv and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
		<script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
		<script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
		<![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
			<div class="sign-form">
				<div class="row">
					<div class="col-md-4 offset-md-4 px-3">
						<div class="box b-a-0">
							<div class="p-2 text-xs-center">
								<h5>Hoşgeldiniz</h5>
							</div>
							<div class="form-material mb-1">
								<div class="form-group">
                                    <asp:TextBox runat="server" ID="exampleInputEmail" CssClass="form-control" Placeholder="Email"></asp:TextBox>
								</div>
								<div class="form-group">
                                    <asp:TextBox runat="server" ID="exampleInputPassword" CssClass="form-control" placeholder="Şifre" TextMode="Password"></asp:TextBox>
								</div>
								<div class="px-2 form-group mb-0">
                                    <asp:Button runat="server" ID="girisyap" CssClass="btn btn-purple btn-block text-uppercase" Text="GİRİŞ YAP" OnClick="girisyap_Click" />
								</div>
							</div>
							<div class="px-2">
								<div class="row">
                                    
								</div>
							</div>
                            <div class="p-2 text-xs-center text-muted colorred">
                                <a style="display:none" href="/panel/resetpassword.aspx">Şifrenizi mi unuttunuz ?</a>
                                <asp:Literal runat="server" ID="Label1"></asp:Literal></div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<script type="text/javascript" src="../vendor/jquery/jquery-1.12.3.min.js"></script>
		<script type="text/javascript" src="../vendor/tether/js/tether.min.js"></script>
		<script type="text/javascript" src="../vendor/bootstrap4/js/bootstrap.min.js"></script>
    </form>
</body>
</html>
