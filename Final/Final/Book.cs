﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Final
{
    class Book
    {
        private string _Title;
        private string _Author;
        private string _Edition;
        private double _Price;
        private string _ISBN;
        //private byte[] _CoverImage;
        private int _inCart;
        private int _Stock;
        public DatabaseConnection conn = new DatabaseConnection();

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        public string Author
        {
            get
            {
                return _Author;
            }
            set
            {
                _Author = value;
            }
        }
        public string Edition
        {
            get
            {
                return _Edition;
            }
            set
            {
                _Edition = value;
            }
        }
        public double Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
            }
        }

        public string ISBN
        {
            get
            {
                return _ISBN;
            }
            set
            {
                if (value=="")
                {
                    MessageBox.Show("ISBN required!");
                }
                bool isbnchecker = ISBN_Cheker(value);//check the given isbn is valid
                if (isbnchecker==true)
                {
                    _ISBN = value;
                }
                else
                {
                    MessageBox.Show("Invalid ISBN. Please enter a valid 10 digit ISBN-10 number.");
                }
            }
        }
        public int InCart
        {
            get
            {
                return _inCart;
            }
            set
            {
                _inCart = value;
            }
        }
        public int Stock
        {
            get
            {
                return _Stock;
            }
            set
            {
                _Stock = value;
            }
        }

        public Boolean ADD_Book(string title,string author,string edition,double price,string isbn, int inCart, int stock)//adds book to database
        {
            Boolean okay = false;

            try
            {  
                using (var connection = conn.con)
                {
                    connection.Open();
                    //SqlCommand cmd = new SqlCommand("insert into Books (Title,Author,Edition,Price,ISBN,InCart,Stock) values('" + @title + "','" + @author + "',@edition,'" + @price + "','" + @isbn + "','" + inCart + "','" + @stock + "')", connection);
                    SqlCommand cmd = new SqlCommand("insert into Books (Title,Author,Edition,Price,ISBN,InCart,Stock) values(@title,@author,@edition,@price,@isbn,@inCart,@stock)", connection);
                    //replace placeholders with values
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@edition", edition);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@isbn", isbn);
                    cmd.Parameters.AddWithValue("@inCart", inCart);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.ExecuteNonQuery();
                    conn.con.Close();
                    MessageBox.Show("Book Registered");
                    okay = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex);
            }
            return okay;
        }

        public bool ISBN_Cheker(string isbn)//checks if isbn is a valid isbn 10 number
        {
            isbn = isbn.Replace("-","");
            if(isbn == "")
            {
                return false;
            }
            char[] Use_input_parsed_to_char = isbn.ToCharArray();       
            char[] ISBN_number;
            ISBN_number = new char[10];
            char letter;
            for (int j = 0; j < isbn.Length; j++)
            {
                letter = Use_input_parsed_to_char[j];
                ISBN_number[j] = letter;
            }
            int Multiplier = 10;
            int sum = 0;
            int mod,subst, parsed_number1;
            for (int i = 0; i < ISBN_number.Length; i++)
            {
                if (Multiplier == 1)
                {
                    break;
                }
                int parsed_number = (int)Char.GetNumericValue(ISBN_number[i]);
                sum = sum + parsed_number * Multiplier;
                Multiplier--;
            }
            mod = sum % 11;
            subst = 11 - mod;
            parsed_number1 = (int)Char.GetNumericValue(ISBN_number[9]);
            if (ISBN_number[9].Equals('x'))
            {
                parsed_number1 = (int)Char.GetNumericValue(ISBN_number[9]);
            }
            if (subst == 10 && ISBN_number[9].Equals('x'))
            {
                return true;               
            }
            else if (subst == parsed_number1)
            {
                return true;
            }
            else
            {
                return false;
            }                        
        }
        
        public bool Update_Book(string title, string author, string edition, double price, string isbn, int stock, string newisbn)//updates the book entry
        {
            bool oky = false;
            try
            {
                
                using (var connection = conn.con)
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("update Books set Title = @title,Author = @author,Edition = @edition ,Price = @price, ISBN = @isbn, Stock = @stock Where ISBN='"+  isbn + "'" , connection);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@edition", edition);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@isbn", newisbn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Book Updated");
                    oky = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bad data detected. Book entry was not updated." + ex);
            }
            return oky;
        }

        public bool Delete_Book(string isbn)//deletes the book with the given isbn-10 number
        {
            bool oky = false;
            try
            {
                using (var connection = conn.con)
                {
                    connection.Open();                                       
                    SqlCommand cmd = new SqlCommand("Delete from Books where ISBN=" + isbn, connection);                                     
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Book Deleted");
                    oky = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex);
            }
            return oky;
        }
        
        public Book SearchBook(string isbn)//returns a book with the given isbn-10 number
        {
            string btitle;
            string bauthor;
            string bedition;
            string bprice;
            string bisbn;
            int bstock;
            Book newbook = new Book();
            try
            {
                using (var connection = conn.con)
                {
                    connection.Open();
                    SqlDataReader dr;
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Books WHERE ISBN = " + isbn, connection);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        btitle = (dr["Title"].ToString());
                        bauthor = (dr["Author"].ToString());
                        bedition = (dr["Edition"].ToString());
                        bprice = (dr["Price"].ToString());
                        bisbn = (dr["ISBN"].ToString());
                        bstock = Convert.ToInt32((dr["Stock"].ToString()));
                        newbook.Title = btitle;
                        newbook.Author = bauthor;
                        newbook.Edition = bedition;
                        newbook.Price = Convert.ToDouble(bprice);
                        newbook.ISBN = bisbn;
                        newbook._Stock = bstock;                                               
                       }
                    else
                    {
                        MessageBox.Show("Book Not found");
                    }
                 }                
            }                    
            catch (Exception ex)
            {                
                MessageBox.Show("error"+ ex);
            }
            return newbook;
        }

        public bool bookExists(string isbn)//checks if the given book exists mostly used to update test books with bad isbns
        {
            try
            {
                using (var connection = conn.con)
                {
                    if (ConnectionState.Closed == connection.State)
                    {
                        connection.Open();
                    }
                    SqlCommand ad = new SqlCommand("select * from Books where ISBN ='" + isbn + "'  ", connection);
                    DataTable dt = new DataTable();
                    SqlDataReader rd = ad.ExecuteReader();
                    dt.Load(rd);
                    if (dt.Rows.Count == 1)//if book is found
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception x)
            {
                throw new Exception("error" + x);
            }
        }

    }
}