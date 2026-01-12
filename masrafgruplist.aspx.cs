using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Masraf
{
    public partial class masrafgruplist : System.Web.UI.Page
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
                        this.getMasrafGrupByName(Request.QueryString["k"].ToString());
                    }
                }
                else
                {
                    this.getMasrafGrup();
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
                        DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], "", "ConString");
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
        void getMasrafGrup()
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafGrup where isActive=1 order by masrafGrupId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                paginator.DataSource = dt.DefaultView;
                paginator.BindToControl = rptMasrafGrup;
                rptMasrafGrup.DataSource = paginator.DataSourcePaged;
                rptMasrafGrup.DataBind();
            }
        }
        void getMasrafGrupByName(string masrafGrupName)
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafGrup where isActive = 1 and masrafGrupName like '%" + masrafGrupName + "%' order by masrafGrupId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                paginator.DataSource = dt.DefaultView;
                paginator.BindToControl = rptMasrafGrup;
                rptMasrafGrup.DataSource = paginator.DataSourcePaged;
                rptMasrafGrup.DataBind();
            }
        }
        protected void rptMasrafGrup_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                Random rnd = new Random();
                string parameter = db.Encrypt(e.CommandArgument.ToString() + "|" + rnd.Next(1111, 9999)).ToString();
                Response.Redirect("editmasrafgrup.aspx?secretkey=" + parameter);
            }
            else if (e.CommandName == "delete")
            {
                db.updateValueWhere1("tbl_masrafGrup", "isActive", "0", "masrafGrupId", "=", e.CommandArgument.ToString(), "ConString");

                Response.Redirect("masrafgruplist.aspx");
            }
        }
        protected void lnkBtnAra_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafgruplist.aspx?k=" + this.txtBxGrupName.Text.Trim());
        }
    }
}