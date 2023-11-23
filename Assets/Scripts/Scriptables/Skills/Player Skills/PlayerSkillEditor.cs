using Enums;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


#if UNITY_EDITOR
[CustomEditor(typeof(PlayerSkill))]
public class PlayerSkillEditor : Editor
{

    private SerializedProperty smallIconProperty;
    private SerializedProperty largeIconProperty;

    protected static bool ShowOffUnlockSettings = true;
    protected static bool ShowOffTargetingSettings = true;
    protected static bool ShowOffUIReferences = true;

    private void OnEnable()
    {
        smallIconProperty = serializedObject.FindProperty("smallIcon");
        largeIconProperty = serializedObject.FindProperty("largeIcon");
        
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        var skill = (PlayerSkill)target;
        const int WIDTH = 145;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Can be upgraded",GUILayout.Width(WIDTH));
        skill.canBeUpgraded = EditorGUILayout.Toggle(skill.canBeUpgraded, GUILayout.Width(WIDTH));
        EditorGUILayout.EndHorizontal();
        if (skill.canBeUpgraded)
        {
            DrawUpgradeGUI(skill, WIDTH);
        }

    }

    private void DrawUpgradeGUI(PlayerSkill skill, int WIDTH)
    {
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Player Skill Upgrade settings:");

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // Unlocking and upgrades
        ShowOffUnlockSettings = EditorGUILayout.Foldout(ShowOffUnlockSettings, "Unlock/Upgrade Settings: ", true);

        if (ShowOffUnlockSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Unlocked:", GUILayout.MaxWidth(WIDTH));
            skill.unlocked = EditorGUILayout.Toggle(skill.unlocked);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            if (skill.unlocked)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level:", GUILayout.MaxWidth(WIDTH));
                skill.level = EditorGUILayout.IntField(skill.level, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Damage Per Upgrade:", GUILayout.MaxWidth(WIDTH));
                skill.damageUpgradeAmount = EditorGUILayout.FloatField(skill.damageUpgradeAmount, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Unlock Cost:", GUILayout.MaxWidth(WIDTH));
                skill.initialUnlockCost = EditorGUILayout.IntField(skill.initialUnlockCost, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);

        ShowOffTargetingSettings = EditorGUILayout.Foldout(ShowOffTargetingSettings, "Targeting and Resources Settings:", true);
        if (ShowOffTargetingSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Targeting Style:", GUILayout.MaxWidth(WIDTH));
            skill.targetingStyle = (TargetingStyle)EditorGUILayout.EnumPopup(skill.targetingStyle, GUILayout.MaxWidth(150));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Resource:", GUILayout.MaxWidth(WIDTH));
            skill.resourceType = (ResourceType)EditorGUILayout.EnumPopup(skill.resourceType, GUILayout.MaxWidth(150));
            EditorGUILayout.EndHorizontal();

            if (skill.resourceType != Enums.ResourceType.none)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Resource Cost:", GUILayout.MaxWidth(WIDTH));
                skill.resourceAmount = EditorGUILayout.IntField(skill.resourceAmount, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        // TARGETING AND RESOURCES

        EditorGUILayout.Space(10);

        // UI REFERENCES

        ShowOffUIReferences = EditorGUILayout.Foldout(ShowOffUIReferences, "UI References: ", true);
        if (ShowOffUIReferences)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Skill Name:", GUILayout.MaxWidth(WIDTH));
            skill.skillName = EditorGUILayout.TextField(skill.skillName, GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Skill Description:", GUILayout.MaxWidth(WIDTH));
            EditorGUILayout.TextArea(skill.skillDescription, GUILayout.MaxHeight(50));

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(smallIconProperty);
            EditorGUILayout.PropertyField(largeIconProperty);
            Texture2D texture = AssetPreview.GetAssetPreview(skill.largeIcon);
            if (texture != null)
            {
                GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif

