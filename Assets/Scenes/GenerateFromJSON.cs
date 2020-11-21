using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

/*
* To add to the serialized data, you need to:
*
* 1) Implement ITypeWithSerialize and SerializeBase in the new component
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
  [SerializeField] private Int16 _levelNumber = 1;
  [SerializeField] private GameObject _cratePrefab;
  [SerializeField] private GameObject _explosiveCratePrefab;
  [SerializeField] private GameObject _platformPrefab;
  [SerializeField] private GameObject _enemyPrefab;

  [Serializable]
  class MutationVariables { public string json; }
  [Serializable]
  class QueryVariables { public Int16 levelNumber; }

  [Serializable]
  class Request<V>
  {
    public V variables;
    public string query;
  }

  IEnumerator fetch<V>(Request<V> req, System.Action<string> successCallback)
  {
    string fullReq = JsonUtility.ToJson(req);
    print(fullReq);
    UnityWebRequest uwr = new UnityWebRequest("http://localhost:4000/graphql", "POST");
    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(fullReq);
    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    uwr.SetRequestHeader("Content-Type", "application/json");

    yield return uwr.SendWebRequest();
    if (uwr.isNetworkError) print("Error while sending: " + uwr.error);
    else
    {
      print("Received: " + uwr.downloadHandler.text);
      successCallback(uwr.downloadHandler.text);
    }
  }

  private string getFilePath()
  {
    return Path.Combine(Application.dataPath, _fileName + _levelNumber.ToString() + ".json");
  }

  public void DiscardChanges()
  {
    string filePath = getFilePath();
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
  }

  public void AttemptLoad()
  {
    if (!File.Exists(getFilePath())) return;
    IDisposable[] objs = UnityEngine.Object.FindObjectsOfType<IDisposable>();
    foreach (IDisposable obj in objs) Destroy(obj.gameObject);
    ImportScene();
  }

  class QueryReturnType
  {
    public Name data;
    public class Name
    {
      public SceneData downloadJsonScene;
    }
  }

  // Downloads the current server layout for the current level number
  public void DownloadThisScene()
  {
    Request<QueryVariables> req = new Request<QueryVariables>();
    req.variables = new QueryVariables();
    req.variables.levelNumber = _levelNumber;
    req.query = "query DownloadJsonScene($levelNumber: Int!) { downloadJsonScene(levelNumber: $levelNumber) }";
    StartCoroutine(fetch(req, (response) =>
    {
      string responseWithoutEnd = response.Remove(response.Length - 4, 4);
      string responseWithoutStart = responseWithoutEnd.Remove(0, 30);
      string unescapedResponse = Regex.Unescape(responseWithoutStart);
      SceneData data = JsonUtility.FromJson<SceneData>(unescapedResponse); // TODO do we need??
      File.WriteAllText(getFilePath(), unescapedResponse);
      AttemptLoad();
    }));
  }

  // Writes locally to a json file and then uses that to upload to a webserver
  public void UploadThisScene()
  {
    ExportScene();
    Request<MutationVariables> req = new Request<MutationVariables>();
    string contents = File.ReadAllText(getFilePath());
    req.variables = new MutationVariables();
    req.variables.json = contents;
    req.query = "mutation UploadJsonScene($json: String!) { uploadJsonScene(json: $json) }";
    StartCoroutine(fetch(req, (_) => { }));
  }

  public void ImportScene()
  {
    string contents = File.ReadAllText(getFilePath());
    SceneData data = JsonUtility.FromJson<SceneData>(contents);
    _levelNumber = data.levelNumber;
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
    data.levelNumber = _levelNumber;
    data.crates = GetSerializedVersion<Crate, CrateSerialized>();
    data.explosiveCrates = GetSerializedVersion<ExplosiveCrate, ExplosiveCrateSerialized>();
    data.platforms = GetSerializedVersion<Platform, PlatformSerialized>();
    data.enemies = GetSerializedVersion<Enemy, EnemySerialized>();
    _data = data;
    _json = JsonUtility.ToJson(_data);
    File.WriteAllText(getFilePath(), _json);
  }

  private S[] GetSerializedVersion<T, S>() where T : ITypeWithSerialize<S>
  {
    T[] objs = FindObjectsOfType<T>();
    S[] serializedObjs = new S[objs.Length];
    for (int x = 0; x < serializedObjs.Length; x++)
    {
      serializedObjs[x] = objs[x].Serialize();
    }
    return serializedObjs;
  }

  private void InstanceFromSerialized<T, S>(S[] data, GameObject prefab) where T : ITypeWithSerialize<S> where S : SerializeBase
  {
    foreach (S s in data)
    {
      GameObject go = Instantiate(prefab, new Vector3(s.x, s.y, 0), Quaternion.Euler(0, 0, s.rotation));
      T t = go.GetComponent<T>();
      t.Deserialize(s);
    }
  }
}

public abstract class IDisposable : MonoBehaviour { }
public abstract class ITypeWithSerialize<S> : IDisposable { public abstract S Serialize(); public virtual void Deserialize(S data) { } }
public class SerializeBase { public float x = 0f; public float y = 0f; public float rotation = 0f; }

[Serializable]
public class SceneData
{
  public Int16 levelNumber = 1;
  // Add type for each serialized type
  public CrateSerialized[] crates;
  public ExplosiveCrateSerialized[] explosiveCrates;
  public PlatformSerialized[] platforms;
  public EnemySerialized[] enemies;
}
