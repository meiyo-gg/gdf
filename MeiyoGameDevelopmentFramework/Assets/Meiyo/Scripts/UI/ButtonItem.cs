using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ButtonItem : MonoBehaviour
{
    public void UpdateSelectedItem()
    {
        InventoryManager.Instance.UpdateSelectedItemButton(gameObject);
        /*CustomPassVolume customPassVolume = GameObject.FindWithTag("ObjectUICustomPassVolume").GetComponent<CustomPassVolume>();
        customPassVolume.enabled = true;*/
        List<CustomPass> customPasses = GameObject.FindWithTag("ObjectUICustomPassVolume").GetComponent<CustomPassVolume>().customPasses;
        CustomPass RenderObjectOnUI = customPasses.Find(x => x.name == "RenderObjectOnUI");
        RenderObjectOnUI.enabled = true;
    }
}
