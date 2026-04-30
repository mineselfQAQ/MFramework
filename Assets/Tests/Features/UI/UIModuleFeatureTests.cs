using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MFramework.Tests.Features.UI
{
    public class UIModuleFeatureTests
    {
        [Test]
        public void FeatureUIModule_AfterBootstrap_RegistersManagerInDI()
        {
            GameObject canvasObject = CreateCanvas(out Canvas canvas);
            GameObject cameraObject = CreateCamera(out Camera camera);
            GameObject eventSystemObject = CreateEventSystem();

            var core = new MCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { new UIModule(canvas, camera, eventSystemObject) });
            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();

            Assert.IsNotNull(core.Container.Resolve<MUIManager>());

            core.Initialize();
            core.Shutdown();

            Object.DestroyImmediate(canvasObject);
            Object.DestroyImmediate(cameraObject);
            Object.DestroyImmediate(eventSystemObject);
        }

        [Test]
        public void FeatureUIRoot_CreatePanelFromBehaviour_CanOpenCloseAndDestroy()
        {
            GameObject canvasObject = CreateCanvas(out Canvas canvas);
            GameObject cameraObject = CreateCamera(out Camera camera);
            GameObject eventSystemObject = CreateEventSystem();

            var manager = new MUIManager(canvas, camera, eventSystemObject);
            manager.Initialize();
            UIRoot root = manager.CreateRoot("TestRoot", 0, 100);
            UIPanelBehaviour behaviour = CreatePanelBehaviour(canvas.transform);

            SimpleFadePanel panel = root.CreatePanel<SimpleFadePanel>("PanelA", behaviour, 10, autoEnter: false);

            Assert.IsNotNull(panel);
            Assert.IsTrue(root.ExistPanel("PanelA"));
            Assert.AreEqual(10, panel.SortingOrder);

            root.OpenPanel("PanelA");
            Assert.AreEqual(UIShowState.On, panel.ShowState);

            root.ClosePanel("PanelA");
            Assert.AreEqual(UIShowState.Off, panel.ShowState);

            Assert.IsTrue(root.DestroyPanel("PanelA"));
            Assert.IsFalse(root.ExistPanel("PanelA"));

            Object.DestroyImmediate(canvasObject);
            Object.DestroyImmediate(cameraObject);
            Object.DestroyImmediate(eventSystemObject);
        }

        private static GameObject CreateCanvas(out Canvas canvas)
        {
            GameObject canvasObject = new("UICanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvasObject;
        }

        private static GameObject CreateCamera(out Camera camera)
        {
            GameObject cameraObject = new("UICamera");
            camera = cameraObject.AddComponent<Camera>();
            return cameraObject;
        }

        private static GameObject CreateEventSystem()
        {
            GameObject eventSystemObject = new("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
            return eventSystemObject;
        }

        private static UIPanelBehaviour CreatePanelBehaviour(Transform canvasTransform)
        {
            GameObject panelObject = new("PanelA");
            panelObject.transform.SetParent(canvasTransform, false);
            panelObject.AddComponent<RectTransform>();
            return panelObject.AddComponent<UIPanelBehaviour>();
        }
    }
}
