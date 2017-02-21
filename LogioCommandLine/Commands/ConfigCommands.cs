using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogioCommandLine.Commands
{
    /// <summary>
    /// Add a server information to the config
    /// </summary>
    public class AddServerCommand : ILogioCommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Config to be updated with new server information</param>
        /// <param name="name">Name of server configuration for easier referencing</param>
        /// <param name="host">IP or hostname of server</param>
        /// <param name="port">Port where the logio server is receiving commands</param>
        public AddServerCommand(LogioConfig config, string name, string host, int port)
        {
            Config = config;
            Name = name;
            Host = host;
            Port = port;
        }

        public LogioConfig Config { get; private set; }
        public string Name { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }

        public void Execute()
        {
            Config.AddServer(Name);
            Config.AddServerParam(Name, "host", Host);
            Config.AddServerParam(Name, "port", Port.ToString());
            Config.Save();
        }
    }

    public class RemoveServerCommand : ILogioCommand
    {
        public RemoveServerCommand(LogioConfig config, string name)
        {
            Config = config;
            Name = name;
        }

        public LogioConfig Config { get; private set; }
        public string Name { get; private set; }

        public void Execute()
        {
            if (string.Equals(Name, Config.GetDefaultServer(), StringComparison.OrdinalIgnoreCase))
                throw new ApplicationException("Cannot remove default server!");

            Config.RemoveServer(Name);
            Config.Save();
        }
    }

    public class UseServerCommand : ILogioCommand
    {
        public UseServerCommand(LogioConfig config, string name)
        {
            Config = config;
            Name = name;
        }

        public LogioConfig Config { get; private set; }
        public string Name { get; private set; }

        public void Execute()
        {
            if (!Config.GetServers().Contains(Name))
                throw new ApplicationException(string.Format("Invalid server [{0}]", Name));

            Config.SetDefaultServer(Name);
            Config.Save();
        }
    }

    public class ListServerCommand : ILogioCommand
    {
        public ListServerCommand(LogioConfig config)
        {
            Config = config;
        }

        public LogioConfig Config { get; private set; }
        
        public void Execute()
        {
            var defaultServer = Config.GetDefaultServer();

            foreach (var s in Config.GetServers())
            {
                if (string.Equals(s, defaultServer, StringComparison.OrdinalIgnoreCase))
                    Console.Write("*" + s);
                else
                    Console.Write(" " + s);
            }
        }
    }
}
