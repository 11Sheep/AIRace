using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollector : MonoBehaviour
{
    private Action<Collider, bool> _triggerEnterAction; 
    
    public void SetTriggerEnterAction(Action<Collider, bool> action)
    {
        _triggerEnterAction = action;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _triggerEnterAction?.Invoke(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerEnterAction?.Invoke(other, false);
    }
}
