using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Common;

namespace TN.Info
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [ShowInInspector]
        [NonSerialized]
        [TableList]
        [InlineButton(nameof(SaveMenuInfo))]
        public List<MenuInfo> MenuInfos;

        private void SaveMenuInfo()
        {
            List<MenuInfo> menuInfos = new List<MenuInfo>()
            {
                new MenuInfo()
                {
                    TargetId = ObjType.Scone,
                    Duration = -1,
                    SourceMaterialInfos = null,
                },
                new MenuInfo()
                {
                    TargetId = ObjType.Beaf,
                    Duration = 3,
                    SourceMaterialInfos = new List<SourceMaterialInfo>()
                    {
                        new SourceMaterialInfo()
                        {
                            Id = ObjType.OriginBeaf,
                            Count = 1
                        }
                    }
                },
                new MenuInfo()
                {
                    TargetId = ObjType.Noodle,
                    Duration = 10,
                    SourceMaterialInfos = new List<SourceMaterialInfo>()
                    {
                        new SourceMaterialInfo()
                        {
                            Id = ObjType.Beaf,
                            Count = 1,
                        },
                        new SourceMaterialInfo()
                        {
                            Id = ObjType.Noodle,
                            Count = 1
                        }
                    }
                }
            };
            JsonUtils.WriteJson(menuInfos, "MenuInfo.json");
        }

        /// <summary>
        /// 订单队列
        /// </summary>
        [ShowInInspector]
        [NonSerialized]
        public Queue<MenuInfo> OrderFormMenuQueue = new Queue<MenuInfo>();

        public MenuInfo CurFirstMenu
        {
            get
            {
                if (OrderFormMenuQueue.Count > 0)
                    return OrderFormMenuQueue.Peek();
                return null;
            }
        }

        [ShowInInspector]
        [NonSerialized]
        [TableList]
        [InlineButton(nameof(SaveObjInfo))]
        public List<ObjInfo> ObjInfos;

        private void SaveObjInfo()
        {
            List<ObjInfo> infos = new List<ObjInfo>()
            {
                new ObjInfo()
                {
                    Id = ObjType.Beaf,
                    Name = "熟牛排",
                    MaxCountPerGrid = 1
                },
                new ObjInfo()
                {
                    Id = ObjType.OriginBeaf,
                    Name = "生牛排",
                    MaxCountPerGrid = 1
                },
                new ObjInfo()
                {
                    Id = ObjType.Noodle,
                    Name = "意大利牛排套餐",
                    MaxCountPerGrid = 1
                },
                new ObjInfo()
                {
                    Id = ObjType.Scone,
                    Name = "曲奇",
                    MaxCountPerGrid = 1
                },
                new ObjInfo()
                {
                    Id = ObjType.FireWood,
                    Name = "柴火",
                    MaxCountPerGrid = 50
                },
            };
            JsonUtils.WriteJson(infos, "ObjInfo.json");
        }
        

        public DiningTable       DiningTable;
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
            DiningTable = FindObjectOfType<DiningTable>();
            FoodAllot = FindObjectOfType<FoodAllot>();
            CookingBench = FindObjectOfType<CookingBench>();
            FireWoodContainer = FindObjectOfType<FireWoodContainer>();
        }
    }
}