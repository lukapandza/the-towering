using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : EnemyController
{
    protected override void Awake()
    {
        base.health = 12;
        base.Awake();
    }
    protected override void OnDamageEffect()
    {
        
    }
}
