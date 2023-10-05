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
        protected List<float> mods = new List<float>(); //flat modifiers applied to the base value, usually comes from equipment
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
            float mod = 0;
            float rate = 1f;
            float flat = 0;

            if (!mods.IsNullOrEmpty()) mods.ForEach(x => mod += x);
            if (!rates.IsNullOrEmpty()) rates.ForEach(x => rate *= x);

            float finalValue = ((baseValue + mod) * rate) + flat;

            return finalValue;
        }

        public void Reset()
        {
            baseValue = initalValue;
        }

        public virtual void AddMod(Mod stat)
        {
            if (stat.mod != 0) mods.Add(stat.mod);
            if (stat.rate != 1) rates.Add(stat.rate);
        }

        public virtual void RemoveMod(Mod stat)
        {
            if (stat.mod != 0) mods.Remove(stat.mod);
            if (stat.rate != 1) rates.Remove(stat.rate);
        }

        public static implicit operator float(Stat stat)
        {
            return stat.value();
        }

        [System.Serializable]
        public class Mod
        {
            public Mod(float _rate = 1, float _mod = 0, float _flat = 0)
            {
                rate = _rate;
                mod = _mod;
                flat = _flat;
            }

            public float rate = 1;
            public float mod = 0;
            public float flat = 0;
        }
    }

    [System.Serializable]
    public class StatInt
    {
        [SerializeField]
        protected int baseValue = 1; //Add the ability to base this value off a formula that scales with level (?)

        protected int initalValue = 1;
        protected List<int> mods = new List<int>(); //Flat modifiers applied to the base value, usually comes from equipment
        protected List<float> rates = new List<float>(); // % mod calculated by the multiplicative product of rate modifiers on this stat
        protected List<int> flats = new List<int>(); //Final additive mod to the final value (doesn't scale with rate and buffs)

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
            int mod = 0;
            float rate = 1f;
            int flat = 0;

            if (!mods.IsNullOrEmpty()) mods.ForEach(x => mod += x);
            if (!rates.IsNullOrEmpty()) rates.ForEach(x => rate *= x);
            if (!flats.IsNullOrEmpty()) flats.ForEach(x => flat += x);

            float v = (baseValue + mod) * rate;
            int finalValue = (int)(round == Round.Floor ? Math.Floor(v) :
                                    round == Round.Ceil ? Math.Ceiling(v) :
                                    Math.Round(v, 0));

            finalValue += flat;

            return finalValue;
        }

        public void Reset()
        {
            baseValue = initalValue;
        }

        public void AddMod(Mod stat)
        {
            if (stat.mod != 0) mods.Add(stat.mod);
            if (stat.rate != 1) rates.Add(stat.rate);
            if (stat.flat != 0) flats.Add(stat.flat);
        }

        public void RemoveMod(Mod stat)
        {
            if (stat.mod != 0) mods.Remove(stat.mod);
            if (stat.rate != 1) rates.Remove(stat.rate);
            if (stat.flat != 0) flats.Remove(stat.flat);
        }

        public static implicit operator int(StatInt stat)
        {
            return stat.value();
        }

        [System.Serializable]
        public class Mod
        {
            public Mod(float _rate = 1, int _mod = 0, int _flat = 0)
            {
                rate = _rate;
                mod = _mod;
                flat = _flat;
            }

            public float rate = 1;
            public int mod = 0;
            public int flat = 0;
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

        public void AddMod(Stat.Mod stat)
        {
            x.AddMod(stat);
            y.AddMod(stat);
        }

        public void AddMod(Stat.Mod statX, Stat.Mod statY)
        {
            x.AddMod(statX);
            y.AddMod(statY);
        }

        public void AddMod(Mod stat)
        {
            x.AddMod(stat.x);
            y.AddMod(stat.y);
        }

        public void RemoveMod(Stat.Mod stat)
        {
            x.RemoveMod(stat);
            y.RemoveMod(stat);
        }

        public void RemoveMod(Stat.Mod statX, Stat.Mod statY)
        {
            x.RemoveMod(statX);
            y.RemoveMod(statY);
        }

        public void RemoveMod(Mod stat)
        {
            x.RemoveMod(stat.x);
            y.RemoveMod(stat.y);
        }

        [System.Serializable]
        public class Mod
        {
            public Stat.Mod x;
            public Stat.Mod y;

            public Mod(float rate = 1, float modX = 0, float modY = 0, float flatX = 0, float flatY = 0)
            {
                x = new Stat.Mod(rate, modX, flatX);
                y = new Stat.Mod(rate, modY, flatY);
            }

            public Mod(float rate, Vector2 mod)
            {
                x = new Stat.Mod(rate, mod.x);
                y = new Stat.Mod(rate, mod.y);
            }

            public Mod(float rate, Vector2 mod, Vector2 flat)
            {
                x = new Stat.Mod(rate, mod.x, flat.x);
                y = new Stat.Mod(rate, mod.y, flat.y);
            }

            public Mod(Vector2 mod)
            {
                x = new Stat.Mod(1, mod.x);
                y = new Stat.Mod(1, mod.y);
            }

            public Mod(Vector2 mod, Vector2 flat)
            {
                x = new Stat.Mod(1, mod.x, flat.x);
                y = new Stat.Mod(1, mod.y, flat.y);
            }
        }
    }
}
