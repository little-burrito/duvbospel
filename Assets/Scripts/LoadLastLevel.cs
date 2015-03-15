using UnityEngine;
using System.Collections;

public class LoadLastLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if ( PlayerPrefs.GetString( "savedLevelName" ) != null ) {
            Application.LoadLevel( PlayerPrefs.GetString( "savedLevelName" ) );
        } else {
            Application.LoadLevel( Application.loadedLevel + 1 );
        }
	}
}
