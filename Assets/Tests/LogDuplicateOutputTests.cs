using MFramework.Core;
using NUnit.Framework;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace MFramework.Tests.Log
{
    public class LogDuplicateOutputTests
    {
        [SetUp]
        public void SetUp()
        {
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
        }

        [Test]
        public void Warning_LogString_ShouldContainMessageOnce()
        {
            var log = new TestLog(MLog.LogFilter.Debug);
            const string message = "warning_body_once";
            CapturedLog capturedLog = CaptureSingleLog(() => log.W(message, "WarnCase.cs", 12, "WarnCaller"));

            Assert.That(capturedLog.Type, Is.EqualTo(LogType.Warning));
            Assert.That(CountOccurrences(capturedLog.LogString, message), Is.EqualTo(1));
        }

        [Test]
        public void Error_LogString_ShouldContainMessageOnce_AndKeepStackTrace()
        {
            var log = new TestLog(MLog.LogFilter.Debug);
            const string message = "error_body_once";
            LogAssert.Expect(LogType.Error, new Regex($".*{Regex.Escape(message)}$"));
            CapturedLog capturedLog = CaptureSingleLog(() => log.E(message, "ErrCase.cs", 24, "ErrCaller"));

            Assert.That(capturedLog.Type, Is.EqualTo(LogType.Error));
            Assert.That(CountOccurrences(capturedLog.LogString, message), Is.EqualTo(1));
            Assert.That(capturedLog.StackTrace, Is.Not.Null.And.Not.Empty);
        }

        private static CapturedLog CaptureSingleLog(TestDelegate action)
        {
            int receivedCount = 0;
            string logString = null;
            string stackTrace = null;
            LogType? logType = null;

            void Handler(string condition, string trace, LogType type)
            {
                receivedCount++;
                logString = condition;
                stackTrace = trace;
                logType = type;
            }

            Application.logMessageReceived += Handler;
            try
            {
                action();
            }
            finally
            {
                Application.logMessageReceived -= Handler;
            }

            Assert.That(receivedCount, Is.EqualTo(1));
            Assert.That(logType, Is.Not.Null);

            return new CapturedLog(logString, stackTrace, logType.Value);
        }

        private static int CountOccurrences(string text, string value)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value)) return 0;

            int count = 0;
            int startIndex = 0;

            while (true)
            {
                int index = text.IndexOf(value, startIndex, System.StringComparison.Ordinal);
                if (index < 0) return count;

                count++;
                startIndex = index + value.Length;
            }
        }

        private readonly struct CapturedLog
        {
            public CapturedLog(string logString, string stackTrace, LogType type)
            {
                LogString = logString;
                StackTrace = stackTrace;
                Type = type;
            }

            public string LogString { get; }

            public string StackTrace { get; }

            public LogType Type { get; }
        }

        private sealed class TestLog : LogBase
        {
            protected override string SrcName => nameof(TestLog);

            public TestLog(MLog.LogFilter logFilter) : base(nameof(TestLog), logFilter)
            {
            }
        }
    }
}
