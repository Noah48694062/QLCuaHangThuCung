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

        public void CheckField()
        {
            if (txtTen.Text == "" | txtGiaBan.Text == "" | txtSoLuong.Text == "" | txtID.Text == "" | txtGiaNhap.Text == "")
            {
                MessageBox.Show("Required data field!", "Warning");
                return;
            }
            check = true;
        }

        #endregion Method

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
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

            MessageBox.Show("Đã thêm sản phẩm!");
            parent.LoadProduct();
            this.Close();
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
