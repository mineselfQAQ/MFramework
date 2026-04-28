using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.Tracker;
using MFramework.Tween;
using UnityEngine;

namespace MFrameworkExamples.Tween
{
    public class MEntry : MEntryBase
    {
        private GameObject _target;
        private MTweenManager _tweenManager;

        protected override MLog.LogFilter SetLogFilter()
        {
            return MLog.LogFilter.Debug;
        }

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            _tweenManager = Core.Container.Resolve<MTweenManager>();
            _target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _target.name = "TweenExampleCube";
            _target.transform.position = new Vector3(-2f, 0f, 0f);

            _tweenManager.DoTween01NoRecord(value =>
            {
                _target.transform.position = Vector3.Lerp(
                    new Vector3(-2f, 0f, 0f),
                    new Vector3(2f, 0f, 0f),
                    value);
            }, MCurve.SineInOut, 1.5f, () => MLog.Default.D("Position tween finished."));

            _tweenManager.ScaleNoRecord(
                _target.transform,
                scaleMultiplier: 1.5f,
                curve: MCurve.BackOut,
                duration: 1.5f,
                onFinish: () => MLog.Default.D("Scale tween finished."));
        }

        protected override void OnShutDown(TrackerStoppedEvent e)
        {
            if (_target != null)
            {
                Destroy(_target);
                _target = null;
            }
        }

        protected override IModule[] ConfigureModules()
        {
            return new IModule[]
            {
                new TweenModule(),
            };
        }
    }
}
