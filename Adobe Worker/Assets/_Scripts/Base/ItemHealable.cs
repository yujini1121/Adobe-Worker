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
            Debug.Log($"<!>ItemHealable.Use(AdobeItemUseArguments arguments) : {gameObject.name}�� �÷��̾� ��Ʈ�ѷ��� �������� ����!");
            return;
        }
        if (itemPack.IsExist(id) == false)
        {
            Debug.Log($"<!>ItemHealable.Use(AdobeItemUseArguments arguments) : {gameObject.name}�� itempack�� �ش� ������Ʈ�� ����Ű����, �κ��丮�� ��ȿ�� amount({amount})�� �ƴմϴ�. id : {id}");
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
