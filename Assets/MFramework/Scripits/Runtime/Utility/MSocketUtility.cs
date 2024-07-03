using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MFramework
{
    public static class MSocketUtility
    {
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }

        public static List<IPAddress> GetAllIPV4Addresses()
        {
            List<IPAddress> res = new List<IPAddress>();

            IPHostEntry hostEntry = Dns.GetHostEntry(GetHostName());
            foreach (var ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)//InterNetwork---IPV4
                {
                    res.Add(ip);
                }
            }

            if (res.Count == 0) return null;

            return res;
        }

        public static IPAddress GetDefaultNICIPV4Address()
        {
            // 获取所有网络接口信息
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // 遍历所有网络接口
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // 如果网络接口类型为以太网并且状态为操作中
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // 获取网络接口的 IP 属性
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    // 遍历网络接口的 IPv4 地址信息
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        // 如果是 IPv4 地址
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }

            return null;
        }
    }
}
