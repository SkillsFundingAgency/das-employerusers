using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using SFA.DAS.EmployerUsers.Domain;

namespace BulkUserCreator
{
    public partial class Form1 : Form
    {
        private const string Password = "Pa55word";
        private bool _isRunning = false;
        private CancellationTokenSource _cancellationTokenSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                SetUiEnabled(false);
                btnStartStop.Text = "Stop";
                _isRunning = true;

                _cancellationTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(Run, _cancellationTokenSource.Token)
                    .ContinueWith((result) =>
                    {
                        Invoke(new Action(() =>
                        {
                            if (result.Exception != null)
                            {
                                MessageBox.Show(result.Exception.ToString(), "Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            SetUiEnabled(true);
                            btnStartStop.Text = "Start";
                            btnStartStop.Enabled = true;
                            pbProgress.Style = ProgressBarStyle.Continuous;
                            pbProgress.Value = 0;
                        }));
                    });
            }
            else
            {
                btnStartStop.Enabled = false;
                _cancellationTokenSource.Cancel();
            }
        }
        private void SetUiEnabled(bool enabled)
        {
            txtUsersConnStr.Enabled = enabled;
            txtProfileId.Enabled = enabled;
            txtProfileId.Enabled = enabled;
            txtNameFormat.Enabled = enabled;
            nudNoUsers.Enabled = enabled;
        }
        private void UpdateStatus(int userIndex)
        {
            lblStatus.Text = $"Finished user {userIndex} of {pbProgress.Maximum}";
            if (userIndex >= pbProgress.Maximum)
            {
                return;
            }

            pbProgress.Value = userIndex;
            pbProgress.Style = ProgressBarStyle.Continuous;
        }



        private void Run()
        {
            var usersConnectionString = txtUsersConnStr.Text;
            var profilesConnectionString = txtProfilesConnStr.Text;
            var profileId = txtProfileId.Text;
            var nameFormat = txtNameFormat.Text;
            var numberOfUsers = (int)nudNoUsers.Value;

            Invoke(new Action(() =>
            {
                pbProgress.Value = 0;
                pbProgress.Maximum = numberOfUsers;
                pbProgress.Style = ProgressBarStyle.Marquee;
                lblStatus.Text = "Starting";
            }));

            CreateUsers(usersConnectionString, profilesConnectionString, profileId, nameFormat, numberOfUsers).Wait();
        }

        private async Task CreateUsers(string usersConnectionString, string profilesConnectionString, string profileId, string nameFormat, int numberOfUsers)
        {
            var profile = await GetProfile(profilesConnectionString, profileId);
            var userNumberLength = numberOfUsers.ToString().Length;

            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[profile.SaltLength];
            rng.GetBytes(salt);
            var password = SecurePassword(Password, salt, profile);

            using (var stream = new FileStream(@"C:\temp\loadtestusers.csv", FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            using (var connection = new SqlConnection(usersConnectionString))
            {
                await writer.WriteLineAsync("Username,Password");

                for (var i = 1; i <= numberOfUsers; i++)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    var username = string.Format(nameFormat, i.ToString().PadLeft(userNumberLength, '0'));
                    await CreateUser(connection, username, password, salt, profile, rng);

                    await writer.WriteLineAsync($"\"{username}\",\"{Password}\"");

                    Invoke(new Action(() =>
                    {
                        UpdateStatus(i);
                    }));
                }

                await writer.FlushAsync();
            }
        }
        private async Task<PasswordProfile> GetProfile(string profilesConnectionString, string profileId)
        {
            using (var connection = new SqlConnection(profilesConnectionString))
            {
                var resultset = await connection.QueryAsync<PasswordProfile>("SELECT * FROM PasswordProfile WHERE Id=@profileId", new { profileId });
                return resultset.SingleOrDefault();
            }
        }
        private async Task CreateUser(SqlConnection connection, string username, string password, byte[] salt, PasswordProfile profile, RNGCryptoServiceProvider rng)
        {
            var firstName = SampleData.GetRandomSelectionFrom(SampleData.FirstNames);
            var lastName = SampleData.GetRandomSelectionFrom(SampleData.LastNames);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Email = $"{username}@loadtest.local",
                Password = password,
                Salt = Convert.ToBase64String(salt),
                PasswordProfileId = profile.Id,
                IsActive = true,
                FailedLoginAttempts = 0,
                IsLocked = false
            };

            await connection.ExecuteAsync("CreateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked",
                user);

            await connection.ExecuteAsync("CreateHistoricalPassword @UserId, @Password, @Salt, @PasswordProfileId, @DateSet",
                   new { UserId = user.Id, user.Password, user.Salt, user.PasswordProfileId, DateSet = DateTime.Now });
        }
        private string SecurePassword(string plainText, byte[] salt, PasswordProfile profile)
        {
            //TODO: Refactor password service code to make it consumable from here too
            var saltedPassword = salt.Concat(Encoding.Unicode.GetBytes(plainText)).ToArray();

            var hasher = new HMACSHA256(Convert.FromBase64String(profile.Key));
            var hash = hasher.ComputeHash(saltedPassword);

            var pbkdf2 = new Rfc2898DeriveBytes(Convert.ToBase64String(hash), salt, profile.WorkFactor);
            var password = pbkdf2.GetBytes(profile.StorageLength);

            return Convert.ToBase64String(password);
        }
    }
}
