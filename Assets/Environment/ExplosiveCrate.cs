using UnityEngine;

public class ExplosiveCrate : MonoBehaviour
{
  [SerializeField] private GameObject _explosionParticles;
  [SerializeField] private float _explosionRadius = 2f;
  [SerializeField] private float _explosionForce = 500f;
  [Tooltip("Minimum hit velocity magnitude to cause crate to explode")]
  [SerializeField] private float _hitSensitivity = 5f;
  [SerializeField] private float _chainReactionRadius = 1f;
  [SerializeField] private bool _onlyPlayerCanTrigger = true;

  public bool _isExploding { get; private set; } = false;

  private void OnCollisionEnter2D(Collision2D other)
  {
    bool otherCanTrigger = !_onlyPlayerCanTrigger || !!other.gameObject.GetComponent<Bug>();
    if (otherCanTrigger && other.relativeVelocity.magnitude > _hitSensitivity)
    {
      Explode();
    }
  }
  public void Explode()
  {
    if (_isExploding) return;
    _isExploding = true;
    Instantiate(_explosionParticles, transform.position, Quaternion.identity);
    Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
    foreach (Collider2D other in others)
    {
      Rigidbody2D body = other.GetComponent<Rigidbody2D>();
      if (!body) continue;
      Vector2 dir = body.transform.position - transform.position;
      ExplosiveCrate otherExplosiveCrate = other.GetComponent<ExplosiveCrate>();
      if (dir.magnitude < _chainReactionRadius && otherExplosiveCrate)
      {
        otherExplosiveCrate.Explode();
      }
      else
      {
        float wearoff = 1 - (dir.magnitude / _explosionRadius);
        body.AddForce(dir.normalized * _explosionForce * wearoff);
      }
    }
    Destroy(gameObject);
  }
}
