<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="setpassword.aspx.cs" Inherits="Masraf.setpassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Masraf Yönetim Paneli Şifreni belirle</title>
    <script src="https://code.jquery.com/jquery-3.6.1.js" integrity="sha256-3zlB5s2uwoUzrXK3BT7AX3FyvojsraNFxCc2vC/7pNI=" crossorigin="anonymous"></script>
    <link id="twStyle" rel="stylesheet" href="dist/app_skin_sale-light.css?v=1" asp-append-version="true">
</head>
<body>
    <form id="form1" runat="server">
        <div class="row contentSticky">
            <div class="col-lg-12 col-md-12 col-xs-12 mb-30 " style="width:100%; text-align:center; font-weight:bold;">
                <h4 class="formTitle p-0">Şifreni Belirle</h4>
                
            </div>
            <div class="box box-block" style="width: 100%; margin-top: 10px;">
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-xs-12 row white-bg">
                        <div class="form-group">
                            <label>Email</label>
                            <asp:TextBox runat="server" ID="email" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>Şifre</label>
                            <asp:TextBox runat="server" ID="sifre" CssClass="form-control" type="password"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>Şifre Tekrar</label>
                            <asp:TextBox runat="server" ID="sifretekrar" CssClass="form-control" type="password"></asp:TextBox>
                        </div>
                        <div class="col-lg-12 col-md-12 col-xs-12">
                            <div class="flex justifyEnd" style="width:100%; text-align:center; font-weight:bold;"style="width:100%; text-align:center; font-weight:bold;">
                                <div class="form-group">
                                    <asp:Button runat="server" ID="kaydet" Text="Şifreyi Güncelle" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" BackColor="50,205,50" OnClick="kaydet_Click" />
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Literal runat="server" ID="islemsonucu"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>