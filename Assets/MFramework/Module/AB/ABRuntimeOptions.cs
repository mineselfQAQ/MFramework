using System;
using System.IO;
using UnityEngine;

using MFramework.Util;
namespace MFramework
{
    public sealed class ABRuntimeOptions
    {
        public bool Encrypt { get; set; }
        public bool LuaResourcesLoad { get; set; } = true;
        public Injection[] LuaInjections { get; set; } = Array.Empty<Injection>();
        public string Platform { get; set; }
        public ulong Offset { get; set; }
        public Func<string, string> GetFileCallback { get; set; }

        public void EnsureDefaults()
        {
            if (string.IsNullOrEmpty(Platform))
            {
                Platform = MABUtility.GetPlatform();
            }

            if (GetFileCallback == null)
            {
                GetFileCallback = file => Path.Combine(MABUtility.GetABRootPath(Encrypt) ?? string.Empty, file).ReplaceSlash();
            }
        }

        public static ABRuntimeOptions CreateDefault()
        {
            ABRuntimeOptions options = new ABRuntimeOptions();
            options.EnsureDefaults();
            return options;
        }
    }
}

