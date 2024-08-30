using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    private Vector3 _nextMarkerPosition;
    
    /// <summary>
    /// Callback to move the car based on the decision
    /// </summary>
    private Action<float, float> _moveCar;
    
    /// <summary>
    /// Callback to reset the enviournment
    /// </summary>
    private Action _resetEnv; 
    
    public void Initialize(Action<float, float> moveCar, Action resetEnv)
    {
        _moveCar = moveCar;
        _resetEnv = resetEnv;
    }
    
    /// <summary>
    /// Set the absolute position of the next marker
    /// </summary>
    /// <param name="nextMarkerPosition"></param>
    public void ReachedMarker(Vector3 nextMarkerPosition, bool givePrize)
    {
        _nextMarkerPosition = nextMarkerPosition;

        if (givePrize)
        {
            Debug.Log("Checkpoint reached");
            
            AddRewardHelperFuncion(5);
        }
    }
    
    /// <summary>
    /// We get this indication from the GameManager when car is off track / on track
    /// </summary>
    /// <param name="onTrack"></param>
    public void SetOnTrackIndication(bool onTrack)
    {
        if (!onTrack)
        {
            Debug.Log("Car is off track");
            
            AddRewardHelperFuncion(-1);
            
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> countinuesActions = actionsOut.ContinuousActions;
        countinuesActions[0] = Input.GetAxis("Vertical");
        countinuesActions[1] = Input.GetAxis("Horizontal");
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        float acceleration = vectorAction.ContinuousActions[0];
        float steering = vectorAction.ContinuousActions[1];
        
        _moveCar?.Invoke(acceleration, steering);
    }
    
    public void UpdateCarPositionReward()
    {
        float distance = Vector3.Distance(transform.position, _nextMarkerPosition);
        float maxDistance = 10f;
        
        if (distance > maxDistance)
        {
            distance = maxDistance;
        }
        
        // Normalize the reward so it's between 0 and 0.2f, closer to the marker, higher the reward
        float rewardToAdd = (1f - distance / maxDistance) * 0.2f;
        
        Debug.Log("Distance: " + distance + ", reward: " + rewardToAdd);
        
        AddRewardHelperFuncion(rewardToAdd);
    }

    private void AddRewardHelperFuncion(float reward)
    {
        Debug.Log("Adding reward: " + reward);
        
        AddReward(reward);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       sensor.AddObservation(_nextMarkerPosition);
       sensor.AddObservation(transform.position);
    }

    public override void OnEpisodeBegin()
    {
        // Reset the car position and state
        _resetEnv?.Invoke();
    }
}