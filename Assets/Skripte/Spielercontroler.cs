using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielercontroler : MonoBehaviour
{
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] Kamera kamera;
    [SerializeField] float geschwindigkeit;
    [SerializeField] float sprungkraft;
    [SerializeField] Sprungstatus sprungstatus;
    private Vector3 laufrichtung;

    #nullable enable
    private Collider[] ueberlapkreis = new Collider[0];


    public enum Sprungstatus
    {
        Boden,
        Sprungvorbereitung,
        Sprung
    }

    // Start is called before the first frame update
    void Start()
    {
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

        Perspektivsprung();
    }

    public void Perspektivsprung()
    {
        if (transform.position.z < -19)
        {
            Vector3 linieVonKameraZuSpieler = -kamera.transform.position + transform.position;
            float zielYKoordinate = 0 + transform.localScale.y;
            //a * linie = ziel.y
            //transform.position = 
        }
    }

    public void Laufen()
    {
        rigidbody.velocity = new Vector3(laufrichtung.normalized.x, rigidbody.velocity.y, laufrichtung.normalized.z);
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
                        transform.position.y - transform.localScale.y,
                        transform.position.z), 0.2f, ~LayerMask.GetMask("Spieler"));

        if (ueberlapkreis.Length != 0 && sprungstatus == Sprungstatus.Sprung)
        {
            sprungstatus = Sprungstatus.Boden;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x,
                        transform.position.y - transform.localScale.y,
                        transform.position.z), 0.2f);
    }
}
