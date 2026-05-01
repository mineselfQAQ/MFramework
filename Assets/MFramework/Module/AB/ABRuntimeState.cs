using System;

namespace MFramework
{
    public sealed class ABRuntimeState
    {
        public bool ABEncryptState { get; private set; }
        public bool LuaResourcesLoad { get; private set; } = true;
        public Injection[] LuaInjections { get; private set; } = Array.Empty<Injection>();

        public ABRuntimeState(ABRuntimeOptions options)
        {
            Apply(options);
        }

        public void Apply(ABRuntimeOptions options)
        {
            if (options == null) return;

            ABEncryptState = options.Encrypt;
            LuaResourcesLoad = options.LuaResourcesLoad;
            LuaInjections = options.LuaInjections ?? Array.Empty<Injection>();
        }
    }
}
