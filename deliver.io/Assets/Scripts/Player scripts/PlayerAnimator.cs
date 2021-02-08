using UnityEngine;

[RequireComponent(typeof(Animator))][RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    PlayerController pc;

    const string run_Param = "run";
    const string carry_Param = "carry";

    void Start()
    {
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        switch(pc.playerState)
        {
            case PLAYERSTATE.Idle:

                resetAnimation(run_Param);
                resetAnimation(carry_Param);

                break;

            case PLAYERSTATE.Run:

                resetAnimation(run_Param, true);
                resetAnimation(carry_Param);

                break;

            case PLAYERSTATE.CarryIdle:

                resetAnimation(carry_Param,true);
                resetAnimation(run_Param);

                break;

            case PLAYERSTATE.CarryRun:

                resetAnimation(run_Param, true);
                resetAnimation(carry_Param, true);

                break;

            case PLAYERSTATE.Dodged:

                resetAnimation(run_Param);
                resetAnimation(carry_Param);

                break;
        }
    }

    void resetAnimation(string s,bool isTrue = false)
    {
        if (anim.GetBool(s) != isTrue)
            anim.SetBool(s, isTrue);
    }
}
