using UnityEngine;
using System.Collections;

public class Hopp : MonoBehaviour {

    public Animator animator;
    private float fallSpeed;
    private float verticalMovement;
    private bool onGround;
    private Transform transform;

	// Use this for initialization
	void Start () {
        onGround = true;
        fallSpeed = 0.02f;
        this.transform = this.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 mouseWorldPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, 0 ) ;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint( mouseWorldPosition );
	    if ( ( Input.GetKeyDown( KeyCode.Space ) || ( Input.GetMouseButtonDown( 0 ) && Mathf.Abs( mouseWorldPosition.x - this.gameObject.transform.position.x ) < 3 && Mathf.Abs( mouseWorldPosition.y - this.gameObject.transform.position.y ) < 3 ) ) && onGround ) {
            verticalMovement = 1.0f;
            onGround = false;
        } else if ( !onGround && verticalMovement > 0 ) {
            verticalMovement -= fallSpeed;
            if ( verticalMovement < 0 ) {
                verticalMovement = 0;
                onGround = true;
            }
        }
        if (!onGround) {
            transform.position += new Vector3(0, verticalMovement - 0.5f, 0);
        }
        animator.SetFloat( "VerticalMovement", verticalMovement );
        animator.SetBool( "OnGround", onGround );
	}
}
