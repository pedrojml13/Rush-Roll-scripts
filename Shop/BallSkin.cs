using UnityEngine;
using System;

namespace PJML.RushAndRoll
{
    [Serializable]
    public class BallSkin
    {
        public int id;
        public Material material;
        public string name;
        public int price;
        public bool isUnlocked;
        public bool isRMSkin;
    }
}