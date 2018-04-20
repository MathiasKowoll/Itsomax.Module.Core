using System.IO;
using Itsomax.Module.Core.Interfaces;

namespace Itsomax.Module.Core.Services
{
    public class ManageFiles : IManageFiles
    {
        public string CreateFile(string path, string fileName)
        {
            if (path == null)
            {
                return null;
            }

            var file = Path.Combine(path, fileName);
            //var file = path + "/" + fileName+"."+extention;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            return file;

        }
        public void EditFile(string path, string content, string fileName)
        {
            var file = Path.Combine(path, fileName);
            CleanFile(path,fileName);
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(file)))
            {
                writer.WriteLine(content);
                writer.Dispose();
            }
        }
        public  void CleanFile(string path, string fileName)
        {
            var file = Path.Combine(path, fileName);
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
        public string GetFileContent(string path, string fileName)
        {
            var file = Path.Combine(path, fileName);
            string content;
            using (StreamReader reader = new StreamReader(File.OpenRead(file)))
            {
                content = reader.ReadToEnd();
                reader.Dispose();
            }
            return content;

        }
        public bool ExistFile(string path, string fileName)
        {
            var file = Path.Combine(path, fileName);
            try
            {
                FileStream fileStream = new FileStream(file, FileMode.Open);
                fileStream.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}