using Assets.Skripte;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Spielercontroler : MonoBehaviour
{
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] Kamera kamera;
    [SerializeField] float geschwindigkeit;
    [SerializeField] float sprungkraft;
    [SerializeField] private Sprungstatus sprungstatus;
    [SerializeField] private Vector3 groesseIntern = Vector3.one;
    [SerializeField] private sichtbarkeitsschleussenflag schleussenstatus;
    [SerializeField] private bool befindetSichAufDerHinterenHaelfte;

    private Vector3 laufrichtung;
    private float teleportationsplattformdetektorMaxRayLaenge = 5;

    //Delegator & Event
    public delegate void Teleportation();
    public static Teleportation onPlattformSichtbarkeitsEventhaendler;


#nullable enable
    private Collider[] ueberlapkreis = new Collider[0];

    [Flags]
    public enum sichtbarkeitsschleussenflag
    {
        sichtbarkeitsschleusse1 = 1,
        sichtbarkeitsschleusse2 = 2,
    }

    public enum Sprungstatus
    {
        Boden,
        Sprungvorbereitung,
        Sprung
    }

    private void OnEnable()
    {
        //Unity meckert, wenn dem eventhandler vlt. keine Methode zugewiesen ist vermute ich, weil = Leer; behebt es
        onPlattformSichtbarkeitsEventhaendler = Leer;
    }

    private void Leer()
    {
        Debug.Log("onTeleportationEventHandler ruft Leere Methode auf!");
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 linieVonKameraZuSpieler = kamera.transform.position - transform.position;
        //wir muessen das vom untersten Punkt des Spielers betrachten, sonst nährt sich der Spieler durchs größer werden beim entfernen der Kamera, ...
        //warte das ergibt keinen Sinn, warum macht er das?
        float faktor = linieVonKameraZuSpieler.magnitude;
        groesseIntern = transform.localScale / faktor;
    }

    // Update is called once per frame
    void Update()
    {
        Laufenpruefen();
        Springenvorbereiten();
    }

    public void FixedUpdate()
    {
        Laufen();
        BodenPruefen();
        Springen();

        PerspektivsprungAbfrage();
        groesseAendern();
    }

    public void groesseAendern()
    {

        Vector3 linieVonKameraZuSpieler = kamera.transform.position - transform.position;
        //wir muessen das vom untersten Punkt des Spielers betrachten, sonst nährt sich der Spieler durchs größer werden beim entfernen der Kamera, ...
        //warte das ergibt keinen Sinn, warum macht er das?
        float faktor = linieVonKameraZuSpieler.magnitude;

        //der Betrag! Sonst ist die Magnitude ggf. negativ und dann wird der Spieler upsidedown gedreht,
        //was bei der Kapsel kein Problem, aber der Overlappcircle ist oben und dann wird der Ground nicht detected
        //Debug.Log(kamera.transform.position + " - " + transform.position + " + " + (transform.localScale.y * Vector3.down) + 
        //    " = linie von Kamera zu spieler = " + linieVonKameraZuSpieler + " Faktor:" + faktor);
        transform.localScale = faktor * groesseIntern;
    }

    public void PerspektivsprungAbfrage()
    {
        if (transform.position.x < -15)
        {
            if (transform.position.z < -8)
            {
                if (befindetSichAufDerHinterenHaelfte)
                {
                    onPlattformSichtbarkeitsEventhaendler();
                    befindetSichAufDerHinterenHaelfte = false;
                }
            }
            else
            {
                if (!befindetSichAufDerHinterenHaelfte)
                {
                    onPlattformSichtbarkeitsEventhaendler();
                    befindetSichAufDerHinterenHaelfte = true;
                }
            }
        }
    
        if (Physics.Raycast(transform.position, Vector3.down,
                        out RaycastHit magischePlattformTreffer,
                        teleportationsplattformdetektorMaxRayLaenge))
        {

            if (magischePlattformTreffer.transform.CompareTag("Teleportationsausloeser"))
            {
                Debug.Log("Teleplattform entdeckt!");

                float distanzZurPlatform = magischePlattformTreffer.distance;
                float hoeheDerZielplattform = magischePlattformTreffer.transform.GetComponent<ITeleportationsausloesend>().HoeheZiel;

                Debug.Log("Zielhoehe " + hoeheDerZielplattform);
                Debug.Log("distanz zur Plattform " + distanzZurPlatform);

                //float hoeheDerPlattformmagischePlattform.rigidbody.gameObject.Hoehe;
                //float hoeheDerPlattform = magischePlattformTreffer.transform.position.y;
                if (transform.position.z < -19)  //magicnumber der Teleportplattform
                {
                    Debug.Log("z < -20");
                    teleportationEntlangDerKameraSpielerStrecke(hoeheDerZielplattform + distanzZurPlatform);
                    befindetSichAufDerHinterenHaelfte = true;
                    Debug.Log("nach unten teleportiert!");
                    //teleportationEntlangDerKameraSpielerStrecke(distanzZurPlatform);
                }
                if (transform.position.z > 0)
                {
                    Debug.Log("z > 0");
                    teleportationEntlangDerKameraSpielerStrecke(hoeheDerZielplattform + distanzZurPlatform); //Magicnumber (hoeheDerZielplattform der Plattform = 10.7f)
                    befindetSichAufDerHinterenHaelfte = false;
                    Debug.Log("nach oben teleportiert!");
                }
            }
            //if (magischePlattformTreffer.transform.CompareTag("Sichtbarkeitsschleusse1"))
            //{
            //    if (schleussenstatus.HasFlag(~sichtbarkeitsschleussenflag.sichtbarkeitsschleusse2))
            //    {
            //        schleussenstatus |= sichtbarkeitsschleussenflag.sichtbarkeitsschleusse1;
            //    }
            //}
            //if (magischePlattformTreffer.transform.CompareTag("Sichtbarkeitsschleusse1")){ }
        }
    }


    private void teleportationEntlangDerKameraSpielerStrecke(float zielhoehe)
    {
        onPlattformSichtbarkeitsEventhaendler();

        Vector3 linieVonKameraZuSpieler = -kamera.transform.position + transform.position;
        //auf die ZielyKoordinate muss noch die Hoehe rein, falls er gerade im Sprung ist,
        //oder: erstmal einfacher ich frage ab, ob er im Sprung ist und teleportiere erst anschliesend auf dem Boden,
        //aber ich will ja auch de Raycast ueben

        // a * linie + kamerapos = f(a)
        // gesucht ist a mit f(a).y = ziel.y
        // a * linie.y + kamerapos.y = ziel.y

        // a = ziel.y - kamerapos.y / (linie.y) ;

        float faktor = (zielhoehe - kamera.transform.position.y) / (linieVonKameraZuSpieler.y);

        Vector3 neuePosition = faktor * linieVonKameraZuSpieler + kamera.transform.position;
        transform.position = neuePosition;

        Debug.Log("Zielkoorindate" + zielhoehe + " | skalar: " + faktor +
                    "\n * (" + linieVonKameraZuSpieler.x + "|" + linieVonKameraZuSpieler.y + "|" +
                    linieVonKameraZuSpieler.z + ") + (" + kamera.transform.position.x +
                    "|" + kamera.transform.position.y + "|" + kamera.transform.position.z + ")");

    }

    public void Laufen()
    {
        rigidbody.velocity = new Vector3(laufrichtung.normalized.x * geschwindigkeit, rigidbody.velocity.y, laufrichtung.normalized.z * geschwindigkeit);
        laufrichtung = Vector3.zero;
    }

    public void Laufenpruefen()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            laufrichtung += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            laufrichtung += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            laufrichtung += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            laufrichtung += Vector3.back;
        }
    }

    public void Springenvorbereiten()
    {
        if (Input.GetKey(KeyCode.Space) && sprungstatus == Sprungstatus.Boden)
        {
            sprungstatus = Sprungstatus.Sprungvorbereitung;
        }
    }

    public void Springen()
    {
        if (sprungstatus == Sprungstatus.Sprungvorbereitung)
        {
            rigidbody.AddForce( (sprungkraft * Vector3.up), ForceMode.Impulse);
            sprungstatus = Sprungstatus.Sprung;
        }
    }

    private void BodenPruefen()
    {
        //warum das 'auf einmal' ein Array ist, weiß ich nicht. 2D ist das nicht so, oder?
        ueberlapkreis = Physics.OverlapSphere(
            new Vector3(transform.position.x,
                        transform.position.y,
                        transform.position.z), 0.2f, ~LayerMask.GetMask("Spieler"));

        if (ueberlapkreis.Length != 0 && sprungstatus == Sprungstatus.Sprung)
        {
            sprungstatus = Sprungstatus.Boden;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x,
                        transform.position.y,
                        transform.position.z), 0.2f);


        Vector3 linieVonKameraZuSpieler = -kamera.transform.position + transform.position;
        //Gizmos.DrawLine(kamera.transform.position, transform.position);
        Gizmos.DrawRay(kamera.transform.position, 5 * linieVonKameraZuSpieler);
        //Gizmos.DrawRay(new Vector3(-12, 21, -38), new Vector3(6, -10, 20));
    }
}