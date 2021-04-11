using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;


[System.Serializable]
[XmlRoot("dialogue")]
public class ReaderXML
{
    [XmlElement("node")]
    public Node[] nodes;

    public static ReaderXML Load(TextAsset _xml)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReaderXML));
        StringReader reader = new StringReader(_xml.text);
        ReaderXML readerXML = xmlSerializer.Deserialize(reader) as ReaderXML;
        return readerXML;
    }


    /*public string Read(TextAsset _xml)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReaderXML));
        StringReader reader = new StringReader(_xml.text);
        ReaderXML readerXML = xmlSerializer.Deserialize(reader) as ReaderXML;

        return readerXML.text;
    }*/
}

[System.Serializable]
public class Node
{
    [XmlElement("text")]
    public string text;
}
