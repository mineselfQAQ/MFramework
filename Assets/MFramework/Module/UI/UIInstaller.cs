using MFramework.Core.CoreEx;
using MFramework.Coroutines;
using UnityEngine;

namespace MFramework.UI
{
    public sealed class UIInstaller : IModuleInstaller
    {
        private readonly Canvas _canvas;
        private readonly Camera _camera;
        private readonly GameObject _eventSystem;
        private readonly IUIPrefabLoader _prefabLoader;
        private readonly string _canvasName;
        private readonly string _cameraName;

        public UIInstaller(
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

        public void Install(IModuleContext context)
        {
            context.Container.RegisterSingleton(container => new MUIManager(
                _canvas,
                _camera,
                _eventSystem,
                _prefabLoader,
                container.Resolve<MCoroutineManager>(),
                _canvasName,
                _cameraName));
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MUIManager>();
        }
    }
}
