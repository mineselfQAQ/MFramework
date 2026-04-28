using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Coroutines;
using MFramework.Tween;
using NUnit.Framework;

namespace MFramework.Tests.Features.Tween
{
    public class TweenModuleFeatureTests
    {
        [Test]
        public void FeatureTweenModule_AfterBootstrap_RegistersTweenAndCoroutineManagers()
        {
            var core = new MCore();
            var bootstrap = new ModuleBootstrap(core, new IModule[] { new TweenModule() });

            core.AddBootstrap(bootstrap);
            core.AddShutdown(bootstrap);

            core.Bootstrap();

            Assert.IsNotNull(core.Container.Resolve<MCoroutineManager>());
            Assert.IsNotNull(core.Container.Resolve<MTweenManager>());

            core.Initialize();
            core.Shutdown();
        }

        [Test]
        public void FeatureCurveSampler_WithLinearReverse_ReturnsDecrementValue()
        {
            MCurve curve = MCurve.Linear.Reverse();

            float value = MCurveSampler.Sample(curve, 0.25f);

            Assert.AreEqual(0.75f, value, 0.0001f);
        }
    }
}
