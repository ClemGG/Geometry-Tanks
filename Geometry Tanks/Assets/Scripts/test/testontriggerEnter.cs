using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testontriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("on trigger enter");
    }

    private void OnCollisionEnter(Collision other)
    {
        print("on collision enter");
    }
}
