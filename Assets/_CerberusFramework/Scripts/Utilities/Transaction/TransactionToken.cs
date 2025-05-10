namespace CerberusFramework.Utilities.Transaction
{
    public class TransactionToken<T>
    {
        public T Data;
        public long Id;

        public TransactionToken(T data, long id)
        {
            Data = data;
            Id = id;
        }
    }
}