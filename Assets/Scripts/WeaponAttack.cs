using UnityEngine;
using System.Collections;

public class WeaponAttack : MonoBehaviour {

    private PlayerControl playerControl;

	// Use this for initialization
	void Start () {
        playerControl = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D( Collider2D other ) {
        if ( other.tag == "enemy" ) {
        Debug.Log( "Collision enter!" );
            if ( playerControl.facingRight ) {
                other.gameObject.rigidbody2D.AddForce( new Vector2( 50000.0f, 4000.0f ) );
            } else {
                other.gameObject.rigidbody2D.AddForce( new Vector2( -50000.0f, 4000.0f ) );
            }
            Destroy( other );
        }
    }
}
