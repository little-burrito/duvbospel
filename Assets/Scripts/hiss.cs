using UnityEngine;
using System.Collections;

public class hiss : MonoBehaviour {

    public float buttonMargin = 1.0f;
    private Transform knapp;
    private bool playerEntered;

    private Transform soundPling;
    private Transform soundOpen;
    private Transform soundClose;
    private Transform soundFloorLevel;

    private GameObject player;
    private Animator anim;

    public string sceneToGoTo;

    public bool previousLevelHadElevator = true;

	// Use this for initialization
	void Start() {
        knapp = transform.Find( "knapp" );
        soundPling = transform.Find( "pling" );
        soundOpen = transform.Find( "open" );
        soundClose = transform.Find( "close" );
        soundFloorLevel = transform.Find( "floorLevel" );
		anim = GetComponent<Animator>();
        anim.SetBool( "PreviousLevelHadElevator", previousLevelHadElevator );
	}
	
	// Update is called once per frame
    void Update() {
        if ( playerEntered ) {
            player = GameObject.FindGameObjectWithTag( "Player" );
            PlayerControl playerControl = player.GetComponent<PlayerControl>();
            if ( playerControl.defeatedEnemies == playerControl.numEnemies ) {
                for ( int i = 0; i < Input.touchCount; i++ ) {
                    Touch touch = Input.GetTouch( i );
                    if ( touch.phase == TouchPhase.Began ) {
                        Vector3 pointWorldPosition = new Vector3( touch.position.x, touch.position.y, 0 );
                        pointWorldPosition = Camera.main.ScreenToWorldPoint( pointWorldPosition );
                        if ( Mathf.Abs( pointWorldPosition.x - knapp.position.x ) < buttonMargin && Mathf.Abs( pointWorldPosition.y - knapp.position.y ) < buttonMargin ) {
                            if ( !anim.GetBool( "Hiss öppnas" ) ) {
                                disablePlayerControls();
                                anim.SetBool( "Hiss öppnas", true );
                            }
                        }
                    }
                }
            }
        }
	}

    void OnTriggerEnter2D( Collider2D other ) {
        if ( other.tag == "Player" ) {
            player = other.gameObject;
            playerEntered = true;
        }
    }

    void OnTriggerExit2D( Collider2D other ) {
        if ( other.tag == "Player" ) {
            playerEntered = false;
        }
    }

    void playSoundPling() {
        soundPling.audio.Play();
    }

    void playSoundOpen() {
        soundOpen.audio.Play();
    }

    void playSoundClose() {
        soundClose.audio.Play();
    }

    void playSoundFloorLevel() {
        soundFloorLevel.audio.Play();
    }

    void disablePlayerControls() {
        if ( !player ) {
            player = GameObject.FindGameObjectWithTag( "Player" );
        }
        PlayerControl playerControl = player.GetComponent<PlayerControl>();
        playerControl.enabled = false;
        Animator playerAnimator = player.GetComponent<Animator>();
        playerAnimator.SetBool( "Attack", false );
    }

    void enablePlayerControls() {
        if ( !player ) {
            player = GameObject.FindGameObjectWithTag( "Player" );
        }
        PlayerControl playerControl = player.GetComponent<PlayerControl>();
        playerControl.enabled = true;
    }

    void loadScene() {
        Application.LoadLevel( sceneToGoTo );
    }
}
