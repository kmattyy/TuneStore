using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuneStore.Model;

namespace TuneStore.Controller
{
    /// <summary>
    /// Controller class for user operations,
    /// </summary>
    class UserController
    {
        private Login myUserLogin;//Login user
        private MainFrame myViewFrame;//View
        private User myUser;//Register user

        /// <summary>
        /// Guest user controller constructor
        /// </summary>
        /// <param name="ts">View frame</param>
        public UserController(MainFrame ts)
        {
            myViewFrame = ts;
            myUser = new User(-1);
        }
        /// <summary>
        /// Login user by username and password and set view by user type
        /// </summary>
        /// <param name="uname">Username</param>
        /// <param name="passw">Password</param>
        /// <returns>Succesful/Unsuccesful(true/false)</returns>
        public Boolean login(String uname, String passw)
        {

            myUserLogin = new Login(uname, passw);
            myUser = new User(myUserLogin.userId);
            if (myUserLogin.loggedIn)
                myViewFrame.setViewByUserType(myUserLogin.group);

            return myUserLogin.loggedIn;
        }
        /// <summary>
        /// Logout user , set guest view
        /// </summary>
        /// <returns>True</returns>
        public Boolean logout()
        {
            myUserLogin = null;
            myUser = new User(-1);
            myViewFrame.setGuestViewOptions();
            return true;
        }
        /// <summary>
        /// Fill user informations into view
        /// </summary>
        internal void getUserInfromation()
        {
            //fill user informations into registration form
            // MessageBox.Show("getUserInformation not implemented");

        }
        /// <summary>
        /// Checking information correctness and registering user 
        /// </summary>
        /// <param name="userinfo"></param>
        internal void registerUser(Dictionary<string, string> userinfo)
        {
            if (userinfo["pass1"] == userinfo["pass2"] && userinfo["pass1"].Length > 5)
            {
                if (userinfo["username"].Length == 0 || userinfo["email"].Length == 0)
                {
                    MessageBox.Show("Please fill the required fields");
                }
                else
                {

                    if (User.registerUser(userinfo))
                    {
                        myViewFrame.setUserandPass(userinfo["username"], userinfo["pass1"]);
                        myUser = new User(myUserLogin.userId);
                    }
                }

            }
            else
                MessageBox.Show("Passwords didn't match/Short password!(Min 6 char)");


        }

        /// <summary>
        /// User identifier(id) getter
        /// </summary>
        /// <returns>User identifier(Int)</returns>
        public int getUserId()
        {
            return myUser.userID;
        }

        /// <summary>
        /// Fill account with credit for pro users/Withdraw for publisher
        /// </summary>
        /// <param name="amount">Amount of credit</param>
        /// <param name="command">Fill/Withdraw</param>
        internal void fillwithdrawCredit(string amount, string command)
        {
            int a = Int32.Parse(amount.TrimEnd('$'));
            myUser.fillwithdraw(a, command);
        }
    }
}
