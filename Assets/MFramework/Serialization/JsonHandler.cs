using MFramework;
using UnityEngine;

[MonoSingletonSetting(HideFlags.NotEditable, "#JsonHandler#")]
public class JsonHandler : MonoSingleton<JsonHandler>
{
    //暂时在MSerializationUtility中，如果出现需要存储情况，可以移过来
}
