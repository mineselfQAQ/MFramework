using System;
using MFramework.Core.CoreEx;
using UnityEngine;

namespace MFramework.Text
{
    public sealed class MTextRuntimeService : IRuntimeService, IRuntimeServiceWithContext
    {
        private IDIContainer _container;
        private MLocalizationManager _localizationManager;

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(MTextRuntimeService)} must be registered through MFrameworkCore before Initialize.");
            }

            _localizationManager = _container.Resolve<MLocalizationManager>();
            foreach (MText text in UnityEngine.Object.FindObjectsByType<MText>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                text.SetLocalizationManager(_localizationManager);
            }
        }

        public void Shutdown()
        {
            foreach (MText text in UnityEngine.Object.FindObjectsByType<MText>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                text.SetLocalizationManager(null);
            }

            _localizationManager = null;
            _container = null;
        }
    }
}
