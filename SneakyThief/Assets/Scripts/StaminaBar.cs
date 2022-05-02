using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    public Slider staminaBar;
    public Gradient gradient;
    public Image fill;
    private static float staminaRegenTime = 3;
    private static float staminaDeployTime = 1;
    private static float maxStamina = 100;
    public float currentStamina;
    private WaitForSeconds regenTick = new WaitForSeconds(staminaRegenTime/maxStamina);
    private Coroutine regen;
    public static StaminaBar instance;

    private void Awake(){
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        fill.color = gradient.Evaluate(1f);

    }

    public void UserStamina(){
        if(currentStamina >= 0 ){
            currentStamina -= Time.deltaTime*maxStamina/staminaDeployTime;
            staminaBar.value = currentStamina;
            fill.color = gradient.Evaluate(staminaBar.normalizedValue);
            
            if(regen != null)
                StopCoroutine(regen);

            regen = StartCoroutine(RegenStamina());
        }
        else{
            currentStamina = 0;
            Debug.Log("Not enough stamina");
        }
    }

    private IEnumerator RegenStamina(){

        yield return new WaitForSeconds(2);

        while(currentStamina < maxStamina){

            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;
            fill.color = gradient.Evaluate(staminaBar.normalizedValue);
            yield return regenTick;
        }
        regen = null;


    }

    
}
