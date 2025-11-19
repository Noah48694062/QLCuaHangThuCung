using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class CashProduct : Form
    {
        private Cash _cashForm;
        DBConnect db = new DBConnect();
        public CashProduct(Cash cashform)
        {
            InitializeComponent();
            _cashForm = cashform;
        }

        private void CashProduct_Load(object sender, EventArgs e)
        {
            LoadProduct();
        }
        // --- Hàm Cập nhật STT (nên được tách riêng) ---
        private void UpdateSTT()
        {
            int i = 1;
            foreach (DataGridViewRow row in dgvProduct.Rows)
            {
                if (row.IsNewRow == false)
                {
                    // Giả sử cột STT là cột index 0 (cột đầu tiên)
                    row.Cells[0].Value = i;
                    i++;
                }
            }
        }
        private void LoadProduct()
        {
            try
            {
                string query = "SELECT ChiTietHDB.IDHDB, ChiTietHDB.IDSanPham, TenSanPham , LoaiSP , SoLuongBan, DonGiaBan, TenKhachHang " +
                            "FROM ChiTietHDB " +
                            "JOIN SanPham ON ChiTietHDB.IDSanPham = SanPham.IDSanPham " +
                            "JOIN LoaiSanPham ON SanPham.IDLoaiSP = LoaiSanPham.IDLoaiSP " +
                            "JOIN HoaDonBan ON ChiTietHDB.IDHDB = HoaDonBan.IDHDB " +
                            "JOIN KhachHang ON HoaDonBan.IDKhachHang = KhachHang.IDKhachHang ";
                DataTable dt = db.GetData(query);
                dgvProduct.DataSource = dt;   
                UpdateSTT();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load dữ liệu: " + ex.Message);
            }
        }
        private void SearchProduct(string keyword)
        {
            try
            {
                // Sử dụng toán tử LIKE và dấu % để tìm kiếm chứa từ khóa
                string likeKeyword = $"%{keyword}%";

                string query = "SELECT ChiTietHDB.IDHDB, ChiTietHDB.IDSanPham, TenSanPham , LoaiSP , SoLuongBan, DonGiaBan, TenKhachHang " +
                            "FROM ChiTietHDB " +
                            "JOIN SanPham ON ChiTietHDB.IDSanPham = SanPham.IDSanPham " +
                            "JOIN LoaiSanPham ON SanPham.IDLoaiSP = LoaiSanPham.IDLoaiSP " +
                            "JOIN HoaDonBan ON ChiTietHDB.IDHDB = HoaDonBan.IDHDB " +
                            "JOIN KhachHang ON HoaDonBan.IDKhachHang = KhachHang.IDKhachHang " +
                            // Tìm kiếm theo Mã SP HOẶC Tên SP HOẶC Loại SP
                            $"WHERE ChiTietHDB.IDSanPham LIKE N'{likeKeyword}' " +
                            $"OR TenSanPham LIKE N'{likeKeyword}' " +
                            $"OR LoaiSP LIKE N'{likeKeyword}'" +
                            $"OR IDHDB LIKE N'{likeKeyword}'";

                DataTable dt = db.GetData(query);
                dgvProduct.DataSource = dt;
                UpdateSTT(); // Cập nhật lại cột STT
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if(string.IsNullOrEmpty(keyword) )
            {
                LoadProduct();
            }
            else
            {
                SearchProduct(keyword);
            }
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            // DataTable để chứa các sản phẩm được tích chọn
            DataTable selectedProducts = new DataTable();

            // Khởi tạo cấu trúc cột: IdSanPham, TenSanPham, GiaBan, SoLuong (mặc định 1)
            selectedProducts.Columns.Add("IDSanPham", typeof(string));
            selectedProducts.Columns.Add("TenSanPham", typeof(string));
            selectedProducts.Columns.Add("DonGiaBan", typeof(decimal));
            selectedProducts.Columns.Add("SoLuongBan", typeof(int));
            selectedProducts.Columns.Add("TenKhachHang", typeof(string));
            foreach (DataGridViewRow dr in dgvProduct.Rows)
            {
                // Bỏ qua hàng mới (NewRow)
                if (dr.IsNewRow) continue;

                try
                {
                    // Kiểm tra giá trị của cột Checkbox (cột "Select")
                    // Cần đảm bảo tên cột "Select" khớp với Name hoặc DataPropertyName của cột Checkbox
                    object selectValue = dr.Cells["Select"].Value;
                    bool isSelected = (selectValue != null) && Convert.ToBoolean(selectValue);

                    if (isSelected)
                    {
                        // Lấy dữ liệu từ các cột
                        string maSP = dr.Cells["IdSanPham"].Value.ToString();
                        string tenSP = dr.Cells["TenSanPham"].Value.ToString();
                        string tenKH = dr.Cells["TenKhachHang"].Value.ToString();
                        // Chuyển đổi sang decimal để tính toán chính xác
                        decimal giaBan = Convert.ToDecimal(dr.Cells["DonGiaBan"].Value);
                        int soLuong = Convert.ToInt32(dr.Cells["SoLuongBan"].Value);
                        // Thêm dòng sản phẩm đã chọn vào DataTable với số lượng 
                        selectedProducts.Rows.Add(maSP, tenSP, giaBan, soLuong, tenKH);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lấy dữ liệu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (selectedProducts.Rows.Count > 0)
            {
                _cashForm.LoadSelectedReceipt(selectedProducts);

                // Đóng form chọn sản phẩm
                this.Close();
            }
        }
    }
}
