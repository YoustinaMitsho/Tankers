using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [Header("Time Settings:")]
    [SerializeField] private float _timer;

    void Start()
    {
        Destroy(gameObject, _timer);
    }
}
