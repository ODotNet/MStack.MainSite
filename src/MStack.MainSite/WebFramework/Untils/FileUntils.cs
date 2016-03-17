using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

namespace MStack.MainSite.WebFramework.Untils
{
    public class FileUntils
    {
        public static void Delete(string filePath)
        {
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
        }
        public static void CreateDirIfNotExists(string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
        }
        public static string FileRootPath
        {
            get { return ConfigurationManager.AppSettings["fileRootPath"]; }
        }
        public static string FileHost
        {
            get { return ConfigurationManager.AppSettings["fileHost"]; }
        }
        public static string GetRelativePath(string absolutePath)
        {
            var result = absolutePath.Replace(FileRootPath, FileHost).Replace(@"\", "/");
            return result;
        }
        public static string GetAbsolutePath(string relativePath)
        {
            var result = relativePath.Replace(FileHost, FileRootPath).Replace("/", @"\");
            return result;
        }
        public static string SaveFile(Stream stream, string fileType, string fileName)
        {
            var savePath = string.Format("{0}\\{1}\\{2}", FileRootPath, fileType, fileName);
            CreateDirIfNotExists(Path.GetDirectoryName(savePath));
            using (var sw = new StreamWriter(savePath))
            {
                stream.CopyTo(sw.BaseStream);
                sw.Flush();
                sw.Close();
            }
            return GetRelativePath(savePath);
        }

    }
}