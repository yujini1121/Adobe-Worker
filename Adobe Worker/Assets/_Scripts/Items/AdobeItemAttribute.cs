using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Add/Item")]
public class AdobeItemAttribute : ScriptableObject
{
    [Header("ID")]
    [SerializeField] private int mItemID;
    public int itemID
    {
        get { return mItemID; }
    }

    [Header("Name")]
    [SerializeField] private string mItemName;
    public string itemName
    {
        get { return mItemName; }
    }

    [Header("Type")]
    [SerializeField] private string mItemType;
    public string itemType
    {
        get { return mItemType; }
    }

    [Header("Image")]
    [SerializeField] private Sprite mItemImage;
    public Sprite itemImage
    {
        get { return mItemImage; }
    }

    [Header("Description")]
    [SerializeField] private string mItemDescription;
    public string itemDescription
    {
        get { return mItemDescription; }
    }
}
