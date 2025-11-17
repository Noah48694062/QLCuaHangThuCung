using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            openChildForm(new Cash(this));
        }
        private Form activeForm = null;
        public void openChildForm(Form childForm)
        {
            if(activeForm != null)
            {
                activeForm.Close();
            }
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            lblTitle.Text = childForm.Text;
            panelChild.Controls.Add(childForm);
            panelChild.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        public void AddToDailyReport(decimal TongDoanhThu)
        {
            decimal currentTongDoanhThu = 0;
            if(decimal.TryParse(lblDailySale.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out currentTongDoanhThu))
            {
                decimal newTongDoanhThu = currentTongDoanhThu + TongDoanhThu;
                //Cập nhật tổng doanh thu
                lblDailySale.Text = newTongDoanhThu.ToString("C0");
            }
        }
    }
}
