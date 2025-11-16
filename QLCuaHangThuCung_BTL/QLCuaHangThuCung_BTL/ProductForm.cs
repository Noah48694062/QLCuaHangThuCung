//using QLCuaHangThuCung_BTL;
//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Windows.Forms;

//namespace QLCuaHangThuCung_BTL
//{
//    public partial class ProductForm : Form
//    {
//        DBConnect db = new DBConnect();

//        public ProductForm()
//        {
//            InitializeComponent();
//            LoadProduct();
//        }

//        private void btnAdd_Click(object sender, EventArgs e)
//        {
//            ProductModule module = new ProductModule(this);
//            module.ShowDialog();
//        }

//        private void txtSearch_TextChanged(object sender, EventArgs e)
//        {
//            LoadProduct();
//        }

//        public void LoadProduct()
//        {
//            dgvProduct.Rows.Clear();

//            string query =
//                "SELECT SP.IDSanPham, SP.TenSanPham, LSP.LoaiSP, SP.SoLuong, SP.GiaBan " +
//                "FROM SanPham SP " +
//                "JOIN LoaiSanPham LSP ON SP.IDLoaiSP = LSP.IDLoaiSP " +
//                "WHERE SP.TenSanPham LIKE @search";

//            DataTable dt = db.GetData(query,
//                new SqlParameter("@search", "%" + txtSearch.Text + "%"));

//            int i = 0;
//            foreach (DataRow row in dt.Rows)
//            {
//                i++;
//                dgvProduct.Rows.Add(
//                    i,
//                    row["IDSanPham"].ToString(),
//                    row["TenSanPham"].ToString(),
//                    row["LoaiSP"].ToString(),
//                    row["SoLuong"].ToString(),
//                    row["GiaBan"].ToString()
//                );
//            }
//        }

//        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            string colName = dgvProduct.Columns[e.ColumnIndex].Name;

//            if (colName == "Edit")
//            {
//                ProductModule module = new ProductModule(this);
//                module.txtID.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
//                module.txtTen.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
//                module.cbLoai.Text = dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString();
//                module.txtSoLuong.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
//                module.txtGiaBan.Text = dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString();

//                module.btnSave.Enabled = false;
//                module.btnUpdate.Enabled = true;

//                module.ShowDialog();
//            }
//            else if (colName == "Delete")
//            {
//                if (MessageBox.Show("Xóa sản phẩm này?", "Confirm",
//                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
//                {
//                    db.Execute(
//                        "DELETE FROM SanPham WHERE IDSanPham = '" +
//                        dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString() + "'"
//                    );

//                    MessageBox.Show("Đã xóa!");

//                    LoadProduct();
//                }
//            }
//        }
//    }
//}


using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLCuaHangThuCung_BTL
{
    public partial class ProductForm : Form
    {
        DBConnect db = new DBConnect();

        public ProductForm()
        {
            InitializeComponent();
            LoadProduct();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        // ============================
        // LOAD PRODUCT
        // ============================
        public void LoadProduct()
        {
            dgvProduct.Rows.Clear();

            string query = @"
                SELECT SP.IDSanPham, SP.TenSanPham, LSP.LoaiSP,
                       SP.SoLuong, SP.GiaBan, SP.GiaNhap, NSX.TenNSX
                FROM SanPham SP
                JOIN LoaiSanPham LSP ON SP.IDLoaiSP = LSP.IDLoaiSP
                join NhaSanXuat NSX ON SP.IDNSX = NSX.IDNSX
                WHERE SP.TenSanPham LIKE @search
                ORDER BY SP.IDSanPham ASC";

            DataTable dt = db.GetData(query,
                new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%")
            );

            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                i++;
                dgvProduct.Rows.Add(
                    i,
                    row["IDSanPham"].ToString(),
                    row["TenSanPham"].ToString(),
                    row["LoaiSP"].ToString(),
                    row["SoLuong"].ToString(),
                    row["GiaBan"].ToString(),
                    row["GiaNhap"].ToString(),
                    row["TenNSX"].ToString()
                );
            }
        }

        // ============================
        // CLICK EDIT / DELETE
        // ============================
        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dgvProduct_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Chống click header bị lỗi
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvProduct.Columns[e.ColumnIndex].Name;

            // ================= EDIT =================
            if (colName == "Edit")
            {
                ProductModule module = new ProductModule(this)
                {
                    btnSave = { Enabled = false },
                    btnUpdate = { Enabled = true }
                };

                module.txtID.Text = dgvProduct.Rows[e.RowIndex].Cells["IDSanPham"].Value.ToString();
                module.txtTen.Text = dgvProduct.Rows[e.RowIndex].Cells["TenSanPham"].Value.ToString();
                module.cbLoai.Text = dgvProduct.Rows[e.RowIndex].Cells["Loai"].Value.ToString();
                module.txtSoLuong.Text = dgvProduct.Rows[e.RowIndex].Cells["SoLuong"].Value.ToString();
                module.txtGiaBan.Text = dgvProduct.Rows[e.RowIndex].Cells["GiaBan"].Value.ToString();
                module.txtGiaNhap.Text = dgvProduct.Rows[e.RowIndex].Cells["GiaNhap"].Value.ToString();
                module.cbNSX.Text = dgvProduct.Rows[e.RowIndex].Cells["TenNSX"].Value.ToString();

                module.ShowDialog();
            }

            // ================= DELETE =================
            else if (colName == "Delete")
            {
                string id = dgvProduct.Rows[e.RowIndex].Cells["IDSanPham"].Value.ToString();

                if (MessageBox.Show("Xóa sản phẩm này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    string deleteQuery = "DELETE FROM SanPham WHERE IDSanPham = @id";

                    db.Execute(deleteQuery, new SqlParameter("@id", id));

                    MessageBox.Show("Đã xóa sản phẩm!", "Thông báo");

                    LoadProduct();
                }
            }
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            ProductModule module = new ProductModule(this);

            module.btnSave.Enabled = true;
            module.btnUpdate.Enabled = false;

            module.ShowDialog();
        }
    }
}
