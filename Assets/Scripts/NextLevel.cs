using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextLevel : MonoBehaviour {

    private Collider2D ownCollider;
    private int hitFingerIdBegan;

	// Use this for initialization
	void Start () {
        hitFingerIdBegan = -1;
        ownCollider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach ( Touch touch in Input.touches ) {
            if ( touch.phase == TouchPhase.Began ) {
                Collider2D[] hitColliders = Physics2D.OverlapPointAll( Camera.main.ScreenToWorldPoint( new Vector3( touch.position.x, touch.position.y, 0.0f ) ) );
                foreach ( Collider2D collider in hitColliders ) {
                    if ( collider == ownCollider ) {
                        hitFingerIdBegan = touch.fingerId;
                    }
                }
            }
            if ( touch.phase == TouchPhase.Ended ) {
                if ( touch.fingerId == hitFingerIdBegan ) {
                    Collider2D[] hitColliders = Physics2D.OverlapPointAll( Camera.main.ScreenToWorldPoint( new Vector3( touch.position.x, touch.position.y, 0.0f ) ) );
                    foreach ( Collider2D collider in hitColliders ) {
                        if ( collider == ownCollider ) {
                            Application.LoadLevel( Application.loadedLevel + 1 );
                        }
                    }
                    hitFingerIdBegan = -1;
                }
            }
        }
	}

    void OnTriggerEnter2D( Collider2D other ) {
        /*if ( other.tag == "Player" ) {
            Application.LoadLevel( Application.loadedLevel + 1 );
        }*/
    }
}
