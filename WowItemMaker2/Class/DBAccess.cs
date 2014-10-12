using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace WowItemMaker2
{
    public class DBAccess
    {
        private MySqlConnection conn;
        private MySqlTransaction tran;
        private string _connStr;
        private string charSet;

        public string ConnStr
        {
            get { return _connStr; }
            set { _connStr = value; }
        }

        public DBAccess()
        {
            this._connStr = string.Empty;
            this.conn = new MySqlConnection();
        }

        public DBAccess(string host, string user, string pwd, string db, string charSet, uint port)
        {
            this._connStr = "Database=" + db + ";Data Source=" + host + ";port=" + port + ";User Id=" + user + ";Password=" + pwd + ";CharSet=" + charSet;
            MySqlConnectionStringBuilder b = new MySqlConnectionStringBuilder();
            if (charSet != null)
                //this.charSet = charSet;
                b.CharacterSet = charSet;
            if(db != null)
                b.Database = db;
            b.Password = pwd;
            b.Port = port;
            b.Server = host;
            b.UserID = user;
            this.conn = new MySqlConnection(b.ToString());
            //this.conn = new MySqlConnection(this._connStr);
        }

        public void open()
        {
            if (conn.State == ConnectionState.Broken)
                conn.Close();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            if (this.charSet != null && this.charSet.Trim() != string.Empty)
            {
                MySqlCommand cmd = new MySqlCommand("set names '" + this.charSet + "';", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        public void close()
        {
            if (conn != null)
                conn.Close();
        }

        public int execute(string sql)
        {
            int res = -1;
            if (this.conn == null)
                this.conn = new MySqlConnection(this._connStr);
            //MySqlConnection conn = new MySqlConnection(this._connStr);
            //Encoding encoding = Encoding.UTF8;
            //if (encoding != null)
            //{
            //    byte[] sqlByte = Encoding.Default.GetBytes(sql);
            //    sql = encoding.GetString(sqlByte);
            //}
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            try
            {
                open();
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //conn.Close();
            }
            return res;
        }
        
        public object executeScalar(string sql)
        {
            object obj = null;
            if (this.conn == null)
                this.conn = new MySqlConnection(this._connStr);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            try
            {
                open();
                obj = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //conn.Close();
            }
            return obj;
        }

        public DataTable query(string sql)
        {
            DataTable res = null;
            if (this.conn == null)
                this.conn = new MySqlConnection(this._connStr);
            //MySqlConnection conn = new MySqlConnection(this._connStr);
            MySqlDataAdapter adp = new MySqlDataAdapter(sql, conn);
            try
            {
                DataSet ds = new DataSet();
                open();
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                    res = ds.Tables[0];
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //conn.Close();
            }
            return res;
        }

        public void beginTransaction()
        {
            open();
            this.tran = this.conn.BeginTransaction();
        }

        public void cancleTransaction()
        {
            if (this.tran != null)
            {
                this.tran.Rollback();
                this.conn.Close();
            }
        }

        public void commit()
        {
            if (this.tran != null)
                this.tran.Commit();
        }

        public void rollback()
        {
            if (this.tran != null)
                this.tran.Rollback();
        }

    }
}
