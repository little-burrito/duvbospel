using UnityEngine;
using System.Collections;

public class Walk : MonoBehaviour {

    public Animator animator;
    private float walkSpeed;
    private float horizontalMovement;
    private Transform transform;

    // Use this for initialization
    void Start()
    {
        horizontalMovement = 0.0f;
        walkSpeed = 0.1f;
        this.transform = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = 0;
        if (Input.GetKey(KeyCode.RightArrow) || ( Input.GetMouseButton( 0 ) && Input.mousePosition.x > Screen.width / 4 + Screen.width / 2 ) )
        {
            horizontalMovement = walkSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || ( Input.GetMouseButton( 0 ) && Input.mousePosition.x < Screen.width / 4 ) )
        {
            horizontalMovement -= walkSpeed;
        }
        transform.position += new Vector3(horizontalMovement, 0, 0);
        if (horizontalMovement < 0)
        {
            this.gameObject.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (horizontalMovement > 0)
        {
            this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        animator.SetFloat("HorizontalMovement", horizontalMovement);
    }
}
