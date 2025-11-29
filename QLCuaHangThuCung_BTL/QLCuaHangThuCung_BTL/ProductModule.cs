using QLCuaHangThuCung_BTL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace QLCuaHangThuCung_BTL
{
    public partial class ProductModule : Form
    {
        DBConnect db = new DBConnect();
        ProductForm parent;
        bool check = false;

        public ProductModule(ProductForm form)
        {
            InitializeComponent();
            parent = form;

            LoadLoaiSP();
            LoadNSX();
        }

        private void LoadLoaiSP()
        {
            DataTable dt = db.GetData("SELECT * FROM LoaiSanPham");
            cbLoai.DataSource = dt;
            cbLoai.DisplayMember = "LoaiSP";
            cbLoai.ValueMember = "IDLoaiSP";
        }

        private void LoadNSX()
        {
            DataTable dt = db.GetData("SELECT * FROM NhaSanXuat");
            cbNSX.DataSource = dt;
            cbNSX.DisplayMember = "TenNSX";
            cbNSX.ValueMember = "IDNSX";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //string query = @"
            //    UPDATE SanPham SET 
            //        TenSanPham=@ten,
            //        IDLoaiSP=@loai,
            //        IDNSX=@nsx,
            //        GiaNhap=@gianhap,
            //        GiaBan=@giaban,
            //        SoLuong=@soluong,
            //        MoTa=@mota
            //    WHERE IDSanPham=@id";

            //db.Execute(query,
            //    new SqlParameter("@id", txtID.Text),
            //    new SqlParameter("@ten", txtTen.Text),
            //    new SqlParameter("@loai", cbLoai.SelectedValue),
            //    new SqlParameter("@nsx", cbNSX.SelectedValue),
            //    new SqlParameter("@giaban", decimal.Parse(txtGiaBan.Text)),
            //    new SqlParameter("@soluong", int.Parse(txtSoLuong.Text))
                
            //);

            //MessageBox.Show("Đã cập nhật!");
            //parent.LoadProduct();
            //this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }


        #region Method
        public void Clear()
        {
            txtID.Clear();
            txtTen.Clear();
            txtGiaBan.Clear();
            txtGiaNhap.Clear();
            txtMoTa.Clear();
            txtSoLuong.Clear();

            if (cbLoai.Items.Count > 0)
                cbLoai.SelectedIndex = 0;

            if (cbNSX.Items.Count > 0)
                cbNSX.SelectedIndex = 0;

            btnUpdate.Enabled = false;
        }

        public bool CheckField()
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) ||
            string.IsNullOrWhiteSpace(txtTen.Text) ||
            string.IsNullOrWhiteSpace(txtGiaNhap.Text) ||
            string.IsNullOrWhiteSpace(txtGiaBan.Text) ||
            string.IsNullOrWhiteSpace(txtSoLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các thông tin bắt buộc!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Kiểm tra Combobox (Bắt buộc phải chọn)
            if (cbLoai.SelectedIndex == -1 || cbNSX.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Loại sản phẩm và Nhà sản xuất!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 3. Validate Giá Nhập (Phải là số và >= 0)
            if (!decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap) || giaNhap < 0)
            {
                MessageBox.Show("Giá nhập phải là số hợp lệ và không được âm!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaNhap.Focus();
                return false;
            }

            // 4. Validate Giá Bán (Phải là số và >= 0)
            if (!decimal.TryParse(txtGiaBan.Text, out decimal giaBan) || giaBan < 0)
            {
                MessageBox.Show("Giá bán phải là số hợp lệ và không được âm!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaBan.Focus();
                return false;
            }

            // 5. Logic kinh doanh: Giá bán không được thấp hơn giá nhập (Tùy chọn)
            if (giaBan < giaNhap)
            {
                if (MessageBox.Show("Cảnh báo: Giá bán đang thấp hơn giá nhập. Bạn có chắc chắn muốn lưu?", "Cảnh báo lỗ vốn", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    txtGiaBan.Focus();
                    return false;
                }
            }

            // 6. Validate Số Lượng (Phải là số nguyên và >= 0)
            if (!int.TryParse(txtSoLuong.Text, out int soLuong) || soLuong < 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoLuong.Focus();
                return false;
            }
            return true;
        }

        #endregion Method

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            // Nên kiểm tra dữ liệu trước khi xử lý
            if (!CheckField()) return;

            try
            {
                string query = @"
            INSERT INTO SanPham(IDSanPham, TenSanPham, IDLoaiSP, IDNSX, GiaNhap, GiaBan, SoLuong, MoTa)
            VALUES (@id, @ten, @loai, @nsx, @gianhap, @giaban, @soluong, @mota)";

                db.Execute(query,
                    new SqlParameter("@id", txtID.Text),
                    new SqlParameter("@ten", txtTen.Text),
                    new SqlParameter("@loai", cbLoai.SelectedValue),
                    new SqlParameter("@nsx", cbNSX.SelectedValue),
                    new SqlParameter("@gianhap", decimal.Parse(txtGiaNhap.Text)),
                    new SqlParameter("@giaban", decimal.Parse(txtGiaBan.Text)),
                    new SqlParameter("@soluong", int.Parse(txtSoLuong.Text)),
                    new SqlParameter("@mota", string.IsNullOrWhiteSpace(txtMoTa.Text) ? (object)DBNull.Value : txtMoTa.Text)
                );

                MessageBox.Show("Đã thêm sản phẩm!", "Thành công");
                parent.LoadProduct();
                this.Close();
            }
            // 1. Bắt lỗi SQL (trùng mã, lỗi kết nối...)
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Lỗi trùng khóa chính (Primary Key)
                {
                    MessageBox.Show("Mã sản phẩm '" + txtID.Text + "' đã tồn tại!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtID.Focus();
                    txtID.SelectAll();
                }
                else
                {
                    MessageBox.Show("Lỗi CSDL: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 2. Bắt các lỗi khác (ví dụ: nhập chữ vào ô giá tiền gây lỗi FormatException)
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            string query = @"
            UPDATE SanPham SET 
                TenSanPham=@ten,
                IDLoaiSP=@loai,
                IDNSX=@nsx,
                GiaNhap=@gianhap,
                GiaBan=@giaban,
                SoLuong=@soluong,
                MoTa=@mota
            WHERE IDSanPham=@id";

            db.Execute(query,
                new SqlParameter("@id", txtID.Text),
                new SqlParameter("@ten", txtTen.Text),
                new SqlParameter("@loai", cbLoai.SelectedValue),
                new SqlParameter("@nsx", cbNSX.SelectedValue),
                new SqlParameter("@gianhap", decimal.Parse(txtGiaNhap.Text)),
                new SqlParameter("@giaban", decimal.Parse(txtGiaBan.Text)),
                new SqlParameter("@soluong", int.Parse(txtSoLuong.Text)),
                new SqlParameter("@mota", string.IsNullOrWhiteSpace(txtMoTa.Text) ? (object)DBNull.Value : txtMoTa.Text)
            );

            MessageBox.Show("Đã cập nhật!");
            parent.LoadProduct();
            this.Close();
        }
    }
}
