using System.Collections.Generic;
using UnityEngine;

// For ordering https://blog.redbluegames.com/guide-to-extending-unity-editors-menus-b2de47a746db#62b3 (⊙_ ☉)
[CreateAssetMenu(fileName = "SoundCue", menuName = "SoundCue", order = 216)]
public class SoundCue : ScriptableObject
{
  public List<AudioSlot> _audioSlots;
}

[System.Serializable]
public class AudioSlot
{
  public AudioClip _audioClip;
  [Range(0f, 2f)]
  public float _volume = 1f;

  private void OnEnable()
  {
    _volume = 1f;
  }
}
