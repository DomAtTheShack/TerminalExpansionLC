using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> ActionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lock (ActionQueue)
        {
            while (ActionQueue.Count > 0)
            {
                ActionQueue.Dequeue()?.Invoke();
            }
        }
    }

    public static void Enqueue(Action action)
    {
        lock (ActionQueue)
        {
            ActionQueue.Enqueue(action);
        }
    }
}