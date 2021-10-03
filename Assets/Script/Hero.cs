using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hero : MonoBehaviour
{
    //----------------------------------------------------------------------------- Variables pour le Hero

    [SerializeField] private Collider2D collider_Attack = null;
    [SerializeField] private float vitesseDeplacement = 2f;
    [SerializeField] private float vitesseSaut = 4.5f;

    private Rigidbody2D rb_Hero;
    private Animator anim_Hero;
    private Collider2D collider_Hero;
    public AudioSource perso_AudioSrc;//

    private float largeurHero; //---------- Orientation de l'Hero
    private float flip = 1f; //------------ Modifie l'orientation
    private float punchTimer = 0f;
    private float punchCd = 0.3f;
    private Vector2 positionDepart;
    private float gravite_Hero;
    private bool persoMarche = false;
    private bool persoInterieur = true; //------------ Détermine si l'Hero est à l'intérieur de la mine
    private bool persoToucheMurDroit = false;
    private bool persoToucheMurGauche = false;

    public float orientationRoche = 1f; //---------- Orientation de l'Hero lorsqu'il lance une roche


    //----------------------------------------------------------------------------- Variables pour déterminer si la partie est en cours

    public int nbCristaux = 5;//
    private int nbVies = 3;
    public int points = 0;
    public bool partieEnCours;
    public bool partieReussie = false;


    //----------------------------------------------------------------------------- “Sound effects obtained from https://www.zapsplat.com“

    [SerializeField] private AudioClip sound_Jump = null;                               //Classic game sound, jump. Version 3 "https://www.zapsplat.com/music/classic-game-sound-jump-version-3/"
    [SerializeField] public AudioClip sound_Hurt = null;                               //Retro game tone, impact, punch, hit 1 "https://www.zapsplat.com/music/retro-game-tone-impact-punch-hit-1/"
    [SerializeField] public AudioClip sound_Crystal = null;                             //Game tone, bright musical ping. Version 1 "https://www.zapsplat.com/music/game-tone-bright-musical-ping-version-1/"
    [SerializeField] private AudioClip sound_Victory = null;                            //Retro game tone, musical level complete 7 "https://www.zapsplat.com/music/retro-game-tone-musical-level-complete-7/"
    [SerializeField] private AudioClip sound_Defeat = null;                             //Retro game tone, musical negative 2 "https://www.zapsplat.com/music/retro-game-tone-musical-negative-2/"
    [SerializeField] private AudioClip sound_Respawn = null;                            //Game tone, magical and mysterious short dark swell 1 "https://www.zapsplat.com/music/game-tone-magical-and-mysterious-short-dark-swell-1/"
    [SerializeField] private AudioClip sound_Throw = null;                              //Classic game sound, jump. Version 1 "https://www.zapsplat.com/music/classic-game-sound-jump-version-1/"


    //----------------------------------------------------------------------------- “Sound effects obtained from http://www.freesoundslibrary.com“

    public AudioClip sound_Countdown = null;                          //Countdown Sound par Spanac "https://www.freesoundslibrary.com/countdown-sound/"


    //----------------------------------------------------------------------------- "Music obtained from https://www.looperman.com/" 

    [SerializeField] public AudioSource trameInt = null;                                //PVLACE x Gunboi Bell Type 130 BPM - Gundbt Loops par gundb "https://www.looperman.com/loops/detail/206249/pvlace-x-gunboi-bell-type-130-bpm-gundbt-loops-130bpm-trap-bells-loop"
    [SerializeField] public AudioSource trameExt = null;                                //Say Yeah Piano par deesan "https://www.looperman.com/loops/detail/218139/say-yeah-piano-68bpm-hip-hop-piano-loop"
    //Trame d'ambiance pour le menu                                                     //Sad Piano Type 130 BPM - Gundbt Loops par gundb "https://www.looperman.com/loops/detail/206248/sad-piano-type-130-bpm-gundbt-loops-130bpm-ambient-piano-loop"


    //----------------------------------------------------------------------------- Création de GameObjects

    [SerializeField] private GameObject respawn = null;
    [SerializeField] private GameObject bouleRespawn = null;
    [SerializeField] private GameObject bouleCristal = null;
    [SerializeField] private GameObject cristal = null;
    [SerializeField] private GameObject poussiere = null;
    [SerializeField] private GameObject roche = null;
    [SerializeField] private GameObject teleportation = null;


    //----------------------------------------------------------------------------- Initialisation des variables

    void Start()
    {
        rb_Hero = GetComponent<Rigidbody2D>();
        anim_Hero = GetComponent<Animator>();
        collider_Hero = GetComponent<Collider2D>();
        perso_AudioSrc = GetComponent<AudioSource>();

        largeurHero = transform.localScale.x;
        positionDepart = new Vector2(rb_Hero.position.x, rb_Hero.position.y);
        collider_Attack.enabled = false; //-------------------------------------- Pour activer le collider de l'attaque
        gravite_Hero = rb_Hero.gravityScale;

        trameInt.Play();
        partieEnCours = true;

    }


    void Update()
    {
        if (partieEnCours)
        {
            Deplacement();
            Saut();
            Flip();
            Climb();
            Punch();
            Throw();

            partieReussie = false;
        }

    }

    //----------------------------------------------------------------------------- Déplacer horizontalement le personnage
    void Deplacement()
    {
        float mouvementHorizontal = Input.GetAxisRaw("Horizontal");
        rb_Hero.velocity = new Vector2(mouvementHorizontal * vitesseDeplacement, rb_Hero.velocity.y);
        

        bool heroMouvement = Mathf.Abs(rb_Hero.velocity.x) > 0;
        anim_Hero.SetBool("Walk", heroMouvement);

        if (heroMouvement == true)
        {
            poussiere.SetActive(true); //--------- Fait apparaître la poussière
            persoMarche = true;

            if (mouvementHorizontal > 0)
            {
                poussiere.transform.position = new Vector3(rb_Hero.position.x, rb_Hero.position.y - 0.3f); //--------- Modifie la position de la poussière en fonction de la position du personnage
                poussiere.transform.localScale = new Vector3(1, 1, 1); //--------- Change l'orientation de la poussière

            }
            else if (mouvementHorizontal < 0)
            {
                poussiere.transform.position = new Vector3(rb_Hero.position.x, rb_Hero.position.y - 0.3f);
                poussiere.transform.localScale = new Vector3(1,1,-1);

            }

        } else if (heroMouvement == false)
        {
            poussiere.SetActive(false);
            persoMarche = false;

        }


    }

    //----------------------------------------------------------------------------- Faire sauter le personnage
    void Saut()
    {
        //----------------------------------------------------------------------------- Saut sur le mur gauche
        if (persoToucheMurGauche)
        {
            poussiere.SetActive(false);

            if (Input.GetButtonDown("Jump"))
            {
                Vector2 forceSaut = new Vector2(0f, vitesseSaut);
                rb_Hero.velocity = forceSaut;

                anim_Hero.SetTrigger("Jump");
                perso_AudioSrc.PlayOneShot(sound_Jump);

                persoToucheMurGauche = false; //--------- Permet de faire sauter une fois de suite sur le mur gauche
            }
        }

        //----------------------------------------------------------------------------- Saut sur le mur droit
        if (persoToucheMurDroit)
        {
            poussiere.SetActive(false);

            if (Input.GetButtonDown("Jump"))
            {
                Vector2 forceSaut = new Vector2(0f, vitesseSaut);
                rb_Hero.velocity = forceSaut;

                anim_Hero.SetTrigger("Jump");
                perso_AudioSrc.PlayOneShot(sound_Jump);

                persoToucheMurDroit = false; //--------- Permet de faire sauter une fois de suite sur le mur droit
            }
        }

        //----------------------------------------------------------------------------- Saut sur le sol
        if (!collider_Hero.IsTouchingLayers(LayerMask.GetMask("Sol")))
        {
            poussiere.SetActive(false);
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
               Vector2 forceSaut = new Vector2(0f, vitesseSaut);
               rb_Hero.velocity = forceSaut;

              anim_Hero.SetTrigger("Jump");
              perso_AudioSrc.PlayOneShot(sound_Jump);

        }
    }

    //----------------------------------------------------------------------------- Changer l'orientation du personnage

    void Flip()
    {
        float direction = rb_Hero.velocity.x;

        if (direction < 0f){
        flip = -1f;

        } 
        else if (direction > 0f)
        {
        flip = 1f;

        }

        transform.localScale = new Vector2(largeurHero * flip, largeurHero);

    }

    //----------------------------------------------------------------------------- Détection des collisions de type trigger

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //----------------------------------------------------------------------------- Avec les cristaux
        if (collision.isTrigger != true && collision.CompareTag("Objects"))
        {
            GameObject cristauxActive = Instantiate(bouleCristal, collision.transform.position, collision.transform.rotation) as GameObject; //--------- Ajoute un nouveau GameObject dans le Hierarchy à la position de la collision
            GameObject cristalVert = Instantiate(cristal, collision.transform.position, collision.transform.rotation) as GameObject;

            Destroy(collision.gameObject);
            Destroy(cristauxActive, 1f);

            nbCristaux--;
            perso_AudioSrc.PlayOneShot(sound_Crystal);
            points += 10;

            GameObject.Find("Pointage").GetComponent<Text>().text = points.ToString();

            if (nbCristaux == 0)
            {
                Victoire();
            }
        }

        //----------------------------------------------------------------------------- Avec le respawn du milieu
        if (collision.transform.tag == "Respawn")
        {
            perso_AudioSrc.PlayOneShot(sound_Respawn);
            positionDepart = new Vector2(rb_Hero.position.x, rb_Hero.position.y);

            GameObject respawnActive = Instantiate(respawn, collision.transform.position, collision.transform.rotation) as GameObject;
            GameObject respawnParticles = Instantiate(bouleRespawn, collision.transform.position, collision.transform.rotation) as GameObject;

            Destroy(respawnParticles, 2f);
            Destroy(collision.gameObject);

        }

        //----------------------------------------------------------------------------- Permet de déterminer si le personnage est dans la mine
        if (collision.transform.tag == "Musique")
        {
            if (persoInterieur)
            {
                trameExt.Play();
                trameInt.Stop();
                persoInterieur = false;

            } else
            {
                trameExt.Stop();
                trameInt.Play();
                persoInterieur = true;
            }

        }

        //----------------------------------------------------------------------------- Permet de déterminer si le personnage frappe l'ennemi
        if (collision.isTrigger != true && collision.CompareTag("Ennemi"))
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("Mort");
            collision.gameObject.GetComponent<Ennemi>().mort = true;
            collision.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;

            Destroy(collision.gameObject, 1.5f);

            perso_AudioSrc.PlayOneShot(sound_Hurt);
            points += 40;

            GameObject.Find("Pointage").GetComponent<Text>().text = points.ToString();
            
        }


    }

    //----------------------------------------------------------------------------- Annonce la victoire

    public void Victoire()
    {
        anim_Hero.SetBool("Loop", true);
        anim_Hero.speed = 2f;
        perso_AudioSrc.PlayOneShot(sound_Victory);
        rb_Hero.bodyType = RigidbodyType2D.Static;

        GameObject.Find("GestionnaireJeu").GetComponent<GameManager>().tempsDeJeu = 0f;

        partieEnCours = false;
        partieReussie = true;

        if (persoInterieur)
        {
            trameInt.Stop();
        }
        else
        {
            trameExt.Stop();
        }

    }

    //----------------------------------------------------------------------------- Faire grimper le personnage

    void Climb()
    {
            if (!collider_Hero.IsTouchingLayers(LayerMask.GetMask("Echelle")))
            {
                rb_Hero.gravityScale = gravite_Hero;
                anim_Hero.SetBool("Climb", false);
                return;
            }

            float mouvementVertical = Input.GetAxisRaw("Vertical");
            rb_Hero.velocity = new Vector2(rb_Hero.velocity.x, mouvementVertical * vitesseDeplacement);

            rb_Hero.gravityScale = 0f;

            bool isClimbing = Mathf.Abs(rb_Hero.velocity.y) > 0;

            if (collider_Hero.IsTouchingLayers(LayerMask.GetMask("Echelle")))
            {
                anim_Hero.SetBool("Climb", isClimbing);

            }

    }

    //----------------------------------------------------------------------------- Faire attaquer le personnage
    void Punch()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim_Hero.SetTrigger("Punch");
            collider_Attack.enabled = true; //--------- Fait activer le collider de l'attaque
            punchTimer = punchCd;

        } 

        //------------------------------------ Délai pour le contact de l'attaque
        if (punchTimer > 0)
        {
           
            punchTimer -= Time.deltaTime;

        } else {
            collider_Attack.enabled = false;

        }
           

        
    }

    //----------------------------------------------------------------------------- Pour indiquer que le joueur a perdu

    public void GameOver()
    {
        anim_Hero.SetBool("Dead", true);
        perso_AudioSrc.PlayOneShot(sound_Defeat);
        rb_Hero.bodyType = RigidbodyType2D.Static;

        GameObject.Find("GestionnaireJeu").GetComponent<GameManager>().tempsDeJeu = 0f;

        partieEnCours = false;

        if (persoInterieur)
        {
            trameInt.Stop();
        }
        else
        {
            trameExt.Stop();
        }

    }

    //----------------------------------------------------------------------------- Détecter des collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //----------------------------------------------------------------------------- Avec la plateforme en mouvement (le personnage suit la plateforme)
        if (collision.transform.tag == "PlateformeMouvement")
        {
            transform.parent = collision.transform;
        }

        //----------------------------------------------------------------------------- Avec les pièges
        if (collision.transform.tag == "Piege" || collision.transform.tag == "Ennemi")
        {
            anim_Hero.SetTrigger("Hurt");
            rb_Hero.position = positionDepart;
            perso_AudioSrc.PlayOneShot(sound_Hurt);

            nbVies--;
            points -= 20;

            GameObject.Find("NbDeVies").GetComponent<Text>().text = "x" + nbVies.ToString();
            GameObject.Find("Pointage").GetComponent<Text>().text = points.ToString();

            //----------------------------------------------------------------------------- Fait jouer la trame d'ambiance intérieur si le personnage était à l'extérieur de la mine
            if (!persoInterieur)
            {
                trameExt.Stop();
                trameInt.Play();
                persoInterieur = true;
            }


            if (nbVies > 0)
            {
                GameObject teleportation_particules = Instantiate(teleportation, positionDepart, rb_Hero.transform.rotation) as GameObject;
                Destroy(teleportation_particules, 2f);
            }


            if (nbVies <= 0)
            {
                GameOver();
            }
        }

        //----------------------------------------------------------------------------- Avec les boîtes
        if (collision.transform.tag == "Boite")
        {

            if (persoMarche == true)
            {
                anim_Hero.SetBool("Push", true);

            } else if (persoMarche == false)
            {
                anim_Hero.SetBool("Push", false);

            }


        }

        //----------------------------------------------------------------------------- Avec les murs et le sol
        if (collision.transform.tag == "MurDroit")
        {
            persoToucheMurDroit = true;
            persoToucheMurGauche = false;
        }

        if (collision.transform.tag == "MurGauche")
        {
            persoToucheMurDroit = false;
            persoToucheMurGauche = true;
        }

        if (collision.transform.tag == "Sol")
        {
            persoToucheMurDroit = false;
            persoToucheMurGauche = false;
        }


    }


    //----------------------------------------------------------------------------- Détecter si le personnage ne fait plus de collisions

    private void OnCollisionExit2D(Collision2D collision)
    {
        //----------------------------------------------------------------------------- Avec une plateforme
        if (collision.gameObject.tag == "PlateformeMouvement")
        {
            transform.parent = null;
        }

        //----------------------------------------------------------------------------- Avec les boîtes
        if (collision.transform.tag == "Boite")
        {
            anim_Hero.SetBool("Push", false);
        }

    }

    //----------------------------------------------------------------------------- Détecter si le personnage est en collision avec les roches qui tombent (particles)

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.tag == "Piege")
        {
            anim_Hero.SetTrigger("Hurt");
            rb_Hero.position = positionDepart;
            perso_AudioSrc.PlayOneShot(sound_Hurt);

            nbVies--;
            points -= 20;

            GameObject.Find("NbDeVies").GetComponent<Text>().text = "x" + nbVies.ToString();
            GameObject.Find("Pointage").GetComponent<Text>().text = points.ToString();

            if (nbVies > 0)
            {
                GameObject teleportation_particules = Instantiate(teleportation, positionDepart, rb_Hero.transform.rotation) as GameObject;
                Destroy(teleportation_particules, 2f);
            }

            if (nbVies <= 0)
            {
                GameOver();
            }

        }
    }

    //----------------------------------------------------------------------------- Pour lancer des roches

    private void Throw()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            anim_Hero.SetTrigger("Throw");
            perso_AudioSrc.PlayOneShot(sound_Throw);

            GameObject rocheLancee = Instantiate(roche, rb_Hero.transform.position, rb_Hero.transform.rotation) as GameObject;

            if (flip == 1)
            {
                rocheLancee.transform.position = new Vector3(rb_Hero.position.x + 0.3f, rb_Hero.position.y);
                orientationRoche = 1f;

            } else if (flip == -1)
            {
                rocheLancee.transform.position = new Vector3(rb_Hero.position.x - 0.3f, rb_Hero.position.y);
                orientationRoche = -1f;
            }

            
            
        }
    }

    //----------------------------------------------------------------------------- Met la partie en pause quand le bouton d'aide est activé

    public void PartieEnPause()
    {
        partieEnCours = false;
        rb_Hero.bodyType = RigidbodyType2D.Static;

    }

    //----------------------------------------------------------------------------- Met la partie en cours quand le bouton d'aide est désactivé

    public void PartieEnJeu()
    {
        partieEnCours = true;
        rb_Hero.bodyType = RigidbodyType2D.Dynamic;
    }

} //Fin de la classe
