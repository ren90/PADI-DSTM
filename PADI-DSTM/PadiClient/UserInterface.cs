using System;
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

        private void UserInterface_Load(object sender, EventArgs e) { }

        private void beginTx_button_Click(object sender, EventArgs e)
        {
            this.beginTx_button.Enabled = false;
            this.abortTx_button.Enabled = true;
            this.commitTx_button.Enabled = true;
            this.statusTx_button.Enabled = true;

            if (!_client.TxBegin()) {
                this.beginTx_button.Enabled = true;
                this.abortTx_button.Enabled = false;
                this.commitTx_button.Enabled = false;
                this.statusTx_button.Enabled = false;
            }
        }

        private void serverURL_textBox_TextChanged(object sender, EventArgs e)
        {
            serverFail_button.Enabled = true;
            serverFreeze_button.Enabled = true;
			if (serverURL_textBox.Text == "")
			{
				serverFail_button.Enabled = false;
				serverFreeze_button.Enabled = false;
			}
        }

        private void statusTx_button_Click(object sender, EventArgs e)
        {
            _client.Status();
        }

        private void serverFail_button_Click(object sender, EventArgs e)
        {
			if (serverURL_textBox.Text != "")
			{
				serverRecover_button.Enabled = true;
				serverFail_button.Enabled = false;
				_client.Fail(serverURL_textBox.Text);
			}
        }

        private void serverRecover_button_Click(object sender, EventArgs e)
        {
			if (serverURL_textBox.Text != "")
			{
				serverRecover_button.Enabled = false;
				serverFail_button.Enabled = true;
				_client.Recover(serverURL_textBox.Text);
			}
        }

        private void serverFreeze_button_Click(object sender, EventArgs e)
        {
			if (serverURL_textBox.Text != "")
			{
				serverRecover_button.Enabled = true;
				serverFreeze_button.Enabled = false;
				_client.Freeze(serverURL_textBox.Text);
			}
        }

        private void createPADInt_button_Click(object sender, EventArgs e)
        {
            int uid = Convert.ToInt32(uidPADInt_textBox.Text);    
            uidPADInt_textBox.Clear();
            _client.CreatePADInt(Convert.ToInt32(uid));
            disablePADIntControl();
        }

        private void uidPADInt_textBox_TextChanged(object sender, EventArgs e)
        {
            enablePADIntControl();
            enablePADIntManipulation();
            enableValueTextBox();
        }

        private void enableValueTextBox()
        {
            valuePADInt_textBox.Enabled=true;
        }

        private void disableValueTextBox()
        {
            valuePADInt_textBox.Enabled = false;
        }

        private void enablePADIntControl()
		{
            createPADInt_button.Enabled = true;
            accessPADInt_button.Enabled = true;
        }

		private void disablePADIntControl()
		{
            createPADInt_button.Enabled = false;
            accessPADInt_button.Enabled = false;
        }

		private void enablePADIntManipulation()
		{
            readPADInt_button.Enabled = true;
            writePADInt_button.Enabled = true;
        }

		private void disablePADIntManipulation()
		{
            readPADInt_button.Enabled = false;
            writePADInt_button.Enabled = false;
        }


        //escrever nos logs

        public void LogWrite(string s)
        {
            log_textBox.AppendText(s + Environment.NewLine);
            Console.WriteLine("Write: " + s);
        }


        //escrever na lista de padi ints

        public void ListWrite(string s)
        {
            listPADInt_textBox.AppendText(s + Environment.NewLine);
            Console.WriteLine("Write: " + s);
        }

        private void accessPADInt_button_Click(object sender, EventArgs e)
        {
            _client.AccessPADInt(readFromUidTextBox());
            disablePADIntControl();
            disablePADIntManipulation();
        }

        private void readPADInt_button_Click(object sender, EventArgs e)
        {
            _client.Read(readFromUidTextBox());
            disablePADIntControl();
            disablePADIntManipulation();
        }

        private void writePADInt_button_Click(object sender, EventArgs e)
        {
            _client.Write(readFromUidTextBox(), readFromValueTextBox());
            disablePADIntControl();
            disablePADIntManipulation();
        }


        //aceder aos valores dos campos e apagar a text box

        private int readFromValueTextBox()
        {
            int value = Convert.ToInt32(valuePADInt_textBox.Text);
            valuePADInt_textBox.Clear();
            return value;
        }

        private int readFromUidTextBox()
        {
            int uid = Convert.ToInt32(uidPADInt_textBox.Text);
            uidPADInt_textBox.Clear();
            return uid;
        }

        private void valuePADInt_textBox_TextChanged(object sender, EventArgs e)
        {
            enablePADIntManipulation();
        }

        private void transaction_Enter(object sender, EventArgs e)
        {

        }

        private void commitTx_button_Click(object sender, EventArgs e)
        {
            this.beginTx_button.Enabled = true;
            this.abortTx_button.Enabled = false;
            this.commitTx_button.Enabled = false;
            this.statusTx_button.Enabled = false;
            _client.TxCommit();
        }

        private void abortTx_button_Click(object sender, EventArgs e)
        {
            this.beginTx_button.Enabled = true;
            this.abortTx_button.Enabled = false;
            this.commitTx_button.Enabled = false;
            this.statusTx_button.Enabled = false;
            _client.TxAbort();
        }
    }
}
