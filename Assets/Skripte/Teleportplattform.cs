using Assets.Skripte;
using JetBrains.Annotations;
using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class Teleportplattform : MonoBehaviour, ITeleportationsausloesend
{
    //die Farbe kann komplett aus dem Skript getilgt werrden

    public RaycastHit Plattformtreffer { get; set; }

    [SerializeField] private Material spezialmaterial;
    [SerializeField] private Teleportplattform zielplattform;

    [SerializeField] private bool kannUnsichtbarWerden;
    [SerializeField] private bool istUnsichtbar;

    [SerializeField] private Vector3 anfangsposition;
    [SerializeField] private Vector3 verschwindeposition;
    [SerializeField] private MonoBehaviour unsichtbarwerendePlattform;

    //[SerializeField]
    //private (TypeName typ, BinaryOperator vergleicher) Voraussetzungen;  //das ist vermutlich keine gute Idee, wenn auch eine recht witzige

    public float Hoehe
    {
        get
        {
            return transform.position.y + transform.localScale.y;
        }
    }

    public float HoeheZiel
    {
        get
        {
            return zielplattform.Hoehe;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anfangsposition = transform.position;
        if (kannUnsichtbarWerden)
        {
            Spielercontroler.onPlattformSichtbarkeitsEventhaendler += SichtbarkeitAendern;

            List<Material> materialien = new List<Material>();
            materialien.Add(spezialmaterial);
            //farbe = spezialmaterial.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SichtbarkeitAendern()
    {
        if (istUnsichtbar)
        {
            SichtbarWerden();
            istUnsichtbar = false;
        }
        else
        {
            UnsichtbarWerden();
            istUnsichtbar = true;
        }
    }

    public void UnsichtbarWerden()
    {
        //spezialmaterial.color = Color.clear;  //das funktioniert  nicht, Color.clear macht das Ding schwarz
        transform.position = verschwindeposition;
    }

    public void SichtbarWerden()
    {
        //spezialmaterial.color = farbe;
        transform.position = anfangsposition;
    }

    
    ////wie sage ich, dass T eine ITeleportationsausloesende Sache sein muss
    //public bool Teleportationsvoraussetzung(MonoBehaviour pruefEntitaet)
    //{
    //    if (pruefEntitaet.transform.position.y < 0) { }
    //    return true;
    //}

    ////public void teste(TypeName typ, BinaryOperator vergleicher, var Vergleichsvariable) { }

    //public void Teleportation() //?? hierhin? und ein MonoBehavior übergibt sich (this) und wird teleportiert?
    //{}
}
