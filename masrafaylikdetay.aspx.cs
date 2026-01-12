using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Masraf
{
    public partial class masrafaylikdetay : System.Web.UI.Page
    {
        public bool isYoneticiMi = false;
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.checkUser();

                this.getMasrafByUid(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);

                if (rptMasrafFormu.Items.Count > 0)
                {
                    double toplamTutar = 0;
                    foreach (RepeaterItem item in rptMasrafFormu.Items)
                    {
                        Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                        toplamTutar += Convert.ToDouble(lblMasrafFormuKDVDahilTutar.Text);
                    }
                    ltrlToplamTutar.Text = "Toplam Tutar: " + toplamTutar.ToString() + "&nbsp;TL";
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
                        if (dr["usr_rank"].ToString() == "yonetici")
                        {
                            isYoneticiMi = true;
                        }
                        else if (dr["usr_rank"].ToString() == "admin")
                        {
                            isYoneticiMi = true;
                        }
                        else if (dr["usr_rank"].ToString() == "personel")
                        {
                            isYoneticiMi = false;
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
        void getMasrafByUid(string userId)
        {
            DataTable dt = null;
            string ay = Request.QueryString["ay"].ToString();
            string yil = Request.QueryString["yil"].ToString();

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive = 1 and mf.createdUserId=" + userId + " and createdDate > '" + yil + "-" + ay + "-01' and createdDate < '" + yil + "-" + (Convert.ToInt32(ay) + 1).ToString() + "-01' order by masrafFormuId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                rptMasrafFormu.Visible = true;
                paginator.DataSource = dt.DefaultView;
                rptMasrafFormu.DataSource = paginator.DataSourcePaged;
                rptMasrafFormu.DataBind();
            }
            else
            {
                rptMasrafFormu.Visible = false;
                ltrlToplamTutar.Text = string.Empty;
            }
        }
        public string getMasrafGrup(int masrafFormuGrupId)
        {
            masraf mObj = new masraf();
            string masrafGrupName = string.Empty;
            DataTable dt = null;

            //dt = db.getDataTable("select masrafGrupName from tbl_masrafGrup where isActive=1 and masrafGrupId=" + masrafFormuGrupId.ToString(), "ConString");
            dt = mObj.getMasrafGrup(masrafFormuGrupId);
            if ((dt != null) && dt.Rows.Count > 0)
            {
                masrafGrupName = dt.Rows[0]["masrafGrupName"].ToString();
            }

            return masrafGrupName;
        }
        protected void rptMasrafFormu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                Literal ltrlMasrafGrup = (Literal)e.Item.FindControl("ltrlMasrafGrup");
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trRow");
                Label lblMasrafFormuId = (Label)e.Item.FindControl("lblMasrafFormuId");
                Label lblMasrafFormuKDVDahilTutar = (Label)e.Item.FindControl("lblMasrafFormuKDVDahilTutar");
                Literal ltrlMasrafFormuDate = (Literal)e.Item.FindControl("ltrlMasrafFormuDate");
                Literal ltrlMasrafFormuKimdenAlindigi = (Literal)e.Item.FindControl("ltrlMasrafFormuKimdenAlindigi");
                Label lblCreatedUserId = (Label)e.Item.FindControl("lblCreatedUserId");
                Literal ltrlKDVsiz = (Literal)e.Item.FindControl("ltrlKDVsiz");

                ltrlMasrafGrup.Text = getMasrafGrup(Convert.ToInt32(rV["masrafFormuGrupId"].ToString()));

                if ((bool)rV["masrafFormuOnay"])
                {
                    trRow.Attributes.Add("style", "background-color:mediumaquamarine !important");
                }
                else
                {
                    trRow.Attributes.Add("style", "background-color:lightcoral !important");
                }

                lblMasrafFormuId.Text = rV["masrafFormuId"].ToString();
                lblMasrafFormuKDVDahilTutar.Text = rV["masrafFormuKDVDahilTutar"].ToString();

                DateTime dtBas = DateTime.Parse(rV["createdDate"].ToString());
                ltrlMasrafFormuDate.Text = dtBas.ToString("dd.MM.yyyy");
                ltrlMasrafFormuKimdenAlindigi.Text = rV["masrafFormuKimdenAlindigi"].ToString();
                lblCreatedUserId.Text = rV["createdUserId"].ToString();
                ltrlKDVsiz.Text = (Convert.ToDouble(rV["masrafFormuKDVDahilTutar"].ToString()) - Convert.ToDouble(rV["masrafFormuKKEG"].ToString())).ToString();
            }
        }
        protected void btnGeri_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }
    }
}