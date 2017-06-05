using System;
using System.IO;
using System.IO.Compression;
using Itsomax.Module.Core.Interfaces;

namespace Itsomax.Module.Core.Services
{
    public class ManageFiles : IManageFiles
    {
        public string CreateFile(string Path, string FileName, string Extention)
        {
            if (Path == null)
            {
                return null;
            }

            var file = Path + "/" + FileName+"."+Extention;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            return file;

        }
        public void EditFile(string Path, string Content, string FileName)
        {
            var file = Path + "/" + FileName;
            CleanFile(Path,FileName);
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(file)))
            {
                writer.WriteLine(Content);
                writer.Dispose();
            }
        }
        public  void CleanFile(string Path, string FileName)
        {
            var file = Path + "/" + FileName;
            FileStream fileStream = new FileStream(file, FileMode.Truncate);
            fileStream.Dispose();
        }
        public void EmptyDir(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}