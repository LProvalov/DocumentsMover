﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace ExcelReaderConsole
{
    public class AppSettings
    {
        private static readonly string APP_SETTINGS_PATH = "AppConfig.xml";
        private AppSettingsModel model;
        private XmlSerializer serializer;
        private bool isLoaded;

        private AppSettings()
        {
            model = new AppSettingsModel();
            serializer = new XmlSerializer(typeof(AppSettingsModel));
            isLoaded = false;
        }

        private bool DeserializeAppSettingsModel(string settingsPath)
        {
            FileInfo settingsFile = new FileInfo(settingsPath);
            if (!settingsFile.Exists) return false;
            StreamReader sr = null;
            try
            {
                using (sr = new StreamReader(settingsPath))
                {
                    model = serializer.Deserialize(sr) as AppSettingsModel;
                }
            }
            finally
            {
                if (sr != null) sr.Close();
            }
            return true;
        }
        private void SerializeAppSettingsModel(string settingsPath)
        {
            StreamWriter streamWriter = null;
            try
            {
                streamWriter = new StreamWriter(settingsPath, false);
                serializer.Serialize(streamWriter, model);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
            }
        }

        public void LoadAppSettings()
        {
            DeserializeAppSettingsModel(APP_SETTINGS_PATH);
            isLoaded = true;
        }
        public void SaveAppSettings()
        {
            SerializeAppSettingsModel(APP_SETTINGS_PATH);
        }

        private static AppSettings instance = null;
        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppSettings();
                }
                return instance;
            }
        }

        private void CheckAppSettingsLoaded()
        {
            if (!isLoaded) throw new Exception("AppSettings does not loaded.");
        }

        public string GetCardFileExtension()
        {
            CheckAppSettingsLoaded();
            return model.CardFileExtension;
        }

        public string GetExcelTemplateFilePath()
        {
            CheckAppSettingsLoaded();
            return model.ExcelTemplateFilePath;
        }

        public string ExcelTemplateFilePath
        {
            get
            {
                CheckAppSettingsLoaded();
                return model.ExcelTemplateFilePath;
            }
            set
            {
                if (File.Exists(value))
                {
                    model.ExcelTemplateFilePath = value;
                }
                else
                {
                    model.ExcelTemplateFilePath = string.Empty;
                }
            }
        }

        private string GetDirectoryPath(string path, string defaultFolder)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var dir = Utility.CheckDirectoryAndCreate(path);
                return dir.FullName;
            }
            else
            {
                string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), defaultFolder);
                var dir = Utility.CheckDirectoryAndCreate(defaultPath);
                return dir.FullName;
            }
        }

        public string GetInputDirectoryPath()
        {
            CheckAppSettingsLoaded();
            return GetDirectoryPath(model.InputDirectoryPath, "Input");
        }
        public string InputDirectoryPath
        {
            get
            {
                CheckAppSettingsLoaded();
                return GetDirectoryPath(model.InputDirectoryPath, "Input");
            }
            set
            {
                if (Directory.Exists(value))
                {
                    model.InputDirectoryPath = value;
                }
                else
                {
                    model.InputDirectoryPath = string.Empty;
                }
            }
        }

        public string GetOutputDirectoryPath()
        {
            CheckAppSettingsLoaded();
            return GetDirectoryPath(model.OutputDirectoryPath, "Output");
        }
        public string OutputDirectoryPath
        {
            get
            {
                CheckAppSettingsLoaded();
                return GetDirectoryPath(model.OutputDirectoryPath, "Output");
            }
            set
            {
                if (Directory.Exists(value))
                {
                    model.OutputDirectoryPath = value;
                }
                else
                {
                    model.OutputDirectoryPath = string.Empty;
                }
            }
        }

        public string GetEncoding()
        {
            CheckAppSettingsLoaded();
            return model.Encoding;
        }
    }
}
