using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Masraf
{
    public partial class masrafgonderilen : System.Web.UI.Page
    {
        public bool isYoneticiMi = false;
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.checkUser();

                this.ddlMonth.Items.Insert(0, new ListItem("-Seçiniz-", "-1"));
                for (int i = 0; i < 12; i++)
                {
                    this.ddlMonth.Items.Insert((i + 1), new ListItem(db.monthArry[i].ToString(), (i + 1).ToString()));
                }

                this.ddlYear.Items.Insert(0, new ListItem("-Seçiniz-", "-1"));
                for (int i = 0; i < 8; i++)
                {
                    this.ddlYear.Items.Insert((i + 1), new ListItem(db.yearArry[i].ToString(), (i + 1).ToString()));
                }

                if (isYoneticiMi)
                {
                    this.getMasraf();
                }
                else
                {
                    this.getMasrafByUid(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
                }

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

                if (Request.QueryString["k"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["k"].ToString()))
                    {
                        this.getMasrafByName(Request.QueryString["k"].ToString());
                    }
                }
                else if (Request.QueryString["month"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["month"].ToString()))
                    {
                        this.getMasrafFiltre(Request.QueryString["month"].ToString() + "-" + Request.QueryString["year"].ToString(), db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
                    }
                }
                else
                {
                    if (isYoneticiMi)
                    {
                        this.getMasraf();
                    }
                    else
                    {
                        this.getMasrafByUid(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
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
                        DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], "", "ConString");
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
        void getMasraf()
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where isActive=1 and yoneticiyeGonderildimi=1 order by masrafFormuGonderilenDate desc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                rptMasrafFormu.Visible = true;
                ltrlResult.Text = string.Empty;
                rptMasrafFormu.DataBind();
            }
            else
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Sonuç bulunamadı.";
                ltrlToplamTutar.Text = string.Empty;
            }
        }
        void getMasrafByUid(string userId)
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 and mf.createdUserId=" + userId + " and yoneticiyeGonderildimi=1 order by masrafFormuGonderilenDate desc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                rptMasrafFormu.Visible = true;
                ltrlResult.Text = string.Empty;
                paginator.DataSource = dt.DefaultView;
                rptMasrafFormu.DataSource = paginator.DataSourcePaged;
                rptMasrafFormu.DataBind();
            }
            else
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Sonuç bulunamadı.";
                ltrlToplamTutar.Text = string.Empty;
            }
        }
        void getMasrafFiltre(string filter, string userId)
        {
            //and (masrafFormuDate > '2023-02-01 00:00:00.000' and masrafFormuDate < '2023-03-01 00:00:00.000')

            DataTable dt = null;
            Array filterArry = filter.Split('-');

            int year = 0;
            int month = 0;
            try
            {
                year = Convert.ToInt32(filterArry.GetValue(1).ToString());
            }
            catch (Exception)
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Lütfen Ay ve Yıl Seçiniz";
                ltrlToplamTutar.Text = string.Empty;
                return;
            }
            try
            {
                month = Convert.ToInt32(filterArry.GetValue(0).ToString());
            }
            catch (Exception)
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Lütfen Ay ve Yıl Seçiniz";
                ltrlToplamTutar.Text = string.Empty;
                return;
            }

            int nextMonth = month + 1;
            string monthTemp = string.Empty;
            string nextMonthTemp = string.Empty;

            if (month < 10)
            {
                monthTemp = "0" + month.ToString();
            }
            else
            {
                monthTemp = month.ToString();
            }

            if (nextMonth < 10)
            {
                nextMonthTemp = "0" + nextMonth.ToString();
            }
            else
            {
                nextMonthTemp = nextMonth.ToString();
            }

            if (nextMonth > 12)
            {
                nextMonthTemp = "01";
                filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + (year + 1).ToString() + "-" + nextMonthTemp + "-01 00:00:00.000')";
            }
            else
            {
                filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000')";
            }

            this.ddlMonth.SelectedValue = month.ToString();
            this.ddlYear.Items.FindByText(year.ToString()).Selected = true;

            //filter = "and (masrafFormuDate > '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000')";

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 " + filter + " and mf.createdUserId=" + userId + " and mf.yoneticiyeGonderildimi=1 order by masrafFormuId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                rptMasrafFormu.Visible = true;
                ltrlResult.Text = string.Empty;
                paginator.DataSource = dt.DefaultView;
                paginator.BindToControl = rptMasrafFormu;
                rptMasrafFormu.DataSource = paginator.DataSourcePaged;
                rptMasrafFormu.DataBind();

                double toplamTutar = 0;
                foreach (RepeaterItem item in rptMasrafFormu.Items)
                {
                    Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                    toplamTutar += Convert.ToDouble(lblMasrafFormuKDVDahilTutar.Text);
                }
                ltrlToplamTutar.Text = "Toplam Tutar: " + toplamTutar.ToString() + "&nbsp;TL";
            }
            else
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Sonuç bulunamadı.";
                ltrlToplamTutar.Text = string.Empty;
            }
        }
        void getMasrafByName(string masrafName)
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 and mf.masrafFormuKimdenAlindigi like '%" + masrafName + "%' and mf.yoneticiyeGonderildimi=1 order by masrafFormuId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                rptMasrafFormu.Visible = true;
                ltrlResult.Text = string.Empty;
                paginator.DataSource = dt.DefaultView;
                paginator.BindToControl = rptMasrafFormu;
                rptMasrafFormu.DataSource = paginator.DataSourcePaged;
                rptMasrafFormu.DataBind();
            }
            else
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Sonuç bulunamadı.";
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
                int rowIndex = e.Item.ItemIndex + 1;
                Label lblMasrafSiraNo = (Label)e.Item.FindControl("lblMasrafSiraNo");

                if (lblMasrafSiraNo != null)
                {
                    lblMasrafSiraNo.Text = rowIndex.ToString();
                }

                DataRowView rV = (DataRowView)e.Item.DataItem;

                Literal ltrlMasrafGrup = (Literal)e.Item.FindControl("ltrlMasrafGrup");
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trRow");
                Label lblMasrafFormuId = (Label)e.Item.FindControl("lblMasrafFormuId");
                Label lblMasrafFormuKDVDahilTutar = (Label)e.Item.FindControl("lblMasrafFormuKDVDahilTutar");
                Literal ltrlMasrafGonderilenDate = (Literal)e.Item.FindControl("ltrlMasrafGonderilenDate");
                Literal ltrlMasrafFormuKimdenAlindigi = (Literal)e.Item.FindControl("ltrlMasrafFormuKimdenAlindigi");
                Label lblCreatedUserId = (Label)e.Item.FindControl("lblCreatedUserId");
                Literal ltrlKDVsiz = (Literal)e.Item.FindControl("ltrlKDVsiz");

                ltrlMasrafGrup.Text = getMasrafGrup(Convert.ToInt32(rV["masrafFormuGrupId"].ToString()));

                lblMasrafFormuId.Text = rV["masrafFormuId"].ToString();
                lblMasrafFormuKDVDahilTutar.Text = rV["masrafFormuKDVDahilTutar"].ToString();

                if (!string.IsNullOrEmpty(rV["masrafFormuGonderilenDate"].ToString()))
                {
                    DateTime dtBas = DateTime.Parse(rV["masrafFormuGonderilenDate"].ToString());
                    ltrlMasrafGonderilenDate.Text = dtBas.ToString("dd.MM.yyyy");
                }
                ltrlMasrafFormuKimdenAlindigi.Text = rV["masrafFormuKimdenAlindigi"].ToString();
                lblCreatedUserId.Text = rV["createdUserId"].ToString();
                ltrlKDVsiz.Text = (Convert.ToDouble(rV["masrafFormuKDVDahilTutar"].ToString()) - Convert.ToDouble(rV["masrafFormuKKEG"].ToString())).ToString();
            }
        }
        protected void lnkBtnFiltrele_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafgonderilen.aspx?month=" + Server.UrlEncode(this.ddlMonth.SelectedItem.Value.ToString()) + "&year=" + Server.UrlEncode(this.ddlYear.SelectedItem.Text.ToString()));
        }
    }
}