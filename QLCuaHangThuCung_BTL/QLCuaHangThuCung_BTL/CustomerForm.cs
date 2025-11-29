using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class CustomerForm : Form
    {
        DBConnect db = new DBConnect();

        public CustomerForm()
        {
            InitializeComponent();
            LoadCustomer();

            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //this.dgvCustomer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomer_CellContentClick);
        }

        private string GetPasswordFromTaiKhoan(string idTaiKhoan)
        {
            string query = "SELECT MatKhau FROM TaiKhoan WHERE IDTaiKhoan = @id";
            DataTable dt = db.GetData(query, new SqlParameter("@id", idTaiKhoan));
            return dt.Rows.Count > 0 ? dt.Rows[0]["MatKhau"].ToString() : "";
        }

        public void LoadCustomer()
        {
            dgvCustomer.Rows.Clear();

            string query = @"
                SELECT IDKhachHang, TenKhachHang, DiaChi, SDT, Email, GioiTinh, IDTaiKhoan
                FROM KhachHang 
                WHERE TenKhachHang LIKE @search OR IDKhachHang LIKE @search 
                ORDER BY IDKhachHang ASC";

            DataTable dt = db.GetData(query,
                new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%")
            );

            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                i++;
                string idTaiKhoan = row["IDTaiKhoan"].ToString();

                dgvCustomer.Rows.Add(
                    i,
                    row["IDKhachHang"].ToString(), // Col 1: Mã người dùng
                    row["TenKhachHang"].ToString(), // Col 2: Họ và tên
                    row["GioiTinh"].ToString(), // Col 3: Giới tính
                    row["DiaChi"].ToString(), // Col 4: Địa chỉ
                    row["SDT"].ToString(), // Col 5: SĐT
                    row["Email"].ToString(), // Col 6: Email
                    "********" // Col 7: Mật khẩu hiển thị ẩn danh
                );
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            CustomerModule module = new CustomerModule(this);
            module.btnSave.Enabled = true;
            module.btnUpdate.Enabled = false;
            module.txtIDKhachHang.Enabled = true;
            module.txtIDKhachHang.Visible = true; // Hiển thị ID để nhập mới
            module.ShowDialog();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvCustomer.Columns[e.ColumnIndex].Name;
            // Lấy Mã khách hàng từ cột thứ 2 (chỉ mục 1)
            string customerId = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
            string idTaiKhoan = customerId;

            // ================= EDIT =================
            if (colName == "Edit")
            {
                string originalPassword = GetPasswordFromTaiKhoan(idTaiKhoan);

                CustomerModule module = new CustomerModule(this)
                {
                    btnSave = { Enabled = false },
                    btnUpdate = { Enabled = true },
                    txtIDKhachHang = { Enabled = false, Visible = true } // Không cho phép sửa Mã KH
                };

                // Đổ dữ liệu vào CustomerModule theo tên cột trong DataGridView
                module.txtIDKhachHang.Text = customerId;
                module.txtHoVaTen.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
                module.txtDiaChi.Text = dgvCustomer.Rows[e.RowIndex].Cells[4].Value.ToString();
                module.txtSDT.Text = dgvCustomer.Rows[e.RowIndex].Cells[5].Value.ToString();
                module.txtEmail.Text = dgvCustomer.Rows[e.RowIndex].Cells[6].Value.ToString();

                module.txtMatKhau.Text = originalPassword;
                module.ShowDialog();
            }

            // ================= DELETE =================
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Xóa khách hàng này? Thao tác này sẽ xóa cả tài khoản liên quan.", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        // Xóa các dữ liệu phụ thuộc (Ví dụ: Giỏ hàng)
                        string deleteGHQuery = "DELETE FROM GioHang WHERE IDKhachHang = @id";
                        db.Execute(deleteGHQuery, new SqlParameter("@id", customerId));

                        // Xóa khách hàng
                        string deleteKHQuery = "DELETE FROM KhachHang WHERE IDKhachHang = @id";
                        db.Execute(deleteKHQuery, new SqlParameter("@id", customerId));

                        // Xóa tài khoản liên quan
                        string deleteTKQuery = "DELETE FROM TaiKhoan WHERE IDTaiKhoan = @id";
                        db.Execute(deleteTKQuery, new SqlParameter("@id", idTaiKhoan));

                        MessageBox.Show("Đã xóa khách hàng và tài khoản thành công!", "Thông báo");
                        LoadCustomer();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa khách hàng: " + ex.Message, "Lỗi");
                    }
                }
            }
        }
    }
}