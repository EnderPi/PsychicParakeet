namespace EnderPi.Framework.DataAccess
{
    public interface IEmpiricalBirthdayDataAccess
    {
        void WriteBirthdaySimulationRecord(int simulationId, ulong seed, int birthdaysInTheYear);
        void WriteDuplicatesForBirthdaySimulation(int simulationId, int iterationNumber, int duplicates);
    }
}