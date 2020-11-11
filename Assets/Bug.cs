using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bug : MonoBehaviour
{
  private Vector3 _initialPosition;
  private bool _birdWasLaunched;
  private float _timeSittingAround = 0;

  [SerializeField] private float _launchPower = 600;

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
    if (_timeSittingAround > .5 || Math.Abs(transform.position.y) > 10 || Math.Abs(transform.position.x) > 10)
    {
      string currentSceneName = SceneManager.GetActiveScene().name;
      SceneManager.LoadScene(currentSceneName);
    }
  }

  private void OnMouseDown()
  {
    GetComponent<SpriteRenderer>().color = Color.red;
    GetComponent<LineRenderer>().enabled = true;
  }

  private void OnMouseUp()
  {
    GetComponent<SpriteRenderer>().color = Color.white;
    GetComponent<LineRenderer>().enabled = false;

    Vector2 directionToInitialPosition = _initialPosition - transform.position;
    GetComponent<Rigidbody2D>().AddForce(directionToInitialPosition * _launchPower);
    GetComponent<Rigidbody2D>().gravityScale = 1;
    _timeSittingAround = 0;
    _birdWasLaunched = true;
  }

  private void OnMouseDrag()
  {
    Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    transform.position = new Vector3(newPosition.x, newPosition.y, 0);
  }
}
