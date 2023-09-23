using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TN.Info;
using UnityEngine;
using VirtualStage.Core;

namespace TN.Building
{
    /// <summary>
    /// 食物分配桌
    /// </summary>
    public class FoodAllot: BaseObj
    {
        protected override string GizmoLabel
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (OrderInfo orderInfo in _orderQueue)
                {
                    s.AppendLine(orderInfo.ToString());
                }

                return $@"{s}";
            }
        }

        [ShowInInspector]
        [ReadOnly]
        private Queue<OrderInfo> _orderQueue = new Queue<OrderInfo>();
        public void EnqueueOrder(OrderInfo curOrder)
        {
            _orderQueue.Enqueue(curOrder);
        }

        public bool HaveFood()
        {
            return _orderQueue.Count > 0;
        }

        public OrderInfo RemoveFood()
        {
            return _orderQueue.Dequeue();
        }
    }
}