using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Markers on track for AI learning
    /// </summary>
    [SerializeField] private MarkerScript[] _markers;
    
    /// <summary>
    /// The car script so we can control the car
    /// </summary>
    [SerializeField] private CarController[] _carControllers;
    
    
    private void Start()
    {
        for (int carIndex = 0; carIndex < _carControllers.Length; carIndex++)
        {
            _carControllers[carIndex].Initialize(_markers);
        }
    }
}
