using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSerialized : SerializeBase { }

public class Crate : TypeWithSerialize<CrateSerialized>
{

  public override CrateSerialized Serialize()
  {
    CrateSerialized data = new CrateSerialized();
    data.position = new Vector2(transform.position.x, transform.position.y);
    data.rotation = transform.eulerAngles.z;
    return data;
  }
}

