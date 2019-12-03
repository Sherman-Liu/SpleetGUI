using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Collections.ObjectModel;
namespace spleetGUI
{
    public partial class Form1 : Form
    {
        public string arg_var =" ";
        public string file_name = " ";
        public string output_folder = "output";
        public int status = 0;
        public string seperation = "2stems";
        public Form1()
        {
            
            
            InitializeComponent();

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.WorkerReportsProgress = true;

        }
        private void LoadSettings()
        {
            if (Properties.Settings.Default.OutputPath != null)
            {
               
                output_folder = Properties.Settings.Default.OutputPath;
            }
          
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            checkifinstalled();
        }

        private void checkifinstalled()
        {
            UpdateStatus();
            run_cmd_arg("pip install spleeter");
            
        }
        private void run_cmd_file(string filepath)
        {

            string fileName = filepath;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Console.WriteLine("Starting:" + fileName + " Using Python");
            Console.WriteLine(output);

            Console.ReadLine();

        }

        private void addtopath()
        {

            const string name = "PATH";
            string pathvar = System.Environment.GetEnvironmentVariable(name);
            var value = pathvar + @";"+ @"c:\ffmpeg";
            var target = EnvironmentVariableTarget.Machine;
            System.Environment.SetEnvironmentVariable(name, value, target);
           


        }
        
        private void InstallFFMPEG()
        {


            string installpath = @"c:\ffmpeg";

            if (File.Exists(installpath + @"\ffmpeg.exe"))
            {
                Console.WriteLine("ffmpeg already installed");
                label1.Text = "ffmpeg already installed";
                return;
            }

            addtopath();
            DirectoryInfo di = Directory.CreateDirectory(installpath);
            Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(installpath));

            File.Copy(Directory.GetCurrentDirectory() + @"\ffmodule\ffmpeg.exe", installpath + @"\ffmpeg.exe");
            File.Copy(Directory.GetCurrentDirectory() + @"\ffmodule\ffplay.exe", installpath + @"\ffplay.exe");
            File.Copy(Directory.GetCurrentDirectory() + @"\ffmodule\ffprobe.exe", installpath + @"\ffprobe.exe");
            Console.WriteLine("ffmpeg suscefuly installed");
            label1.Text = "ffmpeg suscefully installed";
            
        }
        private void run_cmd_arg(string arg)
        {
            


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = "/c " + arg,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

          


            if (p.HasExited)
            {
                textBox2.Text += output;
                if (arg_var.Contains("separate"))
                {

                    //splitting done
                    status = 1;
                   // Console.WriteLine(@output_folder + @"\" + @Path.GetFileNameWithoutExtension(textBox1.Text) + @"\");
                    //Console.ReadLine();
                  Process.Start(@output_folder + @"\" + @Path.GetFileNameWithoutExtension(textBox1.Text)+ @"\");
                    
                }
               
                if (output.Contains("INFO:spleeter:File"))
                {
                    //splitting done alt
                    status = 1;
                    Process.Start(@output_folder+@"\" + Path.GetFileNameWithoutExtension(textBox2.Text) + @"\");
                }else if (output.Contains("Requirement already satisfied"))
                {
                    //spleeter already installed
                    status = 2;
                }

                
                UpdateStatus();
            }





            Console.WriteLine(output);
            Console.ReadLine();
            p.Close();
        }
        private void UpdateStatus()
        {

            if (status == 0)
            {
                label1.Text = "";
            } else if (status == 1) {
               label1.Text = "Splitting Finished";
                button1.Enabled = true;
               // button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
              //  backgroundWorker1.Dispose();
            }
            else if (status == 2)
            {
              
                button2.BackColor = Color.GreenYellow;
                button2.Text = "Spleeter Installed";
            }
            else if (status == 3)
            {

                //splitting
                label1.Text = "Please wait while Spleeter is splitting your file";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            //splitfile
            if (folderBrowserDialog1.SelectedPath == "")
            {
                folderBrowserDialog1.ShowDialog();
            }
            else
            {

                InstallFFMPEG();



                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;


                output_folder = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.OutputPath = output_folder;
                Properties.Settings.Default.Save();

                Console.WriteLine("spleeter separate -i '" + textBox1.Text + "' -p spleeter:" + seperation + " -o " + @output_folder);
                Console.ReadLine();

                status = 3;
                UpdateStatus();


                
                arg_var = "spleeter separate -i '" + textBox1.Text + "' -p spleeter:" + seperation + " -o " + "'" + @output_folder+"'";

                 run_cmd_arg("spleeter separate -i '" + textBox1.Text + "' -p spleeter:" + seperation + " -o " + "'" + @output_folder + "'");
               // backgroundWorker1.RunWorkerAsync();
            }
        





        }

        private void button2_Click(object sender, EventArgs e)
        {
            //install spleeter
            Console.WriteLine("Running command: pip install spleeter");
            Console.ReadLine();

            arg_var = "pip install spleeter";
            run_cmd_arg("pip install spleeter");


        }

      
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            file_name = openFileDialog1.FileName;
            textBox1.Text = openFileDialog1.FileName;
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            openFileDialog1.ShowDialog();
        }



        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

           
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = "/c " + arg_var,
                RedirectStandardOutput = true,
                UseShellExecute = true,
                CreateNoWindow = true
            };

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();




            if (p.HasExited)
            {
                textBox2.Text += output;
                if (arg_var.Contains("separate"))
                {

                    //splitting done
                    status = 1;
                    // Console.WriteLine(@output_folder + @"\" + @Path.GetFileNameWithoutExtension(textBox1.Text) + @"\");
                    //Console.ReadLine();
                    Process.Start(@output_folder + @"\" + @Path.GetFileNameWithoutExtension(textBox1.Text) + @"\");
                

                }

                if (output.Contains("INFO:spleeter:File"))
                {
                    //splitting done alt
                    status = 1;
                    Process.Start(@output_folder + @"\" + Path.GetFileNameWithoutExtension(textBox2.Text) + @"\");
                }
                else if (output.Contains("Requirement already satisfied"))
                {
                    //spleeter already installed
                    status = 2;
                }


                UpdateStatus();
            }





            Console.WriteLine(output);
            Console.ReadLine();
            p.Close();

        }



        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
         
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

          
            if (e.Cancelled)
            {
                textBox2.Text = "Process was cancelled";
            }
            else if (e.Error != null)
            {
                    textBox2.Text = "There was an error running the process. The thread aborted";
            }
            else
            {
                   textBox2.Text = "Process was completed";
            }

            UpdateStatus();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(221, 4, 38);
            button4.BackColor = Color.Gainsboro;
            button5.BackColor = Color.Gainsboro;
            seperation = "2stems";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.Gainsboro;
            button4.BackColor = Color.FromArgb(221, 4, 38);
            button5.BackColor = Color.Gainsboro;
            seperation = "4stems";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.Gainsboro;
            button4.BackColor = Color.Gainsboro;
            button5.BackColor = Color.FromArgb(221, 4, 38);
            seperation = "5stems";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            InstallFFMPEG();
        }
    }


   
    }


