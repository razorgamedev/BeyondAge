namespace BeyondAge.Entities
{
    interface IFrame
    {
        bool Equals(object obj);
        int GetHashCode();
        string ToString();
    }
}