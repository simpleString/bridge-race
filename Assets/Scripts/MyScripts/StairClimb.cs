using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairClimb : MonoBehaviour
{
    Rigidbody rigidBody;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] LayerMask stairsLayer;
    [SerializeField] LayerMask bridgeLayer;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }
    
    private void FixedUpdate()
    {
        stepClimb();
    }

    void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, stairsLayer) ||
        (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, bridgeLayer)))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f, stairsLayer) || 
            (Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f, bridgeLayer)))
            {
                Debug.Log("first");
                //rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                rigidBody.MovePosition(transform.position + Vector3.up * stepSmooth * Time.fixedDeltaTime);
            }
        }

        //  RaycastHit hitLower45;
        // if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f, stairsLayer))
        // {

        //     RaycastHit hitUpper45;
        //     if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f,0,1), out hitUpper45, 0.2f, stairsLayer))
        //     {
        //         Debug.Log("second");
        //         rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
        //     }
        // }

        // RaycastHit hitLowerMinus45;
        // if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f,0,1), out hitLowerMinus45, 0.1f, stairsLayer))
        // {

        //     RaycastHit hitUpperMinus45;
        //     if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f,0,1), out hitUpperMinus45, 0.2f, stairsLayer))
        //     {
        //         Debug.Log("third");
        //         rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
        //     }
        // }
    }
}