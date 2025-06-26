using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public static class Util
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }
    }
}

