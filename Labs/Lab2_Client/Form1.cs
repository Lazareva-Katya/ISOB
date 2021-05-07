using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2_Client
{
    public partial class Form1 : Form
    {
        readonly Client _client;
        public Form1()
        {
            InitializeComponent();
            _client = new Client(textBox2);
            Task.Run(() => _client.Listener());
        }
        private void label1_Click(object sender, EventArgs e)
        {
            Message msg = new Message(MessageType.AS_To_C)
            {
                Data = new List<List<byte>>() { new List<byte>() { 1, 2, 23, 4 } }
            };
            textBox2.Text += msg.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _client.Register(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _client.GetRes();
        }
    }
}
