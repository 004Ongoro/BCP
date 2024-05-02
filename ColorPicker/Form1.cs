using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ColorPicker
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Fields
        private readonly List<string> ColorList = new List<string>();
        private List<string> ColorSchemes = new List<string>();
        private string ColorListFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BestColorPicker001", "Color List.txt");
        string color;
        Color selectedColor;
        
        
 
        // =====================================================//
        //Methods
        Color GetColor(Point Location)
        {
            using(Bitmap pixelContainer = new Bitmap(1, 1))
            {
                using(Graphics graphics = Graphics.FromImage(pixelContainer))
                {
                    graphics.CopyFromScreen(Location, Point.Empty, pixelContainer.Size);
                }

                return pixelContainer.GetPixel(0, 0);
            }

            
        }

        private void BackgroundThread()
        {
            while (true)
            {
                Point cursorPosition = Cursor.Position;
                selectedColor = GetColor(cursorPosition);
                panelPreview2.BackColor = selectedColor;
                string hexValue = ColorTranslator.ToHtml(selectedColor);

                ChangeArgbValues();

                if (comboBoxConverter.SelectedIndex == 0)
                {
                    textBox1.Text = hexValue;
                }
                else if (comboBoxConverter.SelectedIndex == 1)
                {
                    textBox1.Text = selectedColor.R.ToString() + ", " + selectedColor.G.ToString() + ", " + selectedColor.B.ToString();
                }

                foreach (var listItem in listBoxColorList.Items)
                {
                    listBoxColorList.BackColor = selectedColor;
                }
            }

            
        }

        private void AddColorList()
        {
            listBoxColorList.Items.Clear();

            foreach(var colorItem in ColorList)
            {
                listBoxColorList.Items.Add(colorItem);
            }
        }

        private void FillColorSchemes()
        {
            ColorSchemes.Add("HEX");
            ColorSchemes.Add("ARBG");
        }

        private void ChangeArgbValues()
        {
            int red = selectedColor.R;
            int blue = selectedColor.B;
            int green = selectedColor.G;

             red = Convert.ToInt32(numericUpDownRed.Value);
             green = Convert.ToInt32(numericUpDownGreen.Value);
             blue = Convert.ToInt32(numericUpDownBlue.Value);

            panelPreview.BackColor = Color.FromArgb(selectedColor.A, (byte)red, (byte)green, (byte)blue);

        }

        [DllImport("User32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr intPtr, int wmsg, int wParam, int IParam);

        // ===================================================== //
        //Events
        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (File.Exists(ColorListFilePath))
            {
                ColorList.AddRange(File.ReadAllLines(ColorListFilePath));
            }

            listBoxColorList.Items.Clear();

            foreach (var colorItem in ColorList)
            {
                listBoxColorList.Items.Add(colorItem);
            }

            CheckForIllegalCrossThreadCalls = false;
            Thread thread = new Thread(BackgroundThread);
            thread.Start();

            comboBoxConverter.SelectedIndex = 0;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.ShiftKey)
            {
                color = textBox1.Text;

                ColorList.Add(color);
                AddColorList();

            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                color = textBox1.Text;

                ColorList.Add(color);
                AddColorList();

            }
        }

        

        private void comboBoxConverter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxConverter.SelectedIndex == 0)
            {
                
            }else if(comboBoxConverter.SelectedIndex == 1)
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Copy();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void numericUpDownRed_ValueChanged(object sender, EventArgs e)
        {
            ChangeArgbValues();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ColorListFilePath));
            File.WriteAllLines(ColorListFilePath, ColorList);
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorList.Clear();
            listBoxColorList.Items.Clear();
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void listBoxColorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBoxColorList.SelectedItem.ToString();
        }

        private void listBoxColorList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColorDialog color = new ColorDialog();
            if(color.ShowDialog() == DialogResult.OK)
            {
                string newColor = color.ToString();
                textBox1.Text = newColor;
            }
        }

        // ===================================================== //
    }
}
