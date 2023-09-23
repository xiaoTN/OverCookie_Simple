using System;
using Sirenix.OdinInspector;
using TN.Info;
using UniRx;
using UnityEngine;

namespace TN.Building
{
    /// <summary>
    /// 灶台
    /// </summary>
    public class CookingBench : BaseObj
    {
        [ReadOnly]
        public ObjType CookingFoodId;

        /// <summary>
        /// 剩余柴火数
        /// </summary>
        [ReadOnly]
        public int RemainFireWood;


        protected override string GizmoLabel
        {
            get { return $@"柴火数：{RemainFireWood}
正在烹饪的食物：{CookingFoodId.ToString()}"; }
        }

        private ReactiveProperty<int> _usingCount = new ReactiveProperty<int>();

        private void Start()
        {
            CompositeDisposable cookDis = null;
            _usingCount.Select(i => i > 0)
                       .ToReactiveProperty()
                       .Subscribe(isUsing =>
                       {
                           if (isUsing)
                           {
                               cookDis = new CompositeDisposable();
                               Observable.Interval(TimeSpan.FromSeconds(1))
                                         .Subscribe(l =>
                                         {
                                             RemainFireWood--;
                                         })
                                         .AddTo(this)
                                         .AddTo(cookDis);
                           }
                           else
                           {
                               cookDis?.Dispose();
                           }
                       })
                       .AddTo(this);
        }

        public void StartCook()
        {
            _usingCount.Value++;
        }

        public void StopCook()
        {
            _usingCount.Value--;
        }

        public void AddFireWood(int count)
        {
            RemainFireWood += count;
        }
    }
}