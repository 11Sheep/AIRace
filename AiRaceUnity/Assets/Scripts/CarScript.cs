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
    
    [SerializeField] private Rigidbody _rigidbody;
    
    [SerializeField] private float _maxWheelAngel = 30;
    
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
        if (_rigidbody.velocity.magnitude < 10)
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
    }
    
    /// <summary>
    /// Set the angle of the front wheels between -1 and 1
    /// </summary>
    /// <param name="angle"></param>
    public void SetWheelAngle(float angle)
    {
        // Transform to "real" angle
        angle *= 30; 
        
        // Set the angle of the front wheels
        _frontWheelCurrentAngle += angle;
        
        // Verify that the angle is between -30 and 30
        if (_frontWheelCurrentAngle < -_maxWheelAngel)
        {
            _frontWheelCurrentAngle = -_maxWheelAngel;
        }
        else if (_frontWheelCurrentAngle > _maxWheelAngel)
        {
            _frontWheelCurrentAngle = _maxWheelAngel;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.angularVelocity = Vector3.zero;
        
        // Move the car forward or backward
        if (_currentForce != 0)
        {
            // Move forward or backward
            Vector3 forwardMovement = transform.forward * _currentForce * Time.fixedDeltaTime * 10;
            //_rigidbody.MovePosition(_rigidbody.position + forwardMovement);
            
            // Calculate the forward angle with 10 degreees to the right 
            //Vector3 rightMovement = Quaternion.Euler(0, 10, 0) * forwardMovement;

            float addToRotation = 0;
            
            if (_frontWheelCurrentAngle > 0)
            {
                addToRotation = 1;
            }
            else if (_frontWheelCurrentAngle < 0)
            {
                addToRotation = -1;
            }
            
            
            Quaternion rotation = Quaternion.Euler(0, addToRotation , 0); // 10 degrees rotation around the y-axis (right)
            Vector3 newDirection = rotation * transform.forward;

// Apply the new direction to the transform
            transform.forward = newDirection;
            
            
            //_rigidbody.AddForce(transform.forward * _currentForce * 100, ForceMode.Acceleration);
            transform.localPosition += transform.forward * _currentForce * Time.fixedDeltaTime * 10;
            
            //Debug.Log("transform.forward: " + transform.forward);
            
           
            
            // Create a gizmo for the forward movement
            //Debug.DrawRay(transform.position, forwardMovement, Color.yellow);
        }

        // Rotate the front wheels
        //transform.GetChild(0).localEulerAngles = new Vector3(0, _frontWheelCurrentAngle, 0);

        // Apply friction
        if (Math.Abs(_currentForce) <= _lowerForceLimitToStop)
        {
            _currentForce = 0;
            
            // Wheels come back to normal if stopped
            //_frontWheelCurrentAngle = 0;

            //Debug.Log("Current force: " + _currentForce + ", velocity: " + _rigidbody.velocity);
        }
        else if (_currentForce > 0)
        {
            _currentForce -= _fictionForce;
            
            //Debug.Log("Current force: " + _currentForce + ", velocity: " + _rigidbody.velocity);
        }
        else if (_currentForce < 0)
        {
            _currentForce += _fictionForce;
            
            //Debug.Log("Current force: " + _currentForce + ", velocity: " + _rigidbody.velocity);
        }
        
        // Return wheel to 0 angle
        if (Math.Abs(_frontWheelCurrentAngle) < _lowerWheelAngleToReset)
        {
            _frontWheelCurrentAngle = 0;
        }
        else if ((_frontWheelCurrentAngle > 0) && (_currentForce != 0))
        {
            _frontWheelCurrentAngle -= _wheelReturnForce;
            
            //Debug.Log("_frontWheelCurrentAngle: " + _frontWheelCurrentAngle);
        }
        else if ((_frontWheelCurrentAngle < 0) && (_currentForce != 0))
        {
            _frontWheelCurrentAngle += _wheelReturnForce;
            
            //Debug.Log("_frontWheelCurrentAngle: " + _frontWheelCurrentAngle);
        }
        
        // Turn the car only if the car is moving
        if ((_currentForce != 0) && (_frontWheelCurrentAngle != 0)) 
        {
            if (_frontWheelCurrentAngle > 0)
            {
                //transform.Rotate(0, _wheelRotetaForce, 0);    
            }
            else
            {
                //transform.Rotate(0, -_wheelRotetaForce, 0);
            }
        }
        
        // Set the rotation to _frontWheelCurrentAngle
        //transform.localEulerAngles = new Vector3(0, _frontWheelCurrentAngle, 0);

        // Get size of velocity
        float velocity = _rigidbody.velocity.magnitude;

        // Debug.Log("_frontWheelCurrentAngle: " + _frontWheelCurrentAngle + ", _currentForce: " + _currentForce + ", velocity: " + velocity);
    }

    private void TriggerEnterOnCar(Collider other, bool isEnter)
    {
        // Get the gameobject that the car collided with
        if (other.gameObject.GetComponent<MarkerScript>() != null)
        {
            MarkerScript marker = other.gameObject.GetComponent<MarkerScript>();
            
            _carHitMarker?.Invoke(marker);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("🎈MainTrack"))
        {
            Debug.Log("Entering main track");
        }
        //Debug.Log("collision enter: " + other.gameObject.name);
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag.Equals("🎈MainTrack"))
        {
            Debug.Log("Exiting main track");
        }
        //Debug.Log("collision exit: " + other.gameObject.name);
    }
}
