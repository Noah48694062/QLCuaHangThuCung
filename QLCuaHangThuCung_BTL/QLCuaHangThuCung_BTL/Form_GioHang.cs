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
    partial class Form_GioHang : Form
    {
        DBConnect db = new DBConnect();
        private string idKhachHang;
        public Form_GioHang(string currentKhachHangID)
        {
            InitializeComponent();
            this.idKhachHang = currentKhachHangID;
        }

        //#region Assembly Attribute Accessors
        //// (Phần code Assembly này là metadata, giữ nguyên)
        //public string AssemblyTitle { get { /*...*/ } }
        //public string AssemblyVersion { get { /*...*/ } }
        //public string AssemblyDescription { get { /*...*/ } }
        //public string AssemblyProduct { get { /*...*/ } }
        //public string AssemblyCopyright { get { /*...*/ } }
        //public string AssemblyCompany { get { /*...*/ } }
        //#endregion

        private void Form_GioHang_Load(object sender, EventArgs e)
        {
            LoadGioHang();
        }

        public void LoadGioHang()
        {
            string query = @"
            SELECT 
                ROW_NUMBER() OVER (ORDER BY SP.TenSanPham) AS STT, -- Tạo cột số thứ tự
                GH.IDSanPham, 
                SP.TenSanPham, 
                SP.GiaBan, 
                GH.SoLuong,
                NSX.TenNSX,
                (SP.GiaBan * GH.SoLuong) AS ThanhTien
            FROM GioHang GH
            JOIN SanPham SP ON GH.IDSanPham = SP.IDSanPham
            JOIN NhaSanXuat NSX ON SP.IDNSX = NSX.IDNSX
            WHERE GH.IDKhachHang = @idKH";

            SqlParameter[] parameters = { new SqlParameter("@idKH", this.idKhachHang) };
            DataTable dt = db.GetData(query, parameters);

            dgvGioHang.AutoGenerateColumns = false;
            dgvGioHang.DataSource = dt;

            TinhTongTien();
        }

        private decimal TinhTongTien()
        {
            decimal tongTien = 0;
            DataTable dt = (DataTable)dgvGioHang.DataSource;

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    tongTien += Convert.ToDecimal(row["ThanhTien"]);
                }
            }

            // (Giả sử Label của bạn tên là lblTongTien)
            lblTongTien.Text = string.Format("Tổng cộng: {0:N0} đ", tongTien);
            return tongTien;
        }

        private void dgvGioHang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string colName = dgvGioHang.Columns[e.ColumnIndex].Name;

            if (colName == "XoaGioHang")
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string idSP = dgvGioHang.Rows[e.RowIndex].Cells["IDSanPham"].Value.ToString();

                    string queryDelete = "DELETE FROM GioHang WHERE IDKhachHang = @idKH AND IDSanPham = @idSP";
                    SqlParameter[] parameters = {
                        new SqlParameter("@idKH", this.idKhachHang),
                        new SqlParameter("@idSP", idSP)
                    };
                    db.Execute(queryDelete, parameters);
                    LoadGioHang(); // Tải lại giỏ hàng
                }
            }
        }

        private void dgvGioHang_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string colName = dgvGioHang.Columns[e.ColumnIndex].Name;

            if (colName == "SoLuong")
            {
                try
                {
                    string idSP = dgvGioHang.Rows[e.RowIndex].Cells["IDSanPham"].Value.ToString();
                    int soLuongMoi = Convert.ToInt32(dgvGioHang.Rows[e.RowIndex].Cells["SoLuong"].Value);

                    if (soLuongMoi <= 0)
                    {
                        MessageBox.Show("Số lượng phải lớn hơn 0. (Để xóa, hãy dùng nút Xóa).");
                        LoadGioHang(); // Hủy thay đổi
                        return;
                    }

                    // ==================================================
                    // == BỔ SUNG KIỂM TRA TỒN KHO (SỬA LỖI 1) ==
                    // ==================================================
                    string queryCheckStock = "SELECT SoLuong FROM SanPham WHERE IDSanPham = @idSP";
                    DataTable dtStock = db.GetData(queryCheckStock, new SqlParameter("@idSP", idSP));
                    int tonKho = Convert.ToInt32(dtStock.Rows[0]["SoLuong"]);

                    if (tonKho < soLuongMoi)
                    {
                        MessageBox.Show("Sản phẩm không đủ số lượng! (Tồn kho: " + tonKho + ")", "Lỗi Tồn Kho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        LoadGioHang(); // Hủy thay đổi, tải lại số lượng cũ
                        return;
                    }

                    // Cập nhật CSDL
                    string queryUpdate = "UPDATE GioHang SET SoLuong = @soLuong WHERE IDKhachHang = @idKH AND IDSanPham = @idSP";
                    SqlParameter[] parameters = {
                        new SqlParameter("@soLuong", soLuongMoi),
                        new SqlParameter("@idKH", this.idKhachHang),
                        new SqlParameter("@idSP", idSP)
                    };
                    db.Execute(queryUpdate, parameters);

                    LoadGioHang(); // Tải lại để cập nhật Thành Tiền và Tổng Tiền
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Số lượng không hợp lệ hoặc lỗi: " + ex.Message);
                    LoadGioHang(); // Hủy thay đổi
                }
            }
        }

        // ==================================================
        // == BỔ SUNG NÚT THANH TOÁN (SỬA LỖI 2) ==
        // ==================================================

        // (Giả sử nút của bạn tên là btnThanhToan)
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            dgvGioHang.EndEdit();
            decimal tongTien = TinhTongTien();

            if (tongTien <= 0)
            {
                MessageBox.Show("Giỏ hàng của bạn đang trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Mở Form Thanh Toán (đã thảo luận ở tin nhắn trước)
            // (Bạn cần tạo Form_ThanhToan.cs)
            Form_ThanhToan frmTT = new Form_ThanhToan(this.idKhachHang, tongTien);
            frmTT.ShowDialog();

            // 2. Sau khi Form Thanh Toán đóng (giả sử thanh toán thành công),
            // giỏ hàng đã bị xóa (theo logic của Form_ThanhToan), 
            // chúng ta tải lại giỏ hàng (lúc này sẽ trống)
            LoadGioHang();
        }
    }
}