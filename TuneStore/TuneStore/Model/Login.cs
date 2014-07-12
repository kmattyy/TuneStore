using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TuneStore.Model
{
    /// <summary>
    /// User login on model layer
    /// </summary>
    class Login : DAL
    {
        public int userId = -1;//User identifier
        public String group = "";//User group name
        public String userName = "";//Logged user name
        public bool loggedIn = false;//Is user logged in/Login was succesful

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="name">Username</param>
        /// <param name="pass">Password</param>
        public Login(string name, string pass)
        {
            userName = name;
            string err = "";
            string encpass = MD5Hash(pass);
            string comm = "SELECT UserTypeName,UserId FROM Users, UserTypes WHERE Users.UserTypeID=UserTypes.utypeID and UserName = '" + name + "' and encPass = '" + encpass + "'";

            SqlDataReader reader = ExecuteReader(comm, ref err);

            while (reader.Read())
            {
                group = reader["UserTypeName"].ToString();
                userId = Int32.Parse(reader["UserId"].ToString());
                loggedIn = true;

            }
            reader.Close();

        }



    }
}
