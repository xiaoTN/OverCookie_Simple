using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TN.Info;
using TN.Role;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VirtualStage.Core;

public class GameRoot : MonoBehaviour
{
    public Text Text;
    private void Start()
    {
        //读取持久化文本
        //实例化物体，如 厨师、服务员等
        //将持久化文本赋值给对应的object
        //所有物体初始化
        GameManager.Instance.Init();

        GameManager.Instance.OnGameOver
                   .Subscribe(unit =>
                   {
                       StringBuilder s = new StringBuilder();
                       foreach (Cook cook in GameManager.Instance.Cooks)
                       {
                           s.AppendLine($"==={cook.SingleName} 完成订单===");
                           foreach (KeyValuePair<ObjType, int> item in cook.HaveFinishOrder)
                           {
                               s.AppendLine($"{item.Key} {item.Value}个");
                           }
                       }
                       foreach (Customer customer in GameManager.Instance.Customers)
                       {
                           s.AppendLine($"==={customer.SingleName} 用餐数量===");
                           foreach (KeyValuePair<ObjType, int> item in customer.HaveFinishOrder)
                           {
                               s.AppendLine($"{item.Key} {item.Value}个");
                           }
                       }
                       
                       
                       Text.text = $@"游戏结束
{s.ToString()}";
                   })
                   .AddTo(this);
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