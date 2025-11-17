using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLCuaHangThuCung_BTL
{
    public class DBConnect
    {
        //private string connectionString = @"Data Source=msitoiyeu;Initial Catalog=QLCuaHangThuCung1;Integrated Security=True;Trust Server Certificate=True";
        private readonly string connectionString = @"Data Source=LAPTOP-UGRNRM8L;Initial Catalog = QLCuaHangThuCung1;Integrated Security = True";

        // Lấy đối tượng SqlConnection
        private SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Lấy dữ liệu (SELECT)
        public DataTable GetData(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }


        // INSERT, UPDATE, DELETE
        public void Execute(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}