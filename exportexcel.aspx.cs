using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Masraf
{
    public partial class exportexcel : System.Web.UI.Page
    {
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

                DataTable dtPersonel = null;
                dtPersonel = db.getDataTable("select * from tbl_users where usr_status = 1 and usr_rank = 'personel' order by usr_name asc", "ConString");
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

                    this.ddlPersonnel.Items.Insert(0, new ListItem("-Seçiniz-", "-1"));
                }

                if (Request.QueryString["personnelid"] != null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["personnelid"].ToString()))
                    {
                        this.getMasrafFiltre(Request.QueryString["month"].ToString() + "|" + Request.QueryString["year"].ToString() + "|" + Request.QueryString["personnelid"].ToString());
                    }
                    if (Request.QueryString["mod"] != null)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["mod"].ToString()))
                        {
                            this.exportExcel(Request.QueryString["month"].ToString() + "|" + Request.QueryString["year"].ToString() + "|" + Request.QueryString["personnelid"].ToString());
                        }
                    }
                }

                double toplamTutar = 0;
                foreach (RepeaterItem item in rptMasrafFormu.Items)
                {
                    Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                    toplamTutar += Convert.ToDouble(lblMasrafFormuKDVDahilTutar.Text);
                }
                ltrlToplamTutar.Text = "Toplam Tutar: " + toplamTutar.ToString() + " TL";

                double normalGider = 0;
                foreach (RepeaterItem item in rptMasrafFormu.Items)
                {
                    Literal ltrlKDVsiz = (Literal)item.FindControl("ltrlKDVsiz");
                    normalGider += Convert.ToDouble(ltrlKDVsiz.Text);
                }
                ltrlNormalGider.Text = "Normal Gider: " + normalGider.ToString() + " TL";

                ltrlKKEG.Text = "KKEG: " + (toplamTutar - normalGider).ToString() + " TL";
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
        void exportExcel(string filter)
        {
            DataTable dt = null;
            Array filterArry = filter.Split('|');

            int year;
            int month = Convert.ToInt32(filterArry.GetValue(0).ToString());
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

            if (nextMonth > 12)
            {
                year = Convert.ToInt32(filterArry.GetValue(1).ToString());
                nextMonthTemp = "01";
                filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + (year + 1).ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;
            }
            else
            {
                year = Convert.ToInt32(filterArry.GetValue(1).ToString());
                filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;
            }

            this.ddlMonth.SelectedValue = month.ToString();
            this.ddlYear.Items.FindByText(year.ToString()).Selected = true;
            this.ddlPersonnel.SelectedValue = personnelId;

            //filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;

            DataSet ds = new DataSet();
            string temp = string.Empty;

            dt = db.getDataTable("select ROW_NUMBER() OVER (ORDER BY masrafFormuDate ASC) AS SiraNo, masrafFormuDate, masrafFormuKimdenAlindigi AS kimdenAlindigi, masrafFormuAciklama AS aciklama, mg.masrafGrupName AS grupName, masrafFormuOKCFisNo AS fisNo, masrafFormuGiderAnaHesabi AS giderAnaHesabi, masrafFormuKDVDahilTutar AS kdvDahilTutar, masrafFormuKKEG AS kanunenKabulEdilmeyenGider, (u.usr_name + ' ' + u.usr_surname) AS personelAdSoyad, u.usr_accountcode AS muhasebeKodu,  u.usr_codeuse from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId=u.usr_user_id inner join tbl_masrafGrup mg on mf.masrafFormuGrupId = mg.masrafGrupId where mf.isActive=1 and mf.masrafFormuOnay=1 " + filter + " order by masrafFormuDate asc", "ConString");

            decimal toplamTutar = 0;
            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                Label lblMasrafFormuKDVDahilTutar = (Label)item.FindControl("lblMasrafFormuKDVDahilTutar");
                toplamTutar += Math.Round(Convert.ToDecimal(lblMasrafFormuKDVDahilTutar.Text), 2);
            }
            ltrlToplamTutar.Text = "Toplam Tutar: " + toplamTutar.ToString() + " TL";

            decimal normalGider = 0;
            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                Literal ltrlKDVsiz = (Literal)item.FindControl("ltrlKDVsiz");
                normalGider += Math.Round(Convert.ToDecimal(ltrlKDVsiz.Text), 2);
            }
            ltrlNormalGider.Text = "Normal Gider: " + normalGider.ToString() + " TL";

            ltrlKKEG.Text = "KKEG: " + (toplamTutar - normalGider).ToString() + " TL";

            decimal normalGiderTemp = Math.Round(Convert.ToDecimal(normalGider), 2);
            string personelAdSoyadTemp = string.Empty;
            string muhasebeKodTemp = string.Empty;
            decimal KKEGTemp = Math.Round(Convert.ToDecimal((toplamTutar - normalGider)), 2);
            decimal genelToplamTemp = normalGiderTemp + KKEGTemp;

            Random rnd = new Random();
            //int randomNumber = rnd.Next(10000, 99999);
            //string belgeNo = "KFM" + randomNumber.ToString();

            string usrCodeUse = dt.Rows[0]["usr_codeuse"].ToString();
            if (!string.IsNullOrEmpty(usrCodeUse))
            {
                int codeValue = int.Parse(usrCodeUse);

                if (codeValue < 10)
                {
                    usrCodeUse = "0" + codeValue.ToString();
                }
            }

            string selectedMonth = ddlMonth.SelectedItem.Value.ToString();
            if (!string.IsNullOrEmpty(selectedMonth))
            {
                int monthValue = int.Parse(selectedMonth);

                if (monthValue < 10)
                {
                    selectedMonth = "0" + monthValue.ToString();
                }
            }

            string belgeNo = "KFM " + ddlYear.SelectedItem.Text.ToString() + selectedMonth + usrCodeUse;//+ randomNumber.ToString();

            dt.Columns.Add("masrafFormuDateStr", typeof(string));
            int index = -1;

            if ((dt != null) && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows) // search whole table
                {
                    index++;
                    if (!string.IsNullOrEmpty(dr["masrafFormuDate"].ToString()))
                    {
                        //dr["masrafFormuDate"] = dr["masrafFormuDate"].ToString().Replace(" 00:00:00", "");
                        DateTime dtime = DateTime.Parse(dr["masrafFormuDate"].ToString()).Date;
                        dr["masrafFormuDateStr"] = DateTime.Parse(dtime.ToShortDateString()).ToString("dd.MM.yyyy");
                        //dt.Rows[index].SetField("masrafFormuDate", Convert.ToDateTime(dr["masrafFormuDate"].ToString()).Date);
                    }
                }

                dt.Columns["SiraNo"].ColumnName = "Sıra";
                dt.Columns["masrafFormuDate"].ColumnName = "Tarih";
                dt.Columns["kimdenAlindigi"].ColumnName = "Kimden Alındı";
                dt.Columns["aciklama"].ColumnName = "Açıklama";
                dt.Columns["grupName"].ColumnName = "Masraf Grubu";
                dt.Columns["fisNo"].ColumnName = "Fiş No";
                dt.Columns["giderAnaHesabi"].ColumnName = "Gider Ana Hesabı";
                dt.Columns["kdvDahilTutar"].ColumnName = "KDV Dahil Tutar";
                dt.Columns["kanunenKabulEdilmeyenGider"].ColumnName = "Kanunen Kabul Edilmeyen Tutar";
                dt.Columns["personelAdSoyad"].ColumnName = "Personel Ad Soyad";
                dt.Columns["muhasebeKodu"].ColumnName = "Muhasebe Kodu";

                dt.Columns.Remove("usr_codeuse");

                ds.Tables.Add(dt); //Boş olarak yaratılan DataSet'e dt'ye tanımlanan proof tablosu eklendi

                personelAdSoyadTemp = dt.Rows[0]["Personel Ad Soyad"].ToString();
                muhasebeKodTemp = dt.Rows[0]["Muhasebe Kodu"].ToString();

                string fileName = "MF_" + dt.Rows[0]["Personel Ad Soyad"].ToString().Replace(" ", "_") + "_" + ddlMonth.SelectedItem.Text + "_" + ddlYear.SelectedItem.Text + ".xls"; //Dışa aktarıldığında dosya Rapor-dışa aktarılan tarih şeklinde isimlendirildi.
                Response.ClearContent();
                Response.AddHeader("content-disposition", string.Format("attachment;filename=\"{0}\"", fileName)); //Ekin indirilmesini sağlayan kod betiği.
                Response.ContentType = "application/vnd.ms-excel"; //İndirildiğinde Excel olarak düşmesini sağlandı.
                Response.ContentEncoding = System.Text.Encoding.Unicode; //Dosya ismini okunabilir bir biçimde kullanıcıya gösterdi.
                Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

                temp = "<table class=\"table\" style=\"font-family:Arial;\" cellspacing=\"10\">\r\n<tr>\r\n";

                for (int i = 0; i < ds.Tables.Count; i++) //Excel görünümünü sağlayacak kod betiği.
                {
                    if (i == 0) //İlk sütunun ilk satırına başlık tanımlandı.
                    {
                        temp += "<td colspan=\"8\" style=\"background-color:#FFFFFF;text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">PERAKENDE SATIŞ VESİKALARI İLE TEVSİK EDİLEN GİDERLER İCMALİ<br style=\"mso-data-placement:same-cell;\" />MASRAF LİSTESİ</td>\r\n</tr>\r\n<tr>\r\n<td colspan=\"3\" style=\"background-color:#FFFFFF;text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">Mükellefin Ünvanı :<br style=\"mso-data-placement:same-cell;\" />KIRMIZI FİKİRLER TANITIM HİZ.LTD.ŞTİ.</td>\r\n<td colspan=\"3\" style=\"background-color:#DCDCDC;text-align:center;font-size:11pt;border:1px solid #000000;font-weight:bold;\">Vergi Kimlik Numarası :<br style=\"mso-data-placement:same-cell;\" />9210097959</td>\r\n<td style=\"background-color:#DCDCDC;text-align:center;vertical-align:middle;font-size:9pt;border:1px solid #000000;font-weight:bold;\">Sayı :<br> Muhasebe Kodu :</td>\r\n<td style=\"background-color:#DCDCDC;vertical-align:middle;font-size:11pt;border:1px solid #000000;font-weight:bold;\">" + belgeNo.ToString() + "<br>" + muhasebeKodTemp + "</td>\r\n<table>";
                    }
                    temp += "<table><tr>";
                    foreach (DataColumn dc in ds.Tables[i].Columns) //Her bir sütuna veritabanındaki tanımlamaları eklendi.
                    {
                        if (dc.ColumnName != "Kanunen Kabul Edilmeyen Tutar" && dc.ColumnName != "Personel Ad Soyad" && dc.ColumnName != "masrafFormuDateStr" && dc.ColumnName != "Muhasebe Kodu")
                        {
                            temp += "\r\n<td style=\"background-color:#DCDCDC;text-align:center;font-size:10pt;font-weight:bold;border:1px solid #000000;\">" + dc.ColumnName + "</td>\r\n" + "\t";
                        }
                    }
                    temp += "</tr>";
                    foreach (DataRow dr in ds.Tables[i].Rows) //Ne kadar veri varsa o kadar veri çekildi, en sondaki veriye kadar.
                    {
                        temp += "<tr>";
                        for (int j = 0; j < ds.Tables[i].Columns.Count - 4; j++)
                        {
                            if (j == 1)
                            {
                                temp += "\r\n<td style=\"vertical-align:top;font-size:9pt;border:1px solid #ebebeb;\">" + dr[11].ToString() + "</td>\r\n" + "\t";
                            }
                            else if (j == 0)
                            {
                                temp += "\r\n<td style=\"vertical-align:top;font-size:9pt;border:1px solid #ebebeb\">" + dr[0].ToString() + "</td>\r\n" + "\t";
                            }
                            else
                            {
                                temp += "\r\n<td style=\"vertical-align:top;font-size:9pt;border:1px solid #ebebeb\">" + dr[j].ToString() + "</td>\r\n" + "\t";
                            }

                            if (ds.Tables[i].Columns.Count - 2 == j + 1)
                            {
                                normalGiderTemp += Math.Round(Convert.ToDecimal(dr[j].ToString()), 2);
                                //personelAdSoyadTemp = dr[8].ToString();
                            }
                            if (ds.Tables[i].Columns.Count + 1 == j)
                            {
                                KKEGTemp += Math.Round(Convert.ToDecimal(dr[j].ToString()), 2);
                                //personelAdSoyadTemp = dr[8].ToString();
                            }
                        }

                        temp += "\n";
                        temp += "</tr>";
                    }
                    temp += "\r\n<tr>\r\n<td colspan=\"3\" style=\"vertical-align:top;font-size:9pt;font-weight:bold;\">GENEL TOPLAM</td>\r\n<td></td>\r\n<td></td>\r\n<td style=\"vertical-align:top;font-size:9pt;font-weight:bold;\"> </td>\r\n<td></td>\r\n<td style=\"vertical-align:top;font-size:11pt;font-weight:bold;\"> " + genelToplamTemp.ToString() + " </td>\r\n<td style=\"vertical-align:top;font-size:9pt;font-weight:bold;\"> </td>\r\n</tr>\r\n<tr>\r\n<td colspan=\"8\"></td>\r\n</tr>\r\n<tr>\r\n<td colspan=\"2\" style=\"font-size:9pt;\">MASRAFI YAPAN PERSONEL İSİM VE SOYAD </td>\r\n<td colspan=\"3\"></td>\r\n</tr>\r\n<tr><td colspan=\"2\" style=\"font-size:9pt;\">" + personelAdSoyadTemp + "</td></tr>\r\n<tr>\r\n<td colspan=\"3\"></td>\r\n<td rowspan=\"3\" style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;vertical-align:middle\">DAĞILIM</td>\r\n<td style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">KKEG</td>\r\n<td style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">" + Math.Round(Convert.ToDecimal(KKEGTemp), 2) + "</td>\r\n</tr>\r\n<tr>\r\n<td colspan=\"3\"></td>\r\n<td style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">NORMAL GİDER</td>\r\n<td style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">" + Math.Round(Convert.ToDecimal(genelToplamTemp - KKEGTemp), 2) + "</td>\r\n</tr>\r\n<tr>\r\n<td colspan=\"3\"></td>\r\n<td style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\">TOPLAM GİDER</td>\r\n<td style=\"text-align:center;font-size:11pt;font-weight:bold;border:1px solid #000000;\"> " + Math.Round(genelToplamTemp, 2) + "</td>\r\n</tr>";
                    temp += "</table>";
                }

                temp += "</table></table>";

                Response.Write(temp); //Eklenen sütun ve satırlara göre temp bilinmeyeni dolduruldu ve kullanıcıya sunuldu.
                Response.End();
            }
        }
        void getMasrafFiltre(string filter)
        {
            this.lnkBtnExcelOnaylananMasraflar.Visible = true;
            DataTable dt = null;
            Array filterArry = filter.Split('|');

            int year = Convert.ToInt32(filterArry.GetValue(1).ToString());
            int month = Convert.ToInt32(filterArry.GetValue(0).ToString());
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

            if(nextMonth > 12)
            {
                nextMonthTemp = "01";
                filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + (year + 1).ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;
            }
            else
            {
                filter = "and (masrafFormuDate >= '" + year.ToString() + "-" + monthTemp + "-01 00:00:00.000' and masrafFormuDate < '" + year.ToString() + "-" + nextMonthTemp + "-01 00:00:00.000') and mf.createdUserId=" + personnelId;
            }

            this.ddlMonth.SelectedValue = month.ToString();
            this.ddlYear.Items.FindByText(year.ToString()).Selected = true;
            this.ddlPersonnel.SelectedValue = personnelId;

            dt = db.getDataTable("select * from tbl_masrafFormu mf inner join tbl_users u on mf.createdUserId = u.usr_user_id inner join tbl_masrafGrup mg on mf.masrafFormuGrupId = mg.masrafGrupId where mf.isActive=1 and mf.masrafFormuOnay = 1 " + filter + " order by masrafFormuDate asc", "ConString");
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
        protected void lnkBtnExcelOnaylananMasraflar_Click(object sender, EventArgs e)
        {
            Response.Redirect("exportexcel.aspx?month=" + Server.UrlEncode(this.ddlMonth.SelectedItem.Value.ToString()) + "&year=" + Server.UrlEncode(this.ddlYear.SelectedItem.Text.ToString()) + "&personnelid=" + this.ddlPersonnel.SelectedItem.Value.ToString() + "&mod=excel");
        }
        protected void lnkBtnFiltrele_Click(object sender, EventArgs e)
        {
            Response.Redirect("exportexcel.aspx?month=" + Server.UrlEncode(this.ddlMonth.SelectedItem.Value.ToString()) + "&year=" + Server.UrlEncode(this.ddlYear.SelectedItem.Text.ToString()) + "&personnelid=" + this.ddlPersonnel.SelectedItem.Value.ToString());

            ltrlNormalGider.Visible = true;
            double normalGider = 0;
            foreach (RepeaterItem item in rptMasrafFormu.Items)
            {
                Label lblMasrafFormuKDVHaric = (Label)item.FindControl("lblMasrafFormuKDVHaric");
                normalGider += Convert.ToDouble(lblMasrafFormuKDVHaric.Text);
            }
            ltrlNormalGider.Text = "Normal Gider: " + normalGider.ToString() + "&nbsp;TL";
        }
        protected void rptMasrafFormu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                int rowIndex = e.Item.ItemIndex + 1;
                Label lblMasrafSiraNo = (Label)e.Item.FindControl("lblMasrafSiraNo");

                if (lblMasrafSiraNo != null)
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
                DateTime dtBas = DateTime.Parse(rV["masrafFormuDate"].ToString());
                ltrlMasrafFormuDate.Text = dtBas.ToString("dd.MM.yyyy");
                Label lblPDF = (Label)e.Item.FindControl("lblPDF");

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

                ltrlMasrafFormuDate.Text = dtBas.ToString("dd.MM.yyyy");
                ltrlMasrafFormuKimdenAlindigi.Text = rV["masrafFormuKimdenAlindigi"].ToString();
                lblCreatedUserId.Text = rV["createdUserId"].ToString();
                ltrlKDVsiz.Text = (Convert.ToDouble(rV["masrafFormuKDVDahilTutar"].ToString()) - Convert.ToDouble(rV["masrafFormuKKEG"].ToString())).ToString();
                lblPDF.Text = rV["pdf"].ToString();
            }
        }
    }
}