
#if UNITY_EDITOR
using System.Diagnostics.PerformanceData;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    protected LevelGenerator levelGenerator;

    protected const int WINDOW_WIDHT = 350;
    protected const int WINDOW_HEIGHT = 500;

    protected const int LABEL_WIDTH = 150;

    protected const int BUTTON_HEIGHT = 25;
    protected const int BUTTON_WIDHT = 100;

    protected const int FIELD_WIDHT = 5;

    private bool hasLevelMap;
    private bool isSelectingTiles = false;
    private ObjectType objectType;
    private int tileID = 0;
    private int selectedTileID;
    private DummySelectable currentDummy;
    private static EditorWindow window;

    protected Vector2 labelPosition;
    protected Vector2 labelWidth;
    private Vector2 scrollPosition = Vector2.zero;

    protected Rect labelRect;

    protected SerializedObject m_serializedLevelGenerator;
    protected SerializedProperty m_stageNameProperty;
    protected SerializedProperty m_stageMapProperty;
    protected SerializedProperty m_stageMaterialProperty;

    // protected SerializedProperty m_

    protected LevelGenerator lastGenerator;
    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        window = GetWindow<LevelEditor>("Level Editor");
        //window.maxSize = new Vector2(WINDOW_WIDHT, );
        //window.minSize = window.maxSize;
        window.autoRepaintOnSceneChange = true;

    }


    private void OnEnable()
    {
        levelGenerator = GameObject.FindAnyObjectByType<LevelGenerator>();
        EditorUtility.SetDirty(levelGenerator);
        if (levelGenerator == null) return;

        m_serializedLevelGenerator = new SerializedObject(levelGenerator);
        m_stageNameProperty = m_serializedLevelGenerator.FindProperty("stageName");
        m_stageMapProperty = m_serializedLevelGenerator.FindProperty("map");
        m_stageMaterialProperty = m_serializedLevelGenerator.FindProperty("stageMaterial");
        //mappingsProperty = serializedLevelGenerator.FindProperty("colorMappings");
        //EditorUtility.SetDirty(levelGenerator);

    }

    private void OnFocus()
    {
        levelGenerator = GameObject.FindAnyObjectByType<LevelGenerator>();
        EditorUtility.SetDirty(levelGenerator);
        if (m_serializedLevelGenerator == null) m_serializedLevelGenerator = new SerializedObject(levelGenerator);
    }

    private void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            if (levelGenerator) lastGenerator = levelGenerator;
            return;
        }
        else
        {
            if (lastGenerator) levelGenerator = lastGenerator;
        }

        if (levelGenerator == null)
        {
            GUILayout.Label("No level editor found", EditorStyles.helpBox);
            if (GUILayout.Button("Set up level editor"))
            {
                GameObject tmpObj = new GameObject("Level Generator");
                levelGenerator = tmpObj.AddComponent<LevelGenerator>();
                levelGenerator.SetUpParents();
            }
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height), GUILayout.Width(position.width));
        if (m_serializedLevelGenerator != null) m_serializedLevelGenerator.Update();

        DrawCenteredLabel($"Stage: {levelGenerator.name}");
        DrawMainWindow();
        DrawLine();

        EditorGUILayout.EndScrollView();
        if (m_serializedLevelGenerator != null) m_serializedLevelGenerator.ApplyModifiedProperties();
    }

    private static void DrawLine()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
    }


    public void OnSelectionChange()
    {
        if (levelGenerator == null) return;
        if (Selection.activeGameObject != null)
        {
            DummySelectable obj = Selection.activeGameObject.TryGetComponent(out DummySelectable component) ? component : null;
            if (obj != null)
            {
                objectType = obj.objectType;

                isSelectingTiles = true;
                selectedTileID = obj.currentPreview;
                currentDummy = obj;

            }
            else
            {
                isSelectingTiles = false;
                currentDummy = null;
                objectType = ObjectType.none;

            }
        }
        else
        {
            isSelectingTiles = false;
            currentDummy = null;
            objectType = ObjectType.none;
        }

    }

    private void DrawMainWindow()
    {
        if (levelGenerator == null) return;

        // Level map

        EditorGUILayout.BeginHorizontal();
        try
        {
            EditorGUILayout.LabelField("Stage map:", GUILayout.Width(LABEL_WIDTH - 50));
            EditorGUILayout.PropertyField(m_stageMapProperty, GUIContent.none);
            // levelGenerator.map = EditorGUILayout.ObjectField(levelGenerator.map, typeof(Texture2D), false) as Texture2D;
        }
        finally
        {
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.PropertyField(m_stageNameProperty);
        EditorGUILayout.PropertyField(m_stageMaterialProperty);

        DrawMaterialPreview();
        DrawLine();

        hasLevelMap = levelGenerator.map != null;

        if (IsReadyToGenerate() == false) return;
        EditorGUILayout.Space(10);
        DrawLevelGenerationButtons();
    }

    private bool IsReadyToGenerate()
    {
        if (hasLevelMap == false) return false;
        if (levelGenerator.HasParentSetup() == false) return false;
        if (levelGenerator.HasValidMappings() == false) return false;

        return true;
    }

    private void DrawMaterialPreview()
    {

        if (levelGenerator.stageMaterial == null) return;

        Texture2D stageTexture = AssetPreview.GetAssetPreview(levelGenerator.stageMaterial.mainTexture);
        GUILayout.Label("", GUILayout.Height(100), GUILayout.Width(100));

        if (stageTexture != null)
        {
            stageTexture.filterMode = FilterMode.Point;
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), stageTexture);
        }
    }
    private static void DrawCenteredLabel(string text)
    {
        using (var l = new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{text}", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
        }
    }

    private void DrawLevelGenerationButtons()
    {
        if (GUILayout.Button("Generate Level"))
        {
            levelGenerator.GenerateLevel();
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Show Generated Tiles"))
        {
            levelGenerator.ShowGeneratedTiles();
        }

        if (GUILayout.Button("Hide Generated Tiles"))
        {
            levelGenerator.HideGeneratedTiles();
        }

        EditorGUILayout.EndHorizontal();

        DrawLine();

        DrawTileEditor();

        DrawLine();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Selected Encounter"))
        {
            levelGenerator.RemoveEncounter(Selection.activeGameObject);
        }

        if (GUILayout.Button("Clear Encounters"))
        {
            levelGenerator.CleanEncounters();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear Generated Level"))
        {
            levelGenerator.CleanPreviewLevel();
        }

        if (GUILayout.Button("Clear ALL objects"))
        {
            levelGenerator.CleanAllObjects();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Bake level"))
        {
            levelGenerator.BakeObjects();
        }
        if (levelGenerator.HasBakedObjects() == false) return;

        if (GUILayout.Button("Clear Baked objects"))
        {
            levelGenerator.CleanBakedMeshObjects();
        }
    }

    private void DrawTileEditor()
    {
        if (levelGenerator == null) return;

        if (isSelectingTiles)
        {
            DrawCenteredLabel("TILE EDITOR");
            if (currentDummy != null) EditorGUILayout.LabelField($"Tile selected: {currentDummy.transform.name}");
            EditorGUILayout.LabelField($"Object Type: {objectType}");


            tileID = EditorGUILayout.IntSlider("Tile ID", tileID, 0, currentDummy.visualVariations.Count - 1);

            EditorGUILayout.LabelField($"Current Tile ID: {selectedTileID}");
            string title = Selection.gameObjects.Length > 1 ? "Tiles" : "Tile";


            if (GUILayout.Button($"Set {title} To ID"))
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    DummySelectable objSelected = obj.GetComponent<DummySelectable>();
                    if (objSelected != null)
                    {
                        objSelected.PreviewVisual(tileID, levelGenerator.stageMaterial);
                        levelGenerator.AddToList(objSelected);
                        selectedTileID = objSelected.currentPreview;
                    }

                }
            }
            string buttonName = $"Reset Selected {title}";
            if (GUILayout.Button(buttonName))
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    DummySelectable objSelected = obj.GetComponent<DummySelectable>();
                    if (objSelected != null)
                    {
                        objSelected.SetToDefault();
                    }
                }
            }

            if (GUILayout.Button("Reset ALL"))
            {
                levelGenerator.ResetTiles();
            }

            if (GUILayout.Button("Create Encounter"))
            {
                levelGenerator.CreateEncounterAt(currentDummy.gameObject);
            }
        }
        else
        {
            GUILayout.Label("Select a dummy to edit", EditorStyles.helpBox);
        }
    }
}

#endif
