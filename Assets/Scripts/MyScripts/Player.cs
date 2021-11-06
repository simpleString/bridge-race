using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : BasePlayer {
    [SerializeField] FloatingJoystick _floatingJoystick;
    private PlayerController _playerController;

    new void Awake() {
        _playerController = GetComponent<PlayerController>();
        base.Awake();
    }
    void Update()
    {
    #if UNITY_EDITOR
        var vertical = Input.GetAxis ("Vertical");
        var horizontal = Input.GetAxis ("Horizontal"); 
        

    #else
        var vertical = _floatingJoystick.Vertical;
        var horizontal = _floatingJoystick.Horizontal;
    #endif
        movement = new Vector3(horizontal, 0f, vertical);
        if (movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= _speed * Time.fixedDeltaTime;
            
        }
        _playerController.Move(movement);

        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);
        
        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }
    
}
