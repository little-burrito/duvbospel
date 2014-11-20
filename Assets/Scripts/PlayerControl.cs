using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.


	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.


	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
    
	private Transform weapon;   			// Player weapon

    private float horizontalInput = 0.0f;

    private Transform soundWoosh;


	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		weapon = transform.Find("weapon");
		anim = GetComponent<Animator>();
        soundWoosh = transform.Find("soundWoosh");
	}


	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  

		// Cache the horizontal input.
		horizontalInput = Input.GetAxis("Horizontal");

        bool jumpTouchInput = false;

        // Touch input overrides the horizontal input.
        for ( int i = 0; i < Input.touchCount; i++ ) {
            Touch touch = Input.GetTouch( i );
            if ( touch.deltaPosition.y > 20.0f && Mathf.Abs( touch.deltaPosition.x ) < 30.0f ) {
                jumpTouchInput = true;
            } else if ( touch.position.x < Screen.width / 5 ) {
                horizontalInput = -1.0f;
            } else if ( touch.position.x > ( Screen.width / 5 ) * 4 ) {
                horizontalInput = 1.0f;
            }

            if ( touch.position.x > ( Screen.width / 5 ) * 1.5 && touch.position.x < ( Screen.width / 5 ) * 3.5 && touch.phase == TouchPhase.Began ) {
                anim.SetBool( "Attack", true );
            } else {
                anim.SetBool( "Attack", false );
            }
        }

		// If the jump button is pressed and the player is grounded then the player should jump.
        if ( ( jumpTouchInput || Input.GetButtonDown( "Jump" ) ) && grounded ) {
            jump = true;
        }
        if ( grounded ) {
            anim.SetBool( "Jump", false );
        }

        anim.SetFloat("VerticalSpeed", this.rigidbody2D.velocity.y);
	}


	void FixedUpdate ()
	{
		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat("Speed", Mathf.Abs(horizontalInput));

		// If the player is changing direction (horizontalInput has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(horizontalInput * rigidbody2D.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody2D.AddForce(Vector2.right * horizontalInput * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

		// If the input is moving the player right and the player is facing left...
		if(horizontalInput > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(horizontalInput < 0 && facingRight)
			// ... flip the player.
			Flip();

		// If the player should jump...
		if(jump)
		{
			// Set the Jump animator trigger parameter.
            anim.SetBool("Jump", true);

			// Play a random jump audio clip.
			int i = Random.Range(0, jumpClips.Length);
			AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

			// Add a vertical force to the player.
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
			if(!audio.isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();

				// Play the new taunt.
				audio.clip = taunts[tauntIndex];
				audio.Play();
			}
		}
	}


	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}

    void playSoundWoosh() {
        soundWoosh.audio.Play();
    }
}
