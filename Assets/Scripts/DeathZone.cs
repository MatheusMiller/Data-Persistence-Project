using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private MainManager Manager;
    
    private void OnCollisionEnter(Collision other)
    {
        Manager = MainManager.instance;
        Destroy(other.gameObject);
        Manager.GameOver();
    }
}
