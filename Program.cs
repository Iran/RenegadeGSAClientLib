using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace GameSpyRenegadeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var Servers = GameSpy.GetMasterServerList("ccrenegade", "yctNYu1jSG8lqa", EncType.Advanced2, "");


            foreach (var server in Servers)
            {
                if (server.Address.ToString().Trim() != "69.162.65.2")
                {
                    continue;
                }

                Get_Game_Server_Data(server);
            }

            Console.Read();
        }



        static void Get_Game_Server_Data(IPEndPoint server)
        {

            Console.WriteLine("{0}", server.Address.ToString());

            var client = new UdpClient(AddressFamily.InterNetwork);


            // byte[] EchoBytes = Encoding.ASCII.GetBytes("\\echo\\1");
            byte[] EchoBytes = Encoding.ASCII.GetBytes("\\status\\");
            client.Send(EchoBytes, EchoBytes.Length, server);

            Thread.Sleep(1000);

            string Received = "";

            do
            {
                System.Net.IPEndPoint blabla = server;
                var receivedData = client.Receive(ref blabla);

                Received += Encoding.ASCII.GetString(receivedData);
                //                Console.WriteLine(Received);
            } while (client.Available > 0);

            var Dict = Parse_Server_Status_Data(Received);

            foreach (var Pair in Dict)
            {
                Console.WriteLine(Pair.ToString());
            }
        }

        static Dictionary<string, string> Parse_Server_Status_Data(string Data)
        {
            Data = Data.Substring(1);
            string[] SplitData = Data.Split(new char[] { '\\' });

            var Dict = new Dictionary<string, string>();

            string Key = null;

            foreach (string str in SplitData)
            {
                if (Key == null)
                {
                    Key = str;
                    continue;
                }
                else
                {
                    try
                    {
                        Dict.Add(Key, str);
                    }
                    catch { }

                    Key = null;
                }
            }

            return Dict;
        }
    }
}

