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

        // CẦN BỔ SUNG: Bạn nên thêm một TextBox công khai (public) tên là txtIDKhachHang vào Designer
        // Nếu chưa có, bạn có thể thêm nó vào Designer.cs hoặc khai báo thủ công ở đây:
        public System.Windows.Forms.TextBox txtIDKhachHang = new System.Windows.Forms.TextBox();
        public System.Windows.Forms.TextBox txtID; // Giả định đây là Mã ID
        public System.Windows.Forms.TextBox txtTen; // Giả định đây là Tên (HoVaTen)
        // Constructor chỉ nhận ID khách hàng
        public InformationModule(string customerID)
        {
            InitializeComponent();
            this.currentCustomerID = customerID;
            this.currentIDTaiKhoan = customerID; // Giả định ID Khách hàng = ID Tài khoản

            // Nếu bạn dùng txtIDKhachHang:
            this.txtIDKhachHang.Text = currentCustomerID;
            this.txtIDKhachHang.Enabled = false; // Chặn sửa ID

            // Gán sự kiện Click cho các nút
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
        }

        private void SetupProfileView()
        {
            btnUpdate.Enabled = true;
            btnUpdate.Text = "CẬP NHẬT THÔNG TIN";

            // Khách hàng nên thấy mật khẩu gốc để sửa
            txtMatKhau.PasswordChar = '\0';

            // Cập nhật tiêu đề form con
            this.Text = "THÔNG TIN CÁ NHÂN";
        }
        // Trong InformationModule.cs

        // CONSTRUCTOR CHỨC NĂNG QUẢN LÝ NCC
        public InformationModule(InformationForm form)
        {
            InitializeComponent();
            // Lưu trữ form cha
                           // ... (logic khởi tạo)
        }

        private void LoadCustomerProfile()
        {
            try
            {
                string query = @"
                    SELECT 
                        KH.TenKhachHang, KH.SDT, KH.DiaChi, KH.Email, TK.MatKhau
                    FROM KhachHang KH
                    JOIN TaiKhoan TK ON KH.IDTaiKhoan = TK.IDTaiKhoan
                    WHERE KH.IDKhachHang = @id";

                DataTable dt = db.GetData(query, new SqlParameter("@id", currentCustomerID));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Hiển thị dữ liệu lên các controls
                    txtHoVaTen.Text = row["TenKhachHang"].ToString();
                    txtDiaChi.Text = row["DiaChi"].ToString();
                    txtSDT.Text = row["SDT"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtMatKhau.Text = row["MatKhau"].ToString();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin khách hàng.", "Lỗi dữ liệu");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông tin: " + ex.Message, "Lỗi");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text) ||
                string.IsNullOrWhiteSpace(txtDiaChi.Text) || string.IsNullOrWhiteSpace(txtSDT.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Cảnh báo");
                return;
            }

            if (MessageBox.Show("Xác nhận cập nhật thông tin cá nhân?", "Cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                string gioiTinh = "Nam"; // Giả định Giới tính

                // 1. Cập nhật Tài khoản (Mật khẩu)
                string queryTK = @"
                    UPDATE TaiKhoan SET MatKhau = @matkhau
                    WHERE IDTaiKhoan = @id";

                db.Execute(queryTK,
                    new SqlParameter("@id", currentIDTaiKhoan),
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
                    new SqlParameter("@id", currentCustomerID),
                    new SqlParameter("@ten", txtHoVaTen.Text.Trim()),
                    new SqlParameter("@sdt", txtSDT.Text.Trim()),
                    new SqlParameter("@diachi", txtDiaChi.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@gioitinh", gioiTinh)
                );

                MessageBox.Show("Đã cập nhật thông tin cá nhân thành công!", "Thông báo");
                this.DialogResult = DialogResult.OK; // Thiết lập kết quả để form cha biết
                this.Close(); // Đóng form module
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

        // HÀM XỬ LÝ SỰ KIỆN LOAD (ĐỂ KHẮC PHỤC LỖI CS0117 TRƯỚC ĐÓ)
        private void InformationModule_Load(object sender, EventArgs e)
        {
            SetupProfileView(); // Cấu hình giao diện (tiêu đề, nút,...)
            LoadCustomerProfile(); // Tải dữ liệu khi form mở
        }
    }
}