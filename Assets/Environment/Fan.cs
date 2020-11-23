using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
  [Tooltip("Force to push objects away with")]
  [SerializeField] private float _force = 100f;
  [SerializeField] private bool _onlyAffectPlayer = false;

  private CapsuleCollider2D _airStreamCollider;

  void Awake()
  {
    _airStreamCollider = GetComponent<CapsuleCollider2D>();
  }

  void OnTriggerStay2D(Collider2D other)
  {
    if (_onlyAffectPlayer && !other.gameObject.GetComponent<Bug>()) return;
    Rigidbody2D body = other.GetComponent<Rigidbody2D>();
    if (!body) return;
    Vector2 dir = body.transform.position - transform.position;
    float wearoff = 1 - (dir.magnitude / _airStreamCollider.size.x);
    body.AddForce(dir.normalized * _force * wearoff);
  }

  // TODO Maybe? Idk.... depends if we're going to adjust the size of fans all the time
  float getParticleLifetime(float speed)
  {
    // https://mycurvefit.com/ used this after plugging in the data points, only changing speed and lifetime
    return 7.661808f * Mathf.Pow(speed, -.9686923f);
  }

  float getLifetimeFromSize()
  {
    return .2471429f * _airStreamCollider.size.x - .45f;
  }
}
