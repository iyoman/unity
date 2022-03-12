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

    [SerializeField] Transform ParticleCubeTransform;
    [SerializeField] Transform symmetryPosition;

    ParticleSystem.Particle[] particleArray;

    Vector3[] posArray;
    Vector3 worldPosition;
    Vector3 mouseWorldTemp;

    int mouseTicker = 0;
    bool firstClick = true;

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

        ps.Emit(s_count);
        ps.GetParticles(particleArray);

        for (int i = 0; i < posArray.Length; i++)
        {
            particleArray[i].position = posArray[i];
        }
        ps.SetParticles(particleArray, particleArray.Length);

        //PrintArray(posArray);
        //Debug.Log(particleArray[0]);
    }

    void Update()
    {
        ps.GetParticles(particleArray);
        for (int i = 0; i < posArray.Length; i++)
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

        int mouseTickerTarg = 7;
        if (Input.GetMouseButton(0))
        {
            // mouseTicker = mouseTickerTarg;
            if (mouseWorldTemp != worldPosition)
            {
                if (++mouseTicker >= mouseTickerTarg || firstClick == true)
                {
                    mouseTicker = 0;

                    //add particles below
                    ps.Emit(1);
                    Array.Resize<Vector3>(ref posArray, posArray.Length+1);
                    Array.Resize<ParticleSystem.Particle>(ref particleArray, particleArray.Length+1);
                    ps.GetParticles(particleArray);
                    particleArray[particleArray.Length-1].position = worldPosition;
                    ps.SetParticles(particleArray, particleArray.Length);
                    
                    firstClick = false;


                    Debug.Log($"X: {mousePos.x}    Y: {mousePos.y}");
                }
            }
            mouseWorldTemp = worldPosition;
        } else {
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
    }
}
