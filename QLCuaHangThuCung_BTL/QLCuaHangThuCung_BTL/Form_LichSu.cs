using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    partial class Form_LichSu : Form
    {
        DBConnect db = new DBConnect();
        private string idKhachHang;
        public Form_LichSu(string currentKhachHangID)
        {
            InitializeComponent();
            this.idKhachHang = currentKhachHangID;
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

        private void Form_LichSu_Load(object sender, EventArgs e)
        {
            LoadDanhSachDonHang();
        }

        public void LoadDanhSachDonHang()
        {
            try
            {
                // Chỉ lấy các cột quan trọng để tiết kiệm diện tích
                string query = @"
                    SELECT IDHDB, ThoiGian, TongTien, GhiChu 
                    FROM HoaDonBan 
                    WHERE IDKhachHang = @idKH 
                    ORDER BY ThoiGian DESC";

                DataTable dt = db.GetData(query, new SqlParameter("@idKH", this.idKhachHang));
                dgvDonHang.DataSource = dt;

                // Đặt tên cột
                dgvDonHang.Columns["IDHDB"].HeaderText = "Mã Đơn";
                dgvDonHang.Columns["IDHDB"].Width = 80; // Cố định chiều rộng cột Mã

                dgvDonHang.Columns["ThoiGian"].HeaderText = "Ngày Mua";
                dgvDonHang.Columns["ThoiGian"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; // Format ngày giờ gọn

                dgvDonHang.Columns["TongTien"].HeaderText = "Tổng Tiền";
                dgvDonHang.Columns["TongTien"].DefaultCellStyle.Format = "N0";

                dgvDonHang.Columns["GhiChu"].HeaderText = "TT"; // Phương thức thanh toán
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void dgvDonHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 2. Kiểm tra xem cột "IDHDB" có tồn tại không (để tránh lỗi sai tên cột)
            if (dgvDonHang.Columns["IDHDB"] == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy cột có tên 'IDHDB'. Hãy kiểm tra lại phần Design!");
                return;
            }

            // 3. Lấy giá trị ô an toàn
            var cellValue = dgvDonHang.Rows[e.RowIndex].Cells["IDHDB"].Value;

            // 4. Nếu ô đó null thì dừng lại, không làm gì cả
            if (cellValue == null) return;

            // 5. Nếu mọi thứ ổn, mới chuyển sang chuỗi và chạy tiếp
            string idHDB = cellValue.ToString();

            lblChiTiet.Text = "Chi tiết đơn: " + idHDB;
            LoadChiTietDonHang(idHDB);
        }

        private void LoadChiTietDonHang(string idHDB)
        {
            try
            {
                string query = @"
                    SELECT SP.TenSanPham, CT.SoLuongBan, CT.DonGiaBan, CT.ThanhTien
                    FROM ChiTietHDB CT
                    JOIN SanPham SP ON CT.IDSanPham = SP.IDSanPham
                    WHERE CT.IDHDB = @idHDB";

                DataTable dt = db.GetData(query, new SqlParameter("@idHDB", idHDB));
                dgvChiTiet.DataSource = dt;

                dgvChiTiet.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";

                dgvChiTiet.Columns["SoLuongBan"].HeaderText = "SL";
                dgvChiTiet.Columns["SoLuongBan"].Width = 40; // Cột SL nhỏ thôi

                dgvChiTiet.Columns["DonGiaBan"].HeaderText = "Đơn Giá";
                dgvChiTiet.Columns["DonGiaBan"].DefaultCellStyle.Format = "N0";

                dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành Tiền";
                dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}
