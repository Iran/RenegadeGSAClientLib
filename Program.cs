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
            var Servers = GameSpy.GetRenegadeGSAMasterServerList();


            foreach (var server in Servers)
            {
//                Console.WriteLine(server.Address.ToString());

                if (server.Address.ToString() == "74.91.113.100")
                    GameSpy.Get_Game_Server_Data(server);
            }

            Console.Read();
        }


    }
}

