using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Masraf
{
    public partial class masrafonaylama : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.checkUser();

                this.ddlMonth.Items.Insert(0, new ListItem("-Ay Seçiniz-", "-1"));
                for (int i = 0; i < 12; i++)
                {
                    this.ddlMonth.Items.Insert((i + 1), new ListItem(db.monthArry[i].ToString(), (i + 1).ToString()));
                }

                this.ddlYear.Items.Insert(0, new ListItem("- Yıl Seçiniz-", "-1"));
                for (int i = 0; i < 8; i++)
                {
                    this.ddlYear.Items.Insert((i + 1), new ListItem(db.yearArry[i].ToString(), (i + 1).ToString()));
                }

                DataTable dtPersonel = null;

                dtPersonel = db.getDataTable("select * from tbl_users where usr_status=1 and usr_rank='personel' and yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " order by usr_name asc", "ConString");
                if ((dtPersonel != null) && dtPersonel.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPersonel.Rows)
                    {
                        dr["usr_name"] = dr["usr_name"].ToString() + " " + dr["usr_surname"].ToString();
                    }

                    this.ddlPersonnel.DataSource = dtPersonel;
                    this.ddlPersonnel.DataValueField = "usr_user_id";
                    this.ddlPersonnel.DataTextField = "usr_name";
                    this.ddlPersonnel.DataBind();

                    this.ddlPersonnel.Items.Insert(0, new ListItem("-Personel Seçiniz-", "-1"));
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
                        this.getMasrafFiltre(Request.QueryString["month"].ToString() + "|" + Request.QueryString["year"].ToString() + "|" + Request.QueryString["personnelid"].ToString());
                    }
                }
                else
                {
                    this.getMasraf();
                }

                if (rptMasrafFormu.Items.Count > 0)
                {
                    double toplamTutar = 0;
                    foreach (RepeaterItem item in rptMasrafFormu.Items)
                    {
                        Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                        toplamTutar += Convert.ToDouble(lblMasrafFormuKDVDahilTutar.Text);
                    }
                    ltrlToplamTutar.Text = "Toplam Tutar: " + toplamTutar.ToString() + "&nbspTL";
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
        void getMasraf()
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where isActive=1 and masrafFormuOnay=0 and yoneticiyeGonderildimi=1 and u.yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " order by masrafFormuId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0)
            {
                rptMasrafFormu.Visible = true;
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
        void getMasrafFiltre(string filter)
        {
            //and (masrafFormuDate > '2023-02-01 00:00:00.000' and masrafFormuDate < '2023-03-01 00:00:00.000')

            DataTable dt = null;
            Array filterArry = filter.Split('|');
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

            if (month == -1)
            {
                rptMasrafFormu.Visible = false;
                ltrlResult.Text = "Lütfen Ay Seçiniz";
                ltrlToplamTutar.Text = string.Empty;
                return;
            }

            int nextMonth = month + 1;
            string monthTemp = string.Empty;
            string nextMonthTemp = string.Empty;
            string personnelId = filterArry.GetValue(2).ToString();

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

            this.ddlMonth.SelectedValue = month.ToString();
            this.ddlYear.Items.FindByText(year.ToString()).Selected = true;
            this.ddlPersonnel.SelectedValue = personnelId;

            if (personnelId == "-1")
            {
                if (nextMonth > 12)
                {

                    nextMonthTemp = "01";
                    filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + (year + 1).ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and u.yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1];
                }
                else
                {
                    filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and u.yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1];
                }
            }
            else
            {
                if (nextMonth > 12)
                {
                    nextMonthTemp = "01";
                    filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + (year + 1).ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;
                }
                else
                {
                    filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;
                }
            }

            //filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + (year + 1).ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and u.yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1];

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 and mf.masrafFormuOnay=0 " + filter + " order by masrafFormuId asc", "ConString");
            if ((dt != null) && dt.Rows.Count > 0 || ddlPersonnel.SelectedValue == null)
            {
                ltrlResult.Text = string.Empty;
                rptMasrafFormu.Visible = true;
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

                if (personnelId != "-1")
                {
                    DataRow drUser = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", personnelId, string.Empty, "ConString");
                    if (drUser != null)
                    {
                        DataRow drPersonel = db.getValueWhere1("select * from tbl_masrafFormu", "createdUserId", "=", personnelId, "order by masrafFormuId desc", "ConString");
                        if (drPersonel != null)
                        {
                            ltrlResult.Text = drUser["usr_name"].ToString() + " " + drUser["usr_surname"].ToString() + " personelinin masraflarını " + drPersonel["masrafonaytarih"].ToString() + " tarihinde onayladınız.";
                        }
                    }
                }
            }
        }
        void getMasrafByName(string masrafName)
        {
            DataTable dt = null;

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 and mf.masrafFormuKimdenAlindigi like '%" + masrafName + "%' and u.yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " order by masrafFormuId asc", "ConString");
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
            int rowIndex = e.Item.ItemIndex + 1;
            Label lblMasrafSiraNo = (Label)e.Item.FindControl("lblMasrafSiraNo");

            if (lblMasrafSiraNo != null)
            {
                lblMasrafSiraNo.Text = rowIndex.ToString();
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                Literal ltrlMasrafGrup = (Literal)e.Item.FindControl("ltrlMasrafGrup");
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trRow");
                Label lblMasrafFormuId = (Label)e.Item.FindControl("lblMasrafFormuId");
                Label lblMasrafFormuKDVDahilTutar = (Label)e.Item.FindControl("lblMasrafFormuKDVDahilTutar");
                Literal ltrlMasrafFormuKimdenAlindigi = (Literal)e.Item.FindControl("ltrlMasrafFormuKimdenAlindigi");
                Literal ltrlMasrafFormuDate = (Literal)e.Item.FindControl("ltrlMasrafFormuDate");
                Literal ltrlKDVsiz = (Literal)e.Item.FindControl("ltrlKDVsiz");
                Literal ltrlMasrafFormuKKEG = (Literal)e.Item.FindControl("ltrlMasrafFormuKKEG");
                Literal ltrlPersonelFullName = (Literal)e.Item.FindControl("ltrlPersonelFullName");
                Label lblPDF = (Label)e.Item.FindControl("lblPDF");
                Label lblCreatedUserId = (Label)e.Item.FindControl("lblCreatedUserId");

                ltrlMasrafGrup.Text = getMasrafGrup(Convert.ToInt32(rV["masrafFormuGrupId"].ToString()));

                if (!(bool)rV["masrafFormuOnay"])
                {
                    trRow.Attributes.Add("style", "background-color:lightcoral !important");
                }

                lblMasrafFormuId.Text = rV["masrafFormuId"].ToString();
                lblMasrafFormuKDVDahilTutar.Text = rV["masrafFormuKDVDahilTutar"].ToString();

                DateTime dtBas = DateTime.Parse(rV["masrafFormuDate"].ToString());
                ltrlMasrafFormuDate.Text = dtBas.ToString("dd.MM.yyyy");
                ltrlMasrafFormuKimdenAlindigi.Text = rV["masrafFormuKimdenAlindigi"].ToString();

                if (rV["masrafFormuGrupId"].ToString() == "10" || rV["masrafFormuGrupId"].ToString() == "12")
                {
                    ltrlKDVsiz.Text = (Convert.ToDouble(rV["masrafFormuKDVDahilTutar"].ToString()) - Convert.ToDouble(rV["masrafFormuKKEG"].ToString())).ToString();
                    ltrlMasrafFormuKKEG.Text = string.Empty;
                    ltrlMasrafFormuKKEG.Text = rV["masrafFormuKKEG"].ToString() + "&nbsp;TL";
                }
                else
                {
                    ltrlMasrafFormuKKEG.Text = string.Empty;
                    ltrlKDVsiz.Text = rV["masrafFormuKDVDahilTutar"].ToString();
                }

                ltrlPersonelFullName.Text = rV["usr_name"].ToString() + " " + rV["usr_surname"].ToString();
                lblPDF.Text = rV["pdf"].ToString();
                lblCreatedUserId.Text = rV["createdUserId"].ToString();
            }
        }
        protected void lnkBtnFiltrele_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafonaylama.aspx?month=" + Server.UrlEncode(this.ddlMonth.SelectedItem.Value.ToString()) + "&year=" + Server.UrlEncode(this.ddlYear.SelectedItem.Text.ToString()) + "&personnelid=" + this.ddlPersonnel.SelectedItem.Value.ToString());
        }
        protected void btnMasrafOnayla_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                CheckBox checkBxOnay = (CheckBox)item.FindControl("checkBxOnay");
                Label lblMasrafFormuId = (Label)item.FindControl("lblMasrafFormuId");

                if (checkBxOnay.Checked)
                {
                    string result = db.updateValueWhere1("tbl_masrafFormu", "masrafFormuOnay,masrafFormuOnaylayanId,masrafonaytarih", "1" + "~" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + "~é" + DateTime.Now, "masrafFormuId", "=", lblMasrafFormuId.Text.ToString(), "ConString");
                }
            }

            string epostaStr = string.Empty;
            string epostaContentStr = string.Empty;
            string pdf = string.Empty;
            epostaContentStr = "Yeni masraf kayıtları aşağıdaki gibidir<br/><br/>";
            string personelFullName = string.Empty;

            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                Literal ltrlMasrafFormuKimdenAlindigi = (Literal)item.FindControl("ltrlMasrafFormuKimdenAlindigi");
                Literal ltrlMasrafGrup = (Literal)item.FindControl("ltrlMasrafGrup");
                Literal ltrlMasrafFormuDate = (Literal)item.FindControl("ltrlMasrafFormuDate");
                Label lblCreatedUserId = (Label)item.FindControl("lblCreatedUserId");
                Literal ltrlPersonelFullName = (Literal)item.FindControl("ltrlPersonelFullName");
                Label lblPDF = (Label)item.FindControl("lblPDF");
                CheckBox checkBxOnay = (CheckBox)item.FindControl("checkBxOnay");

                if (checkBxOnay.Checked)
                {
                    epostaContentStr += "Masraf Tarihi:&nbsp;" + ltrlMasrafFormuDate.Text + "&nbsp;Kimden alındı: " + ltrlMasrafFormuKimdenAlindigi.Text + "&nbsp;Masraf Grubu: " + ltrlMasrafGrup.Text + "&nbsp;Toplam Tutar: " + lblMasrafFormuKDVDahilTutar.Text + "&nbsp;TL<br/><br/>";

                    personelFullName = ltrlPersonelFullName.Text;

                    pdf = lblPDF.Text;
                }
            }

            DataRow drUserAdmin = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], string.Empty, "ConString");
            if (drUserAdmin != null)
            {
                try
                {
                    db.ePostaGonder(drUserAdmin["usr_name"].ToString() + " " + drUserAdmin["usr_surname"].ToString() + " yöneticisinin onayı ile " + personelFullName + " adlı personelden yeni masraf kayıtları gönderildi.", "bulent@redcellonline.com", epostaContentStr + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' width='701' height='200' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", Server.MapPath("~/upload/pdf/") + pdf);
                }
                catch (Exception) { }

                try
                {
                    db.ePostaGonder(drUserAdmin["usr_name"].ToString() + " " + drUserAdmin["usr_surname"].ToString() + " yöneticisinin onayı ile " + personelFullName + " adlı personelden yeni masraf kayıtları gönderildi.", "ersoy@redcellonline.com", epostaContentStr + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' width='701' height='200' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", Server.MapPath("~/upload/pdf/") + pdf);
                }
                catch (Exception) { }
            }

            Response.Redirect("masrafonaylama.aspx");

            //ltrlInfo.Text = "Seçilen Masraf formu kayıtları başarıyla onaylanmıştır.";
        }
        protected void btnMasrafReddet_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                CheckBox checkBxOnay = (CheckBox)item.FindControl("checkBxOnay");
                Label lblMasrafFormuId = (Label)item.FindControl("lblMasrafFormuId");

                if (checkBxOnay.Checked)
                {
                    string result = db.updateValueWhere1("tbl_masrafFormu", "isActive,masrafFormuOnaylayanId", "0" + "~" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], "masrafFormuId", "=", lblMasrafFormuId.Text.ToString(), "ConString");
                }
            }

            string epostaStr = string.Empty;
            string epostaContentStr = string.Empty;
            string pdf = string.Empty;
            epostaContentStr = "Reddedilen masraf kayıtları aşağıdaki gibidir<br/><br/>";
            string personelFullName = string.Empty;
            string createdUserId = string.Empty;

            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                Literal ltrlMasrafFormuKimdenAlindigi = (Literal)item.FindControl("ltrlMasrafFormuKimdenAlindigi");
                Literal ltrlMasrafGrup = (Literal)item.FindControl("ltrlMasrafGrup");
                Literal ltrlMasrafFormuDate = (Literal)item.FindControl("ltrlMasrafFormuDate");
                Label lblCreatedUserId = (Label)item.FindControl("lblCreatedUserId");
                Literal ltrlPersonelFullName = (Literal)item.FindControl("ltrlPersonelFullName");
                Label lblPDF = (Label)item.FindControl("lblPDF");
                CheckBox checkBxOnay = (CheckBox)item.FindControl("checkBxOnay");

                if (checkBxOnay.Checked)
                {
                    createdUserId = lblCreatedUserId.Text;
                    epostaContentStr += "Masraf Tarihi:&nbsp;" + ltrlMasrafFormuDate.Text + "&nbsp;Kimden alındı: " + ltrlMasrafFormuKimdenAlindigi.Text + "&nbsp;Masraf Grubu: " + ltrlMasrafGrup.Text + "&nbsp;Toplam Tutar: " + lblMasrafFormuKDVDahilTutar.Text + "&nbsp;TL<br/><br/>";

                    personelFullName = ltrlPersonelFullName.Text;

                    pdf = lblPDF.Text;
                }
            }

            DataRow drUserAdmin = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", createdUserId, string.Empty, "ConString");
            if (drUserAdmin != null)
            {
                try
                {
                    db.ePostaGonder("Masraf kayıt(lar)ınız bağlı olduğunuz yönetici tarafından reddedilmiştir. Lütfen reddedilen masraf(lar)ınızı tekrar oluşturunuz.", drUserAdmin["usr_email_address"].ToString(), epostaContentStr + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' width='701' height='200' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", Server.MapPath("~/upload/pdf/") + pdf);
                }
                catch (Exception) { }
            }

            Response.Redirect("masrafonaylama.aspx");

            //ltrlInfo.Text = "Seçilen Masraf formu kayıtları başarıyla reddedilmiştir.";
        }
    }
}