using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Common;
using TN.Role;

namespace TN.Info
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [ShowInInspector]
        [NonSerialized]
        [TableList]
        public List<MenuInfo> MenuInfos;

        public MenuInfo CurFirstMenu
        {
            get
            {
                if (_orderFormMenuQueue.Count > 0)
                    return _orderFormMenuQueue.Peek().MenuInfo;
                return null;
            }
        }

        /// <summary>
        /// 订单队列
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private Queue<OrderInfo> _orderFormMenuQueue = new Queue<OrderInfo>();


        /// <summary>
        /// 添加预定
        /// </summary>
        public OrderInfo AddOrder(Customer customer, MenuInfo menuInfo)
        {
            OrderInfo orderInfo = new OrderInfo(menuInfo, customer);
            _orderFormMenuQueue.Enqueue(orderInfo);
            return orderInfo;
        }

        public OrderInfo AddOrder(Customer customer, ObjType objType)
        {
            MenuInfo findMenu = MenuInfos.Find(info => info.TargetId == objType);
            return AddOrder(customer, findMenu);
        }

        /// <summary>
        /// 移除订单
        /// </summary>
        public OrderInfo RemoveOrder()
        {
            return _orderFormMenuQueue.Dequeue();
        }

        [ShowInInspector]
        [NonSerialized]
        [TableList]
        public List<ObjInfo> ObjInfos;

        public List<Customer>    Customers;
        public List<DiningTable> DiningTables;
        public List<Cook>        Cooks;
        public List<Waiter>      Waiters;


        public FoodAllot         FoodAllot;
        public FoodContainer     FoodContainer;
        public FireWoodContainer FireWoodContainer;
        public CookingBench      CookingBench;


        protected override void SetUp()
        {
            base.SetUp();
            MenuInfos = JsonUtils.ReadJsonFromStreamingAssets<List<MenuInfo>>("MenuInfo.json");
            ObjInfos = JsonUtils.ReadJsonFromStreamingAssets<List<ObjInfo>>("ObjInfo.json");
            FoodContainer = FindObjectOfType<FoodContainer>();
            FoodAllot = FindObjectOfType<FoodAllot>();
            CookingBench = FindObjectOfType<CookingBench>();
            FireWoodContainer = FindObjectOfType<FireWoodContainer>();

            DiningTables = FindObjectsOfType<DiningTable>().ToList();
            Cooks = FindObjectsOfType<Cook>().ToList();
            Waiters = FindObjectsOfType<Waiter>().ToList();
            Customers = FindObjectsOfType<Customer>().ToList();

            foreach (Cook cook in Cooks)
            {
                cook.Init();
            }
            foreach (Waiter waiter in Waiters)
            {
                waiter.Init();
            }

            foreach (DiningTable diningTable in DiningTables)
            {
                diningTable.Init();
            }
        }
    }
}