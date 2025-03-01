using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class Program {
        public  static async Task Main(String[] args) {
            // узнаем системный IP
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            // создаем точку подключения
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 8081);  
            Console.WriteLine($"Слушаю запросы на {ipAddress}:8081");


            // создаем объекта сокета
            Socket socket = new Socket(
                endPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.IP
            );
            
            // подключаем к каналу
            socket.Bind(endPoint);
            // и начинаем слушать соединения
            socket.Listen();

            while(true) {
                Console.WriteLine("Жду приказов");
                
                Socket handler = await socket.AcceptAsync(); // эта строка блокирует код пока кто-нибудь не подключится к сокету

                Console.WriteLine("Accepted");
                
                // как только кто-то подключился мы можем начать обрабатывать команды от юзера
                execute(handler);
            }
        }
        public async static Task execute(Socket handler){
            Console.WriteLine("Runned");
            await Task.Run(() => handleItself(handler));
        }
        public static void handleItself(Socket handler){
            while (true) {  
                try {
                    var bytes = new Byte[1024];   // буфер под введенные данные
                    int bytesRec = handler.Receive(bytes);   // эта штука по сути аналог Console.ReadLine()
                    var input = Encoding.UTF8.GetString(bytes); // только полученные байты превращаем в строку
                    var output = $"Вы написали {input}"; // формируем ответ
                    handler.Send(Encoding.UTF8.GetBytes(output)); // посылаем ответ, преобразуя его обратно в байты
                } catch(SocketException){
                    // в случае ошибки пишу об этом
                    Console.WriteLine("Произошёл разрыв соединения");  
                    // закрываю соединения аккуратно
                    handler.Shutdown(SocketShutdown.Both); 
                    handler.Close();  
                    break; // и выхожу из цикла перехвата ввода
                }
            }
        }
    }
}