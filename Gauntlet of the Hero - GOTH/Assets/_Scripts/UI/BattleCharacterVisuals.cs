using UnityEngine;
using System.Threading.Tasks;

public class BattleCharacterVisuals : MonoBehaviour
{
    public Animator animator;
    public Transform attackPosition; // Prazan objekat ispred mete
    private Vector3 originalPosition;

    private void Awake() => originalPosition = transform.position;

    public async Task PlayAttackAnimation()
    {
        // 1. Okini animaciju zamaha (safety: proveri animator)
        SafeSetTrigger("Attack");

    }

    public void PlayHurt() => SafeSetTrigger("Hurt");
    public void PlayDie() => SafeSetTrigger("Die");

    private void SafeSetTrigger(string trigger)
    {
        if (animator == null)
        {
            Debug.LogWarning($"Animator missing on {gameObject.name}; skipping trigger '{trigger}'");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"Animator on {gameObject.name} has no AnimatorController; skipping trigger '{trigger}'");
            return;
        }

        animator.SetTrigger(trigger);
    }
}