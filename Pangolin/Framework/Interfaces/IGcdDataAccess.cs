using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    public interface IGcdDataAccess
    {
        void CreateGcdTest(GcdTestPoco gcdTest);

        void CreateGcdChiSquared(GcdChiSquaredPoco gcdChiSquaredPoco);
    }
}