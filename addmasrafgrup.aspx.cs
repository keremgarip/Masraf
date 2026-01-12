using System;
using System.Data;

namespace Masraf
{
    public partial class addmasrafgrup : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.checkUser();
            }
        }
        void checkUser()
        {
            try
            {
                if (Request.Cookies["ckmasrafweb"] != null) //Çerez talep ediliyor, çerez yok ise login.aspx sayfasına dönülüyor. Var ise kullanıcının verilerini alıyor.
                {
                    if (db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())) == "false")
                    {
                        Response.Redirect("login.aspx");
                    }
                    else
                    {
                        DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], string.Empty, "ConString");
                        if (dr["usr_rank"].ToString() == "Admin") //Kullanıcının rolü Admin ise bir eylem gerçekleşmiyor, Marketing ise /'a yönleniyor, yoksa login.aspx sayfasına geri dönülüyor
                        {
                        }
                        else if (dr["usr_rank"].ToString() == "Marketing")
                        {
                            Response.Redirect("/");
                        }
                    }
                }
                else
                {
                    Response.Redirect("login.aspx");
                }
            }
            catch (Exception)
            {
                Response.Redirect("login.aspx");
            }

        }
        protected void kaydet_Click(object sender, EventArgs e)
        {
            if (db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())) == "false")
            {
                Response.Redirect("login.aspx");
            }
            else
            {
                if (masrafGrupName.Text.Length < 2)
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir ad giriniz.");
                    return;
                }

                string result = db.insertValue("tbl_masrafGrup", "masrafGrupName,isActive", db.clr(masrafGrupName.Text) + "~" + "1", "ConString");

                if (result.Contains("true") == true)
                {
                    islemsonucu.Text = islembildirimi(true, "Masraf grubu başarıyla eklenmiştir.");
                    masrafGrupName.Text = string.Empty;
                }
                else
                {
                    islemsonucu.Text = islembildirimi(false, "Masraf grubu eklenirken bir hata oluştu. Lütfen sistem yöneticisi ile iletişime geçiniz.");
                }
            }
        }
        public static string islembildirimi(bool islemdurumu, string mesaj)
        {
            string sonuc = string.Empty;
            if (islemdurumu == true)
            {
                sonuc = "<div class=\"alert alert-success-fill alert-dismissible fade in\" role=\"alert\">"
                      + "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">"
                      + "<span aria-hidden=\"true\">×</span>"
                      + "</button>"
                      + "<strong>" + mesaj + "</div>";
            }
            else
            {
                sonuc = "<div class=\"alert alert-danger-fill alert-dismissible fade in mb-0\" role=\"alert\">"
                      + "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">"
                      + "<span aria-hidden=\"true\">×</span>"
                      + "</button>"
                      + "<strong>" + mesaj + "</div>";
            }
            return sonuc;
        }
        protected void vazgec_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafgruplist.aspx");
        }
    }
}