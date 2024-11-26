using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int WalkingHash = Animator.StringToHash("Walking");
    private readonly int JumpingHash = Animator.StringToHash("Jumping");
    private readonly int DeadHash = Animator.StringToHash("Dead");

    void Update()
    {
        UpdateVisuals();

        switch (playerController.GetFacingDirection())
        {
            case FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }
    }

    private void UpdateVisuals()
    {
        if (playerController.previousState != playerController.currentState)
        {
            switch (playerController.currentState)
            {
                case PlayerState.idle:
                    animator.CrossFade(IdleHash, 0);
                    break;
                case PlayerState.walking:
                    animator.CrossFade(WalkingHash, 0);
                    break;
                case PlayerState.jumping:
                    animator.CrossFade(JumpingHash, 0);
                    break;
                case PlayerState.dead:
                    animator.CrossFade(DeadHash, 0);
                    break;
            }
        }
    }
}
