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
    
    /// <summary>
    /// This will be used to 
    /// </summary>
    [SerializeField] private TriggerCollector _outerTrackTriggerCollector;
    
    private int _currentMarkerIndex = 0;
    
    private void Start()
    {
        _carScript.Initialize(CarHitMarker);
        _outerTrackTriggerCollector.SetTriggerEnterAction(OnCarTrackTrigger);
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
            _carScript.SetWheelAngle(-10);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
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
        if (_currentMarkerIndex >= _markers.Length)
        {
            Debug.Log("We hit all the markers");
            
            return;
        }
        
        if (_markers[_currentMarkerIndex] == marker)
        {
            Debug.Log("We hit the correct marker, # " + _currentMarkerIndex);
            
            _currentMarkerIndex++;
        }
    }
    
    private void OnCarTrackTrigger(Collider colider, bool isEnter)
    {
        Debug.Log("Track trigger changed: " + isEnter);
    }
}
