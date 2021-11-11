using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour {

    [SerializeField] protected float _speed = 3;
    public System.Action<GameManager.MyColor> playerLostBrick;

    public float playersCollisionForce;
    public float jumpTime;
    public bool isCanMove = true;
    public Transform BrickHolder;
    public Transform PortableBrickPrefab;

    public GameManager.MyColor myColor;

    public float collisionOffset = .1f; // Offset for climbing

    protected Vector3 movement;

    protected Animator _animator;
    protected void Awake() {

        _animator = GetComponent<Animator>();
    }

    public Stack<Transform> countOfBricks = new Stack<Transform>();

    IEnumerator Jump() {
        var rb = GetComponent<Rigidbody>();
        var currentPosition = rb.transform.position;
        var movePosition = rb.position + Vector3.up - new Vector3(Random.Range(-2, 3), 0, Random.Range(-2, 3)) * playersCollisionForce;
        var t = 0f;
        while (t < jumpTime) {
            rb.MovePosition(Vector3.Lerp(currentPosition, movePosition, t));
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    void CheckPlayerCollision(Collider collider) {
        var otherBasePlayerScript = collider.GetComponent<BasePlayer>();
        if (otherBasePlayerScript.countOfBricks.Count > countOfBricks.Count) {
            StartCoroutine(Jump());
            // rb.MovePosition(rb.position + (Vector3.up - rb.velocity) * playersCollisionForce);
            // rb.MovePosition((-rb.velocity + Vector3.up) * playersCollisionForce);
            // GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6)) * 50);
            // GetComponent<Rigidbody>().MovePosition(transform.position + Vector3.up * 20 * Time.fixedDeltaTime);
        }
    }


    protected void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player") {
            CheckPlayerCollision(collider);
        }
        if (collider.tag == myColor.ToString()) {
            AddBrickToPlayer();
            var brick = collider.gameObject.GetComponent<Brick>();
            brick.Destroy();
            // Destroy(collider.gameObject);
        }
    }

    protected void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            if (collision.gameObject.CompareTag(myColor.ToString())) { // FIXME:: Update to work with bots
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