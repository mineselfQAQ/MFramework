using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyStats : EntityStats<EnemyStats>
{
#if UNITY_EDITOR
    [MenuItem("Assets/MCreate/3DGame/EnemyStats", false, priority = 2, secondaryPriority = 1.0f)]
    internal static void Create()
    {
        var asset = ScriptableObject.CreateInstance<EnemyStats>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Path.GetExtension(path) != "")//选中的是文件
        {
            path = path.Replace(Path.GetFileName(path), "");
        }
        path = $"{path}/New{typeof(EnemyStats)}.asset";

        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif

    [Header("General Stats")]
    public float gravity = 35f;
    public float snapForce = 15f;
    public float rotationSpeed = 970f;
    public float deceleration = 28f;
    public float friction = 16f;
    public float turningDrag = 28f;

    [Header("Dead Stats")]
    public bool cleanCorpse = true;
    public float cleanDuration = 2.0f;

    [Header("Contact Attack Stats")]
    public bool canAttackOnContact = true;
    public bool contactPushback = true;
    public float contactOffset = 0.15f;
    public int contactDamage = 1;
    public float contactPushBackForce = 18f;
    public float contactSteppingTolerance = 0.1f;

    [Header("Patrol Stats")]
    public float patrolEnterRange = 5f;
    public float patrolExitRange = 8f;

    [Header("Follow Stats")]
    public float followAcceleration = 10f;
    public float followTopSpeed = 4f;

    [Header("Waypoint Stats")]
    public bool faceWaypoint = true;
    public float waypointMinDistance = 0.5f;
    public float waypointAcceleration = 10f;
    public float waypointTopSpeed = 2f;
}