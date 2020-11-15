using System;
using System.IO;
using UnityEngine;

/*
* To add to the serialized data, you need to:
*
* 1) Implement TypeWithSerialize and SerializeBase in the new component
* 2) Add new type to SceneData
* 3) Add prefab for new objects
* 4) Add instance call to ImportScene
* 5) Add serialize call to ExportScene
*/
public class GenerateFromJSON : MonoBehaviour
{
  public SceneData _data;
  public string _json;

  [SerializeField] private string _fileName = "ExportedScene";
  // Add prefab for each type
  [SerializeField] private GameObject _cratePrefab;
  [SerializeField] private GameObject _explosiveCratePrefab;
  [SerializeField] private GameObject _platformPrefab;
  [SerializeField] private GameObject _enemyPrefab;

  public void ImportScene()
  {
    string contents = File.ReadAllText(Path.Combine(Application.dataPath, _fileName + ".json"));
    SceneData data = JsonUtility.FromJson<SceneData>(contents);
    // Add call for each serialized type
    InstanceFromSerialized<Crate, CrateSerialized>(data.crates, _cratePrefab);
    InstanceFromSerialized<ExplosiveCrate, ExplosiveCrateSerialized>(data.explosiveCrates, _explosiveCratePrefab);
    InstanceFromSerialized<Platform, PlatformSerialized>(data.platforms, _platformPrefab);
    InstanceFromSerialized<Enemy, EnemySerialized>(data.enemies, _enemyPrefab);
    FindObjectOfType<LevelController>().Initialize();
  }

  public void ExportScene()
  {
    SceneData data = new SceneData();
    // Add call for each serialized type
    data.crates = GetSerializedVersion<Crate, CrateSerialized>();
    data.explosiveCrates = GetSerializedVersion<ExplosiveCrate, ExplosiveCrateSerialized>();
    data.platforms = GetSerializedVersion<Platform, PlatformSerialized>();
    data.enemies = GetSerializedVersion<Enemy, EnemySerialized>();
    _data = data;
    _json = JsonUtility.ToJson(_data);
    File.WriteAllText(Path.Combine(Application.dataPath, _fileName + ".json"), _json);
  }

  private S[] GetSerializedVersion<T, S>() where T : TypeWithSerialize<S>
  {
    T[] objs = FindObjectsOfType<T>();
    S[] serializedObjs = new S[objs.Length];
    for (int x = 0; x < serializedObjs.Length; x++)
    {
      serializedObjs[x] = objs[x].Serialize();
    }
    return serializedObjs;
  }

  private void InstanceFromSerialized<T, S>(S[] data, GameObject prefab) where T : TypeWithSerialize<S> where S : SerializeBase
  {
    foreach (S s in data)
    {
      GameObject go = Instantiate(prefab, new Vector3(s.position.x, s.position.y, 0), Quaternion.Euler(0, 0, s.rotation));
      T t = go.GetComponent<T>();
      t.Deserialize(s);
    }
  }
}

public abstract class TypeWithSerialize<S> : MonoBehaviour { public abstract S Serialize(); public virtual void Deserialize(S data) { } }
public class SerializeBase { public Vector2 position = new Vector2(0f, 0f); public float rotation = 0f; }

[Serializable]
public class SceneData
{
  // Add type for each serialized type
  public CrateSerialized[] crates;
  public ExplosiveCrateSerialized[] explosiveCrates;
  public PlatformSerialized[] platforms;
  public EnemySerialized[] enemies;
}
