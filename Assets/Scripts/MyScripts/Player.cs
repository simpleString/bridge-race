using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Player : BasePlayer {

    //////////////////////////////////// Base properties
    [SerializeField] FloatingJoystick _floatingJoystick;

    public static Player Instance = null;
    ///////////////////////////////////




    public float collisionOffset = .1f;


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs") && !collider.CompareTag(color.ToString())) {
            if (bricks.Count > 0) {
                GameManager.Vibrate();
                AddBrickToBridge(collider.gameObject);
            } else {
                // Don't let player go to stairs
                _agent.Move(collider.gameObject.transform.right * collisionOffset);
            }
        } else if (collider.tag == "Player") {
            if (!Physics.Raycast(transform.position, Vector3.down, 10f, LayerMask.NameToLayer("Stairs"))) return;
            CheckPlayerCollision(collider);
        } else if ((collider.tag == color.ToString() || collider.tag == "Free") && collider.gameObject.layer != LayerMask.NameToLayer("Stairs")) {
            GameManager.Vibrate();
            AddBrickToPlayer(collider.gameObject);
        } else if (collider.CompareTag("Bonus")) {
            GameManager.Vibrate();
            var bonusScript = collider.GetComponent<Bonus>();
            GetBunusEffect(bonusScript.type);
            bonusScript.Destory();
        }
    }


    new void Awake() {
        if (Instance != null) {
            Destroy(this);
        }
        Instance = this;

        base.Awake();
    }

    new void Start() {
        base.Start();
        color = GameManager.Instance.playerColor;
    }


    new void Update() {
#if UNITY_EDITOR
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
#else
        var vertical = _floatingJoystick.Vertical;
        var horizontal = _floatingJoystick.Horizontal;
#endif
        _movement = new Vector3(horizontal, 0f, vertical);
        

        _animator.SetBool("IsRun", _movement.magnitude > 0);
    }

    void FixedUpdate()
    {
        
        if (_movement.magnitude > 0) {
            _movement.Normalize();
            _movement *= _speed;


            transform.rotation = Quaternion.LookRotation(new Vector3(_movement.x, 0, _movement.z));
        }

        if (_agent.enabled)
        {
            _agent.Move(_movement * Time.deltaTime);
        }
    }
            

}
