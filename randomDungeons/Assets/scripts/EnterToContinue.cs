using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnterToContinue : MonoBehaviour {

    public string levelToLoad;
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKey("enter") || Input.GetKey("return"))
        {
            Application.LoadLevel(levelToLoad);
        }
	}
}
