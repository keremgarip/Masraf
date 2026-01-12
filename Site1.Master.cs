using System;
using System.Data;

namespace Masraf
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        dbLibrary db = new dbLibrary();
        public string loggedInUserNameStr = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.checkUser();

                if (Request.Url.AbsolutePath.Contains("default.aspx"))
                {
                    liAnasayfa.Attributes.Add("class", "menu-item mm-active");
                }

                if (Request.Url.AbsolutePath.Contains("masrafpersonnel"))
                {
                    liPersonel.Attributes.Add("class", "menu-item mm-active");
                }

                if (Request.Url.AbsolutePath.Contains("masrafgrup"))
                {
                    liMasrafGrup.Attributes.Add("class", "menu-item mm-active");
                }

                if (Request.Url.AbsolutePath.Contains("yonetici"))
                {
                    liYonetici.Attributes.Add("class", "menu-item mm-active");
                }

                if (Request.Url.AbsolutePath.Contains("masrafformu"))
                {
                    liMasrafFormu.Attributes.Add("class", "menu-item mm-active");
                }

                if (Request.Url.AbsolutePath.Contains("masrafonaylama"))
                {
                    liMasrafOnay.Attributes.Add("class", "menu-item mm-active");
                }

                if (Request.Url.AbsolutePath.Contains("exportexcel"))
                {
                    liExcelExport.Attributes.Add("class", "menu-item mm-active");
                }

                if(Request.Url.AbsolutePath.Contains("masrafgonderilen"))
                {
                    liGonderilen.Attributes.Add("class", "menu-item mm-active");
                }
            }
        }
        void checkUser()
        {
            if (Request.Cookies["ckmasrafweb"] != null)
            {
                if (db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())) == "false")
                {
                    Response.Redirect("login.aspx");
                }
                else
                {
                    try
                    {
                        Array loggedInUserNameStrArry = Request.Cookies["ckmasrafweb"]["Name"].Split(' ');
                        string loggedInUserFisrtNameStr = loggedInUserNameStrArry.GetValue(0).ToString();
                        loggedInUserFisrtNameStr = loggedInUserFisrtNameStr[0].ToString();
                        string loggedInUserLastNameStr = loggedInUserNameStrArry.GetValue(1).ToString();
                        loggedInUserLastNameStr = loggedInUserLastNameStr[0].ToString();

                        loggedInUserNameStr = loggedInUserFisrtNameStr + "." + loggedInUserLastNameStr;
                        //this.aProfil.InnerText = loggedInUserFisrtNameStr + "." + loggedInUserLastNameStr;
                    }
                    catch (Exception)
                    {

                    }

                    DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], string.Empty, "ConString");
                    if (dr["usr_rank"].ToString() == "admin")
                    {
                        liAnasayfa.Visible = true;
                        liYonetici.Visible = true;
                        liPersonel.Visible = true;
                        liMasrafGrup.Visible = true;
                        liMasrafFormu.Visible = false;
                        liGonderilen.Visible = false;
                        liMasrafOnay.Visible = false;
                        liExcelExport.Visible = true;
                    }
                    else if(dr["usr_rank"].ToString() == "yonetici")
                    {
                        liAnasayfa.Visible = true;
                        liYonetici.Visible = false;
                        liPersonel.Visible = false;
                        liMasrafGrup.Visible = false;
                        liMasrafFormu.Visible = false;
                        liGonderilen.Visible = false;
                        liMasrafOnay.Visible = true;
                        liExcelExport.Visible = false;
                    } 
                    else if (dr["usr_rank"].ToString() == "personel")
                    {
                        liAnasayfa.Visible = true;
                        liYonetici.Visible = false;
                        liPersonel.Visible = false;
                        liMasrafGrup.Visible = false;
                        liMasrafFormu.Visible = true;
                        liMasrafOnay.Visible = false;
                        liExcelExport.Visible = false;
                    }
                }
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
    }
}