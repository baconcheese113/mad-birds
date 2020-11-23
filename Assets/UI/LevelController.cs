using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
  [Header("Settings for this level (change in level instance)")]
  [SerializeField] private int _numLaunches = 2;

  [Header("Settings across all levels (change in prefab)")]
  [SerializeField] private int _totalLevels = 3;
  [SerializeField] private TextMeshProUGUI _levelText;
  [SerializeField] private TextMeshProUGUI _launchesText;
  [SerializeField] private Slider _enemiesSlider;
  [SerializeField] private GameObject _pauseMenu;
  [SerializeField] private float _cameraPadding = 2f;

  private int _launchesUsed = 1;
  private Vector3 _playerStartPosition;
  private int _enemiesStart;
  private List<Enemy> _enemiesLeft;
  private static int _nextLevelIndex = 1;
  private bool _levelFinished = false;
  private MusicPlayer _musicPlayer;

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
    _launchesText.SetText("Launch 1 / " + _numLaunches);
    _musicPlayer = FindObjectOfType<MusicPlayer>();
    CinemachineTargetGroup targetGroup = FindObjectOfType<CinemachineTargetGroup>();
    Bug bug = FindObjectOfType<Bug>();
    if (bug)
    {
      targetGroup.AddMember(bug.transform, 1f, _cameraPadding);
      _playerStartPosition = bug.transform.position;
    }
    foreach (Enemy e in enemies)
    {
      targetGroup.AddMember(e.transform, 1f, _cameraPadding);
    }
  }

  public void NotifyEnemyDestroyed(Enemy enemy)
  {
    _enemiesLeft.Remove(enemy);
    _enemiesSlider.value = _enemiesStart - _enemiesLeft.Count;
    if (_enemiesLeft.Count == 1) _musicPlayer.IncreaseIntensity();
    if (_enemiesLeft.Count == 0) FinishLevel();
  }

  public void NotifyLaunchEnd(Bug bug)
  {
    if (_levelFinished) return;
    if (_launchesUsed >= _numLaunches)
    {
      string currentSceneName = SceneManager.GetActiveScene().name;
      SceneManager.LoadScene(currentSceneName);
    }
    else
    {
      bug.Reset();
      _launchesUsed++;
      _launchesText.SetText("Launch " + _launchesUsed + " / " + _numLaunches);
    }
  }

  public void RestartLevel()
  {
    _musicPlayer.Reset();
    ShowPauseMenu(false);
    SceneManager.LoadScene("Level" + _nextLevelIndex);
  }

  private void FinishLevel()
  {
    if (_levelFinished) return;
    _levelFinished = true;

    _musicPlayer.Reset();
    FindObjectOfType<CinemachineTargetGroup>().m_Targets[0].radius = 20; // zero because bug should be added first

    IEnumerator ProceedToNextLevel()
    {
      yield return new WaitForSeconds(2.5f);
      _nextLevelIndex = Mathf.Min(_nextLevelIndex + 1, _totalLevels);
      string nextLevelName = "Level" + _nextLevelIndex;
      SceneManager.LoadScene(nextLevelName);
    }
    StartCoroutine(ProceedToNextLevel());
  }

  // Logic for UI things
  public void ShowPauseMenu(bool shouldShow)
  {
    Time.timeScale = shouldShow ? 0 : 1;
    _pauseMenu.SetActive(shouldShow);
  }

  public void JumpToScene(string name)
  {
    if (name.StartsWith("UI")) _musicPlayer.TriggerMenu();
    ShowPauseMenu(false);
    SceneManager.LoadScene(name);
  }
}
