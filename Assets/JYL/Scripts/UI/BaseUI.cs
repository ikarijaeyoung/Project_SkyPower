using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    // 특수한 기능이 있는 UI를 사용하고자 할 경우, 식별되는 이름을 사용한다.
    public class BaseUI : MonoBehaviour
    {
        private Dictionary<string, GameObject> goDict;
        private Dictionary<string, Component> compDict;
        protected void Awake()
        {
            RectTransform[] transforms = GetComponentsInChildren<RectTransform>(true);
            goDict = new Dictionary<string, GameObject>(transforms.Length<<2);
            foreach (Transform t in transforms)
            {
                goDict.TryAdd(t.gameObject.name, t.gameObject);
            }

            Component[] components = GetComponentsInChildren<Component>(true);
            compDict = new Dictionary<string, Component>(components.Length << 2);
            foreach (Component comp in components)
            {
                compDict.TryAdd($"{comp.gameObject.name}_{comp.GetType().Name}",comp);
            }
        }

        // string으로 특정 UI 게임오브젝트 찾기
        public GameObject GetUI(in string name)
        {
            if (goDict == null)
            {
                Debug.Log("goDict없음"); 
                return null;
            }
            goDict.TryGetValue(name, out GameObject gameObject);
            if(gameObject == null)
            {
                Debug.LogError($"다음 UI 오브젝트가 없습니다: {name}");
            }
            return gameObject; // 없을경우 Null
        }

        // string으로 특정 UI 컴포넌트 찾기
        public T GetUI<T>(in string name) where T : Component
        {
            compDict.TryGetValue(name, out Component comp);
            if(comp != null)
            {
                return comp as T;
            }
            GameObject go = GetUI(name);
            if(go == null)
            {
                return null;
            }
            comp = go.GetComponent<T>();
            if (comp == null) return null;
            compDict.TryAdd($"{name}_{typeof(T).Name}", comp);
            return comp as T;
        }

        //  UI에 포인터핸들러를 부착 후, 가져오기
        public PointerHandler GetEvent(in string name)
        {
            GameObject go = GetUI(name);
            PointerHandler temp = go.GetOrAddComponent<PointerHandler>();

            return temp;
        }
    }
}

