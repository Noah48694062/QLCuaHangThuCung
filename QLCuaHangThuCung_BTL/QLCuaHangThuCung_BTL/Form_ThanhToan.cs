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
    public partial class Form_ThanhToan : Form
    {
        DBConnect db = new DBConnect();
        private string idKhachHang;
        private decimal tongTien;

        // Hàm khởi tạo nhận dữ liệu từ Giỏ Hàng
        public Form_ThanhToan(string currentKhachHangID, decimal currentTongTien)
        {
            InitializeComponent();
            this.idKhachHang = currentKhachHangID;
            this.tongTien = currentTongTien;
        }

        private void Form_ThanhToan_Load_1(object sender, EventArgs e)
        {
            // 1. Hiển thị tổng tiền
            lblTongTien.Text = string.Format("Tổng tiền: {0:N0} đ", this.tongTien);

            // 2. Mặc định chọn "Thanh toán khi nhận hàng" (để tránh lỗi nếu khách quên chọn)
            rdbTrucTiep.Checked = true;

            // 3. Tải địa chỉ có sẵn
            LoadDiaChiMacDinh();
        }

        private void LoadDiaChiMacDinh()
        {
            try
            {
                string query = "SELECT DiaChi FROM KhachHang WHERE IDKhachHang = @idKH";
                DataTable dt = db.GetData(query, new SqlParameter("@idKH", this.idKhachHang));

                if (dt.Rows.Count > 0)
                {
                    txtDiaChiGiaoHang.Text = dt.Rows[0]["DiaChi"].ToString();
                }
            }
            catch
            {
                // Lỗi tải địa chỉ thì bỏ qua, để khách tự nhập
            }
        }

        // Hàm sinh mã tự động tăng: HDB0001, HDB0002...
        private string TaoMaHoaDonMoi()
        {
            try
            {
                // Lấy IDHDB cuối cùng (lớn nhất) trong bảng
                string query = "SELECT TOP 1 IDHDB FROM HoaDonBan ORDER BY IDHDB DESC";
                DataTable dt = db.GetData(query);

                if (dt.Rows.Count == 0)
                {
                    return "HDB0001"; // Nếu bảng trống, bắt đầu từ 0001
                }

                string lastID = dt.Rows[0]["IDHDB"].ToString(); // Ví dụ: HDB0005

                // Cắt bỏ 3 ký tự đầu ("HDB") để lấy phần số ("0005")
                string numberPart = lastID.Substring(3);

                // Chuyển sang số và cộng thêm 1
                int number = int.Parse(numberPart);
                number++;

                // Ghép lại thành chuỗi, "D4" nghĩa là đảm bảo luôn có 4 chữ số (0006)
                return "HDB" + number.ToString("D4");
            }
            catch
            {
                // Phòng trường hợp lỗi format hoặc dữ liệu cũ không đúng chuẩn
                return "HDB" + DateTime.Now.ToString("ddHHmm");
            }
        }

        // ==========================================
        // XỬ LÝ NÚT XÁC NHẬN MUA HÀNG
        // ==========================================
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đã nhập địa chỉ chưa
            if (string.IsNullOrWhiteSpace(txtDiaChiGiaoHang.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ giao hàng!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChiGiaoHang.Focus();
                return;
            }
            string diaChiGiaoHang = txtDiaChiGiaoHang.Text;
            string phuongThuc = rdbQR.Checked ? "Thanh toán Online (QR)" : "Thanh toán khi nhận hàng (COD)";

            try
            {

                string newIDHDB = TaoMaHoaDonMoi();


                string queryHDB = @"INSERT INTO HoaDonBan (IDHDB, IDKhachHang, TongTien, ThoiGian, DiaChiGiaoHang, GhiChu)
                                    VALUES (@id, @idKH, @tong, @time, @diaChi, @ghiChu)";

                db.Execute(queryHDB,
                    new SqlParameter("@id", newIDHDB),
                    new SqlParameter("@idKH", this.idKhachHang),
                    new SqlParameter("@tong", this.tongTien),
                    new SqlParameter("@time", DateTime.Now),
                    new SqlParameter("@diaChi", diaChiGiaoHang),
                    new SqlParameter("@ghiChu", phuongThuc)
                );

                
                string queryChuyenGio = @"
                    INSERT INTO ChiTietHDB (IDHDB, IDSanPham, SoLuongBan, DonGiaBan)
                    SELECT @idHDBMoi, GH.IDSanPham, GH.SoLuong, SP.GiaBan
                    FROM GioHang GH
                    JOIN SanPham SP ON GH.IDSanPham = SP.IDSanPham
                    WHERE GH.IDKhachHang = @idKH";

                db.Execute(queryChuyenGio,
                    new SqlParameter("@idHDBMoi", newIDHDB),
                    new SqlParameter("@idKH", this.idKhachHang)
                );

                // 4. TRỪ SỐ LƯỢNG TỒN KHO
                string queryTruKho = @"
                    UPDATE SanPham
                    SET SoLuong = SP.SoLuong - GH.SoLuong
                    FROM SanPham SP
                    JOIN GioHang GH ON SP.IDSanPham = GH.IDSanPham
                    WHERE GH.IDKhachHang = @idKH";
                db.Execute(queryTruKho, new SqlParameter("@idKH", this.idKhachHang));

                // 5. XÓA SẠCH GIỎ HÀNG CỦA KHÁCH
                string queryXoaGio = "DELETE FROM GioHang WHERE IDKhachHang = @idKH";
                db.Execute(queryXoaGio, new SqlParameter("@idKH", this.idKhachHang));

                // 6. Hoàn tất
                MessageBox.Show("Đặt hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Đóng form thanh toán và trả về kết quả OK để Form Giỏ Hàng biết đường load lại
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}