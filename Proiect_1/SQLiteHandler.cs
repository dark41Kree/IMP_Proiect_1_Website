using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace Proiect_1
{
    internal class SQLiteHandler
    {

            public static SQLiteConnection SQLiteConnect()
            {
                SQLiteConnection conn;
                conn = new SQLiteConnection("Data Source=db.sqlite; Version=3;");
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                }
                return conn;
            }

            public static void InsertKeyword(SQLiteConnection conn, string keyword)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Keywords (keyword)" +
                    "VALUES ('" + keyword + "')";
                cmd.ExecuteNonQuery();
                Console.WriteLine("Adaugare cu succes!");
            }

            public static void SQLiteDisconnect(SQLiteConnection conn)
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            public static void DeleteKeyword(SQLiteConnection conn, string keyword)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Keywords WHERE keyword = @keyword";
                cmd.Parameters.AddWithValue("@keyword", keyword);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Stergere cu succes!");
            }

            public static List<string> GetAllKeywords(SQLiteConnection conn)
            {
                List<string> keywords = new List<string>();

                SQLiteDataReader reader = null;

                SQLiteCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Keywords";

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    keywords.Add(reader.GetString(1));
                }

                return keywords;

            }

    }
    
}
