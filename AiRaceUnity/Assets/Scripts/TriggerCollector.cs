using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollector : MonoBehaviour
{
    private Action<Collider> _triggerEnterAction; 
    
    public void SetTriggerEnterAction(Action<Collider> action)
    {
        _triggerEnterAction = action;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _triggerEnterAction?.Invoke(other);
    }
}
