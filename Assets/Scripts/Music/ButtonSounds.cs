using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
            SFXManager.Instance.PlaySFX(SFXManager.Instance.clickClip));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        SFXManager.Instance.PlaySFX(SFXManager.Instance.hoverClip);
    }
}