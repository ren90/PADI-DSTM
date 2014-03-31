using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PADIClient
{
    public partial class UserInterface : Form
    {
        private Client _client;

        public UserInterface()
        {

            InitializeComponent();
            _client = new Client(new WriteDelegate(LogWrite), new WriteDelegate(ListWrite));
            
        }

        private void UserInterface_Load(object sender, EventArgs e)
        {


        }

        private void beginTx_button_Click(object sender, EventArgs e)
        {
            this.abortTx_button.Enabled = true;
            this.commitTx_button.Enabled = true;
            this.statusTx_button.Enabled = true;
        }

        private void serverURL_textBox_TextChanged(object sender, EventArgs e)
        {
            serverFail_button.Enabled = true;
            serverFreeze_button.Enabled = true;
        }

        private void serverFail_button_Click(object sender, EventArgs e)
        {
            serverRecover_button.Enabled = true;
            serverFail_button.Enabled = false;
        }

        private void serverRecover_button_Click(object sender, EventArgs e)
        {
            serverRecover_button.Enabled = false;
            serverFail_button.Enabled = true;
        }

        private void createPADInt_button_Click(object sender, EventArgs e)
        {
            int uid = Convert.ToInt32(uidPADInt_textBox.Text);    
            uidPADInt_textBox.Clear();
            _client.CreatePADInt(Convert.ToInt32(uid));
            disablePADIntCommands();
        }

        private void uidPADInt_textBox_TextChanged(object sender, EventArgs e)
        {
            createPADInt_button.Enabled = true;
            accessPADInt_button.Enabled = true;
        }

        private void disablePADIntCommands()
        {
            createPADInt_button.Enabled = false;
            accessPADInt_button.Enabled = false;
        }

        public void LogWrite(string s)
        {

            log_textBox.AppendText(s + Environment.NewLine);
            Console.WriteLine("Write: " + s);

        }

        public void ListWrite(string s)
        {

            listPADInt_textBox.AppendText(s + Environment.NewLine);
            Console.WriteLine("Write: " + s);

        }

        private void accessPADInt_button_Click(object sender, EventArgs e)
        {
            int uid = Convert.ToInt32(uidPADInt_textBox.Text);
            uidPADInt_textBox.Clear();
            _client.AccessPADInt(Convert.ToInt32(uid));
            disablePADIntCommands();
        }

    }
}
