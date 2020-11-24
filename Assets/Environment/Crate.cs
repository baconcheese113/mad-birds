using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrateSerialized : SerializeBase { }

public class Crate : ITypeWithSerialize<CrateSerialized>
{
  [SerializeField] private AudioSource _crateCollision;

  private void OnCollisionEnter2D(Collision2D other)
  {
    if (other.gameObject.GetComponent<Crate>())
    {
      if (_crateCollision) _crateCollision.Play();
    }
  }

  public override CrateSerialized Serialize()
  {
    CrateSerialized data = new CrateSerialized();
    data.x = transform.position.x;
    data.y = transform.position.y;
    data.rotation = transform.eulerAngles.z;
    return data;
  }
}

