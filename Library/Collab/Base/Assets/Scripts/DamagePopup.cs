using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro popupText;
    private static int sortingOrder;
    public static DamagePopup Create(Vector2 position, float damageAmount, bool isHeal = false){
        Transform damagePopupTransform = Instantiate(MainGameController.instance.prefabDamagePopup, position, Quaternion.identity).transform;
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, damagePopupTransform, isHeal);
        return damagePopup;
    }
    private void Awake() {
        popupText = GetComponent<TextMeshPro>();
    }
    private void Setup(float damageAmount, Transform transform, bool isHeal){
        if(isHeal){
            popupText.color = new Color(4, 255, 0, 255);
        }
        else{
            popupText.color = new Color(236, 43, 34, 255);
        }
        GameObject go = transform.gameObject;
        Color baseColor = popupText.color;
        Vector3 idPos = transform.position;
        Vector3 movPos = new Vector3(idPos.x + Random.Range(-1f, 1f), idPos.y + 1.5f, idPos.z);
        
        go.transform.localScale = new Vector3(.7f, .7f, .7f);
        popupText.sortingOrder = sortingOrder;
        sortingOrder++;

        popupText.SetText(damageAmount.ToString());
        LeanTween.scale(go, new Vector3(1.1f, 1.1f, 1.1f), .5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.move(go, movPos, 1f).setEase(LeanTweenType.easeOutBack);
        LeanTween.value( go, a => popupText.color = a, baseColor, new Color(1,1,1,0), .9f).setDelay(.3f).setOnComplete(Destroy);
    }
    void Destroy() {
        Destroy(this.gameObject);
    }
}
