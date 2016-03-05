using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KinectSetupDev
{
    public partial class Keyboard : Form
    {
        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle = createParams.ExStyle | WS_EX_NOACTIVATE;
                return createParams;
            }
        }

        public Keyboard()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void writeKey(string key)
        {
            this.textBox1.Text += key.ToLower();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            writeKey(this.button1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writeKey(this.button2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            writeKey(this.button3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            writeKey(this.button4.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            writeKey(this.button5.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            writeKey(this.button6.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            writeKey(this.button7.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            writeKey(this.button8.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            writeKey(this.button9.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            writeKey(this.button10.Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            writeKey(this.button11.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            writeKey(this.button12.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            writeKey(this.button13.Text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            writeKey(this.button14.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            writeKey(this.button15.Text);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            writeKey(this.button16.Text);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            writeKey(this.button17.Text);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            writeKey(this.button18.Text);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            writeKey(this.button19.Text);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            writeKey(this.button20.Text);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            writeKey(this.button21.Text);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            writeKey(this.button22.Text);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            writeKey(this.button23.Text);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            writeKey(this.button24.Text);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            writeKey(this.button25.Text);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            writeKey(this.button26.Text);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            writeKey(this.button27.Text);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            writeKey(this.button28.Text);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            writeKey(this.button29.Text);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            writeKey(this.button30.Text);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length != 0)
                this.textBox1.Text = this.textBox1.Text.Substring(0, this.textBox1.Text.Length - 1);
        }
        private void button32_Click(object sender, EventArgs e)
        {
            char[] input = this.textBox1.Text.ToCharArray();

            if (!(String.IsNullOrEmpty(this.textBox1.Text))) {						
                foreach (char ch in input)
                {
                    if (ch != ' ')
                        SendKeys.SendWait("{" + ch + "}");
                    else
                        SendKeys.SendWait(" ");
                }
                SendKeys.SendWait("{ENTER}");
            }
            this.Close();
        }

        private void Keyboard_Load(object sender, EventArgs e)
        {
            InputLanguage l = InputLanguage.CurrentInputLanguage;

            if (l.Culture.TwoLetterISOLanguageName.ToUpperInvariant() == "EL")
            {
                setLanguage("el");
            }
            else
            {
                setLanguage("en");
            }
            KinectSetupDev.MainWindow.keybo = true;
            this.BringToFront();
            this.Focus();
        }

        private void Keyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
                KinectSetupDev.MainWindow.keybo = false;
        }

        private void setLanguage(string lan)
        {
            if (lan.Equals("en"))
            {
                this.button1.Text = "Q";
                this.button2.Text = "W";
                this.button3.Text = "E";
                this.button4.Text = "R";
                this.button5.Text = "T";
                this.button6.Text = "Y";
                this.button7.Text = "U";
                this.button8.Text = "I";
                this.button9.Text = "O";
                this.button10.Text = "P";
                this.button11.Text = "A";
                this.button12.Text = "S";
                this.button13.Text = "D";
                this.button14.Text = "F";
                this.button15.Text = "G";
                this.button16.Text = "H";
                this.button17.Text = "J";
                this.button18.Text = "K";
                this.button19.Text = "L";
                this.button20.Text = "Z";
                this.button21.Text = "X";
                this.button22.Text = "C";
                this.button23.Text = "V";
                this.button24.Text = "B";
                this.button25.Text = "N";
                this.button26.Text = "M";
            }
            else
            {
                this.button1.Text = ";";
                this.button2.Text = "ς";
                this.button3.Text = "Ε";
                this.button4.Text = "Ρ";
                this.button5.Text = "Τ";
                this.button6.Text = "Υ";
                this.button7.Text = "Θ";
                this.button8.Text = "Ι";
                this.button9.Text = "Ο";
                this.button10.Text = "Π";
                this.button11.Text = "Α";
                this.button12.Text = "Σ";
                this.button13.Text = "Δ";
                this.button14.Text = "Φ";
                this.button15.Text = "Γ";
                this.button16.Text = "Η";
                this.button17.Text = "Ξ";
                this.button18.Text = "Κ";
                this.button19.Text = "Λ";
                this.button20.Text = "Ζ";
                this.button21.Text = "Χ";
                this.button22.Text = "Ψ";
                this.button23.Text = "Ω";
                this.button24.Text = "Β";
                this.button25.Text = "Ν";
                this.button26.Text = "Μ";
            }
        }
    }
}
