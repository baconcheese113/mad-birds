using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{

  [SerializeField] private GameObject _payload;
  [SerializeField] private ParticleSystem _popParticles;
  [SerializeField] private float _minPopVelocity = 2f;
  [SerializeField] private AudioSource _pop;


  private void OnCollisionEnter2D(Collision2D other)
  {
    if (other.relativeVelocity.magnitude < _minPopVelocity) return;
    Instantiate(_popParticles, transform.TransformPoint(Vector3.up * 2f), transform.rotation);
    if (_pop) LevelController.PlayOnNewObject(transform.position, _pop);
    _payload.transform.SetParent(null);
    _payload.gameObject.GetComponent<Rigidbody2D>().mass = 1f;
    GetComponent<HingeJoint2D>().enabled = false;
    Destroy(gameObject);
  }
}
