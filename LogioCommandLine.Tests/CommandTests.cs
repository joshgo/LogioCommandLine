using LogioCommandLine.Commands;
using LogioCommandLine.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace LogioCommandLine.Tests
{
    public static class Helper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        public static bool Is(this IEnumerable<string> list, params string[] args)
        {
            return list.SequenceEqual(args, StringComparer.OrdinalIgnoreCase);
        }
    }

    public class LogioTestClient : LogioClient
    {
        public string Message { get; set; }

        protected override void Send(string message)
        {
            Message = message;
        }
    }

    public class CommandLineTests
    {
        private readonly string MACHINE_NAME = Environment.MachineName;

        private LogioConfig GetTestConfig()
        {
            var config = new LogioConfig(new MemoryStream());
            config.AddServer("server1");
            config.AddServerParam("server1", "host", "123.456.789.0");
            config.AddServerParam("server1", "port", "27888");

            config.AddServer("server2");
            config.AddServerParam("server2", "host", "123.456.789.1");
            config.AddServerParam("server2", "port", "27888");

            config.SetDefaultServer("server1");
            return config;
        }

        private LogioConfig RunAndReturnConfig(string args, Type commandType)
        {
            var config = GetTestConfig();
            var client = new LogioTestClient();

            var argsArray = Helper.CommandLineToArgs(args);

            var parser = new CommandLineArgParser(() => config, x => client);
            var command = parser.Parse(argsArray);

            Assert.NotNull(command);
            Assert.IsType(commandType, command);

            command.Execute();

            return config;
        }

        [Fact]
        public void VerifyUseServer()
        {
            var config = RunAndReturnConfig("use-server server2", typeof(UseServerCommand));
            Assert.Equal("server2", config.GetDefaultServer());
        }

        [Fact]
        public void InvalidUseServer()
        {
            Assert.ThrowsAny<Exception>(() => RunAndReturnConfig("use-server server3", typeof(UseServerCommand)));
        }

        [Fact]
        public void AddServer()
        {
            var config = RunAndReturnConfig("add-server default 127.0.0.1 28777", typeof(AddServerCommand));

            Assert.Equal("127.0.0.1", config.GetServerParam("default", "host"));
            Assert.Equal("28777", config.GetServerParam("default", "port"));
        }

        [Fact]
        public void RemoveServer()
        {
            var config = RunAndReturnConfig("remove-server server2", typeof(RemoveServerCommand));
            Assert.True(config.GetServers().Is("server1"));
        }

        [Fact]
        public void RemoveDefaultServerNotAllowed()
        {
            Assert.ThrowsAny<Exception>(() => RunAndReturnConfig("remove-server server1", typeof(RemoveServerCommand)));
        }

        [Fact]
        public void RemoveInvalidServerErrors()
        {
            Assert.ThrowsAny<Exception>(() => RunAndReturnConfig("remove-server server3", typeof(RemoveServerCommand)));
        }

        [Theory]
        [InlineData(
            "send-log -m message -n node -s stream",
            typeof(SendLogCommand),
            "+log|stream|node|info|message\r\n")]
        [InlineData(
            "remove-node node1",
            typeof(RemoveNodeCommand),
            "-node|node1\r\n")]
        [InlineData(
            "remove-stream stream12",
            typeof(RemoveStreamCommand),
            "-stream|stream12\r\n")]
        public void CommandMessage(string args, Type commandType, string message)
        {
            LogioConfig config = GetTestConfig();
            LogioTestClient client = new LogioTestClient();

            var parser = new CommandLineArgParser(() => config, x => client);
            var command = parser.Parse(Helper.CommandLineToArgs(args));

            Assert.NotNull(command);
            Assert.IsType(commandType, command);

            command.Execute();

            Assert.Equal(message, client.Message);
        }

        [Fact]
        public void SendLogWithDefaults()
        {
            var args = "send-log -m \"Hello world!!!\"";
            var commandType = typeof(SendLogCommand);
            var message = string.Format("+log|{0}|{1}|info|Hello world!!!\r\n", "Test.xUnit", Environment.MachineName);

            CommandMessage(args, commandType, message);
        }
    }
}
