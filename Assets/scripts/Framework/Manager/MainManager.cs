using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance = null;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
