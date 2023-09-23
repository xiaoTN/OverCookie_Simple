using System;
using System.Collections.Generic;
using System.Text;
using TN.Info;
using TN.Role;
using UnityEngine;

namespace TN.Building
{
    public class DiningTable: BaseObj
    {
        protected override string GizmoLabel
        {
            get
            {
                StringBuilder s = new StringBuilder();
                
                return "";
            }
        }

        public List<Customer>  Customers;
        [NonSerialized]
        public List<OrderInfo> OrderInfos=new List<OrderInfo>();
        public void Init()
        {
            foreach (Customer customer in Customers)
            {
                customer.Init(this);
            }
        }

        public void AddOrderInfo(OrderInfo orderInfo)
        {
            OrderInfos.Add(orderInfo);
            Customer findCustom = Customers.Find(customer => customer.WantOrderInfo == orderInfo);
            if (findCustom == null)
            {LogError($"找不到顾客：{orderInfo}");
                return;
            }
            findCustom.Eat();
        }

        public void RemoveOrderInfo(OrderInfo orderInfo)
        {
            OrderInfos.Remove(orderInfo);
        }
    }
}