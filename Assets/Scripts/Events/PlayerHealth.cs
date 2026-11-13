using UnityEngine;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    
    private void Sethealth(float health)
    {
        healthText.text = health.ToString();
    }


    public void UpdateHealth(Component sender, object data )
    {
        if(data is float)
        {
            float amount = (float) data;

            Sethealth((int)amount);
        }
       
    }
}
