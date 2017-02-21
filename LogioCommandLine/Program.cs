using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LogioCommandLine
{
    /*
        logio add-server    [name] [host] [port]
        logio remove-server [name]
        logio use-server    [name]

        // Node defaults to current machinename, stream defaults to [logio-cli], 
        logio send-log [message]  											
        logio send-log -s [servername] -m [message] -n [node] -s [stream] 

        // Not frequently used commands
        logio remove-node   [name]
        logio remove-stream [name]
    */
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /// Example:  c:/users/foo/.logioConfig
                var configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".logioConfig");

                var parser = new Parser.CommandLineArgParser(() => new LogioConfig(configFile));
                var command = parser.Parse(args);

                command.Execute();
            }
            catch(Exception exc)
            {
                Console.WriteLine("Error: " + exc.Message);
            }
        }
    }
}
