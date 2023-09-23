using System;
using System.Collections.Generic;
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
                return "";
            }
        }

        public List<Customer>  Customers;
        [NonSerialized]
        public List<OrderInfo> OrderInfos;
        public void Init()
        {
            foreach (Customer customer in Customers)
            {
                customer.Init(this);
            }

            OrderInfos ??= new List<OrderInfo>();
        }

        public void AddOrderInfo(OrderInfo orderInfo)
        {
            OrderInfos.Add(orderInfo);
        }

        public void RemoveOrderInfo(OrderInfo orderInfo)
        {
            OrderInfos.Remove(orderInfo);
        }
    }
}