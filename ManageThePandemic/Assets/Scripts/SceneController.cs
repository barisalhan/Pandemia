using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OnStartButton()
    {
        Debug.Log("Game is started.");
        SceneManager.LoadScene("GameScreen");
    }

    public void OnLoadButton()
    {
        
    }
}
