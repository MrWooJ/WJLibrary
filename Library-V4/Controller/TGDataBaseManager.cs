using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using MySql.Data.MySqlClient; 

namespace TGDataBaseManager
{
	public class DBManager
	{
		public static string connectionString = @"server=localhost;userid=root;password=root;database=ALdb;Charset=utf8";

		public static MySqlConnection connection = null;
		public static MySqlDataReader dataReader = null;
		public static MySqlTransaction Transaction = null; 

		public static bool CheckUserRegistered(string userId)
		{
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				string stm = "SELECT UserIdentifier FROM PersonTable";
				MySqlCommand cmd = new MySqlCommand(stm, connection);
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read())
					if (((string)dataReader["UserIdentifier"]).Equals(userId))
						return true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
			return false;
		}

		public static bool RegisterUser(string firstname, string lastname, string nationalCode, string userId)
		{
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = connection;

				cmd.CommandText = "INSERT INTO PersonTable(FirstName, LastName, NationalCode, UserIdentifier) VALUES(@FirstName, @LastName, @NationalCode, @UserIdentifier)";
				cmd.Prepare();

				cmd.Parameters.AddWithValue("@FirstName", firstname);
				cmd.Parameters.AddWithValue("@LastName", lastname);
				cmd.Parameters.AddWithValue("@NationalCode", nationalCode);
				cmd.Parameters.AddWithValue("@UserIdentifier", userId);
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
				return false;
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
		}

		public static bool AddToWishBook(string bookName, string authorName, string publisher, string translator, string price, string userId)
		{
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = connection;

				cmd.CommandText = "INSERT INTO WishBookTable(BookName, AuthorName, Publisher, Translator, Price, PersonIdentifier) VALUES(@BookName, @AuthorName, @Publisher, @Translator, @Price, @PersonIdentifier)";
				cmd.Prepare();

				cmd.Parameters.AddWithValue("@BookName", bookName);
				cmd.Parameters.AddWithValue("@AuthorName", authorName);
				cmd.Parameters.AddWithValue("@Publisher", publisher);
				cmd.Parameters.AddWithValue("@Translator", translator);
				cmd.Parameters.AddWithValue("@Price", price);
				cmd.Parameters.AddWithValue("@PersonIdentifier", userId);
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
				return false;
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
		}

		public static bool RemoveFromWishBook(string bookNumber, string userId)
		{
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				
				string stm = "SELECT COUNT(*) AS COUNT FROM WishBookTable WHERE WishBookId="+bookNumber+ " AND " + "PersonIdentifier="+userId;
				MySqlCommand cmd = new MySqlCommand(stm, connection);
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read())
					if (dataReader["COUNT"].ToString().Equals("0"))
					{
						dataReader.Close();
						return false;
					}
				dataReader.Close();
				
				MySqlCommand cmd1 = new MySqlCommand();
				cmd1.Connection = connection;

				cmd1.CommandText = "DELETE FROM WishBookTable WHERE WishBookId="+bookNumber+ " AND " + "PersonIdentifier="+userId;
				cmd1.ExecuteNonQuery();
				return true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
				return false;
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
		}

		public static bool IsWishListEmpty(string userId)
		{
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				string stm = "SELECT COUNT(PersonIdentifier) AS COUNT FROM WishBookTable WHERE PersonIdentifier=" + userId;
				MySqlCommand cmd = new MySqlCommand(stm, connection);
				dataReader = cmd.ExecuteReader();
				
				while (dataReader.Read())
					if (dataReader["COUNT"].ToString().Equals("0"))
						return true;
					return false;					
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
			return false;
		}

		public static List<NameValueCollection> WishBookList(string userId)
		{
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				string stm = "SELECT BookName, AuthorName, Publisher, Translator, Price, WishBookId FROM WishBookTable WHERE PersonIdentifier=" + userId;
				MySqlCommand cmd = new MySqlCommand(stm, connection);
				dataReader = cmd.ExecuteReader();

				List<NameValueCollection> list = new List<NameValueCollection>();
				while (dataReader.Read())
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("bookname", dataReader.GetString(0));
					nvc.Add("authorname", dataReader.GetString(1));
					nvc.Add("publisher", dataReader.GetString(2));
					nvc.Add("translator", dataReader.GetString(3));
					nvc.Add("price", dataReader.GetString(4));
					nvc.Add("wishbookid", dataReader.GetString(5));
					list.Add(nvc);
				}
				return list;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
				return null;
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
			return null;
		}



		public static List<NameValueCollection> GetEnteredExibitionBooks(string userId)
		{
			List<NameValueCollection> list = new List<NameValueCollection>();
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				string stm = "SELECT BookName1, Author1, Publisher1, BookName2, Author2, Publisher2, BookName3, Author3, Publisher3, BookName4, Author4, Publisher4, BookName5, Author5, Publisher5 FROM BookExibitionV1 WHERE PersonIdentifier=" + userId;
				MySqlCommand cmd = new MySqlCommand(stm, connection);
				dataReader = cmd.ExecuteReader();
				dataReader.Read();
				//NameValueCollection nvc = new NameValueCollection();

				if (!dataReader.GetString(0).Equals("NULL"))
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("bookname", dataReader.GetString(0));
					nvc.Add("authorname", dataReader.GetString(1));
					nvc.Add("publisher", dataReader.GetString(2));
					list.Add(nvc);		
				}
				if (!dataReader.GetString(3).Equals("NULL"))
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("bookname", dataReader.GetString(3));
					nvc.Add("authorname", dataReader.GetString(4));
					nvc.Add("publisher", dataReader.GetString(5));
					list.Add(nvc);		
				}
				if (!dataReader.GetString(6).Equals("NULL"))
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("bookname", dataReader.GetString(6));
					nvc.Add("authorname", dataReader.GetString(7));
					nvc.Add("publisher", dataReader.GetString(8));
					list.Add(nvc);		
				}
				if (!dataReader.GetString(9).Equals("NULL"))
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("bookname", dataReader.GetString(9));
					nvc.Add("authorname", dataReader.GetString(10));
					nvc.Add("publisher", dataReader.GetString(11));
					list.Add(nvc);		
				}
				if (!dataReader.GetString(12).Equals("NULL"))
				{
					NameValueCollection nvc = new NameValueCollection();
					nvc.Add("bookname", dataReader.GetString(12));
					nvc.Add("authorname", dataReader.GetString(13));
					nvc.Add("publisher", dataReader.GetString(14));
					list.Add(nvc);		
				}
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
				list =  null;
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
			return list;
		}

		public static bool AddBookToExibitionBook(string bookName, string authorName, string publisherName, string userId)
		{
			int numberOfBooks = DBManager.GetEnteredExibitionBooks(userId).Count;
			if (numberOfBooks == 5)
			return false;
			numberOfBooks++;
			bool val = false;
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				
				MySqlCommand cmd1 = new MySqlCommand();
				cmd1.Connection = connection;

				string book = "BookName"+numberOfBooks.ToString()+"=";
				string author = "Author"+numberOfBooks.ToString()+"=";
				string publisher = "Publisher"+numberOfBooks.ToString()+"=";

				string minQuery = book + "\"" + bookName + "\"" + " , " + author + "\"" + authorName + "\"" + " , " + publisher + "\"" + publisherName + "\"";
				string query = "UPDATE BookExibitionV1 SET "+ minQuery + " WHERE PersonIdentifier=" + userId;
				Console.WriteLine("Add EXBOOK Query: " +query);
				byte[] bytes = Encoding.Default.GetBytes(query);
				string query1 = Encoding.UTF8.GetString(bytes);

				cmd1.CommandText = query1;
				cmd1.ExecuteNonQuery();
				val = true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error Set Personality: {0}",  ex.ToString());
				val = false;
			}
			finally 
			{
				if (dataReader != null) 
				dataReader.Close();

				if (connection != null)
				connection.Close();
			}
			return val;

		}

		public static bool AddUserToExibitionBook(string userId)
		{
			bool check = false;
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = connection;

				cmd.CommandText = "INSERT INTO BookExibitionV1(PersonIdentifier, BookName1, Author1, Publisher1, BookName2, Author2, Publisher2, BookName3, Author3, Publisher3, BookName4, Author4, Publisher4, BookName5, Author5, Publisher5) VALUES(@PersonIdentifier, @BookName1, @Author1, @Publisher1, @BookName2, @Author2, @Publisher2, @BookName3, @Author3, @Publisher3, @BookName4, @Author4, @Publisher4, @BookName5, @Author5, @Publisher5)";
				cmd.Prepare();

				cmd.Parameters.AddWithValue("@PersonIdentifier", userId);
				cmd.Parameters.AddWithValue("@BookName1", "NULL");
				cmd.Parameters.AddWithValue("@Author1", "NULL");
				cmd.Parameters.AddWithValue("@Publisher1", "NULL");
				cmd.Parameters.AddWithValue("@BookName2", "NULL");
				cmd.Parameters.AddWithValue("@Author2", "NULL");
				cmd.Parameters.AddWithValue("@Publisher2", "NULL");
				cmd.Parameters.AddWithValue("@BookName3", "NULL");
				cmd.Parameters.AddWithValue("@Author3", "NULL");
				cmd.Parameters.AddWithValue("@Publisher3", "NULL");
				cmd.Parameters.AddWithValue("@BookName4", "NULL");
				cmd.Parameters.AddWithValue("@Author4", "NULL");
				cmd.Parameters.AddWithValue("@Publisher4", "NULL");
				cmd.Parameters.AddWithValue("@BookName5", "NULL");
				cmd.Parameters.AddWithValue("@Author5", "NULL");
				cmd.Parameters.AddWithValue("@Publisher5", "NULL");

				cmd.ExecuteNonQuery();
				check = true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}",  ex.ToString());
				check = false;
			}
			finally 
			{
				if (dataReader != null) 
					dataReader.Close();

				if (connection != null)
					connection.Close();
			}
			return check;
		}

		public static bool CheckUserExistsInExibitionBookTable(string userId)
		{
			bool val = false;
			try
			{
				connection = new MySqlConnection(connectionString);
				connection.Open();
				string stm = "SELECT PersonIdentifier AS PID FROM BookExibitionV1 WHERE PersonIdentifier=" + userId;
				MySqlCommand cmd = new MySqlCommand(stm, connection);
				dataReader = cmd.ExecuteReader();

				if (dataReader.HasRows)
				{
					val = true;
				}
				else
				{
					val = DBManager.AddUserToExibitionBook(userId);
				}
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error Check UserExists: {0}",  ex.ToString());
				val = false;
			}
			finally 
			{
				if (dataReader != null) 
				dataReader.Close();

				if (connection != null)
				connection.Close();
			}
			return val;
		}
	}
}
