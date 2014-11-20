using UnityEngine;
using System.Collections;

public class ParallaxBackground : MonoBehaviour {

    public float cameraPositionMultiplier = 1.0f;
    private Vector3 relativePosition;

	// Use this for initialization
	void Start () {
        relativePosition = this.gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

    }

    void LateUpdate() {
        Vector3 position = Camera.main.transform.position;
        position.x *= cameraPositionMultiplier;
        position.y *= cameraPositionMultiplier;
        position.x += this.relativePosition.x;
        position.y += this.relativePosition.y;
        position.z = this.gameObject.transform.position.z;
        this.gameObject.transform.position = position;
    }
}
