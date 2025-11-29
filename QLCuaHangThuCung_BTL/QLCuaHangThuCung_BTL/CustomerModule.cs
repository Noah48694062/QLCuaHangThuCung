using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class CustomerModule : Form
    {
        DBConnect db = new DBConnect();
        CustomerForm parent;

        public CustomerModule(CustomerForm form)
        {
            InitializeComponent();
            parent = form;

            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
        }
        private void CustomerModule_Load(object sender, EventArgs e)
        {
            // Logic sẽ chạy khi form được load, hiện tại để trống
        }

        public void Clear()
        {
            txtIDKhachHang.Clear();
            txtHoVaTen.Clear();
            txtDiaChi.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtMatKhau.Clear();

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtIDKhachHang.Enabled = true;
        }

        private bool CheckRequiredFields()
        {
            // 1. Kiểm tra để trống
            if (string.IsNullOrWhiteSpace(txtIDKhachHang.Text) ||
                string.IsNullOrWhiteSpace(txtHoVaTen.Text) ||
                string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
                string.IsNullOrWhiteSpace(txtSDT.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường bắt buộc!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Kiểm tra Số điện thoại (Phải là số và độ dài khoảng 10 số)
            if (!long.TryParse(txtSDT.Text, out _) || txtSDT.Text.Length != 10)
            {
                MessageBox.Show("Số điện thoại không hợp lệ! (Phải là 10 chữ số)", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            // 3. Kiểm tra Email (Đơn giản: Phải chứa @ và dấu chấm)
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Email không đúng định dạng (VD: khachhang@gmail.com)", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // 4. Kiểm tra Độ dài Mật khẩu (Ví dụ: Tối thiểu 6 ký tự)
            if (txtMatKhau.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự để đảm bảo an toàn!", "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return false;
            }

            // 5. Kiểm tra Mã khách hàng không chứa ký tự đặc biệt (Tùy chọn)
            // Giả sử mã chỉ được chứa chữ và số
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtIDKhachHang.Text, "^[a-zA-Z0-9]*$"))
            {
                MessageBox.Show("Mã khách hàng không được chứa ký tự đặc biệt!", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIDKhachHang.Focus();
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredFields())
            {
                return;
            }

            try
            {
                string id = txtIDKhachHang.Text.Trim();
                string gioiTinh = "Nam"; // Giả định Giới tính

                // 1. Thêm Tài khoản
                string queryTK = @"
                    INSERT INTO TaiKhoan(IDTaiKhoan, TenDangNhap, MatKhau, IDRole)
                    VALUES (@id, @tendn, @matkhau, @role)";

                db.Execute(queryTK,
                    new SqlParameter("@id", id),
                    new SqlParameter("@tendn", id),
                    new SqlParameter("@matkhau", txtMatKhau.Text),
                    new SqlParameter("@role", "R02") // Vai trò Khách hàng
                );

                // 2. Thêm Khách hàng
                string queryKH = @"
                    INSERT INTO KhachHang(IDKhachHang, TenKhachHang, SDT, DiaChi, Email, IDTaiKhoan, GioiTinh)
                    VALUES (@id, @ten, @sdt, @diachi, @email, @idtk, @gioitinh)";

                db.Execute(queryKH,
                    new SqlParameter("@id", id),
                    new SqlParameter("@ten", txtHoVaTen.Text.Trim()),
                    new SqlParameter("@sdt", txtSDT.Text.Trim()),
                    new SqlParameter("@diachi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@idtk", id),
                    new SqlParameter("@gioitinh", gioiTinh)
                );

                MessageBox.Show("Đã thêm khách hàng thành công!", "Thông báo");
                parent.LoadCustomer();
                this.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    MessageBox.Show("Lỗi: Mã khách hàng/Tài khoản đã tồn tại. Vui lòng chọn mã khác.", "Lỗi trùng lặp ID");
                }
                else
                {
                    MessageBox.Show("Lỗi cơ sở dữ liệu: " + ex.Message, "Lỗi SQL");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredFields())
            {
                return;
            }

            if (MessageBox.Show("Xác nhận cập nhật thông tin khách hàng?", "Cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                string idKhachHang = txtIDKhachHang.Text.Trim();
                string gioiTinh = "Nam"; // Giả định Giới tính

                // 1. Cập nhật Tài khoản (Mật khẩu)
                string queryTK = @"
                    UPDATE TaiKhoan SET MatKhau = @matkhau
                    WHERE IDTaiKhoan = @id";

                db.Execute(queryTK,
                    new SqlParameter("@id", idKhachHang),
                    new SqlParameter("@matkhau", txtMatKhau.Text)
                );

                // 2. Cập nhật Khách hàng
                string queryKH = @"
                    UPDATE KhachHang SET
                        TenKhachHang = @ten,
                        SDT = @sdt,
                        DiaChi = @diachi,
                        Email = @email,
                        GioiTinh = @gioitinh
                    WHERE IDKhachHang = @id";

                db.Execute(queryKH,
                    new SqlParameter("@id", idKhachHang),
                    new SqlParameter("@ten", txtHoVaTen.Text.Trim()),
                    new SqlParameter("@sdt", txtSDT.Text.Trim()),
                    new SqlParameter("@diachi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@gioitinh", gioiTinh)
                );

                MessageBox.Show("Đã cập nhật thông tin khách hàng!", "Thông báo");
                parent.LoadCustomer();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật: " + ex.Message, "Lỗi");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lblcid_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
    }
}