using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
  [SerializeField] private int _totalLevels = 3;
  [SerializeField] private TextMeshProUGUI _levelText;
  [SerializeField] private Slider _enemiesSlider;
  [SerializeField] private GameObject _pauseMenu;

  private int _enemiesStart;
  private List<Enemy> _enemiesLeft;
  private static int _nextLevelIndex = 1;
  private bool _levelFinished = false;

  private void OnEnable()
  {
    Initialize();
  }

  public void Initialize()
  {
    Enemy[] enemies = FindObjectsOfType<Enemy>();
    _enemiesLeft = new List<Enemy>(enemies);
    _enemiesStart = _enemiesLeft.Count;
    _enemiesSlider.value = 0;
    _enemiesSlider.maxValue = _enemiesStart;
    _levelText.SetText("Level " + _nextLevelIndex + " / " + _totalLevels);
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
    _enemiesSlider.value = _enemiesStart - _enemiesLeft.Count;
    if (_enemiesLeft.Count == 0) FinishLevel();
  }

  public void RestartLevel()
  {
    ShowPauseMenu(false);
    SceneManager.LoadScene("Level" + _nextLevelIndex);
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

  public void ShowPauseMenu(bool shouldShow)
  {
    Time.timeScale = shouldShow ? 0 : 1;
    _pauseMenu.SetActive(shouldShow);
  }

  public void JumpToScene(string name)
  {
    ShowPauseMenu(false);
    SceneManager.LoadScene(name);
  }
}
