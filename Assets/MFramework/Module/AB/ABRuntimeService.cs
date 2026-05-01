using System;
using MFramework.Core.CoreEx;

namespace MFramework
{
    public sealed class ABRuntimeService : IRuntimeService, IRuntimeUpdateService, IRuntimeLateUpdateService, IRuntimeServiceWithContext
    {
        private IDIContainer _container;
        private MResourceManager _resourceManager;

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(ABRuntimeService)} must be registered through MFrameworkCore before Initialize.");
            }

            ABRuntimeOptions options = _container.Resolve<ABRuntimeOptions>() ?? ABRuntimeOptions.CreateDefault();
            options.EnsureDefaults();
            ABRuntimeState state = _container.Resolve<ABRuntimeState>();
            state.Apply(options);
            _resourceManager = _container.Resolve<MResourceManager>();
            _resourceManager.Initialize(options.Platform ?? MABUtility.GetPlatform(), options.GetFileCallback, options.Offset);
        }

        public void Update()
        {
            _resourceManager?.Update();
        }

        public void LateUpdate()
        {
            _resourceManager?.LateUpdate();
        }

        public void Shutdown()
        {
            _resourceManager?.Shutdown();
            _resourceManager = null;
        }
    }
}

