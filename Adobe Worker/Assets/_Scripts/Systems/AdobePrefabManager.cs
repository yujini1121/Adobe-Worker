using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobePrefabManager : MonoBehaviour
{
    [Header("AttackRange")]
    [SerializeField] GameObject swordAttackRange;
    [SerializeField] GameObject spearAttackRange;
    [SerializeField] GameObject arrowAttackRange;
    [SerializeField] GameObject shurikenAttackRange;
    [SerializeField] GameObject amuletAttackRange;

    [Header("ActionRange")]
    [SerializeField] GameObject axeActionRange;
    [SerializeField] GameObject pickaxeActionRange;

    [Header("DropItem")]
    [SerializeField] GameObject defaultDropItemPrefab;


    static public GameObject swordRange { get; private set; }
    static public GameObject spearRange { get; private set; }
    static public GameObject arrowRange { get; private set; }
    static public GameObject shurikenRange { get; private set; }
    static public GameObject amuletRange { get; private set; }
    static public GameObject defaultDropItem { get; private set; }

    static public GameObject axeRange { get; private set; }
    static public GameObject pickaxeRange { get; private set; }


    private void Awake()
    {
        swordRange = swordAttackRange;
        spearRange = spearAttackRange;
        arrowRange = arrowAttackRange;
        shurikenRange = shurikenAttackRange;
        amuletRange = amuletAttackRange;

        axeRange = axeActionRange;
        pickaxeRange = pickaxeActionRange;

        defaultDropItem = defaultDropItemPrefab;
    }
}
