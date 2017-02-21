using CommandLine;
using System;
using System.Linq;

namespace LogioCommandLine.Parser
{
    public class CommandLineArgParser
    {
        private readonly Func<LogioConfig> _createConfigFn = () => null;
        private readonly Func<LogioConfig, LogioClient> _createClientFn = new Func<LogioConfig, LogioClient>(x =>
            {
                var server = x.GetDefaultServer();

                if (server == null)
                    throw new ApplicationException("Default server not specified");

                var host = x.GetServerParam(server, "host");
                var port = x.GetServerParam(server, "port");
                return new LogioTcpClient(host, int.Parse(port));
            });

        public CommandLineArgParser(Func<LogioConfig> createConfigFn) : this(createConfigFn, null) {}
        public CommandLineArgParser(Func<LogioConfig> createConfigFn, Func<LogioConfig, LogioClient> createClientFn)
        {
            _createConfigFn = createConfigFn;
            if (createClientFn != null)
                _createClientFn = createClientFn;
        }

        private T GetArgumentObj<T>(string[] args) where T : class, ICommandLineArgs, new()
        {
            var t = new T();
            if (!CommandLine.Parser.Default.ParseArguments(args, t))
                throw new ApplicationException(t.GetUsage());

            return t;
        }

        public Commands.ILogioCommand Parse(string[] args)
        {
            if (args.Length == 0)
                return null;

            var command = args[0].ToLower();
            var subArgs = args.Skip(1).ToArray();

            if (command == "add-server")
            {
                var argsObj = GetArgumentObj<AddServerArgs>(subArgs);
                if (argsObj == null)
                    return null;

                return new Commands.AddServerCommand(_createConfigFn(), argsObj.Name, argsObj.Host, argsObj.Port);
            }
            else if (command == "remove-server")
            {
                var argsObj = GetArgumentObj<RemoveServerArgs>(subArgs);
                if (argsObj == null)
                    return null;

                return new Commands.RemoveServerCommand(_createConfigFn(), argsObj.Name);
            }
            else if (command == "use-server")
            {
                var argsObj = GetArgumentObj<UseServerArgs>(subArgs);
                if (argsObj == null)
                    return null;

                return new Commands.UseServerCommand(_createConfigFn(), argsObj.Name);
            }
            else if (command == "list-server")
            {
                var argsObj = GetArgumentObj<ListServerArgs>(subArgs);
                if (argsObj == null)
                    return null;

                return new Commands.ListServerCommand(_createConfigFn());
            }
            else if (command == "send-log")
            {
                var argsObj = GetArgumentObj<SendLogArgs>(subArgs);
                if (argsObj == null)
                    return null;

                var client = _createClientFn(_createConfigFn());

                return new Commands.SendLogCommand(client, argsObj.Message, 
                    argsObj.Node ?? Environment.MachineName, 
                    argsObj.Stream ?? AppDomain.CurrentDomain.FriendlyName);
            }
            else if (command == "remove-node")
            {
                var argsObj = GetArgumentObj<RemoveNodeArgs>(subArgs);
                if (argsObj == null)
                    return null;

                var client = _createClientFn(_createConfigFn());

                return new Commands.RemoveNodeCommand(client, argsObj.Name);
            }
            else if (command == "remove-stream")
            {
                var argsObj = GetArgumentObj<RemoveStreamArgs>(subArgs);
                if (argsObj == null)
                    return null;

                var client = _createClientFn(_createConfigFn());
                return new Commands.RemoveStreamCommand(client, argsObj.Name);
            }

            throw new ApplicationException(string.Format("Unknown command [{0}]", command));
        }
    }

    public interface ICommandLineArgs
    {
        string GetUsage();
    }

    public class AddServerArgs : ICommandLineArgs
    {
        [ValueOption(0)]
        public string Name { get; set; }

        [ValueOption(1)]
        public string Host { get; set; }

        [ValueOption(2)]
        public int Port { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio add-server <name> <host> <port>" +
                   "\n";
        }
    }

    public class RemoveServerArgs : ICommandLineArgs
    {
        [ValueOption(0)]
        public string Name { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio remove-server <name>" +
                   "\n";
        }
    }

    public class UseServerArgs : ICommandLineArgs
    {
        [ValueOption(0)]
        public string Name { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio use-server <name>" +
                   "\n";
        }
    }

    public class ListServerArgs : ICommandLineArgs
    {
        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio list-server" +
                   "\n";
        }
    }

    public class SendLogArgs : ICommandLineArgs
    {
        [Option('m', "message", Required = true, HelpText = "Set log message")]
        public string Message { get; set; }

        [Option('n', "node", DefaultValue = null, HelpText = "Set the node value")]
        public string Node { get; set; }

        [Option('s', "stream", DefaultValue = null, HelpText = "Set the stream value")]
        public string Stream { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio send-log -m <message> [-n <node>] [-s <stream>]" +
                   "\n";
        }
    }

    public class RemoveNodeArgs : ICommandLineArgs
    {
        [ValueOption(0)]
        public string Name { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio remove-node <name>" +
                   "\n";
        }
    }

    public class RemoveStreamArgs : ICommandLineArgs
    {
        [ValueOption(0)]
        public string Name { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return "Usage details:\n" +
                   "\tlogio remove-stream <name>" +
                   "\n";
        }
    }
}
