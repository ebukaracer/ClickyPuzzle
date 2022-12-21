using System.Collections.Generic;
using UnityEngine;

// Stores some properties of a sliced-sprite.
[System.Serializable]
internal class SlicedSprite
{
    public string name;

    public List<Sprite> sprites;

    public Sprite emptySlot;
}