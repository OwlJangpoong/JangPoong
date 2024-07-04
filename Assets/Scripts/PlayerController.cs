using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.W;
    [SerializeField]
    private KeyCode slideKeyCode = KeyCode.S;
    [SerializeField]
    private float slideDistance = 3.0f;
    [SerializeField]
    private float slideSpeed = 10.0f;

    private MovementRigidbody2D movement;
    private PlayerAnimator playerAnimator;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D rb;
    private bool isSliding = false;
    private Vector2 slideDirection;
    private float slideRemainingDistance;
    private Quaternion originalRotation;

    private void Awake()
    {
        movement = GetComponent<MovementRigidbody2D>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        originalRotation = capsuleCollider.transform.rotation;
    }

    private void Update()
    {
        // Ű �Է� (��/�� ���� Ű, ���� Shift Ű)
        float x = Input.GetAxisRaw("Horizontal");
        float offset = 0.5f + Input.GetAxisRaw("Sprint") * 0.5f;

        // �ȱ��� �� ���� ������ -0.5 ~ 0.5
        // �ٱ��� �� ���� ������ -1 ~ 1�� ����
        x *= offset;

        // �÷��̾��� �̵� ���� (��/��)
        UpdateMove(x);
        // �÷��̾��� ���� ����
        UpdateJump();
        // �÷��̾��� �����̵� ����
        UpdateSlide();
        // �÷��̾� �ִϸ��̼� ����
        playerAnimator.UpdateAnimation(x);
    }

    private void UpdateMove(float x)
    {
        // �����̵� �߿��� �̵��� ����
        if (!isSliding)
        {
            // �÷��̾��� ������ �̵� (��/��)
            movement.MoveTo(x);
        }
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKeyCode))
        {
            movement.Jump();
        }

        if (Input.GetKey(jumpKeyCode))
        {
            movement.IsLongJump = true;
        }
        else if (Input.GetKeyUp(jumpKeyCode))
        {
            movement.IsLongJump = false;
        }
    }

    private void UpdateSlide()
    {
        if (Input.GetKeyDown(slideKeyCode))
        {
            if (!isSliding)
            {
                isSliding = true;
                slideRemainingDistance = slideDistance;
                slideDirection = new Vector2(transform.localScale.x, 0).normalized; // �÷��̾��� ���� �ٶ󺸴� ����
                Debug.Log("�����̵�!");
                // capsuleCollider ȸ��
                capsuleCollider.transform.rotation = Quaternion.Euler(0, 0, 90);
                // �����̵� �ִϸ��̼� ����
                playerAnimator.StartSliding();
            }
        }

        if (isSliding)
        {
            float moveStep = slideSpeed * Time.deltaTime;
            if (moveStep > slideRemainingDistance)
            {
                moveStep = slideRemainingDistance;
            }

            // �����̵� �߿��� Rigidbody2D�� �ӵ��� ������ ���������� �̵�
            rb.velocity = new Vector2(slideDirection.x * slideSpeed, rb.velocity.y);
            slideRemainingDistance -= moveStep;

            if (slideRemainingDistance <= 0)
            {
                isSliding = false;
                Debug.Log("�����̵� ��");
                // capsuleCollider ȸ�� ����
                capsuleCollider.transform.rotation = originalRotation;
                // �����̵� �ִϸ��̼� ����
                playerAnimator.StopSliding();
                // �����̵� ���� �� �ӵ� �ʱ�ȭ
                rb.velocity = Vector2.zero;
            }
        }
    }
}
