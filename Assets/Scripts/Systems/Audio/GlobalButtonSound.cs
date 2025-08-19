using UnityEngine;
using UnityEngine.EventSystems; // Bắt buộc phải có để kiểm tra UI
using UnityEngine.UI;           // Bắt buộc phải có để nhận diện Button

public class GlobalButtonSound : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject != null && selectedObject.GetComponent<Button>() != null)
            {
                SoundManager.instance.PlayClick();
            }
        }
    }
}