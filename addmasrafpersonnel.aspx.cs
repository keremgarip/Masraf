using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Masraf
{
    public partial class addmasrafpersonnel : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack) //Veritabanında data var mı yok mu kontrol ediliyor.
            {
                this.checkUser();
                DataTable dtYonetici = null;

                dtYonetici = db.getDataTable("select *, (usr_name + ' ' + usr_surname) as adSoyad from tbl_users where usr_status = 1 and usr_rank = 'yonetici' order by usr_user_id asc", "ConString");
                if ((dtYonetici != null) && dtYonetici.Rows.Count > 0)
                {
                    this.masrafYonetici.DataSource = dtYonetici;
                    this.masrafYonetici.DataValueField = "usr_user_id";
                    this.masrafYonetici.DataTextField = "adSoyad";
                    this.masrafYonetici.DataBind();

                    this.masrafYonetici.Items.Insert(0, new ListItem("-Seçiniz-", "-1"));
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
            if (db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())) == "false") //Çerez kontol ediliyor, bulunamadıysa login.aspx'e yönlendiriliyor.
            {
                Response.Redirect("login.aspx");
            }
            else
            {
                if (ad.Text.Length < 2) //Ad uzunluğu 2'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir ad giriniz.");
                    return;
                }
                if (soyad.Text.Length < 2) //Soyad uzunluğu 2'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir soyad giriniz.");
                    return;
                }
                if (email.Text.Length < 6 || email.Text.Contains("@") == false || email.Text.Contains(".") == false) //Mail uzunluğu 6'dan küçük, veya @ veya . işareti yoksa uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir E-Posta adresi giriniz.");
                    return;
                }
                if (telefon.Text.Length < 11) //Telefon numarası uzunluğu 11'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir telefon numarası giriniz.");
                    return;
                }

                if (masrafYonetici.SelectedValue == null)
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen bir yönetici seçiniz.");
                    return;
                }

                DataRow dr = db.getValueWhere3("select usr_email_address from tbl_users", "usr_status", "=", "1", "and", "usr_email_address", "=", email.Text, "or", "usr_phone", "=", telefon.Text, string.Empty, "ConString"); //Verileri veritabanında topluyor.
                if (dr != null) //dr verileri zaten veritabanında kayıtlı ise, kullanıcıya uyarı mesajı veriliyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Bu E-Posta adresi yada telefon numarasına kayıtlı personel bulunmaktadır.");
                    return;
                }

                string code = string.Empty;
                Random rnd = new Random();
                code = db.MD5(email.Text + "~" + telefon.Text + "~" + DateTime.Now.ToString() + "~" + rnd.Next(1111, 9999));
                string result = db.insertValue("tbl_users", "usr_name,usr_surname,usr_email_address,usr_phone,yoneticiId,usr_regist_date,usr_status,usr_rank,usr_code,usr_codeuse", db.clr(ad.Text) + "~" + db.clr(soyad.Text) + "~" + db.clr(email.Text) + "~" + db.clr(telefon.Text) + "~" + db.clr(masrafYonetici.SelectedValue) + "~é" + DateTime.Now + "~" + "1" + "~" + "personel" + "~" + code + "~" + "0", "ConString");

                DataTable dtUser = null;
                string userId = string.Empty;

                dtUser = db.getDataTable("select * from tbl_users where usr_status = 1 and usr_rank = 'personel' and usr_email_address='" + email.Text.Trim() + "'", "ConString");
                if ((dtUser != null) && dtUser.Rows.Count > 0)
                {
                    userId = dtUser.Rows[0]["usr_user_id"].ToString();

                    db.ePostaGonder("Masraf formu portalına hoşgeldin.", email.Text.Trim(), "Aşağıdaki linke tıklayarak şifreni belirle<br/><br/><a href='http://www.kf-ik.com/setpassword.aspx?userId=" + userId + "'>Şifreni belirlemek için tıkla</a>" + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' width='701' height='200' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", string.Empty);
                }

                if (result.Contains("true") == true)
                {
                    islemsonucu.Text = islembildirimi(true, "Personel başarıyla eklenmiştir.");
                    this.ad.Text = this.soyad.Text = this.telefon.Text = this.email.Text = string.Empty;
                }
                else
                {
                    islemsonucu.Text = islembildirimi(false, "Personel eklenirken bir hata oluştu. Lütfen sistem yöneticisi ile iletişime geçiniz.");
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
            Response.Redirect("masrafpersonnels.aspx");
        }
    }
}