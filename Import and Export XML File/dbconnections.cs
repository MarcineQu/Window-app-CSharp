using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace Import_and_Export_XML_File
{
    public class dbconnections
    {
        string query;
        public void export_to_database(DataTable dt) {
            //Zapis konfiguracji połączenia
            string connectionString = "" + "datasource=127.0.0.1;" + // adres domyślny dla serwera lokalnego
            "port=3306;" + // domyślny port
            "username=root;" + // domyślna nazwa użytkownika który ma dostęp do bazy danych
            "password=;" + // hasło dostępowe
            "SslMode = none;" + // tryb połączenia SSL
            "database=project;"; // nazwa bazy danych do które się             podłączamy;
            MySqlConnection databaseConnection = new
            MySqlConnection(connectionString); // deklaracja połączenia w oparciu o konfigurację
            MySqlCommand commandDatabase;
            MySqlDataReader reader;
            try
            {
                // nawiązanie połączenia z bazą danych
                databaseConnection.Close();
                databaseConnection.Open();
                try
                {



                    query = "";
                    //query += "VALUES('test','test1','test2','test3',4)";
                    for (int i = 1; i < dt.Rows.Count;i++) {
                        //j++;
                        query += "INSERT INTO project.Tracks (Title, Artist, Artwork, Url) ";
                        query +="VALUES(\"" + dt.Rows[i].Field<String>(0) +"\",\""+ dt.Rows[i].Field<String>(1) + "\",\""+ dt.Rows[i].Field<String>(2) + "\",\""+ dt.Rows[i].Field<String>(3) +"\"); ";
                                         
                    }
                    MessageBox.Show(query);
                    
                    commandDatabase = new MySqlCommand(query,
                    databaseConnection); // deklaracja zlecenia wykonania 
                    //zapytania w obrębie bazy z którą jest ustanowione połączenie
                    commandDatabase.CommandTimeout = 15;
                    // wykonanie zapytania i zapis odpowiedzi do obiektu reader
                    reader = commandDatabase.ExecuteReader();
                    
                    MessageBox.Show("Dane wysłane do bazy danych");
                }
                // jeśli zapytanie zawiera błąd składni ...
                catch (Exception ex)
                {
                    // Show any error message.
                    MessageBox.Show(ex.Message);
                }



                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                MessageBox.Show(ex.Message);
            }
        }
        /*
        public void clear_database() {
            //Zapis konfiguracji połączenia
            string connectionString = "" + "datasource=127.0.0.1;" + // adres domyślny dla serwera lokalnego
            "port=3306;" + // domyślny port
            "username=root;" + // domyślna nazwa użytkownika który ma dostęp do bazy danych
            "password=;" + // hasło dostępowe
            "SslMode = none;" + // tryb połączenia SSL
            "database=project;"; // nazwa bazy danych do które się             podłączamy;
            MySqlConnection databaseConnection = new
            MySqlConnection(connectionString); // deklaracja połączenia w oparciu o konfigurację
            MySqlCommand commandDatabase;
            try
            {
                // nawiązanie połączenia z bazą danych
                databaseConnection.Close();
                databaseConnection.Open();
                try
                {
                    query = "DELETE FROM tracks";
                    //query += "VALUES('test','test1','test2','test3',4)";
                   

                    commandDatabase = new MySqlCommand(query,
                    databaseConnection); // deklaracja zlecenia wykonania 
                    MessageBox.Show("Database_cleaned");
                }
                // jeśli zapytanie zawiera błąd składni ...
                catch (Exception ex)
                {
                    // Show any error message.
                    MessageBox.Show(ex.Message);
                }



                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                MessageBox.Show(ex.Message);
            }

        }
        */
        public DataTable import_from_database() {
            DataTable dt = new DataTable();
            DataColumn column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "title";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            dt.Columns.Add(column);
            column = new DataColumn();
            column.ColumnName = "Artist";
            dt.Columns.Add(column);
            column = new DataColumn();
            column.ColumnName = "Artwork";
            dt.Columns.Add(column);
            column = new DataColumn();
            column.ColumnName = "url";
            dt.Columns.Add(column);


            //Zapis konfiguracji połączenia
            string connectionString = "" + "datasource=127.0.0.1;" + // adres domyślny dla serwera lokalnego
            "port=3306;" + // domyślny port
            "username=root;" + // domyślna nazwa użytkownika który ma dostęp do bazy danych
            "password=;" + // hasło dostępowe
            "SslMode = none;" + // tryb połączenia SSL
            "database=project;"; // nazwa bazy danych do które się             podłączamy;
            MySqlConnection databaseConnection = new
            MySqlConnection(connectionString); // deklaracja połączenia w oparciu o konfigurację
            MySqlCommand commandDatabase;
            MySqlDataReader reader;
            try
            {
                // nawiązanie połączenia z bazą danych
                databaseConnection.Close();
                databaseConnection.Open();
                try
                {
                    query = "SELECT * FROM tracks";
                    //query += "VALUES('test','test1','test2','test3',4)";


                    commandDatabase = new MySqlCommand(query,
                    databaseConnection); // deklaracja zlecenia wykonania 
                    commandDatabase.CommandTimeout = 60;
                    reader = commandDatabase.ExecuteReader();


                    // jeśli zapytanie zwróciło jakieś wartości
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //TODO ADDING TO DT
                            dt.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));

                        }
                        MessageBox.Show("Wczytano dane z bazy danych");
                    }
                    // jeśli zapytanie nie zwróciło wartości
                    else { MessageBox.Show("Baza danych jest pusta"); }
                    reader.Close();

                }
                // jeśli zapytanie zawiera błąd składni ...
                catch (Exception ex)
                {
                    // Show any error message.
                    MessageBox.Show(ex.Message);
                }



                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                MessageBox.Show(ex.Message);
            }

            return dt;
        }
    }
}
