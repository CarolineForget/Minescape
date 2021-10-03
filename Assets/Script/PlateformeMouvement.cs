using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeMouvement : MonoBehaviour
{

    private float positionX = 0f;
    [SerializeField] private float vitesseMouvement = 1f;
    private bool bougeDroite = true;

    void Start()
    {
        positionX = transform.position.x;
    }

    void Update()
    {
        BougePlateforme();
    }

    //----------------------------------------------------------------------------- Faire bouger une plateforme

    void BougePlateforme()
    {
        if (positionX <= 0f )
        {
            bougeDroite = true;

        } else if (positionX >= 2f) {
            bougeDroite = false;

        }
        
        if (bougeDroite == true)
        {
            positionX += vitesseMouvement * Time.deltaTime;

        } else {
            positionX -= vitesseMouvement * Time.deltaTime;

        }


        transform.position = new Vector2(positionX, transform.position.y);
    }

} //Fin de la classe
