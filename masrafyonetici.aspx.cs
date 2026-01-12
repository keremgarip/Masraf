using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Masraf
{
    public partial class masrafyonetici : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.Page.Form.DefaultButton = lnkBtnAra.UniqueID;

                this.checkUser();

                if (Request.QueryString["k"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["k"].ToString()))
                    {
                        this.getYoneticiByName(Request.QueryString["k"].ToString());
                    }
                }
                else
                {
                    this.getpersonnels();
                }
                if (Request.QueryString["mode"] != null)
                {
                    if (Request.QueryString["mode"].ToString() == "resetpass")
                    {
                        ltrlResult.Text = "Yöneticiye şifre sıfırlama E-Postası başarıyla gönderildi.";
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
        void getpersonnels()
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_users where usr_status=1 and usr_rank='yonetici' order by usr_name asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                paginator.DataSource = dt.DefaultView;
                paginator.BindToControl = rptyonetici;
                rptyonetici.DataSource = paginator.DataSourcePaged;
                rptyonetici.DataBind();
            }
        }
        protected void rptyonetici_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                Random rnd = new Random();
                string parameter = db.Encrypt(e.CommandArgument.ToString() + "|" + rnd.Next(1111, 9999)).ToString();
                Response.Redirect("editmasrafyonetici.aspx?secretkey=" + parameter);
            }
            else if (e.CommandName == "delete")
            {
                db.updateValueWhere1("tbl_users", "usr_status,usr_last_update_datetime", "0" + "~é" + DateTime.Now, "usr_user_id", "=", e.CommandArgument.ToString(), "ConString");

                Response.Redirect("masrafyonetici.aspx");
            }
            else if (e.CommandName == "resetpass")
            {
                Label lblEposta = (Label)e.Item.FindControl("lblEposta");

                db.ePostaGonder("Şifre sıfırlama", lblEposta.Text, "Şifre sıfırlamak için <a href=\"http://www.kf-ik.com/setpassword.aspx?userid=" + e.CommandArgument + "\">tıklayınız</a>." + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' width='701' height='200' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", string.Empty);

                Response.Redirect("masrafyonetici.aspx?mode=resetpass");
            }
        }
        void getYoneticiByName(string yoneticiName)
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_users where usr_status=1 and usr_rank='yonetici' and usr_name like '%" + yoneticiName + "%' order by usr_user_id asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                paginator.DataSource = dt.DefaultView;
                paginator.BindToControl = rptyonetici;
                rptyonetici.DataSource = paginator.DataSourcePaged;
                rptyonetici.DataBind();
            }
        }
        protected void lnkBtnAra_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafyonetici.aspx?k=" + this.txtBxYoneticiName.Text.Trim());
        }
    }
}