using System;
using TN.Info;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.Init();
    }
}