using System;
using UnityEngine;

namespace MFramework
{
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class DisallowComponentAttribute : PropertyAttribute
    {
        public Type[] disallowedTypes;

        public DisallowComponentAttribute(params Type[] types)
        {
            this.disallowedTypes = types;
        }
    }
}
