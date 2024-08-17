using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CarScript _carScript;

    [SerializeField] private float _forceToAdd = 0.001f;

    /// <summary>
    /// Markers on track for AI learning
    /// </summary>
    [SerializeField] private MarkerScript[] _markers;
    
    private int _currentMarkerIndex = 0;
    
    private void Start()
    {
        _carScript.Initialize(CarHitMarker);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _carScript.Move(_forceToAdd);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _carScript.Move(-_forceToAdd);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            _carScript.SetWheelAngle(-10);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _carScript.SetWheelAngle(10);
        }
    }
    
    /// <summary>
    /// The car hit a marker
    /// </summary>
    /// <param name="marker"></param>
    private void CarHitMarker(MarkerScript marker)
    {
        if (_markers[_currentMarkerIndex] == marker)
        {
            Debug.Log("We hit the correct marker, # " + _currentMarkerIndex);
            
            _currentMarkerIndex++;
        }
    }
}
