using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace QLCuaHangThuCung_BTL
{
    public class DBConnect
    {
        //private string connectionString = @"Data Source=msitoiyeu;Initial Catalog=QLCuaHangThuCung1;Integrated Security=True;Trust Server Certificate=True";
        private readonly string connectionString = @"Data Source=DESKTOP-M1JOGNG\SQLEXPRESS;Initial Catalog=QLCuaHangThuCung1;Integrated Security=True";

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


        // Thực hiện lệnh INSERT, UPDATE, DELETE
        public void Execute(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
