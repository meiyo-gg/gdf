using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ButtonCategory : MonoBehaviour
{
    [SerializeField] private CategoryType category;

    public CategoryType Category { get { return category; } }

    public void UpdateSelectedCategory()
    {
        InventoryManager.Instance.UpdateSelectedCategoryButton(gameObject);

        /*List<CustomPass> customPasses = GameObject.FindWithTag("ObjectUICustomPassVolume").GetComponent<CustomPassVolume>().customPasses;
        CustomPass RenderObjectOnUI = customPasses.Find(x => x.name == "RenderObjectOnUI");
        RenderObjectOnUI.enabled = false;*/

        InventoryManager.Instance.ListItems();
    }
}
