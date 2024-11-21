using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTable
{
    public float damage;
}

public class SimpleWeaponSpec : MonoBehaviour
{
    public const int ITEM_ID_STONE_ARROW = 208;
    public const int ITEM_ID_IRON_ARROW = 209;
    public const float STAMINA_FOR_MAGIC = 5.0f;

    public static WeaponTable Get(int id)
    {
        switch (id)
        {
            case 201: return new WeaponTable() { damage = 5 }; // 검
            case 202: return new WeaponTable() { damage = 7 };
            case 203: return new WeaponTable() { damage = 9 };
            case 204: return new WeaponTable() { damage = 4 }; // 창
            case 205: return new WeaponTable() { damage = 5 };
            case 206: return new WeaponTable() { damage = 6 };
            case ITEM_ID_STONE_ARROW: return new WeaponTable() { damage = 5 }; // 화살
            case ITEM_ID_IRON_ARROW: return new WeaponTable() { damage = 6 };
            case 211: return new WeaponTable() { damage = 10 }; // 부적
            default: return new WeaponTable() { damage = 2 }; // 맨손
        }
    }
}
