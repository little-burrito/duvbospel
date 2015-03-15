using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public float moveSpeed = 2f;		// The speed the enemy moves at.
	public int HP = 2;					// How many times the enemy can be hit before it dies.
	public Sprite deadEnemy;			// A sprite of the enemy when it's dead.
	public Sprite damagedEnemy;			// An optional sprite of the enemy when it's damaged.
	public AudioClip[] deathClips;		// An array of audioclips that can play when the enemy dies.
	public float deathSpinMin = -100f;			// A value to give the minimum amount of Torque when dying
	public float deathSpinMax = 100f;			// A value to give the maximum amount of Torque when dying


	private SpriteRenderer ren;			// Reference to the sprite renderer.
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.
	private bool dead = false;			// Whether or not the enemy is dead.

	private bool grounded = false;			// Whether or not the player is grounded.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.

    private float horizontalInput = 0.0f;

    private GameObject player;
    public float maxSpeed = 10.0f;
    public float moveForce = 5000.0f;

    public float health = 100.0f;

    public float attackDistance = 15.0f;
    public float attackDamage = 15.0f;
    public float attackTime = 200.0f;
    public float attackKnockback = 0.5f;
    public float attackCooldown = 0.0f;

    private Transform soundScare;
    private Transform soundTakeDamage;
    private Transform soundDefeat;
    private Transform soundAttack;
    private bool didPlayDefeatSound;
    private bool isAttacking = false;

    [HideInInspector]
    public bool facingRight = true;

    private Animator anim;
	
	void Awake()
	{
		// Setting up the references.
		//ren = transform.Find("body").GetComponent<SpriteRenderer>();
		//frontCheck = transform.Find("frontCheck").transform;
        anim = GetComponent<Animator>();
		groundCheck = transform.Find("groundCheck");
        player = GameObject.FindGameObjectWithTag( "Player" );
        soundScare = transform.Find( "SoundScare" );
        soundTakeDamage = transform.Find( "SoundTakeDamage" );
        soundDefeat = transform.Find( "SoundDefeat" );
        soundAttack = transform.Find( "SoundAttack" );
        PlayerControl playerControl = player.GetComponent<PlayerControl>();
        playerControl.numEnemies++;
	}

    void Update() {
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        if ( health <= 0 ) {
            this.collider2D.enabled = false;
            if ( !didPlayDefeatSound ) {
                playSoundDefeat();
                didPlayDefeatSound = true;
                PlayerControl playerControl = player.GetComponent<PlayerControl>();
                playerControl.defeatedEnemies++;
            }
            if ( transform.position.y < -1000.0f ) {
                Destroy( this );
            }
        } else {
            grounded = Physics2D.Linecast( transform.position, groundCheck.position, 1 << LayerMask.NameToLayer( "Ground" ) );

            PlayerControl playerControl = player.GetComponent<PlayerControl>();
            horizontalInput = 0;
            if ( playerControl.enabled ) {
                Vector2 playerRelativePosition = new Vector2( player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y );

                if ( !isAttacking ) {
                    if ( playerRelativePosition.x < 0 && health > 0 ) {
                        horizontalInput = -1.0f;
                    }
                    if ( playerRelativePosition.x > 0 && health > 0 ) {
                        horizontalInput = 1.0f;
                    }
                }
                anim.SetFloat( "horizontalInput", horizontalInput );

                if ( playerRelativePosition.magnitude < attackDistance ) {
                    if ( attackCooldown <= 0 ) {
                        attackBegins();
                    }
                }
            }
        }
        anim.SetBool( "isAttacking", isAttacking );

        // Tap the enemy to attack it
        if ( Input.touchCount > 0 ) {
            foreach ( Touch touch in Input.touches ) {
                if ( touch.phase == TouchPhase.Began ) {
                    testTouch( touch );
                }
            }
        }
    }

    private void testTouch( Touch touch ) {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint( touch.position );
        Vector2 touchPosition = new Vector2( worldPoint.x, worldPoint.y );
        Collider2D hit = Physics2D.OverlapPoint( touchPosition );
        if ( hit ) {
            PlayerControl pc = player.GetComponent<PlayerControl>();
            pc.attack( touch );
        }
    }

	void FixedUpdate ()
	{
        if ( attackCooldown > 0 ) {
            attackCooldown--;
        } else {
            attackCooldown = 0;
        }

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		//anim.SetFloat("Speed", Mathf.Abs(horizontalInput));

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

		// Create an array of all the colliders in front of the enemy.
		/*Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);

		// Check each of the colliders.
		foreach(Collider2D c in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(c.tag == "Obstacle")
			{
				// ... Flip the enemy and stop checking the other colliders.
				Flip ();
				break;
			}
		}*/

		// Set the enemy's velocity to moveSpeed in the x direction.
		rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);	

		// If the enemy has one hit point left and has a damagedEnemy sprite...
		if(HP == 1 && damagedEnemy != null)
			// ... set the sprite renderer's sprite to be the damagedEnemy sprite.
			ren.sprite = damagedEnemy;
			
		// If the enemy has zero or fewer hit points and isn't dead yet...
		if(HP <= 0 && !dead)
			// ... call the death function.
			Death ();
	}
	
	public void Hurt()
	{
		// Reduce the number of hit points by one.
		HP--;
	}
	
	void Death()
	{
		// Find all of the sprite renderers on this object and it's children.
		SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

		// Disable all of them sprite renderers.
		foreach(SpriteRenderer s in otherRenderers)
		{
			s.enabled = false;
		}

		// Re-enable the main sprite renderer and set it's sprite to the deadEnemy sprite.
		ren.enabled = true;
		ren.sprite = deadEnemy;

		// Set dead to true.
		dead = true;

		// Allow the enemy to rotate and spin it by adding a torque.
		rigidbody2D.fixedAngle = false;
		rigidbody2D.AddTorque(Random.Range(deathSpinMin,deathSpinMax));

		// Find all of the colliders on the gameobject and set them all to be triggers.
		Collider2D[] cols = GetComponents<Collider2D>();
		foreach(Collider2D c in cols)
		{
			c.isTrigger = true;
		}

		// Play a random audioclip from the deathClips array.
		int i = Random.Range(0, deathClips.Length);
		AudioSource.PlayClipAtPoint(deathClips[i], transform.position);

		// Create a vector that is just above the enemy.
		Vector3 scorePos;
		scorePos = transform.position;
		scorePos.y += 1.5f;
	}


	public void Flip()
	{
        facingRight = !facingRight;
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

    public void playSoundScare() {
        soundScare.audio.Play();
    }
    public void playSoundTakeDamage() {
        soundTakeDamage.audio.Play();
    }
    public void playSoundDefeat() {
        soundDefeat.audio.Play();
    }
    public void playSoundAttack() {
        soundAttack.audio.Play();
    }
    public void attackBegins() {
        isAttacking = true;
    }
    public void attackEnds() {
        isAttacking = false;
    }
    public void dealAttackDamage() {
        if ( health > 0 ) {
            Vector2 playerRelativePosition = new Vector2( player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y );
            if ( playerRelativePosition.magnitude < attackDistance ) {
                if ( attackCooldown <= 0 ) {
                    PlayerControl playerControl = player.GetComponent<PlayerControl>();
                    playerControl.health -= attackDamage;
                    playerControl.playSoundTakeDamage();
                    //playSoundAttack();
                    if ( this.facingRight ) {
                        player.rigidbody2D.AddForce( new Vector2( 5000.0f * attackKnockback, 400.0f * attackKnockback ) );
                    } else {
                        player.rigidbody2D.AddForce( new Vector2( 5000.0f * attackKnockback, 400.0f * attackKnockback ) );
                    }
                    attackCooldown = attackTime;
                }
            }
        }
    }
}
