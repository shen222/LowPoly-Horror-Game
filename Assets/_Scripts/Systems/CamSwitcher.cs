using UnityEngine;
using Unity.Cinemachine;

public class CamSwitcher : MonoBehaviour
{
    public Transform player;
    public CinemachineCamera activeCamera;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCamera.Priority = 1;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCamera.Priority = 0;
        }
    }
}
