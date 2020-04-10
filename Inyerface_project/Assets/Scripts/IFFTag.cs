using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFFTag : MonoBehaviour
{
    public IFF IFF_channel = IFF.bot1;

    [Tooltip("Higher number means enemies will prioritize you")]
    public int aggroPriority = 1;

    public enum IFF
    {
        playerFriendly,
        bot1,
        bot2,
        bot3,
        bot4
    }

}
