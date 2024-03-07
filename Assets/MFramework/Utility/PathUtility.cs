using System.IO;

namespace MFramework
{
    public class PathUtility
    {
        private string GetFullPathBaseProjectRoot(string secondPath)
        {
            string fullPath = Path.GetFullPath(secondPath);
            return fullPath;
        }
    }
}