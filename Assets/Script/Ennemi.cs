using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemi : MonoBehaviour
{
    [SerializeField] private float vitesseDeplacement = 1.5f;

    private bool bougeDroite = true;
    public bool mort = false; //------------ Permet d'identifer si l'ennemi est mort

    private Animator anim_Ennemi;

    //----------------------------------------------------------------------------- Initialiser des variables
    void Start()
    {
        anim_Ennemi = GetComponent<Animator>();

    }

    void Update()
    {
        if (!mort)
        {
            Walk();

        } 
        
    }

    //----------------------------------------------------------------------------- Fait déplacer l'ennemi vers la droite

    private void Walk()
    {
    transform.Translate(Vector2.right * vitesseDeplacement * Time.deltaTime);
    anim_Ennemi.SetBool("Marche", true);

    }

    //----------------------------------------------------------------------------- Détecte les collisions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Patrouille")
        {
             if (bougeDroite)
             {
                 bougeDroite = false;
                 transform.eulerAngles = new Vector3(0, -180, 0); //------------ Inverse l'orientation de l'ennemi sur l'axe des x pour le faire déplacer vers la gauche.

            }
             else
             {
                 transform.eulerAngles = new Vector3(0, 0, 0); //------------ Inverse l'orientation de l'ennemi sur l'axe des x pour le faire déplacer vers la droite.
                 bougeDroite = true;
             }
        }
    }

}
