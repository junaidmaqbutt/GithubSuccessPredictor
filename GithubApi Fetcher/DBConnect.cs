using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Database
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private bool isClosing = false;
        public bool isConnected
        {
            get
            {
                if (connection.State == System.Data.ConnectionState.Open) return true;
                else return false;
            }
        }
        private bool checkConnection()
        {
            int x = 0;
            do
            {
                if (!isClosing && connection.State != System.Data.ConnectionState.Open && connection.State != System.Data.ConnectionState.Connecting)
                {
                    OpenConnection();
                }
                x++;
                if (x >= 5) return isConnected;
            } while (!isConnected);

            
            return isConnected;
        }
        public DBConnect(string serverLink, string databaseName, string userName, string pass)
        {
            server = serverLink;
            database = databaseName;
            uid = userName;
            password = pass;
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);
            connection.StateChange += connection_StateChange;
            OpenConnection();
        }
        void connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (!isClosing && connection.State != System.Data.ConnectionState.Open && connection.State != System.Data.ConnectionState.Connecting)
            {
                string connectionString;
                connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
                connection = new MySqlConnection(connectionString);
                OpenConnection();
            }
        }
        private bool OpenConnection()
        {
            try
            {
                if (isClosing) 
                    return false;
                else if (connection.State != System.Data.ConnectionState.Open)
                {
                    isClosing = false;
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }
        public bool CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed) return true;
                isClosing = true;
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }
        public void Insert(string TableName, string TableEntries, string Values)
        {
            string query = "INSERT INTO " + TableName + "(" + TableEntries + ") VALUES (" + Values + ");";
            if (! checkConnection()) return ;
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        public void ExecuteCustomQuery(string query)
        {
            if (!checkConnection()) return;
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.CommandTimeout = 1000;
            cmd.ExecuteNonQuery();
        }
        public void Update(string TableName, string Set, string Where)
        {
            string query = "UPDATE " + TableName + " SET " + Set + "  WHERE " + Where + ";";
            //Open connection
            if (!checkConnection()) return;
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        public void Update(string TableName, string word, string answer,string weights)
        {
            string query = "UPDATE " + TableName + " SET Answers=\"" + answer + "\" , Weights=\"" + weights +"\" WHERE Word=\"" + word + "\";";
            if (!checkConnection()) return;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        public void Delete(string tableName)
        {
            string query = "DELETE FROM " + tableName + " WHERE 1";
            if (!checkConnection()) return;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        public void Delete(string tableName, string word)
        {
            string query = "DELETE FROM " + tableName + " WHERE  Word=" + word;
            if (!checkConnection()) return;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        public List<string> Select(string TableName, string Where ,List<string> Entries)
        {
            string query = "SELECT * FROM " + TableName + " WHERE " + Where +";";
            List<string> list = new List<string>();
            if (!checkConnection()) return null;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                foreach (string entry in Entries)
                {
                    list.Add(dataReader[entry] + "");
                }
            }
            dataReader.Close();
            return list;
        }
        public List<string[]> ExecuteQuery(string TableName, string Query)
        {
            List<string[]> Entries = new List<string[]>();
            if (!checkConnection()) return null;
            MySqlCommand cmd = new MySqlCommand(Query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                string[] Temp = new string[dataReader.FieldCount];
                for (int x =0; x<dataReader.FieldCount;x++)
                {
                    Temp[x]=dataReader[x] + "";
                }
                Entries.Add(Temp);
            }
            dataReader.Close();
            return Entries;
        }
        public List<string>[] SelectAll(string TableName)
        {
            string query = "SELECT * FROM " + TableName;
            List<string>[] list = new List<string>[3];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            if (!checkConnection()) return null;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.CommandTimeout = 60;
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                list[0].Add(dataReader["Word"] + "");
                list[1].Add(dataReader["Answers"] + "");
                list[2].Add(dataReader["Weights"] + "");
            }
            dataReader.Close();
            return list;
        }
        public bool doesExist(String tableName, String Where)
        {
            string query = "SELECT * FROM " + tableName + " WHERE " + Where + ";";
            if (!checkConnection()) return false;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (dataReader.HasRows)
            {
                dataReader.Close();
                return true;
            }
            dataReader.Close();
            return false;
        }
        public int Count(string tableName)
        {
            string query = "SELECT Count(*) FROM " + tableName;
            int Count = -1;
            if (!checkConnection()) return -1;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            Count = int.Parse(cmd.ExecuteScalar() + "");
            return Count;
        }
    }
}