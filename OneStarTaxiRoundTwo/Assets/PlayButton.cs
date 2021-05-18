using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] string nameOfSceneToGoTo = "";

    public void OnClick()
    {
        if (nameOfSceneToGoTo != "")
        {
            SceneManager.LoadScene(nameOfSceneToGoTo);
        }
        else
        {
            Debug.LogError("FUCKING LIBTARD! You didn't type in the name of the scene you wanna load!", gameObject);
        }
    }
}
