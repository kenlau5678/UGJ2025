using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/CardData", order = 1)]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite cardSprite;  // For the card's image.
    public GameObject visualPrefab;  // Optional: Custom visual prefab per type (overrides default).
    public int manaCost;  // Example: Add stats like cost, damage, etc.

    // Effect logic: Use an enum or delegate for flexibility.
    public CardEffectType effectType;

    // Enum for simple effects (expand as needed).
    public enum CardEffectType
    {
        MoveUnit,  // Current default.
        Attack,    // Example: Deal damage.
        Heal,      // Example: Restore health.
        // Add more...
    }

    // Optional: Delegate for complex effects (e.g., custom actions).
    public System.Action ExecuteCustomEffect;  // Assign in editor or code.
}