using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Skripte
{
    internal interface ITeleportationsausloesend
    {
        public float Hoehe { get; }
        public float HoeheZiel { get; }

        ////das <T> braucht es vielleicht nicht? Ich werde nur Monobehaviors teleportieren oder?
        //public bool Teleportationsvoraussetzung(MonoBehaviour pruefentitaet);

        //public void Teleportation();
    }
}
