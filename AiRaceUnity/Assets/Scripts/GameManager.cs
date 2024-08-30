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
    /// Keep the original car transform so we can reset the car position
    /// </summary>
    private Transform _originalCarTransform; 
    
    private int _currentMarkerIndex = 0;

    /// <summary>
    /// This will count the time the car is off track
    /// </summary>
    private float _offTrackCounter = 0;

    private float _updateCarPositionForRewardTime = 0.2f;
    
    private void Start()
    {
        // Duplicate the transform by copying the values
        _originalCarTransform = new GameObject().transform;
        _originalCarTransform.position = _carScript.transform.position;
        _originalCarTransform.rotation = _carScript.transform.rotation;
        
        _carScript.Initialize(CarHitMarker, OnCarTrackTrigger);
        _carAgent.Initialize(OnMoveCarAction, OnResetEnv);
        _carAgent.ReachedMarker(_markers[_currentMarkerIndex].gameObject.transform.position, false);
    }
    
    private void OnMoveCarAction(float acceleration, float steering)
    {
        _carScript.Move(acceleration);
        _carScript.SetWheelAngle(steering);
    }
    
    private void OnResetEnv()
    {
        Debug.Log("Resetting the environment");
        
        _currentMarkerIndex = 0;
        _carAgent.ReachedMarker(_markers[_currentMarkerIndex].gameObject.transform.position, false);

        _carScript.transform.position = _originalCarTransform.position;
        _carScript.transform.rotation = _originalCarTransform.rotation;
        _carScript.ResetEnv();
    }

    // Update is called once per frame
    void Update()
    {
        if (_offTrackCounter > 0)
        {
            _offTrackCounter -= Time.deltaTime;

            if (_offTrackCounter <= 0)
            {
                _offTrackCounter = 0;
                
                _carAgent.SetOnTrackIndication(false);
            } 
        }
        
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

        _updateCarPositionForRewardTime -= Time.deltaTime;

        if (_updateCarPositionForRewardTime <= 0)
        {
            _updateCarPositionForRewardTime = 0.2f;
            
            _carAgent.UpdateCarPositionReward();
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
            
            _carAgent.ReachedMarker(_markers[_currentMarkerIndex].gameObject.transform.position, true);
        }
    }
    
    private void OnCarTrackTrigger(bool isEnter)
    {
        Debug.Log("Track trigger changed: " + isEnter);
        
        if (!isEnter)
        {
            _offTrackCounter = 0.3f;
        }
        else
        {
            _offTrackCounter = 0;
        }
    }
}
