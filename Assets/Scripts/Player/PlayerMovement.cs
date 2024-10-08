/*
	!*!&@==================================================@&!*!
	Created by @DawnosaurDev at youtube.com/c/DawnosaurStudios
	Check them out, they make amazing content :D
	!*!&@==================================================@&!*!
 */

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    //Scriptable object
    public PlayerData Data;
	Animator animator;
	SpriteRenderer Sprite;
	bool isMovingRight = true;
    public ParticleSystem dust;
	#region Variables
	//Components
	public Rigidbody2D RB { get; private set; }
	public WispManager wispManager;
    public GameObject UI;
	public GameObject deathScreen;
	public AudioSource DeathSound;
    public AudioSource Pose;
	public Sprite[] poses;
	public GameObject PoseBackground;

    public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsSliding { get; private set; }

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }

	//Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	//Wall Jump
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	private Vector2 _moveInput;
	public float LastPressedJumpTime { get; private set; }

    //Set all of these up in the inspector
    [Header("Checks")] 
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _iceLayer;
    #endregion

    private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		deathScreen = GameObject.Find("DeathScreen");
		deathScreen.SetActive(false);
		UI = GameObject.Find("UI");
		UI.SetActive(false);
		Sprite = GetComponent<SpriteRenderer>();
		GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
	}
	void OnDestroy()
	{
		GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
	}
    #region Pose Voids
    public void StrikeAPose()
    {
        PoseBackground.SetActive(true);
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        Pose.Play();
        int randomIndex = Random.Range(0, poses.Length);
        Sprite.sprite = poses[randomIndex];
        Invoke("ReturnToNormal", 0.6f);
    }
    public void ReturnToNormal()
    {
        PoseBackground.SetActive(false);
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.GetComponent<Animator>().enabled = true;
    }
    #endregion
    private void OnGameStateChanged(GameState newGameState)
	{
		enabled = newGameState == GameState.Gameplay;
		if (newGameState == GameState.Gameplay)
		{
			UI.SetActive(false);
		}
		else
		{
			UI.SetActive(true);
		}
	}
	private void Start()
	{
		SetGravityScale(Data.gravityScale);
		IsFacingRight = true;
		animator = GetComponent<Animator>();
		DeathSound = GetComponent<AudioSource>();
	}
    private void Update()
    {

        //checks direction of movement
        if (RB.velocity.x > 0.1)
        {
			isMovingRight = true;

        }		
		else if (RB.velocity.x < -0.1)
        {
			isMovingRight = false;
			
        }



		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER

		_moveInput.x = Input.GetAxisRaw("Horizontal");
		_moveInput.y = Input.GetAxisRaw("Vertical");

		//Debug.Log("Movement=" + _moveInput.x);
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		//Pose
        if (Input.GetKeyDown(KeyCode.LeftShift) && !PoseBackground.activeSelf && !animator.GetBool("Dead"))
        {
            this.gameObject.GetComponent<Animator>().enabled = false;
            Invoke("StrikeAPose", 0.1f);
        }

        if (_moveInput.x != 0)
		{
			CheckDirectionToFace();
			animator.SetFloat("Speed", 1);
		}
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
			animator.SetFloat("Speed", 0);
        }
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
			OnJumpInput();
        }

		if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
		{
			OnJumpUpInput();
            animator.SetBool("IsOnIce", false);
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
		{
			//Ground Check
			if ((Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping))//checks if set box overlaps with ground
            {
				LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
				animator.SetBool("IsJumping", false);
                animator.SetBool("IsOnIce", false);
                Data.runAccMul = 1f;

            }
			else if ((Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _iceLayer) && !IsJumping)) //Ice Ground Check
			{
                LastOnGroundTime = Data.coyoteTime;
                animator.SetBool("IsJumping", false);
				animator.SetBool("IsOnIce", true);
				Data.runAccMul = 0.05f;

            }
            else
			{

                Data.runAccMul = 1f;
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsOnIce", false);

            }
            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = Data.coyoteTime;
            //Right Wall Check Ice
            else if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _iceLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _iceLayer) && !IsFacingRight)) && !IsWallJumping)
            {
                LastOnWallRightTime = Data.coyoteTime;
            }

            //Left Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = Data.coyoteTime;
            //Left Wall Check Ice
            else if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _iceLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _iceLayer) && IsFacingRight)) && !IsWallJumping)
			{
                LastOnWallLeftTime = Data.coyoteTime;
			}

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;

			if(!IsWallJumping)
				_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
			_isJumpCut = false;

			if(!IsJumping)
				_isJumpFalling = false;
		}

		//Jump
		if (CanJump() && LastPressedJumpTime > 0)
		{
			IsJumping = true;
			IsWallJumping = false;
			_isJumpCut = false;
			_isJumpFalling = false;
			Jump();
			animator.SetBool("IsJumping", true);
		}
		//WALL JUMP
		else if (CanWallJump() && LastPressedJumpTime > 0)
		{
			IsWallJumping = true;
			IsJumping = false;
			_isJumpCut = false;
			_isJumpFalling = false;
			_wallJumpStartTime = Time.time;
			_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
			
			WallJump(_lastWallJumpDir);
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;
		#endregion

		#region GRAVITY
		//Higher gravity if we've released the jump input or are falling
		if (IsSliding)
		{
			SetGravityScale(0);
		}
		else if (RB.velocity.y < 0 && _moveInput.y < 0)
		{
			//Much higher gravity if holding down
			SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
			//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
		}
		else if (_isJumpCut)
		{
			//Higher gravity if jump button released
			SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
		}
		else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
		}
		else if (RB.velocity.y < 0)
		{
			//Higher gravity if falling
			SetGravityScale(Data.gravityScale * Data.fallGravityMult);
			//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
		}
		else
		{
			//Default gravity if standing on a platform or moving upwards
			SetGravityScale(Data.gravityScale);
		}
		#endregion
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.CompareTag("Projectile"))
		{
			animator.SetBool("Dead", true);
			DeathSound.Play();
			RB.constraints = RigidbodyConstraints2D.FreezeAll;
		}
	}

    public void Die()
	{
		deathScreen.SetActive(true);
		gameObject.SetActive(false);
	}

    private void FixedUpdate()
	{
		//RB.AddForce(Vector2.up * 10);

		//Handle Run
		if (IsWallJumping)
		{
			Run(Data.wallJumpRunLerp);
            animator.SetBool("OnLadder", false);
			animator.SetBool("IsJumping", true);
		}
		else
		{
			Run(1);
		}
		//Handle Slide
		if (IsSliding) 
		{ 
			Slide();
			animator.SetBool("OnLadder", true);
		}
		else
        {
			animator.SetBool("OnLadder", false);
        }
    }

    #region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
    public void OnJumpInput()
	{
		LastPressedJumpTime = Data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}
    #endregion

	//MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * Data.runMaxSpeed;
		//We can reduce our control using Lerp() this smooths changes to our direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.runAccMul : Data.runDeccelAmount * Data.runAccMul;
        else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir * Data.runAccMul : Data.runDeccelAmount * Data.deccelInAir * Data.runAccMul;
        #endregion
        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0; 
		}
        #endregion
		
        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Sprite.flipX = !Sprite.flipX;
		dust.Play();
	}
    #endregion

    #region JUMP METHODS
    private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		float force = Data.jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
    {
		

			//Ensures we can't call Wall Jump multiple times from one press
			LastPressedJumpTime = 0;
			LastOnGroundTime = 0;
			LastOnWallRightTime = 0;
			LastOnWallLeftTime = 0;

			#region Perform Wall Jump
			Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
			force.x *= dir; //apply force in opposite direction of wall

			if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
				force.x -= RB.velocity.x;

			if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
				force.y -= RB.velocity.y;

			//Unlike in the run we want to use the Impulse mode.
			//The default mode will apply are force instantly ignoring masss
			RB.AddForce(force, ForceMode2D.Impulse);
			#endregion
		
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		float speedDif = Data.slideSpeed - RB.velocity.y;	
		float movement = speedDif * Data.slideAccel;
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.up);
	}
	#endregion


	#region CHECK METHODS
	public void CheckDirectionToFace()
	{
		if (isMovingRight == false && Sprite.flipX == false)
		{
			Turn();
		}
		else if (isMovingRight == true && Sprite.flipX == true)
		{
			Turn();
		} 
	}

    private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanWallJump()
	{
			return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
				 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	
	private bool CanJumpCut()
    {
		return IsJumping && RB.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.velocity.y > 0;
	}

	public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}
    #endregion


 //   #region EDITOR METHODS
 //   private void OnDrawGizmosSelected()
 //   {
	//	Gizmos.color = Color.green;
	//	Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
	//	Gizmos.color = Color.blue;
	//	Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
	//	Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	//}
 //   #endregion
}

// created by Dawnosaur :D