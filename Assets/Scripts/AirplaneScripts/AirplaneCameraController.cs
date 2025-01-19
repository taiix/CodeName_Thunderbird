using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneCameraController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> virtualCameras; // List of Cinemachine virtual cameras

    private int currentCameraIndex = 0;

    public InputActionAsset planeActions;

    private InputAction switchCameraAction;
    private InputAction switchCameraBackAction;

    private void OnEnable()
    {
        switchCameraAction = planeActions.FindAction("SwitchCamera");
        switchCameraBackAction = planeActions.FindAction("SwitchCameraBack");
        if (switchCameraAction != null)
        {
            switchCameraAction.Enable();
            switchCameraAction.performed += OnSwitchCameraNext;
        }

        if (switchCameraBackAction != null)
        {
            switchCameraBackAction.performed += OnSwitchCameraBack;
            switchCameraBackAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (switchCameraAction != null)
        {
            switchCameraAction.performed -= OnSwitchCameraNext;
            switchCameraAction.Disable();
        }
        if (switchCameraBackAction != null)
        {
            switchCameraBackAction.performed -= OnSwitchCameraBack;
            switchCameraBackAction.Disable();
        }
    }

    private void Start()
    {
        //InitializeCameras();
    }
    private void InitializeCameras()
    {
        if (virtualCameras.Count == 0)
        {
            Debug.LogWarning("No Cinemachine virtual cameras assigned.");
            return;
        }

        // Enable only the first virtual camera by setting its priority high
        for (int i = 0; i < virtualCameras.Count; i++)
        {
            virtualCameras[i].Priority = (i == currentCameraIndex) ? 10 : 0;
        }
    }

    private void OnSwitchCameraNext(InputAction.CallbackContext context)
    {
        if (virtualCameras.Count == 0) return;

        if (GameManager.Instance)
        {
            if (!GameManager.Instance.IsPLayerInPlane()) return;
        }

        virtualCameras[currentCameraIndex].Priority = 0;

        currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Count;

        virtualCameras[currentCameraIndex].Priority = 10;

        //Debug.Log($"Switched to camera: {virtualCameras[currentCameraIndex].name}");
    }

    private void OnSwitchCameraBack(InputAction.CallbackContext context)
    {
        if (virtualCameras.Count == 0) return;

        if (GameManager.Instance)
        {
            if (!GameManager.Instance.IsPLayerInPlane()) return;
        }

        virtualCameras[currentCameraIndex].Priority = 0;

        currentCameraIndex = (currentCameraIndex - 1 + virtualCameras.Count) % virtualCameras.Count;

        virtualCameras[currentCameraIndex].Priority = 10;

        //Debug.Log($"Switched to camera: {virtualCameras[currentCameraIndex].name}");
    }
}
