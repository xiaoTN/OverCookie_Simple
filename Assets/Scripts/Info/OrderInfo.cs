using System;
using System.Collections.Generic;
using TN.Role;

namespace TN.Info
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public class OrderInfo
    {
        public MenuInfo MenuInfo
        {
            get
            {
                return GameManager.Instance.MenuInfos.Find(info => info.TargetId == TargetFood);
            }
        }

        public Customer Customer
        {
            get
            {
                return GameManager.Instance.Customers.Find(customer => customer.GUID.Equals(CustomGuid));
            }
        }
        
        public ObjType  TargetFood;
        public string   CustomGuid;

        public OrderInfo(MenuInfo menuInfo, Customer customer)
        {
            TargetFood = menuInfo.TargetId;
            CustomGuid = customer.GUID;
        }

        public override string ToString()
        {
            if (Customer == null)
            {
                return String.Empty;
            }

            return $"{MenuInfo.TargetId.ToString()}[1个]->{Customer.SingleName}";
        }
    }
}