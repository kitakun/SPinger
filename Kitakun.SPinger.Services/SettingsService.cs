namespace Kitakun.SPinger.Services
{
    using System;
    using System.IO;

    using Kitakun.SPinger.Core.Services;

    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// File name with settings
        /// </summary>
        private readonly string _fileName = "serverslist.ini";

        public string[] Load()
        {
            var saveFilePath = GetFilePath();
            if (!File.Exists(saveFilePath))
            {
                return null;
            }

            return File.ReadAllLines(saveFilePath);
        }

        public void Save(string[] data)
        {
            var saveFilePath = GetFilePath();
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }

            File.WriteAllLines(saveFilePath, data);
        }

        private string GetFilePath()
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;

            return Path.Combine(appPath, _fileName);
        }
    }
}
