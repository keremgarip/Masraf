<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="masrafgruplist.aspx.cs" Inherits="Masraf.masrafgruplist" %>

<%@ Register Assembly="CollectionPager" Namespace="SiteUtils" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <h4 class="formTitle p-0">Masraf Grup Yönetimi</h4>
            <div class="breadcrumbs flex" id="crumbs">
                <ul>
                    <li>
                        <a href="default.aspx">
                            <i class="fa fa-home" aria-hidden="true"></i>
                            Anasayfa
                        </a>
                    </li>
                    <li>
                        <a href="#" style="font-weight: bold;">Masraf Grup Yönetimi
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="col-lg-12 col-md-12 col-xs-12 row white-bg">
            <div class="col-lg-4 col-md-4 col-xs-12">
                <div class="flex">
                    <div class="search-group">
                        <asp:TextBox ID="txtBxGrupName" runat="server" placeholder="Aramak istediğiniz grubu yazın." CssClass="form-control" onkeypress="return DefaultButton(event, '<%= lnkBtnAra.ClientId%>')"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 col-md-4 col-xs-12">
                <div class="flex justifyEnd">
                    <div>
                        <asp:LinkButton ID="lnkBtnAra" runat="server" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" OnClick="lnkBtnAra_Click">Ara</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;
                            <a href="addmasrafgrup.aspx" class="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light">Yeni Masraf Grubu Ekle</a>
                    </div>
                </div>
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
        </div>
    </div>
    <div class="box box-block bg-white">
        <div class="table-responsive">
            <table class="recordTable dataTable no-footer" role="grid" style="width: 1231px;">
                <thead>
                    <tr>
                        <th style="text-align: left;">Masraf Grubu</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="rptMasrafGrup" OnItemCommand="rptMasrafGrup_ItemCommand">
                        <ItemTemplate>
                            <tr role="row" class="odd">
                                <td><%#Eval("masrafGrupName")%></td>
                                <td>
                                    <asp:Button runat="server" ID="btnedit" CommandName="edit" CommandArgument='<%#Eval("masrafGrupId") %>' Text="Düzenle" CssClass="btn btn-success btn-sm w-min-sm mb-0-25 waves-effect waves-light" />
                                    <asp:Button runat="server" ID="btndelete" CommandName="delete" OnClientClick="return confirm('Bu grubu silmek istediğinize emin misiniz?')" CommandArgument='<%#Eval("masrafGrupId") %>' Text="Sil" CssClass="btn btn-danger btn-sm w-min-sm mb-0-25 waves-effect waves-light" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
    <div class="col-lg-12 col-md-12 col-xs-12" style="text-align: center; width: 100% !important; margin-top: 10px;">
        <cc1:CollectionPager ID="paginator" runat="server" BackNextLocation="None" BackText="Önceki Sayfa" HideOnSinglePage="True" LabelText="" NextText="Sonraki Sayfa" QueryStringKey="page" ResultsLocation="Bottom" PageNumbersSeparator="&amp;nbsp;" PageSize="10" ShowLabel="False" MaxPages="100000" PagingMode="QueryString" ResultsFormat="Toplam {2} kayıt içerisinden {0}-{1} arasındakiler gösteriliyor." ResultsStyle="float:left;" BackNextLinkSeparator="" IgnoreQueryString="False" PageNumbersDisplay="Numbers" SectionPadding="0" PageNumberStyle="paginate_button" ControlCssClass="dataTables_paginate paging_simple_numbers" PageNumbersStyle="font-size:20px;"></cc1:CollectionPager>
    </div>
</asp:Content>
