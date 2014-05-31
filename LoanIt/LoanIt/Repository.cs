using System;
using System.IO;
using SQLite;
using LoanIt.Core;
using System.Collections.Generic;
using System.Linq;

namespace LoanIt
{
	public class Repository
	{
		public const string DB_NAME = "db.sqlite";

		protected static Repository instance;

		protected string dbName;
		protected SQLiteConnection conn;

		protected Repository() {}

		public static Repository GetInstance()
		{
			if (Repository.instance == null) {
				Repository.instance = new Repository();
			}
			return Repository.instance;
		}

		~Repository() {
			this.Disconnect();
		}

		public static string GetDbPath()
		{
			return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(), Repository.DB_NAME);
		}

		public SQLiteConnection GetConnection()
		{
			if (this.conn == null) {
				this.conn = new SQLiteConnection(Repository.GetDbPath());
			}
			return this.conn;
		}

		public void Disconnect()
		{
			if (this.conn != null) {
				this.conn.Close();
			}
		}

		public int InsertObject(object item)
		{
			return this.GetConnection().Insert(item);
		}

		public int GetTotalAmount()
		{
			var table = this.GetConnection().Table<Loan>();
			int owedToUser = table.Where(l => l.IsOwedToUser == true).Sum(l => l.Amount);
			Console.WriteLine(String.Format("Owed To: {0}", owedToUser.ToString()));
			int owedByUser = table.Where(l => l.IsOwedToUser == false).Sum(l => l.Amount);
			Console.WriteLine(String.Format("Owed By: {0}", owedByUser.ToString()));
			return owedToUser - owedByUser;
		}

		public Person GetPersonByName(string name)
		{
			var table = this.GetConnection().Table<Person>();
			return (
			    from p in table
				where p.Name == name
				select p
			).FirstOrDefault();
		}

		public Loan[] GetAllLoans()
		{
			var table = this.GetConnection().Table<Loan>();
			return (from l in table
					orderby l.DateAdded descending
					select l).ToArray();
		}

		public Person[] GetAllPeople()
		{
			var table = this.GetConnection().Table<Person>();
			return (from p in table
					orderby p.Name
					select p).ToArray();
		}
	}
}

