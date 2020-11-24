using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
  [SerializeField] private AudioMixer _mixer;
  [SerializeField] private Slider[] _volumeSliders;

  [SerializeField] private AudioSource _menuMusic;
  [SerializeField] private AudioSource _levelAmbiance;
  [Tooltip("Array of dynamic audio to play in levels, 0 is the least intense")]
  [SerializeField] private AudioSource[] _levelMusic;
  [Tooltip("Seconds it takes to increase a full level")]
  [SerializeField] private float _musicIntensityDamping = 4f;
  [SerializeField] private float _slowmoPitch = .7f;

  private float _targetIntensityLevel = 0f;
  private float _intensityLevel = 0f;

  public void Reset()
  {
    _targetIntensityLevel = 0;
    for (int i = 0; i < _levelMusic.Length; i++)
    {
      _levelMusic[i].pitch = _slowmoPitch;
    }
  }

  void Start()
  {
    if (SceneManager.GetActiveScene().name.StartsWith("Level")) TriggerLevel();
    else TriggerMenu();

    foreach (Slider s in _volumeSliders)
    {
      float volume;
      _mixer.GetFloat(s.name, out volume);
      s.SetValueWithoutNotify(volume);
    }
  }

  public void TriggerMenu()
  {
    _levelAmbiance.Stop();
    _menuMusic.Play();
    foreach (AudioSource music in _levelMusic) music.Stop();
  }

  public void TriggerLevel()
  {
    _menuMusic.Stop();
    _levelAmbiance.Play();
    foreach (AudioSource music in _levelMusic) music.Play();

  }

  public void IncreaseIntensity()
  {
    _targetIntensityLevel += 1f;
  }

  private void Update()
  {
    _intensityLevel += (_targetIntensityLevel - _intensityLevel) * (Time.deltaTime / _musicIntensityDamping);
    for (int i = 0; i < _levelMusic.Length; i++)
    {
      _levelMusic[i].volume = Mathf.Max(.99f - Mathf.Abs(_intensityLevel - i), 0f);
    }
  }

  public void SetVolume(Slider s)
  {
    _mixer.SetFloat(s.name, s.value);
  }
}
