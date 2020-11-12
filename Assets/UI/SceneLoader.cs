﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  public void LoadGame()
  {
    SceneManager.LoadScene("Level1");
  }
  public void QuitGame()
  {
    Application.Quit();
  }
}
