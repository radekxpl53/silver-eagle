using System.Data.Common;
using System.Drawing;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_ResourceSlot : MonoBehaviour
{

    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;
    public void Setup(ResourceDefinition data, int amount)
    {
    
    if (iconImage != null) 
    {
        
        if (data.Icon != null) 
        {
            iconImage.sprite = data.Icon;
        }
        else 
        {
            
        }
    }
if (nameText != null) nameText.text = data.Name;
    if (amountText != null) amountText.text = amount.ToString();
    }
}
