using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;
using System.Numerics;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "New entity data", menuName = "Entity Data")]
public class EntityData : ScriptableObject
{
    [Header("Entity Data:"), Space(10)]
    [HideInInspector] public string entityName;
    
    [HideInInspector] public EntityType entityType;
    [HideInInspector] public int entityID;
    [HideInInspector] public Stats stats;
    [HideInInspector,SerializeField] public List<SkillEnemy> avaliableSkills;

    [HideInInspector] public RuntimeAnimatorController entityAnimator;

}

#if UNITY_EDITOR
[CustomEditor(typeof(EntityData))]
public class EntityDataEditor : Editor
{
    private SerializedProperty avaliableSkills;
    protected EntityData entity;
    const int LABEL_WIDTH = 145;
    const int FIELD_WIDTH = 100;

    protected static bool ShowSkillList = false;

    private void OnEnable()
    {
        entity =  (EntityData)target;
        EditorUtility.SetDirty(target);
        avaliableSkills = serializedObject.FindProperty("avaliableSkills");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();




        // DRAW ENTITY TYPE SELECTOR
        DrawEntityName(entity);

        DrawEntitySelect(entity);

        // DrawEntityStats(entity);
       // EditorGUILayout.PropertyField(avaliableSkills);
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawEntityStats(EntityData entity)
    {
        // Entity Stats

        EditorGUILayout.LabelField("Stats:", GUILayout.Width(LABEL_WIDTH));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Health:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.health = EditorGUILayout.IntField(entity.stats.health, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();


        if (entity.entityType == EntityType.player)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Energy:", GUILayout.Width(LABEL_WIDTH));
            entity.stats.energy = EditorGUILayout.IntField(entity.stats.energy, GUILayout.Width(FIELD_WIDTH));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ammo:", GUILayout.Width(LABEL_WIDTH));
            entity.stats.ammo = EditorGUILayout.IntField(entity.stats.ammo, GUILayout.Width(FIELD_WIDTH));
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // ------------------------------------------------------------------------------------------

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Physical Attributes:", GUILayout.Width(LABEL_WIDTH));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Physical Damage:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.physicalDamage = EditorGUILayout.IntField(entity.stats.physicalDamage, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Physical Armor:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.physicalArmor = EditorGUILayout.IntField(entity.stats.physicalArmor, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // ------------------------------------------------------------------------------------------
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Magical Attributes:", GUILayout.Width(LABEL_WIDTH));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Magical Damage:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.magicDamage = EditorGUILayout.IntField(entity.stats.magicDamage, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Magical Armor:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.magicArmor = EditorGUILayout.IntField(entity.stats.magicArmor, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // ------------------------------------------------------------------------------------------
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Attributes:", GUILayout.Width(LABEL_WIDTH));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Defense bonus:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.defenseBonus = EditorGUILayout.FloatField(entity.stats.defenseBonus, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Speed:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.speed = EditorGUILayout.FloatField(entity.stats.speed, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Accuracy:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.accuracy = EditorGUILayout.IntField(entity.stats.accuracy, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Crit Rate:", GUILayout.Width(LABEL_WIDTH));
        entity.stats.critRate = EditorGUILayout.FloatField(entity.stats.critRate, GUILayout.Width(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();
    }

    private static void DrawEntityName(EntityData entity)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Entity Name:", GUILayout.Width(LABEL_WIDTH));
        entity.entityName = EditorGUILayout.TextField(entity.entityName, GUILayout.MaxWidth(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();
    }

    private static void DrawEntitySelect(EntityData entity)
    {
        EditorGUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Entity Type:", GUILayout.Width(LABEL_WIDTH));
        entity.entityType = (EntityType)EditorGUILayout.EnumPopup(entity.entityType, GUILayout.MaxWidth(FIELD_WIDTH));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Entity ID:",GUILayout.Width(LABEL_WIDTH));
        entity.entityID = EditorGUILayout.IntField(entity.entityID, GUILayout.Width(FIELD_WIDTH));

        EditorGUILayout.EndHorizontal();
        DrawEntityStats(entity);
        EditorGUILayout.Space(5);

        switch (entity.entityType)
        {
            case EntityType.player:

                break;
            case EntityType.enemy:
               
                EditorGUILayout.LabelField("REFERENCES:", GUILayout.Width(LABEL_WIDTH));
                ShowSkillList = EditorGUILayout.Foldout(ShowSkillList, "Skill List", true);

                if (ShowSkillList)
                {

                    EditorGUI.indentLevel++;
                    List<SkillEnemy> list = entity.avaliableSkills;
                    int size = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));

                    while (size > list.Count)
                    {
                        list.Add(null);
                    }

                    while (size < list.Count)
                    {
                        list.RemoveAt(list.Count - 1);
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = (SkillEnemy)EditorGUILayout.ObjectField("Skill " + i, list[i], typeof(SkillEnemy), false);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Enemy Animator:", GUILayout.Width(LABEL_WIDTH));
                entity.entityAnimator = (RuntimeAnimatorController)EditorGUILayout.ObjectField(entity.entityAnimator, typeof(RuntimeAnimatorController), false);

                break;
            case EntityType.boss:
                break;
        }
    }
}

#endif