using Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Player Skill",menuName ="Skill/PlayerSkill")]
public class PlayerSkill : Skill
{

    // [Header("Player Skill upgrades:"),Space(10)]
    [HideInInspector] public bool canBeUpgraded = true;
    [HideInInspector] public bool unlocked;
    [HideInInspector] [Min(0)] public int level = 0;
    [HideInInspector] public int initialUnlockCost;
    private int baseXPCost = 200;
    
    [HideInInspector] public float damageUpgradeAmount;

    [HideInInspector] public string skillName; // Use it for UI.
    [HideInInspector] [TextArea] public string skillDescription;

   // [HideInInspector] public Sprite smallIcon;
    [HideInInspector] public Sprite icon;

    //[Header("Targeting and resources:"), Space(10)]
    [HideInInspector] public TargetingStyle targetingStyle;

    [HideInInspector] public ResourceType resourceType;
    [HideInInspector] public int resourceAmount;
    private float initialDamage;
    private int initialevel;
    private bool wasUnlocked;
    public void Initialize()
    {
        initialDamage = baseDamage;
        //level = 0;
        initialevel = level;
        wasUnlocked = unlocked;
        //unlocked = false;
    }

    public void RestoreToDefault()
    {
        baseDamage = initialDamage;
        level = initialevel;
        unlocked = wasUnlocked;
    }


    public void ResetToFactory()
    {
        initialDamage = baseDamage;
        level = 0;
        unlocked = false;
    }

    public void UpdgradeSkill()
    {
        level++;
        baseDamage += damageUpgradeAmount;
    }

    public int RequiredXp()
    {
        switch (level)
        {
            case -1:
                return initialUnlockCost;
            default:
                return initialUnlockCost + (baseXPCost * level);
        }
    }

    public void ResetToLevel1()
    {
        if (unlocked == false) return;
        level = 1;

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerSkill)), CanEditMultipleObjects]
    public class PlayerSkillEditor : Editor
    {
        private PlayerSkill m_PlayerSkill;

        protected float damage;

        protected static bool ShowOffUnlockSettings = true;
        protected static bool ShowOffTargetingSettings = true;
        protected static bool ShowOffUIReferences = true;
        const int WIDTH = 145;
        PlayerSkill playerSkill;
        private SerializedProperty canBeUnlocked;

        private void OnEnable()
        {
            m_PlayerSkill = (PlayerSkill)target;
            previewSprite = serializedObject.FindProperty("icon");
            playerSkill = (PlayerSkill)target;
            EditorUtility.SetDirty(playerSkill);
    
        }

        protected SerializedProperty previewSprite;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //m_PlayerSkill.Update();
            //canBeUnlocked = m_PlayerSkill.FindProperty("canBeUpgraded").boolValue;

            EditorGUILayout.BeginVertical();
            //  EditorGUILayout.PropertyField(largeIconProperty);
            Texture2D texture = AssetPreview.GetAssetPreview(playerSkill.icon);
            if (texture != null)
            {
                texture.filterMode = FilterMode.Point;
                GUILayout.Label("", GUILayout.Height(125), GUILayout.Width(125));
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
            }


            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Can be upgraded", GUILayout.Width(WIDTH));
            //canBeUnlocked = EditorGUILayout.Toggle(skill.canBeUpgraded, GUILayout.Width(WIDTH));
            playerSkill.canBeUpgraded = EditorGUILayout.Toggle(playerSkill.canBeUpgraded, GUILayout.Width(WIDTH));
            EditorGUILayout.EndHorizontal();
            if (playerSkill.canBeUpgraded == true)
            {
                DrawUpgradeGUI(playerSkill, WIDTH);
            }
            serializedObject.ApplyModifiedProperties();
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
                skill.skillDescription = EditorGUILayout.TextArea(skill.skillDescription, GUILayout.MaxHeight(50));

                EditorGUILayout.Space(5);


                EditorGUI.indentLevel--;
            }
        }
    }

#endif
}
