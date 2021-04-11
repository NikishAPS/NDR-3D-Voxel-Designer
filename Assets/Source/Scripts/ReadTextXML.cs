using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

public class ReadTextXML : MonoBehaviour
{

    [SerializeField]
    private string Patch = @"Resources\XML\var.xml";

    public int amountCoins;
    public int amountHealth;

    public string ff;


    void Awake()
    {
        string FullPatch = Application.dataPath + "\\" + Patch;

        if (!File.Exists(FullPatch))
        {
            Debug.LogError("File not found");
            return;
            //throw new FileNotFoundException(Patch);
        }

        XmlDocument Doc = new XmlDocument();
        Doc.Load(FullPatch);

        XmlElement Root = Doc.DocumentElement;
        XmlNodeList XNL = Root.ChildNodes;


        ff = Root.ChildNodes[0].Attributes[""].Value;

        amountCoins = Convert.ToInt32(Root.ChildNodes[0].Attributes["value"].Value);
        amountHealth = Convert.ToInt32(Root.ChildNodes[1].Attributes["value"].Value);
    }
}