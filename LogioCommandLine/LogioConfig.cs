using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogioCommandLine
{
    /*
     * [server]
     * servers = jjlogserv:27888, foobar:28777
     * 
     * [server jjlogserv]
     * host = jjlogserv
     * port = 27888
     * 
     * 
     */
    public class LogioConfig : IDisposable
    {
        private readonly Stream _stream = null;
        private readonly IniData _iniData = null;
        private const string SERVER_PREFIX = @"server\";

        public LogioConfig(string filepath)
            : this(new FileStream(filepath, FileMode.OpenOrCreate))
        {
        }

        public LogioConfig(Stream stream)
        {
            var streamIniParser = new IniParser.StreamIniDataParser();
            _stream = stream;
            _iniData = streamIniParser.ReadData(new StreamReader(_stream));
            stream.Position = 0;
        }
        
        public IEnumerable<string> GetServers() {
            return _iniData.Sections.Where(x => x.SectionName.StartsWith(SERVER_PREFIX, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.SectionName.Substring(SERVER_PREFIX.Length));
        }

        public void SetDefaultServer(string server)
        {
            _iniData.Global["server"] = server;
        }

        public string GetDefaultServer()
        {
            return _iniData.Global["server"];
        }

        public void AddServer(string server)
        {
            _iniData.Sections.AddSection(SERVER_PREFIX + server);
        }

        public void AddServerParam(string server, string key, string value)
        {
            _iniData.Sections[SERVER_PREFIX + server].AddKey(key, value);
        }

        public string GetServerParam(string server, string key)
        {
            return _iniData.Sections[SERVER_PREFIX + server][key];
        }

        public void RemoveServer(string server)
        {
            if (!_iniData.Sections.ContainsSection(SERVER_PREFIX + server))
                throw new ApplicationException(string.Format("Server [{0}] does not exist!", server));
            _iniData.Sections.RemoveSection(SERVER_PREFIX + server);
        }

        public void Save()
        {
            var streamIni = new IniParser.StreamIniDataParser();
            using (var writer = new StreamWriter(_stream))
            {
                streamIni.WriteData(writer, _iniData);
                _stream.Flush();
                _stream.Position = 0;
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
