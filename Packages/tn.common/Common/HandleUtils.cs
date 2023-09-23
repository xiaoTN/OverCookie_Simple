#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace VirtualStage.Core
{
    public static class HandleUtils
    {
        public static void Label(Vector3 pos, string content)
        {
#if UNITY_EDITOR
            Handles.Label(pos, content);
#endif
        }

        public static void Label(this Transform trans, string content)
        {
            Label(trans.position, content);
        }
    }
}