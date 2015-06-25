using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Server_Cs_ui
{
    public partial class Form1 : Form
    {
        //второй, дополнительный, поток (первый - это главный, исходный, поток; ему имя не нужно - он и так есть)
        private Thread Potok2 = null; 

        public Form1()
        {
            InitializeComponent();
        }

//=================================================================================================

        public static StreamReader obtainRequest(string recMes)
        {
            Process proc = new Process(); // Создаём процесс
            proc.StartInfo.FileName = "cmd.exe"; // В качестве запускаемой программы выбираем cmd.exe
            proc.StartInfo.Arguments = "/C " + recMes; // Добавляем в качестве аргумента сообщение клиентта 
            proc.StartInfo.RedirectStandardOutput = true; // Говорим, что мы хотим перенаправить вывод
            proc.StartInfo.UseShellExecute = false; // Сообщаем, что не хотим запускать процесс в отдельной обочки
            proc.StartInfo.CreateNoWindow = true; // И что не нужно создавать отдельное окно

            proc.Start(); // Запускаем процесс

            // Функция будет возвращать выходной поток результата работы команды cmd.exe
            StreamReader resStream = proc.StandardOutput;

            return resStream;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //если запускаем в первый раз или сервер был остановлен
            if ((Potok2 == null)||(Potok2.ThreadState == System.Threading.ThreadState.Stopped)) 
            {
                Potok2 = new Thread(StartServer);
                Potok2.IsBackground = true;
                Potok2.Start();
                textBox1.Text += "Server started\r\n";
            }
            else
            {
                textBox1.Text += "Server is already working\r\n"; 
            }
        }

//=================================================================================================
        //функция, которая вызывается потоком Potok2 при старте
        public void StartServer()
        {
            TcpListener server = null;
            //создаем функцию-делегат для безопасного добавления текста в textBox1 из данного параллельного потока
            TypeAddTextDelegate AddTextDelegate = new TypeAddTextDelegate(AddText);
            try
            {
                Int32 port = 12344; //порт сервера
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");//ip-адрес сервера (интерфейс)
                String answer_message, recv_message;//пошлем клиенту, когда он подключится
                // буффер для приема сообщений
                Byte[] bytes = new Byte[1000];
                
                server = new TcpListener(localAddr, port);//выполняем bind

                // начинаем ожидание подсоединений клиентов на интерфейсе localAddr и порту port
                server.Start();

                //вместо следующего оператора попробуйте просто написать textBox1.Text += "Waiting for a connection... \r\n"
                //тогда выскочит исключение: нельзя пользоваться объектами из потока, в котором они не созданы - это небезопасно
                //выход: поручить добавление текста главному потоку, который создал textBox1 - это делает функция Invoke (поток-создатель, она определяет автоматически)
                Invoke(AddTextDelegate, "Waiting for a connection... \r\n");
                //цикл обработки подсоединений клиентов
                while (true)
                {
                    if (server.Pending())//проверяем, есть ли в очереди клиенты
                    {
                        // Ждем соединения клиента
                        TcpClient client = server.AcceptTcpClient();
                        //Ура! Кто-то подсоединился! - пишем соответствующее сообщение
                        Invoke(AddTextDelegate, "Connected!\r\n");
                        // вводим поток stream для чтения и записи через установленное соединение
                        NetworkStream stream = client.GetStream();
                        //получаем данные и записываем их в массив bytes
                        int i = stream.Read(bytes, 0, bytes.Length);
                        if (i > 0)
                        {
                            // преобразуем принятые данные в строку ASCII string.
                            recv_message = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                            //печатаем то, что получили
                            Invoke(AddTextDelegate, "Received: " + recv_message + "\r\n");
                            //анализируем запрос клиента и вычисляем результат
                            StreamReader obtResultStream = obtainRequest(recv_message);
                            // Полностью считываем данные из него
                            answer_message = obtResultStream.ReadToEnd();
                            //перед отправкой чуток подолждем - это мы симулируем задержки в сети
                            Thread.Sleep(4000);
                            //печатаем то, что будем отправлять
                            Invoke(AddTextDelegate, "Sent: " + answer_message + "\r\n");
                            //преобразуем строчку-ответ сервера в массив байт
                            byte[] answer_bytes = System.Text.Encoding.Default.GetBytes(answer_message);
                            // отправляем ответ
                            stream.Write(answer_bytes, 0, answer_bytes.Length);
                        }
                        // закрываем соединение
                        client.Close();
                        Invoke(AddTextDelegate, "Waiting for a connection... \r\n");
                    }
                    else //если желающих подсоединиться клиентов нет
                        //ждем 0.1 секунду и проверяем дальше (ждем, чтобы не загружать процессор быстрым бесконечным циклом)
                        Thread.Sleep(100);
                }
            }
            catch (SocketException expt)
            {
                //выводим окно с сообщением об ошибке
                MessageBox.Show(expt.ToString(), "Error", MessageBoxButtons.OK); 
            }
            catch (ThreadAbortException) // это исключение возникает, когда мы нажимаем на кнопку "Остановить сервер"
            {
                Invoke(AddTextDelegate, "Server stoped\r\n");
            }
            finally
            {
                // останавливаем сервер
                server.Stop();
                // заканчиваем работу потока
                Thread.ResetAbort();
            }
        }

//=================================================================================================
       
        private void button2_Click(object sender, EventArgs e)
        {
            //функция Abort выбрасывает исключение ThreadAbortException в потоке
            Potok2.Abort(); 
        }

//=================================================================================================

        //тип функции, с аргументом string, возвращающей void ( делегаты в C# заменяют указатели на функции в C++ )
        //нужен для безопасного использования textBox1 из нескольких потоков
        private delegate void TypeAddTextDelegate(string text);

//=================================================================================================

        //функция, которую главный поток будет вызывать по запросу других потоков 
        //нужна, так как только главный поток - создатель textBox1 - может вносить изменения в textBox1
        private void AddText(string text)
        {
            textBox1.Text += text;
        }
    }
}