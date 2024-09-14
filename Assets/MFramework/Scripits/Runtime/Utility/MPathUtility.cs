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

        /// <summary>
        /// 保证文件夹的创建
        /// </summary>
        public static bool CreateFolderIfNotExist(string path)
        {
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
    }
}