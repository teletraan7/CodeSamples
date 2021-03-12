using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Character Statistics")]
    public Vector2 movementDirection;
    public float movementSpeed;
    public bool isInMenu = false;


    [Space]
    [Header("Character Attributes")]
    public float MOVEMENT_BASE_SPEED = 1.0f;


    [Space]
    [Header("References:")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Dont Touch Me")]
    public DialogueTrigger dt;


    // Update is called once per frame
    void Update()
    {       
        if (!isInMenu)
        {
            ProcessInputs();
            Move();
            Animate();
        }       
    }


    void ProcessInputs()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize(); 
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
    }

    public void disablemovement()
    {
        rb.velocity = Vector2.zero;
    }

    void Animate()
    {
        if (movementDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.y);
            animator.SetFloat("Speed", movementSpeed);
        }

        animator.SetFloat("Speed", movementSpeed);
    }


}
