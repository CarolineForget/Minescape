using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class RocheLancee : MonoBehaviour
{
    [SerializeField] private GameObject bouleCristal = null;
    [SerializeField] private GameObject cristal = null;
    [SerializeField] private GameObject cristalTime = null;

    private Rigidbody2D rb_Roche;
    private Collider2D collider_Roche;

    private float thrust = 2.5f;
    private bool rocheLanceeDroite = false;
    private bool rocheLanceeGauche = false;

    //----------------------------------------------------------------------------- Initialisation des variables

    void Start()
    {
        rb_Roche = GetComponent<Rigidbody2D>();
        collider_Roche = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!rocheLanceeDroite)
        {
            versLaGauche();
        }

        if (!rocheLanceeGauche)
        {
            versLaDroite();
        }

        if (collider_Roche.IsTouchingLayers(LayerMask.GetMask("Sol")) || collider_Roche.IsTouchingLayers(LayerMask.GetMask("Contour"))) //--------- Pour détruire la roche si elle touche au sol ou au contour du jeu
        {
            rocheLanceeGauche = false;
            rocheLanceeDroite = false;
            Destroy(gameObject);
        }

    }

    //----------------------------------------------------------------------------- Pour bouger les roches lancées vers la droite

    void versLaDroite()
    {
        if (GameObject.Find("Hero").GetComponent<Hero>().orientationRoche == 1)
        {
            rb_Roche.AddForceAtPosition(new Vector2(thrust, -0.03f), new Vector2(transform.position.x, transform.position.y), ForceMode2D.Impulse); //--------- Crée une impulsion pour faire bouger la roche vers la droite avec la force de l'impulsion
            rocheLanceeDroite = true;

            Destroy(gameObject, 0.5f); //--------- Détruit la roche après un délai
        }
    }

    //----------------------------------------------------------------------------- Pour bouger les roches lancées vers la gauche

    void versLaGauche()
    {
        if (GameObject.Find("Hero").GetComponent<Hero>().orientationRoche == -1)
        {
            rb_Roche.AddForceAtPosition(new Vector2(thrust *-1, -0.03f), new Vector2(transform.position.x, transform.position.y), ForceMode2D.Impulse); //--------- Crée une impulsion pour faire bouger la roche vers la gauche avec la force de l'impulsion
            rocheLanceeGauche = true;

            Destroy(gameObject, 0.5f); //--------- Détruit la roche après un délai
        }

    }

    //----------------------------------------------------------------------------- Pour détecter les collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //----------------------------------------------------------------------------- Avec les cristaux
        if (collision.transform.tag == "Objects")
        {
            GameObject cristauxActive = Instantiate(bouleCristal, collision.transform.position, collision.transform.rotation) as GameObject;
            GameObject cristalVert = Instantiate(cristal, collision.transform.position, collision.transform.rotation) as GameObject;

            GameObject.Find("Hero").GetComponent<Hero>().nbCristaux--;
            GameObject.Find("Hero").GetComponent<Hero>().points += 10;

            GameObject.Find("Hero").GetComponent<Hero>().perso_AudioSrc.PlayOneShot(GameObject.Find("Hero").GetComponent<Hero>().sound_Crystal);
            GameObject.Find("Pointage").GetComponent<Text>().text = GameObject.Find("Hero").GetComponent<Hero>().points.ToString();

            if (GameObject.Find("Hero").GetComponent<Hero>().nbCristaux == 0)
            {
                GameObject.Find("Hero").GetComponent<Hero>().Victoire();
            }

            Destroy(cristauxActive, 1f);
            Destroy(collision.gameObject);
            Destroy(gameObject);

            rocheLanceeDroite = false;
            rocheLanceeGauche = false;
        }

        //----------------------------------------------------------------------------- Avec les cristaux de temps
        if (collision.transform.tag == "Time")
        {
            GameObject cristauxTimeActive = Instantiate(cristalTime, collision.transform.position, collision.transform.rotation) as GameObject;

            GameObject.Find("Hero").GetComponent<Hero>().perso_AudioSrc.PlayOneShot(GameObject.Find("Hero").GetComponent<Hero>().sound_Countdown);
            GameObject.Find("Pont_Anim").GetComponent<Animator>().SetTrigger("Bouge");

            Destroy(gameObject);
            Destroy(cristauxTimeActive, GameObject.Find("Pont_Anim").GetComponent<Animator>().GetNextAnimatorStateInfo(0).length + 5f);

        }

        //----------------------------------------------------------------------------- Avec les ennemis
        if (collision.transform.tag == "Ennemi")
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("Mort");
            collision.gameObject.GetComponent<Ennemi>().mort = true;
            collision.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

            Destroy(collision.gameObject, 1.5f);
            Destroy(gameObject);

            GameObject.Find("Hero").GetComponent<Hero>().points += 20;
            GameObject.Find("Hero").GetComponent<Hero>().perso_AudioSrc.PlayOneShot(GameObject.Find("Hero").GetComponent<Hero>().sound_Hurt);
            GameObject.Find("Pointage").GetComponent<Text>().text = GameObject.Find("Hero").GetComponent<Hero>().points.ToString();
        }

    }

}
