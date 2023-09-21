using System;
using UnityEngine;

namespace TN.Common
{
    /// <summary>
    /// 脚本单例类，只存在于当前场景，切换场景时销毁
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        protected virtual void Awake()
        {
            _instance = this as T;
        }

        /// <summary>
        /// 单例，只存在于当前场景，切换场景时销毁
        /// </summary>
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }

                return _instance;
            }
            set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
            }
        }
    }
}