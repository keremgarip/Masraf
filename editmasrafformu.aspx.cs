using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Masraf
{
    public partial class editmasrafformu : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack) //Veritabanında data var mı yok mu kontrol ediliyor.
            {
                this.checkUser();

                DataTable dtMasrafGrubu = null;
                DataTable dtMasrafYonetici = null;

                dtMasrafGrubu = db.getDataTable("select * from tbl_masrafGrup where isActive = 1 order by masrafGrupId asc", "ConString");
                if ((dtMasrafGrubu != null) && dtMasrafGrubu.Rows.Count > 0)
                {
                    this.ddlMasrafGrup.DataSource = dtMasrafGrubu;
                    this.ddlMasrafGrup.DataValueField = "masrafGrupId";
                    this.ddlMasrafGrup.DataTextField = "masrafGrupName";
                    this.ddlMasrafGrup.DataBind();

                    this.ddlMasrafGrup.Items.Insert(0, new ListItem("-Seçiniz-", "-1"));
                }

                dtMasrafYonetici = db.getDataTable("select * from tbl_users where usr_status = 1 and usr_rank='yonetici' order by usr_user_id asc", "ConString");
                if ((dtMasrafYonetici != null) && dtMasrafYonetici.Rows.Count > 0)
                {
                    this.ddlMasrafYonetici.DataSource = dtMasrafYonetici;
                    this.ddlMasrafYonetici.DataValueField = "usr_user_id";
                    this.ddlMasrafYonetici.DataTextField = "usr_name";
                    this.ddlMasrafYonetici.DataBind();

                    this.ddlMasrafYonetici.Items.Insert(0, new ListItem("-Seçiniz-", "-1"));
                }

                DataRow dr = db.getValueWhere1("select * from tbl_masrafFormu", "masrafFormuId", "=", db.Decrypt(Request.QueryString["secretkey"].ToString().Replace(" ", "+")).Split('|')[0].ToString(), string.Empty, "ConString");
                if (dr != null)
                {
                    DateTime dtMt = DateTime.Parse(dr["masrafFormuDate"].ToString());
                    masrafTarih.Text = dtMt.ToString("yyyy-MM-dd");
                    masrafKimden.Text = dr["masrafFormuKimdenAlindigi"].ToString();
                    masrafAciklama.Text = dr["masrafFormuAciklama"].ToString();
                    ddlMasrafGrup.SelectedValue = dr["masrafFormuGrupId"].ToString();
                    ddlMasrafYonetici.SelectedValue = dr["masrafFormuOnaylayacakId"].ToString();
                    //ddlMasrafYonetici.Items.FindByText(dr["masrafFormuOnaylayacakId"].ToString()).Selected = true;
                    masrafFis.Text = dr["masrafFormuOKCFisNo"].ToString();
                    masrafGider.Text = dr["masrafFormuGiderAnaHesabi"].ToString();
                    masrafTutar.Text = dr["masrafFormuKDVDahilTutar"].ToString();
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
                if (masrafTarih.Text == null) //Ad uzunluğu 2'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen tarih seçiniz.");
                    return;
                }
                if (masrafKimden.Text.Length < 2) //Soyad uzunluğu 2'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir hedef giriniz.");
                    return;
                }
                if (masrafAciklama.Text.Length < 2) //Mail uzunluğu 6'dan küçük, veya @ veya . işareti yoksa uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir açıklama giriniz.");
                    return;
                }
                if (ddlMasrafGrup.SelectedValue == null) //Telefon numarası uzunluğu 11'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen bir masraf grubu seçiniz.");
                    return;
                }

                if (masrafFis.Text.Length < 1) //Telefon numarası uzunluğu 11'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir fiş numarası giriniz.");
                    return;
                }

                if (masrafGider.Text.Length < 1) //Telefon numarası uzunluğu 11'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir gider ana hesabı giriniz.");
                    return;
                }

                if (masrafTutar.Text.Length < 1) //Telefon numarası uzunluğu 11'den küçük ise uyarı veriyor.
                {
                    islemsonucu.Text = islembildirimi(false, "Lütfen geçerli bir tutar giriniz.");
                    return;
                }

                decimal masrafFormuKKEG = 0;
                if (ddlMasrafGrup.SelectedItem.Value == "10" || ddlMasrafGrup.SelectedItem.Value == "12")//AKARYAKIT || OTOPARK-ARAÇ YIKAMA
                {
                    masrafFormuKKEG = (Convert.ToDecimal(masrafTutar.Text) / 100) * 30;
                }

                DataTable dtYonetici = null;
                string yoneticiIdStr = string.Empty;

                dtYonetici = db.getDataTable("SELECT * FROM tbl_users where usr_status = 1 and usr_user_id=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], "ConString");
                if ((dtYonetici != null) && dtYonetici.Rows.Count > 0)
                {
                    yoneticiIdStr = dtYonetici.Rows[0]["yoneticiId"].ToString();
                }

                string result = db.updateValueWhere1("tbl_masrafFormu", "masrafFormuKimdenAlindigi,masrafFormuAciklama,masrafFormuGrupId,masrafFormuOKCFisNo,masrafFormuGiderAnaHesabi,masrafFormuKDVDahilTutar,isActive,masrafFormuDate,masrafFormuOnaylayacakId,masrafFormuKKEG", db.clr(masrafKimden.Text) + "~" + db.clr(masrafAciklama.Text) + "~" + db.clr(ddlMasrafGrup.SelectedValue) + "~" + db.clr(masrafFis.Text) + "~" + masrafGider.Text + "~" + db.clr(masrafTutar.Text) + "~" + "1" + "~é" + db.clr(masrafTarih.Text) + "~" + yoneticiIdStr + "~" + masrafFormuKKEG, "masrafFormuId", "=", db.Decrypt(Request.QueryString["secretkey"].ToString().Replace(" ", "+")).Split('|')[0].ToString(), "ConString");

                if (result.Contains("true") == true)
                {
                    islemsonucu.Text = islembildirimi(true, "Masraf başarıyla düzenlenmiştir.");
                }
                else
                {
                    islemsonucu.Text = islembildirimi(false, "Masraf düzenlenirken bir hata oluştu. Lütfen site yöneticisi ile iletişime geçiniz.");
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
            Response.Redirect("masrafformulist.aspx");
        }
    }
}