using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
  private Enemy[] _enemies;
  private static int _nextLevelIndex = 1;
  private int _totalLevels = 3;
  private bool _levelFinished = false;

  private void OnEnable()
  {
    _enemies = FindObjectsOfType<Enemy>();

  }

  // Update is called once per frame
  void Update()
  {
    foreach (Enemy enemy in _enemies)
    {
      if (enemy)
      {
        return;
      }
    }
    FinishLevel();
  }

  void FinishLevel()
  {
    if (_levelFinished) return;
    _levelFinished = true;

    IEnumerator ProceedToNextLevel()
    {
      yield return new WaitForSeconds(1.5f);
      _nextLevelIndex = Mathf.Min(_nextLevelIndex + 1, _totalLevels);
      string nextLevelName = "Level" + _nextLevelIndex;
      SceneManager.LoadScene(nextLevelName);
    }
    StartCoroutine(ProceedToNextLevel());
  }
}
