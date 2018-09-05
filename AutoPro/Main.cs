using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSmart
{
    public partial class Main : Form
    {
        private bool _temp;
        private bool _hide = false; //Hide program
        private ConnectFactory _factory = new ConnectFactory(); //Thực hiện các câu truy vấn với database
        private string _query = String.Empty; // Lệnh truy vấn
        private int _interval = 0;
        private int _step = 0;
        private List<object> _scripts = new List<object>(); // List<Tuple<int,int>,String>)
        public static Cursor ActuallyLoadCursor(String path)
        {
            return new Cursor(LoadCursorFromFile(path));
        }
        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }
        /// <summary>
        /// Chạy script
        /// </summary>
        public void Run()
        {
            _step = 0;
            _interval = (int)(numericUpDown1.Value * 1000 + numericUpDown2.Value * 100 + numericUpDown3.Value * 10 + numericUpDown4.Value);
            foreach (var item in _scripts)
            {
                if (item is Tuple<int, int>)
                {
                    var pos = (Tuple<int, int>)item;
                    LeftMouseClick(pos.Item1, pos.Item2);
                }
                else
                {
                    SendKeys.Send(txtBarcode.Text.Trim());
                }
                Thread.Sleep(_interval);
                _step++;
            }
            if (_step == _scripts.Count)
            {
                this.Show();
                this.Select();
            }
        }
        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public void Init()
        {
            _query = "SELECT * FROM tb_Scripts"; // Lấy toàn bộ bản ghi
            //Load dữ liệu vào List
            _factory.ExecuteQuery(_query).AsEnumerable().ToList().ForEach(r =>
            {
                var cl = r["info"].ToString().Split(',');
                if (cl.Count() == 2)
                {
                    _scripts.Add(new Tuple<int, int>(Convert.ToInt32(cl[0]), Convert.ToInt32(cl[1])));
                }
                else
                {
                    _scripts.Add(cl[0]);
                }
            });
            txtCountRecord.Text = _scripts.Count.ToString();
            txtBarcode.Select();
        }

        public Main()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtX.Text = MousePosition.X.ToString();
            txtY.Text = MousePosition.Y.ToString();
        }

        private void btnGet_MouseDown(object sender, MouseEventArgs e)
        {
            _temp = true;
            Cursor.Current = ActuallyLoadCursor("TargetLock.ani");
            if (_temp) timer1.Enabled = true;
        }

        private void btnGet_MouseUp(object sender, MouseEventArgs e)
        {
            _temp = false;
            this.Cursor = Cursors.Default;
            timer1.Enabled = false;
            _scripts.Add(new Tuple<int, int>(MousePosition.X, MousePosition.Y));
            _query = String.Format("INSERT INTO tb_Scripts values ('{0},{1}')", MousePosition.X, MousePosition.Y);
            _factory.ExecuteNonQuery(_query);
            txtCountRecord.Text = _scripts.Count.ToString();
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (_hide) Hide(); else Show();
                Run();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to clear all records?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                _scripts.Clear();
                txtCountRecord.Text = _scripts.Count.ToString();
                _query = "DELETE FROM tb_Scripts";
                _factory.ExecuteNonQuery(_query);
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                if (_hide)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
                Run();
            }
        }

        private void chkSendKey_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSendKey.Checked)
            {
                if (MessageBox.Show("Are you sure to send key?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    _scripts.Add(txtBarcode.Text.Trim());
                    txtCountRecord.Text = _scripts.Count.ToString();
                    _query = "INSERT INTO tb_Scripts values ('SenKey')";
                    _factory.ExecuteNonQuery(_query);
                }
            }
        }

        private void chkHide_CheckedChanged(object sender, EventArgs e)
        {
            _hide = chkHide.Checked ? true : false;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

    }
}
