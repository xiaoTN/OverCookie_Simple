using System;
using UniRx;
using UniRx.Triggers;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TN.Common
{
    /// <summary>
    ///     设置物体Transform的工具类
    /// </summary>
    public static class TransformUtils
    {
        public static void SetPosition(this Transform root, float? x, float? y, float? z)
        {
            Vector3 target = root.position;
            if (x != null)
                target.x = (float)x;
            if (y != null)
                target.y = (float)y;
            if (z != null)
                target.z = (float)z;

            root.position = target;
        }

        public static void SetLocalPosition(this Transform root, float? x, float? y, float? z)
        {
            Vector3 target = root.localPosition;
            if (x != null)
                target.x = (float)x;
            if (y != null)
                target.y = (float)y;
            if (z != null)
                target.z = (float)z;

            root.localPosition = target;
        }

        public static void SetLocalRotation(this Transform root, float? x, float? y, float? z)
        {
            Vector3 target = root.localEulerAngles;
            if (x != null)
                target.x = (float)x;
            if (y != null)
                target.y = (float)y;
            if (z != null)
                target.z = (float)z;

            root.localEulerAngles = target;
        }

        public static void CopyFromTransform(this Transform root, Transform target, bool sameParent = false)
        {
            if (sameParent)
            {
                root.transform.SetParent(target.parent);
            }

            root.localPosition = target.localPosition;
            root.localRotation = target.localRotation;
            root.localScale = target.localScale;
        }

        /// <summary>
        /// 将Transform的localPosition和localRotation置零
        /// </summary>
        public static void ResetLocal(this Transform root)
        {
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 将Transform的localPosition和localRotation置零，localScale置（1，1，1）
        /// </summary>
        /// <param name="root"></param>
        public static void ResetAllLocal(this Transform root)
        {
            root.ResetLocal();
            root.localScale = Vector3.one;
        }

        public static void ResetAllLocal(this Transform root, Transform parent)
        {
            root.SetParent(parent);
            root.ResetLocal();
            root.localScale = Vector3.one;
        }

        /// <summary>
        /// 将Transform的localPosition置零
        /// </summary>
        public static void ResetLocalPosition(this Transform root)
        {
            root.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 将Transform的localRotation置零
        /// </summary>
        public static void ResetLocalRotation(this Transform root)
        {
            root.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 忽略与other的所有碰撞盒
        /// </summary>
        /// <param name="root"></param>
        /// <param name="other"></param>
        /// <param name="isIgnore"></param>
        public static void IgnoreCollision(this Transform root, Transform other, bool isIgnore = true)
        {
            Collider[] colliders1 = root.GetComponentsInChildren<Collider>();
            Collider[] colliders2 = other.GetComponentsInChildren<Collider>();
            foreach (Collider collider1 in colliders1)
            {
                foreach (Collider collider2 in colliders2)
                {
                    Physics.IgnoreCollision(collider1, collider2, isIgnore);
                }
            }
        }

        /// <summary>
        /// 隐藏/显示所有碰撞盒
        /// </summary>
        /// <param name="root"></param>
        /// <param name="isHide"></param>
        public static void HideCollider(this Transform root, bool isHide = true)
        {
            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                collider.enabled = !isHide;
            }
        }

        /// <summary>
        /// 设置碰撞盒Trigger
        /// </summary>
        /// <param name="root"></param>
        /// <param name="isTrigger"></param>
        public static void SetColliderTrigger(this Transform root, bool isTrigger = true)
        {
            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                collider.isTrigger = isTrigger;
            }
        }

        /// <summary>
        /// 隐藏/显示所有子节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="isEnable"></param>
        public static void SetChildrenActive(this Transform root, bool isEnable)
        {
            foreach (Transform trans in root)
            {
                trans.gameObject.SetActive(isEnable);
            }
        }

        public static void FollowTarget(this Transform root, Transform target)
        {
            if (target != null)
            {
                root.position = target.position;
                root.rotation = target.rotation;
                Transform parent = root.parent;
                root.SetParent(null);
                root.localScale = target.lossyScale;
                root.SetParent(parent);
            }
        }

        public static void FollowTargetIgnoreScale(this Transform root, Transform target)
        {
            if (target != null)
            {
                root.position = target.position;
                root.rotation = target.rotation;
            }
        }

        public static Vector3 GetForward(this Quaternion rotation)
        {
            return rotation * Vector3.forward;
        }

        public static void LookAtCameraOnLateUpdate(this GameObject source, bool inverse = true)
        {
            source.UpdateAsObservable()
                .Sample(TimeSpan.FromSeconds(0.1f))
                .FirstOrDefault(unit => Camera.main != null)
                .Subscribe(unit =>
                {
                    Camera curCam = Camera.main;
                    if (curCam == null)
                        return;
                    LookAtTargetOnLateUpdate(source, curCam.gameObject, inverse);
                });
        }

        public static void FollowEyeOnLateUpdate(this GameObject source,float distance)
        {
            Camera curCam = Camera.main;
            if (curCam == null)
                return;
            LookAtTargetOnLateUpdate(source,curCam.gameObject);
            source.LateUpdateAsObservable()
                .Subscribe(unit =>
                {
                    Vector3 targetPos = distance* curCam.transform.forward+curCam.transform.position;
                    source.transform.position = targetPos;
                })
                .AddTo(curCam);
        }

        public static void LookAtTargetOnLateUpdate(this GameObject source, GameObject target, bool inverse = true)
        {
            source.LateUpdateAsObservable()
                .Subscribe(unit =>
                {
                    source.transform.LookAt(target.transform);
                    if (inverse)
                    {
                        source.transform.forward *= -1;
                    }
                })
                .AddTo(source)
                .AddTo(target);
        }

        public static void FollowTargetOnLateUpdate(this GameObject source, GameObject target)
        {
            source.LateUpdateAsObservable()
                .Subscribe(unit =>
                {
                    source.transform.position = target.transform.position;
                    source.transform.rotation = target.transform.rotation;
                })
                .AddTo(target);
        }

        public static void FollowTargetPositionOnLateUpdate(this GameObject source, GameObject target)
        {
            source.LateUpdateAsObservable()
                .Subscribe(unit =>
                {
                    source.transform.position = target.transform.position;
                })
                .AddTo(target);
        }

        public static Transform GetOrAddChild(this Transform root, string name)
        {
            Transform child = root.Find(name);
            if (child == null)
            {
                GameObject newChild = new GameObject();
                newChild.name = name;
                newChild.transform.SetParent(root);
                newChild.transform.ResetAllLocal();
                child = newChild.transform;
            }

            return child;
        }
        
        public static void SetLocalPositionX(this Transform trans, float x)
        {
            if (trans)
            {
                var pos = trans.localPosition;
                pos.x = x;
                trans.localPosition = pos;
            }
        }

        public static void SetLocalPositionY(this Transform trans, float y)
        {
            if (trans)
            {
                var pos = trans.localPosition;
                pos.y = y;
                trans.localPosition = pos;
            }
        }

        public static void SetLocalPositionZ(this Transform trans, float z)
        {
            if (trans)
            {
                var pos = trans.localPosition;
                pos.z = z;
                trans.localPosition = pos;
            }
        }
    }
}