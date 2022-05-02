using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;
    // Start is called before the first frame update
    private Animator animator;
    public float moveSpeedWalking = 2;
    public float moveSpeedRunning = 4;
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;//O tempo que a smooth magnitude vai demorar a chegar a inputmagnitude
    public float turnSpeed = 8;

    float angle; // faz track do angulo
    float smoothInputMagnitude;
    float smoothMoveVelocity;//Faz tracking da velocidade do smothing
    Vector3 velocity;
    Rigidbody rigidbody;
    bool disabled;// desliga o controlo quando o player � encontrado

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;//Subscreve ao evento
        RotatingGuard.OnRotGuardHasSpottedPlayer += Disable;
        animator = GetComponentInChildren<Animator>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDirection = Vector3.zero; //� zero caso o player tenha sido encontrado
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;//Vai buscar a dire��o do movimento
        }

        if(Input.GetKey(KeyCode.LeftShift) && StaminaBar.instance.currentStamina != 0 && inputDirection != Vector3.zero){
            moveSpeed = moveSpeedRunning;
            StaminaBar.instance.UserStamina();
        }else{
            moveSpeed = moveSpeedWalking;
        }  

        //muda a dire��o
        //inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;//Vai buscar a dire��o do movimento
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude,ref smoothMoveVelocity,smoothMoveTime);

        
        float targetAngle = Mathf.Atan2(inputDirection.x,inputDirection.z)*Mathf.Rad2Deg;//faz o arctag para as diagonais 
        angle = Mathf.LerpAngle(angle, targetAngle,Time.deltaTime * turnSpeed * inputMagnitude);//Faz smoothing da rota��o e muultiplicamos pela magnitude para que o angulo nao fa�a reset                                                                             
        velocity = transform.forward * moveSpeed * smoothInputMagnitude;

        if(inputDirection != Vector3.zero){
            if(moveSpeed == moveSpeedWalking){
                animator.SetBool("isMoving", true);
                animator.SetBool("isRunning", false);
            }else if(moveSpeed == moveSpeedRunning){
                animator.SetBool("isRunning", true);
            }
        }else{
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
        }
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        if(hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel!=null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    void Disable()
    {
        disabled = true;
    }

    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));//Rota��o
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);//Movimento
    }

    void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;// Retira a subscri��o no caso da scene ser reloaded
        RotatingGuard.OnRotGuardHasSpottedPlayer -= Disable;
    }


}

