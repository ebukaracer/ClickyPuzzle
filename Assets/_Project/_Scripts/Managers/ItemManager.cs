using Racer.Utilities;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-14)]
internal class ItemManager : SingletonPattern.SingletonPersistent<ItemManager>
{
    [SerializeField] private PuzzleItem[] puzzles;


    /// <summary>
    /// Retrieves a scriptable object item.
    /// </summary>
    /// <param name="index">Index(ItemID) defined in the scriptable object data.</param>
    /// <returns>Scriptable object based on the index defined, otherwise null.</returns>
    /// <remarks>
    /// The indexes(ItemID) for every item must be consecutively sorted and start from 0 -> count.
    /// </remarks>
    public PuzzleItem GetPuzzle(int index)
    {
        var currentLevel = puzzles.SingleOrDefault(p => p.ItemID == index);

        if (currentLevel != null)
            return currentLevel;

        Logging.LogError($"Specified Index: <b>{index}</b> does not exist!");

        return null;
    }

    public int ItemCount => puzzles.Length;
}
