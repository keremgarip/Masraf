using System;
using System.Data;

namespace Masraf
{
    public partial class setpassword : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        public bool isYoneticimi = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", Request.QueryString["userid"].ToString(), string.Empty, "ConString");
                if (dr != null)
                {
                    email.Text = dr["usr_email_address"].ToString();
                }
            }
        }
        protected void kaydet_Click(object sender, EventArgs e)
        {
            if (sifre.Text.Length < 2)
            {
                islemsonucu.Text = "Lütfen geçerli bir şifre giriniz.";
                return;
            }
            if (sifre.Text != sifretekrar.Text)
            {
                islemsonucu.Text = "Şifreler birbiriyle uyuşmamaktadır.";
            }

            string result = string.Empty;

            result = db.updateValueWhere1("tbl_users", "usr_pass", db.Encrypt(sifre.Text), "usr_user_id", "=", Request.QueryString["userid"].ToString(), "ConString");

            islemsonucu.Text = "Şifreniz başarıyla güncellenmiştir.";
            //kullanıcının yeni şifre oluşturması için kullanılacak link
            //maili gondermeden once, select yapıp , yenı olusturulan kullanıcının idsini alıp, mail linkinin sonuna koyacagız
            //<a href='https://localhost:44374/setpassword.aspx?userId=' style='color:green;'>Şifre belirlemek için tıklayınız.</a>
            //+ "<br/><br/>Şifreniz: " + sifre.Text
            db.ePostaGonder("Masraf Formu Portalı şifreniz başarıyla değişti.", email.Text, "Şifreniz başarıyla güncellenmiştir." + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", string.Empty);
        }
    }
}