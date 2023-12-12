using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Structs;
[SelectionBase]
public class DummySelectable : MonoBehaviour
{
    public ObjectType objectType;
    private int tileIndex = -1;
    [HideInInspector] public int currentPreview = -1;
    [HideInInspector] public List<GameObject> visualVariations = new List<GameObject>();
    private GameObject preview;
    private GameObject blockPlaceholder;
    
    public void FakeAwake()
    {
        blockPlaceholder = gameObject.transform.GetChild(0).gameObject;
    }

    #region TILES
    public void UpdateVisuals(int index)
    {
        if (visualVariations.Count == 0) return;
        tileIndex = index;

        if (tileIndex == -1)
        {
            int r = Random.Range(0, visualVariations.Count);
            visualVariations[r].gameObject.SetActive(true);
        }
        else visualVariations[tileIndex].gameObject.SetActive(true);
    }

    public void SetToDefault()
    {
        if (blockPlaceholder) blockPlaceholder.SetActive(true);
        if (preview) DestroyImmediate(preview);
        currentPreview = -1;
        tileIndex = -1;
    }

    public void PreviewVisual(int id, Material material)
    {
        if (preview) DestroyImmediate(preview);
        if (blockPlaceholder.activeSelf) blockPlaceholder.SetActive(false);

        currentPreview = id; // Store if its random or not

        tileIndex = id;
        preview = (GameObject)PrefabUtility.InstantiatePrefab(GetPreview(currentPreview));
        preview.transform.parent = transform;
        preview.transform.position = transform.position;
        preview.GetComponentInChildren<Renderer>().material = material;

    }

    public GameObject GetFinalVersion()
    {
        tileIndex = currentPreview;
        if (tileIndex == -1) tileIndex = Random.Range(0, visualVariations.Count);
        if (visualVariations.Count == 0)
        {
            Debug.LogWarning("No visuals added!");
            return null;
        }
        return visualVariations[tileIndex];
    }

    private GameObject GetPreview(int id)
    {

        if (id == -1) id = Random.Range(0, visualVariations.Count);
        if (visualVariations.Count == 0)
        {
            Debug.LogWarning("No visuals added!");
            return null;
        }

        return visualVariations[id].gameObject;
    }

    #endregion

}

#if UNITY_EDITOR
[CustomEditor(typeof(DummySelectable))]
public class DummyEditor : Editor
{
    protected SerializedProperty m_serializedProperty;

    private void OnEnable()
    {
        m_serializedProperty = serializedObject.FindProperty("visualVariations");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_serializedProperty, true);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif 
