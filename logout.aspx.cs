using System;
using System.Web;

namespace Masraf
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie adminCookie = new HttpCookie("ckmasrafweb");
            adminCookie["SessID"] = "";
            adminCookie["Name"] = "";
            adminCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(adminCookie);
            Response.Redirect("login.aspx");
        }
    }
}