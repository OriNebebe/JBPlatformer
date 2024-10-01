using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WispManager : MonoBehaviour
{
    BoxCollider2D col;
    Animator animator;
    public int requiredWisps;
    public int wispCount;
    public GameObject door;
    public AudioSource power;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        col.enabled = false;
    }
 
    void Update()
    {
        if(wispCount >= requiredWisps)
        {
            door.GetComponent<Animator>().SetBool("GotAllWisps", true);
            col.enabled = true;
        }
    }

    void Power()
    {
        power.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
