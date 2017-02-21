using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogioCommandLine.Commands
{
    public class SendLogCommand : ILogioCommand
    {
        private readonly LogioClient _client;

        public SendLogCommand(LogioClient client, string message, string node, string stream)
        {
            _client = client;
            Message = message;
            Node = node;
            Stream = stream;
        }

        public string Node { get; private set; }
        public string Stream { get; private set; }
        public string Message { get; private set; }

        public void Execute()
        {
            _client.SendLog(Node, Stream, Message);
        }
    }

    public class RemoveNodeCommand : ILogioCommand
    {
        private readonly LogioClient _client;

        public RemoveNodeCommand(LogioClient client, string node)
        {
            _client = client;
            Node = node;
        }

        public string Node { get; private set; }

        public void Execute()
        {
            _client.RemoveNode(Node);
        }
    }

    public class RemoveStreamCommand : ILogioCommand
    {
        private readonly LogioClient _client;

        public RemoveStreamCommand(LogioClient client, string stream)
        {
            _client = client;
            Stream = stream;
        }

        public string Stream { get; private set; }

        public void Execute()
        {
            _client.RemoveStream(Stream);
        }
    }
}
