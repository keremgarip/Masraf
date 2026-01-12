<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="masrafaylikdetay.aspx.cs" Inherits="Masraf.masrafaylikdetay" %>

<%@ Register Assembly="CollectionPager" Namespace="SiteUtils" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <h4 class="formTitle p-0">Masraflarım</h4>
            <div class="breadcrumbs flex" id="crumbs">
                <ul>
                    <li>
                        <a href="default.aspx">
                            <i class="fa fa-home" aria-hidden="true"></i>
                            Anasayfa
                        </a>
                    </li>
                    <li>
                        <a href="#" style="font-weight: bold;">Masraflarım
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="box box-block bg-white" style="width: 100%;">
            <div class="table-responsive">
                <table class="recordTable dataTable no-footer" role="grid">
                    <thead>
                        <tr>
                            <th>Tarih</th>
                            <th>Personel</th>
                            <th>Kimden Alındı</th>
                            <th>Masraf Açıklaması</th>
                            <th>Masraf Grubu</th>
                            <th>Fiş Numarası</th>
                            <th>Gider Ana Hesabı</th>
                            <th>Toplam Tutar</th>
                            <th>KKEG</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="rptMasrafFormu" OnItemDataBound="rptMasrafFormu_ItemDataBound">
                            <ItemTemplate>
                                <asp:Label ID="lblMasrafFormuId" runat="server" Style="display: none;"></asp:Label>
                                <asp:Label ID="lblMasrafFormuKDVDahilTutar" runat="server" Style="display: none;"></asp:Label>
                                <asp:Label ID="lblCreatedUserId" runat="server" Style="display: none;"></asp:Label>
                                <tr id="trRow" runat="server">
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
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr>
                            <th colspan="10" style="text-align: right;">
                                <asp:Literal ID="ltrlToplamTutar" runat="server"></asp:Literal>
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
    <tr>
        <th colspan="10" style="text-align: right">
            <asp:Button ID="btnGeri" runat="server" Text="Anasayfaya Geri Dön" BackColor="#3658C7" ForeColor="White" Font-Bold="true" Font-Size="Large" OnClick="btnGeri_OnClick" />
        </th>
    </tr>
</asp:Content>
