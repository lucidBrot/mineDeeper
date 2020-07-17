using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Frontend;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Solver;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Camera Camera;

    public CameraBehaviour CameraBehaviour;

    public CameraMovement CameraMovement;

    public KeyCode ActivateKey;

    public KeyCode MarkKey;

    public LayerMask LayerMask;

    public float RotationTriggerMarginPixels = 5f;

    private KeyDownState keyState;

    private GameObject keyDownObject;

    private Vector3 keyDownPosition;

    private Revelation activeRevelation;

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (keyState == KeyDownState.None)
        {
            if (Input.GetKeyDown(ActivateKey))
            {
                keyState = KeyDownState.ActivationKey;
                HandleKeyDown();
            }
            else if (Input.GetKeyDown(MarkKey))
            {
                keyState = KeyDownState.MarkKey;
                HandleKeyDown();
            }
        }
        else
        {
            if (activeRevelation != null)
            {
                // Can't accept clicks while we are given a revelation...
                if (!activeRevelation.IsFinished)
                {
                    return;
                }

                activeRevelation = null;
            }

            if (keyState == KeyDownState.ActivationKey && Input.GetKeyUp(ActivateKey) || keyState == KeyDownState.MarkKey && Input.GetKeyUp(MarkKey))
            {
                var ray = Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, LayerMask))
                {
                    if (keyDownObject != null && keyDownObject == hit.collider.gameObject)
                    {
                        var field = keyDownObject.GetComponentInParent<FieldVisual>();
                        if (field != null)
                        {
                            if (keyState == KeyDownState.ActivationKey && field.BoardCell.State == CellState.Default)
                            {
                                activeRevelation = RevealCellsAnimated(field.BoardCell);
                            }
                            else if (keyState == KeyDownState.MarkKey)
                            {
                                Game.Instance.ToggleMarking(field.BoardCell);
                            }
                        }
                    }
                }

                HandleKeyUp();
            }

            if (keyState == KeyDownState.MarkKey)
            {
                var currentMousePos = Input.mousePosition;
                if ((keyDownPosition - currentMousePos).magnitude >= RotationTriggerMarginPixels)
                {
                    HandleKeyUp();
                }
            }
        }
    }

    private void HandleKeyDown()
    {
        keyDownPosition = Input.mousePosition;
        SetCameraMovementActive(false);

        var ray = Camera.ScreenPointToRay(keyDownPosition);
        if (Physics.Raycast(ray, out var hit, LayerMask))
        {
            keyDownObject = hit.collider.gameObject;
        }
    }

    private void HandleKeyUp()
    {
        keyState = KeyDownState.None;
        keyDownObject = null;
        SetCameraMovementActive(true);
    }

    private void SetCameraMovementActive(bool active)
    {
        if (CameraBehaviour != null)
        {
            CameraBehaviour.enabled = active;
        }

        if (CameraMovement != null)
        {
            CameraMovement.enabled = active;
        }
    }

    private Revelation RevealCellsAnimated(BoardCell cell)
    {
        var revelation = Game.Instance.RevealSlow(cell);
        revelation.FollowWith(Game.Instance.CheckIfGameFinished);
        return revelation;
    }

    private enum KeyDownState
    {
        None,

        ActivationKey,

        MarkKey
    }
}
