using MFramework;
using System;
using UnityEngine;

public class Test_ChainOfResponsibilityPattern : MonoBehaviour
{
    private void Start()
    {
        AbstractLogger chain = GetChain();
        chain.LogMessage(AbstractLogger.INFO, "Info");//chain��3���ģ���������Ϊ1����3������ȥ2����2������ȥ1�������
        chain.LogMessage(AbstractLogger.ERROR, "Error");//3������ȫ�����
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

        protected AbstractLogger nextLogger;//ÿ���˶����ҵ�������һ��

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
                nextLogger.LogMessage(level, message);//�ݹ飬�¸�LoggerҲ���
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
