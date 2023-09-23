using System;
using TN.Role;

namespace TN.Info
{
    /// <summary>
    /// 订单信息
    /// </summary>
    [Serializable]
    public class OrderInfo
    {
        public MenuInfo MenuInfo;
        public Customer Customer;

        public OrderInfo(MenuInfo menuInfo, Customer customer)
        {
            MenuInfo = menuInfo;
            Customer = customer;
        }

        public override string ToString()
        {
            return $"{MenuInfo.TargetId.ToString()}[1个]->{Customer.Name}";
        }
    }
}