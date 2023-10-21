using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStopAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update
    public ThirdPersonMovement player;

    void TakingEnd()
    {
        player.TakingEnd();
    }
}
