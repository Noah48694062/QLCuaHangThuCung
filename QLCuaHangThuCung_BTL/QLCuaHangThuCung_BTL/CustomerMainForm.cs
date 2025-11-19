using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    partial class CustomerMainForm : Form
    {
        private DBConnect db = new DBConnect();
        private Form activeForm = null;

        private string currentIDKhachHang;
        private string currentTenNguoiDung;

        public CustomerMainForm(string tenNguoiDung, string idKhachHang)
        {
            InitializeComponent();

            this.currentIDKhachHang = idKhachHang;
            this.currentTenNguoiDung = tenNguoiDung;

            SetupSidebarInfo(tenNguoiDung);
        }

        private void SetupSidebarInfo(string tenHienThi)
        {
            lblUsername.Text = tenHienThi;
            lblRole.Text = "Khách hàng";
            lblTitle.Text = "Chào mừng, " + tenHienThi.Split(' ')[0];
        }


        public void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();

            panelDash.Visible = false; // Ẩn Dash panel

            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelChild.Controls.Clear();
            panelChild.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();

            lblTitle.Text = childForm.Text.ToUpper();
        }

        // ===============================================
        // LOGIC XỬ LÝ NÚT THÔNG TIN TÀI KHOẢN
        // ===============================================
        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {
            this.lblTitle.Text = "THÔNG TIN CÁ NHÂN";

            // Mở InformationForm và TRUYỀN ID KHÁCH HÀNG ĐANG ĐĂNG NHẬP
            // Chú ý: Dùng constructor mới InformationForm(string customerID)
            InformationForm profileForm = new InformationForm(this.currentIDKhachHang);

            openChildForm(profileForm);
        }

        // ... (Các hàm khác giữ nguyên)

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát ứng dụng?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Login login = new Login();
                this.Dispose();
                login.ShowDialog();
            }
        }

        private void btnMuaSam_Click(object sender, EventArgs e)
        {
            openChildForm(new Form_MuaSam(this.currentIDKhachHang));
        }

        private void btnGioHang_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "GIỎ HÀNG";
        }

        private void btnLichSu_Click(object sender, EventArgs e)
        {
            lblTitle.Text = "LỊCH SỬ MUA HÀNG";
        }

        private void panelChild_Paint(object sender, PaintEventArgs e)
        {
            // Để trống
        }

        // ... (Phần Assembly Accessors giữ nguyên)
    }
}