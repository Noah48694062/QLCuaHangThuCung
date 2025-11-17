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

        // 1. Biến để LƯU TRỮ ID và Tên của khách hàng
        private string currentIDKhachHang;
        private string currentTenNguoiDung;
        //public CustomerMainForm()
        //{
        //    InitializeComponent();

        //}

        public CustomerMainForm(string tenNguoiDung, string idKhachHang)
        {
            InitializeComponent();

            // 4. Lưu thông tin khách hàng lại
            this.currentIDKhachHang = idKhachHang;
            this.currentTenNguoiDung = tenNguoiDung;

            // 5. Hiển thị tên lên các Label (Giả sử CMainForm có các Label này)
            // (Nếu bạn kế thừa từ MainForm, các Label này đã có sẵn)
            if (this.Controls.ContainsKey("lblUsername"))
            {
                (this.Controls["lblUsername"] as Label).Text = tenNguoiDung;
            }
            if (this.Controls.ContainsKey("lblRole"))
            {
                (this.Controls["lblRole"] as Label).Text = "Khách hàng";
            }
        }

        public void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();

            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            lblTitle.Text = childForm.Text;
            panelChild.Controls.Clear();
            panelChild.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
        }


        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Logout Application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

        }

        private void btnLichSu_Click(object sender, EventArgs e)
        {

        }

        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {

        }
    }
}
