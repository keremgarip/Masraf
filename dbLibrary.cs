using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Web.Security;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Masraf
{
    public class dbLibrary
    {
        SqlDataReader te = null;

        #region Settings

        private static int _iterations = 2;
        private static int _keySize = 256;

        private static string _hash = "SHA1";
        private static string _salt = "aselriasGYhB0a32"; // Random
        private static string _vector = "89CFtg34awl34kjq"; // Random

        #endregion

        public string[] monthArry = { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

        public string[] yearArry = { "2023", "2024", "2025", "2026", "2027", "2028", "2029", "2030" };

        public bool ePostaGonder(string Konu, string Alici, string Mesaj, string kullanilcakEposta, string kullanilcakEpostaSifresi, string dosya)
        {
            bool actionOk = false;
            string tsrmliMsg = string.Empty;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (dosya == "")
                {
                    tsrmliMsg = "" + Mesaj + "";
                    SmtpClient istemci = new System.Net.Mail.SmtpClient("smtp.office365.com");
                    MailAddress gon = new MailAddress(kullanilcakEposta, "Masraf Formu");
                    MailAddress alan = new MailAddress(Alici);
                    MailMessage msg = new MailMessage(gon, alan);
                    msg.Subject = Konu;
                    msg.Body = tsrmliMsg;
                    msg.IsBodyHtml = true;
                    istemci.Port = 587;
                    istemci.EnableSsl = true;
                    NetworkCredential SMTPUserInfo = new NetworkCredential(kullanilcakEposta, kullanilcakEpostaSifresi);
                    istemci.UseDefaultCredentials = false;
                    istemci.Credentials = SMTPUserInfo;
                    istemci.Send(msg);
                    actionOk = true;
                }
                else
                {
                    tsrmliMsg = "" + Mesaj + "";
                    SmtpClient istemci = new System.Net.Mail.SmtpClient("smtp.office365.com");
                    MailAddress gon = new MailAddress(kullanilcakEposta, "Masraf Formu");
                    MailAddress alan = new MailAddress(Alici);
                    MailMessage msg = new MailMessage(gon, alan);
                    msg.Subject = Konu;
                    msg.Body = tsrmliMsg;
                    msg.Attachments.Add(new Attachment(dosya));
                    msg.IsBodyHtml = true;
                    istemci.Port = 587;
                    istemci.EnableSsl = true;
                    NetworkCredential SMTPUserInfo = new NetworkCredential(kullanilcakEposta, kullanilcakEpostaSifresi);
                    istemci.UseDefaultCredentials = false;
                    istemci.Credentials = SMTPUserInfo;
                    istemci.Send(msg);
                    actionOk = true;
                }
            }
            catch (Exception)
            {
                actionOk = false;
            }
            return actionOk;
        }

        public string dbStr()
        {
            string cnnStr = System.Configuration.ConfigurationManager.AppSettings["ConString"];

            return cnnStr;
        }
        public string clr(string clrValues)
        {
            string value = "";
            value = clrValues;
            value = value.Replace("~", "");
            //value = value.Replace("é", "");
            return value;
        }
        public string insertValue(string tableName, string databaseFields, string databaseValues, string ConString)
        {
            string parameters = "", result = "";
            int i = 0;
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string[] fields = databaseFields.Split(',');
                string[] values = databaseValues.Split('~');
                foreach (string item in fields)
                {
                    parameters = parameters + "@" + item + ",";
                }
                parameters = parameters.Substring(0, parameters.Length - 1);
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into " + tableName + " (" + databaseFields + ")" + " Values (" + parameters + ")", con);
                foreach (string item in fields)
                {
                    if (values[i].StartsWith("é"))
                    {
                        StringBuilder sb = new StringBuilder();
                        string s = values[i].Replace("é", "");
                        DateTime dt = DateTime.Parse(s);
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, dt));
                        i++;
                    }
                    else if ((i == 6 || i == 12) && tableName == "tbl_masrafFormu")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, Convert.ToDecimal(values[i])));
                        i++;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, values[i]));
                        i++;
                    }
                }
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                result = "true";
                return result;
            }
            catch (Exception)
            {
                result = "false";
                return result;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public string updateValueWhere1(string tableName, string databaseFields, string databaseValues, string whereField, string whereOperator, string whereValue, string ConString)
        {
            string UpdateParameters = "", updateField = "", updateResult = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                int i = 0;
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string[] fields = databaseFields.Split(',');
                string[] values = databaseValues.Split('~');
                foreach (string item in fields)
                {
                    UpdateParameters = UpdateParameters + "@" + item + ",";
                }
                UpdateParameters = UpdateParameters.Substring(0, UpdateParameters.Length - 1);
                con.Open();
                foreach (string item in fields)
                {
                    if (updateField == "")
                    {
                        updateField = item + "=" + "@" + item;
                    }
                    else
                    {
                        updateField = updateField + "," + item + "=" + "@" + item;
                    }
                }
                string where = "where " + whereField + whereOperator + "@" + whereField + "1";
                SqlCommand cmd = new SqlCommand("update " + tableName + " set " + updateField + " " + where, con);
                cmd.Parameters.AddWithValue("@" + whereField + "1", whereValue);
                foreach (string item in fields)
                {
                    if (values[i].StartsWith("é"))
                    {
                        StringBuilder sb = new StringBuilder();
                        string s = values[i].Replace("é", "");
                        DateTime dt = DateTime.Parse(s);
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, dt));
                        i++;
                    }
                    else if ((i == 5 || i == 9) && tableName == "tbl_masrafFormu")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, Convert.ToDecimal(values[i])));
                        i++;
                    }
                    else if (values[i] == "Né")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, DBNull.Value));
                        i++;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, values[i]));
                        i++;
                    }
                }
                cmd.ExecuteNonQuery();
                con.Dispose();
                con.Close();
                updateResult = "true";
                return updateResult;
            }
            catch (Exception)
            {
                updateResult = "false";
                return updateResult;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        internal string updateValueWhere1(string v1, string v2, string v3, string v4, string v5, string v6)
        {
            throw new NotImplementedException();
        }
        public string updateValueWhere2(string tableName, string databaseFields, string databaseValues, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string ConString)
        {
            string UpdateParameters = "", updateField = "", updateResult = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                int i = 0;
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string[] fields = databaseFields.Split(',');
                string[] values = databaseValues.Split('~');
                foreach (string item in fields)
                {
                    UpdateParameters = UpdateParameters + "@" + item + ",";
                }
                UpdateParameters = UpdateParameters.Substring(0, UpdateParameters.Length - 1);
                con.Open();
                foreach (string item in fields)
                {
                    if (updateField == "")
                    {
                        updateField = item + "=" + "@" + item;
                    }
                    else
                    {
                        updateField = updateField + "," + item + "=" + "@" + item;
                    }
                }
                string where = "where " + whereField + whereOperator + "@" + whereField + "1" + " " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + whereField2 + "2";
                SqlCommand cmd = new SqlCommand("update " + tableName + " set " + updateField + " " + where, con);
                cmd.Parameters.AddWithValue("@" + whereField + "1", whereValue);
                cmd.Parameters.AddWithValue("@" + whereField2 + "2", whereValue2);
                foreach (string item in fields)
                {
                    if (values[i].StartsWith("é"))
                    {
                        StringBuilder sb = new StringBuilder();
                        string s = values[i].Replace("é", "");
                        DateTime dt = DateTime.Parse(s);
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, dt));
                        i++;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, values[i]));
                        i++;
                    }
                }
                cmd.ExecuteNonQuery();
                con.Dispose();
                con.Close();
                updateResult = "true";
                return updateResult;
            }
            catch (Exception)
            {
                updateResult = "false";
                return updateResult;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public string updateValueWhere3(string tableName, string databaseFields, string databaseValues, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string ConString)
        {
            string UpdateParameters = "", updateField = "", updateResult = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                int i = 0;
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string[] fields = databaseFields.Split(',');
                string[] values = databaseValues.Split('~');
                foreach (string item in fields)
                {
                    UpdateParameters = UpdateParameters + "@" + item + ",";
                }
                UpdateParameters = UpdateParameters.Substring(0, UpdateParameters.Length - 1);
                con.Open();
                foreach (string item in fields)
                {
                    if (updateField == "")
                    {
                        updateField = item + "=" + "@" + item;
                    }
                    else
                    {
                        updateField = updateField + "," + item + "=" + "@" + item;
                    }
                }
                string where = "where " + whereField + whereOperator + "@" + whereField + "1" + " " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + whereField2 + "2" + " " + whereAndOr2 + " " + whereField3 + whereOperator3 + "@" + whereField3 + "3";
                SqlCommand cmd = new SqlCommand("update " + tableName + " set " + updateField + " " + where, con);
                cmd.Parameters.AddWithValue("@" + whereField + "1", whereValue);
                cmd.Parameters.AddWithValue("@" + whereField2 + "2", whereValue2);
                cmd.Parameters.AddWithValue("@" + whereField3 + "3", whereValue3);
                foreach (string item in fields)
                {
                    if (values[i].StartsWith("é"))
                    {
                        StringBuilder sb = new StringBuilder();
                        string s = values[i].Replace("é", "");
                        DateTime dt = DateTime.Parse(s);
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, dt));
                        i++;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(cmd.Parameters.AddWithValue("@" + item, values[i]));
                        i++;
                    }
                }
                cmd.ExecuteNonQuery();
                con.Dispose();
                con.Close();
                updateResult = "true";
                return updateResult;
            }
            catch (Exception)
            {
                updateResult = "false";
                return updateResult;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public string deleteValueWhere1(string tableName, string whereField, string whereOperator, string whereValue, string ConString)
        {
            string result = "", where = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                where = "where " + whereField + whereOperator + "@" + whereField + "1";
                SqlCommand cmd = new SqlCommand("Delete From " + tableName + " " + where, con);
                con.Open();
                cmd.Parameters.AddWithValue("@" + whereField + "1", whereValue);
                cmd.ExecuteNonQuery();
                con.Dispose();
                con.Close();
                result = "true";
            }
            catch (Exception)
            {
                result = "false";
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
            return result;
        }
        public string deleteValueWhere2(string tableName, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string ConString)
        {
            string result = "", where = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                where = "where " + whereField + whereOperator + "@" + whereField + "1 " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + whereField2 + "2";
                SqlCommand cmd = new SqlCommand("Delete From " + tableName + " " + where, con);
                con.Open();
                cmd.Parameters.AddWithValue("@" + whereField + "1", whereValue);
                cmd.Parameters.AddWithValue("@" + whereField2 + "2", whereValue2);
                cmd.ExecuteNonQuery();
                con.Dispose();
                con.Close();
                result = "true";
            }
            catch (Exception)
            {
                result = "false";
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
            return result;
        }
        public DataTable getDataTable(string selectQuery, string ConString)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                SqlCommand cmd = new SqlCommand(selectQuery, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                con.Dispose();
                con.Close();
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public DataTable getDataTableWhere1(string selectQuery, string whereField, string whereOperator, string whereValue, string orderBy, string ConString)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string sql = selectQuery + " Where " + whereField + " " + whereOperator + " " + "@" + whereField.Replace(".", "") + " " + orderBy;
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@" + whereField.Replace(".", ""), whereValue);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                con.Dispose();
                con.Close();
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public DataTable getDataTableWhere2(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string orderBy, string ConString)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);

            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string sql = selectQuery + " Where " + whereField + whereOperator + "@" + whereField.Replace(".", "").Replace(" ", "") + "1 " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + whereField2.Replace(".", "") + "2" + " " + orderBy;
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@" + whereField.Replace(".", "").Replace(" ", "") + "1", whereValue);
                cmd.Parameters.AddWithValue("@" + whereField2.Replace(".", "") + "2", whereValue2);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                con.Dispose();
                con.Close();
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public DataTable getDataTableWhere3(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string orderBy, string ConString)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string sql = selectQuery + " Where " + whereField + whereOperator + "@" + SeoURL(whereField).Replace("-", "").Replace(".", "") + "1" + " " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + SeoURL(whereField2).Replace("-", "").Replace(".", "") + "2" + " " + whereAndOr2 + " " + whereField3 + whereOperator3 + "@" + SeoURL(whereField3).Replace("-", "").Replace(".", "") + "3" + " " + orderBy;
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField).Replace("-", "").Replace(".", "") + "1", whereValue);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField2).Replace("-", "").Replace(".", "") + "2", whereValue2);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField3).Replace("-", "").Replace(".", "") + "3", whereValue3);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                con.Dispose();
                con.Close();
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public DataTable getDataTableWhere4(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string whereAndOr3, string whereField4, string whereOperator4, string whereValue4, string orderBy, string ConString)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            try
            {
                SqlConnection.ClearPool(con);
                SqlConnection.ClearAllPools();
                string sql = selectQuery + " Where " + whereField + whereOperator + "@" + SeoURL(whereField).Replace("-", "") + "1" + " " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + SeoURL(whereField2).Replace("-", "") + "2" + " " + whereAndOr2 + " " + whereField3 + whereOperator3 + "@" + SeoURL(whereField3).Replace("-", "") + "3" + " " + whereAndOr3 + " " + whereField4 + whereOperator4 + "@" + SeoURL(whereField4).Replace("-", "") + "4" + " " + orderBy;
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField).Replace(".", "").Replace("-", "") + "1", whereValue);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField2).Replace(".", "").Replace("-", "") + "2", whereValue2);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField3).Replace(".", "").Replace("-", "") + "3", whereValue3);
                cmd.Parameters.AddWithValue("@" + SeoURL(whereField4).Replace(".", "").Replace("-", "") + "4", whereValue4);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                con.Dispose();
                con.Close();
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                { con.Close(); }
            }
        }
        public DataRow getValue(string selectQuery, string ConString)
        {
            DataTable table = getDataTable(selectQuery, ConString);
            try
            {
                if (table.Rows.Count == 0) return null;
                return table.Rows[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataRow getValueWhere1(string selectQuery, string whereField, string whereOperator, string whereValue, string orderBy, string ConString)
        {
            DataTable table = getDataTableWhere1(selectQuery, whereField, whereOperator, whereValue, orderBy, ConString);
            try
            {
                if (table.Rows.Count == 0) return null;
                return table.Rows[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataRow getValueWhere2(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string orderBy, string ConString)
        {
            DataTable table = getDataTableWhere2(selectQuery, whereField, whereOperator, whereValue, whereAndOr, whereField2, whereOperator2, whereValue2, orderBy, ConString);
            try
            {
                if (table.Rows.Count == 0) return null;
                return table.Rows[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataRow getValueWhere3(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string orderBy, string ConString)
        {
            DataTable table = getDataTableWhere3(selectQuery, whereField, whereOperator, whereValue, whereAndOr, whereField2, whereOperator2, whereValue2, whereAndOr2, whereField3, whereOperator3, whereValue3, orderBy, ConString);
            try
            {
                if (table.Rows.Count == 0) return null;
                return table.Rows[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataRow getValueWhere4(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string whereAndOr3, string whereField4, string whereOperator4, string whereValue4, string orderBy, string ConString)
        {
            DataTable table = getDataTableWhere4(selectQuery, whereField, whereOperator, whereValue, whereAndOr, whereField2, whereOperator2, whereValue2, whereAndOr2, whereField3, whereOperator3, whereValue3, whereAndOr3, whereField4, whereOperator4, whereValue4, orderBy, ConString);
            try
            {
                if (table.Rows.Count == 0) return null;
                return table.Rows[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SqlDataReader getDataReader(string selectQuery, string ConString)
        {
            SqlConnection conn;
            SqlCommand comm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.AppSettings[ConString];
            conn = new SqlConnection(connectionString);
            try
            {
                SqlConnection.ClearPool(conn);
                SqlConnection.ClearAllPools();
                comm = new SqlCommand(selectQuery, conn);
                conn.Open();
                reader = comm.ExecuteReader();
                return reader;
            }
            catch (Exception)
            {
                return te;
            }
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //    { conn.Close(); }
            //}
        }
        public SqlDataReader getDataReaderWhere2(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string orderBy, string ConString)
        {
            SqlConnection conn;
            SqlCommand comm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.AppSettings[ConString];
            string sql = selectQuery + " Where " + whereField + whereOperator + "@" + whereField.Replace(".", "").Replace(" ", "") + "1 " + whereAndOr + " " + whereField2 + whereOperator2 + "@" + whereField2.Replace(".", "") + "2" + " " + orderBy;
            conn = new SqlConnection(connectionString);
            try
            {
                SqlConnection.ClearPool(conn);
                SqlConnection.ClearAllPools();
                comm = new SqlCommand(selectQuery, conn);
                comm.Parameters.AddWithValue("@" + whereField.Replace(".", "").Replace(" ", "") + "1", whereValue);
                comm.Parameters.AddWithValue("@" + whereField2.Replace(".", "") + "2", whereValue2);
                conn.Open();
                reader = comm.ExecuteReader();
                return reader;
            }
            catch (Exception)
            {
                return te;
            }
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //    { conn.Close(); }
            //}
        }
        public SqlDataAdapter getDataAdapter(string selectQuery, string ConString)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            SqlConnection.ClearPool(con);
            SqlConnection.ClearAllPools();
            SqlDataAdapter da = new SqlDataAdapter(selectQuery, con);
            return da;
        }
        public SqlDataAdapter getDataAdapterWhere1(string selectQuery, string whereField, string whereOperator, string whereValue, string orderBy, string ConString)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            SqlConnection.ClearPool(con);
            SqlConnection.ClearAllPools();
            SqlDataAdapter da = new SqlDataAdapter();
            string sql = selectQuery + " where " + whereField + " " + whereOperator + " " + "@" + whereField + " " + orderBy;
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@" + whereField, whereValue);
            da.SelectCommand = cmd;
            con.Close(); con.Dispose(); 
            return da;
        }
        public SqlDataAdapter getDataAdapterWhere2(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string orderBy, string ConString)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            SqlConnection.ClearPool(con);
            SqlConnection.ClearAllPools();
            SqlDataAdapter da = new SqlDataAdapter();
            string sql = selectQuery + " where " + whereField + " " + whereOperator + " @" + whereField + " " + whereAndOr + " " + whereField2 + " " + whereOperator2 + " @" + whereField2 + " " + orderBy;
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@" + whereField, whereValue);
            cmd.Parameters.AddWithValue("@" + whereField2, whereValue2);
            da.SelectCommand = cmd;
            con.Close(); con.Dispose();
            return da;
        }
        public SqlDataAdapter getDataAdapterWhere3(string selectQuery, string whereField, string whereOperator, string whereValue, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string OrderBy, string ConString)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            SqlConnection.ClearPool(con);
            SqlConnection.ClearAllPools();
            SqlDataAdapter da = new SqlDataAdapter();
            string sql = selectQuery + " where " + whereField + " " + whereOperator + " @" + whereField + " " + whereAndOr + " " + whereField2 + " " + whereOperator2 + " @" + whereField2 + " " + whereAndOr2 + " " + whereField3 + " " + whereOperator3 + " @" + whereField3 + " " + OrderBy;
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@" + whereField, whereValue);
            cmd.Parameters.AddWithValue("@" + whereField2, whereValue2);
            cmd.Parameters.AddWithValue("@" + whereField3, whereValue3);
            da.SelectCommand = cmd;
            con.Close(); con.Dispose(); 
            return da;
        }
        public SqlDataAdapter getDataAdapterWhere4(string selectQuery, string whereField, string whereOperator, string WhereDeger, string whereAndOr, string whereField2, string whereOperator2, string whereValue2, string whereAndOr2, string whereField3, string whereOperator3, string whereValue3, string whereAndOr3, string whereField4, string whereOperator4, string whereValue4, string orderBy, string ConString)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings[ConString]);
            SqlConnection.ClearPool(con);
            SqlConnection.ClearAllPools();
            SqlDataAdapter da = new SqlDataAdapter();
            string sql = selectQuery + " where " + whereField + " " + whereOperator + " @" + whereField + " " + whereAndOr + " " + whereField2 + " " + whereOperator2 + " @" + whereField2 + " " + whereAndOr2 + " " + whereField3 + " " + whereOperator3 + " @" + whereField3 + " " + whereAndOr3 + " " + whereField4 + " " + whereOperator4 + " @" + whereField4 + " " + orderBy;
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@" + whereField, WhereDeger);
            cmd.Parameters.AddWithValue("@" + whereField2, whereValue2);
            cmd.Parameters.AddWithValue("@" + whereField3, whereValue3);
            cmd.Parameters.AddWithValue("@" + whereField4, whereValue4);
            da.SelectCommand = cmd;
            con.Close(); con.Dispose();
            return da;
        }
        public string sendMail(string mailAdress, string subject, string maildetail, string yourmail, string yourpassword, string SMTPAdress, bool sslTrueFalse, int portNumber)
        {
            string sonuc = string.Empty;
            try
            {
                SmtpClient ss = new SmtpClient(SMTPAdress, portNumber);
                ss.DeliveryMethod = SmtpDeliveryMethod.Network;
                ss.UseDefaultCredentials = false;
                ss.Credentials = new NetworkCredential(yourmail, yourpassword);
                MailMessage mm = new MailMessage(yourmail, mailAdress);
                mm.IsBodyHtml = true;
                mm.Subject = subject;
                mm.Body = maildetail;
                ss.EnableSsl = sslTrueFalse;
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                ss.Send(mm);
                sonuc = "true";
                return sonuc;
            }
            catch (Exception)
            {
                sonuc = "false";
                return sonuc;
            }
        }
        public string SeoURL(string adress)
        {
            string value = string.Empty;

            value = adress.ToLower();
            value = value.Replace("ı", "i");
            value = value.Replace("ö", "o");
            value = value.Replace("ü", "u");
            value = value.Replace("ş", "s");
            value = value.Replace("ç", "c");
            value = value.Replace("ğ", "g");
            value = value.Replace("İ", "i");
            value = value.Replace("Ö", "o");
            value = value.Replace("Ü", "u");
            value = value.Replace("Ş", "s");
            value = value.Replace("Ç", "c");
            value = value.Replace("Ğ", "g");
            value = value.Replace("!", "");
            value = value.Replace("'", "");
            value = value.Replace("£", "");
            value = value.Replace("^", "");
            value = value.Replace("#", "");
            value = value.Replace("+", "");
            value = value.Replace("$", "");
            value = value.Replace("%", "");
            value = value.Replace("½", "");
            value = value.Replace("&", "");
            value = value.Replace("/", "");
            value = value.Replace("{", "");
            value = value.Replace("(", "");
            value = value.Replace("[", "");
            value = value.Replace(")", "");
            value = value.Replace("]", "");
            value = value.Replace("=", "");
            value = value.Replace("}", "");
            value = value.Replace("?", "");
            value = value.Replace("\\", "");
            value = value.Replace("_", "");
            value = value.Replace(" ", "-");
            value = value.Replace("|", "");
            value = value.Replace(";", "");
            value = value.Replace("`", "");
            value = value.Replace("<", "");
            value = value.Replace(">", "");
            value = value.Replace("@", "");
            value = value.Replace(".", "");
            value = value.Replace(":", "");
            value = value.Replace("*", "");
            value = value.Replace("\"", "");
            value = value.Replace("'", "");
            value = value.Replace("''", "");

            return value.ToLower();
        }
        public string getIPAdress(string value)
        {
            string IpAdress = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                IpAdress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                IpAdress = HttpContext.Current.Request.UserHostAddress;
            }
            return IpAdress;
        }
        public string MD5(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
        }
        public string sha512(string data)
        {
            SHA512 sha = new SHA512Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
        public string Encrypt(string value)
        {
            return Encrypt<AesManaged>(value);
        }
        public static string Encrypt<T>(string value)
                where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = ASCIIEncoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = ASCIIEncoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = ASCIIEncoding.ASCII.GetBytes(value);

            byte[] encrypted;
            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes =
                    new PasswordDeriveBytes("aHygbmgY28umdcX", saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (MemoryStream to = new MemoryStream())
                    {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }
        public string Decrypt(string value)
        {
            return Decrypt<AesManaged>(value);
        }
        public static string Decrypt<T>(string value) where T : SymmetricAlgorithm, new()
        {
            byte[] vectorBytes = ASCIIEncoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = ASCIIEncoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes("aHygbmgY28umdcX", saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream from = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }

                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }
        public string checkCookie(string sessionID)
        {
            string result;
            try
            {
                DataRow dr = getValueWhere1("select * from tbl_users", "usr_lastsession_id", "=", sessionID, string.Empty, "ConString");
                if (dr == null)
                { result = "false"; }
                else { result = dr["usr_name"].ToString() + " " + dr["usr_surname"].ToString() + "," + dr["usr_user_id"].ToString(); }
                return result;
            }
            catch (Exception)
            {
                result = "";
                return result;
            }
        }
        public string mailContent(string buttonsandmessage, string browserLink, string type)
        {
            string head = "", body = "", bodytext = "", end = "";

            head = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
"<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
"<head>" +
    "<meta content=\"text/html; charset=iso-8859-9\" http-equiv=\"Content-Type\" />" +
    "<meta content=\"width=device-width, initial-scale=1, maximum-scale=0\" name=\"viewport\" />" +
    "<!-- NAME: 1:3 COLUMN - BANDED --><!--[if gte mso 15]>" +
    "<xml>" +
        "<o:OfficeDocumentSettings>" +
            "<o:AllowPNG/>" +
            "<o:PixelsPerInch>96</o:PixelsPerInch>" +
        "</o:OfficeDocumentSettings>" +
    "</xml>" +
    "<![endif]-->" +
    "<meta charset=\"UTF-8\" />" +
    "<meta content=\"IE=edge\" http-equiv=\"X-UA-Compatible\" />" +
    "<meta content=\"width=device-width, initial-scale=1\" name=\"viewport\" />" +
    "<title></title>" +
    "<style type=\"text/css\">p{margin:10px 0;padding:0;}" +
    "table{border-collapse:collapse;}" +
    "h1,h2,h3,h4,h5,h6{display:block;margin:0;padding:0;}" +
    "img,a img{border:0;height:auto;outline:none;text-decoration:none;}" +
    "body,#bodyTable,#bodyCell{height:100%;margin:0;padding:0;width:100%;}" +
    "#outlook a{padding:0;}" +
    "img{-ms-interpolation-mode:bicubic;}" +
    "table{mso-table-lspace:0pt;mso-table-rspace:0pt;}" +
    "@media only screen and (max-width: 480px) {}" +
    "@media only screen and (max-width: 480px){" +
        ".mc-rss-item-img{max-width: 100% !important;}" +
    "}" +
    "@media only screen and (max-device-width: 480px){" +
        ".columnWrapper{max-width:50% !important;width:50% !important;}" +
    "}" +
    "@media only screen and (max-device-width: 480px){" +
        ".mcnImage{width:100% !important;}" +
    "}" +
    "@media only screen and (max-device-width: 480px){" +
        "div.wrapper{max-width: 100% !important;min-width: 100% !important;padding: 5px 0;}" +
    "}" +
    "</style>" +
"</head>";

            bodytext = "<body style=\"height: 100%;margin: 0;padding: 0;width: 100%;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;background-color: #000000;font-family: 'Arial', sans-serif !important;border: none;\" yahoo=\"fix\">";

            body = "<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"100%\" style=\"border: none;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;background-color: #000000;\" width=\"100%\">" +
"<tbody>" +
    "<tr><!--üst bölüm-->" +
        "<td align=\"center\"><!--[if gte mso 9]>" +
            "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\" style=\"width:690px;\">" +
                "<tr>" +
                    "<td align=\"center\" valign=\"top\" width=\"690\" style=\"width:690px;\">" +
            "<![endif]-->" +
            "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 690px !important; background-color: #000;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;\" width=\"100%\">" +
                "<tbody>" +
                "<tr>" +
                    "<td align=\"left\" valign=\"top\">" +
                        "<table style=\"width: 100%;\" width=\"100%\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td align=\"left\"><a href=\"http://www.mercedes-benz.com.tr/\"><img alt=\"Mercedes-Benz\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/mbt-star.png\" width=\"117\" /> </a></td>" +
                                "<td align=\"right\"><a href=\"http://www.mercedes-benz.com.tr/\"><img alt=\"Mercedes-Benz\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/mbt-woodmark-claim.png\" width=\"146\" /> </a></td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                    "</td>" +
                "</tr>" +
                "</tbody>" +
            "</table>" +
            "<!--[if gte mso 9]>" +
            "</td>" +
            "</tr>" +
            "</table>" +
            "<![endif]--></td>" +
    "</tr>" +
    "<tr><!--image bölümü-->" +
        "<td align=\"center\"><!--[if gte mso 9]>" +
            "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\" style=\"width:690px;\">" +
                "<tr>" +
                    "<td align=\"center\" valign=\"top\" width=\"690\" style=\"width:690px;\">" +
            "<![endif]-->" +
            "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 690px !important; background-color: #000;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;\" width=\"100%\">" +
                "<tbody>" +
                "<tr>" +
                    "<td align=\"left\" valign=\"top\">" +
                        "<table style=\"width: 100%;\" width=\"100%\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td align=\"center\"><a href=\"#\"><img alt=\"Mercedes-Benz\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/mbt-image.jpg\" style=\"width: 100%; max-width: 690px;\" width=\"auto\" /> </a></td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                    "</td>" +
                "</tr>" +
                "</tbody>" +
            "</table>" +
            "<!--[if gte mso 9]>" +
            "</td>" +
            "</tr>" +
            "</table>" +
            "<![endif]--></td>" +
    "</tr>" +
    "<tr>" +
        "<td style=\"height: 10px;\">&nbsp;</td>" +
    "</tr>" +
    "<tr><!--yazı bölümü-->" +
        "<td align=\"center\"><!--[if gte mso 9]>" +
            "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\" style=\"width:690px;\">" +
                "<tr>" +
                    "<td align=\"center\" valign=\"top\" width=\"690\" style=\"width:690px;\">" +
            "<![endif]-->" +
            "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 690px !important; background-color: #000;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;\" width=\"100%\">" +
                "<tbody>" +
                "<tr>" +
                    "<td style=\"width: 50px !important;\" width=\"50\">&nbsp;</td>" +
                    "<td align=\"left\" style=\"height: 20px !important;\" valign=\"top\">&nbsp;</td>" +
                "</tr>" +
                "<tr>" +
                    "<td style=\"width: 50px !important;\" width=\"50\">&nbsp;</td>" +
                    "<td align=\"left\" valign=\"top\"><img alt=\"\" class=\"mcnImage\" height=\"4\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Line.jpg\" style=\"max-width: 56px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" width=\"56\" /></td>" +
                "</tr>" +
                "</tbody>" +
            "</table>" +
            "<!--[if gte mso 9]>" +
            "</td>" +
            "</tr>" +
            "</table>" +
            "<![endif]--></td>" +
    "</tr>" +
    "<tr><!--yazı bölümü-->" +
        "<td align=\"center\"><!--[if gte mso 9]>" +
            "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\" style=\"width:690px;\">" +
                "<tr>" +
                    "<td align=\"center\" valign=\"top\" width=\"690\" style=\"width:690px;\">" +
            "<![endif]-->" +
            "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 690px !important; background-color: #000;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;\" width=\"100%\">" +
                "<tbody>" +
                "<tr>" +
                    "<td align=\"left\" valign=\"top\">" +
                        "<table style=\"width: 100%;\" width=\"100%\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td style=\"width: 50px !important;\" width=\"50\">&nbsp;</td>" +
                                "<td align=\"left\">" +
                                    "<!--<p style=\"font-size:25px; color: #ffffff;display: block; width: 100% !important;font-family: Arial;\">Bu hafta sonu Efsane ile test sürüşüne hazır mısınız?</p>-->" +
                                    "<!--karşılama metni-->" +
                                   // İÇERİK ve BUTONLAR
                                   buttonsandmessage +
                                "</td>" +
                                "<td style=\"width: 20px !important;\" width=\"20\">&nbsp;</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                    "</td>" +
                "</tr>" +
                "</tbody>" +
            "</table>" +
            "<!--[if gte mso 9]>" +
            "</td>" +
            "</tr>" +
            "</table>" +
            "<![endif]--></td>" +
    "</tr>" +
    "<tr>" +
        "<td align=\"center\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;border-top: 0;border-bottom:0;max-width: 690px !important;\" valign=\"top\">" +
            "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color: #000;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;max-width: 690px !important;\" width=\"100%\">" +
                "<tbody>" +
                "<tr>" +
                    "<td style=\"width: 50px !important;\" width=\"50\">&nbsp;</td>" +
                    "<td style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;max-width: 620px !important;\" valign=\"top\"><!--[if gte mso 9]>" +
                        "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"620\" style=\"width:620px;\">" +
                            "<tr>" +
                                "<td align=\"left\" valign=\"top\" width=\"103\" style=\"width:103px;\">" +
                        "<![endif]-->" +
                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"columnWrapper\" style=\"max-width: 16%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td class=\"columnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBlock\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\">" +
                                        "<tbody class=\"mcnCaptionBlockOuter\">" +
                                        "<tr>" +
                                            "<td class=\"mcnCaptionBlockInner\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBottomContent\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"false\">" +
                                                    "<tbody>" +
                                                    "<tr>" +
                                                        "<td align=\"left\" class=\"mcnCaptionBottomImageContent\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\"><a href=\"http://www.mercedes-benz.com.tr/content/turkey/mpc/mpc_turkey_website/tr/home_mpc/passengercars/home/new_cars/model_overview_configurator.html\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Configurator_1_Configurator.png\" style=\"width:100%;max-width: 92px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                        "<!--[if gte mso 9]>" +
                        "</td>" +
                        "<td align=\"left\" valign=\"top\" width=\"103\" style=\"width:103px;\">" +
                        "<![endif]-->" +
                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"columnWrapper\" style=\"max-width: 16%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td class=\"columnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBlock\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\">" +
                                        "<tbody class=\"mcnCaptionBlockOuter\">" +
                                        "<tr>" +
                                            "<td class=\"mcnCaptionBlockInner\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBottomContent\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"false\">" +
                                                    "<tbody>" +
                                                    "<tr>" +
                                                        "<td align=\"left\" class=\"mcnCaptionBottomImageContent\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\"><a href=\"http://donanim.mercedes-benz.com.tr\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Configurator_2_ModelOverview.png\" style=\"width:100%;max-width: 92px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                        "<!--[if gte mso 9]>" +
                        "</td>" +
                        "<td align=\"left\" valign=\"top\" width=\"103\" style=\"width:103px;\">" +
                        "<![endif]-->" +
                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"columnWrapper\" style=\"max-width: 16%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td class=\"columnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBlock\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\">" +
                                        "<tbody class=\"mcnCaptionBlockOuter\">" +
                                        "<tr>" +
                                            "<td class=\"mcnCaptionBlockInner\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBottomContent\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"false\">" +
                                                    "<tbody>" +
                                                    "<tr>" +
                                                        "<td align=\"left\" class=\"mcnCaptionBottomImageContent\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\"><a href=\"https://e-services.mercedes-benz.com/Dialog_RQBY/RQBY;jsessionid=0000gkTD0ibQxvlCdn7lPCRQD7_:186sb5v94?subprocess=RQBcy&locale=tr_TR&site_locale=tr_TR&prev_sub=SRQc\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Configurator_3_OrderBrochures.jpg\" style=\"width:100%;max-width: 92px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                        "<!--[if gte mso 9]>" +
                        "</td>" +
                        "<td align=\"left\" valign=\"top\" width=\"103\" style=\"width:103px;\">" +
                        "<![endif]-->" +

                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"columnWrapper\" style=\"max-width: 16%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td class=\"columnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBlock\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\">" +
                                        "<tbody class=\"mcnCaptionBlockOuter\">" +
                                        "<tr>" +
                                            "<td class=\"mcnCaptionBlockInner\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBottomContent\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"false\">" +
                                                    "<tbody>" +
                                                    "<tr>" +
                                                        "<td align=\"left\" class=\"mcnCaptionBottomImageContent\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\"><a href=\"http://crm.mercedes-benz.com.tr/formserver/Source/Web417\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Configurator_4_TestDrive.png\" style=\"width:100%;max-width: 92px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                        "<!--[if gte mso 9]>" +
                        "</td>" +
                        "<td align=\"left\" valign=\"top\" width=\"103\" style=\"width:103px;\">" +
                        "<![endif]-->" +

                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"columnWrapper\" style=\"max-width: 16%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td class=\"columnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBlock\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\">" +
                                        "<tbody class=\"mcnCaptionBlockOuter\">" +
                                        "<tr>" +
                                            "<td class=\"mcnCaptionBlockInner\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBottomContent\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"false\">" +
                                                    "<tbody>" +
                                                    "<tr>" +
                                                        "<td align=\"left\" class=\"mcnCaptionBottomImageContent\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\"><a href=\"http://www.mercedes-benz.com.tr/content/turkey/mpc/mpc_turkey_website/tr/home_mpc/passengercars/home/new_cars/bayi.html#_int_passengercars:home:core-navi:bayi\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Configurator_5_Retailersearch.png\" style=\"width:100%;max-width: 92px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                        "<!--[if gte mso 9]>" +
                        "</td>" +
                        "<td align=\"left\" valign=\"top\" width=\"103\" style=\"width:103px;\">" +
                        "<![endif]-->" +

                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"columnWrapper\" style=\"max-width: 16%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">" +
                            "<tbody>" +
                            "<tr>" +
                                "<td class=\"columnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                    "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBlock\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\">" +
                                        "<tbody class=\"mcnCaptionBlockOuter\">" +
                                        "<tr>" +
                                            "<td class=\"mcnCaptionBlockInner\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\">" +
                                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnCaptionBottomContent\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"false\">" +
                                                    "<tbody>" +
                                                    "<tr>" +
                                                        "<td align=\"left\" class=\"mcnCaptionBottomImageContent\" style=\"padding: 0;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" valign=\"top\"><a href=\"tel:4446244\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Configurator_6_CallCenter.png\" style=\"width:100%;max-width: 92px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                                    "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</tbody>" +
                        "</table>" +
                        "<!--[if gte mso 9]>" +
                        "</td>" +
                        "</tr>" +
                        "</table>" +
                        "<![endif]--></td>" +
                    "<td style=\"width: 20px !important;\" width=\"20\">&nbsp;</td>" +
                "</tr>" +
                "</tbody>" +
            "</table>" +
        "</td>" +
    "</tr>" +
        /* "<tr><!--benim mercedesim  bölümü-->" + 
             "<td align=\"center\"><!--[if gte mso 9]>" +
                 "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\" style=\"width:690px;\">" +
                     "<tr>" +
                         "<td align=\"center\" valign=\"top\" width=\"690\" style=\"width:690px;\">" +
                 "<![endif]-->" +
                 "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 690px !important; background-color: #1f1f1f;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;\" width=\"100%\">" +
                     "<tbody>" +
                     "<tr>" +
                         "<td align=\"left\" valign=\"top\">" +
                             "<table style=\"width: 100%;\" width=\"100%\">" +
                                 "<tbody>" +
                                 "<tr>" +
                                     "<td colspan=\"3\" style=\"height: 20px;\">&nbsp;</td>" +
                                 "</tr>" +
                                 "<tr>" +
                                     "<td style=\"width: 50px !important;\" width=\"50\">&nbsp;</td>" +
                                     "<td align=\"left\"><a href=\"http://www.benimmercedesim.com/\"><img alt=\"Mercedes-Benz\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/benimmercedesim-Logo.png\" style=\"display: block;\" /></a></td>" +
                                     "<td style=\"width: 20px !important;\" width=\"20\">&nbsp;</td>" +
                                 "</tr>" +
                                 "<tr>" +
                                     "<td colspan=\"3\" style=\"height: 20px;\">&nbsp;</td>" +
                                 "</tr>" +
                                 "</tbody>" +
                             "</table>" +
                         "</td>" +
                     "</tr>" +
                     "</tbody>" +
                 "</table>" +
                 "<!--[if gte mso 9]>" +
                 "</td>" +
                 "</tr>" +
                 "</table>" +
                 "<![endif]--></td>" +
         "</tr>" +
         "<!--benim mercedesim image bölümü-->" +
         "<tr>" +
             "<td align=\"center\"><!--[if gte mso 9]>" +
                 "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\" style=\"width:690px;\">" +
                     "<tr>" +
                         "<td align=\"center\" valign=\"top\" width=\"690\" style=\"width:690px;\">" +
                 "<![endif]-->" +
                 "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 690px !important; background-color: #000000;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;\" width=\"100%\">" +
                     "<tbody>" +
                     "<tr>" +
                         "<td align=\"left\" valign=\"top\">" +
                             "<table style=\"width: 100%;\" width=\"100%\">" +
                                 "<tbody>" +
                                 "<tr>" +
                                     "<td align=\"left\"><a href=\"http://www.benimmercedesim.com/\"><img alt=\"Mercedes-Benz\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/11_Connectme_Basis_579_01.jpg\" style=\"width: 100%; max-width: 690px;\" width=\"auto\" /></a></td>" +
                                 "</tr>" +
                                 "</tbody>" +
                             "</table>" +
                         "</td>" +
                     "</tr>" +
                     "</tbody>" +
                 "</table>" +
                 "<!--[if gte mso 9]>" +
                 "</td>" +
                 "</tr>" +
                 "</table>" +
                 "<![endif]--></td>" +
             "</tr>" +
         "<tr>" +
             "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                 "<!--[if (gte mso 9)|(IE)]>" +
                 "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                     "<tr>" +
                         "<td align=\"left\" valign=\"top\" width=\"690\">" +
                 "<![endif]-->" +
                 "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#1f1f1f\">" +
                     "<tbody>" +
                     "<tr>" +
                         "<td style=\"width: 50px !important;\" width=\"50\">&nbsp;</td>" +
                         "<td align=\"left\">" +
                             "<img src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Line.png\" width=\"56\" height=\"49\" alt=\"Mercedes-Benz\" style=\"display: block;\">" +
                         "</td>" +
                         "<td style=\"width: 20px !important;\" width=\"20\">&nbsp;</td>" +
                     "</tr>" +
                     "<tr>" +
                         "<td style=\"width: 50px !important;\" width=\"10\">&nbsp;</td>" +
                         "<td>" +
                             "<!--[if (gte mso 9)|(IE)]>" +
                             "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"620\">" +
                                 "<tr>" +
                                     "<td align=\"left\" valign=\"top\" width=\"620\">" +
                             "<![endif]-->" +

                             "<table style=\"max-width: 620px;\" width=\"100%\" bgcolor=\"#1f1f1f\">" +
                                 "<tbody>" +
                                 "<tr>" +
                                     "<td align=\"left\">" +
                                         "<!--[if (gte mso 9)|(IE)]>" +
                                         "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">" +
                                             "<tr>" +
                                                 "<td align=\"left\" valign=\"top\" width=\"310\">" +
                                         "<![endif]-->" +
                                         "<div class=\"wrapper\" style=\"float:left;display:inline-block; margin: 0 ; max-width:50%; vertical-align:top; width:100%;\">" +
                                             "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"wrapper\" style=\"max-width:310px;\" width=\"100%\">" +
                                                 "<tbody>" +
                                                 "<tr>" +
                                                     "<td align=\"center\" valign=\"top\">" +
                                                         "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                                             "<tbody>" +
                                                             "<tr>" +
                                                                 "<td style=\"padding: 0;\">" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                                                         "<tbody>" +
                                                                         "<tr>" +
                                                                             "<td align=\"left\" bgcolor=\"#1f1f1f\" valign=\"top\">" +
                                                                                 "<p style=\"padding:5px 0 !important;margin:0 !important;font-size:25px; color: #ffffff;display: block; width: 100% !important;font-family: Arial;\">\"Benim bir Mercedes'im var!\" diyorsanız,size özel bir platformunuz var.</p>" +
                                                                             "</td>" +
                                                                         "</tr>" +
                                                                         "</tbody>" +
                                                                     "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                             "</tbody>" +
                                                         "</table>" +
                                                     "</td>" +
                                                 "</tr>" +
                                                 "</tbody>" +
                                             "</table>" +
                                         "</div>" +
                                         "<!--[if (gte mso 9)|(IE)]>" +
                                         "</td>" +
                                         "<td align=\"left\" valign=\"top\" width=\"310\">" +
                                         "<![endif]-->" +
                                         "<div class=\"wrapper\" style=\"float:left;display:inline-block; margin: 0; max-width:50%; vertical-align:top; width:100%;\">" +
                                             "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                                 "<tbody>" +
                                                 "<tr>" +
                                                     "<td align=\"left\">" +
                                                         "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"wrapper\" style=\"max-width:310px; float: left;\" width=\"100%\">" +
                                                             "<tbody>" +
                                                             "<tr>" +
                                                                 "<td align=\"left\" valign=\"top\"></td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td align=\"left\" valign=\"top\">" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                                                         "<tbody>" +
                                                                         "<tr>" +
                                                                             "<td style=\"padding: 0;\">" +
                                                                                 "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                                                                     "<tbody>" +
                                                                                     "<tr>" +
                                                                                         "<td align=\"left\" bgcolor=\"#1f1f1f\" valign=\"top\">" +
                                                                                             "<p style=\"padding:5px 0 !important;margin:0;font-size:14px; color: #ffffff;display: block; width: 100% !important;font-family: Arial;\">Bu platform sayesinde Mercedes-Benz sizi daha yakından tanıyor. Sizinle, size özel bir bağ kuruyor. '<a href=\"http://www.benimmercedesim.com/\" style=\"color: #ffffff;text-decoration: underline;\">benimmercedesim.com</a>'a kayıt olun, bilgilerinizi güncelleyin. Siz de güncellenin.<br /></p>" +
                                                                                             "<a href=\"http://www.benimmercedesim.com/\"><img alt=\"\" class=\"mcnImage\" height=\"71\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Duyuru-Mailibg-PC_20.png\" style=\"max-width: 115px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" width=\"115\" /> </a>" +
                                                                                         "</td>" +
                                                                                     "</tr>" +
                                                                                     "</tbody>" +
                                                                                 "</table>" +
                                                                             "</td>" +
                                                                         "</tr>" +
                                                                         "</tbody>" +
                                                                     "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                             "</tbody>" +
                                                         "</table>" +
                                                     "</td>" +
                                                 "</tr>" +
                                                 "</tbody>" +
                                             "</table>" +
                                         "</div>" +

                                         "<!--[if (gte mso 9)|(IE)]>" +
                                         "</td>" +
                                         "</tr>" +
                                         "</table>" +
                                         "<![endif]-->" +
                                     "</td>" +
                                 "</tr>" +
                                 "</tbody>" +
                             "</table>" +
                             "<!--[if (gte mso 9)|(IE)]>" +
                             "</td>" +
                             "</tr>" +
                             "</table>" +
                             "<![endif]-->" +
                         "</td>" +
                         "<td style=\"width: 20px !important;\" width=\"10\">&nbsp;</td>" +
                     "</tr>" +
                     "</tbody>" +
                 "</table>" +
                 "<!--[if (gte mso 9)|(IE)]>" +
                 "</td>" +
                 "</tr>" +
                 "</table>" +
                 "<![endif]-->" +
             "</td>" +
         "</tr>" + */

        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#000000\">" +
                    "<tbody>" +
                        "<tr>" +
                            "<td style=\"width: 50px !important;\" width=\"10\">&nbsp;</td>" +
                            "<td align=\"left\" height=\"15\" style=\"min-height: 15px;\"></td>" +
                            "<td style=\"width: 20px !important;\" width=\"10\">&nbsp;</td>" +
                        "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                     "<![endif]-->" +
                        "<table width=\"100%\" style=\"max-width: 690px;\" align=\"center\" bgcolor=\"#000000\">" +
                            "<tbody>" +
                                "<tr>" +
                                    "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                                    "<td align=\"left\"><p style=\"padding:0;margin:0;font-size:18px; color: #ffffff;display: block; width: 100% !important;font-family: Arial;\">Mercedes'i Takip Edin.</p></td>" +
                                    "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                                "</tr>" +
                            "</tbody>" +
                        "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\" bgcolor=\"#000000\">" +
                    "<tbody>" +
                        "<tr>" +
                            "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                            "<td align=\"left\">" +
                                "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:620px;\">" +
                                    "<tbody>" +
                                    "<tr>" +
                                        "<td align=\"left\" valign=\"top\"><a href=\"https://www.facebook.com/mercedesbenztr\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" height=\"43\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Duyuru-Mailibg-PC_25.jpg\" style=\"max-width: 60px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                        "<td align=\"left\" valign=\"top\"><a href=\"https://twitter.com/mercedesturkiye\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" height=\"43\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Duyuru-Mailibg-PC_26.jpg\" style=\"max-width: 60px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                        "<td align=\"left\" valign=\"top\"><a href=\"https://www.youtube.com/user/mbtrvideo\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" height=\"43\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Duyuru-Mailibg-PC_27.jpg\" style=\"max-width: 60px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                        "<td align=\"left\" valign=\"top\"><a href=\"https://instagram.com/mercedesbenzturkiye\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" height=\"43\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Duyuru-Mailibg-PC_28.jpg\" style=\"max-width: 60px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                        "<td align=\"left\" style=\"width:103px;\" valign=\"top\" width=\"103\"><a href=\"http://blog.mercedes-benz.com.tr/\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" target=\"_blank\" title=\"\"><img alt=\"\" class=\"mcnImage\" height=\"43\" src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Duyuru-Mailibg-PC_29.jpg\" style=\"max-width: 60px; border: 0px; height: auto; outline: none; text-decoration: none; vertical-align: bottom;\" /> </a></td>" +
                                    "</tr>" +
                                    "</tbody>" +
                                "</table>" +
                            "</td>" +
                            "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#000000\">" +
                    "<tbody>" +
                    "<tr>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "<td align=\"left\">" +
                            "<img src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Line_long2.png\" alt=\"Mercedes-Benz\" style=\"width: 100%; height: 31px;\"/>" +
                        "</td>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#000000\">" +
                    "<tbody>" +
                    "<tr>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "<td align=\"left\">" +
                            // browserLinki
                            browserLink +
                            "<p style=\"padding:0 !important;margin:0 !important;font-size:12px; color: #bdbdbd;display: block; width: 100% !important;font-family: Arial;\">---Bu mesaj bilgilendirme amaçlı gönderilmiştir, lütfen cevap vermeyiniz.---</p>" +
                        "</td>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#000000\">" +
                    "<tbody>" +
                    "<tr>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "<td align=\"left\">" +
                            "<img src=\"http://cdn.mercedes-benz.com.tr/files/mail-images/Line_long2.png\" alt=\"Mercedes-Benz\" style=\"width: 100%; height: 31px;\"/>" +
                        "</td>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#000000\">" +
                    "<tbody>" +
                    "<tr>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "<td align=\"left\">" +
                            "<p style=\"padding:0 !important;margin:0 !important;font-size:12px; color: #bdbdbd;display: block; width: 100% !important;font-family: Arial;\">Telif hakkı Mercedes-Benz Türk'e aittir. Mercedes-Benz Türk bütün haklarını saklı tutar. Mercedes-Benz Türk'e ait metinler, resimler, çizimler, ses, animasyon, video dosyaları bu telif hakkına ve diğer fikir haklarına tabidir. Bu düzenlemeler ticari kullanım amacıyla kopya edilemez, değiştirilemez, dağıtılamaz veya diğer sitelere gönderilemez. Bazı Mercedes-Benz Türk internet sayfaları telif hakkı üçüncü kişilere ait olan materyalleri içermektedir.<br />" +
                                "<br />" +
                                "Mercedes-Benz Türk A.Ş. Telefon <a href=\"tel:02128673000\" style=\"color: #ffffff;\">0212 8673000</a> Akçaburgaz Mah. Süleyman Şah Cad. No:6 Esenyurt 34522 İstanbul Mersis No 0616004407900018</p>" +
                        "</td>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\"  bgcolor=\"#000000\">" +
                    "<tbody>" +
                    "<tr>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "<td align=\"left\" height=\"15\" style=\"min-height: 15px;\"></td>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\" bgcolor=\"#000000\">" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "<table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"690\">" +
                    "<tr>" +
                        "<td align=\"left\" valign=\"top\" width=\"690\">" +
                "<![endif]-->" +
                "<table width=\"100%\" style=\"max-width: 690px;\" bgcolor=\"#1f1f1f\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">" +
                    "<tbody>" +
                    "<tr>" +
                        "<td colspan=\"3\" style=\"height: 10px !important;\" height=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                        "<td align=\"left\" style=\"max-width: 670px;\"  bgcolor=\"#1f1f1f\">" +
                            "<span style=\"float:left; width:auto;padding:0;margin:0 15px 0 0;font-size:12px; color: #666666;display: block; font-family: Arial;\">© 2016 <a href=\"http://www.mercedes-benz.com.tr/\" style=\"color:#666666;text-decoration:none;\">Mercedes-Benz</a></span>&nbsp;" +
                            "<span style=\"float:left; width:auto;padding:0;margin:0 10px 0 0;font-size:12px; color: #666666;display: block; font-family: Arial;\"><a href=\"http://www.mercedes-benz.com.tr/content/turkey/mpc/mpc_turkey_website/tr/home_mpc/passengercars/home/new_cars/bayi.html#_int_passengercars:home:core-navi:bayi\" style=\"color:#666666;text-decoration:none;\">Bayiler</a></span>&nbsp;" +
                            "<span style=\"float:left; padding:0;margin:0;font-size:12px; color: #666666;display: block; font-family: Arial;\"><a href=\"https://e-services.mercedes-benz.com/Dialog_SRQ/SRQ;jsessionid=0000JGHHWqmMX-qgldRxnQHhQkt:186sb5v94?subprocess=SRQc&locale=tr_TR&site_locale=tr_TR\" style=\"color:#666666;text-decoration:none;\">Bize Ulaşın</a></span>&nbsp;" +
                        "</td>" +
                        "<td style=\"width: 10px !important;\" width=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan=\"3\" style=\"height: 10px !important;\" height=\"10\">&nbsp;</td>" +
                    "</tr>" +
                    "</tbody>" +
                "</table>" +
                "<!--[if (gte mso 9)|(IE)]>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "<![endif]-->" +
            "</td>" +
        "</tr>" +
    "</tbody></table>";

            end = "</body></html>";
            if (type == "mail")
            {
                return head + bodytext + body + end;
            }
            else
            {
                return body;
            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public Image RezizeImage(Image img, int maxWidth, int maxHeight)
        {
            using (img)
            {
                Double xRatio = (double)img.Width / maxWidth;
                Double yRatio = (double)img.Height / maxHeight;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(img.Width / ratio);
                int nny = (int)Math.Floor(img.Height / ratio);
                Bitmap cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);

                    // This is said to give best quality when resizing images
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                return cpy;
            }
        }
    }
    public class helper
    {
        public string Left(string str, int howmany)
        {
            try
            {
                return str.Substring(0, howmany) + "...";
            }
            catch (Exception)
            {
                return str;
            }
        }
        public string SeoURL(string adress)
        {
            string value = string.Empty;

            value = adress.ToLower();
            value = value.Replace("ı", "i");
            value = value.Replace("ö", "o");
            value = value.Replace("ü", "u");
            value = value.Replace("ş", "s");
            value = value.Replace("ç", "c");
            value = value.Replace("ğ", "g");
            value = value.Replace("İ", "i");
            value = value.Replace("Ö", "o");
            value = value.Replace("Ü", "u");
            value = value.Replace("Ş", "s");
            value = value.Replace("Ç", "c");
            value = value.Replace("Ğ", "g");
            //value = value.Replace("!", "");
            //value = value.Replace("'", "");
            //value = value.Replace("£", "");
            //value = value.Replace("^", "");
            //value = value.Replace("#", "");
            //value = value.Replace("+", "");
            //value = value.Replace("$", "");
            //value = value.Replace("%", "");
            //value = value.Replace("½", "");
            //value = value.Replace("&", "");
            //value = value.Replace("/", "");
            //value = value.Replace("{", "");
            //value = value.Replace("(", "");
            //value = value.Replace("[", "");
            //value = value.Replace(")", "");
            //value = value.Replace("]", "");
            //value = value.Replace("=", "");
            //value = value.Replace("}", "");
            //value = value.Replace("?", "");
            ////value = value.Replace("\", "");

            //value = value.Replace("_", "");
            //value = value.Replace(" ", "-");
            //value = value.Replace("|", "");
            //value = value.Replace(";", "");
            //value = value.Replace("`", "");
            //value = value.Replace("<", "");
            //value = value.Replace(">", "");
            //value = value.Replace("@", "");
            //value = value.Replace(".", "");
            //value = value.Replace(":", "");
            //value = value.Replace("*", "");

            return value.ToLower();
        }
        public string ClearBadCharacters(string strText)
        {
            string illegal = HttpUtility.HtmlEncode(strText);
            string invalid = new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                illegal = illegal.Replace(c.ToString(), string.Empty);
            }
            return illegal;
        }

        public const int ImageMinimumBytes = 512;
        public bool IsImage(HttpPostedFile postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.InputStream.Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {

                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }

            return true;
        }
    }
    public class media
    {
        dbLibrary db = new dbLibrary();
        public DataTable getAllWithPaging(int status, int min, int max)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getAllWithPaging", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@min", min);
                cmd.Parameters.AddWithValue("@max", max);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable postFile(string userId, string fileExtension, string ip, string path)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();
            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_postFile", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@fileExtension", fileExtension);
                cmd.Parameters.AddWithValue("@ip", ip);
                cmd.Parameters.AddWithValue("@path", path);
                sDa.SelectCommand = cmd;
                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }
            return dT;
        }
        public DataTable postDeleteFile(int uploadFileId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();
            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_postDeleteFile", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@uploadFileId", uploadFileId);
                sDa.SelectCommand = cmd;
                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }
            return dT;
        }
    }
    public class personel
    {
        dbLibrary db = new dbLibrary();
        public DataTable getpersonnels(string userId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getpersonnels", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userId", userId);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getpersonnelKendisi(string userId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getpersonnelKendisi", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userId", userId);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getpersonnelsAll()
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getpersonnelsAll", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getgonderilenMasrafKendisi(string userId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getgonderilenMasrafKendisi", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userId", userId);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getgonderilmeyenMasrafAdet(string userId, string personelId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getgonderilmeyenMasrafAdet", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@personelId", personelId);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
    }
    public class masraf
    {
        dbLibrary db = new dbLibrary();
        public DataTable getMasraf()
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getMasraf", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getMasrafByUid(string userId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getMasrafByUid", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userId", userId);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getMasrafByName(string masrafName)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getMasrafByName", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@masrafName", masrafName);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
        public DataTable getMasrafGrup(int masrafFormuGrupId)
        {
            SqlCommand cmd = null;
            SqlConnection cnn = null;
            DataTable dT = new DataTable();
            SqlDataAdapter sDa = new SqlDataAdapter();

            try
            {
                cnn = new SqlConnection(db.dbStr());

                cnn.Open();

                cmd = new SqlCommand("sp_getMasrafGrup", cnn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@masrafFormuGrupId", masrafFormuGrupId);
                sDa.SelectCommand = cmd;

                sDa.Fill(dT);
            }
            catch (Exception)
            {
                dT = null;
            }
            finally
            {
                cnn.Close();
                cnn.Dispose();
            }

            return dT;
        }
    }
}