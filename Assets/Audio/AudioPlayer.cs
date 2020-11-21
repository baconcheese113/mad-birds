using UnityEngine;

enum PlayMethod
{
  RANDOM,
  CONSECUTIVE
}

public class AudioPlayer : MonoBehaviour
{
  [SerializeField] private SoundCue _soundCue;
  [SerializeField] private PlayMethod _playMethod = PlayMethod.RANDOM;
  [SerializeField] private bool _playOnAwake = true;
  [SerializeField] private bool _loop = false;
  [SerializeField] private float _pitchMin = 1f;
  [SerializeField] private float _pitchMax = 1f;

  private int _lastPlayedIdx = -1;
  private AudioSource _audioSource;

  private void OnEnable()
  {
    _audioSource = gameObject.AddComponent<AudioSource>();
  }

  private void Start()
  {
    if (_playOnAwake) _audioSource.Play();
  }

  private void Update()
  {
    if (_loop && !_audioSource.isPlaying) Play();
  }

  public void Play()
  {
    // print("Trying to play");
    int slotIdx = GetNextSlotIdx();
    if (slotIdx == -1) return;
    print("Playing audioClip at index " + slotIdx);
    AudioSlot slot = _soundCue._audioSlots[slotIdx];
    _audioSource.clip = slot._audioClip;
    _audioSource.volume = slot._volume;
    _audioSource.pitch = Random.Range(_pitchMin, _pitchMax);
    _audioSource.Play();
    _lastPlayedIdx = slotIdx;
  }

  private int GetNextSlotIdx()
  {
    if (!_soundCue || _soundCue._audioSlots.Count == 0) return -1;
    int randIdx = Random.Range(0, _soundCue._audioSlots.Count);
    int nextIdx = _playMethod == PlayMethod.RANDOM ? randIdx : (_lastPlayedIdx + 1) % _soundCue._audioSlots.Count;
    return nextIdx;
  }
}
