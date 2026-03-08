using System;

namespace MFramework.Core.CoreEx
{
    public class FrameworkException : Exception
    {
        public FrameworkException(string message) : base(message)
        {
        }
    }
}