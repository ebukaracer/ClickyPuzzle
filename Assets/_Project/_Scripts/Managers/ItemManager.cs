using Racer.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Retrieves a scriptable object.
/// </summary>
class ItemManager : SingletonPattern.Singleton<ItemManager>
{
    // Scriptable objects available
    [SerializeField]
    List<PuzzlePopulator> puzzles;



    /// <summary>
    /// Retrieves a scriptable object 
    /// </summary>
    /// <param name="index">Index defined in the scriptable object data</param>
    /// <returns>Scriptable object based on the index defined, otherwise null</returns>
    public PuzzlePopulator GetPuzzle(int index)
    {
        var currentLevel = puzzles.SingleOrDefault(p => p.GetIndexID == index);

        if (currentLevel == null)
        {
            Logging.LogWarning("Index does not exist!");

            return null;
        }

        return currentLevel;
    }

    public int GetItemCount => puzzles.Count;
}
