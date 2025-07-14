namespace UnitySC.DataAccess.Base
{
    public interface IUnitOfWorkBase
    {
        void Save();

        string GetConnectionString();
    }
}
