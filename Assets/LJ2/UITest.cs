using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    private LinkedList<int> test = new LinkedList<int>();
    [SerializeField] public Button testButton;
    [SerializeField] public ScrollRect scrollRect;
    

    // Start is called before the first frame update
    void Start()
    {
        if (test.Count < 3)
        {
            int need = 3 - test.Count;
            for (int i = 0; i < need; ++i)
            {
                test.AddLast(0);
            }
        }

        Debug.Log(test.First.Value);
        Debug.Log(test.Count);
        Debug.Log(test.Last.Value);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            CreateButton();
        }
    }

    private void CreateButton()
    {

    }
}
