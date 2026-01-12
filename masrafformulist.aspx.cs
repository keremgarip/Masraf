using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Masraf
{
    public partial class masrafformulist : System.Web.UI.Page
    {
        public bool isYoneticiMi = false;
        dbLibrary db = new dbLibrary();
        public string adSoyad = string.Empty;
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

                this.Form.DefaultButton = this.lnkBtnAra.UniqueID;
                if (isYoneticiMi)
                {
                    this.getMasraf();
                }
                else
                {
                    this.getMasrafByUid(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
                }

                if (Request.QueryString["mod"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["mod"].ToString()))
                    {
                        if (Request.QueryString["mod"] == "yg")
                        {
                            //ltrlInfoYg.Text = Request.QueryString["monthe"].ToString() + " " + Request.QueryString["yeare"].ToString() + " masraf listesi yöneticiye gönderildi";
                            ltrlInfoYg.Text = "Masraf listesi yöneticiye gönderildi";
                            ltrlInfoYg.Visible = true;
                        }
                        if (Request.QueryString["mod"] == "nsyg")
                        {
                            ltrlInfoYg.Text = "Ay ve Tarih seçmelisiniz";
                            ltrlInfoYg.Visible = true;
                        }
                    }
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

                if (Request.QueryString["userIdEmail"] != null)
                {
                    this.getMasrafByUid(Request.QueryString["userIdEmail"].ToString());
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
        void getMasraf()
        {
            masraf mObj = new masraf();
            DataTable dt = null;

            //dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId=u.usr_user_id where isActive=1 and yoneticiyeGonderildimi=0 order by masrafFormuId asc", "ConString");
            dt = mObj.getMasraf();
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
            masraf mObj = new masraf();
            DataTable dt = null;

            //dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId=u.usr_user_id where mf.isActive=1 and mf.createdUserId=" + userId + " and yoneticiyeGonderildimi=0 order by masrafFormuId asc", "ConString");
            dt = mObj.getMasrafByUid(userId);
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

            this.ddlMonth.SelectedValue = month.ToString();
            this.ddlYear.Items.FindByText(year.ToString()).Selected = true;

            filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000')";

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 and mf.masrafFormuOnay=0 " + filter + " and mf.createdUserId=" + userId + " and mf.yoneticiyeGonderildimi=0 order by masrafFormuId asc", "ConString");
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
        void getMasrafByName(string masrafName)
        {
            masraf mObj = new masraf();
            DataTable dt = null;

            //dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id where mf.isActive=1 and mf.masrafFormuKimdenAlindigi like '%" + masrafName + "%' and mf.yoneticiyeGonderildimi=0 order by masrafFormuId asc", "ConString");
            dt = mObj.getMasrafByName(masrafName);
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
        protected void rptMasrafFormu_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                Random rnd = new Random();
                string parameter = db.Encrypt(e.CommandArgument.ToString() + "|" + rnd.Next(1111, 9999)).ToString();
                Response.Redirect("editmasrafformu.aspx?secretkey=" + parameter);
            }
            else if (e.CommandName == "delete")
            {
                db.updateValueWhere1("tbl_masrafFormu", "isActive", "0", "masrafFormuId", "=", e.CommandArgument.ToString(), "ConString");

                this.getMasraf();
            }
        }
        protected void rptMasrafFormu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                int rowIndex = e.Item.ItemIndex + 1;
                Label lblMasrafSiraNo = (Label)e.Item.FindControl("lblMasrafSiraNo");

                if(lblMasrafSiraNo != null)
                {
                    lblMasrafSiraNo.Text = rowIndex.ToString();
                }
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

                DateTime dtBas = DateTime.Parse(rV["masrafFormuDate"].ToString());
                ltrlMasrafFormuDate.Text = dtBas.ToString("dd.MM.yyyy");
                ltrlMasrafFormuKimdenAlindigi.Text = rV["masrafFormuKimdenAlindigi"].ToString();
                lblCreatedUserId.Text = rV["createdUserId"].ToString();
                ltrlKDVsiz.Text = (Convert.ToDouble(rV["masrafFormuKDVDahilTutar"].ToString()) - Convert.ToDouble(rV["masrafFormuKKEG"].ToString())).ToString();
            }
        }
        protected void lnkBtnAra_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafformulist.aspx?k=" + this.txtBxMasraf.Text.Trim());
        }
        protected void lnkBtnFiltrele_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafformulist.aspx?month=" + Server.UrlEncode(this.ddlMonth.SelectedItem.Value.ToString()) + "&year=" + Server.UrlEncode(this.ddlYear.SelectedItem.Text.ToString()));
        }
        protected void btnYoneticiyeGonder_Click(object sender, EventArgs e)
        {
            if (this.fuPDF.HasFile)
            {
                //ilk önce seçilen dosyanın dosya tipi pdf mi diye kontrol eklenecek

                //eger pdf formatında bir dosya secılmıs ıse, upload/pdf klasorunun ıcıne save edilecek

                //masrafformu tablosuna "pdf" sütunu acılacak ve 14838247832.pdf dosyası olarak bu yeni sütüna masraf bazlı olarak update edılecek

                //masrafgonderilen.aspx sayfasında en sag sutune pdf eklenecek ve tıklanabılır olacak

                DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], string.Empty, "ConString");
                if (dr != null)
                {
                    adSoyad = db.SeoURL(dr["usr_name"].ToString()) + "_" + db.SeoURL(dr["usr_surname"].ToString());
                }

                string filePath = string.Empty;
                string fileName = "masrafkalemleri_" + adSoyad + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Millisecond + "_" + DateTime.Now.Second;
                string fileNameTemp = string.Empty;
                string tarih = DateTime.Now.Day.ToString();
                string fileExt = System.IO.Path.GetExtension(fuPDF.PostedFile.FileName).ToString().Trim();

                if (fileExt.Contains(".pdf"))
                {
                    filePath = Server.MapPath("~/upload/pdf/") + fileName + fileExt;
                    fileNameTemp = fileName + fileExt;

                    fuPDF.SaveAs(filePath);
                }
                else
                {
                    return;
                }

                //if (ddlMonth.SelectedItem.Value != "-1" && ddlYear.SelectedItem.Value != "-1")
                //{
                string epostaStr = string.Empty;
                string epostaContentStr = string.Empty;
                string createdUserIdStr = string.Empty;
                string personelIdStr = string.Empty;
                string userName = string.Empty;
                string surName = string.Empty;
                string month = string.Empty;
                string year = string.Empty;
                string userIdStr = string.Empty;

                epostaContentStr = "Yeni masraf kayıtları aşağıdaki gibidir<br/><br/>";

                foreach (RepeaterItem item in rptMasrafFormu.Items)
                {
                    Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                    Literal ltrlMasrafFormuKimdenAlindigi = (Literal)item.FindControl("ltrlMasrafFormuKimdenAlindigi");
                    Literal ltrlMasrafGrup = (Literal)item.FindControl("ltrlMasrafGrup");
                    Literal ltrlMasrafFormuDate = (Literal)item.FindControl("ltrlMasrafFormuDate");
                    Label lblCreatedUserId = (Label)item.FindControl("lblCreatedUserId");
                    Label lblMasrafFormuId = (Label)item.FindControl("lblMasrafFormuId");

                    epostaContentStr += "Masraf Tarihi:&nbsp;" + ltrlMasrafFormuDate.Text + "&nbsp;Kimden alındı: " + ltrlMasrafFormuKimdenAlindigi.Text + "&nbsp;Masraf Grubu: " + ltrlMasrafGrup.Text + "&nbsp;Toplam Tutar: " + lblMasrafFormuKDVDahilTutar.Text + "&nbsp;TL<br/><br/>";

                    createdUserIdStr = lblCreatedUserId.Text;

                    db.updateValueWhere1("tbl_masrafFormu", "yoneticiyeGonderildimi,masrafFormuGonderilenDate", "1" + "~é" + DateTime.Now, "masrafFormuId", "=", lblMasrafFormuId.Text, "ConString");

                    //pdf guncelle
                    db.updateValueWhere1("tbl_masrafFormu", "pdf", fileNameTemp, "masrafFormuId", "=", lblMasrafFormuId.Text, "ConString");
                }

                DataTable dtUser = db.getDataTable("select * from tbl_users where usr_status = 1 and usr_user_id=" + createdUserIdStr, "ConString");
                if ((dtUser != null) && dtUser.Rows.Count > 0)
                {
                    personelIdStr = dtUser.Rows[0]["yoneticiId"].ToString();
                }

                DataTable dtUserYonnetici = db.getDataTable("select * from tbl_users where usr_status = 1 and usr_user_id=" + personelIdStr, "ConString");
                if ((dtUserYonnetici != null) && dtUserYonnetici.Rows.Count > 0)
                {
                    userIdStr = dtUserYonnetici.Rows[0]["usr_user_id"].ToString();
                    epostaStr = dtUserYonnetici.Rows[0]["usr_email_address"].ToString();
                }

                string loggedInUserNameStr = string.Empty;
                Array loggedInUserNameStrArry = Request.Cookies["ckmasrafweb"]["Name"].Split(' ');
                userName = loggedInUserNameStrArry.GetValue(0).ToString();
                surName = loggedInUserNameStrArry.GetValue(1).ToString();

                epostaContentStr = "<br/><br/><a href='http://www.kf-ik.com/masrafonaylama.aspx?userIdEmail=" + userIdStr + "'>Sisteme girmek için tıklayınız.</a>";

                month = ddlMonth.SelectedItem.Text;
                year = ddlYear.SelectedItem.Text;

                //"<br/><br/><img src='http://www.kf-ik.com/img/logo.jpg' border='0'/>"
                db.ePostaGonder(userName + " " + surName + " adlı personelinizden yeni masraf kayıtları gönderildi.", epostaStr, epostaContentStr + "<br/><br/><a href='http://www.kf-ik.com/' target='_blank'><img src='http://www.kf-ik.com/img/logo.jpg' width='701' height='200' border='0'/></a>", "masraf@kf-ik.com", "Kuy50523", Server.MapPath("~/upload/pdf/") + fileName + fileExt);

                //Response.Redirect("masrafformulist.aspx?mod=yg&monthe=" + ddlMonth.SelectedItem.Text.ToString() + "&yeare=" + ddlYear.SelectedItem.Text.ToString());
                Response.Redirect("masrafformulist.aspx?mod=yg");
                //}
                //else
                //{
                //    Response.Redirect("masrafformulist.aspx?mod=nsyg");
                //}
            }
            else
            {
                this.ltrlInfoYg.Visible = true;
                this.ltrlInfoYg.Text = "Lütfen PDF olarak hazırladığınız masraf fiş dosyanızı seçiniz.";
            }
        }
    }
}