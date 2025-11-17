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
    partial class Form_MuaSam : Form
    {
        private DBConnect db = new DBConnect();
        private string idKhachHang;
        

        
        public Form_MuaSam(string currentKhachHangID)
        {
            InitializeComponent();

            // Lưu ID khách hàng lại để sử dụng
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

        private void dgvMuaSam_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Bỏ qua nếu click vào header
            if (e.RowIndex < 0) return;

            // Lấy tên cột được click
            string colName = dgvMuaSam.Columns[e.ColumnIndex].Name;

            // ===================================
            // == SỬA TÊN CỘT Ở ĐÂY ==
            // ===================================
            if (colName == "ThemGioHang") // Đổi từ "ThemVaoGio" thành "ThemGioHang"
            {
                // 1. Lấy ID Sản phẩm (Giả sử bạn có cột IDSanPham, có thể ẩn đi)
                string idSanPham = dgvMuaSam.Rows[e.RowIndex].Cells["IDSanPham"].Value.ToString();
                int soLuongThem = 0;

                try
                {
                    // 2. Lấy SỐ LƯỢNG từ cột "SoLuongMua"
                    if (dgvMuaSam.Rows[e.RowIndex].Cells["SoLuongMua"].Value == null ||
                        !int.TryParse(dgvMuaSam.Rows[e.RowIndex].Cells["SoLuongMua"].Value.ToString(), out soLuongThem))
                    {
                        MessageBox.Show("Vui lòng nhập số lượng (là số) hợp lệ.", "Lỗi Số Lượng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 3. Kiểm tra số lượng > 0
                    if (soLuongThem <= 0)
                    {
                        MessageBox.Show("Số lượng thêm phải lớn hơn 0.", "Lỗi Số Lượng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 4. Gọi hàm ThemSanPhamVaoGio (Giả sử bạn có biến idKhachHang)
                    ThemSanPhamVaoGio(this.idKhachHang, idSanPham, soLuongThem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // (Bạn đặt hàm này bên trong lớp Form_MuaSam.cs)

        /// <summary>
        /// Thêm một sản phẩm vào giỏ hàng, tự động kiểm tra tồn kho và cập nhật số lượng.
        /// </summary>
        /// <param name="idKH">ID của khách hàng đang đăng nhập.</param>
        /// <param name="idSP">ID của sản phẩm cần thêm.</param>
        /// <param name="soLuongThem">Số lượng muốn thêm (đã lấy từ cột 'SoLuongMua').</param>
        private void ThemSanPhamVaoGio(string idKH, string idSP, int soLuongThem)
        {
            try
            {
                // 1. Kiểm tra tồn kho
                string queryCheckStock = "SELECT SoLuong FROM SanPham WHERE IDSanPham = @idSP";
                DataTable dtStock = db.GetData(queryCheckStock, new SqlParameter("@idSP", idSP));

                if (dtStock.Rows.Count == 0)
                {
                    MessageBox.Show("Sản phẩm này không còn tồn tại!", "Lỗi Sản Phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int tonKho = Convert.ToInt32(dtStock.Rows[0]["SoLuong"]);

                // 2. Kiểm tra xem sản phẩm đã có trong giỏ chưa
                string queryCheckCart = "SELECT SoLuong FROM GioHang WHERE IDKhachHang = @idKH AND IDSanPham = @idSP";
                SqlParameter[] checkParams = {
                    new SqlParameter("@idKH", idKH),
                    new SqlParameter("@idSP", idSP)
                };
                DataTable dtCart = db.GetData(queryCheckCart, checkParams);

                int soLuongTrongGio = 0;
                if (dtCart.Rows.Count > 0)
                {
                    soLuongTrongGio = Convert.ToInt32(dtCart.Rows[0]["SoLuong"]);
                }

                // 3. Kiểm tra xem tổng số lượng (trong giỏ + thêm mới) có vượt tồn kho không
                if (tonKho < (soLuongTrongGio + soLuongThem))
                {
                    MessageBox.Show("Sản phẩm không đủ số lượng! (Tồn kho: " + tonKho + ")", "Lỗi Tồn Kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dtCart.Rows.Count > 0)
                {
                    // 4A. Nếu ĐÃ CÓ: Cập nhật số lượng
                    string queryUpdate = "UPDATE GioHang SET SoLuong = SoLuong + @soLuongThem " +
                                         "WHERE IDKhachHang = @idKH AND IDSanPham = @idSP";
                    SqlParameter[] updateParams = {
                        new SqlParameter("@soLuongThem", soLuongThem),
                        new SqlParameter("@idKH", idKH),
                        new SqlParameter("@idSP", idSP)
                    };
                    db.Execute(queryUpdate, updateParams);
                }
                else
                {
                    // 4B. Nếu CHƯA CÓ: Thêm mới
                    string queryInsert = "INSERT INTO GioHang (IDKhachHang, IDSanPham, SoLuong) " +
                                         "VALUES (@idKH, @idSP, @soLuongThem)";
                    SqlParameter[] insertParams = {
                        new SqlParameter("@idKH", idKH),
                        new SqlParameter("@idSP", idSP),
                        new SqlParameter("@soLuongThem", soLuongThem)
                    };
                    db.Execute(queryInsert, insertParams);
                }

                MessageBox.Show("Đã thêm sản phẩm vào giỏ hàng!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vào giỏ: " + ex.Message);
            }
        }

        private void Form_MuaSam_Load(object sender, EventArgs e)
        {
            LoadSanPham();
        }

        public void LoadSanPham()
        {
            // (Giả sử bạn có TextBox 'txtSearch')
            string searchText = "%" + txtSearch.Text.Trim() + "%"; // (thay "" bằng txtSearch.Text.Trim() nếu có)

            // Câu query này lấy các cột bạn đã thiết kế (STT, Tên, Loại, Tồn kho...)
            string query = @"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY SP.TenSanPham) AS STT,
                    SP.IDSanPham, 
                    SP.TenSanPham, 
                    LSP.LoaiSP AS Loai, 
                    SP.SoLuong AS TonKho,
                    SP.GiaBan, 
                    NSX.TenNSX
                FROM SanPham SP
                JOIN LoaiSanPham LSP ON SP.IDLoaiSP = LSP.IDLoaiSP
                JOIN NhaSanXuat NSX ON SP.IDNSX = NSX.IDNSX
                WHERE SP.TenSanPham LIKE @search";

            SqlParameter[] parameters = {
                new SqlParameter("@search", searchText)
            };
            dgvMuaSam.AutoGenerateColumns = false;
            DataTable dt = db.GetData(query, parameters);

            // (Giả sử DataGridView của bạn tên là 'dgvMuaSam')
            dgvMuaSam.DataSource = dt;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadSanPham();
        }
    }
}
