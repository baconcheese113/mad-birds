using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBug : Bug
{
  [SerializeField] private byte _numEggs = 3;
  [SerializeField] private GameObject _eggBomb;

  private byte _eggsLeft;

  protected override void Awake()
  {
    base.Awake();
    _eggsLeft = _numEggs;
  }

  public override void Reset()
  {
    base.Reset();
    _eggsLeft = _numEggs;
  }

  protected override void Update()
  {
    base.Update();
    if (Input.GetKeyDown(KeyCode.Mouse0) && _birdWasLaunched && _eggsLeft > 0)
    {
      Instantiate(_eggBomb, transform.TransformPoint(Vector3.down * 2), transform.rotation);
      _eggsLeft--;
    }
  }
}
