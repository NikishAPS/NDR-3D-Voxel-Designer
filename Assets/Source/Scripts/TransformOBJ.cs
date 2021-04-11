using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformOBJ : MonoBehaviour
{
    [SerializeField] private Transform _target;

    //position
    [SerializeField] private InputField posX;
    [SerializeField] private InputField posY;
    [SerializeField] private InputField posZ;

    //rotation
    [SerializeField] private InputField rotX;
    [SerializeField] private InputField rotY;
    [SerializeField] private InputField rotZ;

    //scale
    [SerializeField] private InputField scaleX;
    [SerializeField] private InputField scaleY;
    [SerializeField] private InputField scaleZ;

    //transparency
    [SerializeField] private MyGUI.MySlider my_transparency;

    private Vector3 _pos, _rot, _scale;

    private float _transparency;

    private MeshRenderer meshRenderer;

    void Start()
    {

    }

    void LateUpdate()
    {
        _pos = new Vector3(CorrectValue(posX.text), CorrectValue(posY.text), CorrectValue(posZ.text));
        _rot = new Vector3(CorrectValue(rotX.text), CorrectValue(rotY.text), CorrectValue(rotZ.text));
        _scale = new Vector3(CorrectValue(scaleX.text), CorrectValue(scaleY.text), CorrectValue(scaleZ.text));

        if(_target)
        {
            /*_target.position = new Vector3(
                float.Parse(posX.text),
                float.Parse(posY.text),
                float.Parse(posZ.text)
                );
            */
            _target.position = _pos;
            _target.eulerAngles = _rot;
            _target.localScale = _scale;

            Color curColor = meshRenderer.material.color;
            for(int i =0; i < meshRenderer.materials.Length; i++)
            meshRenderer.materials[i].color = new Color(curColor.r, curColor.g, curColor.b, my_transparency.value);
            //meshRenderer.material.color = new Color(curColor.r, curColor.g, curColor.b, my_transparency.value);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        
        if(_target != null)
        {
            posX.text = _target.position.x.ToString();
            posY.text = _target.position.y.ToString();
            posZ.text = _target.position.z.ToString();

            rotX.text = _target.eulerAngles.x.ToString();
            rotY.text = _target.eulerAngles.y.ToString();
            rotZ.text = _target.eulerAngles.z.ToString();

            scaleX.text = _target.localScale.x.ToString();
            scaleY.text = _target.localScale.y.ToString();
            scaleZ.text = _target.localScale.z.ToString();

            meshRenderer = _target.GetComponent<MeshRenderer>();
            my_transparency.value = meshRenderer.material.color.a;
        }
    }

    public void SetPos(Vector3 pos)
    {
        _pos = pos;

        posX.text = pos.x.ToString();
        posY.text = pos.y.ToString();
        posZ.text = pos.z.ToString();
    }
    public Vector3 GetPos()
    {
        return _pos;

        return new Vector3(
        float.Parse(posX.text),
                float.Parse(posY.text),
                float.Parse(posZ.text)
                );
    }

    public void SetRot(Vector3 rot)
    {
        _rot = rot;

        rotX.text = rot.x.ToString();
        rotY.text = rot.y.ToString();
        rotZ.text = rot.z.ToString();
    }
    public Vector3 GetRot()
    {
        return _rot;
    }

    public void SetScale(Vector3 scale)
    {
        _scale = scale;

        scaleX.text = scale.x.ToString();
        scaleY.text = scale.y.ToString();
        scaleZ.text = scale.z.ToString();
    }
    public Vector3 GetScale()
    {
        return _scale;
    }

    private float CorrectValue(string value)
    {
        if (value != "")
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '.')
                {
                    value = value.Remove(i, 1).Insert(i, ",");
                    print(value);
                }
            }

            return float.Parse(value);
        }


        return 0;
    }

    private void CorrectSting(string value)
    {

    }

    public void SetTransparency(float value)
    {
        value = Mathf.Clamp(value, 0.1f, 1);
        _transparency = value;
    }
}
