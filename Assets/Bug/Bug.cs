using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bug : MonoBehaviour
{

  [SerializeField] private AudioSource _bugLaunch;
  [SerializeField] private AudioSource _bugImpactCrate;
  [SerializeField] private AudioSource _bugImpactGround;
  [SerializeField] private AudioSource _bugPull;

  [SerializeField] private float _launchPower = 600f;
  [SerializeField] private float _timeBeforeRestart = 2f;
  [SerializeField] private float _gravity = 1f;

  [SerializeField] protected LineRenderer _launchLine;
  [SerializeField] private bool _ignoreLaunchBounds = false;

  private Vector3 _initialPosition;
  protected bool _bugWasLaunched = false;
  private float _timeSittingAround = 0f;
  private LevelController _controller;

  [Header("Bug flight controls ✈️")]
  [SerializeField] private AudioSource _bugFlight;
  [Tooltip("Minimum bug velocity to INCREASE flight audio volume (it's set to zero when velocity is below .4)")]
  [SerializeField] private float _minFlightVelocity = 10f;
  [Tooltip("Minimum amount of time bug needs to be above minFlightVelocity before increasing volume")]
  [SerializeField] private float _minTimeTillFlight = .5f;
  [Tooltip("Multiplier for flight volume increase (volume is still clamped between 0 - 1)")]
  [SerializeField] private float _flightAudioDamping = 1f;
  private float _timeInFlight = 0f;

  protected virtual void Awake()
  {
    _initialPosition = transform.position;
    _controller = FindObjectOfType<LevelController>();
    if (_bugFlight) _bugFlight.volume = 0f;
  }

  public virtual void Reset()
  {
    GetComponent<Rigidbody2D>().gravityScale = 0;
    _bugWasLaunched = false;
    _timeSittingAround = 0;
    transform.position = _initialPosition;
    transform.rotation = Quaternion.identity;
  }

  protected virtual void Update()
  {
    if (!_bugWasLaunched)
    {
      _launchLine.SetPosition(0, transform.position);
      _launchLine.SetPosition(1, _initialPosition);
      return;
    }

    Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

    if (rigidbody.velocity.magnitude > _minFlightVelocity && _bugFlight)
    {
      _timeInFlight += Time.deltaTime;
      _bugFlight.volume = Mathf.Clamp01((_timeInFlight - _minTimeTillFlight) * _flightAudioDamping);
    }
    if (rigidbody.velocity.magnitude <= .4f)
    {
      _timeInFlight = 0f;
      _timeSittingAround += Time.deltaTime;
    }
    if (_timeSittingAround > _timeBeforeRestart || Math.Abs(transform.position.y) > 100 || Math.Abs(transform.position.x) > 200)
    {
      _controller.NotifyLaunchEnd(this);
    }
  }

  protected void OnCollisionEnter2D(Collision2D other)
  {
    float volume = Mathf.Min(other.relativeVelocity.magnitude / 20f, 1f);
    if (_bugImpactCrate && !_bugImpactCrate.isPlaying && other.gameObject.GetComponent<Crate>())
    {
      _bugImpactCrate.volume = volume;
      _bugImpactCrate.Play();
    }
    if (_bugImpactGround && !_bugImpactGround.isPlaying && other.gameObject.CompareTag("Ground"))
    {
      _bugImpactGround.volume = volume;
      _bugImpactGround.Play();
    }
  }

  protected void OnMouseDown()
  {
    if (_bugWasLaunched) return;
    GetComponentInChildren<SpriteRenderer>().color = Color.red;
    _launchLine.enabled = true;
    if (_bugPull && !_bugPull.isPlaying) _bugPull.Play();
  }

  protected void OnMouseUp()
  {
    if (_bugWasLaunched) return;
    GetComponentInChildren<SpriteRenderer>().color = Color.white;
    _launchLine.enabled = false;

    Vector2 directionToInitialPosition = _initialPosition - transform.position;
    GetComponent<Rigidbody2D>().AddForce(directionToInitialPosition * _launchPower);
    GetComponent<Rigidbody2D>().gravityScale = _gravity;
    _timeSittingAround = 0;
    _bugWasLaunched = true;
    if (_bugPull && _bugPull.isPlaying) _bugPull.Stop();
    if (_bugLaunch) _bugLaunch.Play();
    if (_bugFlight) _bugFlight.Play();
  }

  protected void OnMouseDrag()
  {
    if (_bugWasLaunched) return;
    Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    if (_ignoreLaunchBounds) transform.position = new Vector3(newPosition.x, newPosition.y, 0f);
    transform.position = new Vector3(Mathf.Clamp(newPosition.x, 1f, _initialPosition.x), Mathf.Clamp(newPosition.y, 8f, 40f), 0);
  }
}
