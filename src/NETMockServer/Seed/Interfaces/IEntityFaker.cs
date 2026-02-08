namespace NETMockServer.Seed.Interfaces;

public interface IEntityFaker<T> where T : class
{
    T Generate();
}