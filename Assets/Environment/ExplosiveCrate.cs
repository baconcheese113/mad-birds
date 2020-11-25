using UnityEngine;

[System.Serializable]
public class ExplosiveCrateSerialized : SerializeBase
{
  public float explosionRadius = 2f;
  public float explosionForce = 500f;
  public float hitSensitivity = 5f;
  public float chainReactionRadius = 1f;
  public bool onlyPlayerCanTrigger = true;
}

public class ExplosiveCrate : ITypeWithSerialize<ExplosiveCrateSerialized>
{
  [SerializeField] private GameObject _explosionParticles;
  [SerializeField] private float _explosionRadius = 2f;
  [SerializeField] private float _explosionForce = 500f;
  [Tooltip("Minimum hit velocity magnitude to cause crate to explode")]
  [SerializeField] private float _hitSensitivity = 5f;
  [SerializeField] private float _chainReactionRadius = 1f;
  [SerializeField] private bool _onlyPlayerCanTrigger = true;
  [SerializeField] private AudioSource _explosion;

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
    if (_explosion) LevelController.PlayOnNewObject(transform.position, _explosion);
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

  public override void Deserialize(ExplosiveCrateSerialized data)
  {
    base.Deserialize(data);
    _explosionRadius = data.explosionRadius;
    _explosionForce = data.explosionForce;
    _hitSensitivity = data.hitSensitivity;
    _chainReactionRadius = data.chainReactionRadius;
    _onlyPlayerCanTrigger = data.onlyPlayerCanTrigger;
  }

  public override ExplosiveCrateSerialized Serialize()
  {
    ExplosiveCrateSerialized crate = new ExplosiveCrateSerialized();
    crate.explosionRadius = _explosionRadius;
    crate.explosionForce = _explosionForce;
    crate.hitSensitivity = _hitSensitivity;
    crate.chainReactionRadius = _chainReactionRadius;
    crate.onlyPlayerCanTrigger = _onlyPlayerCanTrigger;

    crate.x = transform.position.x;
    crate.y = transform.position.y;
    crate.rotation = transform.eulerAngles.z;
    return crate;
  }
}
