using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathManager : MonoBehaviour
{
    public void OnCompleteAnimation()
    {
        Destroy(this.gameObject);
    }
}
