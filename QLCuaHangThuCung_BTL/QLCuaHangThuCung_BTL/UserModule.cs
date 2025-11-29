using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class UserModule : Form
    {
        DBConnect db = new DBConnect();
        UserForm parent;

        // Khai báo lại các controls công khai với TÊN MỚI
        


        public UserModule(UserForm form)
        {
            InitializeComponent();
            parent = form;

            // Gán sự kiện Click cho các nút
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
        }

        public void Clear()
        {
            txtIDNhanVien.Clear();
            txtHoVaTen.Clear();
            txtDiaChi.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtMatKhau.Clear();
            txtNgaySinh.Value = DateTime.Now; // Đặt lại ngày sinh mặc định

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtIDNhanVien.Enabled = true;
        }

        private bool CheckRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(txtIDNhanVien.Text) ||
            string.IsNullOrWhiteSpace(txtHoVaTen.Text) ||
            string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
            string.IsNullOrWhiteSpace(txtSDT.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường bắt buộc!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Kiểm tra Độ dài Mật khẩu (Ví dụ: Tối thiểu 6 ký tự)
            if (txtMatKhau.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return false;
            }

            // 3. Kiểm tra Số điện thoại (Phải là số và độ dài khoảng 10 số)
            if (!long.TryParse(txtSDT.Text, out _) || txtSDT.Text.Length != 10)
            {
                MessageBox.Show("Số điện thoại không hợp lệ! (Phải là 10 chữ số)", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Email không đúng định dạng (VD: abc@gmail.com)", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            DateTime limitDate = DateTime.Today.AddYears(-18);

            // Nếu ngày sinh LỚN HƠN mốc này => Chưa đủ 18 tuổi
            if (txtNgaySinh.Value > limitDate)
            {
                MessageBox.Show("Nhân viên phải đủ 18 tuổi trở lên!", "Tuổi không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNgaySinh.Focus();
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!CheckRequiredFields())
            {
                return;
            }

            try
            {
                string id = txtIDNhanVien.Text.Trim();
                //string gioiTinh = "Nam"; // Giả định Giới tính (Cần bổ sung control cho Giới tính)

                // 1. Thêm Tài khoản (IDTaiKhoan = IDNhanVien)
                string queryTK = @"
                    INSERT INTO TaiKhoan(IDTaiKhoan, TenDangNhap, MatKhau, IDRole)
                    VALUES (@id, @tendn, @matkhau, @role)";

                db.Execute(queryTK,
                    new SqlParameter("@id", id),
                    new SqlParameter("@tendn", id),
                    new SqlParameter("@matkhau", txtMatKhau.Text),
                    new SqlParameter("@role", "R01") // Giả định vai trò Admin
                );

                // 2. Thêm Nhân viên
                string queryNV = @"
                    INSERT INTO NhanVien(IDNhanVien, TenNhanVien, SDT, DiaChi, Email, IDTaiKhoan, NgaySinh)
                    VALUES (@id, @ten, @sdt, @diachi, @email, @idtk, @ngaysinh)";

                db.Execute(queryNV,
                    new SqlParameter("@id", id),
                    new SqlParameter("@ten", txtHoVaTen.Text.Trim()),
                    new SqlParameter("@sdt", txtSDT.Text.Trim()),
                    new SqlParameter("@diachi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@idtk", id),
                    new SqlParameter("@ngaysinh", txtNgaySinh.Value)
                );

                MessageBox.Show("Đã thêm nhân viên thành công!", "Thông báo");
                parent.LoadUser();
                this.Close();
            }
            catch (SqlException ex)
            {
                // Xử lý lỗi trùng lặp khóa chính hoặc lỗi SQL khác
                if (ex.Number == 2627)
                {
                    MessageBox.Show("Lỗi: Mã nhân viên/Tài khoản đã tồn tại. Vui lòng chọn mã khác.", "Lỗi trùng lặp ID");
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

            if (MessageBox.Show("Xác nhận cập nhật thông tin nhân viên?", "Cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                string idNhanVien = txtIDNhanVien.Text.Trim();
                //string gioiTinh = "Nam"; // Giả định Giới tính

                // 1. Cập nhật Tài khoản (Mật khẩu)
                string queryTK = @"
                    UPDATE TaiKhoan SET
                        MatKhau = @matkhau
                    WHERE IDTaiKhoan = @id";

                db.Execute(queryTK,
                    new SqlParameter("@id", idNhanVien),
                    new SqlParameter("@matkhau", txtMatKhau.Text)
                );

                // 2. Cập nhật Nhân viên
                string queryNV = @"
                    UPDATE NhanVien SET
                        TenNhanVien = @ten,
                        SDT = @sdt,
                        DiaChi = @diachi,
                        Email = @email,
                        NgaySinh = @ngaysinh
                    WHERE IDNhanVien = @id";

                db.Execute(queryNV,
                    new SqlParameter("@id", idNhanVien),
                    new SqlParameter("@ten", txtHoVaTen.Text.Trim()),
                    new SqlParameter("@sdt", txtSDT.Text.Trim()),
                    new SqlParameter("@diachi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@ngaysinh", txtNgaySinh.Value)
                );

                MessageBox.Show("Đã cập nhật thông tin nhân viên!", "Thông báo");
                parent.LoadUser();
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

        private void UserModule_Load(object sender, EventArgs e)
        {
            // Có thể thêm logic load dữ liệu nếu cần
        }

        // Cần có hàm xử lý sự kiện TextChanged của txtIDNhanVien nếu bạn có gán sự kiện này
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Bạn có thể thêm logic kiểm tra ID ở đây
        }
    }
}