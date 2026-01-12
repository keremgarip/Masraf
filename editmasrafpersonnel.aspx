<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="editmasrafpersonnel.aspx.cs" Inherits="Masraf.editmasrafpersonnel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <h4 class="formTitle p-0">Personel Yönetimi</h4>
            <div class="breadcrumbs flex" id="crumbs">
                <ul>
                    <li>
                        <a href="default.aspx">
                            <i class="fa fa-home" aria-hidden="true"></i>
                            Anasayfa
                        </a>
                    </li>
                    <li>
                        <a href="masrafpersonnels.aspx">Personel Yönetimi
                        </a>
                    </li>
                    <li>
                        <a href="#" style="font-weight:bold;">Personel Düzenle
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="box box-block" style="width: 100%; margin-top: 10px;">
            <div class="row">
                <div class="col-lg-12 col-md-12 col-xs-12 row white-bg">
                    <div class="form-group">
                        <label>Ad</label>
                        <asp:TextBox runat="server" ID="ad" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Soyad</label>
                        <asp:TextBox runat="server" ID="soyad" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Email</label>
                        <asp:TextBox runat="server" ID="email" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Telefon</label>
                        <asp:TextBox runat="server" ID="telefon" MaxLength="11" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Yönetici</label>
                        <asp:DropDownList ID="masrafYonetici" runat="server" CssClass="form-control"></asp:DropDownList>
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