namespace CIG
{
	public class Purchase<T> where T : StoreProduct
	{
		public T Product
		{
			get;
			private set;
		}

		public string TransactionID
		{
			get;
			private set;
		}

		public bool Validated
		{
			get;
			private set;
		}

		public Purchase(T product, string transactionID, bool validated)
		{
			Product = product;
			TransactionID = transactionID;
			Validated = validated;
		}

		public override string ToString()
		{
			return "[Purchase: TransactionID=" + TransactionID + ", Product=" + Product + "]";
		}
	}
}
