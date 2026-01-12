<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="masrafyonetici.aspx.cs" Inherits="Masraf.masrafyonetici" %>

<%@ Register Assembly="CollectionPager" Namespace="SiteUtils" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <h4 class="formTitle p-0">Yöneticiler</h4>
            <div class="breadcrumbs flex" id="crumbs">
                <ul>
                    <li>
                        <a href="default.aspx">
                            <i class="fa fa-home" aria-hidden="true"></i>
                            Anasayfa
                        </a>
                    </li>
                    <li>
                        <a href="#" style="font-weight: bold;">Yöneticiler
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="col-lg-12 col-md-12 col-xs-12 row white-bg">
            <table style="width: 100% !important;">
                <tr>
                    <td style="width: 300px;">
                        <asp:TextBox ID="txtBxYoneticiName" runat="server" placeholder="Aramak istediğiniz yöneticiyi yazın." CssClass="form-control" onkeypress="return DefaultButton(event, '<%= lnkBtnAra.ClientId%>')"></asp:TextBox></td>
                    <td style="width: 100px; padding-left: 20px; line-height: 65px; padding-top: 15px;">
                        <asp:LinkButton ID="lnkBtnAra" runat="server" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" OnClick="lnkBtnAra_Click">Ara</asp:LinkButton></td>
                    <td style="width: 200px;"></td>
                    <td style="width: 100px; padding-left: 20px; line-height: 65px; padding-top: 15px;">
                </tr>
                <tr>
                    <td colspan="4" style="text-align: right;">
                        <a href="addmasrafyonetici.aspx" class="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light">Yeni Yönetici Ekle</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="box box-block bg-white">
        <div class="table-responsive">
            <table class="recordTable dataTable no-footer" role="grid" style="width: 1231px;">
                <thead>
                    <tr>
                        <th>Ad Soyad</th>
                        <th>Email</th>
                        <th>Telefon</th>
                        <th>İşlemler</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th colspan="5" style="text-align: center; color: #3658C7; font-weight: bold; font-size: 18px;">
                            <asp:Literal ID="ltrlResult" runat="server"></asp:Literal>
                            <br />
                            <br />
                        </th>
                    </tr>
                    <asp:Repeater runat="server" ID="rptyonetici" OnItemCommand="rptyonetici_ItemCommand">
                        <ItemTemplate>
                            <asp:Label ID="lblEposta" runat="server" Visible="false"></asp:Label>
                            <tr role="row" class="odd">
                                <td><%#Eval("usr_name") + " " + Eval("usr_surname") %></td>
                                <td><%#Eval("usr_email_address") %></td>
                                <td><%#Eval("usr_phone") %></td>
                                <td>
                                    <asp:Button runat="server" ID="btnedit" CommandName="edit" CommandArgument='<%#Eval("usr_user_id") %>' Text="Düzenle" CssClass="btn btn-success btn-sm w-min-sm mb-0-25 waves-effect waves-light" />
                                    <asp:Button runat="server" ID="btndelete" CommandName="delete" OnClientClick="return confirm('Bu yöneticiyi silmek istediğinize emin misiniz?')" CommandArgument='<%#Eval("usr_user_id") %>' Text="Sil" CssClass="btn btn-danger btn-sm w-min-sm mb-0-25 waves-effect waves-light" />
                                    <asp:Button runat="server" ID="btnResetPass" CommandName="resetpass" OnClientClick="return confirm('Bu üyeye şifre sıfırlama epostası gidecek emin misiniz?')" CommandArgument='<%#Eval("usr_user_id") %>' Text="Şifre Sıfırla" CssClass="btn btn-success btn-sm w-min-sm mb-0-25 waves-effect waves-light" Style="background-color: #3658C7 !important;" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
    <div class="col-lg-12 col-md-12 col-xs-12" style="text-align: right; width: 100% !important; margin-top: 10px;">
        <cc1:CollectionPager ID="paginator" runat="server" BackNextLocation="None" BackText="Önceki Sayfa" HideOnSinglePage="True" LabelText="" NextText="Sonraki Sayfa" QueryStringKey="page" ResultsLocation="Bottom" PageNumbersSeparator="&amp;nbsp;" PageSize="10" ShowLabel="False" MaxPages="100000" PagingMode="QueryString" ResultsFormat="Toplam {2} kayıt içerisinden {0}-{1} arasındakiler gösteriliyor." ResultsStyle="float:left;" BackNextLinkSeparator="" IgnoreQueryString="False" PageNumbersDisplay="Numbers" SectionPadding="0" PageNumberStyle="paginate_button" ControlCssClass="dataTables_paginate paging_simple_numbers" PageNumbersStyle="font-size:20px;"></cc1:CollectionPager>
    </div>
    <script>
        function DefaultButton(event, buttonId) {
            if (event.keyCode === 13) {
                document.getElementById(buttonId).click();
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
