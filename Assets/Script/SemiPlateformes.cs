using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiPlateformes : MonoBehaviour
{
    private PlatformEffector2D effector;

    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        versLeHaut();
        versLeBas();

    }

    //----------------------------------------------------------------------------- Permet de traverser une plateforme par le bas

    void versLeHaut()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            effector.rotationalOffset = 0f;

        }
    }

    //----------------------------------------------------------------------------- Permet de traverser une plateforme par le haut

    void versLeBas()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            effector.rotationalOffset = 180f;

        }
    }
}
