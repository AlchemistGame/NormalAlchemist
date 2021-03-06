﻿using UnityEngine;

namespace MyBattle
{
    public class CharacterData : ActorData
    {
        public CharacterData(string name, ActorCamp camp, Vector2Int coord, int speed, string prefabPath) : base(name, camp, coord, speed, prefabPath)
        {
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
            Renderer[] renderers = sceneObject.gameObject.GetComponentsInChildren<Renderer>();
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
}
