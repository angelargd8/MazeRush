using Unity.VisualScripting;
using UnityEngine;

public class WallDisappear : MonoBehaviour
{
    [SerializeField] float disappearTime = 0.1f;

    private void Start()
    {
        Destroy(gameObject, disappearTime);
    }
}
