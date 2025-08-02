using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    [SerializeField] protected bool destroyOtherInstances = true;
    [SerializeField] protected bool persistent;

    protected virtual void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogError($"There is more than one {gameObject.name} Singleton");
            if (destroyOtherInstances)
            {
                gameObject.transform.SetParent(null);
                Destroy(gameObject);
            }
        }
        else
        {
            Instance = this as T;
            if(persistent) DontDestroyOnLoad(gameObject);
        }
    }
}
