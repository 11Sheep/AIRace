using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarDriverScript : Agent
{
    /// <summary>
    /// Keep the original car position so we can reset the car position
    /// </summary>
    private Vector3 _originalCarPosition;
    
    /// <summary>
    /// Keep the original car rotation so we can reset the car rotation
    /// </summary>
    private Quaternion _originalCarRotation;

    private Action _stopCar;
    
    private Transform _nextMarker;
    
    private Action<float, float> _moveCar;

    public void Initialize(Vector3 originalCarPosition, Quaternion originalCarRotation, Action stopCar, Transform firstMarker, Action<float, float> MoveCar)
    {
        _originalCarPosition = originalCarPosition;
        _originalCarRotation = originalCarRotation;
        _stopCar = stopCar;
        _nextMarker = firstMarker;
        _moveCar = MoveCar;
    }
    
    public void OnReachedCorrectMarker(Transform nextMarker)
    {
        AddRewardHelper(1f);

        _nextMarker = nextMarker;
    }
    
    public void OnReachedWrongCheckpoint()
    {
        AddRewardHelper(-1f);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int fowrardAction = 0;
        int steeringAction = 0;
        
        if (Input.GetKey(KeyCode.UpArrow)) fowrardAction = 1;
        if (Input.GetKey(KeyCode.DownArrow)) fowrardAction = 2;
        
        if (Input.GetKey(KeyCode.LeftArrow)) steeringAction = 1;
        if (Input.GetKey(KeyCode.RightArrow)) steeringAction = 2;
        
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = fowrardAction;
        discreteActions[1] = steeringAction;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = _originalCarPosition;
        transform.rotation = _originalCarRotation;
        _stopCar?.Invoke();   
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 nextMarkerFW = _nextMarker.forward;
        float directionDot = Vector3.Dot(transform.forward, nextMarkerFW);
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float acceleration = 0;
        float steering = 0;
        
        Debug.Log("Action: " + actions.DiscreteActions[0] + ", " + actions.DiscreteActions[1]);
        
        switch (actions.DiscreteActions[0])
        {
            case 0:
                acceleration = 0;
                break;
            case 1:
                acceleration = 1;
                break;
            case 2:
                acceleration = -1;
                break;
        }
        
        switch (actions.DiscreteActions[1])
        {
            case 0:
                steering = 0;
                break;
            case 1:
                steering = 1;
                break;
            case 2:
                steering = -1;
                break;
        }
        
        _moveCar?.Invoke(acceleration, steering);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            AddRewardHelper(-0.1f);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            AddRewardHelper(-0.5f);
        }
    }

    private void AddRewardHelper(float reward)
    {
        Debug.Log("Adding reward: " + reward);
        
        AddReward(reward);
    }
}
