namespace Itsomax.Module.Core.Interfaces
{
    public interface IManageFiles
    {
        string CreateFile(string path, string fileName,string extention);
        void EditFile(string path, string content,string fileName);
        void CleanFile(string path,string fileName);
        void EmptyDir(string dir);
        string GetFileContent(string path, string fileName);
        bool ExistFile(string path, string fileName);
    }
}