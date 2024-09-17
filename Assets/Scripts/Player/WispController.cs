using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WispController : MonoBehaviour
{
    public WispManager wispManager;
    Animator animator;
    public AudioSource Collected;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isCollected", true);
            this.GetComponent<Collider2D>().enabled = false;
            Collected.Play();
            wispManager.wispCount++;
        }

       
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        wispManager = GameObject.FindObjectOfType<WispManager>();
        Collected = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
