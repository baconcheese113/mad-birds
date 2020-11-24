using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckBug : Bug
{
  [SerializeField] private float _flapForce = 5000f;

  private Animator _animator;
  private bool _usedFlap;

  protected override void Awake()
  {
    base.Awake();
    _animator = GetComponentInChildren<Animator>();
  }

  public override void Reset()
  {
    base.Reset();
    _usedFlap = false;
  }

  protected override void Update()
  {
    base.Update();
    if (Input.GetKeyDown(KeyCode.Mouse0) && _bugWasLaunched && !_usedFlap)
    {
      Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
      rigidbody.AddForce((Vector2.up * _flapForce) - new Vector2(rigidbody.velocity.x, 0f), ForceMode2D.Impulse);
      _animator.Play("Base Layer.Flap");
      _usedFlap = true;
    }
  }
}
