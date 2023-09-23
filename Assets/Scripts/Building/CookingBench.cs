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
        /// <summary>
        /// 剩余柴火数
        /// </summary>
        [ReadOnly]
        public int RemainFireWood;

        /// <summary>
        /// 最大使用数量
        /// </summary>
        public int MaxUseCount=2;

        protected override string GizmoLabel
        {
            get { return $@"柴火数：{RemainFireWood}"; }
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

        public bool CanUse()
        {
            if (_usingCount.Value < MaxUseCount)
            {
                return true;
            }

            return false;
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