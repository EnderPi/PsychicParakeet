using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    public class RomulTest
    {
        public int Id { set; get; }
        public ulong Multiplier { set; get; }
        public int Rotate { set; get; }

        public ulong Seed { set; get; }               

        public long LevelOneFitness { set; get; }
        public long LevelTwoFitness { set; get; }
        public long LevelThreeFitness { set; get; }

        public long LevelTwelveFitness { set; get; }

        public long LevelThirteenFitness { set; get; }
        public long LevelFourteenFitness { set; get; }

        public long GcdFitness { set; get; }
        public long Gorilla8Fitness { set; get; }
        public long Gorilla16Fitness { set; get; }
        public long BirthdayFitness { set; get; }
        public long Maurer16Fitness { set; get; }
        public long Maurer8Fitness { set; get; }

    }
}
