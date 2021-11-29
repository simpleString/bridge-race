using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using System.Collections;
using static GameManager;

public class BasePlayer : MonoBehaviour {
    public GameManager.MyColor color;

    public Action<GameManager.MyColor> actionPlayerLostBrick;
    public Action<BasePlayer> actionPlayerDead;


    private bool _isDoubleBonusActive = false;

    [SerializeField] protected float _speed = 5f;

    protected Vector3 _movement;
    protected Animator _animator;
    public Transform brickHolder;
    public Transform portableBrick;
    protected NavMeshAgent _agent;

    protected Renderer _renderer;

    private Rigidbody _rb;
    private CapsuleCollider _collider;

    public float brickForce = 100f;

    public float playerForce = 1000f;

    protected void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _agent.autoTraverseOffMeshLink = false;
    }

    protected void Update() {
        float velocityZ = Vector3.Dot(_agent.velocity.normalized, transform.forward);
        float velocityX = Vector3.Dot(_agent.velocity.normalized, transform.right);

        // TODO:: Implement it code to both player and bot
        if (velocityX != 0 || velocityZ != 0)
            _animator.SetBool("IsRun", true);
        else 
            _animator.SetBool("IsRun", false);
        
        // _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        // _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);

    }

    public Stack<Transform> bricks = new Stack<Transform>();

    protected void AddBrickToBridge(GameObject brick) {
        var brickStript = brick.GetComponent<Stair>();
        brickStript.ChangeColor(color);
        Destroy(bricks.Pop().gameObject);
        actionPlayerLostBrick?.Invoke(color);
    }


    protected void AddBrickToPlayer(GameObject brick) {
        var brickScript = brick.GetComponent<Brick>();
        if (!brickScript.isPickable) return;
        brickScript.Destroy();
        var newPortableBrick = Instantiate(portableBrick, new Vector3(
            brickHolder.position.x,
            brickHolder.position.y + portableBrick.GetComponent<Renderer>().bounds.size.y * bricks.Count,
            brickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.tag = MyConstants.TagNull; // it's save us from bugs, but it's not required
        newPortableBrick.GetComponent<Renderer>().material.color = GameManager.GetUnityColorByMyColor(color);
        newPortableBrick.parent = brickHolder;
        bricks.Push(newPortableBrick);
        //FIXME:: WFT???
        if (_isDoubleBonusActive) {
            newPortableBrick = Instantiate(portableBrick, new Vector3(
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

    protected void Start()
    {
        _renderer.material.color = GameManager.GetUnityColorByMyColor(color);
    }


    protected void CheckPlayerCollision(Collider collider) {
        var otherBasePlayerScript = collider.GetComponent<BasePlayer>();
        if (otherBasePlayerScript.bricks.Count > bricks.Count) {
            DropBricks();
            StartCoroutine(KickPlayer());
        }
    }

    private void DropBricks() {
        foreach (var brick in bricks) {
            brick.GetComponent<Brick>().InitAfterDrop(GameManager.MyColor.black);
            brick.GetComponent<Collider>().isTrigger = false;
            brick.tag = "Free";
            brick.transform.parent = null;
            var rb = brick.gameObject.AddComponent<Rigidbody>();
            rb.AddForce(new Vector3(Random.Range(-2f, 3f), Random.Range(0, 3f), Random.Range(-2f, 3f)) * brickForce);
            actionPlayerLostBrick?.Invoke(color);
        }
        bricks.Clear();

    }


    public void GetBunusEffect(BonusType bonusType) {
        switch (bonusType) {
            case BonusType.Magnet:
                StartCoroutine(MagnetPlayer());
                break;
            case BonusType.Freeze:
                foreach (var player in GameManager.Instance.players) {
                    if (player != this) {
                        player.FreezePlayer();
                    }
                }
                break;
            case BonusType.Double:
                StartCoroutine(DoublePlayer());
                break;
            case BonusType.Speed:
                StartCoroutine(SpeedPlayer());
                break;
        }
    }

    public void FreezePlayer() {
        StartCoroutine(Freezing());

        IEnumerator Freezing() {
            _agent.enabled = false;
            yield return new WaitForSeconds(GameManager.Instance.bonusTime);
            _agent.enabled = true;
        }
    }

    public IEnumerator SpeedPlayer() {
        Player playerThis = null;
        if (this is Player) {
            playerThis = (Player)this;
            playerThis.collisionOffset *= 2;
        }
        _speed *= 2;

        yield return new WaitForSeconds(GameManager.Instance.bonusTime);
        _speed /= 2;
        if (playerThis != null) {
            playerThis.collisionOffset /= 2;
        }
        yield return null;
    }

    public IEnumerator DoublePlayer() {
        _isDoubleBonusActive = true;
        yield return new WaitForSeconds(GameManager.Instance.bonusTime);
        _isDoubleBonusActive = false;
    }

    public IEnumerator MagnetPlayer() {
        // FIXME:: now it's increase main collider. NEeds add another collider for picking bricks
        var initRadius = _collider.radius;
        _collider.radius *= 2;
        yield return new WaitForSeconds(GameManager.Instance.bonusTime);
        _collider.radius = initRadius;
    }

    private IEnumerator KickPlayer() {
        // TODO:: Need's set jump force and add checking onFloor trigger!!!
        // _agent.isStopped = true;
        // _agent.enabled = false;
        // _rb.isKinematic = false;
        // _collider.isTrigger = false;
        // _rb.AddForce(new Vector3(transform.forward.x, 10, transform.forward.z) * playerForce);
        // yield return new WaitForSeconds(1f);
        // _collider.isTrigger = true;
        // _rb.isKinematic = true;
        // _agent.enabled = true;
        // _agent.isStopped = false;
        yield return null;
    }
}