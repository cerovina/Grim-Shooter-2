using System;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace GrimShooter2
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer gameMedia;
        WindowsMediaPlayer shootgMedia;
        WindowsMediaPlayer explosion;

        PictureBox[] enemiesMunition;
        int enemiesMunitionSpeed;

        PictureBox[] stars;
        int backgroundspeed;
        int playerSpeed;
        Random rnd;

        PictureBox[] ammo;
        int ammoSpeed;

        PictureBox[] enemies;
        int enemySpeed;

        Label scoreLabel;
        Label finalScoreLabel;

        public Form1()
        {
            InitializeComponent();

            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Arial", 12, FontStyle.Regular);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.AutoSize = true;
            scoreLabel.Location = new Point(10, 10);
            this.Controls.Add(scoreLabel);

            // Add a label for displaying the final score after the game is over
            finalScoreLabel = new Label();
            finalScoreLabel.Text = "Final Score:";
            finalScoreLabel.Font = new Font("Arial", 20, FontStyle.Bold);
            finalScoreLabel.ForeColor = Color.White;
            finalScoreLabel.AutoSize = true;
            finalScoreLabel.Visible = false;
            finalScoreLabel.Location = new Point(270, 315);
            this.Controls.Add(finalScoreLabel);
        }

        int score;
        int level;
        int difficulty;
        bool pause;
        bool gameIsOver;

        private void Form1_Load(object sender, EventArgs e)
        {
            label1 = new Label();
            label1.Location = new Point(this.Width / 2, this.Height / 2);
            this.Controls.Add(label1);

            pause = false;
            gameIsOver = false;
            score = 0;
            level = 1;
            difficulty = 9;
            backgroundspeed = 1;
            playerSpeed = 3;
            enemySpeed = 6;
            ammoSpeed = 10;
            enemiesMunitionSpeed = 20;
            ammo = new PictureBox[3];

            //Load images
            Image ammunition = Image.FromFile(@"asserts\puca.png");
            Image enemyAmmunition = Image.FromFile(@"asserts\laser.png");

            Image enemy1 = Image.FromFile(@"asserts\heretic.png");
            Image enemy2 = Image.FromFile(@"asserts\heretic.png");
            Image enemy3 = Image.FromFile(@"asserts\E3.png");
            Image boss1 = Image.FromFile(@"asserts\Boss1.png");
            Image boss2 = Image.FromFile(@"asserts\Boss2.png");

            enemies = new PictureBox[1];

            //Initialize enemy picture boxes

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox();
                rnd = new Random();
                enemies[i].Size = new Size(75, 75);
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                enemies[i].BorderStyle = BorderStyle.None;
                enemies[i].Visible = false;
                this.Controls.Add(enemies[i]);
                enemies[i].Location = new Point(this.Width - enemies[i].Width + rnd.Next(50, 200), rnd.Next(this.Height - enemies[i].Height));
            }

            enemies[0].Image = enemy1;

            for (int i = 0; i < ammo.Length; i++)
            {
                ammo[i] = new PictureBox();
                ammo[i].Size = new Size(30, 30);
                ammo[i].Image = ammunition;
                ammo[i].SizeMode = PictureBoxSizeMode.Zoom;
                ammo[i].BorderStyle = BorderStyle.None;
                this.Controls.Add(ammo[i]);
            }

            //Create WMP
            gameMedia = new WindowsMediaPlayer();
            shootgMedia = new WindowsMediaPlayer();
            explosion = new WindowsMediaPlayer();

            //Load all songs
            gameMedia.URL = "songs\\song.mp3";

            //Set up song settings
            gameMedia.settings.setMode("loop", true);
            gameMedia.settings.volume = 30;
            explosion.URL = "songs\\boom.mp3";
            explosion.settings.volume = 0;

            stars = new PictureBox[3]; // Increase the number of stars if desired
            rnd = new Random();

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Size = new Size(3, 3);
                stars[i].BackColor = Color.Wheat;

                // Set the initial position to the right border of the screen
                stars[i].Location = new Point(this.Width + rnd.Next(50, 200), rnd.Next(30, this.Height));

                this.Controls.Add(stars[i]);
            }

            //Enemy ammo
            enemiesMunition = new PictureBox[1];
            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                enemiesMunition[i] = new PictureBox();
                enemiesMunition[i].Size = new Size(30, 30);
                enemiesMunition[i].Visible = false;
                enemiesMunition[i].Image = enemyAmmunition;
                enemiesMunition[i].SizeMode = PictureBoxSizeMode.Zoom;
                int x = rnd.Next(0, enemies.Length);
                enemiesMunition[i].Location = new Point(enemies[x].Location.X + 20, enemies[x].Location.Y + enemies[x].Height / 2 - enemiesMunition[i].Height / 2);
                this.Controls.Add(enemiesMunition[i]);
            }

            gameMedia.controls.play();
        }

        private void MoveBgTimer_Tick_1(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Left -= backgroundspeed;

                if (stars[i].Left + stars[i].Width <= 0)
                {
                    // Move the star to the rightmost part of the screen
                    stars[i].Left = this.Width;
                    // Reset the star's Y position randomly
                    stars[i].Top = rnd.Next(0, this.Height);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Handle click on the player (if needed)
        }

        private void MoveDownTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 285)
            {
                Player.Top += playerSpeed;
            }
        }

        private void MoveUpTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerSpeed;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            MoveUpTimer.Stop();
            MoveDownTimer.Stop();

            if (e.KeyCode == Keys.Space)
            {
                if (!gameIsOver)
                {
                    if (pause)
                    {
                        StartTimers();
                        label1.Visible = false;
                        gameMedia.controls.play();
                        pause = false;
                    }
                    else
                    {
                        label1.Location = new Point(220, 150);
                        label1.Text = "PAUSED";
                        label1.Font = new Font("Arial", 50, FontStyle.Bold);
                        label1.ForeColor = Color.White;
                        label1.AutoSize = false;
                        label1.Width = 700;
                        label1.Height = 200;
                        label1.Visible = true;
                        gameMedia.controls.pause();
                        StopTimers();
                        pause = true;
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {
                if (e.KeyCode == Keys.Up)
                {
                    MoveDownTimer.Stop(); // Stop the down movement when up is pressed
                    MoveUpTimer.Start();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    MoveUpTimer.Stop(); // Stop the up movement when down is pressed
                    MoveDownTimer.Start();
                }
            }
        }

        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            shootgMedia.controls.play();
            for (int i = 0; i < ammo.Length; i++)
            {
                if (ammo[i].Left < this.Width)
                {
                    ammo[i].Visible = true;
                    ammo[i].Left += ammoSpeed;

                    Collision();
                }
                else
                {
                    ammo[i].Visible = false;
                    ammo[i].Location = new Point(Player.Location.X + 20, Player.Location.Y + Player.Height / 2 - ammo[i].Height / 2);

                }
            }
        }

        private void MoveEnemies(PictureBox[] array, int speed)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Visible = true;
                array[i].Left -= speed;  // Move from right to left

                if (array[i].Right < 0)  // Check if the enemy is off the left side of the screen
                {
                    // Reset the enemy to the right side of the screen
                    array[i].Left = this.Width;
                    array[i].Top = rnd.Next(0, this.Height - array[i].Height);  // Reset the enemy's Y position randomly
                }
            }
        }

        private void EnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies(enemies, enemySpeed);
        }
        private void Collision()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < ammo.Length; j++)
                {
                    if (ammo[j].Bounds.IntersectsWith(enemies[i].Bounds))
                    {
                        explosion.settings.volume = 100;
                        explosion.controls.play();
                        enemies[i].Location = new Point((i + 1) * 50, -100);
                        ammo[j].Visible = false;

                        // Increase the score when an enemy is hit
                        score++;
                        UpdateScoreLabel();
                    }
                }

                if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    explosion.settings.volume = 100;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
            }
        }


        private void GameOver(String str)
        {
            label1.Text = str;
            label1.Font = new Font("Arial", 50, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.AutoSize = false;
            label1.Width = 700;
            label1.Height = 200;
            label1.Location = new Point(180, 50);
            label1.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            finalScoreLabel.Text = "Final Score: " + score.ToString();
            finalScoreLabel.Visible = true;

            gameMedia.controls.stop();
            StopTimers();
        }

        //Stop timers
        private void StopTimers()
        {
            MoveBgTimer.Stop();
            EnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
            EnemiesMunitionTimer.Stop();
        }

        //Start timers
        private void StartTimers()
        {
            MoveBgTimer.Start();
            EnemiesTimer.Start();
            MoveMunitionTimer.Start();
            EnemiesMunitionTimer.Start();
        }

        private void EnemiesMunitionTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                if (enemiesMunition[i].Left > 0)
                {
                    enemiesMunition[i].Visible = true;
                    enemiesMunition[i].Left -= enemiesMunitionSpeed;
                }
                else
                {
                    enemiesMunition[i].Visible = false;
                    int x = rnd.Next(0, enemies.Length);
                    enemiesMunition[i].Location = new Point(enemies[x].Location.X + 20, enemies[x].Location.Y + enemies[x].Height / 2 - enemiesMunition[i].Height / 2);
                }
            }
            CollisionWithEnemiesMunition();
        }

        private void CollisionWithEnemiesMunition()
        {
            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                if (enemiesMunition[i].Bounds.IntersectsWith(Player.Bounds))
                {
                    enemiesMunition[i].Visible = false;
                    explosion.settings.volume = 30;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Reset game-related variables
            score = 0;
            level = 1;
            difficulty = 9;
            backgroundspeed = 1;
            playerSpeed = 3;
            enemySpeed = 6;
            ammoSpeed = 10;
            enemiesMunitionSpeed = 20;

            // Clear the form controls and reinitialize
            this.Controls.Clear();
            InitializeComponent();
            Form1_Load(e, e);

            // Add scoreLabel to the form again
            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Arial", 12, FontStyle.Regular);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.AutoSize = true;
            scoreLabel.Location = new Point(10, 10);
            this.Controls.Add(scoreLabel);

            // Recreate finalScoreLabel and set its visibility to false
            finalScoreLabel = new Label();
            finalScoreLabel.Text = "Final Score:";
            finalScoreLabel.Font = new Font("Arial", 20, FontStyle.Bold);
            finalScoreLabel.ForeColor = Color.White;
            finalScoreLabel.AutoSize = true;
            finalScoreLabel.Visible = false;
            finalScoreLabel.Location = new Point(270, 315);
            this.Controls.Add(finalScoreLabel);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void UpdateScoreLabel()
        {
            // Update the score label text
            scoreLabel.Text = "Score: " + score.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}