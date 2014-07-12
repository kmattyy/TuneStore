using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace TuneStore.Model
{
    /// <summary>
    /// Data access layer superclass of model classes
    /// </summary>
    class DAL
    {
        protected static bool Connected;
        protected static bool ConnectionCreated;

        protected static SqlConnection m_Connection;
        protected string m_ConnectionString = "Data Source=MATTYY-PC;Initial Catalog=TuneStore;Integrated Security=SSPI";

        protected SqlCommand m_Command;
  


        protected bool IsConnectCreated()
        {
            return ConnectionCreated;
        }

        protected bool IsConnected()
        {
            return Connected;
        }

        protected SqlConnection GetConnection()
        {
            return m_Connection;
        }

        /// <summary>
        /// Creates a new Database Connection
        /// </summary>
        /// <returns></returns>
        protected bool CreateConnection()
        {
            // Create the Connection if is was not already created.
            if (ConnectionCreated != true)
            {
                try
                {
                    m_Connection = new SqlConnection(m_ConnectionString);
                    m_Connection.Open();
                    ConnectionCreated = true;
                    m_Connection.Close();
                    Connected = false;

                    return true;
                }
                catch
                {

                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Open the Connection when the state is not already open.
        /// </summary>
        protected bool OpenConnection()
        {
            // Open the Connection when the state is not already open.
            if (Connected != true)
            {
                try
                {
                    CreateConnection();
                    m_Connection.Open();
                    Connected = true;
                    return true;
                }
                catch
                {
                    return false;
                }

            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Close the Connection when the connection is opened.
        /// </summary>
        internal void CloseConnection()
        {
            if (Connected == true)
            {
                m_Connection.Close();
                Connected = false;
            }
        }

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected object ExecuteScalar(string query, ref string errorMessage)
        {
            object value;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(query, m_Connection);
                value = cmd.ExecuteScalar();
                errorMessage = "OK";
            }
            catch (SqlException e)
            {
                value = null;
                errorMessage = e.Message;
            }
            finally
            {
                CloseConnection();
            }
            return value;
        }

        /// <summary>
        /// Executes a given query, and returns the result in a datareader
        /// </summary>
        /// <param name="query"> The query to be executed </param>
        /// <param name="errorMessage"> Output error message </param>
        /// <returns></returns>
        protected SqlDataReader ExecuteReader(string query, ref string errorMessage)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(query, m_Connection);
                SqlDataReader rdr = cmd.ExecuteReader();
                errorMessage = "OK";
                return rdr;

            }
            catch (SqlException e)
            {

                errorMessage = e.Message;
                CloseConnection();
                return null;
            }
        }
        /// <summary>
        /// Closes the data reader given as a parameter, and also closes the connection
        /// </summary>
        /// <param name="rdr">The SqlDataReader to be closed</param>
        protected void CloseDataReader(SqlDataReader rdr)
        {
            if (rdr != null)
                rdr.Close();
            CloseConnection();
        }

        /// <summary>
        /// Executes a given insert/update/delete command, and returns an error message. 
        /// (errormessage is "OK" if no exception occured)
        /// </summary>
        /// <param name="command">The command to be executed</param>
        protected string ExecuteNonQuery(string command)
        {
            string error;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(command, m_Connection);
                cmd.ExecuteNonQuery();
                error = "OK";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
            return error;

        }

        /// <summary>
        /// Executes a stored procedure that doesn't have any otuput parameters.
        /// </summary>
        /// <param name="name">The name of the stored procedure</param>
        /// <param name="parameterNames">The list of the parameter names</param>
        /// <param name="parameterValues">The list of the parameter values</param>
        /// <returns></returns>
        protected string ExecuteStoredProcedureNonQuery(string name, string[] parameterNames, string[] parameterValues)
        {
            string error;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(name, m_Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < parameterNames.Length; i++)
                {
                    cmd.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                }
                cmd.ExecuteNonQuery();

                error = "OK";
            }
            catch (SqlException e)
            {
                error = e.Message;
            }
            finally
            {
                CloseConnection();
            }
            return error;
        }

        /// <summary>
        /// String to MD5 hash generator
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}
