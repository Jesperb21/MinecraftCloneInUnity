using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameController : MonoBehaviour {

    public GameObject cube;
    private GameObject[] cubes;
    //public string path = @"Textures";
    Texture text;
    Texture[] Atextures;
    private List<Texture> Ltextures;
    Object[] files;

    //private Texture randomTexture;
	// Use this for initialization
	void Start () {
      // files = Resources.LoadAll("Materials", typeof(Material));
       // text = (Texture)UnityEditor.AssetDatabase.LoadAssetAtPath("Textures/bottom", typeof(Texture));

        //Denne virker fint, til at finde den specifikke texture "bottom".
       // text = Resources.Load("Textures/bottom", typeof(Texture)) as Texture;


        //Dette virker helt fint
        files = Resources.LoadAll("Textures", typeof(Texture));
        Atextures = new Texture[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            Atextures[i] = files[i] as Texture;
        }
        Debug.Log(Atextures.Length);
        

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Instantiate(this, new Vector3(0, 1, 0), Quaternion.identity);

            Instantiate(cube, new Vector3((Random.Range(-5, 5)), (Random.Range(-5, 5)), (Random.Range(-5, 5))), Quaternion.identity);
           
            
            GameObject child = cube.transform.Find("Top").gameObject;

           /* foreach (Object file in files)
            {
                Texture t = (Texture)file;
                Debug.Log("t :" + t.name);
                textures.Add(t);
            }
            Texture[] At = textures.ToArray();*/
           // Material[] m = (Material[])Resources.LoadAll("Materials", typeof(Material));

           // Material tAdd = m[Random.Range(0, m.Length)];



            //Texture[] textures = (Texture[])UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
            //Object[] textures = Resources.LoadAll(path,typeof(Texture));
           
           // Debug.Log("index: " + textures.Length);
            //Texture texture = (Texture)textures[Random.Range(0, textures.Length-1)];

            //Texture[] textures = (Texture[])Resources.LoadAll(path);
           // textures.Add(Resources.LoadAll(path));
            //Texture texture = texturesA[Random.Range(0, texturesA.Length)];
           // Debug.Log("Texture : " + texture.name);
            text = Atextures[Random.Range(0, Atextures.Length)];
            child.GetComponent<Renderer>().material.mainTexture = text;
           // child.GetComponent<Renderer>().material.mainTexture = randomTexture;

        }
	}
}
