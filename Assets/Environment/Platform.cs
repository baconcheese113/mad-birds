using UnityEngine;

[System.Serializable]
public class PlatformSerialized : SerializeBase
{
  // Rotator
  public bool useRotator = false;
  public float degsPerSec = 45f;
  // Bouncer
  public bool useBouncer = false;
  public float bounceMutliplier = 10f;
  public bool onlyBounceFront = true;

}
public class Platform : TypeWithSerialize<PlatformSerialized>
{
  // Rotator
  [SerializeField] private bool _useRotator = false;
  [SerializeField] private float _degsPerSec = 45f;

  // Bouncer
  [SerializeField] private bool _useBouncer = false;
  [SerializeField] private float _bounceMutliplier = 10f;
  [SerializeField] private bool _onlyBounceFront = true;

  void Update()
  {
    if (_useRotator)
    {
      transform.Rotate(new Vector3(0, 0, _degsPerSec * Time.deltaTime));
    }
  }


  private void OnCollisionEnter2D(Collision2D other)
  {
    if (_useBouncer)
    {
      float dotProduct = Vector2.Dot(other.contacts[0].normal, transform.up.normalized);
      if (_onlyBounceFront && dotProduct > 0) return;
      other.rigidbody.AddForce(transform.up * _bounceMutliplier, ForceMode2D.Impulse);
    }
  }

  public override void Deserialize(PlatformSerialized data)
  {
    base.Deserialize(data);
    // Rotator
    _useRotator = data.useRotator;
    _degsPerSec = data.degsPerSec;
    // Bouncer
    _useBouncer = data.useBouncer;
    _bounceMutliplier = data.bounceMutliplier;
    _onlyBounceFront = data.onlyBounceFront;
  }

  public override PlatformSerialized Serialize()
  {
    PlatformSerialized data = new PlatformSerialized();
    data.x = transform.position.x;
    data.y = transform.position.y;
    data.rotation = transform.eulerAngles.z;
    // Rotator
    data.useRotator = _useRotator;
    data.degsPerSec = _degsPerSec;
    // Bouncer
    data.useBouncer = _useBouncer;
    data.bounceMutliplier = _bounceMutliplier;
    data.onlyBounceFront = _onlyBounceFront;
    return data;
  }
}
