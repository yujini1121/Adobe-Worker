using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EActionType
{
    none,
    fist,
    sword,
    spear,
    bow,
    shuriken,
    amulet,
    axe,
    pickaxe,
}

/// <summary>
///     
/// </summary>
/// <remarks>
///     해당 클래스는 플래이어의 움직임, 애니메이션과 쿨타임을 담당합니다.
/// </remarks>
public class PlayerMovement : MonoBehaviour
{
    private bool isRunning = false;
    private int itemType = 0;
    private float ActionableTime;
    private Dictionary<EActionType, float> timeForActions = new Dictionary<EActionType, float>()
    {
        { EActionType.none, 0 },
        { EActionType.fist, 0.125f },
        { EActionType.sword, 0.5f },
        { EActionType.spear, 0.5f },
        { EActionType.bow, 1.0f },
        { EActionType.shuriken, 0.25f },
        { EActionType.amulet, 0.25f },
        { EActionType.axe, 0.5f },
        { EActionType.pickaxe, 0.5f },
    };
    EActionType currentAction = EActionType.none;
    AdobeItemPack inventory;
    Animator animator;
    Rigidbody rigidbodyPlayer;
    Transform characterMeshTransform;

    [SerializeField] private bool isDebugging = true;

    private void Awake()
    {
        ActionableTime = Time.time;
    }
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<AdobeItemPack>();
        // 만약 여기 아래에 에러가 나면, 원래 했던대로 주인공 캐릭터 메쉬 받아서 쓰기

        characterMeshTransform = transform.Find("stand");

        if (characterMeshTransform != null)
        {
            animator = characterMeshTransform.GetComponent<Animator>();
        }
        else
        {
            Debug.Log("<!>PlayerMovement.Start() : Idle이라는 자식 게임오브젝트가 존재하지 않습니다!");
        }

        rigidbodyPlayer = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.inventory.Count > 0)
        {
            switch (inventory.inventory[inventory.InventoryIndex].Id)
            {
                case 201: currentAction = EActionType.sword; itemType = 1; break;
                case 202: currentAction = EActionType.sword; itemType = 1; break;
                case 203: currentAction = EActionType.sword; itemType = 1; break;
                case 204: currentAction = EActionType.spear; itemType = 2; break;
                case 205: currentAction = EActionType.spear; itemType = 2; break;
                case 206: currentAction = EActionType.spear; itemType = 2; break;
                case 207: currentAction = EActionType.bow; itemType = 3; break;
                case 210: currentAction = EActionType.shuriken; itemType = 4; break;
                case 211: currentAction = EActionType.amulet; itemType = 5; break;
                case 701: currentAction = EActionType.pickaxe; itemType = 11; break;
                case 702: currentAction = EActionType.pickaxe; itemType = 11; break;
                case 703: currentAction = EActionType.pickaxe; itemType = 11; break;
                case 704: currentAction = EActionType.axe; itemType = 12; break;
                case 705: currentAction = EActionType.axe; itemType = 12; break;
                case 706: currentAction = EActionType.axe; itemType = 12; break;
                case 1200: currentAction = EActionType.none; itemType = 0; break;
                case 1201: currentAction = EActionType.fist; itemType = 0; break;
                case 1202: currentAction = EActionType.sword; itemType = 1; break;
                // sword here
                case 1203: currentAction = EActionType.spear; itemType = 2; break;
                // spear id here
                case 1204: currentAction = EActionType.bow; itemType = 3; break; // change bow id here
                case 1205: currentAction = EActionType.shuriken; itemType = 4; break;
                case 1206: currentAction = EActionType.amulet; itemType = 5; break;
                default: currentAction = EActionType.none; itemType = 0; break;
            }
        }
        else
        {
            currentAction = EActionType.none;
        }
        
        if (animator != null)
        {
            animator.SetBool("IsUsingItem", IsActionable() == false);
            DoMove();
        }
    }

    public void DoAction()
    {


        ActionableTime = Time.time + timeForActions[currentAction];

        // 여기에 애니메이션 실행 함수
        if (animator != null)
        {
            animator.SetBool("IsUsingItem", true);
            animator.SetInteger("CurrentItemType", itemType);
        }
        if (isDebugging)
        {
            Debug.Log($"PlayerMovement.DoAction() 호출됨 ActionableTime = {ActionableTime} / timeForActions[currentAction] = {timeForActions[currentAction]} / item id = {inventory.inventory[inventory.InventoryIndex].Id}");
        }
    }

    public void DoMove()
    {
        float m_keyHorizontalAxisValue = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        float m_keyVerticalAxisValue = Mathf.Abs(Input.GetAxisRaw("Vertical"));

        if (animator != null)
        {
            //Debug.Log($"speed = {Mathf.Clamp(rigidbodyPlayer.velocity.magnitude, -1, 1)}");
            animator.SetFloat("MoveSpeed", Mathf.Clamp(m_keyHorizontalAxisValue + m_keyVerticalAxisValue, -1, 1));
            //animator.SetFloat("MoveSpeed", Mathf.Clamp(rigidbodyPlayer.velocity.magnitude, -1, 1));
        }
    }

    public bool IsActionable()
    {
        return Time.time >= ActionableTime;
    }

    private void CheckMoveKey()
    {
        if (IsActionable() == false)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {

        }
    }
}
