using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;
using System.Data.SqlClient;

public class SqlHandler : MonoBehaviour {

    public static SqlHandler instance = null;
    MySqlConnection connection = null;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        SetupSQLConnection();
        CloseSQLConnection();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetupSQLConnection()
    {
        if (connection == null)
        {
            string connectionString = "SERVER=empty-hallway-productions.de;" + "DATABASE=houseparty;" + "UID=houseparty;" + "PASSWORD=ggj;";
            try
            {
                connection = new MySqlConnection(connectionString);
                //connection.Open();
            }
            catch (MySqlException ex)
            {
                Debug.LogError("MySQL Error: " + ex.ToString());
            }
            //CloseSQLConnection();

        }
    }

    private void CloseSQLConnection()
    {
        if (connection != null)
        {
            connection.Close();
        }
    }

    /// <summary>
    /// Setzt user score für den dementsprechenden Namen. Erstellt einen User mit dem angegeben Namen, falls er nicht existiert.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_initialScore"></param>
    public void SetUserScore(string _name, float _initialScore = 0)
    {
        connection.Open();

        string commandText = "INSERT INTO scores (name, score) VALUES (" + "'" + _name +"'" + "," + "'" + _initialScore + "'" + ") ON DUPLICATE KEY UPDATE score = " + _initialScore + ";";
        if (connection != null)
        {

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("MySQL error: " + ex.ToString());
            }
        }

        CloseSQLConnection();
    }

    public Dictionary<string, float> GetTop10UserScores()
    {
        Dictionary<string, float> highscore = new Dictionary<string, float>();

        string sql = "SELECT * FROM scores Order by score desc LIMIT 10; ";
        if (connection != null)
        {
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            MySqlDataReader reader = command.ExecuteReader();


            while(reader.Read())
            {
                string playerName = reader["name"].ToString();
                float playerScore = (float)reader["score"];

                highscore.Add(playerName, playerScore);
                
            }

            foreach (var bla in highscore)
            {
                Debug.Log(bla.Key + " ___ " + bla.Value);
            }
        }

        CloseSQLConnection();

        return highscore;
    }

    public float GetUserRank(string _name)
    {
        long playerScore = -1;

        string sql = "SELECT name, score, FIND_IN_SET( score, ( SELECT GROUP_CONCAT(score ORDER BY score DESC ) FROM scores )) AS rank FROM scores WHERE name = '" + _name + "'";
        if (connection != null)
        {
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var t = reader["rank"];
                playerScore =  (long)reader["rank"];
            }
        }

        CloseSQLConnection();

        return playerScore;
    }
}
