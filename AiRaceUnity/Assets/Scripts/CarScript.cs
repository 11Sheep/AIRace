using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    [SerializeField] private float _maxForce = 1;
    
    [SerializeField] private float _lowerForceLimitToStop = 0.0005f;
    
    [SerializeField] private float _fictionForce = 0.01f;
    
    [SerializeField] private float _lowerWheelAngleToReset = 0.5f;
    
    [SerializeField] private float _wheelReturnForce = 0.1f;
    
    [SerializeField] private float _wheelRotetaForce = 0.5f;
    
    [SerializeField] private TriggerCollector _triggerCollector;
    
    /// <summary>
    /// Car force
    /// </summary>
    private float _currentForce = 0;
    
    /// <summary>
    /// Front wheel current angle
    /// </summary>
    private float _frontWheelCurrentAngle = 0;
    
    /// <summary>
    /// Callback for the parent to know when the car hit a marker
    /// </summary>
    private Action<MarkerScript> _carHitMarker;
    
    private void Start()
    {
        _triggerCollector.SetTriggerEnterAction(TriggerEnterOnCar);
    }
    
    public void Initialize(Action<MarkerScript> carHitMarker)
    {
        _carHitMarker = carHitMarker;
    }
    
    /// <summary>
    /// Move the car forward or backward, force is between -1 to 1
    /// </summary>
    /// <param name="force"></param>
    public void Move(float force)
    {
        _currentForce += force;

        // Limit the force to the maximum force
        if (_currentForce > _maxForce)
        {
            _currentForce = _maxForce;
        }
        else if (_currentForce < -_maxForce)
        {
            _currentForce = -_maxForce;
        }
    }
    
    /// <summary>
    /// Set the angle of the front wheels between -30 and 30
    /// </summary>
    /// <param name="angle"></param>
    public void SetWheelAngle(float angle)
    {
        // Verify that the angle is between -30 and 30
        if (angle < -30)
        {
            angle = -30;
        }
        else if (angle > 30)
        {
            angle = 30;
        }
        
        // Set the angle of the front wheels
        _frontWheelCurrentAngle = angle;
    }

    private void FixedUpdate()
    {
        // Move the car forward or backward
        transform.position += transform.forward * _currentForce;
        
        // Rotate the front wheels
        //transform.GetChild(0).localEulerAngles = new Vector3(0, _frontWheelCurrentAngle, 0);

        // Apply friction
        if (Math.Abs(_currentForce) <= _lowerForceLimitToStop)
        {
            _currentForce = 0;
        }
        else if (_currentForce > 0)
        {
            _currentForce -= _fictionForce;
            
            //Debug.Log("Current force: " + _currentForce);
        }
        else if (_currentForce < 0)
        {
            _currentForce += _fictionForce;
            
            //Debug.Log("Current force: " + _currentForce);
        }
        
        // Return wheel to 0 angle
        if (Math.Abs(_frontWheelCurrentAngle) < _lowerWheelAngleToReset)
        {
            _frontWheelCurrentAngle = 0;
        }
        else if (_frontWheelCurrentAngle > 0)
        {
            _frontWheelCurrentAngle -= _wheelReturnForce;
            
            //Debug.Log("_frontWheelCurrentAngle: " + _frontWheelCurrentAngle);
        }
        else if (_frontWheelCurrentAngle < 0)
        {
            _frontWheelCurrentAngle += _wheelReturnForce;
            
            //Debug.Log("_frontWheelCurrentAngle: " + _frontWheelCurrentAngle);
        }
        
        // Turn the car only if the car is moving
        if ((_currentForce != 0) && (_frontWheelCurrentAngle != 0)) 
        {
            if (_frontWheelCurrentAngle > 0)
            {
                transform.Rotate(0, _wheelRotetaForce, 0);    
            }
            else
            {
                transform.Rotate(0, -_wheelRotetaForce, 0);
            }
            
        }

    }

    private void TriggerEnterOnCar(Collider other)
    {
        // Get the gameobject that the car collided with
        if (other.gameObject.GetComponent<MarkerScript>() != null)
        {
            MarkerScript marker = other.gameObject.GetComponent<MarkerScript>();
            
            _carHitMarker?.Invoke(marker);
        }
    }
}
