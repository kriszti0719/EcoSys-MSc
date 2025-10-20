using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
	//Create texture out of a 1D colorMap
	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		//No more blurriness
		//.Point instead of .Bilinear
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colourMap);
		texture.Apply();
		return texture;
	}

	//Getting a texture based on a 2D height map
	public static Texture2D TextureFromHeightMap(float[,] heightMap)
	{
		int width = heightMap.GetLength(0);			//0 - for the first dimension
			int height = heightMap.GetLength(1);	//1 - for the second dimension

			//We're goind te set the color of each of the pixels in this texture
			//ColourMap is a 1D array whereas our noisemap 2D
			Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
			}
		}

		return TextureFromColourMap(colourMap, width, height);
	}

}
