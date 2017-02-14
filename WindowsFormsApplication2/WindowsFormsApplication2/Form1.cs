using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        Socket skt;
        EndPoint epLocal, epRemote;

        public Form1()
        {
            InitializeComponent();
            skt = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            skt.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress,true);
            textLocalIP.Text = GetLocalIP();
            textFriendsIP.Text = GetLocalIP();


        }


        private string GetLocalIP()
        {

            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {

                    return ip.ToString();
                }

            }

            return "127.0.0.1";
        }


        private void MessageCallBack(IAsyncResult aResult)
        {

            try
            {
                int size = skt.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] reciveData = new byte[1464];
                    reciveData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string reciveMessage = eEncoding.GetString(reciveData);

                    listMessage.Items.Add("Friend: "+reciveMessage);


                }


                byte[] buffer = new byte[1500];
                skt.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }

        }

        private void start_Click(object sender, EventArgs e)
        {

            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(textLocalIP.Text), Convert.ToInt32(textLocalPort.Text));
                skt.Bind(epLocal);


                epRemote = new IPEndPoint(IPAddress.Parse(textFriendsIP.Text), Convert.ToInt32(textFriendsPort.Text));
                skt.Connect(epRemote);

                byte[] buffer = new byte[1500];
                skt.BeginReceiveFrom(buffer,0,buffer.Length,SocketFlags.None,ref epRemote,new AsyncCallback(MessageCallBack),buffer);


                start.Text = "Conected";
                start.Enabled = false;
                send.Enabled = true;
                textMessage.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void send_Click(object sender, EventArgs e)
        {


            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textMessage.Text);


                skt.Send(msg);

                listMessage.Items.Add("You: "+textMessage.Text);
                textMessage.Clear();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }
    }
}
