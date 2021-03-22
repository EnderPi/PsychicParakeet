namespace EnderPi.Framework.Random
{
    /// <summary>
    /// Wrapper to make an RNG engine thread safe.
    /// </summary>
    public class ThreadsafeEngine : Engine
    {
        private Engine _engine;
        private object _padlock;

        public ThreadsafeEngine(Engine engine)
        {
            _engine = engine;
            _padlock = new object();
        }

        public override ulong Next64()
        {
            lock(_padlock)
            {
                return _engine.Next64();
            }            
        }

        public override void Seed(ulong seed)
        {
            lock(_padlock)
            {
                _engine.Seed(seed);
            }            
        }
    }

}
