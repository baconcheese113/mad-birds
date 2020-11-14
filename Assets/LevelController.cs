using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
  [SerializeField] private int _totalLevels = 3;

  private List<Enemy> _enemiesLeft;
  private static int _nextLevelIndex = 1;
  private bool _levelFinished = false;

  private void OnEnable()
  {
    Enemy[] enemies = FindObjectsOfType<Enemy>();
    _enemiesLeft = new List<Enemy>(enemies);
    CinemachineTargetGroup targetGroup = FindObjectOfType<CinemachineTargetGroup>();
    foreach (Enemy e in enemies)
    {
      targetGroup.AddMember(e.transform, 1f, 1f);
    }
    Bug bug = FindObjectOfType<Bug>();
    if (bug) targetGroup.AddMember(bug.transform, 1f, 1f);
  }

  public void NotifyEnemyDestroyed(Enemy enemy)
  {
    _enemiesLeft.Remove(enemy);
    if (_enemiesLeft.Count == 0) FinishLevel();
  }

  private void FinishLevel()
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
