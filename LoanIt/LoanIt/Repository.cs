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
			int owedByUser = table.Where(l => l.IsOwedToUser == false).Sum(l => l.Amount);
			return owedToUser - owedByUser;
		}

		public Person GetPersonByName(string name)
		{
			var table = this.GetConnection().Table<Person>();
			return table.Where(p => p.Name == name).FirstOrDefault();
		}

		public Loan[] GetAllLoans()
		{
			var table = this.GetConnection().Table<Loan>();
			return table.OrderByDescending(l => l.DateAdded).ToArray();
		}

		public Person[] GetAllPeople()
		{
			var table = this.GetConnection().Table<Person>();
			return table.OrderBy(p => p.Name).ToArray();
		}
	}
}

