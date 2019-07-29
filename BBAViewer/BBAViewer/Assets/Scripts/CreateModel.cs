using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct containing all the relevant Metadata of the asset to load. Is used to populate the UI panels and to add properties to the 3D model.
/// </summary>
public struct Metadata
{
    public float Scale;
    public string Material;
    public string Name;
    public string Description;
    /// <summary>
    /// Currently, the signature's field is read from the Metadata.txt text file.
    /// We wil use a DSA in the future (probably ECDSA).
    /// </summary>
    public string Signature;
    public List<KeyValuePair<string, string>> Properties;
    public List<string> Images;
}

/// <summary>
/// Provides methods to call ObjImporter to create the 3D Model of the asset while adding properties retrieved from Metadata.
/// </summary>
public class CreateModel : MonoBehaviour {
    /// <summary>
    /// Reads the given stream, pass data to ObjImporter, 
    /// and apply properties recovered from Metadata
    /// </summary>
    /// <param name="streamName"></param>
    /// <param name="s">A MemoryStream to read the .obj file from</param>
    /// <param name="x">The x coordinate to load the asset from</param>
    /// <param name="y">The y coordinate to load the asset from</param>
    /// <param name="metadata">A Metadata struct to add properties to the 3D Model</param>
    public static void CreateModelFromOBJ(string streamName, System.IO.MemoryStream s, int x, int y, Metadata metadata)
    {
        if (GameObject.Find("newObjFromZip") != null)
            Destroy(GameObject.Find("newObjFromZip"));

        var yourGameObject = new GameObject("newObjFromZip");
        yourGameObject.layer = 8; // The layer is hidden from the UI camera

        Mesh holderMesh = new Mesh();
        ObjImporter newMesh = new ObjImporter();

        holderMesh = newMesh.ImportStream(streamName, s);

        MeshRenderer renderer = yourGameObject.AddComponent<MeshRenderer>();
        ObjectsRotation o_rotation = yourGameObject.AddComponent<ObjectsRotation>();

        Material yourMaterial = (Material)Resources.Load(metadata.Material, typeof(Material));

        renderer.sharedMaterial = yourMaterial;

        MeshFilter filter = yourGameObject.AddComponent<MeshFilter>();
        filter.mesh = holderMesh;
        filter.mesh.RecalculateNormals();
        filter.mesh.RecalculateBounds();

        MeshCollider collider = yourGameObject.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = holderMesh;


		// The next two lines add collision and gravity. Unwanted in our case.
		
        //Rigidbody gameObjectsRigidBody = yourGameObject.AddComponent<Rigidbody>(); 
        //gameObjectsRigidBody.mass = 0;  

        yourGameObject.transform.localScale = new Vector3(metadata.Scale * 20f, metadata.Scale * 20f, metadata.Scale * 20f);

        yourGameObject.transform.Translate(-filter.mesh.bounds.center);

    }
}
