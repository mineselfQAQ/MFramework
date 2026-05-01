using System.IO;
using MFramework;
using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.Tracker;
using UnityEngine;
using Object = UnityEngine.Object;

using MFramework.Util;
namespace MFrameworkExamples.ABHotUpdate
{
    public sealed class MEntry : MEntryBase
    {
        [SerializeField] private bool encrypt;
        [SerializeField] private bool startHotUpdateOnInitialize;
        [SerializeField] private string hotUpdateUrl = "http://127.0.0.1:8888";
        [SerializeField] private bool loadAssetOnInitialize;
        [SerializeField] private string assetUrl;
        [SerializeField] private bool asyncLoad;
        [SerializeField] private bool instantiateAfterLoad;
        [SerializeField] private bool autoUnloadWhenInstantiated;

        private MHotUpdateManager _hotUpdateManager;
        private MResourceManager _resourceManager;
        private IResource _resource;
        private GameObject _instance;

        protected override IModule[] ConfigureModules()
        {
            MHotUpdateManager.url = hotUpdateUrl;

            ABRuntimeOptions options = ABRuntimeOptions.CreateDefault();
            options.Encrypt = encrypt;
            options.GetFileCallback = fileName =>
            {
                string root = MABUtility.GetABRootPath(encrypt);
                return Path.Combine(root ?? string.Empty, fileName).ReplaceSlash();
            };

            return new IModule[]
            {
                new HotUpdateModule(options, startHotUpdateOnInitialize),
            };
        }

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            _hotUpdateManager = Core.Container.Resolve<MHotUpdateManager>();
            _resourceManager = Core.Container.Resolve<MResourceManager>();
            MLog.Default.D($"ABHotUpdate example ready. Press H to hot update, L to load, U to unload.");

            if (loadAssetOnInitialize)
            {
                LoadAsset();
            }
        }

        protected override void OnUnityUpdate()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                StartHotUpdate();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadAsset();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                UnloadAsset();
            }
        }

        public void StartHotUpdate()
        {
            _hotUpdateManager?.StartHotUpdate();
        }

        public void LoadAsset()
        {
            if (_resourceManager == null || string.IsNullOrEmpty(assetUrl))
            {
                return;
            }

            UnloadAsset();

            _resource = _resourceManager.Load(assetUrl, asyncLoad);
            if (asyncLoad)
            {
                StartCoroutine(WaitAndUseResource(_resource));
                return;
            }

            UseResource(_resource);
        }

        public void UnloadAsset()
        {
            if (_instance != null)
            {
                Destroy(_instance);
                _instance = null;

                if (autoUnloadWhenInstantiated)
                {
                    _resource = null;
                    return;
                }
            }

            if (_resource == null || _resourceManager == null) return;

            _resourceManager.Unload(_resource);
            _resource = null;
        }

        private System.Collections.IEnumerator WaitAndUseResource(IResource resource)
        {
            yield return resource;

            if (_resource == resource)
            {
                UseResource(resource);
            }
        }

        private void UseResource(IResource resource)
        {
            if (!instantiateAfterLoad || resource == null) return;

            Object asset = resource.GetAsset();
            if (asset is GameObject)
            {
                _instance = resource.Instantiate(autoUnloadWhenInstantiated);
            }
        }

        protected override void OnShuttingDown(TrackerStartedEvent e)
        {
            UnloadAsset();
        }
    }
}
