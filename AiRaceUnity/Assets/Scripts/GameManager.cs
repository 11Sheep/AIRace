using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CarScript _carScript;

    [SerializeField] private float _forceToAdd = 0.001f;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _carScript.Move(_forceToAdd);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _carScript.Move(-_forceToAdd);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            _carScript.SetWheelAngle(-10);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _carScript.SetWheelAngle(10);
        }
    }
}
