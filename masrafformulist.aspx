<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="masrafformulist.aspx.cs" Inherits="Masraf.masrafformulist" %>

<%@ Register Assembly="CollectionPager" Namespace="SiteUtils" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <h4 class="formTitle p-0">Masraf Yönetimi</h4>
            <div class="breadcrumbs flex" id="crumbs">
                <ul>
                    <li>
                        <a href="default.aspx">
                            <i class="fa fa-home" aria-hidden="true"></i>
                            Anasayfa
                        </a>
                    </li>
                    <li>
                        <a href="#" style="font-weight: bold;">Masraf Yönetimi
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="col-lg-12 col-md-12 col-xs-12 row white-bg">
            <div class="col-lg-4 col-md-4 col-xs-12">
                <div class="flex">
                    <div class="search-group">
                        <asp:TextBox ID="txtBxMasraf" runat="server" placeholder="Aramak istediğiniz masrafı yazın." CssClass="form-control" onkeypress="return DefaultButton(event, '<%= lnkBtnAra.ClientId%>')"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="col-lg-8 col-md-8 col-xs-12">
                <div class="flex justifyEnd">
                    <div>
                        <asp:LinkButton ID="lnkBtnAra" runat="server" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" OnClick="lnkBtnAra_Click">Ara</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;
                            <a href="addmasrafformu.aspx" class="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light">Yeni Masraf Ekle</a>
                    </div>
                </div>
            </div>
            <div class="col-lg-8 col-md-8 col-xs-12">
                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" Width="200px"></asp:DropDownList>
                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" Width="200px"></asp:DropDownList>
            </div>
            <div class="col-lg-4 col-md-4 col-xs-12">
                <div class="flex justifyEnd">
                    <div>
                        <asp:LinkButton ID="lnkBtnFiltrele" runat="server" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" OnClick="lnkBtnFiltrele_Click">Filtrele</asp:LinkButton>
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
        <div class="box box-block bg-white" style="width: 100%;">
            <div class="table-responsive">
                <span style="color: red;">* Kırmızı satır, masraf onaylanmadı anlamına gelir.
                    <br />
                    * Yeşil satır, masraf onaylandı anlamına gelir.
                </span>
                <table class="recordTable dataTable no-footer" role="grid">
                    <thead>
                        <tr>
                            <th>Sıra No</th>
                            <th>Tarih</th>
                            <th>Personel</th>
                            <th>Kimden Alındı</th>
                            <th>Masraf Açıklaması</th>
                            <th>Masraf Grubu</th>
                            <th>Fiş Numarası</th>
                            <th>Gider Ana Hesabı</th>
                            <th>Toplam Tutar</th>
                            <th>KKEG</th>
                            <th>İŞLEMLER</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="rptMasrafFormu" OnItemCommand="rptMasrafFormu_ItemCommand" OnItemDataBound="rptMasrafFormu_ItemDataBound">
                            <ItemTemplate>
                                <asp:Label ID="lblMasrafFormuId" runat="server" Style="display: none;"></asp:Label>
                                <asp:Label ID="lblMasrafFormuKDVDahilTutar" runat="server" Style="display: none;"></asp:Label>
                                <asp:Label ID="lblCreatedUserId" runat="server" Style="display: none;"></asp:Label>
                                <tr id="trRow" runat="server">
                                    <td> <asp:Label ID="lblMasrafSiraNo" runat="server" class="row-number"></asp:Label> </td>
                                    <td>
                                        <asp:Literal ID="ltrlMasrafFormuDate" runat="server"></asp:Literal>
                                        <td><%#Eval("usr_name")%> <%#Eval("usr_surname")%></td>
                                    </td>
                                    <td>
                                        <asp:Literal ID="ltrlMasrafFormuKimdenAlindigi" runat="server"></asp:Literal>
                                    </td>
                                    <td><%#Eval("masrafFormuAciklama")%></td>
                                    <td>
                                        <asp:Literal ID="ltrlMasrafGrup" runat="server"></asp:Literal>
                                    </td>
                                    <td><%#Eval("masrafFormuOKCFisNo")%></td>
                                    <td><%#Eval("masrafFormuGiderAnaHesabi")%></td>
                                    <td>
                                        <asp:Literal ID="ltrlKDVsiz" runat="server"></asp:Literal>&nbsp;TL</td>
                                    <td><%#Eval("masrafFormuKKEG")%>&nbsp;TL</td>
                                    <td>
                                        <asp:Button runat="server" ID="btnedit" CommandName="edit" CommandArgument='<%#Eval("masrafFormuId") %>' Text="Düzenle" CssClass="btn btn-success btn-sm w-min-sm mb-0-25 waves-effect waves-light" />
                                        <asp:Button runat="server" ID="btndelete" CommandName="delete" OnClientClick="return confirm('Bu masrafı silmek istediğinize emin misiniz?')" CommandArgument='<%#Eval("masrafFormuId") %>' Text="Sil" CssClass="btn btn-danger btn-sm w-min-sm mb-0-25 waves-effect waves-light" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr>
                            <th colspan="11" style="text-align: center; color: red; font-weight: bold; font-size: 14px;">
                                <br />
                                <br />
                                <asp:Literal ID="ltrlResult" runat="server"></asp:Literal>
                            </th>
                        </tr>
                        <tr>
                            <th colspan="11" style="text-align: right;">
                                <asp:Literal ID="ltrlToplamTutar" runat="server"></asp:Literal>
                            </th>
                        </tr>
                        <tr>
                            <th colspan="11">
                                <asp:FileUpload ID="fuPDF" runat="server" />
                            </th>
                        </tr>
                        <tr>
                            <th colspan="11">
                                <asp:Button ID="btnYoneticiyeGonder" runat="server" Text="Yöneticiye Gönder" OnClick="btnYoneticiyeGonder_Click" Style="background-color: red; color: white !important;" />
                            </th>
                        </tr>
                        <tr>
                            <th colspan="11">
                                <asp:Literal ID="ltrlInfoYg" runat="server" Visible="false"></asp:Literal>
                            </th>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="col-lg-12 col-md-12 col-xs-12" style="text-align: right; width: 100% !important; margin-top: 10px;">
            <cc1:CollectionPager ID="paginator" runat="server" BackNextLocation="None" BackText="Önceki Sayfa" HideOnSinglePage="True" LabelText="" NextText="Sonraki Sayfa" QueryStringKey="page" ResultsLocation="Bottom" PageNumbersSeparator="&amp;nbsp;" PageSize="100" ShowLabel="False" MaxPages="100000" PagingMode="QueryString" ResultsFormat="Toplam {2} kayıt içerisinden {0}-{1} arasındakiler gösteriliyor." ResultsStyle="float:left;" BackNextLinkSeparator="" IgnoreQueryString="False" PageNumbersDisplay="Numbers" SectionPadding="0" PageNumberStyle="paginate_button" ControlCssClass="dataTables_paginate paging_simple_numbers" PageNumbersStyle="font-size:20px;"></cc1:CollectionPager>
        </div>
        </div>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            var repeaterRows = document.querySelectorAll("#<%= rptMasrafFormu.ClientID %> tr");
        var rowCount = 1;

        repeaterRows.forEach(function (row) {
            var rowNumberCell = row.querySelector("#<%= rptMasrafFormu.ClientID %> .row-number");
            if (rowNumberCell) {
                rowNumberCell.innerHTML = rowCount++;
            }
        });
    });
    </script>
</asp:Content>

