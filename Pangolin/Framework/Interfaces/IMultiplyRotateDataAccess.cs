using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    public interface IMultiplyRotateDataAccess
    {
        void InsertResult(MultiplyRotateResult result);
        bool RowExists(uint multiplier, int rotate);
        void InsertResult(RomulTest test);
        void InsertResult32(RomulTest test);
        bool RomulExists(ulong multiplier, int rotate, ulong seed);

        bool RomulExists32(ulong multiplier, int rotate, ulong seed);
        RomulTest[] GetNextRowThatNeedsLevel14();

        RomulTest[] GetRowsFor16BitFillIn();

        void UpdateLevel14Test(RomulTest test);
        bool RowExists16(int item1, int item2);
        void CreateRomul16Row(RomulTest test);
        void UpdateRomul16Row(RomulTest row);
    }
}