namespace Core.Interfaces
{
    public interface IMainWindow
    {
        public string SaveFileDialog(string name, string ext);
        public string OpenFileDialog(string name, string ext);
    }
}
