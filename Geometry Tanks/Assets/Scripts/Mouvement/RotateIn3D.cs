using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIn3D : MonoBehaviour {

    [SerializeField] private float speedInX = 5f;
    [SerializeField] private float speedInY = 5f;
    [SerializeField] private float speedInZ = 5f;

    [Space]

    [Header("Rotate Along : ")]

    [SerializeField] private bool useLocal = true;
    [SerializeField] private bool X;
    [SerializeField] private bool Y;
    [SerializeField] private bool Z;


    private Transform t;
    private Quaternion startRot;

    private void Start()
    {
        t = transform;
        startRot = t.rotation;
    }


    // Update is called once per frame
    void Update () {

#pragma warning disable CS0618 // Le type ou le membre est obsolète

        if (useLocal)
        {
            if (X)
                t.RotateAround(t.right, speedInX * Time.deltaTime);
            if (Y)
                t.RotateAround(t.up, speedInX * Time.deltaTime);
            if (Z)
                t.RotateAround(t.forward, speedInZ * Time.deltaTime);
        }
        else
        {
            if (X)
                t.RotateAround(Vector3.right, speedInX * Time.deltaTime);
            if (Y)
                t.RotateAround(Vector3.up, speedInX * Time.deltaTime);
            if (Z)
                t.RotateAround(Vector3.forward, speedInZ * Time.deltaTime);
        }

#pragma warning restore CS0618 // Le type ou le membre est obsolète


        if (!X && !Y && !Z)
        {
            t.rotation = startRot;
        }
    }
}
