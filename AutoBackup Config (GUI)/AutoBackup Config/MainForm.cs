using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Security.Cryptography;

namespace AutoBackup_Config
{

    // old skool winforms app for configuring the AutoBackup service

    public partial class MainForm : Form
    {
        // controller for managing windows services
        private ServiceController serviceController;
        // service name to manage (AutoBackup is the service name)
        string serviceName = "AutoBackup"; 
        private int savedLabelFlashCount = 0;
        string xmlLocation = AppDomain.CurrentDomain.BaseDirectory + @"\Settings.xml";
        string logsLocation = AppDomain.CurrentDomain.BaseDirectory + @"Logs";
        public MainForm()
        {
            InitializeComponent();
            // initialise folder browser dialog
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            serviceController = new ServiceController(serviceName); // initialise service controller with serviceName variable
            serviceStatusTimer.Tick += new EventHandler(ServiceStatusTimer_Tick);   // add event handler to timer interval
            serviceStatusTimer.Start(); // start timer
        }

 
        // function for opening a folder in the explorer
        public static void OpenFolder(string folderPath)
        {
            // check if folder exists before trying to open it
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("The folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Form Load
        private void MainForm_Load(object sender, EventArgs e)
        {
            ApplicationData loadedData = LoadData(xmlLocation);
            textBoxTargetFolder.Text = loadedData.TargetFolder;

            if (!Directory.Exists(textBoxTargetFolder.Text))
            {
                MessageBox.Show("The backup folder does not exist. Please make sure you meant to do that.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bool saveAfterLoad = false;
            listBoxFolderSources.Items.Clear();
            foreach (string folder in loadedData.FolderSources)
            {
                saveAfterLoad |= ProcessFolder(folder);
            }

            if (saveAfterLoad)
            {
                SaveSettings();
            }
        }

        private bool ProcessFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                if (MessageBox.Show($"The folder '{folder}' does not exist. Do you want to remove it from the list?", "Folder not found", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    return true; // Indicates that saving is needed
                }
            }

            listBoxFolderSources.Items.Add(folder);
            return false; // No need to save
        }

        #endregion

        #region Service Start/Stop/Status
        private void BtnServiceStart_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void BtnServiceStop_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void StartService()
        {
            try
            {
                // if service isn't running, start it
                if (serviceController.Status != ServiceControllerStatus.Running)
                {
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    MessageBox.Show("Service started successfully.");
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Error starting service: {ex.Message}");
            }
        }

        private void StopService()
        {
            try
            {
                // if service is running, stop it
                if (serviceController.Status != ServiceControllerStatus.Stopped)
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    MessageBox.Show("Service stopped successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping service: {ex.Message}");
            }
        }

        // timer for calling function to check the service status and updating the UI
        private void ServiceStatusTimer_Tick(object sender, EventArgs e)
        {
            bool isRunning = IsServiceRunning(serviceName);
            if (isRunning)
            {
                LblServiceStatus.Text = "Active";
                LblServiceStatus.ForeColor = Color.Green;
            }
            else
            {
                LblServiceStatus.Text = "Not Active";
                LblServiceStatus.ForeColor = Color.Red;
            }
        }

        // function to check if the service is running
        private bool IsServiceRunning(string serviceName)
        {
            try
            {
                serviceController.Refresh(); // get current status of the service
                return serviceController.Status == ServiceControllerStatus.Running;
            }
            catch (InvalidOperationException)
            {
                // can't seem to find the service
                return false;
            }
        }



        #endregion

        #region Add folder button and check

        // add folder button and check

        private void BtnFolderAdd_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string selectedPath = folderBrowserDialog.SelectedPath;

            if (IsSubfolderOfTarget(selectedPath) || IsSameAsTarget(selectedPath) || IsAlreadyInList(selectedPath))
            {
                return;
            }

            if (IsParentFolderInList(selectedPath))
            {
                MessageBox.Show("A parent folder of this folder is already in the list.");
            }
            else
            {
                AddFolderToList(selectedPath);
            }
        }

        private bool IsSubfolderOfTarget(string folderPath)
        {
            if (!string.IsNullOrEmpty(textBoxTargetFolder.Text) &&
                folderPath.StartsWith(textBoxTargetFolder.Text + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("You cannot add a subfolder of the target folder.");
                return true;
            }

            return false;
        }

        private bool IsSameAsTarget(string folderPath)
        {
            if (string.Equals(textBoxTargetFolder.Text, folderPath, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("You cannot add the same folder as the target folder.");
                return true;
            }

            return false;
        }

        private bool IsAlreadyInList(string folderPath)
        {
            if (listBoxFolderSources.Items.Cast<string>().Any(item => string.Equals(item, folderPath, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("This folder is already in the list.");
                return true;
            }

            return false;
        }

        private bool IsParentFolderInList(string folderPath)
        {
            return listBoxFolderSources.Items.Cast<string>().Any(item => folderPath.StartsWith(item + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
        }

        private void AddFolderToList(string folderPath)
        {
            var subFolders = listBoxFolderSources.Items.Cast<string>()
                                .Where(item => item.StartsWith(folderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                                .ToList();

            if (subFolders.Any())
            {
                if (MessageBox.Show("This folder is a parent to other folders in the list. Remove them and add this folder?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (var subFolder in subFolders)
                    {
                        listBoxFolderSources.Items.Remove(subFolder);
                    }
                    listBoxFolderSources.Items.Add(folderPath);
                }
            }
            else
            {
                listBoxFolderSources.Items.Add(folderPath);
                CopyFilesToTargetStructure(folderPath);
                MessageBox.Show("Folder added successfully.\nRestart the service to include this new folder in the automatic backups.");
                SaveSettings();
            }
        }
        #endregion

        #region Change target folder
        // change target folder

        private void BtnChangeTargetFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;

                if (IsPathConflictingWithSourceList(selectedPath))
                {
                    MessageBox.Show("The selected folder cannot be the same as or a subfolder of a folder in the sources list.");
                    return;
                }

                if (IsTargetFolderParentOfAnySource(selectedPath))
                {
                    MessageBox.Show("The selected folder cannot be a parent of any folder in the sources list.");
                    return;
                }

                textBoxTargetFolder.Text = selectedPath;
                MessageBox.Show("Target folder changed successfully.\nAn integrity check will now be performed and create new backups.");
                IntegrityCheck();
                SaveSettings();
            }
        }


        private bool IsPathConflictingWithSourceList(string path)
        {
            foreach (var item in listBoxFolderSources.Items)
            {
                string existingPath = item.ToString();
                if (string.Equals(path, existingPath, StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith(existingPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTargetFolderParentOfAnySource(string targetFolderPath)
        {
            foreach (var item in listBoxFolderSources.Items)
            {
                string sourcePath = item.ToString();
                if (sourcePath.StartsWith(targetFolderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        private void UpdateTargetFolder(string path)
        {
            textBoxTargetFolder.Text = path;
        }

        private void PerformPostUpdateActions()
        {
            MessageBox.Show("Target folder changed successfully.\nAn integrity check will now be performed and create new backups.");
            IntegrityCheck();
            SaveSettings();
        }

#endregion

        // remove folder from backup list

        private void BtnFolderRemove_Click(object sender, EventArgs e)
        {

            if (listBoxFolderSources.SelectedIndex != -1)
            {
                string destinationFolderPath = GetDestinationFolderPath(listBoxFolderSources.SelectedItem.ToString());

                if (MessageBox.Show($"Do you also want to delete the backup of this folder?\nThis cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        // delete the folder from the hard drive
                        Directory.Delete(destinationFolderPath, true); // true for recursive delete
                        MessageBox.Show($"Folder '{destinationFolderPath}' has been deleted.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting folder: {ex.Message}");
                    }
                }

                // remove the folder from the listBox in either case
                listBoxFolderSources.Items.RemoveAt(listBoxFolderSources.SelectedIndex);
                SaveSettings();
            }
            else
            {
                MessageBox.Show("Please select a folder to remove.");
            }
        }

        private void BtnOpenLogs_Click(object sender, EventArgs e)
        {
            OpenFolder(logsLocation);
        }

        public void SaveData(string filePath, ApplicationData data)
        {
            // create XmlSerializer instance
            XmlSerializer serializer = new XmlSerializer(typeof(ApplicationData));
            // create a FileStream for writing, using 'using' to automatically close it when done
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // serialise the data and write it to the file
                serializer.Serialize(writer, data);
            }
        }

        public ApplicationData LoadData(string filePath)
        {
            // create XmlSerializer instance
            XmlSerializer serializer = new XmlSerializer(typeof(ApplicationData));
            // create a FileStream for reading, using 'using' to automatically close it when done
            using (StreamReader reader = new StreamReader(filePath))
            {
                // deserialize and return the object
                return (ApplicationData)serializer.Deserialize(reader);
            }
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            try
            {
                // create new ApplicationData object and populate it with the data from the form
                ApplicationData data = new ApplicationData();
                data.TargetFolder = textBoxTargetFolder.Text;
                data.FolderSources.AddRange(listBoxFolderSources.Items.Cast<string>());

                // location of the xml file to save to
                SaveData(xmlLocation, data);

                ShowSavedStatus();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // function to call for flashing the "Settings Saved" label
        private void ShowSavedStatus()
        {
            lblSettingsSaved.Visible = true;
            savedIndicatorTimer.Interval = 500; // interval in milliseconds
            savedIndicatorTimer.Tick += new EventHandler(savedIndicatorTimer_Tick);
            savedIndicatorTimer.Start();
        }


        private void savedIndicatorTimer_Tick(object sender, EventArgs e)
        {
            lblSettingsSaved.Visible = !lblSettingsSaved.Visible;

            savedLabelFlashCount++;

            if (savedLabelFlashCount >= 6)
            {
                savedIndicatorTimer.Stop();
                // unsubscribe from the event
                savedIndicatorTimer.Tick -= savedIndicatorTimer_Tick;
                // make sure the label is hidden
                lblSettingsSaved.Visible = false;
                // reset the counter for next time
                savedLabelFlashCount = 0; 
            }
        }

        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // open the link in the default browser
                string url = "https://www.dries.jp";
                // start new process. 'UseShellExecute' = true allows opening filenames in default applications, in this case an HTTP link
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open the link. Error: " + ex.Message);
            }
        }

        private void BtnIntegrityCheck_Click(object sender, EventArgs e)
        {
            IntegrityCheck();
        }

        private string GetDestinationFolderPath(string sourceFolderPath)
        {
            // path of the target folder for storing backups
            string targetBasePath = textBoxTargetFolder.Text;
            // remove the colon from the relative path so the drive letter is a folder name
            string relativePath = sourceFolderPath.Replace(":", "");
            // combine the target folder path and path from source
            string destinationFolderPath = Path.Combine(targetBasePath, relativePath);
            return destinationFolderPath;
        }


        #region Integrity Check
        // integrity check function compares the original files with the backup files and copies any that are missing or different
        private void IntegrityCheck()
        {
            InitializeProgressBar();
            int copiedFilesCount = 0;
            try
            {
                var allFiles = GetAllFilesFromSources();
                IntegrityCheckProgress.Maximum = allFiles.Count;

                foreach (string sourceFilePath in allFiles)
                {
                    if (ShouldCopyFile(sourceFilePath))
                    {
                        CopyFileToDestination(sourceFilePath);
                        copiedFilesCount++;
                    }
                    UpdateProgressBar();
                }

                ShowCompletionMessage(allFiles.Count, copiedFilesCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                IntegrityCheckProgress.Visible = false;
            }
        }

        private void InitializeProgressBar()
        {
            IntegrityCheckProgress.Value = 0;
            IntegrityCheckProgress.Visible = true;
        }

        private List<string> GetAllFilesFromSources()
        {
            return listBoxFolderSources.Items.Cast<string>()
                .SelectMany(sourceFolderPath => Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories))
                .ToList();
        }

        private bool ShouldCopyFile(string sourceFilePath)
        {
            string sourceFolderPath = Path.GetDirectoryName(sourceFilePath);
            string destinationFilePath = sourceFilePath.Replace(sourceFolderPath, GetDestinationFolderPath(sourceFolderPath));

            if (File.Exists(destinationFilePath))
            {
                return !FilesAreEqual(sourceFilePath, destinationFilePath);
            }

            return true;
        }

        private void CopyFileToDestination(string sourceFilePath)
        {
            string sourceFolderPath = Path.GetDirectoryName(sourceFilePath);
            string destinationFilePath = sourceFilePath.Replace(sourceFolderPath, GetDestinationFolderPath(sourceFolderPath));

            Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
            File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
        }

        private void UpdateProgressBar()
        {
            IntegrityCheckProgress.Value++;
        }

        private void ShowCompletionMessage(int totalFiles, int copiedFiles)
        {
            MessageBox.Show($"Integrity check complete.\n\nTotal Files Checked: {totalFiles}\nFiles Copied: {copiedFiles}");
        }

#endregion

        // function to compare the MD5 hashes of two files
        private bool FilesAreEqual(string filePath1, string filePath2)
        {
            // create MD5 instance object
            using (var md5 = MD5.Create())
            {
                // open the files as streams and compute the hashes
                using (var stream1 = File.OpenRead(filePath1))
                using (var stream2 = File.OpenRead(filePath2))
                {
                    byte[] hash1 = md5.ComputeHash(stream1);
                    byte[] hash2 = md5.ComputeHash(stream2);

                    // compare the hashes and return the result as a boolean
                    return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
                }
            }
        }

        #region Copy Files to Target Structure

        // derived from integrity check function. it copies the newly added folder to the backup folder
        private void CopyFilesToTargetStructure(string sourceDirectory)
        {
            InitializeProgressBarForCopying(sourceDirectory);

            int filesCopiedCount = 0;
            try
            {
                var allSourceFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

                foreach (string sourceFilePath in allSourceFiles)
                {
                    if (ShouldCopyFileToTarget(sourceFilePath))
                    {
                        CopyFile(sourceFilePath);
                        filesCopiedCount++;
                    }
                    UpdateProgressBar();
                }

                ShowCompletionMessageForCopying(filesCopiedCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while copying files: {ex.Message}");
            }
            finally
            {
                IntegrityCheckProgress.Visible = false;
            }
        }

        private void InitializeProgressBarForCopying(string sourceDirectory)
        {
            var allSourceFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            IntegrityCheckProgress.Visible = true;
            IntegrityCheckProgress.Value = 0;
            IntegrityCheckProgress.Maximum = allSourceFiles.Length;
        }

        private bool ShouldCopyFileToTarget(string sourceFilePath)
        {
            string sourceFolderPath = Path.GetDirectoryName(sourceFilePath);
            string destinationFilePath = sourceFilePath.Replace(sourceFolderPath, GetDestinationFolderPath(sourceFolderPath));

            if (File.Exists(destinationFilePath))
            {
                return !FilesAreEqual(sourceFilePath, destinationFilePath);
            }

            return true;
        }

        private void CopyFile(string sourceFilePath)
        {
            string sourceFolderPath = Path.GetDirectoryName(sourceFilePath);
            string destinationFilePath = sourceFilePath.Replace(sourceFolderPath, GetDestinationFolderPath(sourceFolderPath));

            Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath)); // Ensure destination directory exists
            File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
        }

        private void ShowCompletionMessageForCopying(int filesCopiedCount)
        {
            MessageBox.Show($"File copying complete.\nFiles Copied: {filesCopiedCount}");
        }


        #endregion

    }

    // class for storing the data for the XML file
    public class ApplicationData
    {
        // properties for the source folders and target folder
        public List<string> FolderSources { get; set; }
        public string TargetFolder { get; set; }

        public ApplicationData()
        {
            FolderSources = new List<string>();
        }
    }
}
