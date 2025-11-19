using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class MainForm : Form
    {
        // Kết nối DB
        private DBConnect db = new DBConnect();

        // Form con đang mở
        private Form activeForm = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Timer hiển thị thời gian
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            // Load dữ liệu tổng doanh thu ngày
            loadDailySale();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            progress.Text = DateTime.Now.ToString("hh:mm:ss");
            progress.Value = DateTime.Now.Second;
        }

        //=============================
        // OPEN CHILD FORM
        //=============================
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //=============================
        // LOAD DAILY SALES
        //=============================
        public void loadDailySale()
        {
            try
            {
                string query =
                    "SELECT ISNULL(SUM(c.ThanhTien),0) AS total " +
                    "FROM ChiTietHDB c " +
                    "JOIN HoaDonBan h ON c.IDHDB = h.IDHDB " +
                    "WHERE CAST(h.ThoiGian AS date) = CAST(@sdate AS date)";

                DataTable dt = db.GetData(query,
                    new SqlParameter("@sdate", DateTime.Today));

                double total = 0;

                if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                    total = Convert.ToDouble(dt.Rows[0][0]);

                lblDailySale.Text = total.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi loadDailySale: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            openChildForm(new ProductForm());
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            openChildForm(new CustomerForm());
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            openChildForm(new UserForm());
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            openChildForm(new Cash(this));
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
        public void AddToDailyReport(decimal TongDoanhThu)
        {
            decimal currentTongDoanhThu = 0;
            if (decimal.TryParse(lblDailySale.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out currentTongDoanhThu))
            {
                decimal newTongDoanhThu = currentTongDoanhThu + TongDoanhThu;
                //Cập nhật tổng doanh thu
                lblDailySale.Text = newTongDoanhThu.ToString("C0");
            }
        }
    }
}
