using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameControl : MonoBehaviour {

    public Button quitButton = null;

	// Use this for initialization
	void Start () {
        Debug.Assert(quitButton != null);

        quitButton.onClick.AddListener(QuitGame);
	}

    void QuitGame()
    {
        Debug.Log("Quitting");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();

#endif
    }
}
