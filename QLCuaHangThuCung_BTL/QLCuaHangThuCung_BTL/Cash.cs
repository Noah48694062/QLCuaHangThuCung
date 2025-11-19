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
    public partial class Cash : Form
    {
        private MainForm _mainForm;
        public Cash(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            // Gán sự kiện cho nút Cash
            this.btnCash.Click += new System.EventHandler(this.btnCash_Click);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            CashProduct product = new CashProduct(this);
            product.ShowDialog();
        }
        public void LoadSelectedReceipt(DataTable receipt)
        {
            if (dgvCash.DataSource == null)
            {
                dgvCash.DataSource = receipt;
            }
            else
            {
                DataTable currentsReceipt = (DataTable)dgvCash.DataSource;
                foreach (DataRow row in receipt.Rows)
                {
                    currentsReceipt.ImportRow(row);
                }
            }
            ProcessOrderData();
        }
        private void SetDGVColumnOrder()
        {
            try
            {
                // 1. Đặt lại cột Đơn giá (Giá)
                // Giả sử tên cột trong DataTable là "DonGiaBan"
                if (dgvCash.Columns.Contains("DonGiaBan"))
                {
                    // Ép cột "Giá" về vị trí mong muốn (Ví dụ: index 4)
                    dgvCash.Columns["DonGiaBan"].DisplayIndex = 5;
                    // Đặt HeaderText cho cột "Giá"
                    dgvCash.Columns["DonGiaBan"].HeaderText = "Giá";
                }

                // 2. Đặt lại cột Tổng (Thành tiền)
                // Giả sử tên cột trong thiết kế là "TongTien"
                if (dgvCash.Columns.Contains("TongTien"))
                {
                    // Ép cột "Tổng" về vị trí mong muốn (Ví dụ: index 5)
                    dgvCash.Columns["TongTien"].DisplayIndex = 6;
                    dgvCash.Columns["TongTien"].HeaderText = "Tổng";
                }

                // 3. Đặt lại cột Họ Tên KH
                // Giả sử tên cột trong DataTable là "TenKhachHang"
                if (dgvCash.Columns.Contains("TenKhachHang"))
                {
                    // Ép cột "Họ Tên KH" về vị trí mong muốn (Ví dụ: index 6)
                    dgvCash.Columns["TenKhachHang"].DisplayIndex = 7;
                    dgvCash.Columns["TenKhachHang"].HeaderText = "Họ Tên KH";
                }

                // 4. Đảm bảo cột Xóa/Thùng rác (nếu có) luôn ở cuối cùng
                // (Thay "ColDelete" bằng tên cột thực tế của bạn)
                if (dgvCash.Columns.Contains("Delete"))
                {
                    dgvCash.Columns["Delete"].DisplayIndex = dgvCash.Columns.Count - 1;
                }

                // Thiết lập Header Text cho các cột còn lại để đảm bảo hiển thị đúng
                if (dgvCash.Columns.Contains("IDSanPham")) dgvCash.Columns["IDSanPham"].HeaderText = "Mã SP";
                if (dgvCash.Columns.Contains("TenSanPham")) dgvCash.Columns["TenSanPham"].HeaderText = "Tên sản phẩm";
                if (dgvCash.Columns.Contains("SoLuongBan")) dgvCash.Columns["SoLuongBan"].HeaderText = "Số lượng";
            }
            catch (Exception ex)
            {
                // Có thể là do tên cột sai, kiểm tra lại!
                // MessageBox.Show("Lỗi sắp xếp cột: " + ex.Message);
            }
        }
        private void ProcessOrderData()
        {
            decimal grandTotal = 0;
            int stt = 1;

            // Duyệt qua DGV đơn hàng
            foreach (DataGridViewRow row in dgvCash.Rows)
            {
                if (row.IsNewRow) continue;

                try
                {
                    // Cột STT là cột đầu tiên (index 0)
                    row.Cells[0].Value = stt++;

                    // Lấy giá trị để tính toán
                    int soLuong = Convert.ToInt32(row.Cells["SoLuongBan"].Value);
                    decimal giaBan = Convert.ToDecimal(row.Cells["DonGiaBan"].Value);

                    // Tính Tổng tiền
                    decimal tongTienDong = soLuong * giaBan;

                    // Gán giá trị Tổng tiền (Giả sử cột có tên là "TongTien")
                    row.Cells["TongTien"].Value = tongTienDong.ToString("C0"); // Hiển thị định dạng số

                    // Cộng dồn
                    grandTotal += tongTienDong;
                }
                catch { /* Bỏ qua lỗi nếu dữ liệu chưa hợp lệ */ }
            }

            // Hiển thị Tổng cộng
            // Giả sử label Tổng tiền có tên là lblTongTien
            lblTotal.Text = grandTotal.ToString("C0");
            SetDGVColumnOrder();
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            string totalText = lblTotal.Text.Replace("₫", "").Replace("VND", "").Replace(",", "").Trim();
            decimal thanhTien = 0;
            try
            {
                if (decimal.TryParse(totalText, NumberStyles.Currency, CultureInfo.CurrentCulture, out thanhTien))
                {
                    // Kiểm tra xem có đơn hàng nào không
                    if (thanhTien > 0)
                    {
                        // 1. Cập nhật Daily Report trong MainForm
                        // Bạn cần định nghĩa phương thức AddToDailyReport trong MainForm
                        _mainForm.AddToDailyReport(thanhTien);
                        // 3. Xóa đơn hàng sau khi thanh toán xong
                        ClearOrder(); // Cần thêm phương thức ClearOrder()
                    }
                }
                else
                {
                    MessageBox.Show("Không có hóa đơn để xử lý", "Lỗi xử lý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi xử lý: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearOrder()
        {
            // Xóa dữ liệu khỏi DataGridView
            if (dgvCash.DataSource is DataTable dt)
            {
                dt.Clear();
            }
            else if (dgvCash.Rows.Count > 0)
            {
                dgvCash.Rows.Clear();
            }

            // Đặt lại tổng tiền về 0
            lblTotal.Text = 0.ToString("C0");
        }
    }
}
