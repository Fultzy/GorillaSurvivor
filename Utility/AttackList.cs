using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class AttackList
{
    public Attack attack = new Attack()
    {
        name = "ThrownPoop Class Name",
        level = 1,
        damage = 10,
        attackSpeed = 2,
        speed = 100,
        range = 100,
        knockback = 100,
        duration = 5,
        size = 1.0f,

        reloadTime = 5,
        reloadCount = 1,
        maxAmmo = 3,
        ammo = 3,


        isProjectile = true,
        isMelee = false
    };

}
