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
    
    public void Initialize(Action<float, float> moveCar)
    {
        _moveCar = moveCar;
    }
    
    /// <summary>
    /// Set the absolute position of the next marker
    /// </summary>
    /// <param name="nextMarkerPosition"></param>
    public void CheckpointReached(Vector3 nextMarkerPosition)
    {
        _nextMarkerPosition = nextMarkerPosition;
        
        AddReward(5);
        
        Debug.Log("Checkpoint reached, +5 reward");
    }
    
    /// <summary>
    /// We get this indication from the GameManager when car is off track / on track
    /// </summary>
    /// <param name="onTrack"></param>
    public void SetOnTrackIndication(bool onTrack)
    {
        if (!onTrack)
        {
            AddReward(-1);
            
            Debug.Log("Car is off track, -1 reward");
            
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

    public override void CollectObservations(VectorSensor sensor)
    {
       sensor.AddObservation(_nextMarkerPosition);
       sensor.AddObservation(transform.position);
    }

    public override void OnEpisodeBegin()
    {
        // Reset the car position and state
        // TODO:
    }
}