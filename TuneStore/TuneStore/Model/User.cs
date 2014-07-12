using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuneStore.Model
{
    /// <summary>
    /// User registration into database,user information modifying
    /// </summary>
    class User : DAL
    {
        public int userID;//User identifier
        public String err = "";//error message

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uID">UserID</param>
        public User(int uID)
        {
            userID = uID;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="regval">Dictionary containing Fieldname-Value</param>
        /// <returns>Successful/Unsuccessful registration</returns>
        public static bool registerUser(Dictionary<String, String> regval)
        {


            if (!isAlreadyregistered(regval["email"]))
            {
                String comm = "Insert into Users(Username,EncPass,Email,Birthdate,UserTypeId";
                string utype = getUsertype(regval["type"]);
                string encpass = MD5Hash(regval["pass1"]);


                if (utype == "4")//free
                {
                    comm += ") values (";
                    comm += "'" + regval["username"] + "',";
                    comm += "'" + encpass + "',";
                    comm+="'"+regval["email"]+"',";
                    comm+="'"+regval["birthdate"]+"',";
                    comm+=""+utype+")";
                   

                }

                else{//Publisher/Pro

                    comm += ",Firstname,Lastname,Cardnumber,Validationnumber,ExpirationDate,Credit) values (";
                    comm += "'" + regval["username"] + "',";
                    comm += "'" + encpass + "',";
                    comm += "'" + regval["email"] + "',";
                    comm += "'" + regval["birthdate"] + "',";
                    comm += "" + utype + ",";
                    comm += "'" + regval["firstname"] + "',";
                    comm += "'" + regval["lastname"] + "',";
                    comm += "'" + regval["cardnum"] + "',";
                    comm += "'" + regval["valnum"] + "',";
                    comm += "'" + regval["expdate"] + "',";

                  //  MessageBox.Show(regval["initcredit"].TrimEnd('$'));

                    comm += "" + regval["initcredit"].TrimEnd('$') + ")";


                }


                User u=new User(-1);
                string s = u.ExecuteNonQuery(comm);
                if (s != "OK")
                {
                    MessageBox.Show("Registration Error:"+s);
                    return false;
                }
                else {
                    MessageBox.Show("Registration Succsesful!!");
                    return true;    
                }
              
            }
            else
            {
                MessageBox.Show("User with this e-mail already registered!!");
                return false;
            }



        }

        /// <summary>
        /// Get user group id
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string getUsertype(string p)
        {
            string comm = "SELECT UTypeID FROM UserTypes WHERE Usertypename = '" + p + "'";

            User u = new User(-1);
            String s = "";
            SqlDataReader reader = u.ExecuteReader(comm, ref s);

            while (reader.Read())
            {
                s = reader["UTypeID"].ToString();

            }
            reader.Close();

            return s;
        }

        /// <summary>
        /// Checking if is user already registered with given e-mail address
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True/False</returns>
        private static bool isAlreadyregistered(string email)
        {
            string comm = "SELECT * FROM Users WHERE email = '" + email + "'";

            User u = new User(-1);
            String s = "";
            SqlDataReader reader = u.ExecuteReader(comm, ref s);

            bool isRegistered = false;

            while (reader.Read())
            {
                isRegistered = true;

            }
            reader.Close();

            return isRegistered;
        }


        /// <summary>
        /// Add/withdraw credit to/from account
        /// </summary>
        /// <param name="a">Credit amount</param>
        /// <param name="command">Add/Withdraw</param>
        internal void fillwithdraw(int a, string command)
        {
            try
            {
                OpenConnection();
                SqlCommand com = new SqlCommand(command, m_Connection);
                com.CommandType = CommandType.StoredProcedure;
                SqlParameter uid = new SqlParameter("@userid", SqlDbType.Int);
                uid.Value = userID;
                uid.Direction = ParameterDirection.Input;
                com.Parameters.Add(uid);

                SqlParameter amount = new SqlParameter("@amount", SqlDbType.Real);
                amount.Value = a;
                amount.Direction = ParameterDirection.Input;
                com.Parameters.Add(amount);

                SqlParameter returnval = new SqlParameter("@return", SqlDbType.Int);
                returnval.Direction = ParameterDirection.ReturnValue;
                com.Parameters.Add(returnval);
                com.ExecuteNonQuery();
                int ret = (int)returnval.Value;

                switch (ret)
                {
                    case 1:
                        MessageBox.Show("Credit not enough!!");
                        break;
                
                    case 0:
                        MessageBox.Show("Transaction validated!!!");
                        break;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Checkout failed!" + e);
            }
        }

      
       
    }
}
