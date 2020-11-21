using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bug : MonoBehaviour
{
  private Vector3 _initialPosition;
  private bool _birdWasLaunched;
  private float _timeSittingAround = 0;

  [SerializeField] private AudioClip _bugLaunch;
  [SerializeField] private AudioSource _bugImpact;
  [SerializeField] private AudioSource _bugPull;

  [SerializeField] private float _launchPower = 600;
  [SerializeField] private float _timeBeforeRestart = 2;
  [SerializeField] private float _gravity = 1;

  private void Awake()
  {
    _initialPosition = transform.position;
  }

  private void Update()
  {
    GetComponent<LineRenderer>().SetPosition(0, transform.position);
    GetComponent<LineRenderer>().SetPosition(1, _initialPosition);
    if (_birdWasLaunched && GetComponent<Rigidbody2D>().velocity.magnitude <= .1)
    {
      _timeSittingAround += Time.deltaTime;
    }
    if (_timeSittingAround > _timeBeforeRestart || Math.Abs(transform.position.y) > 100 || Math.Abs(transform.position.x) > 200)
    {
      string currentSceneName = SceneManager.GetActiveScene().name;
      SceneManager.LoadScene(currentSceneName);
    }
  }

  private void OnCollisionEnter2D(Collision2D other)
  {
    if (_bugImpact && !_bugImpact.isPlaying)
    {
      _bugImpact.Play();
      float volume = Mathf.Min(other.relativeVelocity.magnitude / 20f, 1f);
      print("Playing impact audio at " + volume);
      _bugImpact.volume = volume;
    }
  }

  private void OnMouseDown()
  {
    GetComponentInChildren<SpriteRenderer>().color = Color.red;
    GetComponent<LineRenderer>().enabled = true;
    if (_bugPull && !_bugPull.isPlaying) _bugPull.Play();
  }

  private void OnMouseUp()
  {
    GetComponentInChildren<SpriteRenderer>().color = Color.white;
    GetComponent<LineRenderer>().enabled = false;

    Vector2 directionToInitialPosition = _initialPosition - transform.position;
    GetComponent<Rigidbody2D>().AddForce(directionToInitialPosition * _launchPower);
    GetComponent<Rigidbody2D>().gravityScale = _gravity;
    _timeSittingAround = 0;
    _birdWasLaunched = true;
    if (_bugPull && _bugPull.isPlaying) _bugPull.Stop();
    if (_bugLaunch) AudioSource.PlayClipAtPoint(_bugLaunch, transform.position);
  }

  private void OnMouseDrag()
  {
    Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    transform.position = new Vector3(newPosition.x, newPosition.y, 0);
  }
}
