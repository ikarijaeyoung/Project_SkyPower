using KYG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltLaserSize : MonoBehaviour
{
    [SerializeField] private float increase = 0.2f;
    [SerializeField] private float maxSize = 5f;

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

        transform.localScale = Vector3.one * size;
    }
}
