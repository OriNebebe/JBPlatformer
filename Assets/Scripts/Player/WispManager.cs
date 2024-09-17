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


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        col.enabled = false;
    }
 
    // Update is called once per frame
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
    //public virtual void OnTriggerEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
