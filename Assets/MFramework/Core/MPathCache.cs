using System.IO;
using UnityEngine;

namespace MFramework.Core
{
    public static class MPathCache
    {
        /// <summary>
        /// PC根目录(包括Editor/Build)
        /// </summary>
        public static string PC_ROOT_PATH = Path.GetDirectoryName(Application.dataPath);
    }
}