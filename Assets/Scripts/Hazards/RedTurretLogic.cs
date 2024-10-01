using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedTurretLogic : MonoBehaviour
{
    public ProjectileBehaviour ProjPrefab;
    public Transform LaunchOffset;
    public Vector3 AimDir;
    public AudioSource Audi;

    void Start()
    {
        Audi = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void Spawn()
    {
        ProjectileBehaviour fire = Instantiate(ProjPrefab, LaunchOffset.position, transform.rotation).GetComponent<ProjectileBehaviour>();
        fire.SetDir(AimDir);
        //Debug.Log("fire dir " + fire.direction);
        Audi.Play();

    }
}
