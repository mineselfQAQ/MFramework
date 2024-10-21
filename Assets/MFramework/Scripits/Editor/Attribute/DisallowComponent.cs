using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [InitializeOnLoad]
    public static class DisallowComponent
    {
        static DisallowComponent()
        {
            // 监听添加组件的操作
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            var componentType = component.GetType();

            var attrs = componentType.GetCustomAttributes(typeof(DisallowComponentAttribute), true);
            if (attrs.Length == 0)
            {
                return;//并非该特性
            }

            //===它检：特性脚本已添加，其它脚本判断是否与特性脚本冲突，冲突则不添加该脚本===
            var allComponents = component.gameObject.GetComponents<MonoBehaviour>();
            foreach (var comp in allComponents)
            {
                var attributes = comp.GetType().GetCustomAttributes(typeof(DisallowComponentAttribute), true)
                                       .Cast<DisallowComponentAttribute>();

                foreach (var attr in attributes)
                {
                    if (attr.disallowedTypes.Contains(componentType))
                    {
                        MLog.Print($"{comp.GetType().Name}：不得添加{componentType.Name}", MLogType.Warning);
                        Object.DestroyImmediate(component);
                        return;
                    }
                }
            }

            //===自检：添加特性脚本，判断是否有与之冲突脚本，冲突则提示(删除)冲突脚本===
            //Tip：删除不太好，防止无法撤回，由用户决定
            var attributes2 = componentType.GetCustomAttributes(typeof(DisallowComponentAttribute), true);
            foreach (DisallowComponentAttribute attr in attributes2)
            {
                foreach (var disallowedType in attr.disallowedTypes)
                {
                    if (component.gameObject.GetComponent(disallowedType) != null)
                    {
                        MLog.Print($"{componentType.Name}：如需添加{componentType.Name}不得添加{disallowedType.Name}，请先删除", MLogType.Warning);
                        //Object.DestroyImmediate(component.gameObject.GetComponent(disallowedType));
                        Object.DestroyImmediate(component);
                        return;
                    }
                }
            }
        }
    }
}
