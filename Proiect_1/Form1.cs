using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Proiect_1
{
    public partial class Form1 : Form
    {
        private SQLiteConnection conn = null;

        private List<string> lstBlock;

        private BooleanSwitch _switch;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lstBlock = SQLiteHandler.GetAllKeywords(conn);

                string inputText = this.toolStripTextBox1.Text;
                string url;
                if (inputText.Contains(".com"))
                {
                    url = inputText;
                }
                else
                {
                    string encodedKeywords = Uri.EscapeDataString(inputText);
                    url = $"https://www.google.com/search?q={encodedKeywords}";
                }

                webBrowser1.Navigate(url);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoBack)
            {
                webBrowser1.GoBack();
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoForward)
            {
                webBrowser1.GoForward();
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            lstBlock = SQLiteHandler.GetAllKeywords(conn);
            
            if (e.KeyCode == Keys.Enter)
            {
                string inputText = this.toolStripTextBox1.Text;
                string url;
                if (inputText.Contains(".com")){
                    url = inputText;
                }
                else{
                    string encodedKeywords = Uri.EscapeDataString(inputText);
                    url = $"https://www.google.com/search?q={encodedKeywords}";
                }

                webBrowser1.Navigate(url);
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            lstBlock = SQLiteHandler.GetAllKeywords(conn);

            string url = e.Url.ToString();

            var res = Task.Run(() => AsyncCheckUrl(url));

            res.Wait();

            if (res.Result==true)
            {
                e.Cancel = true;
                MessageBox.Show("URL Blocat");
            }

        }
   
        private void conectareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStrip1.Visible == false)
            {

                MessageBox.Show("Meniul sa actualizat cu optiunile Adauga, Sterge si Deconectare.");
            }
            webBrowser1.Visible = true;
            this.conn = SQLiteHandler.SQLiteConnect();
            Console.WriteLine("Conexiune reusita");
            lstBlock = SQLiteHandler.GetAllKeywords(conn);
            toolStrip1.Visible = true;
            toolStripComboBox1.Visible = true;
            adaugareToolStripMenuItem.Visible = true;
            stergereToolStripMenuItem.Visible = true;
            deconectareToolStripMenuItem.Visible = true;
            }

        private async Task<bool> AsyncCheckUrl(string url)
        {

            FileStream file = new FileStream("Proiect1Trace.txt", FileMode.Append);
            TextWriterTraceListener textWriterTraceListener = new TextWriterTraceListener(file);

            Trace.Listeners.Add(textWriterTraceListener);
            //Debug.Listeners.Add(textWriterTraceListener);


            _switch = new BooleanSwitch("Switch1", "Trace switch");

            bool res = await Task.Run(async () =>
            {
                bool count = false;
                count = (from x in lstBlock
                             where url.Contains(x)
                             select x).Count()>0;

                if (count==true)
                {
                    Trace.WriteLine(DateTime.Now.ToString("dd-MM-yy h:mm:ss"));
                    Trace.WriteLineIf(_switch.Enabled, "URL-ul a fost blocat\n");
                    
                }

                return count;
            });


            textWriterTraceListener.Flush();
            textWriterTraceListener.Close();
            file.Close();

            return res;
        }

        private void adaugareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result;
            Form inputForm = new Form
            {
                Width = 300,
                Height = 150,
                Text = "Introduceți textul",
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox
            {
                Left = 20,
                Top = 20,
                Width = 240
            };
            inputForm.Controls.Add(textBox);
            Button okButton = new Button
            {
                Text = "OK",
                Left = 90,
                Width = 100,
                Top = 60,
                DialogResult = DialogResult.OK
            };
            inputForm.Controls.Add(okButton);
            inputForm.AcceptButton = okButton;
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                result = textBox.Text;
                SQLiteHandler.InsertKeyword(conn, result);
            }
        }

        private void stergereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result;
            Form inputForm = new Form
            {
                Width = 300,
                Height = 150,
                Text = "Introduceți textul pentru stergere",
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox
            {
                Left = 20,
                Top = 20,
                Width = 240
            };
            inputForm.Controls.Add(textBox);
            Button okButton = new Button
            {
                Text = "OK",
                Left = 90,
                Width = 100,
                Top = 60,
                DialogResult = DialogResult.OK
            };
            inputForm.Controls.Add(okButton);
            inputForm.AcceptButton = okButton;
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                result = textBox.Text;
                SQLiteHandler.DeleteKeyword(conn, result);
            }
        }

        private void deconectareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQLiteHandler.SQLiteDisconnect(conn);
            Console.WriteLine("Baza de date deconectata cu succes!");
            toolStrip1.Visible = false;
            toolStripComboBox1.Visible = false;
            adaugareToolStripMenuItem.Visible = false;
            stergereToolStripMenuItem.Visible = false;
            deconectareToolStripMenuItem.Visible = false;
            webBrowser1.Visible = false;
        }

        private void toolStripComboBox1_Click_1(object sender, EventArgs e)
        {
            lstBlock = SQLiteHandler.GetAllKeywords(conn);

            toolStripComboBox1.Items.Clear();

            foreach (var keyword in lstBlock)
            {
                toolStripComboBox1.Items.Add(keyword);
            }
        }

       
    }
}
