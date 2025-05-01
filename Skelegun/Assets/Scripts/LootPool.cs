using System.Collections.Generic;
using UnityEngine;

public class LootPool : MonoBehaviour
{
    public Dictionary<int, Weapon> commonWeapons;
    public Dictionary<int, Weapon> uncommonWeapons;
    public Dictionary<int, Weapon> rareWeapons;
    public Dictionary<int, Weapon> epicWeapons;
    public Dictionary<int, Weapon> legendaryWeapons;

    public Dictionary<int, Weapon> commonItems;
    public Dictionary<int, Weapon> uncommonItems;
    public Dictionary<int, Weapon> rareItems;
    public Dictionary<int, Weapon> epicItems;
    public Dictionary<int, Weapon> legendaryItems;
}
