using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void damaged(int damage, GameObject a);
    void move();
    void prepareattack();
    bool checkstillalive();
    int stealhealth(int damage);
    void stunfor(int value);
    void reducedamageby(float a, int b);
}
