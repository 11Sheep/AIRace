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
    /// Count the time in the wall so we can reset the car if needed
    /// </summary>
    private float _inWallCounter;
    
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
    
    public void OnReachedCorrectMarker(Transform nextMarker, int markerIndex)
    {
        AddRewardHelper(4f);

        _nextMarker = nextMarker;

        // TODO: just for testing in the beginning
        if (markerIndex == 10)
        {
            EndEpisode();
        }
    }
/*    
    public void OnReachedWrongCheckpoint()
    {
        AddRewardHelper(-1f);
        
        EndEpisode();
    }
  */  
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int fowrardAction = 0;
        int steeringAction = 0;

        if (Input.GetKey(KeyCode.UpArrow)) fowrardAction = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) fowrardAction = 2;
        else fowrardAction = 0;
        
        if (Input.GetKey(KeyCode.RightArrow)) steeringAction = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) steeringAction = 2;
        else steeringAction = 0;
     
        //Debug.Log("Heuristic: " + fowrardAction + " " + steeringAction);
        
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
        
        // Take out the y axis from the positions
        Vector3 nextMarkerPosition = new Vector3(_nextMarker.position.x, 0, _nextMarker.position.z);
        Vector3 carPosition = new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(carPosition, nextMarkerPosition);
        
        sensor.AddObservation(carPosition);
        sensor.AddObservation(nextMarkerPosition);
        sensor.AddObservation(distance);
        sensor.AddObservation(directionDot);
        
        // Show the directionDot in editor gizmo
        // Calculate the point of the directionDot
        

        //Debug.Log("directionDot: " + directionDot);
        
        
        //Debug.DrawLine(transform.position, transform.position + directionDot * 10, Color.red);
        
        // Canculate the distance between the car and the next marker without the y axis
        
        
        
        //float distance = Vector3.Distance(transform.position, _nextMarker.position);
        
        //sensor.AddObservation(directionDot);
        
        
        
        // Make the reward 1 as the car is closer to the marker
        //float rewardToAdd = (3f - distance / 10f) * 0.2f;
        //float
        
        //Debug.Log("Distance: " + distance + ", reward: " + rewardToAdd);
        
        //AddRewardHelper(rewardToAdd);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var input = actions.DiscreteActions;
        
        //Debug.Log("Action received: " + input[0]+ ", " + input[1]);
        
        float acceleration = 0;
        float steering = 0;
        
        switch (input[0])
        {
            case 1:
                acceleration = 1;
                break;
            case 2:
                acceleration = -1;
                break;
        }
        
        switch (input[1])
        {
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
            if (_inWallCounter + 5f < Time.time)
            {
                // Debug.Log("Car is stuck in the wall");
                
                AddRewardHelper(-3f);
                
                EndEpisode();
            }
            else
            {
                AddRewardHelper(-0.02f);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _inWallCounter = Time.time;
            
            AddRewardHelper(-0.5f);
        }
    }
    
    private void AddRewardHelper(float reward)
    {
        // Debug.Log("Adding reward: " + reward);
        
        AddReward(reward);
        
        // Print the total reward
        Debug.Log("Total reward: " + GetCumulativeReward());
    }

    public void OnReachedLastMarker()
    {
        // Finished track
        AddRewardHelper(10);
        
        EndEpisode();
    }
}
