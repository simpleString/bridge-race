using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class BasePlayer : MonoBehaviour {
    public GameManager.MyColor color;

    public Action<GameManager.MyColor> actionPlayerLostBrick;
    public Action<BasePlayer> actionPlayerDead;


    [SerializeField] protected float _speed = 5f;

    protected Vector3 _movement;
    protected Animator _animator;
    public Transform brickHolder;
    public Transform portableBrick;
    protected NavMeshAgent _agent;


    protected void Awake() {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    protected void Update() {
        float velocityZ = Vector3.Dot(_agent.velocity.normalized, transform.forward);
        float velocityX = Vector3.Dot(_agent.velocity.normalized, transform.right);

        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);

    }

    public Stack<Transform> bricks = new Stack<Transform>();

    protected void AddBrickToBridge(GameObject brick) {
        var brickStript = brick.GetComponent<Stair>();
        brickStript.ChangeColor(color);
        Destroy(bricks.Pop().gameObject);
        actionPlayerLostBrick?.Invoke(color);
    }


    protected void AddBrickToPlayer() {
        var newPortableBrick = Instantiate(portableBrick, new Vector3(
            brickHolder.position.x,
            brickHolder.position.y + portableBrick.GetComponent<Renderer>().bounds.size.y * bricks.Count,
            brickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.tag = MyConstants.TagNull; // it's save us from bugs, but it's not required
        newPortableBrick.GetComponent<Renderer>().material.color = GameManager.GetUnityColorByMyColor(color);
        newPortableBrick.parent = brickHolder;
        bricks.Push(newPortableBrick);
    }
}