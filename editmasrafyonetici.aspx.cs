using System;
using System.Data;

namespace Masraf
{
    public partial class editmasrafyonetici : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                if (Request.QueryString["secretkey"] == null) Response.Redirect("default.aspx"); //secretkey parametresı yok ise anasayfaya gidiyor.
                else //Bulunmuyorsa düzenleme sayfasına yönlendiriliyor.
                {
                    this.checkUser();

                    DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.Decrypt(Request.QueryString["secretkey"].ToString().Replace(" ", "+")).Split('|')[0].ToString(), string.Empty, "ConString");
                    if (dr != null)
                    {
                        ad.Text = dr["usr_name"].ToString();
                        soyad.Text = dr["usr_surname"].ToString();
                        email.Text = dr["usr_email_address"].ToString();
                        telefon.Text = dr["usr_phone"].ToString();
                    }
                }
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
                if (ad.Text.Length < 2)
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir ad giriniz.");
                    return;
                }
                if (soyad.Text.Length < 2)
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir soyad giriniz.");
                    return;
                }
                if (email.Text.Length < 6 || email.Text.Contains("@") == false || email.Text.Contains(".") == false)
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir E-Posta adresi giriniz.");
                    return;
                }
                if (telefon.Text.Length < 11)
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir telefon numarası giriniz.");
                    return;
                }

                string result = string.Empty;

                result = db.updateValueWhere1("tbl_users", "usr_name,usr_surname,usr_email_address,usr_phone,usr_rank", db.clr(ad.Text) + "~" + db.clr(soyad.Text) + "~" + db.clr(email.Text) + "~" + db.clr(telefon.Text) + "~" + "yonetici", "usr_user_id", "=", db.Decrypt(Request.QueryString["secretkey"].ToString().Replace(" ", "+")).Split('|')[0].ToString(), "ConString");

                // insert user action logs
                if (result.Contains("true") == true)
                {
                    islemsonucu.Text = islembildirimi(true, "Yönetici başarıyla düzenlenmiştir.");
                }
                else
                {
                    islemsonucu.Text = islembildirimi(false, "Yönetici düzenlenirken bir hata oluştu. Lütfen site yöneticisi ile iletişime geçiniz.");
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
            Response.Redirect("masrafyonetici.aspx");
        }
    }
}