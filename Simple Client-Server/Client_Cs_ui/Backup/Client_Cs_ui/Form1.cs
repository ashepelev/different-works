using System;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Client_Cs_ui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 port = 12345;//порт сервера
                string send_message = textBox1.Text, recv_message;//пошлем серверу
                // буффер для приема сообщений
                Byte[] recv_data = new Byte[1000];

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
                Byte[] send_data = System.Text.Encoding.ASCII.GetBytes(send_message);
                // посылаем сообщение серверу 
                stream.Write(send_data, 0, send_data.Length);

                // получаем сообщение от сервера, i - кол-во реально полученных байт
                int i = stream.Read(recv_data, 0, recv_data.Length);
                recv_message = System.Text.Encoding.ASCII.GetString(recv_data, 0, i);
                textBox2.Text = recv_message;
                
                // закрываем соединение
                stream.Close();
                client.Close();
            }
            catch (SocketException expt)
            {
                MessageBox.Show(expt.ToString(),"Error",MessageBoxButtons.OK);
            }

        }
    }
}