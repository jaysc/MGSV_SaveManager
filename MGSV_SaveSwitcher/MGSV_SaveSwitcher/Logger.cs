using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLogger
{
    class Logger
    {
        string configpath;
        public Logger(string configpath)
        {
            this.configpath = configpath;
        }


        /// <summary>
        /// Log to file
        /// </summary>
        /// <param name="log"></param>
        public void LogToFile(string log)
        {
            if (!Directory.Exists(this.configpath))
            {
                Directory.CreateDirectory(this.configpath);
            }

            Directory.CreateDirectory(Path.Combine(this.configpath, "logs"));
            File.AppendAllText(Path.Combine(this.configpath, "logs", "log.txt"), $"{DateTime.Now.ToString("HH:mm:ss")} : {log}\r\n");
            if (File.ReadAllLines(Path.Combine(this.configpath, "logs", "log.txt")).Length >= 500)
            {
                File.WriteAllLines(Path.Combine(this.configpath, "logs", "log.txt"), File.ReadAllLines(Path.Combine(this.configpath, "logs", "log.txt")).Skip(1).ToArray());
            }
        }
    }
}