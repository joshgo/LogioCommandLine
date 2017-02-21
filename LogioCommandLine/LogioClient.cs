using System.Net.Sockets;
using System.Text;

namespace LogioCommandLine
{
    public class LogioClient
    {
        public void RemoveNode(string node)
        {
            Send("-node|" + node + "\r\n");
        }

        public void RemoveStream(string node)
        {
            Send("-stream|" + node + "\r\n");
        }

        public void SendLog(string node, string stream, string log)
        {
            var message = string.Format("+log|{0}|{1}|info|{2}\r\n", stream, node, log);
            Send(message);
        }

        protected virtual void Send(string message) { }
    }

    public class LogioTcpClient : LogioClient
    {
        private readonly string _host;
        private readonly int _port;

        public LogioTcpClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        protected override void Send(string message)
        {
            var data = Encoding.ASCII.GetBytes(message + "\r\n");

            using (var tcp = new TcpClient(_host, _port))
            {
                var stream = tcp.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
