using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

/// <summary>
/// Adapted from code by Luigi Auriemma: http://aluigi.altervista.org/

public static class GameSpy
{
    public static IPEndPoint[] GetRenegadeGSAMasterServerList()
    {
        TcpClient tcp = new TcpClient("renmaster.cncirc.net", 28900);
        StringBuilder sb = new StringBuilder();
        NetworkStream ns = tcp.GetStream();

        while (tcp.Available == 0)
            Thread.Sleep(10);

        while (tcp.Available > 0)
            sb.Append(Convert.ToChar(ns.ReadByte()));

        byte[] tosend = Encoding.ASCII.GetBytes("\\gamename\\ccrenegade\\enctype\\0");
        ns.Write(tosend, 0, tosend.Length);

        List<byte> received = new List<byte>();

        while (tcp.Available == 0)
            Thread.Sleep(10);

        while (tcp.Available > 0)
            received.Add(Convert.ToByte(ns.ReadByte()));

        ns.Close();
        tcp.Close();

        return ServerListToIPEndPoints(received.ToArray()); ;
    }

    private static IPEndPoint[] ServerListToIPEndPoints(byte[] received)
    {
        // will contain byte array of 6 bytes per array
        List<byte[]> bytesList = new List<byte[]>();
        List<IPEndPoint> temp = new List<IPEndPoint>();

        int i = 0;
        while (i + 6 <= received.Length - 7) // -7 because of garbage "/final/" string at end
        {
            byte[] bytes = new byte[] { received[i], received[i + 1], received[i + 2], received[i + 3],
                received[i + 4], received[i + 5] };

            bytesList.Add(bytes);
            i += 6;
        }

        foreach (byte[] bytes in bytesList)
        {
            String IP = String.Format("{0}.{1}.{2}.{3}", bytes[0], bytes[1], bytes[2], bytes[3]);
            int port =  256 * bytes[4] + bytes[5];
            temp.Add(new IPEndPoint(IPAddress.Parse(IP), port));
        }
        return temp.ToArray();
    }

    public static Dictionary<string, string> Get_Game_Server_Data(IPEndPoint server)
    {
        var client = new UdpClient(AddressFamily.InterNetwork);

        byte[] EchoBytes = Encoding.ASCII.GetBytes("\\status\\");
        client.Send(EchoBytes, EchoBytes.Length, server);

        string Received = "";

        do
        {
            System.Net.IPEndPoint blabla = server;
            var receivedData = client.Receive(ref blabla);

            Received += Encoding.ASCII.GetString(receivedData);
        } while (client.Available > 0);

        var Dict = Parse_Server_Status_Data(Received);

        foreach (var Pair in Dict)
        {
          //  Console.WriteLine(Pair.ToString());
        }

        return Dict;
    }

    private static Dictionary<string, string> Parse_Server_Status_Data(string Data)
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
