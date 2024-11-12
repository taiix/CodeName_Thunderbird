using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaneIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform planePosition;

    private void Update()
    {
        image.transform.position = Camera.main.WorldToScreenPoint(planePosition.position);
        float dot = Vector3.Dot((planePosition.position - this.transform.position).normalized,
            transform.forward.normalized);

        if (dot < 0)
        {
            image.gameObject.SetActive(false);
        }
        else
        {
            image.gameObject.SetActive(true);
            float dist = (int)(planePosition.position - this.transform.position).magnitude;
            text.text = dist.ToString() + "m";
        }
    }
}
