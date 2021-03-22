using EnderPi.Framework.Pocos;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Interface for species name management.
    /// </summary>
    public interface ISpeciesNameDataAccess
    {
        void CreateNewName(string name);
        List<SpeciesName> GetAllNames();
        int GetNameCount();
        SpeciesName GetNameWithNewCounter(int id);
        string GetRandomName();
        IEnumerable<SpeciesName> ReadSpeciesName(SqlCommand command);
    }
}