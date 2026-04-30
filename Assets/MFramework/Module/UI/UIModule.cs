using MFramework.Core.CoreEx;
using MFramework.Coroutines;
using UnityEngine;

namespace MFramework.UI
{
    public sealed class UIModule : IModule
    {
        private readonly Canvas _canvas;
        private readonly Camera _camera;
        private readonly GameObject _eventSystem;
        private readonly IUIPrefabLoader _prefabLoader;
        private readonly string _canvasName;
        private readonly string _cameraName;

        public UIModule(
            Canvas canvas = null,
            Camera camera = null,
            GameObject eventSystem = null,
            IUIPrefabLoader prefabLoader = null,
            string canvasName = MUIManager.DefaultCanvasName,
            string cameraName = MUIManager.DefaultCameraName)
        {
            _canvas = canvas;
            _camera = camera;
            _eventSystem = eventSystem;
            _prefabLoader = prefabLoader;
            _canvasName = canvasName;
            _cameraName = cameraName;
        }

        public IModule[] ConfigureDependencies()
        {
            return new IModule[]
            {
                new CoroutineModule(),
            };
        }

        public IModuleInstaller[] ConfigureInstallers()
        {
            return new IModuleInstaller[]
            {
                new UIInstaller(_canvas, _camera, _eventSystem, _prefabLoader, _canvasName, _cameraName),
            };
        }

        public IRuntimeService[] ConfigureRuntimeServices()
        {
            return new IRuntimeService[]
            {
                new UIRuntimeService(),
            };
        }
    }
}
