using UnityEngine;

[CreateAssetMenu(fileName = "IconPack", menuName = "CharsooAssets/IconPack", order = 1)]
public class IconPack : ScriptableObject
{
    [Header("Errors")]
    public Sprite GeneralError;
    public Sprite NetworkError;
    public Sprite ServiceError;
    public Sprite UnkownPhone;
    public Sprite InvalidCode;
}