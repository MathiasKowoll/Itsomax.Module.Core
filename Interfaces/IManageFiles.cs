namespace Itsomax.Module.Core.Interfaces
{
    public interface IManageFiles
    {
        string CreateFile(string Path, string FileName,string Extention);
        void EditFile(string Path, string Content,string FileName);
        void CleanFile(string Path,string FileName);
        void EmptyDir(string dir);
        string GetFileContent(string Path, string FileName);
        bool ExistFile(string Path, string FileName);
    }
}