using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSmart
{
    public class ConnectFactory
    {
        private SQLiteConnection conn;
        private SQLiteCommand cmd;
        private SQLiteDataAdapter db;
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        private string strConn = "Data Source = autopro.db; Version =3;New=False;Compress = True;";
        /// <summary>
        /// Thực hiện các câu truy vấn insert, update, delete vào trong db
        /// </summary>
        /// <param name="query">Truy vấn</param>
        public void ExecuteNonQuery(string query)
        {
            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Thực hiện truy vấn trả về các bản ghi
        /// </summary>
        /// <param name="query">Câu truy vấn</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteQuery(string query)
        {
            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                cmd = conn.CreateCommand();
                db = new SQLiteDataAdapter(query, conn);
                ds.Reset();
                db.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
        }
        /// <summary>
        /// Lấy tổng số lượng bản ghi
        /// </summary>
        /// <param name="query">Truy vấn</param>
        /// <returns>Tổng số lượng</returns>
        public int GetTotalRecord(string query)
        {
            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandText = query;
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }
    }
}
