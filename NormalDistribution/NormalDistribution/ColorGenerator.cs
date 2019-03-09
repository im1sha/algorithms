using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormalDistribution
{

    public class ColorGenerator
    {
        // 896 = 256 * 7 / 2 
        private const int TOTAL_PATTERNS = 7;
        private const int TOTAL_COLORS_PER_CHANNEL = 256;
        private const int HEX_BASE = 16;

        private int index = 0;
        private IntensityGenerator intensityGenerator = new IntensityGenerator();

        public string NextColor()
        {
            string colour = string.Format(PatternGenerator.NextPattern(index),
                intensityGenerator.NextIntensity(index));
            index++;
            return colour;
        }

        public uint NextColorAsUInt()
        {           
            return Convert.ToUInt32("0xFF" + NextColor(), HEX_BASE);
        }

        private class PatternGenerator
        {
            public static string NextPattern(int index)
            {
                switch (index % TOTAL_PATTERNS)
                {
                    case 0: return "{0}0000";
                    case 1: return "00{0}00";
                    case 2: return "0000{0}";
                    case 3: return "{0}{0}00";
                    case 4: return "{0}00{0}";
                    case 5: return "00{0}{0}";
                    case 6: return "{0}{0}{0}";
                    default: throw new Exception("Math error");
                }
            }
        }

        private class IntensityGenerator
        {
           
            private IntensityValueWalker walker;
            private int current;

            public string NextIntensity(int index)
            {
                if (index == 0)
                {
                    current = TOTAL_COLORS_PER_CHANNEL - 1;
                }
                else if (index % TOTAL_PATTERNS == 0)
                {
                    if (walker == null)
                    {
                        walker = new IntensityValueWalker();
                    }
                    else
                    {
                        walker.MoveNext();
                    }
                    current = walker.Current.Value;
                }
                string currentText = current.ToString("X");
                if (currentText.Length == 1)
                {
                    currentText = "0" + currentText;
                }
                return currentText;
            }
        }

        private class IntensityValue
        {

            private IntensityValue mChildA;
            private IntensityValue mChildB;

            public IntensityValue(IntensityValue parent, int value, int level)
            {
                if (level > TOTAL_PATTERNS)
                {
                    throw new Exception("There are no more colours left");
                }
                Value = value;
                Parent = parent;
                Level = level;
            }

            public int Level { get; set; }
            public int Value { get; set; }
            public IntensityValue Parent { get; set; }

            public IntensityValue ChildA
            {
                get
                {
                    return mChildA ?? (mChildA = new IntensityValue(this, 
                        Value - (1 << (TOTAL_PATTERNS - Level)), Level + 1));
                }
            }

            public IntensityValue ChildB
            {
                get
                {
                    return mChildB ?? (mChildB = new IntensityValue(this, 
                        Value + (1 << (TOTAL_PATTERNS - Level)), Level + 1));
                }
            }
        }

        private class IntensityValueWalker
        {

            public IntensityValueWalker()
            {
                Current = new IntensityValue(null, 1 << TOTAL_PATTERNS, 1);
            }

            public IntensityValue Current { get; set; }

            public void MoveNext()
            {
                if (Current.Parent == null)
                {
                    Current = Current.ChildA;
                }
                else if (Current.Parent.ChildA == Current)
                {
                    Current = Current.Parent.ChildB;
                }
                else
                {
                    int levelsUp = 1;
                    Current = Current.Parent;
                    while (Current.Parent != null && Current == Current.Parent.ChildB)
                    {
                        Current = Current.Parent;
                        levelsUp++;
                    }
                    if (Current.Parent != null)
                    {
                        Current = Current.Parent.ChildB;
                    }
                    else
                    {
                        levelsUp++;
                    }
                    for (int i = 0; i < levelsUp; i++)
                    {
                        Current = Current.ChildA;
                    }
                }
            }
        }
    }  
}
