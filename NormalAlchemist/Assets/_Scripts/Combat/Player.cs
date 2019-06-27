using UnityEngine;

public class Player : Actor
{
    public void Init(ActorInfo basic_info)
    {
        name = basic_info.name;
        transform.position = basic_info.position;
        transform.eulerAngles = basic_info.rotation;
    }

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();

        ChangeModelOutlineColor(Color.green);
    }

    protected override void OnMouseExit()
    {
        base.OnMouseExit();

        ChangeModelOutlineColor(Color.black);
    }

    private void ChangeModelOutlineColor(Color color)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material.shader.name.Equals("Custom/Outline"))
                {
                    material.SetColor("_OutlineColor", color);
                }
            }
        }
    }
}
