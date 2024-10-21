using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class NoComponentAttribute : PropertyAttribute
    {
        public NoComponentAttribute()
        {

        }
    }
}
