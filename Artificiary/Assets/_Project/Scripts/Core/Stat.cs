using Mystie.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Core
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField]
        protected float baseValue = 1; //add the ability to base this value off a formula that scales with level (?)

        protected float initalValue = 1;
        protected List<float> rates = new List<float>(); // % mod calculated by the multiplicative product of rate modifiers on this stat

        public Stat(float _baseValue = 1)
        {
            Set(_baseValue);
            initalValue = baseValue;
        }

        public void Set(float _baseValue = 1)
        {
            baseValue = _baseValue;
        }

        public float value()
        {
            float rate = 1f;

            if (!rates.IsNullOrEmpty()) rates.ForEach(x => rate *= x);

            float finalValue = baseValue * rate;

            return finalValue;
        }

        public void Reset()
        {
            baseValue = initalValue;
        }

        public virtual void AddMod(Mod mod)
        {
            if (mod._rate != 1) rates.Add(mod._rate);
        }

        public virtual void RemoveMod(Mod mod)
        {
            if (mod._rate != 1) rates.Remove(mod._rate);
        }

        public static implicit operator float(Stat stat)
        {
            return stat.value();
        }

        [System.Serializable]
        public class Mod
        {
            public float _rate = 1;

            public Mod(float rate = 1)
            {
                _rate = rate;
            }
        }
    }

    [System.Serializable]
    public class StatInt
    {
        [SerializeField]
        protected int baseValue = 1; //Add the ability to base this value off a formula that scales with level (?)

        protected int initalValue = 1;
        protected List<float> rates = new List<float>(); // % mod calculated by the multiplicative product of rate modifiers on this stat

        protected Round round = Round.Floor;

        public StatInt(int _baseValue, Round _round = Round.Floor)
        {
            Set(_baseValue, _round);
            initalValue = baseValue;
        }

        public void Set(int _baseValue = 1, Round _round = Round.Floor)
        {
            baseValue = _baseValue;
            round = _round;
        }

        public int value()
        {
            float rate = 1f;

            if (!rates.IsNullOrEmpty()) rates.ForEach(x => rate *= x);

            float v = baseValue * rate;
            int finalValue = (int)(round == Round.Floor ? Math.Floor(v) :
                                    round == Round.Ceil ? Math.Ceiling(v) :
                                    Math.Round(v, 0));

            return finalValue;
        }

        public void Reset()
        {
            baseValue = initalValue;
        }

        public void AddMod(Stat.Mod mod)
        {
            if (mod._rate != 1) rates.Add(mod._rate);
        }

        public void RemoveMod(Stat.Mod mod)
        {
            if (mod._rate != 1) rates.Remove(mod._rate);
        }

        public static implicit operator int(StatInt stat)
        {
            return stat.value();
        }

        public enum Round
        {
            Near,
            Ceil,
            Floor
        }
    }

    [System.Serializable]
    public class StatV2
    {
        public Stat x;
        public Stat y;

        public StatV2(float _x, float _y)
        {
            x = new Stat(_x);
            y = new Stat(_y);
        }

        public StatV2(Vector2 v)
        {
            x = new Stat(v.x);
            y = new Stat(v.y);
        }

        public Vector2 value()
        {
            return new Vector2(x.value(), y.value());
        }

        public void Set(float v)
        {
            x.Set(v);
            y.Set(v);
        }

        public void Set(float _x, float _y)
        {
            x.Set(_x);
            y.Set(_y);
        }

        public void Set(Vector2 v)
        {
            x.Set(v.x);
            y.Set(v.y);
        }

        public void Reset()
        {
            x.Reset();
            y.Reset();
        }

        public void AddMod(Stat.Mod mod)
        {
            x.AddMod(mod);
            y.AddMod(mod);
        }

        public void AddMod(Stat.Mod modX, Stat.Mod modY)
        {
            x.AddMod(modX);
            y.AddMod(modY);
        }

        public void AddMod(Mod mod)
        {
            x.AddMod(mod.x);
            y.AddMod(mod.y);
        }

        public void RemoveMod(Stat.Mod mod)
        {
            x.RemoveMod(mod);
            y.RemoveMod(mod);
        }

        public void RemoveMod(Stat.Mod modX, Stat.Mod modY)
        {
            x.RemoveMod(modX);
            y.RemoveMod(modY);
        }

        public void RemoveMod(Mod mod)
        {
            x.RemoveMod(mod.x);
            y.RemoveMod(mod.y);
        }

        [System.Serializable]
        public class Mod
        {
            public Stat.Mod x;
            public Stat.Mod y;

            public Mod(Vector2 mod)
            {
                x = new Stat.Mod(mod.x);
                y = new Stat.Mod(mod.y);
            }

            public Mod(float mod = 1)
            {
                x = new Stat.Mod(mod);
                y = new Stat.Mod(mod);
            }

            public Mod(float modX = 1, float modY = 1)
            {
                x = new Stat.Mod(modX);
                y = new Stat.Mod(modY);
            }
        }
    }
}
