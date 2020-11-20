using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrateSerialized : SerializeBase { }

public class Crate : TypeWithSerialize<CrateSerialized>
{

  public override CrateSerialized Serialize()
  {
    CrateSerialized data = new CrateSerialized();
    data.x = transform.position.x;
    data.y = transform.position.y;
    data.rotation = transform.eulerAngles.z;
    return data;
  }
}

