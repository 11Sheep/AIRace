using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollector : MonoBehaviour
{
    private Action<Collider, bool> _triggerEnterAction; 
    
    private Action<Collider, bool> _colliderEnterAction;
    
    public void SetTriggerEnterAction(Action<Collider, bool> action)
    {
        _triggerEnterAction = action;
    }
    
    public void SetColliderEnterAction(Action<Collider, bool> action)
    {
        _colliderEnterAction = action;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _triggerEnterAction?.Invoke(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerEnterAction?.Invoke(other, false);
    }

    private void OnCollisionEnter(Collision other)
    {
        _colliderEnterAction?.Invoke(other.collider, true);
    }
    
    private void OnCollisionExit(Collision other)
    {
        _colliderEnterAction?.Invoke(other.collider, false);
    }
}
