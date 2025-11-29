using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class UserForm : Form
    {
        DBConnect db = new DBConnect();

        public UserForm()
        {
            InitializeComponent();
            LoadUser();
            // Đảm bảo cột mật khẩu được hiển thị dưới dạng ẩn danh (dấu *)
            if (dgvUser.Columns.Contains("Column8"))
            {
                dgvUser.Columns["Column8"].HeaderText = "Mật khẩu";
            }
            // Gán sự kiện TextChanged cho ô tìm kiếm
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // Gán sự kiện Click cho nút Thêm
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // Gán sự kiện CellContentClick cho DataGridView
            //this.dgvUser.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUser_CellContentClick);
        }

        public void LoadUser()
        {
            dgvUser.Rows.Clear();

            // 1. Thêm NgaySinh vào câu truy vấn
            string query = @"
        SELECT IDNhanVien, TenNhanVien, DiaChi, SDT, Email, NgaySinh, IDTaiKhoan
        FROM NhanVien 
        WHERE TenNhanVien LIKE @search OR IDNhanVien LIKE @search 
        ORDER BY IDNhanVien ASC";

            DataTable dt = db.GetData(query,
                new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%")
            );

            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                i++;
                string matKhau = GetPasswordFromTaiKhoan(row["IDTaiKhoan"].ToString());

                // 2. Đổ dữ liệu đúng thứ tự cột trên Grid
                dgvUser.Rows.Add(
                    i,
                    row["IDNhanVien"].ToString(),
                    row["TenNhanVien"].ToString(),
                    row["DiaChi"].ToString(),
                    row["SDT"].ToString(),
                    row["Email"].ToString(),
                    Convert.ToDateTime(row["NgaySinh"]).ToString("dd/MM/yyyy"), // Cột 7: Ngày Sinh (Format đẹp)
                    "********" // Cột 8: Mật khẩu
                );
            }
        }

        // Hàm này tìm mật khẩu gốc từ bảng TaiKhoan
        private string GetPasswordFromTaiKhoan(string idTaiKhoan)
        {
            string query = "SELECT MatKhau FROM TaiKhoan WHERE IDTaiKhoan = @id";
            DataTable dt = db.GetData(query, new SqlParameter("@id", idTaiKhoan));
            return dt.Rows.Count > 0 ? dt.Rows[0]["MatKhau"].ToString() : "";
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            UserModule module = new UserModule(this);
            module.btnSave.Enabled = true;
            module.btnUpdate.Enabled = false;
            module.txtIDNhanVien.Enabled = true; // Cho phép nhập ID mới
            module.ShowDialog();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUser();
        }

        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ngăn chặn lỗi click vào header
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvUser.Columns[e.ColumnIndex].Name;
            // Column2 là Mã nhân viên
            string userId = dgvUser.Rows[e.RowIndex].Cells["Column2"].Value.ToString();
            string idTaiKhoan = dgvUser.Rows[e.RowIndex].Cells["Column2"].Value.ToString(); // Giả định IDNhanVien trùng IDTaiKhoan cho đơn giản

            // ================= EDIT =================
            if (colName == "Edit")
            {
                // Lấy mật khẩu gốc từ DB
                string originalPassword = GetPasswordFromTaiKhoan(idTaiKhoan);

                UserModule module = new UserModule(this)
                {
                    btnSave = { Enabled = false },
                    btnUpdate = { Enabled = true },
                    txtIDNhanVien = { Enabled = false } // Không cho phép sửa Mã nhân viên
                };

                // Đổ dữ liệu vào UserModule
                module.txtIDNhanVien.Text = userId;
                module.txtHoVaTen.Text = dgvUser.Rows[e.RowIndex].Cells["Column3"].Value.ToString();
                module.txtDiaChi.Text = dgvUser.Rows[e.RowIndex].Cells["Column4"].Value.ToString();
                module.txtSDT.Text = dgvUser.Rows[e.RowIndex].Cells["Column5"].Value.ToString();
                module.txtEmail.Text = dgvUser.Rows[e.RowIndex].Cells["Column6"].Value.ToString();
                // dtDob (Cột 7)
                // dgvUser.Rows[e.RowIndex].Cells["Column7"].Value.ToString(); // GioiTinh

                module.txtMatKhau.Text = originalPassword;
                module.ShowDialog();
            }

            // ================= DELETE =================
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Xóa nhân viên này? Thao tác này sẽ xóa cả tài khoản liên quan.", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        // Xóa nhân viên trước
                        string deleteNVQuery = "DELETE FROM NhanVien WHERE IDNhanVien = @id";
                        db.Execute(deleteNVQuery, new SqlParameter("@id", userId));

                        // Xóa tài khoản liên quan
                        string deleteTKQuery = "DELETE FROM TaiKhoan WHERE IDTaiKhoan = @id";
                        db.Execute(deleteTKQuery, new SqlParameter("@id", idTaiKhoan));

                        MessageBox.Show("Đã xóa nhân viên và tài khoản thành công!", "Thông báo");
                        LoadUser();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message, "Lỗi");
                    }
                }
            }
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            // Thiết lập cột GioiTinh hiển thị tốt hơn
            if (dgvUser.Columns.Contains("Column7"))
            {
                dgvUser.Columns["Column7"].HeaderText = "Giới tính";
            }
        }
    }
}