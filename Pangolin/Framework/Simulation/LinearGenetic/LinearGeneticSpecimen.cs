using EnderPi.Framework.Random;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    [Serializable]
    public class LinearGeneticSpecimen
    {
        //TODO count without introns 
        public int ProgramLength { get { return _generationProgram.Count+_seedProgram.Count; } }

        public string Name { set; get; }

        public long Fitness { set; get; }

        public int TestsPassed { set; get; }

        public int SpecimenId { set; get; }

        public DateTime Birthday { set; get; }

        public List<Command8099> GenerationProgram { set { _generationProgram = value; } get { return _generationProgram; } }

        public List<Command8099> SeedProgram { set { _seedProgram = value; } get { return _seedProgram; } }

        public int Generation { get; internal set; }

        private List<Command8099> _generationProgram;

        private List<Command8099> _seedProgram;

        private int _generation;

        public Bitmap GetImageBitMap()
        {
            var engine = GetEngine() as Engine;
            engine.Seed(Seed);
            return engine.GetBitMap(4096);
        }

        internal bool IsValid(out string errors)
        {
            errors = null;
            var stateAffectingCommand = _generationProgram.FirstOrDefault(x => x.AffectsState());
            var outputAffectingCommand = _generationProgram.FirstOrDefault(x => x.AffectsOutput());
            //verify one command targets one state, and one command targets output.
            return (stateAffectingCommand != null && outputAffectingCommand != null);
        }

        public string ImageString
        {
            get
            {
                if (_imageString == null)
                {
                    _imageString = GetImageString();
                }
                return _imageString;
            }
        }

        private string _imageString;

        public string GetImageString()
        {
            var bitmap = GetImageBitMap();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Gif);
            var base64Data = Convert.ToBase64String(ms.ToArray());
            return "data:image/gif;base64," + base64Data;
        }

        /// <summary>
        /// Get an Engine from the generator.  
        /// </summary>
        /// <returns></returns>
        public IEngine GetEngine()
        {
            return new LinearGeneticEngine(_generationProgram, _seedProgram);            
        }

        public ulong Seed { set; get; }

    }
}
