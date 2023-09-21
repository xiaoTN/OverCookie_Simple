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
        public List<MenuInfo> MenuInfos;

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

        public DiningTable       DiningTable;
        public FoodContainer     FoodContainer;
        public FireWoodContainer FireWoodContainer;
        public CookingBench      CookingBench;


        protected override void SetUp()
        {
            base.SetUp();
            MenuInfos = JsonUtils.ReadJsonFromStreamingAssets<List<MenuInfo>>("MenuInfo.json");
            FoodContainer = FindObjectOfType<FoodContainer>();
            DiningTable = FindObjectOfType<DiningTable>();
            CookingBench = FindObjectOfType<CookingBench>();
        }

        [Button]
        private void Save()
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
    }
}