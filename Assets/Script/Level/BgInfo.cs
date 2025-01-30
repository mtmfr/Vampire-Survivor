using System.Runtime.CompilerServices;
using UnityEngine;

public class BgInfo : MonoBehaviour
{
    [SerializeField] private SO_CurrentLevel currentLevel;
    public int id;
    public Sprite bgSprite { get => currentLevel.BgSprite; }
}
