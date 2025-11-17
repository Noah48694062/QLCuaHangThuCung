using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class Login : Form
    {

        DBConnect db = new DBConnect();
        public Login()
        {
            InitializeComponent();
        }



       

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //string query = @"
            //SELECT TK.TenDangNhap, VT.TenVaiTro
            //FROM TaiKhoan AS TK
            //JOIN VaiTro AS VT ON TK.IDRole = VT.IDRole
            //WHERE TK.TenDangNhap = @name AND TK.MatKhau = @password";

            //SqlParameter[] parameters =
            //{
            //    new SqlParameter("@name", txtname.Text),
            //    new SqlParameter("@password", txtpass.Text) // So sánh mật khẩu plain text
            //};

            //try
            //{
            //    // 3. Gọi GetData để thực thi truy vấn
            //    DataTable dt = db.GetData(query, parameters);

            //    // 4. Kiểm tra kết quả (nếu có 1 hàng trả về là đúng)
            //    if (dt.Rows.Count > 0)
            //    {
            //        // Lấy dữ liệu từ hàng đầu tiên tìm thấy
            //        string _name = dt.Rows[0]["TenDangNhap"].ToString();
            //        string _role = dt.Rows[0]["TenVaiTro"].ToString(); // Tên vai trò (vd: 'Admin')

            //        MessageBox.Show("Chào mừng " + _name + " |", "ĐĂNG NHẬP THÀNH CÔNG", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //        // 5. Mở MainForm và truyền dữ liệu
            //        MainForm main = new MainForm();
            //        main.lblUsername.Text = _name;
            //        main.lblRole.Text = _role;

            //        // Kiểm tra vai trò để bật/tắt chức năng
            //        if (_role == "Admin") // Tên vai trò phải khớp với CSDL
            //        {
            //            main.btnUser.Enabled = true; // (Thay btnUser bằng tên nút của bạn)
            //        }

            //        this.Hide();
            //        main.ShowDialog();

            //        // Cân nhắc: Có thể thêm this.Close() ở đây nếu muốn đóng form login
            //    }
            //    else
            //    {
            //        // Thông báo lỗi
            //        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không hợp lệ!", "ĐĂNG NHẬP THẤT BẠI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Lỗi hệ thống chung (lớp DBConnect đã báo lỗi SQL rồi)
            //    MessageBox.Show("Lỗi hệ thống khi đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            

            string query = @"
            SELECT TK.TenDangNhap, VT.TenVaiTro
            FROM TaiKhoan AS TK
            JOIN VaiTro AS VT ON TK.IDRole = VT.IDRole
            WHERE TK.TenDangNhap = @name AND TK.MatKhau = @password";

            SqlParameter[] parameters =
            {
                new SqlParameter("@name", txtname.Text),
                new SqlParameter("@password", txtpass.Text)
            };

            try
            {
                DataTable dt = db.GetData(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    // Lấy thông tin người dùng
                    string _name = dt.Rows[0]["TenDangNhap"].ToString();
                    string _role = dt.Rows[0]["TenVaiTro"].ToString(); // 'Admin' hoặc 'Khách hàng'

                    MessageBox.Show("Chào mừng " + _name + " |", "ĐĂNG NHẬP THÀNH CÔNG", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide(); // Ẩn form login ngay sau khi đăng nhập đúng

                    // ===================================
                    // == PHÂN LUỒNG DỰA TRÊN VAI TRÒ ==
                    // ===================================
                    if (_role == "Admin")
                    {
                        // 1. Nếu là Admin -> Mở MainForm
                        MainForm main = new MainForm();
                        main.lblUsername.Text = _name;
                        main.lblRole.Text = _role;
                        main.btnUser.Enabled = true; // (Ví dụ: bật nút quản lý user)

                        main.ShowDialog();
                    }
                    else if (_role == "Khách hàng")
                    {
                        // 2. Nếu là Khách hàng -> Mở CustomerMainForm
                        // (Bạn phải tạo một Form mới tên là CustomerMainForm)

                        // CustomerMainForm customerForm = new CustomerMainForm();
                        // customerForm.lblWelcome.Text = "Chào mừng, " + _name; // (Ví dụ)
                        // customerForm.ShowDialog();

                        // Do form chưa có, tạm thời thông báo:
                        MessageBox.Show("Đăng nhập với tư cách Khách hàng thành công.");
                    }
                    else
                    {
                        // 3. Trường hợp vai trò không xác định (nếu có)
                        MessageBox.Show("Vai trò của bạn (" + _role + ") không được hỗ trợ.", "Lỗi Vai Trò", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Sau khi form Admin hoặc form Khách hàng bị đóng, Form Login cũng sẽ tự đóng
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không hợp lệ!", "ĐĂNG NHẬP THẤT BẠI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống khi đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnForget_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please contact your BOSS!", "FORGET PASSWORD", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit Application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
