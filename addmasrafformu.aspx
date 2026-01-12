<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="addmasrafformu.aspx.cs" Inherits="Masraf.addmasrafformu"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <h4 class="formTitle p-0">Masraf Formu Yönetimi</h4>
            <div class="breadcrumbs flex" id="crumbs">
                <ul>
                    <li>
                        <a href="default.aspx">
                            <i class="fa fa-home" aria-hidden="true" onclick="resetInactivityTimer()"></i>
                            Anasayfa
                        </a>
                    </li>
                    <li>
                        <a href="masrafformulist.aspx" onclick="resetInactivityTimer()">Masraf Yönetimi
                        </a>
                    </li>
                    <li>
                        <a href="#" style="font-weight:bold;">Masraf Ekle
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="box box-block" style="width: 100%; margin-top: 10px;">
            <div class="row">
                <div class="col-lg-12 col-md-12 col-xs-12 row white-bg">
                    <div class="form-group">
                        <label>Tarih<span style="color: red;">*</span></label>
                        <asp:TextBox runat="server" ID="masrafTarih" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Kimden Alındı</label>
                        <asp:TextBox runat="server" ID="masrafKimden" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Masraf Açıklaması</label>
                        <asp:TextBox runat="server" ID="masrafAciklama" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Masraf Grubu</label>
                        <asp:DropDownList ID="ddlMasrafGrup" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="form-group" style="display:none;">
                        <label>Gönderilecek Yönetici</label>
                        <asp:DropDownList ID="ddlMasrafYonetici" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Fiş Numarası</label>
                        <asp:TextBox runat="server" ID="masrafFis" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Gider Ana Hesabı</label>
                        <asp:TextBox runat="server" ID="masrafGider" CssClass="form-control" Text="740" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>KDV Dahil Tutar</label>
                        <asp:TextBox runat="server" ID="masrafTutar" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-lg-4 col-md-4 col-xs-12">
                        <div class="flex justifyEnd">
                            <div class="form-group">
                                <asp:Button runat="server" ID="kaydet" Text="Kaydet" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" Font-Bold="true" BackColor="#4dac4a" ForeColor="255,255,255" OnClick="kaydet_Click" />
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-4 col-xs-12">
                        <div class="flex justifyEnd">
                            <div class="form-group">
                                <asp:Button runat="server" ID="vazgec" Text="Vazgeç" CssClass="btn btn-primary w-min-sm mb-1-00 waves-effect waves-light" BackColor="#3658c7" ForeColor="255,255,255" Font-Bold="true" OnClick="vazgec_Click" />
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
</asp:Content>