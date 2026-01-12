using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Masraf
{
    public partial class _default : System.Web.UI.Page
    {
        dbLibrary db = new dbLibrary();
        public bool isYoneticiMi = false;
        public string generalSumStrMasrafOnaylanma = string.Empty;
        public string generalSumTitleMasrafOnaylanma = string.Empty;
        public string generalSumNumberMasrafOnaylanma = string.Empty;
        public int generalSumCountMasrafOnaylanma = 0;
        public string generalSumStrMasrafAy = string.Empty;
        public string generalSumTitleMasrafAy = string.Empty;
        public string generalSumNumberMasrafAy = string.Empty;
        public int generalSumCountMasrafAy = 0;
        public string generalSumStrProofMagaza = string.Empty;
        public string generalSumTitleProofMagaza = string.Empty;
        public string generalSumNumberProofMagaza = string.Empty;
        public int generalSumCountProofMagaza = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.checkUser();

                if (isYoneticiMi)
                {
                    this.divPersonel.Visible = false;
                    this.divPersonellerim.Visible = true;
                    this.getpersonnels();
                }
                else
                {
                    this.divPersonel.Visible = true;
                    this.getpersonnelKendisi();
                }

                DataRow dr = db.getValueWhere1("select * from tbl_users", "usr_user_id", "=", db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], string.Empty, "ConString");

                //Buraya giriş yapan kullanıcı Bülent ise tüm personelleri görüntülemesi için kod denendi.
                if (db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] == "28")
                {
                    this.getpersonnelsAll();
                    divPersonel.Visible = false;
                    divPersonellerim.Visible = true;
                }

                #region //rapor
                //string ayTemp = string.Empty;
                //DataTable dtMasrafOnaylanma = null;
                //try
                //{
                //    generalSumStrMasrafOnaylanma = "Masraf Onaylanma Durum";

                //    dtMasrafOnaylanma = db.getDataTable("select COUNT(*) as onaylanmis, (select COUNT(*) from tbl_masrafFormu m where isActive=0 and masrafFormuOnay=0) as onaylanmamis from tbl_masrafFormu m where isActive=1 and masrafFormuOnay=1", "ConString");
                //    if ((dtMasrafOnaylanma != null) && dtMasrafOnaylanma.Rows.Count > 0)
                //    {
                //        generalSumTitleMasrafOnaylanma += "\"" + "Onaylanmış Masraf Adet " + "(" + dtMasrafOnaylanma.Rows[0]["onaylanmis"].ToString() + ")" + "\"" + ",";
                //        generalSumNumberMasrafOnaylanma += "\"" + dtMasrafOnaylanma.Rows[0]["onaylanmis"].ToString().Replace(",", ".") + "\"" + ",";
                //        generalSumTitleMasrafOnaylanma += "\"" + "Onaylanmamış Masraf Adet " + "(" + dtMasrafOnaylanma.Rows[0]["onaylanmamis"].ToString() + ")" + "\"" + ",";
                //        generalSumNumberMasrafOnaylanma += "\"" + dtMasrafOnaylanma.Rows[0]["onaylanmamis"].ToString().Replace(",", ".") + "\"" + ",";

                //        generalSumCountMasrafOnaylanma += Convert.ToInt32(dtMasrafOnaylanma.Rows[0]["onaylanmis"].ToString()) + Convert.ToInt32(dtMasrafOnaylanma.Rows[0]["onaylanmamis"].ToString());

                //        generalSumTitleMasrafOnaylanma = generalSumTitleMasrafOnaylanma.Remove(generalSumTitleMasrafOnaylanma.Length - 1);
                //        generalSumNumberMasrafOnaylanma = generalSumNumberMasrafOnaylanma.Remove(generalSumNumberMasrafOnaylanma.Length - 1);
                //    }
                //    else
                //    {
                //        generalSumNumberMasrafOnaylanma = "0";
                //    }
                //}
                //catch (Exception)
                //{

                //}

                //DataTable dtMasrafMonth = null;
                //try
                //{
                //    generalSumStrMasrafAy = "Masraf Ay Kırılım";

                //    dtMasrafMonth = db.getDataTable("SELECT DATEPART(MONTH, masrafFormuDate) AS Ay, COUNT(masrafFormuId) AS adet FROM [dbo].[tbl_masrafFormu] WHERE isActive=1 GROUP BY DATEPART(MONTH, masrafFormuDate)", "ConString");
                //    if ((dtMasrafMonth != null) && dtMasrafMonth.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dtMasrafMonth.Rows.Count; i++)
                //        {
                //            ayTemp = db.monthArry.GetValue(Convert.ToInt32(dtMasrafMonth.Rows[i]["Ay"].ToString()) - 1).ToString();

                //            generalSumTitleMasrafAy += "\"" + System.Web.HttpUtility.UrlDecode(ayTemp) + "(" + dtMasrafMonth.Rows[i]["adet"].ToString() + ")" + "\"" + ",";
                //            generalSumNumberMasrafAy += "\"" + dtMasrafMonth.Rows[i]["adet"].ToString().Replace(",", ".") + "\"" + ",";

                //            generalSumCountMasrafAy += Convert.ToInt32(dtMasrafMonth.Rows[i]["adet"].ToString());
                //        }
                //        generalSumTitleMasrafAy = generalSumTitleMasrafAy.Remove(generalSumTitleMasrafAy.Length - 1);
                //        generalSumNumberMasrafAy = generalSumNumberMasrafAy.Remove(generalSumNumberMasrafAy.Length - 1);
                //    }
                //    else
                //    {
                //        generalSumNumberMasrafAy = "0";
                //    }
                //}
                //catch (Exception)
                //{

                //}

                //DataTable dtProofMagaza = null;
                //try
                //{
                //    generalSumStrProofMagaza = "Masraf Ay Tutar (TL)";

                //    dtProofMagaza = db.getDataTable("SELECT DATEPART(MONTH, masrafFormuDate) AS Ay, sum(masrafFormuKDVDahilTutar) AS tutar FROM [dbo].[tbl_masrafFormu] WHERE isActive=1 and masrafFormuOnay=1 GROUP BY DATEPART(MONTH, masrafFormuDate)", "ConString");
                //    if ((dtProofMagaza != null) && dtProofMagaza.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dtProofMagaza.Rows.Count; i++)
                //        {
                //            ayTemp = db.monthArry.GetValue(Convert.ToInt32(dtProofMagaza.Rows[i]["Ay"].ToString()) - 1).ToString();

                //            generalSumTitleProofMagaza += "\"" + System.Web.HttpUtility.UrlDecode(ayTemp) + "(" + dtProofMagaza.Rows[i]["tutar"].ToString() + " TL)" + "\"" + ",";
                //            generalSumNumberProofMagaza += "\"" + dtProofMagaza.Rows[i]["tutar"].ToString().Replace(",", ".") + "\"" + ",";

                //            generalSumCountProofMagaza += Convert.ToInt32(dtProofMagaza.Rows[i]["tutar"].ToString());
                //        }
                //        generalSumTitleProofMagaza = generalSumTitleProofMagaza.Remove(generalSumTitleProofMagaza.Length - 1);
                //        generalSumNumberProofMagaza = generalSumNumberProofMagaza.Remove(generalSumNumberProofMagaza.Length - 1);
                //    }
                //    else
                //    {
                //        generalSumNumberProofMagaza = "0";
                //    }
                //}
                //catch (Exception)
                //{

                //}
                #endregion
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
        void getpersonnels()
        {
            personel pObj = new personel();
            DataTable dt = null;
            //string sqlQuery = "select u.usr_user_id,u.usr_name,u.usr_surname,u.usr_email_address,u.usr_phone, (Select count(yoneticiyeGonderildimi) from tbl_masrafFormu where masrafFormuOnaylayacakId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " and yoneticiyeGonderildimi=1 and masrafFormuOnay = 1 and masrafFormuDate > DATEADD(mm, DATEDIFF(mm, 0, getdate()), 0) and masrafFormuDate < DATEADD(mm, DATEDIFF(mm, 0, getdate()) + 1, 0)) as gonderilenMasrafAdet, (Select count(yoneticiyeGonderildimi) from tbl_masrafFormu where masrafFormuOnaylayacakId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " and yoneticiyeGonderildimi=0 and masrafFormuOnay = 0 and masrafFormuDate > DATEADD(mm, DATEDIFF(mm, 0, getdate()), 0) and masrafFormuDate < DATEADD(mm, DATEDIFF(mm, 0, getdate()) + 1, 0)) as gonderilmeyenMasrafAdet from tbl_users u where u.usr_status = '1' and u.usr_rank = 'personel' and u.yoneticiId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " order by usr_name asc";

            //dt = db.getDataTable(sqlQuery, "ConString");
            dt = pObj.getpersonnels(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
            if ((dt != null) && dt.Rows.Count > 0)
            {
                th3.Visible = false;
                this.rptpersoneladmin.Visible = false;
                this.divPersonellerim.Visible = true;
                rptpersonel.DataSource = dt.DefaultView;
                rptpersonel.DataBind();
            }
        }
        void getpersonnelKendisi()
        {
            personel pObj = new personel();
            DataTable dt = null;
            //string sqlQuery = "SET LANGUAGE Turkish select YEAR(mf.masrafFormuDate) as Yıl, DATEPART(month, mf.masrafFormuDate) as sirano, DATENAME(month, mf.masrafFormuDate) as Ay, SUM(mf.masrafFormuKDVDahilTutar) as Toplam from [dbo].[tbl_masrafFormu] mf inner join tbl_users u on mf.createdUserId = u.usr_user_id inner join tbl_masrafGrup mg on mf.masrafFormuGrupId = mg.masrafGrupId where mf.masrafFormuOnay=1 and mf.isActive=1 and mf.yoneticiyeGonderildimi=1 and mf.createdUserId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " group by YEAR(mf.masrafFormuDate), DATENAME(month, mf.masrafFormuDate), DATEPART(month, mf.masrafFormuDate) order by sirano";

            //dt = db.getDataTable(sqlQuery, "ConString");
            dt = pObj.getpersonnelKendisi(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
            if ((dt != null) && dt.Rows.Count > 0)
            {
                this.rptpersoneladmin.Visible = false;
                this.divPersonel.Visible = true;
                this.rptrPersonelKendisi.DataSource = dt.DefaultView;
                this.rptrPersonelKendisi.DataBind();
            }
        }
        void getpersonnelsAll()
        {
            personel pObj = new personel();
            DataTable dt = null;

            //string sqlQuery = "SELECT u.usr_name , u.usr_surname , SUM(m.masrafFormuKDVDahilTutar) AS 'ToplamTutar', u.usr_email_address, u.usr_phone FROM [dbo].[tbl_users] u JOIN [dbo].[tbl_masrafFormu] m ON u.usr_user_id = m.createdUserId WHERE (MONTH(m.masrafFormuDate) = MONTH(DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)) AND YEAR(m.masrafFormuDate) = YEAR(DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0))) AND m.YoneticiyeGonderildimi = 1 GROUP BY u.usr_user_id, u.usr_name , u.usr_surname, u.usr_email_address, u.usr_phone order by u.usr_name";

            //dt = db.getDataTable(sqlQuery, "ConString");
            dt = pObj.getpersonnelsAll();
            if ((dt != null) && dt.Rows.Count > 0)
            {
                this.rptpersonel.Visible = false;
                th1.Visible = false;
                th2.Visible = false;
                th3.Visible = true;
                this.rptpersoneladmin.Visible = true;
                rptpersoneladmin.DataSource = dt.DefaultView;
                rptpersoneladmin.DataBind();
            }
            else
            {
                th1.Visible = false;
                th2.Visible = false;
            }
        }
        //string getgonderilenMasrafAdet(string userId)
        //{
        //    DataTable dt = null;
        //    string sqlQuery = "(Select count(yoneticiyeGonderildimi) as gonderilenMasrafAdet from tbl_masrafFormu where masrafFormuOnaylayacakId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " and yoneticiyeGonderildimi=1 and isActive=1 AND masrafFormuOnay=1 and yoneticiyeGonderildimi=1 and createdUserId=" + userId + " and createdDate > DATEADD(mm, DATEDIFF(mm, 0, getdate()) - 1, 0) and createdDate < DATEADD(mm, DATEDIFF(mm, 0, getdate()), 0))";

        //    dt = db.getDataTable(sqlQuery, "ConString");
        //    if ((dt != null) && dt.Rows.Count > 0)
        //    {
        //        return dt.Rows[0]["gonderilenMasrafAdet"].ToString();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        string getgonderilenMasrafKendisi()
        {
            personel pObj = new personel();
            DataTable dt = null;
            //string sqlQuery = "SELECT TOP 1 createdDate FROM [dbo].[tbl_masrafFormu] mf WHERE mf.createdUserId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]+ " ORDER BY mf.masrafFormuDate DESC";

            //dt = db.getDataTable(sqlQuery, "ConString");
            dt = pObj.getgonderilenMasrafKendisi(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1]);
            if ((dt != null) && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["createdDate"].ToString();
            }
            else
            {
                return null;
            }
        }
        string getgonderilmeyenMasrafAdet(string personelId)
        {
            personel pObj = new personel();
            DataTable dt = null;
            //string sqlQuery = "(Select count(yoneticiyeGonderildimi) as gonderilmeyenMasrafAdet from tbl_masrafFormu where masrafFormuOnaylayacakId=" + db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] + " and yoneticiyeGonderildimi=0 and isActive=1 AND masrafFormuOnay=1 and yoneticiyeGonderildimi=0 and createdUserId=" + personelId + " and createdDate > DATEADD(mm, DATEDIFF(mm, 0, getdate()) - 1, 0) and createdDate < DATEADD(mm, DATEDIFF(mm, 0, getdate()), 0))";

            //dt = db.getDataTable(sqlQuery, "ConString");
            dt = pObj.getgonderilmeyenMasrafAdet(db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1], personelId);
            if ((dt != null) && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["gonderilmeyenMasrafAdet"].ToString();
            }
            else
            {
                return null;
            }
        }
        protected void rptpersonel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                Literal ltrlgonderilenMasrafAdet = (Literal)e.Item.FindControl("ltrlgonderilenMasrafAdet");
                Literal ltrlgonderilmeyenMasrafAdet = (Literal)e.Item.FindControl("ltrlgonderilmeyenMasrafAdet");
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trRow");

                //ltrlgonderilenMasrafAdet.Text = getgonderilenMasrafAdet(rV["usr_user_id"].ToString());
                ltrlgonderilmeyenMasrafAdet.Text = getgonderilmeyenMasrafAdet(rV["usr_user_id"].ToString());

                if (db.checkCookie(db.Decrypt(Request.Cookies["ckmasrafweb"]["SessId"].ToString())).Split(',')[1] == "28")
                {
                    if (Convert.ToInt32(ltrlgonderilenMasrafAdet.Text) == 0 && Convert.ToInt32(ltrlgonderilmeyenMasrafAdet.Text) >= 0)
                    {
                        trRow.Attributes.Add("style", "background-color:mediumaquamarine !important");
                    }
                    if (Convert.ToInt32(ltrlgonderilenMasrafAdet.Text) > 0 && Convert.ToInt32(ltrlgonderilmeyenMasrafAdet.Text) == 0)
                    {
                        trRow.Attributes.Add("style", "background-color:mediumaquamarine !important");
                    }
                }
            }
        }
        protected void rptpersoneladmin_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trRow");
                trRow.Attributes.Add("style", "background-color:mediumaquamarine !important");
            }
        }
        protected void rptrPersonelKendisi_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rV = (DataRowView)e.Item.DataItem;

                Literal ltrlTarih = (Literal)e.Item.FindControl("ltrlTarih");
                Literal ltrlDonem = (Literal)e.Item.FindControl("ltrlDonem");
                Literal ltrlToplamTutar = (Literal)e.Item.FindControl("ltrlToplamTutar");
                HtmlAnchor aDetay = (HtmlAnchor)e.Item.FindControl("aDetay");

                ltrlTarih.Text = rV["Ay"].ToString() + " " + rV["Yıl"].ToString();
                //ltrlTarih.Text = getgonderilenMasrafKendisi();

                if (rV["Ay"].ToString() == "Ocak" || rV["Ay"].ToString() == "Şubat" || rV["Ay"].ToString() == "Mart")
                {
                    ltrlDonem.Text = "1. Çeyrek";
                }
                if (rV["Ay"].ToString() == "Nisan" || rV["Ay"].ToString() == "Mayıs" || rV["Ay"].ToString() == "Haziran")
                {
                    ltrlDonem.Text = "2. Çeyrek";
                }
                if (rV["Ay"].ToString() == "Temmuz" || rV["Ay"].ToString() == "Ağustos" || rV["Ay"].ToString() == "Eylül")
                {
                    ltrlDonem.Text = "3. Çeyrek";
                }
                if (rV["Ay"].ToString() == "Ekim" || rV["Ay"].ToString() == "Kasım" || rV["Ay"].ToString() == "Aralık")
                {
                    ltrlDonem.Text = "4. Çeyrek";
                }

                ltrlToplamTutar.Text = rV["Toplam"].ToString() + "&nbspTL";

                aDetay.HRef = "masrafaylikdetay.aspx?ay=" + rV["sirano"].ToString() + "&yil=" + rV["Yıl"].ToString();
            }
        }
        protected void btnDetay_Click(object sender, EventArgs e)
        {
            Response.Redirect("masrafaylikdetay.aspx?month=");
        }
    }
}