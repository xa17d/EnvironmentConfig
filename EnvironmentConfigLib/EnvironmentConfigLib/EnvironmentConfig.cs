using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace EnvironmentConfigLib
{
    public class EnvironmentConfig
    {
        private EnvironmentConfig(string assemblyPath, string path)
        {
            this.AssemblyPath = assemblyPath;
            this.Path = path;
        }

        public string Path { get; private set; }
        public string Folder { get { return Directory.GetParent(Path).FullName; } }
        public string AssemblyPath { get; private set; }
        public string AssemblyFolder { get { return Directory.GetParent(AssemblyPath).FullName; } }

        public string XmlGetPath(string tagName)
        {
            return ConvertPath(XmlGetString(tagName, null));
        }

        public string XmlGetString(string tagName, string defaultValue)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Path);
            foreach (XmlNode child in xml.DocumentElement.ChildNodes)
            {
                if (child.Name == tagName) { return child.InnerText; }
            }
            return defaultValue;
        }

        public static EnvironmentConfig Find()
        {
            return Find(Assembly.GetEntryAssembly());
        }

        public static EnvironmentConfig Find(Type typeFromMainAssembly)
        {
            return Find(typeFromMainAssembly.Assembly);
        }

        public static EnvironmentConfig Find(Assembly mainAssembly)
        {
            return Find(mainAssembly, "environment.config", "environment.default.config");
        }

        public static EnvironmentConfig Find(Assembly mainAssembly, string filename, string defaultFilename)
        {
            return Find(mainAssembly.Location, filename, defaultFilename);
        }

        public static EnvironmentConfig Find(string assemblyPath, string filename, string defaultFilename)
        {
            var currentFolderPath = Directory.GetParent(InternConvertPath(assemblyPath)).FullName;

            while (!string.IsNullOrEmpty(currentFolderPath))
            {
                var currentConfigPath = System.IO.Path.Combine(currentFolderPath, filename);

                if (File.Exists(currentConfigPath))
                {
                    // file found!
                    return new EnvironmentConfig(assemblyPath, currentConfigPath);
                }
                else
                {
                    // config not found, but maybe there is a default config?
                    var currentDefaultConfigPath = System.IO.Path.Combine(currentFolderPath, defaultFilename);

                    if (File.Exists(currentDefaultConfigPath))
                    {
                        // try to copy it
                        try
                        {
                            File.Copy(currentDefaultConfigPath, currentConfigPath);

                            // copy did work -> return newly copied
                            return new EnvironmentConfig(assemblyPath, currentConfigPath);
                        }
                        catch (Exception)
                        {
                            // didn't work -> just use the default config
                            return new EnvironmentConfig(assemblyPath, currentDefaultConfigPath);
                        }
                    }
                }

                // movet to parent folder
                currentFolderPath = System.IO.Directory.GetParent(currentFolderPath).FullName;
            }

            // nothing found
            return null;
        }

        public string ConvertPath(string path)
        {
            if (path == null) { return null; }

            path = InternConvertPath(path);
            path = path.Replace("{environment}", Folder);
            path = path.Replace("{assembly}", AssemblyFolder);
            return path;
        }

        public static string InternConvertPath(string path)
        {
            return path.Replace('/', System.IO.Path.DirectorySeparatorChar);
        }
    }
}
