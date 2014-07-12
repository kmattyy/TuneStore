using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuneStore.Model
{
    /// <summary>
    /// Tracks access layer
    /// </summary>
    class Tracks : DAL
    {
        private int userID;//User identifier

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">User identifier</param>
        public Tracks(int Id)
        {
            userID = Id;
        }

        /// <summary>
        /// Get a DataTable from given query
        /// </summary>
        /// <param name="query"></param>
        /// <returns>DataTable with requested tracks</returns>
        public DataTable getDataTable(String query)
        {
            DataTable dt = new DataTable();
            if (this.OpenConnection())
            {

                DataSet ds = new DataSet();

                ds.Tables.Add(dt);
                CreateConnection();
                SqlDataAdapter tableadapter = new SqlDataAdapter(query, m_Connection);
                if (dt != null)
                    tableadapter.Fill(dt);


                this.CloseConnection();
            }
            else
            {
                MessageBox.Show("Grid fill failed!!", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show(query);
            }

            return dt;
        }

        /// <summary>
        /// Get query string from given filtering parameters
        /// </summary>
        /// <param name="artist">Artist Name</param>
        /// <param name="album">Album Title</param>
        /// <param name="title">Track Title</param>
        /// <param name="genre">Genre name</param>
        /// <param name="key">Key</param>
        /// <returns>Sql query string</returns>
        public static string getFilterString(string artist, string album, string title, string genre, string key)
        {
            string filterstring = "Select * from tracksview ";


            filterstring += "where quality like '%' ";
           
            if (artist != "")
            {
             
                filterstring += "and (Artist like '%" + artist + "%' or Title like'%" + artist + "%') ";
            }
            if (album != "")
            {
                filterstring += "and albumtitle like'%" + album + "%' ";
            }
            if (title != "")
            {
                filterstring += "and title like'%" + title + "%' ";
            }
            if (genre != "")
            {
                filterstring += "and genre like'%" + genre + "%' ";
            }
            if (key != "")
            {
                filterstring += "and keymark like'%" + key + "%' ";
            }
            //MessageBox.Show(filterstring);
            return filterstring;
        }

        /// <summary>
        /// Get a track information
        /// </summary>
        /// <param name="trackid">Track identifier</param>
        /// <returns>A DataRow with track informations</returns>
        internal DataRow getTrackInfo(int trackid)
        {

            if (this.OpenConnection())
            {
                string query = "SELECT * from Infoview where trackid=" + trackid;
                m_Command = new SqlCommand(query, m_Connection);
                var da = new SqlDataAdapter(m_Command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows) //csak egyvan
                {
                    return dr;
                }
            }
            else
            {
                MessageBox.Show("Info load error!!", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return null;

        }
        /// <summary>
        /// Get track artwork image from given id
        /// </summary>
        /// <param name="trackid">Track identifier</param>
        /// <returns>Image object</returns>
        internal Image fillPictureBox(int trackid)
        {
            String query = "SELECT bindata From Tracks t, Covers c where t.artworkid=c.coverid AND trackid=" + trackid;
            m_Command = new SqlCommand(query, m_Connection);
            var da = new SqlDataAdapter(m_Command);
            var ds = new DataSet();
            da.Fill(ds, "bindata");

            int count = ds.Tables["bindata"].Rows.Count;

            if (count > 0)
            {
                var data = (Byte[])(ds.Tables["bindata"].Rows[count - 1]["bindata"]);
                //Console.WriteLine(data.Length);
                MemoryStream stream = new MemoryStream(data);
                try
                {
                    Image img = Image.FromStream(stream, false, false);
                    return img;
                }
                catch (System.ArgumentException e)
                {
                    MessageBox.Show("Error in artwork loading!" + e);
                }


            }
            return null;
        }

        /// <summary>
        /// Get file path 
        /// </summary>
        /// <param name="trackid">Track identifier</param>
        /// <returns></returns>
        internal string getTrackPath(int trackid)
        {
            string path = "";
            if (this.OpenConnection())
            {
                string query = "SELECT adress, filename from tracks t ,paths p where t.pathid=p.pathid and trackid=" + trackid;
                m_Command = new SqlCommand(query, m_Connection);
                var da = new SqlDataAdapter(m_Command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    path = dr["adress"].ToString() + dr["filename"].ToString();
                }
            }
            else
            {
                MessageBox.Show("Track load error!!", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return path;
        }

        /// <summary>
        /// Insert track to user's cart
        /// </summary>
        /// <param name="uid">User identifier</param>
        /// <param name="trid">Track identifier</param>
        internal void addToCart(int uid, int trid)
        {
            String comm = "Insert into UserCart(UserID,TrackId) values(";
            comm += uid + "," + trid + ")";
            string s = ExecuteNonQuery(comm);


            if (s == "OK")
            {
                MessageBox.Show("Track added to cart!");
            }
            else
            {
                MessageBox.Show("Track add problem!" + s);
            }
        }

        /// <summary>
        /// Get tracks in user's cart 
        /// </summary>
        /// <param name="userId">UserIdentifier</param>
        /// <returns>Sql command for retrieving tracks</returns>
        internal static string getCart(int userId)
        {
            String s = "Select tv.trackid, tv.Artist, tv.Title, tv.Albumtitle, tv.Genre  from tracksview tv,usercart uc ";
            s += "where uc.UserId=" + userId + " and uc.trackid=tv.trackid";
            return s;
        }


        /// <summary>
        /// Get tracks purchased or uploaded by the user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Sql command string</returns>
        internal static string getUserTracks(int userId)
        {
            String s = "Select tv.trackid, tv.Artist, tv.Title, tv.Albumtitle, tv.Genre from tracksview tv,tracks t ";
            s += "where t.UserId=" + userId + " and t.trackid=tv.trackid union ";
            s += "Select tv.trackid, tv.Artist, tv.Title, tv.Albumtitle, tv.Genre from  tracksview tv,usertracks ut ";
            s += "where tv.trackid=ut.trackid and ut.userid=" + userId;

            return s;
        }

        /// <summary>
        /// Checkout user's cart 
        /// </summary>
        /// <param name="userid">User identifier</param>
        internal void checkoutCart(int userid)
        {
            try
            {
                OpenConnection();
                SqlCommand com = new SqlCommand("Checkout", m_Connection);
                com.CommandType = CommandType.StoredProcedure;
                SqlParameter uid = new SqlParameter("@userid", SqlDbType.Int);
                uid.Value = userid;
                uid.Direction = ParameterDirection.Input;
                com.Parameters.Add(uid);
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
                    case 2:
                        MessageBox.Show("Track invalidated!");
                        break;
                    case 0:
                        MessageBox.Show("Transaction validated!! Tracks added to account!");
                        break;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Checkout failed!"+e);
            }

        }
    }
}
