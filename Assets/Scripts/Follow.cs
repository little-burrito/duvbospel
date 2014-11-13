using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public GameObject objectToFollow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        this.gameObject.transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, this.gameObject.transform.position.z);
    }
}
