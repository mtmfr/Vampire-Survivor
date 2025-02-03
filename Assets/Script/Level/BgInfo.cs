using System.Runtime.CompilerServices;
using UnityEngine;

public class BgInfo : MonoBehaviour
{
    [SerializeField] private SO_Stage currentLevel;
    public int id;
    public Sprite BgSprite { get => currentLevel.BgSprite; }
}
