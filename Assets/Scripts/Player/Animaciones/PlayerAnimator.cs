using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animation anim;
    Animator animator;
    PlayerController controller;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        anim = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if( controller.moveDir.x == 0 && controller.moveDir.y == 0)
        {
            animator.SetBool("moving", false);
        }
        else
        {
            animator.SetBool("moving", true);
        }
        DirectionFix();

        if(controller.moveDir.y > 0 && controller.moveDir.x == 0)
        {
            animator.SetBool("up", true);
        }
        else
        {
            animator.SetBool("up", false);
        }
        
        if(controller.moveDir.y < 0 && controller.moveDir.x == 0) 
        {
            animator.SetBool("down", true);

        }
        else
        {
            animator.SetBool("down", false);
        }

    }

    void DirectionFix()
    {
        if (controller.lastHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
