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
            string query = @"
        SELECT TK.TenDangNhap, VT.TenVaiTro, KH.IDKhachHang
        FROM TaiKhoan AS TK
        JOIN VaiTro AS VT ON TK.IDRole = VT.IDRole
        LEFT JOIN KhachHang AS KH ON TK.IDTaiKhoan = KH.IDTaiKhoan
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
                        // 1. Nếu là Admin -> Mở MainForm (Phần này đã đúng)
                        MainForm main = new MainForm();
                        main.lblUsername.Text = _name;
                        main.lblRole.Text = _role;
                        main.btnUser.Enabled = true;

                        main.ShowDialog();
                    }
                    else if (_role == "Khách hàng")
                    {
                        // ===================================
                        // == SỬA LỖI CHÍNH Ở ĐÂY ==
                        // ===================================

                        // 2. Lấy ID Khách Hàng (bạn đã select nhưng chưa dùng)
                        string _idKhachHang = dt.Rows[0]["IDKhachHang"].ToString();

                        // 3. Truyền Tên VÀ ID sang CustomerMainForm (theo constructor ta đã sửa)
                        CustomerMainForm customerForm = new CustomerMainForm(_name, _idKhachHang);
                        customerForm.ShowDialog();

                        // 4. Bỏ MessageBox này đi, vì form mới đã hiện lên
                        // MessageBox.Show("Đăng nhập với tư cách Khách hàng thành công.");
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
