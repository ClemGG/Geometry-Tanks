﻿using UnityEngine;



public class CameraShake : MonoBehaviour {

    [Tooltip("Utilisé seulement pour les tests.")]
    [SerializeField] KeyCode toucheActivation;


    [Range(0f,1f)][SerializeField] float power = .7f;
    [Range(0f,10f)] [SerializeField] float duration = 1f;

    [Tooltip("(Smooth only) Setting this too high or too low will cause a gap in the rate between the power and the duration.")]
    [Range(0f, 3f)] [SerializeField] float slowDownAmount = 1f;

    [Space]

    bool shouldShake = false;
    [Tooltip("The smooth will be faster and will not depend on the duration of the quake.")]
    [SerializeField] bool useSmoothShake = false;
    [Tooltip("The smooth will be slower, based on the duration.")]
    [SerializeField] bool useSmoothShakeDependingOnDuration = false;
    [Tooltip("Do you want the camera to follow a 2D or 3D movement ?")]
    [SerializeField] bool useCircleInsteadOfSphere = false;

    Vector3 StartPos;
    float initialDuration;
    float initialPower;
    Camera cam;
    Transform t;

    public static CameraShake instance;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }


    // Use this for initialization
    void Start () {
        t = transform;
        cam = GetComponent<Camera>();
        Init();
        
    }


    // Update is called once per frame
    void Update () {

        //DetectCommand();
        ShakeTheCamera();

        

    }

    public void Shake()
    {
        Reset();
        Init();
        shouldShake = true;
    }

    void DetectCommand()
    {
        if (Input.GetKeyDown(toucheActivation))
        {
            if (!shouldShake)
            {
                Reset();
                Init();
                shouldShake = true;
            }
            else
            {
                Reset();
            }
        }
    }

    void ShakeTheCamera()
    {
        Vector3 shakePos = StartPos + transform.position;

        if (shouldShake)
        {
            if (duration > 0 && power > 0)
            {
                t.position = shakePos + ((useCircleInsteadOfSphere == true) ? (Vector3)Random.insideUnitCircle : Random.insideUnitSphere) * power;

                power -= (useSmoothShake == true) ? ((useSmoothShakeDependingOnDuration == true) ? Time.fixedDeltaTime / duration * power : Time.fixedDeltaTime / duration)
                                                  : ((useSmoothShakeDependingOnDuration == true) ? Time.fixedDeltaTime / duration * power : 0f);
                duration -= Time.fixedDeltaTime * slowDownAmount;
            }
            else
            {
                Reset();
            }
        }
    }


    void Init()
    {
        StartPos = t.position;
        initialDuration = duration;
        initialPower = power;
    }

    void Reset()
    {
        shouldShake = false;
        duration = initialDuration;
        power = initialPower;
        t.position = StartPos;
    }
}
