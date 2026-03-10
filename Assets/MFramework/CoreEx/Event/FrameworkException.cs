using System;

namespace MFramework.Core.Event
{
    public class FrameworkException : Exception
    {
        public FrameworkException(string message) : base(message)
        {
        }
    }
}