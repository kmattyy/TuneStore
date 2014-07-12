using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuneStore.Model;

namespace TuneStore.Controller
{   
    /// <summary>
    /// Track operations controller
    /// </summary>
    class TracksController
    {   
        private int userId;
        private MainFrame myViewFrame;
        private UserController myUserController;
        private Tracks myTracks;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mf">View form</param>
        /// <param name="uc">Controller for logged in user</param>
        public TracksController(MainFrame mf, UserController uc)
        {

            myViewFrame = mf;
            myUserController = uc;
            userId = myUserController.getUserId();
            myTracks = new Tracks(userId);
        }


        /// <summary>
        /// Getting tracks by parameters given by the user and filling the grid. 
        /// </summary>
        /// <param name="artist">Artist name</param>
        /// <param name="album">Album title</param>
        /// <param name="title">Track title</param>
        /// <param name="genre">Genre name</param>
        /// <param name="key">Key value</param>
        internal void setFilters(string artist, string album, string title, string genre, string key)
        {
            String s = Tracks.getFilterString(artist, album, title, genre, key);
            DataTable dt = myTracks.getDataTable(s);
            myViewFrame.fillTracksGrid(dt);
        }

        /// <summary>
        /// Getting informations about track and setting information labels
        /// </summary>
        /// <param name="trackid">Track identifier</param>
        internal void setInfoLabels(int trackid)
        {
            DataRow dr = myTracks.getTrackInfo(trackid);
            myViewFrame.setInfoLabels(dr);
        }
        /// <summary>
        /// Loading track artwork picture
        /// </summary>
        /// <param name="trackid">Track identifier</param>
        internal void setArtworkbox(int trackid)
        {
            Image i = myTracks.fillPictureBox(trackid);
            if (i != null)
                myViewFrame.setArtworkBox(i);
        }
        /// <summary>
        /// Getting track path for player
        /// </summary>
        /// <param name="trid"></param>
        /// <returns>File path string</returns>
        internal string getTrackPath(int trid)
        {
            if (userId != -1)
                return myTracks.getTrackPath(trid);
            else
                return "";
        }
        /// <summary>
        /// User identifier setter
        /// </summary>
        /// <param name="userid">User identifier</param>
        internal void setUserId(int userid)
        {
            this.userId = userid;
        }
        /// <summary>
        /// Cart add action
        /// </summary>
        /// <param name="uid">User identifier</param>
        /// <param name="trid">Track identifier</param>
        internal void addToCart(int uid, int trid)
        {
            myTracks.addToCart(uid, trid);
            fillCartGrid();
        }

        /// <summary>
        /// Filling user cart grid
        /// </summary>
        internal void fillCartGrid()
        {
            String s = Tracks.getCart(myUserController.getUserId());
            DataTable dt = myTracks.getDataTable(s);
            myViewFrame.fillCartGrid(dt);
        }



        //String dbFillScript = "D:\\Egyetem\\AB2\\Perl\\Adatbazis\\dbconnect.pl ";
        //D:\Egyetem\AB2\Perl\Adatbazis

        /// <summary>
        /// Fill tracks purchased(pro users)/uploaded(publisher) 
        /// </summary>
        internal void fillMyTracksGrid()
        {
            String s = Tracks.getUserTracks(myUserController.getUserId());
        

            DataTable dt = myTracks.getDataTable(s);
            myViewFrame.fillUserTracksGrid(dt);

        }
        /// <summary>
        /// Purchase tracks from cart
        /// </summary>
        internal void checkoutCart()
        {
            myTracks.checkoutCart(myUserController.getUserId());
        }
    }
}
