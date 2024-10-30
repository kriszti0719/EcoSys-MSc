using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
	//We're going to want a reference to the renderer of that plane, so we can just set its texture
	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	//public MeshCollider meshCollider;

	public void DrawTexture(Texture2D texture)
	{
		//Now we'll want to apply the texture to the texture renderer
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData, Texture2D texture)
	{
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;
		//meshCollider.sharedMesh = meshData.CreateMesh();
	}

}