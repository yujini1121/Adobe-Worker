using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealable : AdobeItemBase
{
    Healamount GetHeal(int id)
    {
        switch (id)
        {
            case 118: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 121: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 401: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 402: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 403: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 404: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 405: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 406: return new Healamount() { addHealth = 10, addStamina = 10 };
            case 501: return new Healamount() { addHealth = 30 };
            case 502: return new Healamount() { addStamina = 50, staminaLimit = 90 };
            case 503: return new Healamount() { addHealth = 10 };
            default: return new Healamount() { };
        }
    }

    public override void Use(AdobeItemUseArguments arguments)
    {
        AdobePlayerController playerController = arguments.itemUser.GetComponent<AdobePlayerController>();
        if (playerController == null)
        {
            Debug.Log($"<!>ItemHealable.Use(AdobeItemUseArguments arguments) : {gameObject.name}의 플레이어 컨트롤러가 존재하지 않음!");
            return;
        }
        if (itemPack.IsExist(id) == false)
        {
            Debug.Log($"<!>ItemHealable.Use(AdobeItemUseArguments arguments) : {gameObject.name}의 itempack은 해당 컴포넌트를 가리키지만, 인벤토리에 유효한 amount({amount})가 아닙니다. id : {id}");
            itemPack.Remove(id, 0);
            return;
        }

        Healamount healamount = GetHeal(id);

        if (id != 502)
        {
            playerController.Heal(healamount.addHealth, healamount.addStamina);
        }
        else
        {
            playerController.HealStamina(healamount.addStamina, healamount.staminaLimit);
        }

        itemPack.Remove(id);
    }
}
