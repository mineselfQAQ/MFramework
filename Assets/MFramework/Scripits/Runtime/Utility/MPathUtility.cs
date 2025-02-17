using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MFramework
{
    public static class MPathUtility
    {
        public static bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }
        public static bool IsFile(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获取文件夹下的所有合法文件
        /// </summary>
        public static List<string> GetFiles(string folder, string prefix, params string[] suffixes)
        {
            string[] files = Directory.GetFiles(folder, $"*.*", SearchOption.AllDirectories);
            List<string> result = new List<string>(files.Length);

            for (int i = 0; i < files.Length; ++i)
            {
                string file = files[i].ReplaceSlash();

                if (prefix != null && !file.StartsWith(prefix, StringComparison.InvariantCulture))
                {
                    continue;
                }

                if (suffixes != null && suffixes.Length > 0)
                {
                    bool exist = false;

                    for (int j = 0; j < suffixes.Length; j++)
                    {
                        string suffix = suffixes[j];
                        if (file.EndsWith(suffix, StringComparison.InvariantCulture))
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist) continue;
                }

                result.Add(file);
            }

            return result;
        }

        //TODO：如果是像AB包中的WINDOW文件怎么办
        /// <summary>
        /// 保证文件夹的创建
        /// </summary>
        public static bool CreateFolderIfNotExist(string path, bool isFile = false)
        {
            //对于文件形式回退到文件夹
            //Tip：之所以不直接用如后缀检测是因为有文件可以没有后缀
            //Tip2:之所以不直接用File.Exists()是因为这是检测文件是否存在的，这里只是提前判断文件不一定存在
            if (isFile)
            {
                path = path.CD();
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果文件已存在，就删除文件
        /// </summary>
        public static bool DeleteFileIfExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保证文件夹的最新状态(重新创建)
        /// </summary>
        /// <param playerName="path"></param>
        /// <returns>重新创建时为true，否则为false</returns>
        public static bool RecreateDirectoryIfFolderNotEmpty(string path)
        {
            if (Directory.GetFiles(path).Length != 0)
            {
                DeleteFolderFilesSurface(path);
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除文件夹中所有文件(只处理一层，包括文件夹)
        /// </summary>
        public static void DeleteFolderFilesSurface(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(folder);
        }

        /// <summary>
        /// 通过"Assets\路径"获取完整路径
        /// </summary>
        public static string GetFullPathBaseProjectRoot(string secondPath)
        {
            string fullPath = Path.GetFullPath(secondPath);
            return fullPath;
        }

        /// <summary>
        /// 获取路径下的文件名并确保带有正确后缀
        /// </summary>
        /// <returns></returns>
        public static string GetFileNameWithExtension(string path, string suffix)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (suffix[0] == '.')
            {
                name += suffix;
            }
            else
            {
                name += $".{suffix}";
            }

            return name;
        }

        public static string HTTPConvert(string url)
        {
            string newUrl = url.Replace("%", "%25").
                                Replace("+", "%2B").
                                Replace(" ", "%20").
                                Replace("#", "%23").
                                Replace("&", "%26").
                                Replace("=", "%3D");
            return newUrl;
        }

        /// <summary>
        /// 删除文件夹下所有带有某后缀的内容(默认递归)
        /// </summary>
        public static void DeleteFileWithExtension(string folder, string suffix, bool recursion = true)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            FileInfo[] files = null;
            if(recursion) files = directoryInfo.GetFiles($"*{suffix}", SearchOption.AllDirectories);
            else files = directoryInfo.GetFiles($"*{suffix}", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                file.Delete();
            }
        }

        public static bool CheckFolderHaveFile(string folder)
        {
            if (!IsFolder(folder)) 
            {
                MLog.Print($"{typeof(MPathUtility)}：传入的不是文件夹，请重试", MLogType.Warning);
                return false;
            }

            string[] files = Directory.GetFiles(folder);
            if (files.Length == 0) return false;
            return true;
        }
    }
}