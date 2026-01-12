using System;
using System.Web;
using System.Data;

namespace Masraf
{
    public partial class login : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Request.Cookies["ckmasrafweb"] != null) //Çerez kabul eden kod, hesabın id'sini veritabanından çekiyor, yoksa default.aspx sayfasına geri yönlendiriyor.
            //{
            //    //cookie daha onceden yaratılmıs ise, kullanıcıyı dırekt admın anasayfaya yonlendırıyoruz
            //    DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_lastsession_id", "=", db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString()), string.Empty, "ConString");
            //    if (dr != null)
            //    {
            //        Response.Redirect("default.aspx");
            //    }
            //}

            //DataRow dr2 = db.getValueWhere3("select count(log_id) as count from tbl_login_logs", "log_IP", "=", db.getIPAdress(""), "and", "log_datetime", ">", DateTime.Now.AddDays(-1).ToString("MM.dd.yyyy hh:mm"), "and", "log_status", "=", "0", "", "ConString"); //Kullanıcının bilgilerini ID yoluyla veritabanından alıyor.
            //if (dr2 != null) //Değeri veritabanından alamazsa 403 hata mesajı geliyor.
            //{
            //    if (Convert.ToInt32(dr2["count"].ToString()) >= 5)
            //    {
            //        Response.Redirect("/" + "error.aspx?code=403");
            //    }
            //}
        }
        protected void girisyap_Click(object sender, EventArgs e)
        {
            DataRow dr = db.getValueWhere3("select * from tbl_users", "usr_email_address", "=", db.clr(exampleInputEmail.Text), "and", "usr_pass", "=", db.Encrypt(exampleInputPassword.Text), "and", "usr_status", "=", "1", string.Empty, "ConString"); //Mail adresi ve şifreyi veritabanından algılıyor ve alıyor.
            if (dr != null) //dr inputları null değil ise, kullanıcının son giriş tatihini ve id'sini güncelliyor. Kullanıcının giriş yaptığı IP adresini, ID'sini veritabanına ekliyor ve onun adına cookie oluşturuluyor adminCookie adıyla.
            {
                string sessionID;
                sessionID = Guid.NewGuid().ToString();

                // basarılı logın olduk isek, login_date field ı guncellenıyor
                db.updateValueWhere1("tbl_users", "usr_lastlogin_date,usr_lastsession_id", "é" + DateTime.Now + "~" + sessionID.ToString(), "usr_user_id", "=", dr["usr_user_id"].ToString(), "ConString");

                string logid = Guid.NewGuid().ToString();
                // login basarılı ise, logın olan kullanıcının , logın olma tarıhını ve ip adresını kaydedıyoruz
                //db.insertValue("tbl_login_logs", "log_id,usr_user_id,log_IP,log_datetime,log_status", logid + "~" + dr["usr_user_id"].ToString() + "~" + db.getIPAdress("") + "~é" + DateTime.Now + "~" + "1", "ConString");

                // basarılı logın ıslemınden sonra, kullanıcıya ozel cookie yaratılıyor, bu cookinın ıcınde de, şifrelenmıs halde userid ve şifresız olarak kullanıcının ad ve soyad bılgısını cookıye kaydedıyoruz, ckmasrafweb adında cookie yaratıyoruz ve ıcındeki bılgılerde kullanıcıya ozel
                HttpCookie adminCookie = new HttpCookie("ckmasrafweb");
                adminCookie["SessID"] = db.Encrypt(sessionID.ToString());
                adminCookie["Name"] = dr["usr_name"].ToString() + " " + dr["usr_surname"].ToString();
                adminCookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(adminCookie);

                Response.Redirect("default.aspx");
            }
            else //dr inputları null ise dr2 adıyla yeni bir DataRow oluşturuluyor ve mail adresini algılıyor.
            {
                //DataRow dr2 = db.getValueWhere2("select * from tbl_users", "usr_email_address", "=", db.clr(exampleInputEmail.Text), "and", "usr_status", "=", "1", string.Empty, "ConString");
                //if (dr2 != null) //dr2 null değil ise, kullanıcının giriş yapma tarihi, saati, IP adresi ve ID'si veritabanına ekleniyor.
                //{
                //    string logid = Guid.NewGuid().ToString();
                //    db.insertValue("tbl_login_logs", "log_id,usr_user_id,log_IP,log_datetime,log_status", logid + "~" + dr2["usr_user_id"].ToString() + "~" + db.getIPAdress("") + "~é" + DateTime.Now + "~" + "0", "ConString");
                //    DataRow dr3 = db.getValueWhere3("select count(log_id) as count from tbl_login_logs", "usr_user_id", "=", dr2["usr_user_id"].ToString(), "and", "log_datetime", ">", DateTime.Now.AddDays(-1).ToString("MM.dd.yyyy hh:mm"), "and", "log_status", "=", "0", string.Empty, "ConString");
                //    if (Convert.ToInt32(dr3["count"].ToString()) >= 5) //Veritabanındaki değerler 5'ten fazla ya da eşit ise kullanıcının durumu alınıyor. 
                //    {
                //        //5 den fazla yanlıs login denemesı yapıldıgında, bu kullanıcının status unu pasıf yapıyoruz , boylelıkle kullanıcı sıfremı unuttum bolumune gıtmek zorunda kalıyor
                //        db.updateValueWhere1("tbl_users", "usr_status", "0", "usr_user_id", "=", dr2["usr_user_id"].ToString(), "ConString");
                //    }
                //}
                //else //dr2 null ise kullanıcının ID, IP adresi, durumu ve giriş yapmaya çalıştığı tarihi veritabanına ekliyor.
                //{
                //    string logid = Guid.NewGuid().ToString();
                //    db.insertValue("tbl_login_logs", "log_id,log_IP,log_datetime,log_status", logid + "~" + db.getIPAdress("") + "~é" + DateTime.Now + "~" + "0", "ConString");
                //}
                Label1.Text = "Kullanıcı adınız veya şifreniz hatalı.";
            }
        }
    }
}