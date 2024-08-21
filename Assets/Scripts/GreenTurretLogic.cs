using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenTurretLogic : MonoBehaviour
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
        Audi.Play();
        //Debug.Log("fire dir " + fire.direction);
    }
}
