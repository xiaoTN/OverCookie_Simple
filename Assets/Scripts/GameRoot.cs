using System;
using Sirenix.OdinInspector;
using TN.Info;
using UnityEditor;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private void Start()
    {
        //读取持久化文本
        //实例化物体，如 厨师、服务员等
        //将持久化文本赋值给对应的object
        //所有物体初始化
        GameManager.Instance.Init();
    }

    [Button]
    private void Custom()
    {
        BaseObj[] findObjectsOfType = FindObjectsOfType<BaseObj>();
        foreach (BaseObj baseObj in findObjectsOfType)
        {
            baseObj.GUID = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(baseObj);
        }
    }
}