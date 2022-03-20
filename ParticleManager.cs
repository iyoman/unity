using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleManager : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps;

    [SerializeField] float s_life;
    [SerializeField] int s_count;
    [SerializeField] int symmetry;
    public int frequency;
    public BezierSpline spline;
    public BezierSpline[] splineList;
    public float lifetimeOffset;
    public int lerpTimes;

    [SerializeField] Transform ParticleCubeTransform;
    [SerializeField] Transform symmetryPosition;

    ParticleSystem.Particle[] particleArray;

    Vector3[] posArray;
    Vector3 worldPosition;
    Vector3 mouseWorldTemp;

    int mouseTicker = 0;
    bool firstClick = true;
    int mouseDownCount = 0;

    private bool started = false;

    void PrintArray(Vector3[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            Debug.Log(arr[i]);
        }
    }

    // void addParticle(Vector3[]) {

    // }

    void Start()
    {
        Invoke("StartDelayed", 1);
    }

    void StartDelayed()
    {
        particleArray = new ParticleSystem.Particle[s_count];
        posArray = new Vector3[s_count];

        for (int i = 0; i < s_count; i++)
        {
            // Debug.Log("hello");
            posArray[i] = UnityEngine.Random.insideUnitCircle * 5;
        }

        var main = ps.main;
        // var emitParams = new ParticleSystem.EmitParams();
        main.startLifetime = s_life;

        //old stuff
        ps.Emit(s_count);
        ps.GetParticles(particleArray);

        for (int i = 0; i < posArray.Length; i++)
        {
            particleArray[i].position = posArray[i];
        }
        ps.SetParticles(particleArray, particleArray.Length);



        for (int j = 0; j < splineList.Length; j++)
        {
            var currentSpline = splineList[j];
            // this didn't work in particlesplinemanager don't know why
            for (int k = 0; k < lerpTimes; k++)
            {
                
            }
            if (frequency <= 0)
            {
                return;
            }
            float stepSize = 1f / (frequency);
            for (int i = 0; i < frequency; i++)
            {
                Vector3 position = currentSpline.GetPoint(i * stepSize);

                ps.Emit(1);
                Array.Resize<ParticleSystem.Particle>(ref particleArray, particleArray.Length + 1);
                ps.GetParticles(particleArray);
                var particle = particleArray[particleArray.Length - 1];
                particleArray[particleArray.Length - 1].position = position;

                float life = s_life - (lifetimeOffset * i);
                if (life == 0)
                {
                    life = 0.05f;
                }
                while (life < 0)
                {
                    life += s_life;
                }

                // Debug.Log("first i = "+i+"    "+life);
                particleArray[particleArray.Length - 1].remainingLifetime = life;
                ps.SetParticles(particleArray, particleArray.Length);
                // Debug.Log("second i = "+i+"    "+life);
            }
        }

        //PrintArray(posArray);
        //Debug.Log(particleArray[0]);
        started = true;
    }

    void Update()
    {
        if (started == false)
        {
            return;
        }
        ps.GetParticles(particleArray);
        for (int i = 0; i < particleArray.Length; i++)
        {
            // Debug.Log(particleArray[i].remainingLifetime);
            if (particleArray[i].remainingLifetime <= 0.1)
            {
                particleArray[i].remainingLifetime = s_life;
            }
        }
        ps.SetParticles(particleArray, particleArray.Length);


        //input
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -1 * Camera.main.transform.position.z;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        ParticleCubeTransform.position = worldPosition;

        int mouseTickerTarg = 0;

        if (Input.GetMouseButton(0))
        {
            // mouseTicker = mouseTickerTarg;
            if (mouseWorldTemp != worldPosition)
            {
                if (++mouseTicker >= mouseTickerTarg || firstClick == true)
                {
                    if (firstClick == true)
                    {
                        mouseDownCount = 0;
                    }
                    mouseDownCount++;
                    mouseTicker = 0;

                    //add particles below
                    ps.Emit(symmetry);
                    Array.Resize<Vector3>(ref posArray, posArray.Length + symmetry);
                    Array.Resize<ParticleSystem.Particle>(ref particleArray, particleArray.Length + symmetry);
                    ps.GetParticles(particleArray);
                    particleArray[particleArray.Length - symmetry].position = worldPosition;
                    for (int i = 1; i < symmetry; i++)
                    {
                        symmetryPosition.transform.position = worldPosition;
                        symmetryPosition.transform.RotateAround(Vector3.zero, Vector3.back, i * (360 / symmetry));
                        particleArray[particleArray.Length - symmetry + i].position = symmetryPosition.transform.position;
                    }
                    ps.SetParticles(particleArray, particleArray.Length);

                    firstClick = false;


                    // Debug.Log($"X: {mousePos.x}    Y: {mousePos.y}");
                }
            }
            mouseWorldTemp = worldPosition;
        }
        else
        {
            firstClick = true;
        }

        //debug press ctrl
        if (Input.GetButtonDown("Fire2"))
        {
            {
                // Debug.Log("---------------------");
                // Debug.Log("particlearray length = " + particleArray.Length);
                // Debug.Log("mPos " + mousePos);
                // Debug.Log(mouseWorldTemp);
                // Debug.Log(worldPosition);
                // Debug.Log("---------------------");
                // float angle = Vector3.SignedAngle(worldPosition, Vector3.left, Vector3.forward);
                // Debug.Log(angle);
                // Vector3 testPos = Vector3.forward;
                // testPos.RotateAround(Vector3.zero, Vector3.back, 0);
            }
        }
        if (Input.GetKeyDown("r"))
        {
            particleArray = new ParticleSystem.Particle[0];
            // Array.Resize<ParticleSystem.Particle>(ref particleArray, particleArray.Length-mouseDownCount);
        }
    }
}
