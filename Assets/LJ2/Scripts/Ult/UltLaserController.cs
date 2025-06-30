using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltLaserController : MonoBehaviour
{
    [SerializeField] private float increase;
    [SerializeField] private float maxSize;
    private float size;

    private void Awake()
    {
        increase = Mathf.Clamp(increase, 0.1f, maxSize);
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        size = 0;
    }

    private void LateUpdate()
    {
        if (size < maxSize)
        {
            size += increase;
        }
        transform.localScale = Vector3.one * (1 + size);
    }

    
}
