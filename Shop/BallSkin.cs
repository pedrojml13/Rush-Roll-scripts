using UnityEngine;
using System;

namespace PJML.RushAndRoll
{
    [Serializable]
    public class BallSkin
    {
        public int id;
        public Texture texture;
        public string name;
        public int price;
        public bool isUnlocked;
    }
}