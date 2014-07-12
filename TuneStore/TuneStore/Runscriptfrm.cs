using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuneStore
{
    /// <summary>
    /// A form for running a given script, shows the console and the error output.
    /// </summary>
    public partial class Runscriptfrm : Form
    {
        // private string dbName;
        // private string hostName;
        private string scriptFile; //vegrahajtando script

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="script">String to script path</param>
        public Runscriptfrm(string script)
        {
            InitializeComponent();

            scriptFile = script;

            // Console.WriteLine(dbName);

        }

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Runscriptfrm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Start action 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(dbName);
            ProcessStartInfo perlStartInfo = new ProcessStartInfo(@"C:\Perl64\bin\perl.exe ");
            perlStartInfo.Arguments = scriptFile;


            perlStartInfo.UseShellExecute = false;
            perlStartInfo.RedirectStandardOutput = true;
            perlStartInfo.RedirectStandardError = true;
            perlStartInfo.CreateNoWindow = true;

            Process perl = new Process();
            perl.StartInfo = perlStartInfo;
            perl.OutputDataReceived += new DataReceivedEventHandler(consoleOutputHandler);
            perl.ErrorDataReceived += new DataReceivedEventHandler(errorOutputHandler);

            perl.Start();

            perl.BeginOutputReadLine();//aszinkron olvasas
            perl.BeginErrorReadLine();
            
            //az olvasas igy blokalodik amig veget nem er a script
            /* using (StreamReader reader = perl.StandardOutput)
             {
                 string result ;
                 while ((result = reader.ReadLine()) != null)
                 {
                 
                 // richTextBox2.AppendText(myString2 + "\n");
                 //myString = myConsoleReader.ReadLine();
                 //myString2 = myStreamReader2.ReadLine(); 
                   
                 }
                    
                      
             }*/
            /*   StreamReader reader = perl.StandardError;
                string error = perl.StandardError.ReadToEnd();
                errorBox.AppendText(error);
                */

            //   Process perl = new Process();
            //   perl.StartInfo = perlStartInfo;

            //   perl.Start();


            /* StreamReader myConsoleReader = perl.StandardOutput;
             StreamReader myErrorReader = perl.StandardError;
             String myString = myConsoleReader.ReadLine();
             //  String myString2 = myStreamReader2.ReadLine();
             while (!String.IsNullOrEmpty(myString))
             {
               
             }

             */

            perl.WaitForExit();
            perl.Close();

        }

        /// <summary>
        /// Writing  error output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="outLine"></param>
        private void errorOutputHandler(object sender, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                // Console.WriteLine(outLine.Data);
                errorBox.AppendText(outLine.Data + "\n");
                errorBox.ScrollToCaret();

            }
        }

        /// <summary>
        /// Writing console output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="outLine"></param>
        private void consoleOutputHandler(object sender, DataReceivedEventArgs outLine)
        {
           
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                //Console.WriteLine(outLine.Data);
                consoleBox.AppendText(outLine.Data + "\n");
                consoleBox.ScrollToCaret();
            }
        }

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doneBut_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
