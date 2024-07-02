using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	private	Animator animator;
	private	MovementRigidbody2D	movement;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		movement = GetComponentInParent<MovementRigidbody2D>();
	}

	public void UpdateAnimation(float x)
	{
		// ��/�� ����Ű �Է��� ���� ��
		if ( x != 0 )
		{
			// �÷��̾� ��������Ʈ ��/�� ���� : �ٶ󺸴� ���� ��
			SpriteFlipX(x);
		}

		animator.SetBool("isJump", !movement.IsGrounded);

		// �ٴڿ� ��� ������
		if ( movement.IsGrounded )
		{
			// velocityX�� 0�̸� "Idle", velocityX�� 0.5�̸� "Walk", velocityX�� 1�̸� "Run" ���
			animator.SetFloat("velocityX", Mathf.Abs(x));
		}
		// �ٴڿ� ��� ���� ������
		else
		{
			// velocityY�� �����̸� "JumpDown", velocityY�� ����̸� "JumpUp" ���
			animator.SetFloat("velocityY", movement.Velocity.y);
		}
	}

	// SpriteRenderer ������Ʈ�� Filp�� �̿��� �̹����� �������� ��
	// ȭ�鿡 ��µǴ� �̹��� ��ü�� �����Ǳ� ������
	// �÷��̾��� ���� Ư�� ��ġ���� �߻�ü�� �����ϴ� �Ͱ� ����
	// ������ȯ�� �ʿ��� ���� Transform.Scale.x�� -1, 1�� ���� ����
	private void SpriteFlipX(float x)
	{
		transform.parent.localScale = new Vector3((x < 0 ? -1 : 1), 1, 1);
	}
}


