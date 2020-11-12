using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

  [SerializeField] private AudioSource _levelMusic;
  void Start()
  {
    if (_levelMusic) _levelMusic.Play();
    DontDestroyOnLoad(gameObject);
  }
}
