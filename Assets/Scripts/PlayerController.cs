using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float knockback;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;

    private bool isClingingWall = true;

    bool isJumping;
    bool knockbackeffect;
    //
    Vector2 movementDirection;
    Rigidbody2D rb;
    Animator animator;
    bool isFacingRight = true;
    //
    public static int points;
    public static int health = 3;
    public static int stars;

    static int sethealth;
    void Start()
    {
        sethealth = health;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //movement input
        if (knockbackeffect == false) { // je¿eli gracz zosta³ zaatakowany nie mo¿e siê ruszaæ dopóki nie dotknie "Ground"
            movementDirection.x = Input.GetAxisRaw("Horizontal");
        }
        //flip direction
        if(movementDirection.x < 0 && isFacingRight){
            Flip();
        } else if ( movementDirection.x > 0 && !isFacingRight)
        {
            Flip();
        }
        //animation
        animator.SetFloat("Speed", Mathf.Abs(movementDirection.x));
        //jumping
        if (isJumping == false && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private void FixedUpdate()
    {
        rb.position += speed * movementDirection * Time.deltaTime;
    }
    public void Jump()
    {
        movementDirection.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            //    if (isGrounded())
            //    {
            //        isJumping = true;
            //        animator.SetBool("IsJumping", true);
            //        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //    }
            //    else if (isClinging())
            //    {
            //        isJumping = true;
            //        animator.SetBool("IsJumping", true);
            //        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //    }
            //}

            //if (isGrounded() && !Input.GetButtonDown("Jump"))
            //{
            //    isJumping = false;
            //    animator.SetBool("IsJumping", false);
            //} 



            Statistics.stats[1] += 1;
            isJumping = true;
            animator.SetBool("IsJumping", true);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    //private bool isGrounded()
    //{
    //    return Physics2D.OverlapCircle(groundCheck.position, 1f, groundLayer);
    //}

    //private bool isClinging()
    //{
    //    return Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);
    //}

    void Die()
    {
        if (health <= 0)
        {
            health = sethealth;
            points = 0;
            stars = 0;

            Statistics.ResetStatistics();
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                isJumping = false;
                animator.SetBool("IsJumping", false);
                rb.velocity = new Vector2(0, 0);
                break;
            case "SilverCoin":
                PlayerPrefs.SetInt("CoinsCollected",1);
                Statistics.stats[3] += 1;
                points += 1;
                Destroy(collision.gameObject);
                break;
            case "GoldCoin":
                Statistics.stats[3] += 5;
                points += 5;
                Destroy(collision.gameObject);
                break;
            case "HealthUP":
                Statistics.stats[4] += 1;
                health += 1;
                Destroy(collision.gameObject);
                break;
            case "Star":
                Statistics.stats[2] += 1;
                stars += 1;
                Destroy(collision.gameObject);
                Debug.Log("stars: " + stars);
                break;
            case "NextLevel":
                SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case "Passage":
                collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(10f / 255f, 100f / 255f, 100f / 255f, 100f / 255f);
                break;
            case "Sign":
                collision.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ladder":
                isJumping = false;
                animator.SetBool("OnLadder", false);
                rb.gravityScale = 1;
                movementDirection.y = 0;
                break;
            case "Passage":
                collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                break;
            case "Sign":
                collision.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ladder":
                isJumping = true;
                animator.SetBool("OnLadder", true);
                movementDirection.y = Input.GetAxisRaw("Vertical");
                rb.gravityScale = 0;
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                knockbackeffect = false;
                isJumping = false;
                animator.SetBool("IsJumping", false);
                break;
            case "Obstacle":
                rb.velocity = new Vector2(Random.Range(-10, 10), Random.Range(2, 10));
                health -= 1;
                Die();
                break;
            case "Enemy":
                health -= 1;
                movementDirection.x = 0;
                rb.velocity = new Vector2(0, 0);
                knockbackeffect = true;
                if (isFacingRight) {
                    rb.velocity = new Vector2(knockback*-1, knockback / 2f);
                } else {
                    rb.velocity = new Vector2(knockback, knockback/2f);
                }
                Die();
                break;
        }
    }
}