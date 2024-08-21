using System;
using UnityEngine;
using UnityEngine.Events;


public class BreakingBlock : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _Break;
    Animator animator;
    public GameObject player;


    public void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<PlayerMovement>())
        {
            animator.SetBool("IsBreaking", true);
        }
    }
    public void Kill()
    {
        Destroy(gameObject);
    }

    public void FreezeToRegen()
    {
        this.GetComponent<Collider2D>().enabled = false;
        this.gameObject.GetComponent<Animator>().enabled = false;
        animator.SetBool("IsRegenereting", true);
        Invoke("Regen", 3.0f);

    }
    public void Regen()
    {
        this.gameObject.GetComponent<Animator>().enabled = true;
    }

    public void RegenToSolid()
    {
        animator.SetBool("IsRegenereting", false);
        animator.SetBool("IsBreaking", false);
        this.GetComponent<Collider2D>().enabled = true;
    }
}
