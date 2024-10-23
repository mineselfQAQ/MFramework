using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 对象池，其中存放着一组ObjectPoolContainer<T>
    /// </summary>
    public class Old_ObjectPool<T>
    {
       private List<Old_ObjectPoolContainer<T>> list;
       //注意：只有正在Used的物体才在表中
       private Dictionary<T, Old_ObjectPoolContainer<T>> lookup;//key---实际存放物体  value---list中的一个Container

       private Func<T> initFunc;
       private int lastIndex = 0;

       public int Count
       {
           get { return list.Count; }
       }
       public int UsedCount
       {
           get { return lookup.Count; }
       }

       public Old_ObjectPool(Func<T> initFunc, int initSize, bool warmObject)
       {
           this.initFunc = initFunc;//通过构造函数获得初始化

           //创建初始list/lookup
           list = new List<Old_ObjectPoolContainer<T>>(initSize);
           lookup = new Dictionary<T, Old_ObjectPoolContainer<T>>(initSize);
           //创建初始Container
           if(warmObject) Warm(initSize);
       }

       /// <summary>
       /// 获取Item(获取Not Used物体或创建Container)
       /// </summary>
       public T GetItem()
       {
           //在list中寻找Not Used的物体
           Old_ObjectPoolContainer<T> container = null;
           for (int i = 0; i < list.Count; i++)
           {
               //循环列表
               //如XXXOXXO，我从左到右找到了第一个O，变为XXXXXXO
               //通过循环列表，我只需要从上一次的后面开始查找，即寻找3次就能找到，避免了没必要的重复寻找
               //如上情况，除非Release()，否则这样遍历一定是最佳的
               lastIndex++;
               if (lastIndex > list.Count - 1) lastIndex = 0;

               if (list[lastIndex].Used)
               {
                   continue;
               }
               else//找到Not Used的物体
               {
                   container = list[lastIndex];
                   break;
               }
           }

           //全部遍历还是没有找到，创个新的
           if (container == null)
           {
               container = CreateContainer();
           }

           container.Consume();
           lookup.Add(container.Item, container);

           return container.Item;
       }

       /// <summary>
       /// 释放Item(禁用物体)
       /// </summary>
       public void ReleaseItem(object item)
       {
           ReleaseItem((T)item);
       }

       /// <summary>
       /// 释放Item(禁用物体)
       /// </summary>
       public void ReleaseItem(T item)
       {
           if (lookup.ContainsKey(item))
           {
               var container = lookup[item];
               container.Release();
               lookup.Remove(item);
           }
           else//只有在表中的物体才是可被释放物体
           {
               MLog.Print($"{typeof(Old_ObjectPool<T>)}：已没有可释放{item}，请检查", MLogType.Warning);
           }
       }

       /// <summary>
       /// 创建Container
       /// </summary>
       private void Warm(int capacity)
       {
           for (int i = 0; i < capacity; i++)
           {
               var container = CreateContainer();
               (container.Item as GameObject).SetActive(false);
           }
       }

       /// <summary>
       /// 将Container加入池中
       /// </summary>
       private Old_ObjectPoolContainer<T> CreateContainer()
       {
           //Container的创建就是实例化物体并将其添加进list
           var container = new Old_ObjectPoolContainer<T>();
           container.Item = initFunc();//其实就是执行InstantiatePrefab()
           list.Add(container);
           return container;
       }
    }
}