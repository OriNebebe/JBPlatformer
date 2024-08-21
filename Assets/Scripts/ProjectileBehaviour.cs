using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public Vector3 direction;
    
    public float Speed = 1f;
    Animator anim;
    public virtual void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    public virtual void Update()
    {
        transform.position += direction * Time.deltaTime * Speed;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision2D)
    {
        if(collision2D.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("didCollide", true);
        }
        else if(collision2D.gameObject.CompareTag("Player"))
        {
            anim.SetBool("didCollide", true);
        }
        
    }

    public void SetDir(Vector3 newDir)
    {
        direction = newDir;
        Quaternion newRot = Quaternion.LookRotation(transform.forward, -newDir);

        transform.rotation = newRot;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
    
}
