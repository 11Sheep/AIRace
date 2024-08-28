using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _forceToAdd = 0.001f;

    /// <summary>
    /// Markers on track for AI learning
    /// </summary>
    [SerializeField] private MarkerScript[] _markers;
    
    /// <summary>
    /// The car script so we can control the car
    /// </summary>
    [SerializeField] private CarScript _carScript;
    
    /// <summary>
    /// Control the car ML agent
    /// </summary>
    [SerializeField] private CarAgent _carAgent;
    
    /// <summary>
    /// This will be used to 
    /// </summary>
    [SerializeField] private TriggerCollector _outerTrackTriggerCollector;
    
    private int _currentMarkerIndex = 0;
    
    private void Start()
    {
        _carScript.Initialize(CarHitMarker);
        _carAgent.Initialize(OnMoveCarAction);
        _carAgent.CheckpointReached(_markers[_currentMarkerIndex].gameObject.transform.position);
        _outerTrackTriggerCollector.SetTriggerEnterAction(OnCarTrackTrigger);
    }
    
    private void OnMoveCarAction(float acceleration, float steering)
    {
        _carScript.Move(acceleration);
        _carScript.SetWheelAngle(steering);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _carScript.Move(_forceToAdd);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _carScript.Move(-_forceToAdd);
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _carScript.SetWheelAngle(-10f / 30f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _carScript.SetWheelAngle(10f / 30f);
        }
    }
    
    /// <summary>
    /// The car hit a marker
    /// </summary>
    /// <param name="marker"></param>
    private void CarHitMarker(MarkerScript marker)
    {
        if (_currentMarkerIndex >= _markers.Length)
        {
            Debug.Log("We hit all the markers");
            
            return;
        }
        
        if (_markers[_currentMarkerIndex] == marker)
        {
            Debug.Log("We hit the correct marker, # " + _currentMarkerIndex);
            
            _currentMarkerIndex++;
            
            _carAgent.CheckpointReached(_markers[_currentMarkerIndex].gameObject.transform.position);
        }
    }
    
    private void OnCarTrackTrigger(Collider colider, bool isEnter)
    {
        Debug.Log("Track trigger changed: " + isEnter);
        
        if (!isEnter)
        {
            _carAgent.SetOnTrackIndication(false);
        }
    }
}
