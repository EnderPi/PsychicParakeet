using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    public interface IMultiplyRotateDataAccess
    {
        void InsertResult(MultiplyRotateResult result);
        bool RowExists(uint multiplier, int rotate);
    }
}