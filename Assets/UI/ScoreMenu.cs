using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreMenu : MonoBehaviour
{
  [SerializeField] private GameObject _timePanel;
  [SerializeField] private GameObject _launchesPanel;
  [SerializeField] private GameObject _scorePanel;

  private int startScore = 1000;

  private int CalculateScore(int totalSeconds, int launchesLeft)
  {
    int score = startScore - (Mathf.Min(totalSeconds, 30) * 30);
    return score + launchesLeft * 100;
  }

  public void PlayScoreSequence(int totalSeconds, int launchesLeft, System.Action callbackFn)
  {
    int score = CalculateScore(totalSeconds, launchesLeft);
    IEnumerator PlayFlow()
    {
      yield return new WaitForSeconds(2f);
      GetComponent<Animator>().SetBool("ShouldEnter", true);
      // TIME PANEL
      _timePanel.GetComponent<Animator>().SetBool("ShouldEnter", true);
      yield return new WaitForSeconds(1.5f);
      TextMeshProUGUI textMesh = _timePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
      for (int i = 0; i < 25; i++)
      {
        textMesh.text = Mathf.RoundToInt(totalSeconds * (i / 25f)).ToString();
        yield return new WaitForSeconds(.04f);
      }

      // LAUNCHES PANEL
      _launchesPanel.GetComponent<Animator>().SetBool("ShouldEnter", true);
      yield return new WaitForSeconds(1f);
      textMesh = _launchesPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
      for (int i = 0; i < 25; i++)
      {
        textMesh.text = Mathf.RoundToInt(launchesLeft * (i / 25f)).ToString();
        yield return new WaitForSeconds(.02f);
      }

      // SCORE PANEL
      _scorePanel.GetComponent<Animator>().SetBool("ShouldEnter", true);
      yield return new WaitForSeconds(1f);
      textMesh = _scorePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
      for (int i = 0; i < 25; i++)
      {
        textMesh.text = Mathf.RoundToInt(score * (i / 25f)).ToString();
        yield return new WaitForSeconds(.04f);
      }
      yield return new WaitForSeconds(2f);
      callbackFn();
    }

    StartCoroutine(PlayFlow());
  }
}
