using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.Tracker;
using MFramework.Coroutines;

namespace MFrameworkExamples.Coroutine
{
    public class MEntry : MEntryBase
    {
        private MCoroutineManager _coroutineManager;
        private BoolWrapper _waitFlag;

        protected override MLog.LogFilter SetLogFilter()
        {
            return MLog.LogFilter.Debug;
        }

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            _coroutineManager = Core.Container.Resolve<MCoroutineManager>();
            _waitFlag = new BoolWrapper(false);

            MLog.Default.D("Coroutine example start.");

            _coroutineManager.DelayOneFrame(() =>
            {
                MLog.Default.D("DelayOneFrame finished.");
            });

            _coroutineManager.DelayNoRecord(() =>
            {
                _waitFlag.Value = true;
                MLog.Default.D("DelayNoRecord finished, wait flag set.");
            }, 1f);

            _coroutineManager.WaitNoRecord(() =>
            {
                MLog.Default.D("WaitNoRecord observed flag.");
            }, _waitFlag);

            _coroutineManager.Repeat(
                "example-repeat",
                () => MLog.Default.D("Named repeat tick."),
                startDo: true,
                count: 3,
                interval: 0.5f,
                onFinish: () => MLog.Default.D("Named repeat finished."));

            _coroutineManager.Loop(
                "example-loop",
                () => MLog.Default.D("Named loop tick."),
                startInterval: 0.25f,
                repeatInterval: 0.25f);

            _coroutineManager.DelayNoRecord(() =>
            {
                bool stopped = _coroutineManager.EndCoroutine("example-loop");
                MLog.Default.D($"Named loop stopped: {stopped}, active named count: {_coroutineManager.Count}");
            }, 1.35f);
        }

        protected override void OnShuttingDown(TrackerStartedEvent e)
        {
            MLog.Default.D($"Coroutine example shutdown, active named count: {_coroutineManager?.Count ?? 0}");
        }

        protected override IModule[] ConfigureModules()
        {
            return new IModule[]
            {
                new CoroutineModule(),
            };
        }
    }
}
