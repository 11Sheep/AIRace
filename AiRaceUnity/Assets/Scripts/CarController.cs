using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    /// <summary>
    /// Control the car ML agent
    /// </summary>
    [SerializeField] private CarDriverScript _carDriverScript;

    [SerializeField] private Rigidbody _rigidbody;
    
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    /// <summary>
    /// Markers on track for AI learning
    /// </summary>
    private MarkerScript[] _markers;

    private int _currentMarkerIndex; 
    
    private Vector3 _carPositionForChecking;

    private float _positionCheckTime;
    
    public void Initialize(MarkerScript[] markers)
    {
        _carDriverScript.Initialize(transform.position, transform.rotation, StopCar, markers[0].transform, MoveCar);
        _markers = markers;
        _currentMarkerIndex = 0;

        _carPositionForChecking = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _positionCheckTime = Time.time;
    }

    private void MoveCar(float acceleration, float steering)
    {
        verticalInput = acceleration;
        horizontalInput = steering;
    }

    private void StopCar()
    {
        _rigidbody.velocity = Vector3.zero;
        
        _currentMarkerIndex = 0;
    }


    private void FixedUpdate() {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void Update()
    {
        if (Time.time - _positionCheckTime > 5f)
        {
            // Check if the car was moved in the last 5 seconds
            
            if (Vector3.Distance(_carPositionForChecking, transform.position) < 0.2f)
            {
                Debug.Log("Car is stuck");
                
                // _carDriverScript.CarIsStuck();
            }

            // Reset the position and time check
            _positionCheckTime = Time.time;
            _carPositionForChecking = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }

    private void GetInput() {
        if (false)
        {
            // Steering Input
            horizontalInput = Input.GetAxis("Horizontal");

            // Acceleration Input
            verticalInput = Input.GetAxis("Vertical");

            if (Mathf.Abs(verticalInput) < 0.1f)
            {
                verticalInput = 0;
            }
            else if (verticalInput < 0)
            {
                verticalInput = -1;
            }
            else
            {
                verticalInput = 1;
            }

            if (Mathf.Abs(horizontalInput) < 0.1f)
            {
                horizontalInput = 0;
            }
            else if (horizontalInput < 0)
            {
                horizontalInput = -1;
            }
            else
            {
                horizontalInput = 1;
            }
        }

        // Debug.Log("Horizontal: " + horizontalInput + " Vertical: " + verticalInput);

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);
    }
    
    private void HandleMotor() {
        // Debug.Log("Speed: " + frontLeftWheelCollider.rpm +", motorTorque: " + verticalInput * motorForce);
        
        // Limit speed
        if (frontLeftWheelCollider.rpm < 1000)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
        
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();    
    }

    private void ApplyBreaking() {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering() {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels() {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform) {
        Vector3 pos;
        Quaternion rot; 
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Marker"))
        {
            if (other.GetComponent<MarkerScript>() == _markers[_currentMarkerIndex])
            {
                Debug.Log("Reached marker: " + _currentMarkerIndex);
                
                if (_currentMarkerIndex < _markers.Length - 1)
                {
                    _currentMarkerIndex++;
                }
                else
                {
                    Debug.Log("Reached last marker");
                    
                    _carDriverScript.OnReachedLastMarker();
                    
                    _currentMarkerIndex = 0;
                }
                
                _carDriverScript.OnReachedCorrectMarker(_markers[_currentMarkerIndex].transform, _currentMarkerIndex);
            }
            else
            {
                Debug.Log("Reached wrong marker");
                
                //_carDriverScript.OnReachedWrongCheckpoint();
            }
        }
    }
}