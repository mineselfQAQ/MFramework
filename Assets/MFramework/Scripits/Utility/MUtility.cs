using UnityEditor;
using UnityEngine;

/// <summary>
/// 暂时未定放在何处的功能
/// </summary>
public static class MUtility
{
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static int CreateUndoGroup(string groupName)
    {
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(groupName);

        return undoGroup;
    }

    public static Vector2 ScreenMidPos => new Vector2(Screen.width / 2, Screen.height / 2);
    
    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static Vector3 RandomVector2()
    {
        return new Vector2(Random(-1f, 1f), Random(-1f, 1f));
    }
}
