using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controller : MonoBehaviour {

    public Image pilUpp;
    public Image pilHoger;
    public Image pilNer;
    public Image pilVanster;
    public GameObject bakgrund;

    public float movementScale = 1.0f;
    public float deadzone = 35.0f;

    private int fingerId;
    private Vector2 startPosition = Vector2.zero;
    private bool touchIsActive = false;
    private RectTransform bakgrundPosition;
    private Image bakgrundImage;
    
    public PlayerControl playerControl;

	// Use this for initialization
	void Start () {
        bakgrundPosition = bakgrund.GetComponent<RectTransform>();
        bakgrundImage = bakgrund.GetComponent<Image>();
        endTouch( new Touch() );
	}
	
	// Update is called once per frame
	void Update () {
        if ( !touchIsActive ) {
            foreach ( Touch touch in Input.touches ) {
                if ( touch.phase == TouchPhase.Began ) {
                    if ( touch.position.x < Screen.width / 3 ) {
                        beginTouch( touch );
                        break;
                    }
                }
            }
        }
        if ( touchIsActive ) {
            Touch currentTouch = new Touch();
            foreach ( Touch touch in Input.touches ) {
                if ( touch.fingerId == fingerId ) {
                    currentTouch = touch;
                    break;
                }
            }
            if ( currentTouch.phase == TouchPhase.Ended ) {
                endTouch( currentTouch );
            } else {
                updateTouch( currentTouch );
            }
        }
	}

    void beginTouch( Touch touch ) {
        touchIsActive = true;
        fingerId = touch.fingerId;
        startPosition = touch.position;
        bakgrundPosition.position = new Vector3( startPosition.x, startPosition.y, 0 );
        bakgrundImage.color = new Color( 255.0f, 255.0f, 255.0f, 255.0f );
        hideArrows();
    }

    void updateTouch( Touch touch ) {

        float deltaX = ( touch.position.x - startPosition.x );
        if ( Mathf.Abs( deltaX ) < deadzone ) {
            deltaX = 0.0f;
        } else {
            if ( deltaX > 0 ) {
                deltaX -= deadzone;
            } else {
                deltaX += deadzone;
            }
            deltaX = deltaX * 1 / movementScale;
            deltaX = range( deltaX, -1.0f, 1.0f );
        }

        float deltaY = ( touch.position.y - startPosition.y );
        if ( Mathf.Abs( deltaY ) < deadzone ) {
            deltaY = 0.0f;
        } else {
            if ( deltaY > 0 ) {
                deltaY -= deadzone;
            } else {
                deltaY += deadzone;
            }
            deltaY = deltaY * 1 / movementScale;
            deltaY = range( deltaY, -1.0f, 1.0f );
        }

        playerControl.move( deltaX, deltaY );
        updateArrows( deltaX, deltaY );
    }

    void endTouch( Touch touch ) {
        touchIsActive = false;
        bakgrundImage.color = new Color( 255.0f, 255.0f, 255.0f, 0.0f );
        playerControl.move( 0.0f, 0.0f );
        hideArrows();
    }

    void hideArrows() {
        pilVanster.color = new Color( 255.0f, 255.0f, 255.0f, 0.0f );
        pilHoger.color = new Color( 255.0f, 255.0f, 255.0f, 0.0f );
        pilUpp.color = new Color( 255.0f, 255.0f, 255.0f, 0.0f );
        pilNer.color = new Color( 255.0f, 255.0f, 255.0f, 0.0f );
    }

    void updateArrows( float deltaX, float deltaY ) {
        pilVanster.color = new Color( 255.0f, 255.0f, 255.0f, range( -deltaX, 0.0f, 1.0f ) );
        pilHoger.color = new Color( 255.0f, 255.0f, 255.0f, range( deltaX, 0.0f, 1.0f ) );
        pilUpp.color = new Color( 255.0f, 255.0f, 255.0f, range( deltaY, 0.0f, 1.0f ) );
        pilNer.color = new Color( 255.0f, 255.0f, 255.0f, range( -deltaY, 0.0f, 1.0f ) );
    }

    float range( float input, float min, float max ) {
        return Mathf.Max( Mathf.Min( input, max ), min );
    }
}
