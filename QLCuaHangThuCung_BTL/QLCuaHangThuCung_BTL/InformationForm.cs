using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class InformationForm : Form
    {
        DBConnect db = new DBConnect();
        private string currentCustomerID = null; // Biến lưu ID khách hàng hiện tại

        // Constructor mới nhận ID khách hàng
        public InformationForm(string customerID)
        {
            InitializeComponent();
            this.currentCustomerID = customerID;

            // Ẩn các chức năng không cần thiết
            this.btnAdd.Visible = false;
            this.txtSearch.Visible = false;

            LoadInformation();
            SetupDataGridViewColumns();

            // Gán sự kiện chỉ cho DataGridView (Edit/Delete)
            this.dgvCustomer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomer_CellContentClick);
        }

        // Constructor cũ (giữ lại nếu Admin vẫn cần dùng form này để quản lý NCC)
        public InformationForm()
        {
            InitializeComponent();
            LoadInformation();
            // ... (Gán sự kiện cho Admin)
        }

        private void SetupDataGridViewColumns()
        {
            // Thiết lập header và ẩn cột Mật khẩu
            if (dgvCustomer.Columns.Contains("dataGridViewTextBoxColumn2"))
                dgvCustomer.Columns["dataGridViewTextBoxColumn2"].HeaderText = "Mã KH";
            if (dgvCustomer.Columns.Contains("dataGridViewTextBoxColumn3"))
                dgvCustomer.Columns["dataGridViewTextBoxColumn3"].HeaderText = "Họ và tên";
            if (dgvCustomer.Columns.Contains("MatKhau"))
                dgvCustomer.Columns["MatKhau"].HeaderText = "Mật khẩu";
            // Không ẩn Mật khẩu ở đây để Admin thấy, nhưng nếu là KH thì phải hiện MatKhau gốc
        }

        private string GetPasswordFromTaiKhoan(string idTaiKhoan)
        {
            string query = "SELECT MatKhau FROM TaiKhoan WHERE IDTaiKhoan = @id";
            DataTable dt = db.GetData(query, new SqlParameter("@id", idTaiKhoan));
            return dt.Rows.Count > 0 ? dt.Rows[0]["MatKhau"].ToString() : "";
        }


        public void LoadInformation()
        {
            dgvCustomer.Rows.Clear();

            // Nếu không có ID khách hàng (có thể là chế độ Admin), thoát hoặc tải tất cả NCC
            if (currentCustomerID == null) return;

            // TRUY VẤN CHỈ THÔNG TIN CỦA KHÁCH HÀNG ĐANG ĐĂNG NHẬP
            string query = @"
                SELECT KH.IDKhachHang, KH.TenKhachHang, KH.DiaChi, KH.SDT, KH.Email, KH.GioiTinh, KH.IDTaiKhoan, TK.MatKhau
                FROM KhachHang KH
                JOIN TaiKhoan TK ON KH.IDTaiKhoan = TK.IDTaiKhoan
                WHERE KH.IDKhachHang = @id";

            DataTable dt = db.GetData(query,
                new SqlParameter("@id", currentCustomerID)
            );

            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                i++;

                dgvCustomer.Rows.Add(
                    i,
                    row["IDKhachHang"].ToString(),
                    row["TenKhachHang"].ToString(),
                    row["GioiTinh"].ToString(),
                    row["DiaChi"].ToString(),
                    row["SDT"].ToString(),
                    row["Email"].ToString(),
                    row["MatKhau"].ToString() // Hiện Mật khẩu gốc cho chính Khách hàng đó
                );
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvCustomer.Columns[e.ColumnIndex].Name;
            string customerId = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();

           
            if (colName == "Edit" && customerId == currentCustomerID)
            {
           
                InformationModule module = new InformationModule(currentCustomerID) 
                {
                 
                    btnSave = { Visible = false },
                    btnUpdate = { Visible = true, Enabled = true },
                    txtIDKhachHang = { Enabled = false } // Chặn sửa ID
                };

                // Đổ dữ liệu từ DataGridView sang Module (vì dữ liệu đã được tải)
                module.txtIDKhachHang.Text = customerId;
                module.txtHoVaTen.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
                module.txtDiaChi.Text = dgvCustomer.Rows[e.RowIndex].Cells[4].Value.ToString();
                module.txtSDT.Text = dgvCustomer.Rows[e.RowIndex].Cells[5].Value.ToString();
                module.txtEmail.Text = dgvCustomer.Rows[e.RowIndex].Cells[6].Value.ToString();
                module.txtMatKhau.Text = dgvCustomer.Rows[e.RowIndex].Cells[7].Value.ToString(); // Mật khẩu gốc

                if (module.ShowDialog() == DialogResult.OK)
                {
                    LoadInformation(); // CHỈ TẢI LẠI KHI CẬP NHẬT THÀNH CÔNG (module đóng)
                }
            }
            // Ẩn nút Delete (hoặc không làm gì)
        }
    }
}