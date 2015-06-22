using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;


public class GameController : MonoBehaviour {

    public GameObject cube;
    private GameObject[] cubes;

    Object[] files;

    public TextAsset xmlBlocks;

    private Texture Top;
    private Texture Side;
    private Texture Bottom;

    private string bottomPath = string.Empty;
    private string topPath = string.Empty;
    private string sidePath = string.Empty;

    public GameObject MakeCube(string tag)
    {
        GameObject NewCube;
        Transform[] allchilden;

        NewCube = (GameObject)Instantiate(cube, new Vector3((Random.Range(-5, 5)), (Random.Range(-5, 5)), (Random.Range(-5, 5))), Quaternion.identity);
        NewCube.gameObject.tag = tag;
        allchilden = NewCube.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in allchilden)
        {
            child.gameObject.tag = tag;
        }
        NewCube.gameObject.tag = tag;

        GetTextures(NewCube.tag);

        if (bottomPath != string.Empty && topPath != string.Empty && sidePath != string.Empty)
        {
            GameObject childBottom = NewCube.transform.Find("Bottom").gameObject;
            childBottom.GetComponent<Renderer>().material.mainTexture = Bottom;
            GameObject childTop = NewCube.transform.Find("Top").gameObject;
            childTop.GetComponent<Renderer>().material.mainTexture = Top;
            GameObject childFront = NewCube.transform.Find("Front").gameObject;
            childFront.GetComponent<Renderer>().material.mainTexture = Side;
            GameObject childBack = NewCube.transform.Find("Back").gameObject;
            childBack.GetComponent<Renderer>().material.mainTexture = Side;
            GameObject childRight = NewCube.transform.Find("Right").gameObject;
            childRight.GetComponent<Renderer>().material.mainTexture = Side;
            GameObject childLeft = NewCube.transform.Find("Left").gameObject;
            childLeft.GetComponent<Renderer>().material.mainTexture = Side;
        }
        else if (bottomPath != string.Empty && topPath == string.Empty && sidePath == string.Empty)
        {
            GameObject childBottom = NewCube.transform.Find("Bottom").gameObject;
            childBottom.GetComponent<Renderer>().material.mainTexture = Bottom;
            GameObject childTop = NewCube.transform.Find("Top").gameObject;
            childTop.GetComponent<Renderer>().material.mainTexture = Bottom;
            GameObject childFront = NewCube.transform.Find("Front").gameObject;
            childFront.GetComponent<Renderer>().material.mainTexture = Bottom;
            GameObject childBack = NewCube.transform.Find("Back").gameObject;
            childBack.GetComponent<Renderer>().material.mainTexture = Bottom;
            GameObject childRight = NewCube.transform.Find("Right").gameObject;
            childRight.GetComponent<Renderer>().material.mainTexture = Bottom;
            GameObject childLeft = NewCube.transform.Find("Left").gameObject;
            childLeft.GetComponent<Renderer>().material.mainTexture = Bottom;
           /* Debug.Log("venter på children");
            foreach (Transform child in allchilden)
            {
                child.GetComponent<Renderer>().material.mainTexture = Bottom;
            }*/
        }
        return NewCube;
    }

    void GetTextures(string cubeTag)
    {
        //TextAsset asset = (TextAsset)Resources.Load("XmlBlocks");
        // string datafilecontent = asset.text;

       // Debug.Log("myXML: " + xmlBlocks.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlBlocks.text);

        foreach (XmlElement child in xmlDoc.SelectNodes("blocks/block"))
        {
            if (child.SelectSingleNode("name").InnerXml == cubeTag)
            {
                foreach (XmlElement item in child.SelectNodes("Textures"))
                {
                    if (item.SelectSingleNode("bottom") != null)
                    {
                        bottomPath = item.SelectSingleNode("bottom").InnerText;
                        Bottom = Resources.Load(bottomPath, typeof(Texture)) as Texture;
                    }
                  //  Debug.Log("bottom: " + bottomPath);
                    if (item.SelectSingleNode("top") != null)
                    {
                        topPath = item.SelectSingleNode("top").InnerText;
                        Top = Resources.Load(topPath, typeof(Texture)) as Texture;
                    }
                  //  Debug.Log("topPath: " + topPath);
                    if (item.SelectSingleNode("side") != null)
                    {
                        sidePath = item.SelectSingleNode("side").InnerText;
                        Side = Resources.Load(sidePath, typeof(Texture)) as Texture;
                    }
                  //  Debug.Log("sidePath: " + sidePath);
                }
            }
        }
    }

	void Start () {

	}

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeCube("stone");         
        }
	}
}
