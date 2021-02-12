using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator anim;
    EnemyController ec;

    const string run_Param = "run";
    const string carry_Param = "carry";

    private void Start()
    {
        anim = GetComponent<Animator>();
        ec = GetComponent<EnemyController>();
    }

    private void Update()
    {
        switch(ec.enemyState)
        {
            case ENEMYSTATE.Idle:
                SetAnimValues(false, false);
                break;

            case ENEMYSTATE.Run:
                SetAnimValues(true, false);
                break;

            case ENEMYSTATE.CarryIdle:
                SetAnimValues(false, true);
                break;

            case ENEMYSTATE.CarryRun:
                SetAnimValues(true, true);
                break;

            case ENEMYSTATE.Dodged:
                SetAnimValues(false, false);
                break;
        }
    }

    void SetAnimValues(bool run, bool carry)
    {
        resetAnimation(run_Param, run);
        resetAnimation(carry_Param, carry);
    }

    void resetAnimation(string s, bool isTrue = false)
    {
        if (anim.GetBool(s) != isTrue)
            anim.SetBool(s, isTrue);
    }
}
