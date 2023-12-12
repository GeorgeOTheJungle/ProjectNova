using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LevelGenerator : MonoBehaviour
{

    
    public List<MapObjectScriptable> colorMappings = new List<MapObjectScriptable>();
    [Header("Parents"),Space(10)]

    [HideInInspector] public Transform grounds;
    [HideInInspector] public Transform encounters;
    [HideInInspector] public Transform walls;
    [HideInInspector] public Transform interactables;

    [HideInInspector] public Texture2D map;
    [HideInInspector] public string stageName;
    [HideInInspector] public Material stageMaterial;

    // Mesh Generation
    [HideInInspector] public List<GameObject> spawnedObjects = new List<GameObject>();
    [HideInInspector] public List<DummySelectable> toBakeMeshObjects = new List<DummySelectable>();
    [HideInInspector] public List<GameObject> bakedMeshObjects = new List<GameObject>();

    // Encounters lists
    [HideInInspector] public List<GameObject> encounterObjects = new List<GameObject>();

    [HideInInspector] public List<DummySelectable> previousSelectedObject = new List<DummySelectable>();

    [HideInInspector] public GameObject stageMesh;
    private const string GROUND_OBJECT_NAME = "GROUNDS";
    private const string WALL_OBJECT_NAME = "WALLS";
    private const string ENCOUNTER_OBJECT_NAME = "ENCOUNTERS";
    private const string INTERACTABLE_OBJECT_NAME = "DOORS";

    #region Map Generation Methods

    public void Initialize()
    {
        MapObjectScriptable groundMapping = Resources.Load<MapObjectScriptable>("LevelEditor/Mappings/GroundTile");
 
        MapObjectScriptable wallMapping = Resources.Load<MapObjectScriptable>("LevelEditor/Mappings/WallTile");

        colorMappings.Add(groundMapping);
        colorMappings.Add(wallMapping);
    }
    public void GenerateLevel()
    {

        grounds.gameObject.SetActive(true);
        walls.gameObject.SetActive(true);
        CleanPreviewLevel();
        CleanGeneratedMesh();

        if (colorMappings.Count == 0)
        {
            Debug.LogWarning("No MapObjectScriptable on the array, did you forgot to add it?");
            return;
        }
        for (int x = 0; x < map.width; x++)
        {
            for (int z = 0; z < map.height; z++)
            {
                GenerateTile(x, z);
            }
        }
    }
    private void GenerateTile(int x, int z)
    {
        Color pixelColor = map.GetPixel(x, z);
        if (pixelColor.a == 0) return; // Pixel is transparent.

        MapObjectScriptable ground = null;

        foreach(var mapping in colorMappings)
        {
            if(mapping.objectType == ObjectType.ground)
            {
                ground = mapping;
                break;
            }
        }
        foreach (var colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                int y = colorMapping.groundLevel ? 0 : 1;
                Vector3 position = new Vector3(x, y, z);
                Transform parent = GetParent(colorMapping.objectType);
                if (colorMapping.objectType == ObjectType.encounter) parent = encounters;
                
                GameObject tile = (GameObject) PrefabUtility.InstantiatePrefab(colorMapping.prefab);
                tile.transform.position = position;
                tile.transform.SetParent(parent);
                
                if(tile.TryGetComponent(out DummySelectable t))
                {
                    t.FakeAwake();
                }
                if(colorMapping.objectType == ObjectType.wall ||  colorMapping.objectType == ObjectType.ground) 
                    toBakeMeshObjects.Add(tile.GetComponent<DummySelectable>());
                if (colorMapping.objectType != ObjectType.ground && colorMapping.objectType != ObjectType.wall) // Check if the object is floating, then add a ground down
                {
                    Vector3 extraPosition = new Vector3(x, 0, z);
                    GameObject extraGroundTile = (GameObject)PrefabUtility.InstantiatePrefab(ground.prefab);
                    extraGroundTile.transform.position = extraPosition;
                    extraGroundTile.transform.SetParent(grounds);
                    spawnedObjects.Add(extraGroundTile);
                    toBakeMeshObjects.Add(extraGroundTile.GetComponent<DummySelectable>());
                    toBakeMeshObjects.Add(tile.GetComponent<DummySelectable>());
                }
                else
                {
                    spawnedObjects.Add(tile);
                }
     
            }
        }
    }
    public void BakeObjects()
    {
        if(toBakeMeshObjects.Count == 0)
        {
            Debug.LogWarning("Nothing to bake!");
            return;
        }
        CleanGeneratedMesh();
        List<MeshFilter> ovenMeshFilters = new List<MeshFilter>();
        foreach (var dummy in toBakeMeshObjects)
        {

            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(dummy.GetFinalVersion());
            prefab.transform.position = dummy.transform.position;
            ovenMeshFilters.Add(prefab.GetComponentInChildren<MeshFilter>());
            prefab.transform.SetParent(dummy.objectType == ObjectType.ground ? grounds : walls);
            bakedMeshObjects.Add(prefab);
            prefab.SetActive(false);
        }

        CombineMeshes(ovenMeshFilters);
    }
    public void ShowGeneratedTiles()
    {
        grounds.gameObject.SetActive(true);
        walls.gameObject.SetActive(true);
        if (stageMesh) stageMesh.SetActive(false);
    }
    public void HideGeneratedTiles()
    {
        grounds.gameObject.SetActive(false);
        walls.gameObject.SetActive(false);
        if (stageMesh) stageMesh.SetActive(true);
    }
    public void FinalizeLevel()
    {
        DestroyImmediate(grounds.gameObject);
        DestroyImmediate(walls.gameObject);

        spawnedObjects.Clear();
        toBakeMeshObjects.Clear();
        bakedMeshObjects.Clear();

        DestroyImmediate(this);
    }
    public void CreateEncounterAt(GameObject dummySelected)
    {
        string path = "EncounterObject";
        Vector3 prefabPosition = dummySelected.transform.position;
        prefabPosition.y += 1;
        GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load(path));
        prefab.transform.position = prefabPosition;
        //prefab.GetComponent<EncounterHandler>().SetEncounter(dummy.GetCurrentData());
        prefab.transform.SetParent(encounters);
        encounterObjects.Add(prefab);
        //spawnedObjects.Add(prefab);
    }
    private void CombineMeshes(List<MeshFilter> bakeMeshFilters)
    {
        GameObject levelMesh = new GameObject($"{stageName}_Stage");
        levelMesh.layer = 3;
        levelMesh.transform.SetParent(transform);
        var target = levelMesh.AddComponent<MeshFilter>();
        levelMesh.AddComponent<MeshRenderer>().material = stageMaterial;
        MeshCollider collider = levelMesh.AddComponent<MeshCollider>();
 
        var combine = new CombineInstance[bakeMeshFilters.Count];

        for(int i =0;i < bakeMeshFilters.Count; i++)
        {
            combine[i].mesh = bakeMeshFilters[i].sharedMesh;
            combine[i].transform = bakeMeshFilters[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.name = $"{stageName}_mesh";
        mesh.CombineMeshes(combine);
        target.mesh = mesh;
        stageMesh = target.gameObject;
        grounds.gameObject.SetActive(false);
        walls.gameObject.SetActive(false);
        collider.sharedMesh = mesh;

        CleanBakedMeshObjects();
        Debug.Log("Done! :)");
    }


    #endregion


    #region Cleaning Methods
    public void CleanPreviewLevel()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            DestroyImmediate(obj);
        }

        spawnedObjects.Clear();
        toBakeMeshObjects.Clear();
        previousSelectedObject.Clear();
        if(stageMesh) DestroyImmediate(stageMesh);
    }
    public void CleanBakedMeshObjects()
    {
        if(bakedMeshObjects.Count == 0)
        {
            Debug.LogWarning("No baked objects to clean!");
            return;
        }
        foreach (GameObject obj in bakedMeshObjects)
        {
            DestroyImmediate(obj);
        }
        bakedMeshObjects.Clear();
    }
    public void RemoveEncounter(GameObject selected)
    {
        if (encounterObjects.Contains(selected) == false)
        {
            Debug.LogWarning("Object not found");
            return;
        }
        DestroyImmediate(selected);
        encounterObjects.Remove(selected);
    }
    public void CleanEncounters()
    {
        foreach(var obj in encounterObjects)
        {
            DestroyImmediate(obj);
        }

        encounterObjects.Clear();
    }
    public void CleanAllObjects()
    {
        CleanPreviewLevel();
        CleanBakedMeshObjects();
        CleanEncounters();
        grounds.gameObject.SetActive(true);
        walls.gameObject.SetActive(true);
        DestroyImmediate(stageMesh);
    }
    private void CleanGeneratedMesh()
    {
        if (stageMesh == null) return;
        DestroyImmediate(stageMesh);

    }
    #endregion

    #region Get Methods
    private Transform GetParent(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.encounter: return encounters;
            case ObjectType.wall: return walls;
            case ObjectType.ground: return grounds;
            case ObjectType.interactable: return interactables;
            default: return transform;
        }
    }

    public bool HasParentSetup()
    {
        if (grounds && walls && interactables && encounters) return true;
        else return false;
    }

    public bool HasValidMappings()
    {
        foreach (var mapping in colorMappings)
        {
            if (mapping == null) return false;
        }
        return true;
    }

    private Transform CreateParent(string name)
    {
        GameObject obj = new GameObject();
        PrefabUtility.InstantiatePrefab(obj);
        obj.transform.SetParent(transform);
        obj.name = name;
        return obj.transform;
    }
    #endregion

    #region Other Methods
    public void AddToList(DummySelectable tile) => previousSelectedObject.Add(tile);

    public void ResetTiles()
    {
        foreach (var tile in previousSelectedObject)
        {
            tile.SetToDefault();
        }

        previousSelectedObject.Clear();
    }

    public void SetUpParents()
    {
        if (grounds == null) grounds = CreateParent(GROUND_OBJECT_NAME);

        if (walls == null) walls = CreateParent(WALL_OBJECT_NAME);

        if (encounters == null) encounters = CreateParent(ENCOUNTER_OBJECT_NAME);

        if (interactables == null) interactables = CreateParent(INTERACTABLE_OBJECT_NAME);

        Initialize();
    }
    #endregion
}

