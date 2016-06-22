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
using TGModels;

namespace TGModels
{
	public class CONSTANTS
	{
		public static string Start_Button = "/start";
		public static string UserID_Button = "کد شناسائی";
		public static string Signup_Button = "ثبت نام";
		public static string AddBook_Button = "اضافه کردن کتاب";
		public static string WishList_Button = "لیست کتاب های منتخب";
		public static string RemoveBook_Button = "حذف کردن کتاب";
		
		public static string BookEx_Button = "کتاب های منتخب نمایشگاه کتاب";
		public static string AddBook_BookEx_Button = "اضافه کردن کتاب منتخب برای نمایشگاه کتاب";
		public static string YourList_BookEx_Button = "نمایش لیست کتاب های شما برای نمایشگاه کتاب";
		public static string FavoriteList_BookEx_Button = "نمایش لیست کتاب های پر مخاطب نمایشگاه کتاب";
		public static string MainMenu_BookEx_Button = "بازگشت به منوی اصلی";

		public static string TempFilePath = "/Meta-Data/temp.txt" ;
		public static string WelcomeQuote = "به کتابخوانه تحلیلی خوش آمدید.\n\n";
		public static string AskSignUp = "لطفا برای استفاده از خدمات کتابخوانه فرم عضویت را پر کنید.";
		public static string FavoriteList_BookEx = "لیست کتاب های منتخب نمایشگاه کتاب ۲ روز مانده به شروع نمایشگاه منتشر خواهد شد.\nبا ما همراه باشید ...";
		public static string EmptyFavoriteList_BookEx = "لیست کتاب های منتخب شما برای نمایشگاه کتاب خالی میباشد.";
		public static string AddBook_BookEx = "لطفا کتاب خود را با فرمت زیر وارد نمائید\n\nنام کتاب (اجباری)\nنام نویسنده (اجباری)\nانتشارات (اجباری)";
		public static string ExceedAddBook_BookEx = "تعداد کتاب های منتخب شما برای نمایشگاه کتاب از حد مجاز فراتر رفته است و نمیتوانید کتاب بیشتری بیفزایید.";		
		public static string MainMenu_BookEx = "بازگشت به منوی اصلی";
		public static string ContinueWorking = "شما در کتابخوانه تحلیلی عضو میباشید و میتوانید از خدمات کتابخوانه استفاده نمائید.";

		public static string SignUpQuote  = "لطفا اطلاعات خواسته شده را با فرمت زیر وارد نمائید\n\nنام متقاضی (اجباری)\nنام خانوادگی ‌(اجباری)\nشماره کد ملی (اختیاری)\n\nشماره کد ملی صرفا برای استفاده از خدمات گسترده ی کتابخوانه میباشد.\n\nلطفا در صورت خالی گذاشتن فیلد های اختیاری از خط تیره - استفاده کنید.";
		public static string AddBookSignUpQuote = "برای اضافه کردن کتاب به لیست علایقتان ٬ باید ابتدا عضو کتابخوانه تحلیلی بشوید." ;
		public static string AddBookQuote = "لطفا کتاب مورد نظر خود را با فرمت زیر وارد نمائید:\n\nنام کتاب (اجباری)\nنام نویسنده (اجباری)\nانتشارات (اجباری)\nمترجم (اختیاری)\nمبلغ کتاب (اختیاری)\n\nلطفا در صورت خالی گذاشتن فیلد های اختیاری از خط تیره - استفاده کنید.";
		public static string SuccessfullSignUp = "ثبت نام شما با موفقیت به اتمام رسید.\nکد عضویت شما برای استفاده از خدمات کتابخوانه در زیر آمده است.\n\n" ;
		public static string SuccessfullAddBook = "کتاب مورد نظرتان با موفقیت ذخیره شد.";
		public static string WrongInput = "اطلاعات ورودی با فرمت خواسته شده سازگار نمیباشد.\nلطفا به فرمت اطلاعات دقت فرمائید.";
		public static string UserIdQuote = "شماره عضویت شما در کتابخوانه:\n\n";
		public static string EmptyWishBookList = "لیست کتاب های منتخب شما خالی میباشید.";
		public static string DeleteFromWishBookList = "لطفا شماره کتاب مورد نظر برای حذف را وارد نمائید.";
		public static string NoWishBookFound = "کتابی با مشخصات مورد نظر شما پیدا نشد.";
		public static string SuccessfullDeleteBook = "کتاب مورد نظرتان با موفقیت حذف شد.";
		public static string FailedOperation = "عملیات مورد نظر شما ناموفق بود.\nلطفا مجددا سعی کنید.";
	}
}
