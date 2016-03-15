using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

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
    }
}