using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

namespace Lab11
{
    public partial class Form1 : Form
    {

        Boolean isFirst = false;
        Boolean wantCheck = false;
        Boolean wantMsg = false;
        Boolean emptyToken = false;
        Boolean stopServer = false;
        Boolean secondMemberConnected = false;
        Boolean circleChecked = false;
        Boolean sendFirstToken = false;
        Thread thread2 = null;
        Thread thread3 = null;
        List<Int32> circlePorts = new List<Int32>();
        List<Int32> circleNum = new List<Int32>();
        int myNumber;
        Int32 nextPort;
        Int32 prevPort = 0;
        Int32 myport;
        public int turnOverCount = 0;
        int waitIter = 0;
        //   int time = 0;
        NetworkStream next = null;
        NetworkStream prev = null;
        TcpClient nextTcp = null;
        TcpClient prevTcp = null;
        int currentMsgId = -1;

        // Объявление делегатов
        private delegate void TypeAddTextDelegate(string text);
        private delegate string TypeGetTextDelegate();
        private delegate void TypeSet(Int32 x);
        private delegate void TypeClearContainers();
        private delegate void TypeAddToContainers(int port, int num);
        private delegate void TypeGetComboBoxItem();

        public Form1()
        {
            InitializeComponent();
        }

        // Функция проверяет доступность порта в системе
        private bool portAvailable(Int32 port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                    return false;
            }
            return true;
        }

        // Основная функция потока
        public void worker()
        {
            try
            {
                Int32 connect_port = getPort(); // Получаем порт, к которому подключаться из textBox'а
                nextPort = connect_port;
                myport = int.Parse(textBox1.Text); // Получаем порт, на котором мы будем слушать из textBox'а
                // Объявляем необходимые делегаты и связываем их с функциями
                TypeAddTextDelegate AddTextDelegate = new TypeAddTextDelegate(addText);
                TypeGetTextDelegate GetTextDelegate = new TypeGetTextDelegate(getMsg);
                TypeSet setPortDelegate = new TypeSet(setPort);
                // Стартуем сервер на узле
                TcpListener server = null;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Byte[] bytes = new Byte[1000];
                server = new TcpListener(localAddr, myport);
                server.Start();
                Invoke(AddTextDelegate, String.Format("Starting server at localhost:{0}\n", myport));
                // Проверяем являемся ли мы первым узлом в кольце
                isFirst = myport == connect_port;
                Invoke(AddTextDelegate, "Connecting port: " + connect_port + "\n");
                // Подключаемся к указанному порту
                try
                {
                    nextTcp = new TcpClient();
                    nextTcp.Connect("localhost", nextPort);
                }
                // Если неудача - то ошибка
                catch (SocketException x)
                {
                    Invoke(AddTextDelegate, "This computer is first in the circle!\n");
                    return;
                }
                int i = 0;
                Byte[] recv_data = null, send_data = null;
                next = nextTcp.GetStream(); // Получаем доступ к сетевому потоку из существующего подключения
                // Если мы не первые в кольце...
                if (!isFirst)
                {
                    secondMemberConnected = true;
                    // Отправляем сообщение JOIN существущему узлу
                    string joinMsg = "JOIN. My port is " + myport;
                    send_data = System.Text.Encoding.Default.GetBytes(joinMsg);
                    Invoke(AddTextDelegate, "Sending: JOIN...\n");
                    next.Write(send_data, 0, send_data.Length);
                    recv_data = new Byte[1000];
                    Invoke(AddTextDelegate, "Recieving answer...\n");
                    // Получаем ответ от существующего узла
                    i = next.Read(recv_data, 0, recv_data.Length);
                    string recv_message = System.Text.Encoding.ASCII.GetString(recv_data, 0, i);
                    // В сообщении мы получим наш ID в кольце
                    myNumber = int.Parse(recv_message);
                }
                else
                {
                    if (!secondMemberConnected)
                        secondMemberConnected = true;
                }
                Invoke(AddTextDelegate, String.Format("The has  No{0} in the circle\n", myNumber));
                // Сервер уже стартовал, мы принимаем предыдущий узел
                prevTcp = server.AcceptTcpClient();
                System.Net.IPEndPoint acceptIPEndPoint = (IPEndPoint)prevTcp.Client.RemoteEndPoint;
                prevPort = acceptIPEndPoint.Port;
                Invoke(AddTextDelegate, "Accepted TcpClient!\n");
                prev = prevTcp.GetStream();
                Invoke(AddTextDelegate, "Getted Stream!\n");
                // Это конец включение узла в кольцо
                // Отправляем первый маркер, мы его потом "съедим", если мы не первый узел
                sendNextHop("MARKER");
                Invoke(AddTextDelegate, "Timer started!\n");
                while (true)
                {
                    // Если к нам подключается наш стандартный предыдущий узел
                    if (prev.DataAvailable)
                    {
                        i = prev.Read(bytes, 0, bytes.Length);
                        string recv_message = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        obtainRequest(recv_message, prev);
                        if (emptyToken)
                        {
                            if (wantCheck)
                            {
                                wantCheck = false;
                                makeCheck();
                            }
                        }
                    }
                      
                    // Новый узел подключается к нам
                    if (server.Pending())
                    {
                        Invoke(AddTextDelegate, "Client Connected!\n");
                        // Создаем для него новой подключение
                        TcpClient newPrevTcp = server.AcceptTcpClient();
                        Invoke(AddTextDelegate, "Accepted TcpClient!\n");
                        NetworkStream newPrev = newPrevTcp.GetStream();
                        Invoke(AddTextDelegate, "Getted Stream!\n");
                        // Получаем для него новый ID
                        int newId = obtainNewId();
                        // Читаем его сообщение о его порте
                        i = newPrev.Read(bytes, 0, bytes.Length);
                        // Переотправляем его сообщение нашему порту, чтоб он знал, куда подключаться в следующий раз
                        prev.Write(bytes, 0, i);                        
                        // Отправляем подключаемому узлу его ID
                        send_data = System.Text.Encoding.Default.GetBytes(newId.ToString());
                        newPrev.Write(send_data, 0, send_data.Length);
                        // Переприсваиваем потоки, теперь подключаемый узел - наш новый предыдущий
                        prevTcp = newPrevTcp;
                        prev = newPrev;
                    }
                    // Пришел Join с портом (Мы предыдущий узел и должен сделать новое соединение
                    // На следущий узел
                    if (next.DataAvailable)
                    {
                        // Читаем сообщение (Оно переотправлено от подключаемого узла)
                        i = next.Read(bytes, 0, bytes.Length);
                        string recv_message = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        String[] join_req = recv_message.Split(' ');
                        // Устанавливаем наше новое подключение к следующему узлу
                        nextPort = int.Parse(join_req[join_req.Length - 1]);
                        nextTcp = new TcpClient("localhost", nextPort);
                        next = nextTcp.GetStream();
                        Invoke(AddTextDelegate, String.Format("New next port: {0}\n", nextPort));
                    }
                    // Если надо остановить сервер
                    if (stopServer)
                    {
                        Invoke(AddTextDelegate, String.Format("Server stopped\n"));
                        server.Stop();
                        break;
                    }
                }
            }
            catch (SocketException x)
            {
                MessageBox.Show(x.ToString(), "Error " + myport, MessageBoxButtons.OK);
            }
        }

        // Программа обработчик запроса на проверку портов
        public void makeCheck()
        {
            string recv_message;
            // Проверяем, если уже кто-то есть в кольце кроме нас
            if (secondMemberConnected)
            {
                // Отправляем запрос CHECK
                sendNextHop("CHECK " + myNumber + "\n");
                Byte[] bytes = new Byte[1000];
                // Ждем от предыдущего ответа
                int i = prev.Read(bytes, 0, bytes.Length);
                recv_message = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
            }
            // Если мы единственные, то просто заполняем списки нашими данными
            else
            {
                recv_message = "CHECK 0\n";
                makePortList(recv_message);
            }            
            makePortList(recv_message);
        }

        // Программа обработчик запроса на отправку сообщения
        public void makeMsg()
        {
            TypeAddTextDelegate AddTextDelegate = new TypeAddTextDelegate(addText);
            // Проверяем доступность портов
            makeCheck();
            // И если мы не первые...
            if (secondMemberConnected)
            {
                // Если сообщение пустое
                if (textBox2.Text.Length == 0)
                {
                    Invoke(AddTextDelegate, String.Format("The message is empty"));
                    return;
                }
                // Если мы выбрали ID получателя - отправляем
                if (currentMsgId!=-1)
                    sendNextHop(String.Format("MSG {0} {1}\n", currentMsgId, textBox2.Text));
            }
            else
            {
                Invoke(AddTextDelegate, String.Format("You are the only one in the circle!\nDo you want to send message yourself?"));
            }
        }

        // Получаем выбранный элемент в comboBox
        public void getItem()
        {
            currentMsgId = (Int32)comboBox1.SelectedItem;
        }

        // Программа обработчик запроса подключения на новый узел
        public int obtainNewId()
        {
            // Отправляем CHECK
            receiveMarkerAndSendNextHop("CHECK " + myNumber + "\n");
            Byte[] bytes = new Byte[1000];
            // Получаем ответ
            int i = prev.Read(bytes, 0, bytes.Length);
            string recv_message = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
            // Строим списки ID и портов
            makePortList(recv_message);
            // Возвращаем ID для нового пользователя
            return circleNum.Count;
        }

        // Отправляет необходимое сообщение, но перед этим "убивает" "бегающий" маркер
        public void receiveMarkerAndSendNextHop(string str)
        {            
            string nothingMsg = str;
            Byte[] bytes = new Byte[1000];
            Byte[] send_data = System.Text.Encoding.Default.GetBytes(nothingMsg);
            int i = prev.Read(bytes, 0, bytes.Length);
            next.Write(send_data, 0, send_data.Length);
            ++turnOverCount;
        }

        // Отправляет необходимое сообщение следующему узлу
        public void sendNextHop(string str)
        {            
            string nothingMsg = str;
            Byte[] send_data = System.Text.Encoding.Default.GetBytes(nothingMsg);
            next.Write(send_data, 0, send_data.Length);
            ++turnOverCount;
        }

        // Отправляет необходимое сообщение предыдущему узлу
        public void sendPrevHop(string str)
        {
            string nothingMsg = str;
            Byte[] send_data = System.Text.Encoding.Default.GetBytes(nothingMsg);
            prev.Write(send_data, 0, send_data.Length);
        }

        // Программа обрабатывает сообщения, пришедшие на сервер
        public void obtainRequest(string recv_message, NetworkStream stream)
        {            
            TypeAddTextDelegate AddTextDelegate = new TypeAddTextDelegate(addText);
            // Если мы получили пустой маркер
            if (recv_message == "MARKER")
            {
                // Если нужно запустить проверку портов
                if (wantCheck)
                {
                    wantCheck = false;
                    makeCheck();
                }
                // Если мы хотим отправить сообщение
                if (wantMsg)
                {
                    wantMsg = false;
                    makeMsg();
                }
                // После всех проверок и действий, отправляем пустой маркер
                sendNextHop("MARKER");
                return;
            }
            // Парсим сообщение, находим пробелы или символы конца строки
            int first_delim = recv_message.IndexOf(" ");
            string first_part;
            if (first_delim < 0)
            {
                first_delim = recv_message.IndexOf("\n");
                if (first_delim < 0)
                    first_part = recv_message;
                else  first_part = recv_message.Substring(0, recv_message.IndexOf("\n"));
                
            }
            else
                first_part = recv_message.Substring(0, first_delim);
            switch (first_part)
            {
                    // Обработчик маркера с командой сообщения
                case "MSG":
                    // Получаем ID получателя
                    int secondDelim = recv_message.IndexOf(' ',first_delim+1);
                    int id_msg = int.Parse(recv_message.Substring(first_delim, secondDelim - first_delim));
                    // Если получатель - мы, то...
                    if (id_msg == myNumber)
                    {
                        string rcvd_msg = recv_message.Substring(secondDelim + 1);
                        Invoke(AddTextDelegate, String.Format("Recieved message: {0}\n", rcvd_msg));
                    }
                    // Иначе отправляем сообщение следущему узлу
                    else sendNextHop(recv_message);
                    break;
                case "CHECK":
                    // Если мы получили CHECK здесь, то мы не тот, кто его отправил
                    // Дописываем наши данные (порт, номер) в маркер и отправляем дальше
                    sendNextHop(recv_message + String.Format("{0} {1}\n", myport, myNumber));
                    break;
                default: break;
            }
        }

        // Стартер таймера (не используется)
        private void timerStart()
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        // Поиск свободного порта по спискам портов (не используется)
        private Int32 findPort()
        {
            Int32 max_port = 0;
            for (int i = 0; i < circlePorts.Count; i++)
                if (circlePorts.ElementAt(i) > max_port)
                    max_port = circlePorts.ElementAt(i);
            return max_port + 1;
        }

        // Обрабатывает маркер с данными о узлах сети
        private void makePortList(string message)
        {
            TypeAddTextDelegate AddTextDelegate = new TypeAddTextDelegate(addText);
            TypeClearContainers ClearContainersDelegate = new TypeClearContainers(clearContainers);
            TypeAddToContainers AddToContainersDelegate = new TypeAddToContainers(addToContainers);
            Invoke(ClearContainersDelegate);
            StringReader sr = new StringReader(message);
            // Считываем первую строку - она не нужна, в ней CHECK <ID>
            sr.ReadLine();
            string port_num;
            while ((port_num = sr.ReadLine()) != null)
            {
                // Парсим каждую строку маркера и берем из неё данные
                String[] arr_port_num = port_num.Split(' ', '\n');
                int port = int.Parse(arr_port_num[0]);
                int num = int.Parse(arr_port_num[1]);
                Invoke(AddToContainersDelegate, port, num);
            }
            Invoke(AddToContainersDelegate, myport, myNumber);
        }

        // Очищает необходимые списки (comboBox с ID, richTextBox2 с портами и ID участников
        // Списки портов и ID участников кольца)
        private void clearContainers()
        {
            comboBox1.Items.Clear();
            richTextBox2.Clear();
            circleNum.Clear();
            circlePorts.Clear();
        }

        // Добавляет данные в необходимые контейнеры
        private void addToContainers(int port, int num)
        {
            circlePorts.Add(port);
            circleNum.Add(num);
            comboBox1.Items.Add(num);
            if (num != myNumber)
                richTextBox2.AppendText(String.Format("{0} {1}\n", port, num));
            else
                richTextBox2.AppendText(String.Format("{0} {1} <-- You are here\n", port, num));
        }

        // Кнопка "Добавить в кольцо"
        private void button1_Click(object sender, EventArgs e)
        {
            // Стартуем таймер
            timer1.Start();
            richTextBox1.AppendText("Timer started!\n");
            // Стартуем поток сервер-клиент
            if ((thread2 == null) || (thread2.ThreadState == ThreadState.Stopped))
            {
                thread2 = new Thread(worker);
                thread2.IsBackground = true;
                thread2.Start();
            }
        }

        // Устанавливает порт (не используется)
        private void setPort(Int32 port)
        {
            textBox1.Text = port.ToString();
        }

        // Добавляет текст в окно информации
        private void addText(string text)
        {
            richTextBox1.AppendText(text);
        }

        // Получает сообщение, которое нужно отправить
        private string getMsg()
        {
            return textBox2.Text;
        }

        // Получает порт
        private Int32 getPort()
        {
            if (textBox3.Text.Length == 0)
            {
                MessageBox.Show("Access port is not correct", "Error", MessageBoxButtons.OK);
                return -1;
            }
            else
                try
                {
                    return Int32.Parse(textBox3.Text);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Error", MessageBoxButtons.OK);
                    return -1;
                }
        }

        // Кнопка "Проверить порты"
        private void button2_Click(object sender, EventArgs e)
        {
            wantCheck = true;
        }

        // Кнопка "Отправить" (сообщение)
        private void button3_Click(object sender, EventArgs e)
        {
            wantMsg = true;
        }

        // Обработчик тика таймера
        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox4.Text = turnOverCount.ToString();
            turnOverCount = 0;
        }

        // Кнопка "Остановить сервер"
        private void button4_Click(object sender, EventArgs e)
        {
            stopServer = true;
        }

        // Обработчик событие выбора элемента comboBox1
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TypeGetComboBoxItem GetComboBoxItem = new TypeGetComboBoxItem(getItem);
            Invoke(GetComboBoxItem);

        }
    }
}
