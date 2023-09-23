using System.Collections.Generic;
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

        public List<Customer> Customers;

        public void Init()
        {
            foreach (Customer customer in Customers)
            {
                customer.Init(this);
            }
        }
    }
}