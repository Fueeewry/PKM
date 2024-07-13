using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void damaged(int damage);
    void move();
    void prepareattack();
}
