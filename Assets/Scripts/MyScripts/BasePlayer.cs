using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour {

    [SerializeField] protected float _speed = 3;
    public System.Action<GameManager.MyColor> playerLostBrick;
    public System.Action<BasePlayer> playerDead;


    public bool isCanMove = true;
    public Transform BrickHolder;
    public Transform PortableBrickPrefab;

    Transform _basePlatform; // Minimal y coord. If player stay smaller that it's he's dead.

    public GameManager.MyColor myColor;

    public float collisionOffset = .1f; // Offset for climbing

    protected Vector3 movement;

    protected Animator _animator;
    protected void Awake() {

        _animator = GetComponent<Animator>();
    }

    public Stack<Transform> countOfBricks = new Stack<Transform>();

    protected IEnumerator Jump(Collider collider) {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        var currentPosition = rb.transform.position;
        var movePosition = rb.position + Vector3.up - collider.transform.forward * GameManager.Instance.playersCollisionForce;
        // var movePosition = rb.position + Vector3.up - new Vector3(Random.Range(-2, 3), 0, Random.Range(-2, 3)) * playersCollisionForce;
        var t = 0f;
        while (t < GameManager.Instance.jumpTime) {
            transform.position = (Vector3.Lerp(currentPosition, movePosition, t));
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        if (agent != null) {
            rb.isKinematic = true;
            agent.enabled = true;
        }
        yield return null;
    }

    void DropBrick(Transform brickTransform) {
        brickTransform.parent = null;
        var movePosition = brickTransform.position + new Vector3(Random.Range(-2, 3), 0, Random.Range(-2, 3)) * GameManager.Instance.playersCollisionForce;
        brickTransform.gameObject.AddComponent<Rigidbody>();
        var rb = brickTransform.GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6)));
        // for (float t = 0; t < GameManager.Instance.jumpTime; t += Time.deltaTime) {
        //     rb.MovePosition(Vector3.Lerp(brickTransform.transform.position, movePosition, t));
        //     yield return new WaitForFixedUpdate();
        // }
        // yield return null;
    }

    void DropBricks() {
        foreach (var brick in countOfBricks) {
            brick.GetComponent<Brick>().Init(GameManager.MyColor.black, 0, 0);
            DropBrick(brick);
            brick.tag = "Free";
            playerLostBrick?.Invoke(myColor);
        }
        countOfBricks.Clear();

    }

    protected void CheckPlayerCollision(Collider collider) {
        var otherBasePlayerScript = collider.GetComponent<BasePlayer>();
        if (otherBasePlayerScript.countOfBricks.Count > countOfBricks.Count) {
            DropBricks();
            StartCoroutine(Jump(collider));
        }
    }

    public void Init(GameManager.MyColor color, Transform basePlatform) {
        myColor = color;
        _basePlatform = basePlatform;
    }

    protected void Update() {
        if (transform.position.y + 1 < _basePlatform.position.y) {
            playerDead?.Invoke(this);
            Destroy(gameObject);
        }
    }


    protected void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player") {
            CheckPlayerCollision(collider);
        }
        if (collider.tag == myColor.ToString() || collider.tag == "Free") {
            AddBrickToPlayer();
            var brick = collider.gameObject.GetComponent<Brick>();
            brick.Destroy();
            // Destroy(collider.gameObject);
        }
    }

    protected void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            if (collision.gameObject.CompareTag(myColor.ToString())) {
            } else {
                if (countOfBricks.Count > 0)
                    AddBrickToBridge(collision.gameObject);
                else {
                    // Don't let player go to stairs
                    GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
                }
            }

        }
    }

    protected void AddBrickToPlayer() {
        var newPortableBrick = Instantiate(PortableBrickPrefab, new Vector3(
            BrickHolder.position.x,
            BrickHolder.position.y + PortableBrickPrefab.GetComponent<Renderer>().bounds.size.y * countOfBricks.Count, // FIXME:: optimize this shit
            BrickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.tag = MyConstants.TagNull; // it's save us from bugs, but it's not required
        newPortableBrick.GetComponent<Renderer>().material.color = GameManager.GetUnityColorByMyColor(myColor);
        newPortableBrick.parent = BrickHolder;
        countOfBricks.Push(newPortableBrick);
    }


    protected void AddBrickToBridge(GameObject brick) {
        var brickStript = brick.GetComponent<Stair>();
        brickStript.ChangeColor(myColor);
        Destroy(countOfBricks.Pop().gameObject);
        playerLostBrick?.Invoke(myColor);
    }
}