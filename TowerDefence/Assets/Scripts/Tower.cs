using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public float health = 100f;
    public Text gameOverText;
    public string mainMenuSceneName = "MainMenu"; // Name of the main menu scene

    void Start()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }

        // Return to the main menu after a delay
        Invoke("ReturnToMainMenu", 3f);
    }

    private void ReturnToMainMenu()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}





