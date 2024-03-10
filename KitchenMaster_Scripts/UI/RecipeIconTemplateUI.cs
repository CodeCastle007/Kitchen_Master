using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIconTemplateUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }
}
