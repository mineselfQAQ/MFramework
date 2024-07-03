using MFramework;
using System;
using UnityEngine;

public class Test_ChainOfResponsibilityPattern : MonoBehaviour
{
    private void Start()
    {
        AbstractLogger chain = GetChain();
        chain.LogMessage(AbstractLogger.INFO, "Info");//chain是3级的，给定级别为1级，3级不成去2级，2级不成去1级，输出
        chain.LogMessage(AbstractLogger.ERROR, "Error");//3个级别全部输出
    }

    private static AbstractLogger GetChain()
    {
        AbstractLogger infoLogger = new InfoLogger(AbstractLogger.INFO);
        AbstractLogger debugLogger = new DebugLogger(AbstractLogger.DEBUG);
        AbstractLogger errorLogger = new ErrorLogger(AbstractLogger.ERROR);

        errorLogger.SetNextLogger(debugLogger);
        debugLogger.SetNextLogger(infoLogger);

        return errorLogger;
    }

    public abstract class AbstractLogger
    {
        public static int INFO = 1;
        public static int DEBUG = 2;
        public static int ERROR = 3;

        protected int level;

        protected AbstractLogger nextLogger;//每个人都能找到它的下一链

        public void SetNextLogger(AbstractLogger nextLogger)
        {
            this.nextLogger = nextLogger;
        }

        public void LogMessage(int level, String message)
        {
            if (this.level <= level)
            {
                Write(message);
            }
            if (nextLogger != null)
            {
                nextLogger.LogMessage(level, message);//递归，下个Logger也输出
            }
        }

        protected abstract void Write(String message);
    }

    public class InfoLogger : AbstractLogger
    {
        public InfoLogger(int level)
        {
            this.level = level;
        }

        protected override void Write(string message)
        {
            MLog.Print($"InfoLogger: {message}");
        }
    }
    public class DebugLogger : AbstractLogger
    {
        public DebugLogger(int level)
        {
            this.level = level;
        }

        protected override void Write(string message)
        {
            MLog.Print($"DebugLogger: {message}");
        }
    }
    public class ErrorLogger : AbstractLogger
    {
        public ErrorLogger(int level)
        {
            this.level = level;
        }

        protected override void Write(string message)
        {
            MLog.Print($"ErrorLogger: {message}");
        }
    }
}
