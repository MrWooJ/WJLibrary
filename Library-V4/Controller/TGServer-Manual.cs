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
using Newtonsoft.Json;
using TGModels;
using TGController;
using TGFileManager;
using TGServerFileUploader;
using TGDataBaseManager;

public enum KEYBOARDTYPE
{
	REPLAYKEYBOARDMARKUP = 1,
	REPLAYKEYBOARDHIDE = 2,
	FORCEREPLAY = 3,
	BOOKEXBUTTON = 4
}

namespace TGServer
{
	public class TGServer
	{
		public static string apiToken 	= "147824448:AAGZaeLRMuRtNjtgvsVJAj3QfxsD3qOKDlM";
		public static string baseURL	= "https://api.telegram.org/bot";

		public static string tempFileFullPath = "null";

		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Error in Input Arguments! " + args.Length + " are Entered!");
				System.Environment.Exit(1);
			}
			
			tempFileFullPath = args[0] + CONSTANTS.TempFilePath;
			GetUpdatesManually();
		}

		public static void GetUpdatesResponseHandler(RESPONSEUPDATE res)
		{
						
			if (!res.ok.ToLower().Equals("true"))
			{
				Console.WriteLine("Error in Response Object With ErrorCode: " + res.error_code + " Descrpition: " + res.description);
			}
			else
			{		
				if (res.result.Length == 0)
					return;
				
				foreach (UPDATE update in res.result)
				{
					Console.WriteLine(update.update_id.ToString());
					FileManager.WriteInTempFile(tempFileFullPath, update.update_id.ToString());	
					MESSAGE msg = update.message;

					if (msg.text.ToLower().Equals(CONSTANTS.Start_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.WelcomeQuote + CONSTANTS.ContinueWorking, "Markdown", true, 0, keyboardType);
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.WelcomeQuote + CONSTANTS.AskSignUp, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Equals(CONSTANTS.Signup_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.ContinueWorking, "Markdown", true, 0, keyboardType);
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Equals(CONSTANTS.AddBook_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBookQuote, "Markdown", true, 0, keyboardType);
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBookSignUpQuote, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.BookEx_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							DBManager.CheckUserExistsInExibitionBookTable(msg.from.id.ToString());
							List<NameValueCollection> list = DBManager.GetEnteredExibitionBooks(msg.from.id.ToString());
							string tempmsg = String.Format("شما تاکنون {0} کتاب به لیست کتاب های مورد علاقه ی خود برای نمایشگاه کتاب افزوده اید.", list.Count);
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.BOOKEXBUTTON);
							Controller.SendMessage(msg.from.id.ToString(), tempmsg, "Markdown", true, 0, keyboardType);
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Equals(CONSTANTS.AddBook_BookEx_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							DBManager.CheckUserExistsInExibitionBookTable(msg.from.id.ToString());
							List<NameValueCollection> list = DBManager.GetEnteredExibitionBooks(msg.from.id.ToString());
							if (list.Count < 5)
							{
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
								Console.WriteLine("Addbook_BookEx is Firing");
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBook_BookEx, "Markdown", true, 0, keyboardType);
							}
							else
							{
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.BOOKEXBUTTON);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.ExceedAddBook_BookEx, "Markdown", true, 0, keyboardType);
							}
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.YourList_BookEx_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							DBManager.CheckUserExistsInExibitionBookTable(msg.from.id.ToString());
							List<NameValueCollection> list = DBManager.GetEnteredExibitionBooks(msg.from.id.ToString());
							if (list.Count > 0)
							{
								string bookListString = null;
								foreach (NameValueCollection nav in list)
								{
									Console.WriteLine("BookName: " + nav.Get("bookname"));
									bookListString = bookListString + "کتاب: " + nav.Get("bookname") + "\n";
									bookListString = bookListString + "نویسنده: " + nav.Get("authorname") + "\n";
									bookListString = bookListString + "انتشارات: " + nav.Get("publisher") + "\n";
									bookListString = bookListString + "--------------" + "\n";
								}
								bookListString = bookListString + string.Format("\n{0} کتاب یافت شدند.", list.Count.ToString());
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.BOOKEXBUTTON);
								Controller.SendMessage(msg.from.id.ToString(), bookListString, "Markdown", true, 0, keyboardType);
							}
							else
							{	
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.BOOKEXBUTTON);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.EmptyFavoriteList_BookEx, "Markdown", true, 0, keyboardType);
							}
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.FavoriteList_BookEx_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.BOOKEXBUTTON);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.FavoriteList_BookEx, "Markdown", true, 0, keyboardType);
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.MainMenu_BookEx_Button))
					{
						string keyboardType = null;
						keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
						Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.MainMenu_BookEx, "Markdown", true, 0, keyboardType);
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.WishList_Button))
					{
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							string keyboardType = null;

							List<NameValueCollection> list = DBManager.WishBookList(msg.from.id.ToString());

							if (list.Count == 0)
							{
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.EmptyWishBookList, "Markdown", true, 0, keyboardType);
								return;
							}
							string bookListString = null;
							foreach (NameValueCollection nav in list)
							{
								Console.WriteLine("BookName: " + nav.Get("bookname"));
								bookListString = bookListString + "شماره کتاب: " + nav.Get("wishbookid") + "\n";
								bookListString = bookListString + "کتاب: " + nav.Get("bookname") + "\n";
								bookListString = bookListString + "نویسنده: " + nav.Get("authorname") + "\n";
								bookListString = bookListString + "انتشارات: " + nav.Get("publisher") + "\n";
								bookListString = bookListString + "مترجم: " + nav.Get("translator") + "\n";
								bookListString = bookListString + "قیمت: " + nav.Get("price") + "\n";
								bookListString = bookListString + "-----------------" + "\n";
							}
							bookListString = bookListString + string.Format("\n{0} کتاب یافت شدند.", list.Count.ToString());
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), bookListString, "Markdown", true, 0, keyboardType);
						}
						else
						{
							string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AskSignUp, "Markdown", true, 0, keyboardType);
						}
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.UserID_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.UserIdQuote + msg.from.id, "Markdown", true, 0, keyboardType);
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AskSignUp, "Markdown", true, 0, keyboardType);
						}

					}
					else if (msg.text.Equals("/admin MrWooJ:#10AliDelWJ(Piero)*"))
					{
						string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
						string fakeData = "Library UpTime:\n103 Days, 17h : 36m : 42s\nBook Exibition UpTime:\n1 Days, 2h : 41m : 02s\n\nRegister Users: 169\nBooks Count: 413\nAverage BPU: ~2.44\n\nTop 5 Hits Books:\n";
						string book1 = "جاودانگی" + "\n" + "میلان کوندرا" + "\n----------\n";
						string book2 = "دوست داشتم کسی جایی منتظرم باشد" + "\n" + "آنا گاوالدا" +"\n----------\n";
						string book3 = "نامه ای به کودکی که هرگز زاده نشد" + "\n" + "اوریانا فالاچی" +"\n----------\n";
						string book4 = "کلیدر" + "\n" + "محمود دولت آبادی" +"\n----------\n";
						string book5 = "کیمیاگر" + "\n" + "پائولو کوئیلو";
						string sent = fakeData + book1 + book2 + book3 + book4 + book5;
						Controller.SendMessage(msg.from.id.ToString(), sent, "Markdown", true, 0, keyboardType);
					}
					else if (msg.text.ToLower().Contains(CONSTANTS.RemoveBook_Button))
					{
						string keyboardType = null;
						if (DBManager.CheckUserRegistered(msg.from.id.ToString()))
						{
							bool check = DBManager.IsWishListEmpty(msg.from.id.ToString());
							if (check)
							{
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.EmptyWishBookList, "Markdown", true, 0, keyboardType);	
							}
							else
							{
								keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.DeleteFromWishBookList, "Markdown", true, 0, keyboardType);							
							}
						}
						else
						{
							keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
							Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AskSignUp, "Markdown", true, 0, keyboardType);
						}
					}
					if (msg.reply_to_message.from.username.Equals("AnalyticalLibrary_Bot"))
					{
						if (msg.reply_to_message.text.Equals(CONSTANTS.SignUpQuote))
						{
							string[] separators = {"\n"};
							string[] words = msg.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
							if (words.Length == 3)
							{
								bool check = DBManager.RegisterUser(words[0], words[1], words[2], msg.from.id.ToString());
								if (check)
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SuccessfullSignUp + msg.from.id, "Markdown", true, 0, keyboardType);
								}
								else
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.FailedOperation, "Markdown", true, 0, keyboardType);
									keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
								}
							}
							else
							{
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.WrongInput, "Markdown", true, 0, null);
								string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SignUpQuote, "Markdown", true, 0, keyboardType);
							}
						}
						else if (msg.reply_to_message.text.Equals(CONSTANTS.AddBook_BookEx))
						{
							string[] separators = {"\n"};
							string[] words = msg.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
							if (words.Length == 3)
							{
								bool check = DBManager.AddBookToExibitionBook(words[0], words[1], words[2], msg.from.id.ToString());
								if (check)
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.BOOKEXBUTTON);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SuccessfullAddBook, "Markdown", true, 0, keyboardType);
								}
								else
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.FailedOperation, "Markdown", true, 0, keyboardType);
									keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBook_BookEx, "Markdown", true, 0, keyboardType);
								}
							}
							else
							{
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.WrongInput, "Markdown", true, 0, null);
								string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBook_BookEx, "Markdown", true, 0, keyboardType);
							}
						}
						else if (msg.reply_to_message.text.Equals(CONSTANTS.AddBookQuote))
						{
							string[] separators = {"\n"};
							string[] words = msg.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

							if (words.Length == 5)
							{
								bool check = DBManager.AddToWishBook(words[0], words[1], words[2], words[3], words[4], msg.from.id.ToString());
								if (check)
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SuccessfullAddBook, "Markdown", true, 0, keyboardType);
								}
								else
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.FailedOperation, "Markdown", true, 0, keyboardType);
									keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBookQuote, "Markdown", true, 0, keyboardType);
								}
							}
							else
							{
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.WrongInput, "Markdown", true, 0, null);
								string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.AddBookQuote, "Markdown", true, 0, keyboardType);
							}			
						}
						else if (msg.reply_to_message.text.Equals(CONSTANTS.DeleteFromWishBookList))
						{
							string[] separators = {"\n"};
							string[] words = msg.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

							if (words.Length == 1)
							{
								bool check = DBManager.RemoveFromWishBook(words[0], msg.from.id.ToString());
								if (check)
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.SuccessfullDeleteBook, "Markdown", true, 0, keyboardType);
								}
								else
								{
									string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.REPLAYKEYBOARDMARKUP);
									Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.NoWishBookFound, "Markdown", true, 0, keyboardType);
								}
							}
							else
							{
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.WrongInput, "Markdown", true, 0, null);
								string keyboardType = CreateInterfaceKeyboards(KEYBOARDTYPE.FORCEREPLAY);
								Controller.SendMessage(msg.from.id.ToString(), CONSTANTS.DeleteFromWishBookList, "Markdown", true, 0, keyboardType);
							}							
						}
					}
				}
			}
		}
		
		public static void GetUpdatesManually()
		{
			while (true)
			{
				WebRequest req = null;
				try
				{						
					string lastOffset = FileManager.ReadFromTempFile(tempFileFullPath);
					if (lastOffset.Length == 0)
					{
						req = WebRequest.Create(baseURL + apiToken + "/getUpdates");
						//Console.WriteLine("URL: " + baseURL + apiToken + "/getUpdates");
					}
						
					else
					{
						decimal decimalVal = 0;
						decimalVal = System.Convert.ToDecimal(lastOffset) + 1;
						string offset = decimalVal.ToString();
						req = WebRequest.Create(baseURL + apiToken + "/getUpdates?offset=" + offset);
						//Console.WriteLine("URL: " + baseURL + apiToken + "/getUpdates?offset=" + offset);
					}
					
					req.UseDefaultCredentials = true;
					
					var result = req.GetResponse();
					Stream stream = result.GetResponseStream();
					StreamReader reader = new StreamReader(stream);
					
					RESPONSEUPDATE res = JsonConvert.DeserializeObject<RESPONSEUPDATE>(reader.ReadToEnd());
					
					GetUpdatesResponseHandler(res);

					req.Abort();

				}
				catch (Exception ex)
				{
					Console.WriteLine("Error Getting Updates");
					req.Abort();
				} 
				finally 
				{
					req.Abort();
				}
			}
		}

		public static string CreateInterfaceKeyboards(KEYBOARDTYPE keyType)
		{
			if (keyType == KEYBOARDTYPE.REPLAYKEYBOARDMARKUP)
			{
				REPLAYKEYBOARDMARKUP keyInterface = new REPLAYKEYBOARDMARKUP();
				keyInterface.keyboard = new string[4, 2] {{CONSTANTS.UserID_Button , CONSTANTS.Signup_Button},{CONSTANTS.AddBook_Button , CONSTANTS.RemoveBook_Button},{CONSTANTS.WishList_Button , ""},{CONSTANTS.BookEx_Button , ""}};
				keyInterface.resize_keyboard = false;
				keyInterface.one_time_keyboard = false;
				keyInterface.selective = false;
				string json = JsonConvert.SerializeObject(keyInterface);
				return json;
			}
			else if (keyType == KEYBOARDTYPE.REPLAYKEYBOARDHIDE)
			{
				REPLAYKEYBOARDHIDE keyInterface = new REPLAYKEYBOARDHIDE();
				keyInterface.hide_keyboard = true;
				keyInterface.selective = false;
				string json = JsonConvert.SerializeObject(keyInterface);
				return json;
			}
			else if (keyType == KEYBOARDTYPE.FORCEREPLAY)
			{
				FORCEREPLAY keyInterface = new FORCEREPLAY();
				keyInterface.force_reply = true;
				keyInterface.selective = false;
				string json = JsonConvert.SerializeObject(keyInterface);
				return json;
			}
			else if (keyType == KEYBOARDTYPE.BOOKEXBUTTON)
			{
				REPLAYKEYBOARDMARKUP keyInterface = new REPLAYKEYBOARDMARKUP();
				keyInterface.keyboard = new string[4, 1] {{CONSTANTS.AddBook_BookEx_Button},{CONSTANTS.YourList_BookEx_Button},{CONSTANTS.FavoriteList_BookEx_Button},{CONSTANTS.MainMenu_BookEx_Button}};
				keyInterface.resize_keyboard = false;
				keyInterface.one_time_keyboard = false;
				keyInterface.selective = false;
				string json = JsonConvert.SerializeObject(keyInterface);
				return json;
			}
			return null;
		}
	}
}
