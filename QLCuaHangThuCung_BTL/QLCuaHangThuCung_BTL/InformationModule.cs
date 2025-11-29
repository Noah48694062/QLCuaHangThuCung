using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class InformationModule : Form
    {
        DBConnect db = new DBConnect();
        private string currentCustomerID;
        private string currentIDTaiKhoan;

        public InformationModule(string customerID)
        {
            InitializeComponent();
            this.currentCustomerID = customerID;
            this.currentIDTaiKhoan = customerID; // Giả định ID Khách hàng = ID Tài khoản

            // Gán sự kiện Click
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            this.Load += new EventHandler(this.InformationModule_Load);
        }

        private void InformationModule_Load(object sender, EventArgs e)
        {
            SetupProfileView();
            LoadCustomerProfile();
        }

        private void SetupProfileView()
        {
            //btnSave.Visible = false;
            btnUpdate.Enabled = true;
            btnUpdate.Text = "CẬP NHẬT THÔNG TIN";
            txtMatKhau.PasswordChar = '\0'; // Hiện mật khẩu rõ
            this.Text = "THÔNG TIN CÁ NHÂN";

            // Khóa trường ID
            txtIDKhachHang.Text = currentCustomerID;
            txtIDKhachHang.Enabled = false;
        }

        // ==========================================================
        // 1. HÀM VALIDATE DỮ LIỆU (MỚI THÊM)
        // ==========================================================
        private bool CheckRequiredFields()
        {
            // Kiểm tra trống
            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text) ||
                string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
                string.IsNullOrWhiteSpace(txtSDT.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng không để trống thông tin!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra SĐT (Phải là số và 10 chữ số)
            if (!long.TryParse(txtSDT.Text, out _) || txtSDT.Text.Length != 10)
            {
                MessageBox.Show("Số điện thoại không hợp lệ (Phải là 10 chữ số)!", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            // Kiểm tra Email
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Email không đúng định dạng!", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Kiểm tra Mật khẩu (Tối thiểu 6 ký tự)
            if (txtMatKhau.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return false;
            }

            return true;
        }

        private void LoadCustomerProfile()
        {
            try
            {
                string query = @"
                    SELECT KH.TenKhachHang, KH.SDT, KH.DiaChi, KH.Email, TK.MatKhau
                    FROM KhachHang KH
                    JOIN TaiKhoan TK ON KH.IDTaiKhoan = TK.IDTaiKhoan
                    WHERE KH.IDKhachHang = @id";

                DataTable dt = db.GetData(query, new SqlParameter("@id", currentCustomerID));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtHoVaTen.Text = row["TenKhachHang"].ToString();
                    txtDiaChi.Text = row["DiaChi"].ToString();
                    txtSDT.Text = row["SDT"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtMatKhau.Text = row["MatKhau"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Gọi hàm kiểm tra trước khi xử lý
            if (!CheckRequiredFields()) return;

            if (MessageBox.Show("Xác nhận cập nhật thông tin cá nhân?", "Cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                // Cập nhật Mật khẩu
                string queryTK = "UPDATE TaiKhoan SET MatKhau = @matkhau WHERE IDTaiKhoan = @id";
                db.Execute(queryTK,
                    new SqlParameter("@id", currentIDTaiKhoan),
                    new SqlParameter("@matkhau", txtMatKhau.Text)
                );

                // Cập nhật Thông tin cá nhân (Bỏ phần Giới tính nếu không cần, hoặc thêm nếu có)
                string queryKH = @"
                    UPDATE KhachHang SET
                        TenKhachHang = @ten,
                        SDT = @sdt,
                        DiaChi = @diachi,
                        Email = @email
                    WHERE IDKhachHang = @id";

                db.Execute(queryKH,
                    new SqlParameter("@id", currentCustomerID),
                    new SqlParameter("@ten", txtHoVaTen.Text.Trim()),
                    new SqlParameter("@sdt", txtSDT.Text.Trim()),
                    new SqlParameter("@diachi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim())
                );

                MessageBox.Show("Đã cập nhật thông tin cá nhân thành công!", "Thông báo");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Lỗi");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}