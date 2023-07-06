using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogLibraryClassLibrary
{
    public static class Logger
    {
        private static readonly string LOGGER_FILE_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AutogestionTiendasSoporte/Logger.log";
        private static readonly string LOGGER_DIRECTORY_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AutogestionTiendasSoporte";

        public static void CreateLog()
        {
            try
            {
                if(!Directory.Exists(LOGGER_DIRECTORY_PATH))
                {
                    Directory.CreateDirectory(LOGGER_DIRECTORY_PATH);
                }

                if(!File.Exists(LOGGER_FILE_PATH))
                {
                    FileStream fs = File.Open(LOGGER_FILE_PATH, FileMode.Create);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Logger Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void WriteToLog(string message)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                using (StreamWriter w = File.AppendText(LOGGER_FILE_PATH))
                {
                    w.WriteLine(dateTime.ToString() + ": " + message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Write To Log Exception: " + ex.Message.ToLower(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool LoggerExists()
        {
            return File.Exists(LOGGER_FILE_PATH);
        }
    }
}
