using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    static public string nomJoueur;
    static public string scene;
    static public bool partieReussie;
    static public string dureePartieNiveau1;
    static public string dureePartieNiveau2;
    static public string dureePartieNiveau3;
    static public string pointsNiveau1;
    static public string pointsNiveau2;
    static public string pointsNiveau3;
    static public string dureePartie;
    static public string nbDePoints;

    public float tempsDeJeu = 0f;
    private Text chronometre;
    private float duree = 0f;

    [SerializeField] private float delay = 2.5f;
    [SerializeField] private AudioClip sound_Defeat = null; //Retro game tone, musical negative 2 "https://www.zapsplat.com/music/retro-game-tone-musical-negative-2/"
    
    private AudioSource gm_AudioSrc;


    //----------------------------------------------------------------------------- Initialiser des variables

    void Start()
    {
        scene = SceneManager.GetActiveScene().name;
        gm_AudioSrc = GetComponent<AudioSource>();

        if (scene == "Level_01")
        {
            GameObject.Find("NomDuPersonnage").GetComponent<Text>().text = nomJoueur;
            tempsDeJeu = 75f;
        }

        if (scene == "Level_02")
        {
            GameObject.Find("NomDuPersonnage").GetComponent<Text>().text = nomJoueur;
            tempsDeJeu = 120f;
        }

        if (scene == "Level_03")
        {
            GameObject.Find("NomDuPersonnage").GetComponent<Text>().text = nomJoueur;
            tempsDeJeu = 150f;
        }

        if (scene == "Transition_01" || scene == "Transition_02")
        {
            GameObject.Find("NomDuPersonnage").GetComponent<Text>().text = nomJoueur;
            GameObject.Find("NbPoints").GetComponent<Text>().text = nbDePoints;
            GameObject.Find("TempsRestant").GetComponent<Text>().text = dureePartie;

        }

        if (scene == "Fin")
        {
            ChangerTexteFinal();
        }
    }

    void Update()
    {
        if (scene == "Level_01" && tempsDeJeu > 0f)
        {
            chronometre = GameObject.Find("TextCountdown").GetComponent<Text>();   
            Chrono();
        }

        if (scene == "Level_02" && tempsDeJeu > 0f)
        {
            chronometre = GameObject.Find("TextCountdown").GetComponent<Text>();
            Chrono();
        }

        if (scene == "Level_03" && tempsDeJeu > 0f)
        {
            chronometre = GameObject.Find("TextCountdown").GetComponent<Text>();
            Chrono();
        }

        if (tempsDeJeu <= 0f)
        {
            VerifPartie();
            nbDePoints = GameObject.Find("Pointage").GetComponent<Text>().text;
            partieReussie = GameObject.Find("Hero").GetComponent<Hero>().partieReussie;
        }
        
    }

    //----------------------------------------------------------------------------- Modifie le texte du chronomêtre

    void Chrono()
    {
        if (GameObject.Find("Hero").GetComponent<Hero>().partieEnCours)
        {
            
            if (tempsDeJeu <= 180 && tempsDeJeu > 130)
            {
                chronometre.text = "02:" + Mathf.Floor(tempsDeJeu - 120);

            } else if (tempsDeJeu <= 130 && tempsDeJeu > 120)
            {
                chronometre.text = "02:0" + Mathf.Floor(tempsDeJeu - 120);

            } else if (tempsDeJeu <= 120 && tempsDeJeu > 70)
            {
                chronometre.text = "01:" + Mathf.Floor(tempsDeJeu - 60);

            } else if (tempsDeJeu <= 70 && tempsDeJeu > 60)
            {
                chronometre.text = "01:0" + Mathf.Floor(tempsDeJeu - 60);

            } else if (tempsDeJeu <= 60 && tempsDeJeu > 10)
            {
                chronometre.text = "00:" + Mathf.Floor(tempsDeJeu);

            }
            else if (tempsDeJeu <= 10 && tempsDeJeu > 5)
            {
                chronometre.text = "00:0" + Mathf.Floor(tempsDeJeu);
               chronometre.color = Color.yellow;

            }
            else
            {
                chronometre.text = "00:0" + Mathf.Floor(tempsDeJeu);
                chronometre.color = Color.red;
            }


            tempsDeJeu -= Time.deltaTime;
            duree += Time.deltaTime;
            TexteDuree();


            if (tempsDeJeu <= 0)
            {
                GameObject.Find("Hero").GetComponent<Animator>().SetBool("Dead", true);
                GameObject.Find("Hero").GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                gm_AudioSrc.PlayOneShot(sound_Defeat);
                GameObject.Find("Hero").GetComponent<Hero>().trameInt.Stop();
            }
        }


    }

    //----------------------------------------------------------------------------- Commencer la partie

    public void DebutPartie()
    {
        nomJoueur = GameObject.Find("LeNomDuJoueur").GetComponent<InputField>().text;

        if (nomJoueur.Length == 0)
        {
            GameObject.Find("Placeholder").GetComponent<Text>().text = "Inscrivez votre nom";

        } else
        {
            SceneManager.LoadScene("Level_01");
 
        }
    }

    //----------------------------------------------------------------------------- Vérifie si la partie est gagnée ou perdue

    void VerifPartie()
    {
        delay -= Time.deltaTime;

        if (delay <= 0f)
        {
            if (scene == "Level_01" && GameObject.Find("Hero").GetComponent<Hero>().partieReussie) //--------- Si le niveau 1 est gagné
            {
                dureePartieNiveau1 = dureePartie;
                pointsNiveau1 = nbDePoints;
                SceneManager.LoadScene("Transition_01");

            } else if (scene == "Level_02" && GameObject.Find("Hero").GetComponent<Hero>().partieReussie) //--------- Si le niveau 2 est gagné
            {
                dureePartieNiveau2 = dureePartie;
                pointsNiveau2 = nbDePoints;
                SceneManager.LoadScene("Transition_02");

            }
            else //--------- Si le niveau 3 est gagné ou si le joueur perd
            {
                if (scene == "Level_01") //--------- Initialise les résultats pour le niveau 1
                {
                    dureePartieNiveau1 = dureePartie;
                    pointsNiveau1 = nbDePoints;
                }

                if (scene == "Level_02") //--------- Initialise les résultats pour le niveau 2
                {
                    dureePartieNiveau2 = dureePartie;
                    pointsNiveau2 = nbDePoints;
                }

                if (scene == "Level_03") //--------- Initialise les résultats pour le niveau 3
                {
                    dureePartieNiveau3 = dureePartie;
                    pointsNiveau3 = nbDePoints;
                }

                SceneManager.LoadScene("Fin");
            }

        }
        
        
    }

    //----------------------------------------------------------------------------- Modifie le texte pour la durée totale de la partie

    void TexteDuree()
    {

        if (duree <= 180 && duree > 130)
        {
            dureePartie = "02:" + Mathf.Floor(duree - 120).ToString();

        }
        else if (duree <= 130 && duree > 120)
        {
            dureePartie = "02:0" + Mathf.Floor(duree - 120).ToString();

        }
        else if (duree <= 120 && duree > 70)
        {
            dureePartie = "01:" + Mathf.Floor(duree - 60).ToString();

        }
        else if (duree <= 70 && duree > 60)
        {
            dureePartie = "01:0" + Mathf.Floor(duree - 60).ToString();

        }
        else if (duree <= 60 && duree >= 10)
        {
            dureePartie = "00:" + Mathf.Floor(duree).ToString();

        }
        else
        {
            dureePartie = "00:0" + Mathf.Floor(duree).ToString();

        }

    }

    //----------------------------------------------------------------------------- Transition vers les autres scènes en cliquant sur le bouton


    public void GererScene()
    {
        if (scene == "Transition_01")
        {
            SceneManager.LoadScene("Level_02");

        } else if (scene == "Transition_02")
        {
            SceneManager.LoadScene("Level_03");

        } else
        {
            dureePartieNiveau1 = null; //--------- Réinitialise les variables static
            dureePartieNiveau2 = null;
            dureePartieNiveau3 = null;
            pointsNiveau1 = null;
            pointsNiveau2 = null;
            pointsNiveau3 = null;
            SceneManager.LoadScene("Accueil");
        }
    }


    //----------------------------------------------------------------------------- Change les textes de la scène de fin

    private void ChangerTexteFinal()
    {
        if (partieReussie)
        {
            GameObject.Find("GrosTexte").GetComponent<Text>().text = "Bravo !";
            GameObject.Find("PetitTexte").GetComponent<Text>().text = "Vous avez terminé le jeu !";

        } else
        {
            GameObject.Find("GrosTexte").GetComponent<Text>().text = "Dommage...";
            GameObject.Find("PetitTexte").GetComponent<Text>().text = "Vous avez perdu.";

        }

        GameObject.Find("NomDuPersonnage").GetComponent<Text>().text = nomJoueur;

        GameObject.Find("NbPointsNiveau3").GetComponent<Text>().text = pointsNiveau3;
        GameObject.Find("TempsRestantNiveau3").GetComponent<Text>().text = dureePartieNiveau3;

        GameObject.Find("NbPointsNiveau2").GetComponent<Text>().text = pointsNiveau2;
        GameObject.Find("TempsRestantNiveau2").GetComponent<Text>().text = dureePartieNiveau2;

        GameObject.Find("NbPoints").GetComponent<Text>().text = pointsNiveau1;
        GameObject.Find("TempsRestant").GetComponent<Text>().text = dureePartieNiveau1;
    }
}
