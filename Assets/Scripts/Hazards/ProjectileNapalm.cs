using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileNapalm : ProjectileBehaviour
{
    public GameObject napalm;
    public GameObject player;
    

    public void Awake()
    {
        player = GameObject.Find("Player");
    }

    public override void Start()
    {
        base.Start();
        if (player != null)
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<CircleCollider2D>(), player.GetComponent<CapsuleCollider2D>());
        }
        transform.Rotate(0,0,90);
        InvokeRepeating("SmellOfNapalm", 0,1);
    }

    public override void Update()
    {
        base.Update();
        
    }

    public override void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Ground"))
        {
            Kill();
        }
        
    }

    public void SmellOfNapalm()
    {
        Instantiate(napalm, gameObject.transform.position, transform.rotation);
    }


}
