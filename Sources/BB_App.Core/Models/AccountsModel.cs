﻿using System.ComponentModel;
using BB_App.Helpers;
using MySql.Data.MySqlClient;
using static BB_App.Core.Models.SqlConnection;
using static BB_App.Core.Models.Consts;
using static BB_App.Core.Properties.Settings;
using static BB_App.Helpers.AccountsHelpers;
using static BB_App.Helpers.StringHelpers;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System;

namespace BB_App.Core.Models
{
	public class AccountsModel
	{

		public static string Login(string username, string password)
		{
			var type = string.Empty;
            var md5 = MD5.Create();
            var sourceBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5.ComputeHash(sourceBytes);
            var md5_pass = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            if (!Connect(Server, DbUser, DbPassword, DbName)) return type;

			const string query =
				"SELECT username, password, account_type FROM accounts WHERE username = @user AND password = @pwd";

			var command = new MySqlCommand(query, Conn);
			command.Prepare();
			command.Parameters.AddWithValue("@user", username);
			command.Parameters.AddWithValue("@pwd", md5_pass);

			var reader = command.ExecuteReader();

			while (reader.Read())
				type = reader.GetString(2);

			return type;
		}

		public static void SetAccountType(AccountType type)
		{
			Default.account_type = ParseAccountType(type);
			Default.Save();
			Default.Reload();
		}

	    public static void SetCurrentUser(string user)
	    {
	        Default.current_user = Capitalize(user).Trim(' ');
	        Default.Save();
	        Default.Reload();
        }

	    public static string WhoIsConnected() => Default.current_user;

	    /// <summary>
	    /// Determines if the user with the username given in parameters is administrator 
	    /// </summary>
	    /// <param name="username">Username of the user to check</param>
	    /// <returns>True if the user is an administrator</returns>
	    public static bool IsAdmin(string username)
	    {
	        var isAdmin = false;

	        if (!Connect(Server, DbUser, DbPassword, DbName)) return false;

	        const string query = "SELECT account_type FROM accounts WHERE username = @user";
            
            var command = new MySqlCommand(query, Conn);
            command.Prepare();
	        command.Parameters.AddWithValue("@user", username.ToLower());
	        var reader = command.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                    isAdmin = reader[0].ToString() == "admin";

	        return isAdmin;
	    }

        public static int UserId(string username)
        {
            var id = 0;

            if (!Connect(Server, DbUser, DbPassword, DbName)) return id;

            const string query = "SELECT id FROM accounts WHERE username = @user";

            var command = new MySqlCommand(query, Conn);
            command.Prepare();
            command.Parameters.AddWithValue("@user", username.ToLower());
            var reader = command.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                    id = reader.GetInt32(0);

            return id;
        }

        /// <summary>
        /// Get the list of all users available in the database
        /// </summary>
        /// <returns>A list containing all the usernames</returns>
	    public static List<string> LoadUsers()
        {
            List<string> users = new List<string>();

            if (!Connect(Server, DbUser, DbPassword, DbName)) return users;

            const string query = "SELECT username FROM accounts ORDER BY username DESC";

            var command = new MySqlCommand(query, Conn);
            var reader = command.ExecuteReader();

            while (reader.Read())
                users.Add(reader[0].ToString());

            return users;
        }

        /// <summary>
        /// Load informations about specified user
        /// </summary>
        /// <param name="username">Username of the user to load informations</param>
        /// <returns>String containing informations about the user</returns>
	    public static string LoadInfo(string username)
	    {
	        string infos = null;

	        if (!Connect(Server, DbUser, DbPassword, DbName)) return infos;

	        const string query = "SELECT username, account_type FROM accounts WHERE username = @user";

            var command = new MySqlCommand(query, Conn);
            command.Prepare();
	        command.Parameters.AddWithValue("@user", username);
	        var reader = command.ExecuteReader();

	        while (reader.Read())
	        {
	            infos = reader[0].ToString() + "|" + reader[1].ToString();
	        }

	        return infos;
	    }

        /// <summary>
        /// Delete user with specified username
        /// </summary>
        /// <param name="username">Username of the user to delete</param>
        /// <returns>True if the user was successfully deleted or false if not</returns>
	    public static bool DeleteUser(string username)
	    {
	        var deleted = false;

	        if (!Connect(Server, DbUser, DbPassword, DbName)) return deleted;

	        const string query = "DELETE FROM accounts WHERE username = @user";

            var command = new MySqlCommand(query, Conn);
            command.Prepare();
	        command.Parameters.AddWithValue("@user", username);
	        deleted = command.ExecuteNonQuery() > 0;

	        return deleted;
	    }

	    public static bool CreateUser(string username, string password, string type)
	    {
	        var added = false;
            var md5 = MD5.Create();
            var sourceBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5.ComputeHash(sourceBytes);
            var md5_pass = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            if (!Connect(Server, DbUser, DbPassword, DbName)) return added;

	        const string query = "INSERT INTO accounts (ref_hospital, username, password, account_type) VALUES (@ref, @user, @pwd, @type)";

            var command = new MySqlCommand(query, Conn);
            command.Prepare();
	        command.Parameters.AddWithValue("@ref", Default.reference);
	        command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pwd", md5_pass.ToLower());
	        command.Parameters.AddWithValue("@type", ParseAccountType(ParseAccountType(type)));

	        added = command.ExecuteNonQuery() > 0;

	        return added;
	    }

    }
}
