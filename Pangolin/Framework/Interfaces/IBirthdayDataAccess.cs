using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    public interface IBirthdayDataAccess
    {
        void CreateBirthdayTest(BirthdayTestPoco poco);
        void CreateBirthdayTestDetail(BirthdayTestDetailPoco poco);
    }
}