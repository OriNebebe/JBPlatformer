using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpingChest : MonoBehaviour
{
    public ParticleSystem fumes;
    Animator animator;
    public GameObject player;
    public float bounce;
    public AudioSource sound;
    public void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<PlayerMovement>())
        {
            animator.SetBool("SteppedOn", true);
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            this.GetComponent<Collider2D>().enabled = false;
            sound.Play();
        }
    }
    public void Boing()
    {
        fumes.Play();
        animator.SetBool("SteppedOn", false);
        animator.SetBool("Boing", true);

    }

    public void Close()
    {
        animator.SetBool("Boing", false);
        this.GetComponent<Collider2D>().enabled = true;
    }
}
