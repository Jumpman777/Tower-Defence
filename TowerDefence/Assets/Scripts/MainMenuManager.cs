using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    
    public void PlayGame()
    {
       
        SceneManager.LoadScene("Game");
    }

    
    public void QuitGame()
    {
        
        Debug.Log("Quit Game"); 
        Application.Quit();
    }
}

