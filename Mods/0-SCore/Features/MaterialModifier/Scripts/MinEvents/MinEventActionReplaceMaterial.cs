﻿using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;
/*
 * 	<append xpath="entity_classes/entity_class[@name='zombieArleneRadiated']">
		<effect_group name="ReplaceMaterial">
		<triggered_effect trigger="onSelfFirstSpawn" action="ReplaceMaterial, SCore" target_material_name="HD_Arlene_Radiated" replace_material="#@modfolder:Resources/ww_zeds_1.unity3d?HD_Arlene_Rad"/>
		<triggered_effect trigger="onSelfFirstSpawn" action="ReplaceMaterial, SCore" target_material_name="HD_Arlene_Radiated2" replace_material="#@modfolder:Resources/ww_zeds_1.unity3d?HD_Arlene_Rad"/>
		<triggered_effect trigger="onSelfFirstSpawn" action="ReplaceMaterial, SCore" target_material_name="HD_Arlene_Radiated3" replace_material="#@modfolder:Resources/ww_zeds_1.unity3d?HD_Arlene_Rad"/>
		</effect_group>
	</append>
 */
[Preserve]
public class MinEventActionReplaceMaterial : MinEventActionTargetedBase
{
    private string targetMaterialName = string.Empty;
    private string replaceMaterialPath = string.Empty;

    private Renderer[] renderers;
    private Material[] materials;


    private void DebugInfo(string entity, string material)
    {
        if (GameManager.IsDedicatedServer) return;
        if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuShowTasks) )
        {
            Log.Out($"ReplaceMaterial: {entity} {material}");
        }
    }

    public override void Execute(MinEventParams _params)
    {
        renderers = _params.Self?.RootTransform?.GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                materials = renderer.materials;

                if (materials[i] == null || string.IsNullOrEmpty(materials[i].name))
                {
                    continue;
                }

                string currentMaterialName = materials[i].name;
                DebugInfo(_params.Self?.EntityName, currentMaterialName);
                
                if (materials[i].name.EndsWith("(Instance)"))
                {
                    currentMaterialName = materials[i].name.Replace("(Instance)", string.Empty).Trim();
                }

                if (currentMaterialName == targetMaterialName)
                {
                    Material replaceMaterial = DataLoader.LoadAsset<Material>(replaceMaterialPath);

                    if (replaceMaterial == null)
                    {
                        Log.Warning(
                            $"Failed to replace target material '{targetMaterialName}' with '{replaceMaterialPath}' because it could not be loaded!");
                        return;
                    }

                    materials[i] = replaceMaterial;
                    renderer.materials = materials;
                }
            }
        }
    }

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
    {
        return base.CanExecute(_eventType, _params) && _params.Self != null && _params.Self.world != null;
    }

    public override bool ParseXmlAttribute(XAttribute _attribute)
    {
        bool flag = base.ParseXmlAttribute(_attribute);
        if (!flag)
        {
            string localName = _attribute.Name.LocalName;
            if (localName == "target_material_name")
            {
                targetMaterialName = _attribute.Value;
                return true;
            }

            if (localName == "replace_material")
            {
                replaceMaterialPath = _attribute.Value;
                DataLoader.PreloadBundle(replaceMaterialPath);
            }
        }

        return flag;
    }
}