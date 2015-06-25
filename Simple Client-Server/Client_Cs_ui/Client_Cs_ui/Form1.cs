using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client_Cs_ui
{
    public partial class Form1 : Form
    {
        Thread thread2 = null;
        public Form1()
        {
            InitializeComponent();
        }

        private delegate void TypeAddTextDelegate(string text);

        public void sendAndRecieve()
        {
            try
            {
                Int32 port = 12344;//порт сервера
                string send_message = textBox1.Text, recv_message;//пошлем серверу
                // буффер для приема сообщений
                Byte[] recv_data = new Byte[1000];
                TypeAddTextDelegate AddTextDelegate = new TypeAddTextDelegate(setText);
                //проверяем ввел ли пользователь хоть что-нибудь
                if (send_message.Length == 0)
                {
                    MessageBox.Show("Строка не должна быть пустой", "Error", MessageBoxButtons.OK);
                    return;
                }
                //подключаемся к серверу
                TcpClient client = new TcpClient("localhost", port);
                // вводим поток stream для чтения и записи через установленное соединение                
                NetworkStream stream = client.GetStream();

                //преобразуем строчку в массив байт
                Byte[] send_data = System.Text.Encoding.Default.GetBytes(send_message);
                // посылаем сообщение серверу 
                stream.Write(send_data, 0, send_data.Length);

                // получаем сообщение от сервера, i - кол-во реально полученных байт
                int i = stream.Read(recv_data, 0, recv_data.Length);
                recv_message = System.Text.Encoding.ASCII.GetString(recv_data, 0, i);
                Invoke(AddTextDelegate,recv_message);
                
                // закрываем соединение
                stream.Close();
                client.Close();
            }
            catch (SocketException expt)
            {
                MessageBox.Show(expt.ToString(),"Error",MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
                if ((thread2 == null) || (thread2.ThreadState == ThreadState.Stopped))
                {
                    thread2 = new Thread(sendAndRecieve);
                    thread2.IsBackground = true;
                    setText("Recieving...");
                    thread2.Start();                    
                }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void setText(string text)
        {
            richTextBox1.Text = text;
        }
    }
}