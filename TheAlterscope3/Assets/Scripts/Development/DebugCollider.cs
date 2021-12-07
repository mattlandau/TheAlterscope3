using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collider");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");

    }

}
