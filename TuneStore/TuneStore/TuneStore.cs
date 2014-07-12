using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuneStore.Controller;

namespace TuneStore
{
    /// <summary>
    /// Main form, the view layer
    /// </summary>
    public partial class MainFrame : Form
    {
        private Point mouseOffset;
        private bool isMouseDown = false;///page-colors dictionary
        private Dictionary<TabPage, Color> TabColors = new Dictionary<TabPage, Color>();//User actions controller
        private UserController myUserController;//Tracks controller
        private TracksController myTracksController;

        /// <summary>
        /// Main Frame constructor
        /// </summary>
        public MainFrame()
        {

            InitializeComponent();
            myUserController = new UserController(this);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainFrame_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainFrame_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainFrame_MouseUp);

            //tracksControl.TabPages.Remove(albumsPage);
           

            this.tracksControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tracksControl_DrawItem);
            SetTabHeader(tracksPage, Color.Lavender);
            SetTabHeader(cartPage, Color.Lavender);
            SetTabHeader(myContentPage, Color.Lavender);
            SetTabHeader(regTab, Color.Lavender);
            tracksControl.TabPages.Remove(regTab);
            tracksControl.TabPages.Add(regTab);
      

            setGuestViewOptions();
            fillTracksGrid();

        }

        public Runscriptfrm Runscriptfrm
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Request controller for tracks on tracks page(tab)
        /// </summary>
        private void fillTracksGrid()
        {
            myUserController = new UserController(this);
            myTracksController = new TracksController(this, myUserController);
            myTracksController.setFilters(artistBox.Text, albumBox.Text, titleBox.Text, genreBox.Text, keyBox.Text);

        }

        /// <summary>
        /// Fill tracks grid with DataTable, Invoked by controller 
        /// </summary>
        /// <param name="dt">DataTable with tracks data</param>
        internal void fillTracksGrid(DataTable dt)
        {
            tracksGrid.DataSource = dt.DefaultView;
            this.tracksGrid.Columns[0].Visible = false;

            //
        }

        /// <summary>
        /// Request cart grid fill from controller
        /// </summary>
        internal void fillCartGrid()
        {
            myTracksController.fillCartGrid();
        }

       /// <summary>
       /// Controller invoked cart grid fill
       /// </summary>
        /// <param name="dt">DataTable with tracks data</param>
        internal void fillCartGrid(DataTable dt)
        {
            cartGrid.DataSource = dt.DefaultView;
            this.cartGrid.Columns[0].Visible = false;

            //
        }

        /// <summary>
        /// Setting tab color
        /// </summary>
        /// <param name="page">Tab page</param>
        /// <param name="color">Color</param>
        private void SetTabHeader(TabPage page, Color color)
        {
            TabColors[page] = color;
            tracksControl.Invalidate();
        }

        /// <summary>
        /// Draw custom tabs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tracksControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            using (Brush br = new SolidBrush(TabColors[tracksControl.TabPages[e.Index]]))
            {
                e.Graphics.FillRectangle(br, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tracksControl.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tracksControl.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

                Rectangle rect = e.Bounds;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(Pens.Lavender, rect);
                e.DrawFocusRectangle();
            }
        }

        /// <summary>
        /// Mouse down event, required for custom form 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFrame_MouseDown(object sender,
    System.Windows.Forms.MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }

        /// <summary>
        /// Mose move event handler,  required for custom form 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFrame_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        /// <summary>
        /// Mouse up event, required for custom form  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFrame_MouseUp(object sender,
    System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        /// <summary>
        /// Custom close button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        /// <summary>
        /// Login event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void loginLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (loginLabel.Text == "Login")
            {

                if (myUserController.login(usernameBox.Text, passwordBox.Text))
                {

                    usernameBox.Enabled = false;
                    passwordBox.Enabled = false;
                    loginLabel.Text = "Logout";
                    myTracksController.setUserId(myUserController.getUserId());
                }
                else
                {
                    MessageBox.Show("Wrong username/password", "Login failed!");
                }
            }
            else
            {
                if (myUserController.logout())
                {
                    loginLabel.Text = "Login";
                    usernameBox.Text = "";
                    passwordBox.Text = "";
                    usernameBox.Enabled = true;
                    passwordBox.Enabled = true;
                    myTracksController.setUserId(myUserController.getUserId());
                }

            }
        }

        /// <summary>
        /// Pro/Free user registration handler.For pro users we need more informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void proCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (proCheckBox.Checked == true)
            {
                regTable2.Visible = true;
                publisherCheckBox.Enabled = false;
                publisherCheckBox.Checked = false;
                //regTable.Enabled = false;
            }
            else
            {
                regTable2.Visible = false;
                publisherCheckBox.Enabled = true;

            }

        }

        /// <summary>
        /// Registration button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regButton_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> userinfo = new Dictionary<String, String>();
            userinfo.Add("username", uNameBox.Text);
            userinfo.Add("pass1", passBox.Text);
            userinfo.Add("pass2", pass2Box.Text);
            userinfo.Add("email", emailBox.Text);
            userinfo.Add("birthdate", birthDatePicker.Text);
            bool justuser = false;
            if (publisherCheckBox.Checked == true)
            {
                userinfo.Add("type", "Publisher");
            }
            else
            {
                if (proCheckBox.Checked == true)
                {
                    userinfo.Add("type", "Pro");
                }
                else
                {
                    userinfo.Add("type", "User");
                    justuser = true;

                }
            }

            if (!justuser)
            {
                userinfo.Add("firstname", firstNamebox.Text);
                userinfo.Add("lastname", lastNameBox.Text);
                userinfo.Add("cardnum", cardNumBox.Text);
                userinfo.Add("valnum", valNumBox.Text);
                userinfo.Add("expdate", expDateBox.Text);
                userinfo.Add("initcredit", initCreditBox.Text);
            }
            myUserController.registerUser(userinfo);

        }

        /// <summary>
        /// Publisher registration needs more details 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void publisherCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (publisherCheckBox.Checked == true)
            {
                regTable2.Visible = true;
                proCheckBox.Enabled = false;
                //regTable.Enabled = true;
            }
            else
            {
                regTable2.Visible = false;
                proCheckBox.Enabled = true;

            }
        }

        /// <summary>
        /// Setting view by user's group
        /// </summary>
        /// <param name="utype">User type</param>
        public void setViewByUserType(String utype)
        {
            regTab.Text = "Account";
            myUserController.getUserInfromation();


            if(utype=="Pro"){
                cartAddButton.Visible = true;
                withdrawTable.Visible =false ; 
                fillCartGrid();
                fillMyTracksGrid();
                tracksControl.TabPages.Remove(regTab);
                trackAddButton.Visible = false;
                albumAddButton.Visible = false;
            }
            if (utype == "Admin")
            {
                cartAddButton.Visible = true;
                withdrawTable.Visible = true;
                regTable.Visible = false;
                regTable2.Visible = false;
                trackAddButton.Visible = true;
                albumAddButton.Visible = true;
                tracksControl.TabPages.Remove(regTab);
                tracksControl.TabPages.Add(cartPage);
                tracksControl.TabPages.Add(myContentPage);
                tracksControl.TabPages.Add(regTab);
                fillCartGrid();
                fillMyTracksGrid();
               
            }
            if (utype == "Publisher"){
                cartAddButton.Visible = false;
                withdrawTable.Visible = true;
                regTable.Visible = false;
                regTable2.Visible = false;
                trackAddButton.Visible = true;
                albumAddButton.Visible = true;
                tracksControl.TabPages.Remove(regTab);
                tracksControl.TabPages.Add(myContentPage);
                tracksControl.TabPages.Add(regTab);
            
                fillMyTracksGrid();

            }
            if (utype == "User")
            {
                cartAddButton.Visible = false;
                withdrawTable.Visible = false;
                regTable.Visible = false;
                regTable2.Visible = false;
                trackAddButton.Visible = true;
                albumAddButton.Visible = true;
             //   tracksControl.TabPages.Remove(cartPage);
                tracksControl.TabPages.Remove(regTab);
            
               // fillMyTracksGrid();

            }

        }

        /// <summary>
        /// Set view to guest 
        /// </summary>
        internal void setGuestViewOptions()
        {
            regTab.Text = "Register";
            withdrawTable.Visible = false;
            regTable2.Visible = false;
            cartAddButton.Visible = false;
            tracksControl.TabPages.Remove(cartPage);
            tracksControl.TabPages.Remove(myContentPage);
            regTable.Visible = true;
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Fill grid with user's purchased/added tracks 
        /// </summary>
        private void fillMyTracksGrid()
        {
            myTracksController.fillMyTracksGrid();
        }

        /// <summary>
        /// Automatic login after registration
        /// </summary>
        /// <param name="p1">Username</param>
        /// <param name="p2">Password</param>
        internal void setUserandPass(string p1, string p2)
        {
            usernameBox.Text = p1;
            passwordBox.Text = p2;
            loginLabel_LinkClicked(this, null);
        }

        /// <summary>
        /// Request controller for filtering tracks grid by introduced parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filterButton_Click(object sender, EventArgs e)
        {
            myTracksController.setFilters(artistBox.Text, albumBox.Text, titleBox.Text, genreBox.Text, keyBox.Text);
        }

        /// <summary>
        /// Grid selection changed handler,request track information change from controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tracksGrid_SelectionChanged(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in tracksGrid.SelectedRows)//csak egy van
            {
                int trackid = Convert.ToInt32(row.Cells[0].Value);
                myTracksController.setInfoLabels(trackid);
                myTracksController.setArtworkbox(trackid);
            }

        }

        /// <summary>
        /// Show track informations 
        /// </summary>
        /// <param name="trackinfo"></param>
        public void setInfoLabels(DataRow trackinfo)
        {

            if (trackinfo != null)
            {
                titleInf.Text = trackinfo["title"].ToString();
                artistInf.Text = trackinfo["name"].ToString();
                albumInf.Text = trackinfo["albumtitle"].ToString();
                genreInf.Text = trackinfo["genre"].ToString();
                keyInf.Text = trackinfo["keymark"].ToString();
                yearInf.Text = trackinfo["year"].ToString();
                bpmInf.Text = trackinfo["bpm"].ToString();
                qualityInf.Text = trackinfo["quality"].ToString();
                pathInf.Text = trackinfo["adress"].ToString();
                filenameInf.Text = trackinfo["filename"].ToString();
                commentInf.Text = trackinfo["comment"].ToString();

                //dr["adress"].ToString() + dr["filename"].ToString();
            }
        }
        /// <summary>
        /// Show artwork
        /// </summary>
        /// <param name="i">Artwork image </param>
        public void setArtworkBox(Image i)
        {
            artworkBox.Image = i;
            artworkBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        private void tracksGrid_Play(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            string trackpath;
            int trid = Convert.ToInt32(tracksGrid.SelectedRows[0].Cells[0].Value);
            trackpath = myTracksController.getTrackPath(trid);
            if (trackpath != "")
            {
                mediaPlayer.URL = @trackpath;
            }
            else
            {
                MessageBox.Show("Please login/register to play sounds!");
            }
        }
        /// <summary>
        /// Request track path for playing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cartGrid_Play(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            string trackpath;
            int trid = Convert.ToInt32(cartGrid.SelectedRows[0].Cells[0].Value);
            trackpath = myTracksController.getTrackPath(trid);
            if (trackpath != "")
            {
                mediaPlayer.URL = @trackpath;
            }
            else
            {
                MessageBox.Show("Please login/register to play sounds!");
            }
        }

        /// <summary>
        /// Cart add button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cartAddButton_Click(object sender, EventArgs e)
        {
            int trid = Convert.ToInt32(tracksGrid.SelectedRows[0].Cells[0].Value);
            int uid = myUserController.getUserId();
            myTracksController.addToCart(uid,trid);
            

        }

        /// <summary>
        /// Single track add button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackAddButton_Click(object sender, EventArgs e)
        {
            String dbFillScript = " D:\\Egyetem\\AB2\\Perl\\Adatbazis\\dbconnect.pl ";
            String userid = myUserController.getUserId() + " ";

            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            // Console.WriteLine(fd.FileName);
           
            // string script = Utils.dbFillScript + " \"" + fb.SelectedPath + "\" " + myDatabase;
            String script = dbFillScript + " \"" + fd.FileName + "\" " + userid;
            MessageBox.Show(script);
            (new Runscriptfrm(script)).ShowDialog();
            myTracksController.setFilters(artistBox.Text, albumBox.Text, titleBox.Text, genreBox.Text, keyBox.Text);
           
        }

        /// <summary>
        /// Album add button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void albumAddButton_Click(object sender, EventArgs e)
        {
            String dbFillScript = " D:\\Egyetem\\AB2\\Perl\\Adatbazis\\dbconnect.pl ";
            String userid = myUserController.getUserId() + " ";

            FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.ShowDialog();
          
                String script = dbFillScript + " \"" + fb.SelectedPath + "\" " + userid;
            MessageBox.Show(script);
            (new Runscriptfrm(script)).ShowDialog();
            myTracksController.setFilters(artistBox.Text, albumBox.Text, titleBox.Text, genreBox.Text, keyBox.Text);

        }

        /// <summary>
        /// Fill user tracks grid 
        /// </summary>
        /// <param name="dt"></param>
        internal void fillUserTracksGrid(DataTable dt)
        {
            
                myTracksGrid.DataSource = dt.DefaultView;
                this.myTracksGrid.Columns[0].Visible = false;

           
        }

        /// <summary>
        /// Cart checkout button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cartCheckoutButton_Click(object sender, EventArgs e)
        {
            myTracksController.checkoutCart();
            fillCartGrid();
            fillMyTracksGrid();
        }

        /// <summary>
        /// Withdraw credit button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void withdrawButton_Click(object sender, EventArgs e)
        {
            myUserController.fillwithdrawCredit(withdrawBox.Text,"Withdraw");
        }

        /// <summary>
        /// Fill account with credit handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fillButton_Click(object sender, EventArgs e)
        {
            myUserController.fillwithdrawCredit(fillBox.Text, "Fill");
        }

    }
}
