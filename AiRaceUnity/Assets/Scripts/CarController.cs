using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    /// <summary>
    /// Control the car ML agent
    /// </summary>
    [SerializeField] private CarDriverScript _carDriverScript;
    
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
    
    public void Initialize(MarkerScript[] markers)
    {
        _carDriverScript.Initialize(transform.position, transform.rotation, StopCar, markers[0].transform, MoveCar);
        _markers = markers;
        _currentMarkerIndex = 0;
    }

    private void MoveCar(float acceleration, float steering)
    {
        verticalInput = acceleration;
        horizontalInput = steering;
    }

    private void StopCar()
    {
        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
        rearLeftWheelCollider.motorTorque = 0;
        rearRightWheelCollider.motorTorque = 0;
        
        _currentMarkerIndex = 0;
    }


    private void FixedUpdate() {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
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