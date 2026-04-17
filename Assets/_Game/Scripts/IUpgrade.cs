

public interface IUpgrade
{

    string Name { get;}
    string Description { get;}

    void ApplyUpgrade();
}
