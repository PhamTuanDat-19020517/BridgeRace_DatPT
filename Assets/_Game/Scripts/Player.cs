using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class Player : Character
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    public int randomColorInt;
    private Vector3 direction;
    private Vector3 velocity;
    private float gravity = -10f;
    private FloatingJoystick joystick;
    private EndLevel endLevel;


    private void Awake()
    {
        ChangeColor();
    }
    private void Start()
    {
        joystick = FindObjectOfType<FloatingJoystick>();
        endLevel = FindObjectOfType<EndLevel>();
        endLevel.OnEndLevelAction += EndGame;
        //ChangeColor();
    }
    private void Update()
    {
        direction = new Vector3(joystick.Direction.x, 0f, joystick.Direction.y).normalized;
        RaycastHit stair;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, out stair, Mathf.Infinity, stairLayer))
        {
            Stair stairScript = stair.transform.gameObject.GetComponent<Stair>();
            Material stairMaterial = stair.transform.gameObject.GetComponent<MeshRenderer>().material;
            if (stairScript.stairColor != characterColor || stairMaterial == stairScript.defaultMaterial)
            {
                if (direction.z >= 0f) direction.z = 0;
            }
        }
        if (Vector3.Distance(Vector3.zero, direction) > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            controller.Move(direction * speed * Time.deltaTime);
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        RaycastHit ground;
        Debug.DrawLine(transform.position, Vector3.down, Color.red);
        if (!Physics.Raycast(transform.position, Vector3.down, out ground, 1f, groundLayer))
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = 0;
        }
    }
    public void ChangeColor()
    {
        randomColorInt = UnityEngine.Random.Range(0, 4);
        ColorData.ColorType color = (ColorData.ColorType)randomColorInt;
        CharacterMesh.GetComponent<SkinnedMeshRenderer>().material = colorData.GetColor(color);
        characterColor = color;
    }
}
