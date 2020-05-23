using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace BTCon
{
    public partial class Form1 : Form
    {

        bool isConnected = false;
        String[] ports;
        SerialPort port;
        string folderName;
      

  




        public void WriteTofile(string puls)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;


            try
            {
                string path = folderName + @"\puls.txt";
                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(puls);

                    }
                }
                else
                {
                    //File.AppendText(path)

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(puls);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }

           


        }


        public Form1()
        {
            InitializeComponent();
            GetAvailableComPorts();// gets all com on pc


            foreach (string port in ports) // add coms ports to ui list
            {
                comboBox1.Items.Add(port);
                Console.WriteLine(port);

                if (ports[0] != null)
                {
                    comboBox1.SelectedItem = ports[0];
                }
            }
        }



      



        
        // connect to eps button
        private void ConnectButton(object sender, EventArgs e)
        {

            if (!isConnected)
            {
                connectToEsp32();
            }
            else
            {
                SetTimer();
                DisconnectToEsp32();

            }


        }


        private void SetTimer()
        {
            int interval = int.Parse(RefreshTimercomboBox2.GetItemText(RefreshTimercomboBox2.SelectedItem));
            timer1.Interval = interval;
          //  timer1.Enabled = true;

        }


        // get all present comports on pc
       private void GetAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }




        // connect to esp32 via virtual serialport
        private void connectToEsp32()
        {
            string SelectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
           int baud = int.Parse(BadRateComboBox2.GetItemText(BadRateComboBox2.SelectedItem));

            port = new SerialPort(SelectedPort, baud, Parity.None, 8, StopBits.One);
           
            port.ReadTimeout = 5000;
            port.RtsEnable = true;
            port.DtrEnable = true;
            timer1.Enabled = true;
            port.Open();
            button1.Text = "Disconnect ";
            
        }


        private void DisconnectToEsp32()
        {
            isConnected = false;
            port.Close();
            button1.Text = "Connect";
        }

        private void ReadBt()
        {
            try
            {
                label1.Text = "reading from " + comboBox1.SelectedItem;
                string incoming = port.ReadLine();

                textBox1.Text = incoming;
                WriteTofile(incoming);
            }
            catch (Exception ex)
            {
                DisconnectToEsp32();
                MessageBox.Show(ex.Message, "message",MessageBoxButtons.OK,MessageBoxIcon.Error);             
                throw;
            }    

           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReadBt();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                 folderName = folderBrowserDialog1.SelectedPath;
                label3.Text = folderName + @"\puls.txt";
            }


        }

       
    }




    
}
