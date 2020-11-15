using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
  [SerializeField] private float _bounceMutliplier = 10f;
  [SerializeField] private bool _onlyBounceFront = true;


  private void OnCollisionEnter2D(Collision2D other)
  {
    float dotProduct = Vector2.Dot(other.contacts[0].normal, transform.up.normalized);
    if (_onlyBounceFront && dotProduct > 0) return;
    other.rigidbody.AddForce(transform.up * _bounceMutliplier, ForceMode2D.Impulse);
  }
}
