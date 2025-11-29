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
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }
        int startPoint = 0;

        

        private void Loading_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            startPoint += 4;
            guna2ProgressBar1.Value = startPoint;
            if (guna2ProgressBar1.Value == 100)
            {
                //guna2ProgressBar1.Value = 0;
                //timer1.Stop();
                //Login login = new Login();
                //login.ShowDialog();
                //this.Hide();
                timer1.Stop(); // Dừng timer

                this.Hide(); // Ẩn form Loading *trước*

                Login login = new Login();
                login.ShowDialog(); // Hiển thị form Login dưới dạng modal (chặn tương tác)

                // Sau khi form Login bị đóng (dù là đăng nhập thành công hay tắt cửa sổ),
                // code sẽ tiếp tục chạy từ đây.
                this.Close(); // Đóng form Loading hoàn toàn để kết thúc.
            }
        }
    }
}
