﻿using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using param = BB_App.Core.Models.Consts;

/* =============================================================
* Class Name : Blood
* Description : Performs CRUD on blood over the database.
* By : Marc Enzo
* At : 17/03/1018
* ============================================================= */

namespace BB_App.Core.Models
{
    /// <summary>
    ///     Represents the blood bank from the database.
    /// </summary>
    public class Bloods
    {
        #region Fields

        public static List<Blood> BloodsList = new List<Blood>();

        #endregion Fields

        #region Constructors

        public Bloods()
        {
            if (Exist())
                LoadBloods();
            else
                InitializeBloods();
        }

        #endregion Constructors

        /// <summary>
        ///     Represents a blood object with group and units.
        /// </summary>
        public class Blood
        {
            public string BloodGroup { get; set; }
            public int BloodUnits { get; set; }
        }

        #region Methods

        /// <summary>
        ///     Populates the blood_bank table with null values from the current hospital.
        /// </summary>
        public static void InitializeBloods()
        {
            SqlConnection.Disconnect();

            if (SqlConnection.Connect(param.Server, param.DbUser, param.DbPassword, param.DbName))
            {
                const string query = "INSERT INTO blood_bank VALUES (@hospital, 0, 0, 0, 0, 0, 0, 0, 0, 0);";
                var command = new MySqlCommand(query, SqlConnection.Conn);

                command.Prepare();
                command.Parameters.AddWithValue("@hospital", param.Hospital);

                command.ExecuteNonQuery();
            }

            LoadBloods();
        }

        /// <summary>
        ///     Load bloodgroups with units from the database.
        /// </summary>
        public static void LoadBloods()
        {
            if (!SqlConnection.Connect(param.Server, param.DbUser, param.DbPassword, param.DbName)) return;

            if (Exist())
            {
                var query = "SELECT AP, AM, BP, BM, ABP, ABM, OP, OM FROM blood_bank WHERE ref_hospital = '" +
                            param.Hospital + "'";
                var command = new MySqlCommand(query, SqlConnection.Conn);
                var reader = command.ExecuteReader();

                BloodsList.Clear();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    reader.Read();

                    var blood = new Blood {BloodGroup = reader.GetName(i), BloodUnits = reader.GetInt32(i)};
                    BloodsList.Add(blood);
                }

                reader.Close();
            }
            else
            {
                InitializeBloods();
            }

            UpdateValues();
        }

        /// <summary>
        ///     Update units of bloodgroups to the database based on the values of the current list.
        /// </summary>
        public static void UpdateValues()
        {
            if (!SqlConnection.Connect(param.Server, param.DbUser, param.DbPassword, param.DbName)) return;

            const string query =
                "UPDATE blood_bank SET AP = @ap, AM = @am, BP = @bp, BM = @bm, ABP = @abp, ABM = @abm, OP = @op, OM = @om, Total = @total WHERE ref_hospital = @hospital" ;
            var command = new MySqlCommand(query, SqlConnection.Conn);

            command.Prepare();

            command.Parameters.AddWithValue("@ap", BloodsList[0].BloodUnits);
            command.Parameters.AddWithValue("@am", BloodsList[1].BloodUnits);
            command.Parameters.AddWithValue("@bp", BloodsList[2].BloodUnits);
            command.Parameters.AddWithValue("@bm", BloodsList[3].BloodUnits);
            command.Parameters.AddWithValue("@abp", BloodsList[4].BloodUnits);
            command.Parameters.AddWithValue("@abm", BloodsList[5].BloodUnits);
            command.Parameters.AddWithValue("@op", BloodsList[6].BloodUnits);
            command.Parameters.AddWithValue("@om", BloodsList[7].BloodUnits);
            command.Parameters.AddWithValue("@total", GetTotal());
            command.Parameters.AddWithValue("@hospital", param.Hospital);

            command.ExecuteNonQuery();
        }

        public static int GetUnits(string bloodgroup)
        {
            bloodgroup = bloodgroup.ToLower();

            foreach (var t in BloodsList)
                if (t.BloodGroup.ToLower() == bloodgroup)
                    return t.BloodUnits;

            return 0;
        }

        public static void ChangeBloodValue(string bloodgroup, int units)
        {

            foreach (var blood in BloodsList)
            {
                if (blood.BloodGroup.ToLower() == bloodgroup)
                    blood.BloodUnits = units;
            }

            UpdateValues();
        }

        #endregion Methods

        #region Functions

        /// <summary>
        ///     Verify if the database contains blood informations of the current hospital.
        /// </summary>
        /// <returns>True if exist and false if not</returns>
        private static bool Exist()
        {
            SqlConnection.Disconnect();

            if (!SqlConnection.Connect(param.Server, param.DbUser, param.DbPassword, param.DbName)) return false;

            var query = "SELECT * FROM blood_bank WHERE ref_hospital = '" + param.Hospital + "'";
            var command = new MySqlCommand(query, SqlConnection.Conn);
            var reader = command.ExecuteReader();

            var exist = reader.HasRows;

            reader.Close();

            return exist;
        }

        /// <summary>
        ///     Calculates the sum of all elements from the blood list.
        /// </summary>
        /// <returns>An integer representing the sum of all items in blood list</returns>
        public static int GetTotal()
        {
            var total = 0;

            foreach (var item in BloodsList)
            {
                total += item.BloodUnits;
            }

            return total;
        }

        #endregion Functions
    }
}