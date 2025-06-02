using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPG_Game
{
    public partial class Form1 : Form
    {
        // Animation state
        private int player1Pose = 0;
        private int player2Pose = 0;
        private int player1Offset = 0;
        private int player2Offset = 0;
        private int player1Dir = 1;
        private int player2Dir = -1;
        private Random moveRng = new Random();
        private Random blockRng = new Random();
        private string[] trashtalkPhrases = new string[]
        {
            "Bumawas ka naman",
            "Parang kagat lang ng lamok",
            "Anong Gentle-gentle!",
            "Mahina nilalang!",
            "No balls!",
            "Yun na 'yon?!",
            "Ang dumi-dumi mo!",
            "Bading!",
            "Mama mo blue",
            "Di ka mahal ng mama mo",
            "Katapusan mo na",
            "Hindi mo ako kaya",
            "Mas magaling ako saiyo",
            "Sipsip ka",
            "You tell me, bakit nga ba?",
            "Sigeeeeeeeeeee! ./.",
            "Paktay ka sakin",
            "Inggetera ka"
        };
        // Dialogue state for drawing above heads
        private string player1Dialogue = "";
        private string player2Dialogue = "";
        private DateTime player1DialogueEndTime;
        private DateTime player2DialogueEndTime;
        private readonly TimeSpan dialogueDuration = TimeSpan.FromSeconds(2);
        // Store last health for drawing health bars
        private int lastPlayer1Health = 100, lastPlayer1MaxHealth = 100;
        private int lastPlayer2Health = 100, lastPlayer2MaxHealth = 100;
        // Blood effect state
        private bool player1HitEffect = false;
        private DateTime player1HitEffectEndTime;
        private bool player2HitEffect = false;
        private DateTime player2HitEffectEndTime;
        private readonly TimeSpan hitEffectDuration = TimeSpan.FromMilliseconds(500);
        private Random effectRng = new Random();
        // Dead state
        private bool player1Dead = false;
        private bool player2Dead = false;
        // Blocking state
        private bool player1Blocking = false;
        private bool player2Blocking = false;
        private readonly TimeSpan blockDuration = TimeSpan.FromSeconds(2);
        private DateTime player1BlockEndTime;
        private DateTime player2BlockEndTime;
        // Skill UI state
        private bool player1SkillReady = true;
        private bool player2SkillReady = true;
        private string player1SkillText = "Skill Ready";
        private string player2SkillText = "Skill Ready";
        // Store last mana for drawing mana bars
        private int lastPlayer1Mana = 100, lastPlayer1MaxMana = 100;
        private int lastPlayer2Mana = 100, lastPlayer2MaxMana = 100;
        // Critical hit flash effect
        private bool player1CriticalFlash = false;
        private bool player2CriticalFlash = false;
        private DateTime player1CriticalEndTime;
        private DateTime player2CriticalEndTime;
        private readonly TimeSpan criticalFlashDuration = TimeSpan.FromMilliseconds(300);
        private int criticalFlashAlpha = 255;
        private int criticalFlashSize = 0;
        // Sword wave effect state
        private bool player1SwordWaveActive = false;
        private bool player2SwordWaveActive = false;
        private DateTime player1SwordWaveEndTime;
        private DateTime player2SwordWaveEndTime;
        private readonly TimeSpan swordWaveDuration = TimeSpan.FromMilliseconds(2500);
        private float player1SwordWaveProgress = 0;
        private float player2SwordWaveProgress = 0;
        // Store positions at the start of skill animation
        private Point player1SkillStartPos;
        private Point player2SkillStartPos;
        private Point player1SkillTargetPos;
        private Point player2SkillTargetPos;
        // Timer for animation updates
        private System.Windows.Forms.Timer animationTimer;
        // Game State Variables
        private int gameTime = 0;

        // Add an exit button
        private Button btnExit;

        // --- Multiplayer Input State ---
        private bool p1MoveLeft = false;
        private bool p1MoveRight = false;
        private bool p1Attack = false;
        private bool p1Block = false;

        private bool p2MoveLeft = false;
        private bool p2MoveRight = false;
        private bool p2Attack = false;
        private bool p2Block = false;
        // Flags for triggering actions once per key press
        private bool p1SkillRequested = false;
        private bool p2SkillRequested = false;
        // --- Potion State ---
        private bool player1PotionUsed = false;
        private bool player2PotionUsed = false;
        // --- Attack Animation State ---
        private bool player1AttackingAnimation = false;
        private int player1AttackAnimationStep = 0;
        private const int player1AttackAnimationLength = 5; // Number of steps in the animation
        private bool player1HitRegistered = false; // Flag to ensure hit is registered only once per animation

        private bool player2AttackingAnimation = false;
        private int player2AttackAnimationStep = 0;
        private const int player2AttackAnimationLength = 5; // Number of steps in the animation
        private bool player2HitRegistered = false; // Flag to ensure hit is registered only once per animation
        // -------------------------------

        // Need to declare player1 and player2 as member variables of Form1
        private ClassFighter player1;
        private ClassFighter player2;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            
            // Enable double buffering for the form
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);
            
            // Enable double buffering for the panel
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic,
                null, pnlArena, new object[] { true });

            // Set panel styles for smoother rendering
            pnlArena.GetType().GetMethod("SetStyle", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic)
                .Invoke(pnlArena, new object[] { 
                    ControlStyles.UserPaint | 
                    ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.OptimizedDoubleBuffer, true });
            
            // Populate character selection ComboBoxes
            cmbPlayer1Class.Items.AddRange(new string[] { "Bida-bida", "Pabibo" });
            cmbPlayer2Class.Items.AddRange(new string[] { "Bida-bida", "Pabibo" });
            cmbPlayer1Class.SelectedIndex = 0;
            cmbPlayer2Class.SelectedIndex = 1;
            btnStartBattle.Click += BtnStartBattle_Click;
            pnlArena.Paint += PnlArena_Paint;
            // Initialize and start the animation timer
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 16; // ~60 FPS (1000ms / 60 = 16.67ms)
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // Initialize and position the Exit button
            btnExit = new Button()
            {
                Text = "X",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(30, 30),
                Location = new Point(this.ClientSize.Width - 35, 5), // Position in top-right corner
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += BtnExit_Click;

            // Add the Exit button to the form controls
            this.Controls.Add(btnExit);

            // Bring the exit button to front to ensure it's clickable
            btnExit.BringToFront();

            // Handle form resize to reposition the button and center other controls
            this.Resize += Form1_Resize;

            // Enable KeyPreview to capture keyboard events at the form level
            this.KeyPreview = true;
        }

        // --- Input Handling Overrides ---
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Check if a text box is currently focused
            bool isTextBoxFocused = this.ActiveControl is TextBox;

            // Player 1 Controls (AWSD) - Only process if a text box is NOT focused
            if (!isTextBoxFocused)
            {
                if (e.KeyCode == Keys.A) p1MoveLeft = true;
                if (e.KeyCode == Keys.D) p1MoveRight = true;
                // W triggers skill/attack once
                if (e.KeyCode == Keys.W) p1SkillRequested = true;
                // S triggers block for a duration if not already blocking
                if (e.KeyCode == Keys.S) 
                {
                    if (!player1Blocking)
                    {
                        player1Blocking = true;
                        player1BlockEndTime = DateTime.Now + blockDuration;
                    }
                }
            }

            // Player 2 Controls (Arrow Keys) - Can always process these, less likely to interfere with typing
            // Although, adding the same check for consistency is safer
            if (!isTextBoxFocused)
            {
                if (e.KeyCode == Keys.Left) p2MoveLeft = true;
                if (e.KeyCode == Keys.Right) p2MoveRight = true;
                // Up arrow triggers skill/attack once
                if (e.KeyCode == Keys.Up) p2SkillRequested = true;
                // Down arrow triggers block for a duration if not already blocking
                if (e.KeyCode == Keys.Down)
                {
                    if (!player2Blocking)
                    {
                        player2Blocking = true;
                        player2BlockEndTime = DateTime.Now + blockDuration;
                    }
                }
            }

            // Prevent default handling for our control keys ONLY if a text box is NOT focused
            // and the key is one of our control keys.
            if (!isTextBoxFocused && 
                (e.KeyCode == Keys.A || e.KeyCode == Keys.D || e.KeyCode == Keys.W || e.KeyCode == Keys.S ||
                 e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            // Player 1 Controls (AWSD)
            if (e.KeyCode == Keys.A) p1MoveLeft = false;
            if (e.KeyCode == Keys.D) p1MoveRight = false;
            if (e.KeyCode == Keys.S) p1Block = false;

            // Player 2 Controls (Arrow Keys)
            if (e.KeyCode == Keys.Left) p2MoveLeft = false;
            if (e.KeyCode == Keys.Right) p2MoveRight = false;
            if (e.KeyCode == Keys.Down) p2Block = false;
        }
        // -------------------------------

        // Event handler for the Exit button click
        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the form
        }

        // Event handler for form resize to reposition the Exit button and center other controls
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Ensure client size is valid before positioning
            if (this.ClientSize.Width > 0 && this.ClientSize.Height > 0)
            {
                // Reposition the Exit button in the top-right corner
                btnExit.Location = new Point(this.ClientSize.Width - 35, 5);

                // Calculate horizontal center of the form
                int formCenterX = this.ClientSize.Width / 2;

                // Define vertical starting position for the top control row
                int topRowY = 10;
                int verticalSpacing = 5; // Spacing between controls in a group
                int horizontalSpacing = 200; // Spacing between Player 1 and Player 2 groups

                // Define a fixed width for input boxes and dropdowns
                int controlWidth = 150;

                // Set the size of the input boxes and dropdowns
                txtPlayer1Name.Width = controlWidth;
                txtPlayer2Name.Width = controlWidth;
                cmbPlayer1Class.Width = controlWidth;
                cmbPlayer2Class.Width = controlWidth;

                // Determine the width of each player's control group (now based on fixed width)
                int playerGroupWidth = controlWidth; // Each control has the same width

                // Calculate horizontal position for Player 1 group (left of center)
                // We want the gap between groups to be centered around the form's horizontal center
                int player1GroupX = formCenterX - horizontalSpacing / 2 - playerGroupWidth;

                // Calculate horizontal position for Player 2 group (right of center)
                int player2GroupX = formCenterX + horizontalSpacing / 2;

                // Position Player 1 controls vertically stacked
                txtPlayer1Name.Location = new Point(player1GroupX, topRowY);
                cmbPlayer1Class.Location = new Point(player1GroupX, txtPlayer1Name.Bottom + verticalSpacing);
                lblPlayer1Health.Location = new Point(player1GroupX, cmbPlayer1Class.Bottom + verticalSpacing);

                // Position Player 2 controls vertically stacked
                txtPlayer2Name.Location = new Point(player2GroupX, topRowY);
                cmbPlayer2Class.Location = new Point(player2GroupX, txtPlayer2Name.Bottom + verticalSpacing);
                lblPlayer2Health.Location = new Point(player2GroupX, cmbPlayer2Class.Bottom + verticalSpacing);

                // Position Start Battle button (centered horizontally, below player info)
                // Find the bottom-most control in the player info section to position the button below it
                int playerInfoBottom = Math.Max(lblPlayer1Health.Bottom, lblPlayer2Health.Bottom);
                btnStartBattle.Location = new Point(formCenterX - btnStartBattle.Width / 2, playerInfoBottom + 15);

                // Position the arena panel (centered horizontally, below button)
                pnlArena.Location = new Point(formCenterX - pnlArena.Width / 2, btnStartBattle.Bottom + 20);

                // Position the battle log (centered horizontally, below arena)
                lstBattleLog.Location = new Point(formCenterX - lstBattleLog.Width / 2, pnlArena.Bottom + 20);

                // Position winner label (centered horizontally, below battle log)
                lblWinner.Location = new Point(formCenterX - lblWinner.Width / 2, lstBattleLog.Bottom + 10);
            }
        }

        // Timer tick event to invalidate the panel for animation and handle game updates
        private async void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Prevent re-entrancy if a previous tick is still processing async operations
            if (gameTime == -1) return; // Use gameTime as a simple lock
            gameTime = -1;

            // Ensure player objects are initialized before processing input and game logic
            if (player1 == null || player2 == null)
            {
                 pnlArena.ResumeLayout(true);
                 gameTime = 0;
                 return; // Do nothing if players are not initialized
            }

            pnlArena.SuspendLayout();

            try
            {
                // Update animation states (keep existing checks for effects, blocking, etc.)
                if (player1SwordWaveActive && DateTime.Now >= player1SwordWaveEndTime)
                {
                    player1SwordWaveActive = false;
                }
                if (player2SwordWaveActive && DateTime.Now >= player2SwordWaveEndTime)
                {
                    player2SwordWaveActive = false;
                }

                // Blocking state is now managed by a duration timer
                if (player1Blocking && DateTime.Now >= player1BlockEndTime)
                {
                    player1Blocking = false;
                    // You might want to add a visual cue here that block is off, e.g., brief text above head
                }
                if (player2Blocking && DateTime.Now >= player2BlockEndTime)
                {
                    player2Blocking = false;
                    // You might want to add a visual cue here that block is off
                }

                // --- Real-time Game Logic Update ---
                // Handle Player 1 Movement (AWSD)
                if (p1MoveLeft && player1Offset > -150 && !player1AttackingAnimation) // Prevent movement during attack animation
                {
                    player1Offset -= 5; // Adjust speed as needed
                }
                if (p1MoveRight && player1Offset < 150 && !player1AttackingAnimation) // Prevent movement during attack animation
                {
                    player1Offset += 5; // Adjust speed as needed;
                }

                // Handle Player 2 Movement (Arrow Keys)
                if (p2MoveLeft && player2Offset > -150 && !player2AttackingAnimation) // Prevent movement during attack animation
                {
                    player2Offset -= 5; // Adjust speed as needed;
                }
                if (p2MoveRight && player2Offset < 150 && !player2AttackingAnimation) // Prevent movement during attack animation
                {
                    player2Offset += 5; // Adjust speed as needed;
                }

                // Update skill status (cooldown)
                player1.UpdateSkillStatus();
                player2.UpdateSkillStatus();
                UpdateSkillUI(player1, player2); // Update UI based on status

                // Handle Player 1 Actions
                // Blocking: activated for a duration when S is pressed
                // Removed: player1Blocking = p1Block; // Set blocking state directly from input

                // Attack/Skill Trigger: Check for requested action if not already attacking
                if (p1SkillRequested && !player1AttackingAnimation) // Check for skill/attack intent and if not already animating
                {
                    // Prioritize skill if ready and enough mana, and not blocking
                    if (!player1Blocking && player1.IsSkillReady && player1.Mana >= 90)
                    {
                        // Store start and target positions for the skill animation (needed for sword wave draw)
                        player1SkillStartPos = new Point(130 + player1Offset, 160); // Player 1 current position
                        player1SkillTargetPos = new Point(570 + player2Offset, 160); // Player 2 current position

                        // Start skill animation
                        player1AttackingAnimation = true;
                        player1AttackAnimationStep = 0;
                        player1HitRegistered = false;
                    }
                    else if (!player1Blocking) // If skill not used, perform normal attack if not blocking
                    {
                        // Start normal attack animation
                        player1AttackingAnimation = true;
                        player1AttackAnimationStep = 0;
                        player1HitRegistered = false;
                    }
                     p1SkillRequested = false; // Reset the flag after processing
                }

                // Handle Player 2 Actions
                // Blocking: activated for a duration when Down arrow is pressed
                // Removed: player2Blocking = p2Block; // Set blocking state directly from input

                // Attack/Skill Trigger: Check for requested action if not already attacking
                if (p2SkillRequested && !player2AttackingAnimation) // Check for skill/attack intent and if not already animating
                {
                    // Prioritize skill if ready and enough mana, and not blocking
                    if (!player2Blocking && player2.IsSkillReady && player2.Mana >= 90)
                    {
                        // Store start and target positions for the skill animation (needed for sword wave draw)
                        player2SkillStartPos = new Point(570 + player2Offset, 160); // Player 2 current position
                        player2SkillTargetPos = new Point(130 + player1Offset, 160); // Player 1 current position

                        // Start skill animation
                        player2AttackingAnimation = true;
                        player2AttackAnimationStep = 0;
                        player2HitRegistered = false;
                    }
                    else if (!player2Blocking) // If skill not used, perform normal attack if not blocking
                    {
                        // Start normal attack animation
                        player2AttackingAnimation = true;
                        player2AttackAnimationStep = 0;
                        player2HitRegistered = false;
                    }
                     p2SkillRequested = false; // Reset the flag after processing
                }

                // --- Update Attack Animations and Handle Hits ---
                // Player 1 Attack Animation Update
                if (player1AttackingAnimation)
                {
                    player1AttackAnimationStep++;
                    // Update player pose based on animation step (define new poses if needed)
                    if (player1AttackAnimationStep <= player1AttackAnimationLength)
                    {
                        // Example: Change pose mid-swing
                        if (player1AttackAnimationStep == 2) player1Pose = 1; // Mid-swing pose
                        else if (player1AttackAnimationStep == 4) player1Pose = 2; // End-swing pose
                        else player1Pose = 0; // Default pose

                        // --- Perform Hit Check and Apply Damage at a specific animation step ---
                        if (player1AttackAnimationStep == 3 && !player1HitRegistered) // Example: Hit occurs at step 3
                        {
                            bool isSkill = player1.IsSkillReady && player1.Mana >= 90; // Determine if it's a skill hit
                            int damage = isSkill ? player1.UseSkill() : player1.Attack(); // Get damage (or skill effect value)
                            HandleDamage(player1, player2, damage, true, isSkill); // Handle damage/effects with range check

                            // Activate sword wave effect for skills (still triggered here for timing with hit)
                            if (isSkill)
                            {
                                player1SwordWaveActive = true;
                                player1SwordWaveEndTime = DateTime.Now + swordWaveDuration;
                            }
                            player1HitRegistered = true; // Mark hit as registered
                        }
                    }

                    // End animation
                    if (player1AttackAnimationStep > player1AttackAnimationLength)
                    {
                        player1AttackingAnimation = false;
                        player1Pose = 0; // Reset to default pose
                        // Ensure skill/attack flags are reset for the next input
                        p1SkillRequested = false;
                        // Any other animation cleanup
                    }
                }

                // Player 2 Attack Animation Update
                if (player2AttackingAnimation)
                {
                    player2AttackAnimationStep++;
                     // Update player pose based on animation step
                     if (player2AttackAnimationStep <= player2AttackAnimationLength)
                    {
                        // Example: Change pose mid-swing
                        if (player2AttackAnimationStep == 2) player2Pose = 1; // Mid-swing pose
                        else if (player2AttackAnimationStep == 4) player2Pose = 2; // End-swing pose
                        else player2Pose = 0; // Default pose

                        // --- Perform Hit Check and Apply Damage at a specific animation step ---
                        if (player2AttackAnimationStep == 3 && !player2HitRegistered) // Example: Hit occurs at step 3
                        {
                            bool isSkill = player2.IsSkillReady && player2.Mana >= 90; // Determine if it's a skill hit
                            int damage = isSkill ? player2.UseSkill() : player2.Attack(); // Get damage (or skill effect value)
                            HandleDamage(player2, player1, damage, false, isSkill); // Handle damage/effects with range check

                            // Activate sword wave effect for skills
                            if (isSkill)
                            {
                                player2SwordWaveActive = true;
                                player2SwordWaveEndTime = DateTime.Now + swordWaveDuration;
                            }
                            player2HitRegistered = true; // Mark hit as registered
                        }
                    }

                    // End animation
                    if (player2AttackAnimationStep > player2AttackAnimationLength)
                    {
                        player2AttackingAnimation = false;
                        player2Pose = 0; // Reset to default pose
                         // Ensure skill/attack flags are reset for the next input
                         p2SkillRequested = false;
                        // Any other animation cleanup
                    }
                }

                // Update Health and Mana UI
                UpdateHealthLabels(player1, player2);

                // Check for win condition
                if (player1.Health <= 0 || player2.Health <= 0)
                {
                     animationTimer.Stop(); // Stop the game loop
                    // Determine winner and show winner dialogue
                     string winnerName = player1.Health > 0 ? player1.Name : player2.Name;
                     lblWinner.Text = $"Winner: {winnerName}!";
                     lstBattleLog.Items.Add($"{winnerName} wins the battle!");

                     // Display winner dialogue above head
                     if (player1.Health > 0)
                     {
                         player1Dialogue = "Ez, walang kaba!";
                         player1DialogueEndTime = DateTime.Now + dialogueDuration + TimeSpan.FromSeconds(1); // Show longer
                         player2Dialogue = ""; // Clear loser's dialogue
                     }
                     else
                     {
                         player2Dialogue = "No sweat, so easy!";
                         player2DialogueEndTime = DateTime.Now + dialogueDuration + TimeSpan.FromSeconds(1); // Show longer
                         player1Dialogue = ""; // Clear loser's dialogue
                     }
                     pnlArena.Invalidate(); // Redraw to show winner dialogue

                     // Set dead flag and loser dialogue for the loser
                     if (player1.Health <= 0)
                     {
                         player1Dead = true;
                         player1Dialogue = "Arayyyy koooh!"; // Loser dialogue
                         player1DialogueEndTime = DateTime.Now + dialogueDuration; // Show for normal duration
                     }
                     if (player2.Health <= 0)
                     {
                         player2Dead = true;
                         player2Dialogue = "Babawi ako, kupal!"; // Loser dialogue
                         player2DialogueEndTime = DateTime.Now + dialogueDuration; // Show for normal duration
                     }
                     player1Pose = 0; player2Pose = 0;
                     // Keep the final offsets so dead bodies stay in place
                     // player1Offset = 0; player2Offset = 0;
                     pnlArena.Invalidate(); // Redraw with dead stickman
                }
            }
            finally
            {
                pnlArena.ResumeLayout(true);
                gameTime = 0; // Release the lock
            }
        }

        // Helper method to handle damage application and effects
        private void HandleDamage(ClassFighter attacker, ClassFighter target, int damage, bool attackerIsPlayer1, bool isSkill)
        {
            // Calculate the current horizontal distance between the two players
            // Base positions are roughly 130 for Player 1 and 570 for Player 2
            int attackerBaseX = attackerIsPlayer1 ? 130 : 570;
            int targetBaseX = attackerIsPlayer1 ? 570 : 130;
            int attackerCurrentX = attackerBaseX + (attackerIsPlayer1 ? player1Offset : player2Offset);
            int targetCurrentX = targetBaseX + (attackerIsPlayer1 ? player2Offset : player1Offset);

            int horizontalDistance = Math.Abs(attackerCurrentX - targetCurrentX);
            // Define the maximum distance for a hit to register (adjust as needed)
            int hitRangeThreshold = 200; // Approx. distance during attack animation

            bool hit = horizontalDistance <= hitRangeThreshold;
            bool blocked = false;

            if (hit)
            {
                // Check for block only if it's a normal attack (skills bypass block for now)
                if (!isSkill && ((attackerIsPlayer1 && player2Blocking) || (!attackerIsPlayer1 && player1Blocking)))
                {
                     // 25% chance to block a normal attack
                    if (blockRng.NextDouble() < 0.25)
                    {                       
                        lstBattleLog.Items.Add($"{target.Name} blocks the attack from {attacker.Name}!");
                        if (attackerIsPlayer1) player2Dialogue = "Laos!"; else player1Dialogue = "Sukot!";
                        if (attackerIsPlayer1) player2DialogueEndTime = DateTime.Now + dialogueDuration; else player1DialogueEndTime = DateTime.Now + dialogueDuration;
                        // Visual feedback for blocking is handled by holding the block key
                        blocked = true;
                    }
                }

                if (!blocked)
                {
                     int actualDamage = damage; // Start with base damage

                     // Implement critical hit chance (e.g., 10% chance for 1.5x damage for normal attacks)
                    if (!isSkill && effectRng.NextDouble() < 0.10) // 10% critical hit chance for normal attacks
                    {
                        actualDamage = (int)(actualDamage * 1.5);
                        lstBattleLog.Items.Add($"CRITICAL HIT! {attacker.Name} deals {actualDamage} damage to {target.Name}!");
                        if (attackerIsPlayer1) player2Dialogue = "Mahapdi ba?"; else player1Dialogue = "Sakit 'no!?";
                        if (attackerIsPlayer1) player2DialogueEndTime = DateTime.Now + dialogueDuration; else player1DialogueEndTime = DateTime.Now + dialogueDuration;

                        // Activate critical hit flash effect (this effect is currently unused in Paint method, needs implementation)
                        // if (attackerIsPlayer1) player2CriticalFlash = true; else player1CriticalFlash = true;
                        // if (attackerIsPlayer1) player2CriticalEndTime = DateTime.Now + criticalFlashDuration; else player1CriticalEndTime = DateTime.Now + criticalFlashDuration;
                    }
                     else if (isSkill)
                     {
                          lstBattleLog.Items.Add($"{attacker.Name} uses skill, dealing {actualDamage} damage to {target.Name}!");
                          if (attackerIsPlayer1) player2Dialogue = "Kainin mo tae ko!"; else player1Dialogue = "Bahog bilat";
                          if (attackerIsPlayer1) player2DialogueEndTime = DateTime.Now + dialogueDuration; else player1DialogueEndTime = DateTime.Now + dialogueDuration;
                     }
                    else // Normal hit, not critical
                    {
                         lstBattleLog.Items.Add($"{attacker.Name} attacks {target.Name} for {actualDamage} damage!");
                         if (attackerIsPlayer1) player2Dialogue = GetRandomTrashtalk(); else player1Dialogue = GetRandomTrashtalk();
                         if (attackerIsPlayer1) player2DialogueEndTime = DateTime.Now + dialogueDuration; else player1DialogueEndTime = DateTime.Now + dialogueDuration;
                    }

                    target.TakeDamage(actualDamage); // Apply the calculated damage

                    // Activate hit effect
                    if (attackerIsPlayer1) player2HitEffect = true; else player1HitEffect = true;
                    if (attackerIsPlayer1) player2HitEffectEndTime = DateTime.Now + hitEffectDuration; else player1HitEffectEndTime = DateTime.Now + hitEffectDuration;
                }
            }
            else // Missed because out of range
            {
                lstBattleLog.Items.Add($"{attacker.Name} attacks but misses!");
                if (attackerIsPlayer1) player2Dialogue = "Haha duling!"; else player1Dialogue = "Itama mo naman!";
                if (attackerIsPlayer1) player2DialogueEndTime = DateTime.Now + dialogueDuration; else player1DialogueEndTime = DateTime.Now + dialogueDuration;
            }

            // Check if target's HP is low and use potion if available
            if (target.Health <= 30)
            {
                if (attackerIsPlayer1) // Player 2 is the target
                {
                    UsePotion(target, false); // Call UsePotion for Player 2
                }
                else // Player 1 is the target
                {
                    UsePotion(target, true); // Call UsePotion for Player 1
                }
            }
        }

        // Draw stickmen and swords
        private void PnlArena_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Player 1 (left, full body, with offset, farther apart)
            int p1x = 130 + player1Offset, p1y = 160;
            int p2x = 570 + player2Offset, p2y = 160;
            // Pass player objects to DrawCoolStickman
            if (player1 != null) DrawCoolStickman(g, p1x, p1y, player1Pose, player1, player1Dead, player1Blocking);
            if (player2 != null) DrawCoolStickman(g, p2x, p2y, player2Pose, player2, player2Dead, player2Blocking);
            // Draw health bars above heads
            if (!player1Dead) DrawHealthBar(g, p1x, p1y - 100, lastPlayer1Health, lastPlayer1MaxHealth, true);
            if (!player2Dead) DrawHealthBar(g, p2x, p2y - 100, lastPlayer2Health, lastPlayer2MaxHealth, false);
            // Draw mana bars below health bars
            if (!player1Dead) DrawManaBar(g, p1x, p1y - 85, lastPlayer1Mana, lastPlayer1MaxMana, true);
            if (!player2Dead) DrawManaBar(g, p2x, p2y - 85, lastPlayer2Mana, lastPlayer2MaxMana, false);
            // Draw dialogue above heads if active
            if (DateTime.Now < player1DialogueEndTime)
            {
                DrawDialogue(g, p1x, p1y - 140, player1Dialogue);
            }
            if (DateTime.Now < player2DialogueEndTime)
            {
                DrawDialogue(g, p2x, p2y - 140, player2Dialogue);
            }
            // Draw blood effect if active
            if (DateTime.Now < player1HitEffectEndTime)
            {
                DrawBloodEffect(g, p1x, p1y + 20); // Draw around body/legs
            }
            if (DateTime.Now < player2HitEffectEndTime)
            {
                DrawBloodEffect(g, p2x, p2y + 20); // Draw around body/legs
            }
            // Draw skill status text
            if (!player1Dead) DrawSkillStatus(g, p1x, p1y + 140, player1SkillText);
            if (!player2Dead) DrawSkillStatus(g, p2x, p2y + 140, player2SkillText);
            // Reset blocking state after duration
            if (player1Blocking && DateTime.Now >= player1BlockEndTime)
            {
                player1Blocking = false;
                pnlArena.Invalidate();
            }
            if (player2Blocking && DateTime.Now >= player2BlockEndTime)
            {
                player2Blocking = false;
                pnlArena.Invalidate();
            }
            // Draw critical hit flash if active
            if (DateTime.Now < player1CriticalEndTime)
            {
                DrawCriticalFlash(g, p1x, p1y);
            }
            if (DateTime.Now < player2CriticalEndTime)
            {
                DrawCriticalFlash(g, p2x, p2y);
            }
            // Draw sword wave if active
            if (player1SwordWaveActive && DateTime.Now < player1SwordWaveEndTime)
            {
                // Calculate elapsed time from the animation start
                TimeSpan elapsed = DateTime.Now - (player1SwordWaveEndTime - swordWaveDuration);
                player1SwordWaveProgress = (float)elapsed.TotalMilliseconds / (float)swordWaveDuration.TotalMilliseconds;
                // Ensure progress is between 0 and 1
                player1SwordWaveProgress = Math.Max(0, Math.Min(1, player1SwordWaveProgress));

                // Draw using stored start and target positions
                DrawSwordWave(g, player1SkillStartPos.X, player1SkillStartPos.Y, player1SkillTargetPos.X, player1SkillTargetPos.Y, true, player1SwordWaveProgress);
            } else if (player1SwordWaveActive && DateTime.Now >= player1SwordWaveEndTime)
            {
                player1SwordWaveActive = false;
            }

            if (player2SwordWaveActive && DateTime.Now < player2SwordWaveEndTime)
            {
                // Calculate elapsed time from the animation start
                TimeSpan elapsed = DateTime.Now - (player2SwordWaveEndTime - swordWaveDuration);
                player2SwordWaveProgress = (float)elapsed.TotalMilliseconds / (float)swordWaveDuration.TotalMilliseconds;
                 // Ensure progress is between 0 and 1
                player2SwordWaveProgress = Math.Max(0, Math.Min(1, player2SwordWaveProgress));

                // Draw using stored start and target positions
                DrawSwordWave(g, player2SkillStartPos.X, player2SkillStartPos.Y, player2SkillTargetPos.X, player2SkillTargetPos.Y, false, player2SwordWaveProgress);
            } else if (player2SwordWaveActive && DateTime.Now >= player2SwordWaveEndTime)
            {
                player2SwordWaveActive = false;
            }
        }

        // Draw a cooler stickman with headband, sword glow, handle, dead pose, and blocking pose
        private void DrawCoolStickman(Graphics g, int x, int y, int pose, ClassFighter player, bool dead, bool blocking)
        {
            // Use the character's color for the body and limbs
            Color characterColor = player.CharacterColor;

            if (dead)
            {
                // Draw stickman lying down
                Pen bodyPenDead = new Pen(Color.Gray, 5);
                Pen limbPenDead = new Pen(Color.DarkGray, 7);
                // Body (lying down)
                g.DrawLine(bodyPenDead, x - 40, y + 60, x + 40, y + 60);
                // Head (on the ground)
                g.FillEllipse(Brushes.DarkGray, x - 50, y + 45, 30, 30);
                g.DrawEllipse(bodyPenDead, x - 50, y + 45, 30, 30);
                // Limbs (collapsed)
                g.DrawLine(limbPenDead, x - 10, y + 60, x - 30, y + 80);
                g.DrawLine(limbPenDead, x + 10, y + 60, x + 30, y + 80);
                // Legs (collapsed, with feet)
                Point legL1Dead = new Point(x - 40, y + 60), legL2Dead = new Point(x - 60, y + 90);
                Point legR1Dead = new Point(x + 40, y + 60), legR2Dead = new Point(x + 60, y + 90);
                g.DrawLine(limbPenDead, legL1Dead, legL2Dead);
                g.DrawLine(limbPenDead, legR1Dead, legR2Dead);
                // Feet (collapsed)
                g.FillRectangle(Brushes.DarkGray, legL2Dead.X - 10, legL2Dead.Y - 5, 15, 10);
                g.FillRectangle(Brushes.DarkGray, legR2Dead.X - 5, legR2Dead.Y - 5, 15, 10);
                // No weapons or effects when dead
                return;
            }

            // Drawing for living stickman
            Pen bodyPenLiving = new Pen(characterColor, 5); // Use characterColor
            Pen limbPenLiving = new Pen(Color.FromArgb(characterColor.R + 20 > 255 ? 255 : characterColor.R + 20, 
                                                        characterColor.G + 20 > 255 ? 255 : characterColor.G + 20, 
                                                        characterColor.B + 20 > 255 ? 255 : characterColor.B + 20), 7); // Slightly lighter limb color
            Pen swordPen = new Pen(Color.Lime, 7); // Green blade
            Pen swordGlow = new Pen(Color.FromArgb(120, 50, 255, 50), 22); // Bright green glow
            Pen handlePen = new Pen(Color.Gray, 14); // Grey handle
            Pen handleOutline = new Pen(Color.Black, 18); // Black outline for handle
            Pen shieldPen = new Pen(Color.Silver, 6);
            Brush shieldBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
            Pen axeHandlePen = new Pen(Color.SaddleBrown, 12);
            Pen axeBladePen = new Pen(Color.Gray, 14);
            // Head
            g.FillEllipse(Brushes.White, x - 18, y - 63, 36, 36);
            g.DrawEllipse(bodyPenLiving, x - 18, y - 63, 36, 36); // Use bodyPenLiving
            // Headband
            Rectangle headbandRect = new Rectangle(x - 18, y - 55, 36, 10);
            // Use player.Name to determine headband color (assuming Player 1 is left, Player 2 is right)
            g.FillRectangle(player == player1 ? Brushes.Blue : Brushes.Red, headbandRect);
            // Body
            g.DrawLine(bodyPenLiving, x, y - 30, x, y + 60); // Use bodyPenLiving
            // Arms
            Point armL1 = new Point(x, y - 10), armR1 = new Point(x, y - 10);
            Point armL2, armR2;

            if (blocking)
            {
                // Blocking pose: both arms forward, shield prominent, weapon hidden/lowered
                if (player == player1) // Player 1 blocking
                {
                    armL2 = new Point(x - 10, y + 10); // Shield arm forward
                    armR2 = new Point(x + 10, y + 30); // Sword arm lowered
                }
                else // Player 2 blocking
                {
                    armL2 = new Point(x - 10, y + 30); // Sword arm lowered
                    armR2 = new Point(x + 10, y + 10); // Shield arm forward
                }
                 // Draw arms for blocking pose
                g.DrawLine(limbPenLiving, armL1, armL2); // Use limbPenLiving
                g.DrawLine(limbPenLiving, armR1, armR2); // Use limbPenLiving

                // Draw shield in front
                // Use player object to determine which player is blocking
                if (player == player1) // Player 1 blocking (shield in left hand)
                {
                    int shieldX = armL2.X + 30, shieldY = armL2.Y - 20; // Position in front of body
                    g.FillEllipse(shieldBrush, shieldX - 25, shieldY - 25, 50, 50);
                    g.DrawEllipse(shieldPen, shieldX - 25, shieldY - 25, 50, 50);
                }
                else // Player 2 blocking (shield in right hand)
                {
                    int shieldX = armR2.X - 30, shieldY = armR2.Y - 20; // Position in front of body
                    g.FillEllipse(shieldBrush, shieldX - 25, shieldY - 25, 50, 50);
                    g.DrawEllipse(shieldPen, shieldX - 25, shieldY - 25, 50, 50);
                }
                // Weapons are not drawn in the blocking pose
            }
            else
            {
                // Normal pose: based on weapon/shield and attack animation
                 // Default arm positions
                Point armL1_normal = new Point(x, y - 10), armR1_normal = new Point(x, y - 10);
                Point armL2_normal, armR2_normal;

                if (player == player1) // Player 1: sword (right), shield (left)
                {
                    armL2_normal = new Point(x - 38, y + 35); // shield arm
                    armR2_normal = new Point(x + 30, y + 20); // sword arm
                }
                else // Player 2: sword (left), shield (right)
                {
                    armL2_normal = new Point(x - 30, y + 20); // sword arm
                    armR2_normal = new Point(x + 38, y + 35); // shield arm
                }
                 // Draw arms for normal pose
                g.DrawLine(limbPenLiving, armL1_normal, armL2_normal); // Use limbPenLiving
                g.DrawLine(limbPenLiving, armR1_normal, armR2_normal); // Use limbPenLiving

                // Draw shield (normal position)
                // Use player object to determine which player's shield to draw
                if (player == player1) // Player 1: shield in left hand
                {
                    int shieldX = armL2_normal.X, shieldY = armL2_normal.Y;
                    g.FillEllipse(shieldBrush, shieldX - 18, shieldY - 18, 36, 36);
                    g.DrawEllipse(shieldPen, shieldX - 18, shieldY - 18, 36, 36);
                }
                else // Player 2: shield in right hand
                {
                    int shieldX = armR2_normal.X, shieldY = armR2_normal.Y;
                    g.FillEllipse(shieldBrush, shieldX - 18, shieldY - 18, 36, 36);
                    g.DrawEllipse(shieldPen, shieldX - 18, shieldY - 18, 36, 36);
                }
                 // Draw weapon (normal position based on attack pose)
                // Use player object to determine which player's weapon to draw
                if (player == player1) // Player 1: sword in right hand
                {
                    // Sword arm (right)
                    int swordX1 = armR2_normal.X;
                    int swordY1 = armR2_normal.Y;
                    int swordX2, swordY2;
                    if (pose == 0) { swordX2 = x + 130; swordY2 = y - 20; }
                    else if (pose == 1) { swordX2 = x + 120; swordY2 = y - 60; }
                    else { swordX2 = x + 110; swordY2 = y + 110; }
                    // Calculate handle
                    double dx = swordX2 - swordX1, dy = swordY2 - swordY1;
                    double len = Math.Sqrt(dx * dx + dy * dy);
                    double ux = dx / len, uy = dy / len;
                    int handleLength = 28;
                    int handleX2 = (int)(swordX1 + ux * handleLength);
                    int handleY2 = (int)(swordY1 + uy * handleLength);
                    g.DrawLine(handleOutline, swordX1, swordY1, handleX2, handleY2);
                    g.DrawLine(handlePen, swordX1, swordY1, handleX2, handleY2);
                    g.DrawLine(swordGlow, handleX2, handleY2, swordX2, swordY2);
                    g.DrawLine(swordPen, handleX2, handleY2, swordX2, swordY2);
                }
                else // Player 2: sword in left hand (purple blade)
                {
                    // Sword arm (left)
                    int swordX1 = armL2_normal.X;
                    int swordY1 = armL2_normal.Y;
                    int swordX2, swordY2;
                    if (pose == 0) { swordX2 = x - 130; swordY2 = y - 20; }
                    else if (pose == 1) { swordX2 = x - 120; swordY2 = y - 60; }
                    else { swordX2 = x - 110; swordY2 = y + 110; }
                    // Calculate handle
                    double dx = swordX2 - swordX1, dy = swordY2 - swordY1;
                    double len = Math.Sqrt(dx * dx + dy * dy);
                    double ux = dx / len, uy = dy / len;
                    int handleLength = 28;
                    int handleX2 = (int)(swordX1 + ux * handleLength);
                    int handleY2 = (int)(swordY1 + uy * handleLength);
                    Pen purpleSwordPen = new Pen(Color.MediumPurple, 7);
                    Pen purpleSwordGlow = new Pen(Color.FromArgb(120, 128, 0, 255), 22);
                    g.DrawLine(handleOutline, swordX1, swordY1, handleX2, handleY2);
                    g.DrawLine(handlePen, swordX1, swordY1, handleX2, handleY2);
                    g.DrawLine(purpleSwordGlow, handleX2, handleY2, swordX2, swordY2);
                    g.DrawLine(purpleSwordPen, handleX2, handleY2, swordX2, swordY2);
                }
            }
             // Legs (with feet) - Moved outside blocking check
            Point legL1Living = new Point(x, y + 60), legL2Living = new Point(x - 22, y + 110);
            Point legR1Living = new Point(x, y + 60), legR2Living = new Point(x + 22, y + 110);
            g.DrawLine(limbPenLiving, legL1Living, legL2Living); // Use limbPenLiving
            g.DrawLine(limbPenLiving, legR1Living, legR2Living); // Use limbPenLiving
            // Feet (living) - Moved outside blocking check
            g.FillRectangle(Brushes.LightGray, legL2Living.X - 10, legL2Living.Y - 5, 15, 10);
            g.FillRectangle(Brushes.LightGray, legR2Living.X - 5, legR2Living.Y - 5, 15, 10);
        }

        // Draw a health bar above the stickman's head
        private void DrawHealthBar(Graphics g, int x, int y, int health, int maxHealth, bool left)
        {
            int barWidth = 70, barHeight = 12;
            int barX = x - barWidth / 2, barY = y;
            float percent = Math.Max(0, Math.Min(1, (float)health / maxHealth));

            // Draw the background (empty portion) of the health bar in grey
            using (Brush bg = new SolidBrush(Color.FromArgb(60, 60, 60)))
            {
                g.FillRectangle(bg, barX, barY, barWidth, barHeight);
            }

            // Draw the filled portion of the health bar with a Green to Yellow gradient
            RectangleF fillRect = new RectangleF(barX, barY, barWidth * percent, barHeight);
            using (System.Drawing.Drawing2D.LinearGradientBrush fillBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new PointF(barX, barY),
                new PointF(barX + barWidth, barY), // Gradient across the full width
                Color.LimeGreen, // Start color (Green)
                Color.Yellow)) // End color (Yellow)
            {
                 // Draw the gradient fill, but only up to the current health percentage
                 g.FillRectangle(fillBrush, fillRect);
            }

            using (Pen border = new Pen(Color.White, 2))
                g.DrawRectangle(border, barX, barY, barWidth, barHeight);
            // Draw health text
            string text = $"{health}/{maxHealth}";
            using (Font font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                SizeF sz = g.MeasureString(text, font);
                g.DrawString(text, font, textBrush, x - sz.Width / 2, barY - sz.Height - 2);
            }
        }

        // Draw a mana bar below the health bar
        private void DrawManaBar(Graphics g, int x, int y, int mana, int maxMana, bool left)
        {
            int barWidth = 70, barHeight = 8;
            int barX = x - barWidth / 2, barY = y;
            float percent = Math.Max(0, Math.Min(1, (float)mana / maxMana));
            Color fillColor = Color.FromArgb(0, 100, 255); // Blue mana color
            using (Brush bg = new SolidBrush(Color.FromArgb(60, 60, 60)))
                g.FillRectangle(bg, barX, barY, barWidth, barHeight);
            using (Brush fill = new SolidBrush(fillColor))
                g.FillRectangle(fill, barX, barY, (int)(barWidth * percent), barHeight);
            using (Pen border = new Pen(Color.White, 1))
                g.DrawRectangle(border, barX, barY, barWidth, barHeight);
        }

        // Draw a dialogue bubble above the stickman's head
        private void DrawDialogue(Graphics g, int x, int y, string text)
        {
            using (Font font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                SizeF sz = g.MeasureString(text, font);
                int padding = 8;
                RectangleF rect = new RectangleF(x - sz.Width / 2 - padding, y - sz.Height / 2 - padding, sz.Width + padding * 2, sz.Height + padding * 2);
                // Draw bubble background
                using (Brush bubbleBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0))) // Semi-transparent black
                using (Pen bubblePen = new Pen(Color.White, 2))
                {
                    g.FillRectangle(bubbleBrush, rect);
                    g.DrawRectangle(bubblePen, rect.X, rect.Y, rect.Width, rect.Height);
                }
                // Draw text
                g.DrawString(text, font, textBrush, x, y, sf);
            }
        }

        // Draw a blood effect around the hit location
        private void DrawBloodEffect(Graphics g, int x, int y)
        {
            using (Brush bloodBrush = new SolidBrush(Color.Red))
            {
                // Draw several small random circles/splatters
                for (int i = 0; i < 8; i++)
                {
                    int offsetX = effectRng.Next(-20, 21);
                    int offsetY = effectRng.Next(-20, 21);
                    int size = effectRng.Next(4, 10);
                    g.FillEllipse(bloodBrush, x + offsetX, y + offsetY, size, size);
                }
            }
        }

        // Draw skill status text below the stickman
        private void DrawSkillStatus(Graphics g, int x, int y, string text)
        {
            using (Font font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                 SizeF sz = g.MeasureString(text, font);
                 // Draw a subtle background for the text
                 RectangleF rect = new RectangleF(x - sz.Width / 2 - 5, y - sz.Height / 2 - 3, sz.Width + 10, sz.Height + 6);
                 using (Brush bg = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                 {
                     g.FillRectangle(bg, rect);
                 }
                 g.DrawString(text, font, textBrush, x, y, sf);
            }
        }

        /// <summary>
        /// Handles the Start Battle button click event.
        /// Now animates stickman sword fighting with move-in, dialogue, hit effects, and skills.
        /// </summary>
        private async void BtnStartBattle_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                string name1 = txtPlayer1Name.Text.Trim();
                string name2 = txtPlayer2Name.Text.Trim();
                if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
                    throw new ArgumentException("Both player names must be entered.");
                if (cmbPlayer1Class.SelectedIndex == -1 || cmbPlayer2Class.SelectedIndex == -1)
                    throw new ArgumentException("Both players must select a character class.");
                if (name1 == name2)
                    throw new ArgumentException("Player names must be different.");

                // Create characters
                player1 = CreateFighter(cmbPlayer1Class.SelectedItem.ToString(), name1);
                player2 = CreateFighter(cmbPlayer2Class.SelectedItem.ToString(), name2);

                // Reset UI and state
                lstBattleLog.Items.Clear();
                lblWinner.Text = "";
                UpdateHealthLabels(player1, player2); // Use player1 and player2
                player1Pose = 0; player2Pose = 0;
                player1Offset = 0; player2Offset = 0;
                player1Dir = 1; player2Dir = -1;
                player1Dialogue = ""; player2Dialogue = "";
                player1HitEffect = false; player2HitEffect = false;
                player1Dead = false; player2Dead = false;
                player1Blocking = false; player2Blocking = false;
                player1SkillReady = true; player2SkillReady = true;
                player1SkillText = "Skill Ready"; player2SkillText = "Skill Ready";
                pnlArena.Invalidate();
                await Task.Delay(400); // Initial delay

                // Reset potion usage flags at the start of a new battle
                player1PotionUsed = false;
                player2PotionUsed = false;

                // Start the animation timer which now acts as the game loop
                animationTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Factory method to create a ClassFighter based on selection.
        /// Demonstrates Polymorphism.
        /// </summary>
        private ClassFighter CreateFighter(string className, string name)
        {
            switch (className)
            {
                case "Bida-bida":
                    return new Pabida(name);
                case "Pabibo":
                    return new Pabibo(name);
                default:
                    throw new ArgumentException("Unknown character class selected.");
            }
        }

        /// <summary>
        /// Updates the health labels for both players.
        /// Now also updates mana labels and triggers a panel redraw.
        /// </summary>
        private void UpdateHealthLabels(ClassFighter f1, ClassFighter f2)
        {
            lastPlayer1Health = f1.Health;
            lastPlayer1MaxHealth = f1.MaxHealth;
            lastPlayer2Health = f2.Health;
            lastPlayer2MaxHealth = f2.MaxHealth;
            lastPlayer1Mana = f1.Mana;
            lastPlayer1MaxMana = f1.MaxMana;
            lastPlayer2Mana = f2.Mana;
            lastPlayer2MaxMana = f2.MaxMana;
            lblPlayer1Health.Text = $"Health: {f1.Health}/{f1.MaxHealth} Mana: {f1.Mana}/{f1.MaxMana}";
            lblPlayer2Health.Text = $"Health: {f2.Health}/{f2.MaxHealth} Mana: {f2.Mana}/{f2.MaxMana}";
            pnlArena.Invalidate();
        }

        // Get skill mana cost based on fighter type
        private int GetSkillManaCost(ClassFighter fighter)
        {
             if (fighter is Pabida) return 20;
             if (fighter is Pabibo) return 30;
             return 0; // Default
        }

        // Update skill status text based on fighter state
        private void UpdateSkillUI(ClassFighter f1, ClassFighter f2)
        {
            player1SkillReady = f1.IsSkillReady;
            player2SkillReady = f2.IsSkillReady;
            if (f1.IsSkillReady)
            {
                player1SkillText = "Skill Ready";
            }
            else
            {
                TimeSpan remaining = f1.SkillCooldownEnd - DateTime.Now;
                player1SkillText = $"CD: {remaining.TotalSeconds:F1}s";
            }
            if (f2.IsSkillReady)
            {
                player2SkillText = "Skill Ready";
            }
            else
            {
                TimeSpan remaining = f2.SkillCooldownEnd - DateTime.Now;
                player2SkillText = $"CD: {remaining.TotalSeconds:F1}s";
            }
        }

        private void pnlArena_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void lstBattleLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Get a random trashtalk phrase
        private string GetRandomTrashtalk()
        {
            return trashtalkPhrases[blockRng.Next(trashtalkPhrases.Length)];
        }

        private void lblWinner_Click(object sender, EventArgs e)
        {

        }

        // Draw a sword wave effect (now a piercing energy bolt)
        private void DrawSwordWave(Graphics g, int attackerX, int attackerY, int targetX, int targetY, bool startsLeft, float progress)
        {
            if (startsLeft) // Player 1's fire wave effect
            {
                // progress goes from 0 (start) to 1 (end)
                int alpha = (int)(255 * (1 - progress)); // Fade out
                if (alpha <= 0) return;

                // Calculate the current position of the bolt head
                int currentX = (int)(attackerX + (targetX - attackerX) * progress);
                int currentY = (int)(attackerY + (targetY - attackerY) * progress);

                float sizeProgress = progress < 0.5f ? progress * 2 : (1 - progress) * 2;
                int headSize = (int)(30 + sizeProgress * 40);

                if (headSize <= 0) headSize = 1;

                // Draw the main bolt head
                using (Brush headBrush = new SolidBrush(Color.FromArgb(alpha, 255, 100, 0)))
                using (Pen headPen = new Pen(Color.FromArgb(alpha, 255, 255, 0), 4))
                {
                    // Draw a diamond/arrow shape for the bolt head pointing right
                    Point[] diamond = new Point[]
                    {
                        new Point(currentX, currentY),
                        new Point(currentX - headSize / 2, currentY - headSize / 2),
                        new Point(currentX - headSize, currentY),
                        new Point(currentX - headSize / 2, currentY + headSize / 2)
                    };
                    g.FillPolygon(headBrush, diamond);
                    g.DrawPolygon(headPen, diamond);
                }

                // Draw fiery trailing effect
                using (Pen trailPen = new Pen(Color.FromArgb(alpha, 255, 60, 0), 6))
                {
                    trailPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    trailPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    int trailLength = (int)(250 * (1 - progress));
                    int numTrailSegments = 6;
                    for (int i = 0; i < numTrailSegments; i++)
                    {
                        float segmentProgress = (float)i / (numTrailSegments - 1);
                        int trailSegmentX = (int)(currentX - segmentProgress * trailLength);
                        int trailYOffset = (int)(Math.Sin(progress * Math.PI * 10 + i) * 12 * (1 - progress));
                        g.DrawLine(trailPen, currentX, currentY + trailYOffset, trailSegmentX, currentY + trailYOffset);
                    }
                }

                // Draw bright core glow
                using (Brush glowBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 50)))
                {
                    int glowSize = headSize + 15;
                    g.FillEllipse(glowBrush, currentX - glowSize / 2, currentY - glowSize / 2, glowSize, glowSize);
                }

                // Add fire particles
                if (alpha > 50)
                {
                    using (Brush particleBrush = new SolidBrush(Color.FromArgb(alpha, 255, 100, 0)))
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int pOffsetX = effectRng.Next(-(headSize + 10), (headSize + 10) + 1);
                            int pOffsetY = effectRng.Next(-(headSize + 10), (headSize + 10) + 1);
                            int pSize = effectRng.Next(3, 7);
                            g.FillEllipse(particleBrush, currentX + pOffsetX, currentY + pOffsetY, pSize, pSize);
                        }
                    }
                }
            }
            else // Player 2's vortex effect
            {
                int alpha = (int)(255 * (1 - progress));
                if (alpha <= 0) return;

                int currentX = (int)(attackerX + (targetX - attackerX) * progress);
                int currentY = (int)(attackerY + (targetY - attackerY) * progress);

                // Create a path for the spiral with bounds checking
                using (System.Drawing.Drawing2D.GraphicsPath spiralPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    float angle = 0;
                    float radius = Math.Max(1, 40 * (1 - progress)); // Ensure radius is never 0
                    float centerX = currentX;
                    float centerY = currentY;

                    PointF lastPoint = new PointF(centerX, centerY);
                    spiralPath.StartFigure();

                    // Create spiral effect with safety checks
                    for (float i = 0; i < 20; i += 0.1f)
                    {
                        angle = i * (float)Math.PI * 2;
                        radius = Math.Max(1, (20 - i) * (1 - progress) * 3); // Ensure radius is never 0
                        float x = centerX + (float)(Math.Cos(angle + progress * 10) * radius);
                        float y = centerY + (float)(Math.Sin(angle + progress * 10) * radius);

                        // Ensure points are valid
                        if (!float.IsInfinity(x) && !float.IsNaN(x) && 
                            !float.IsInfinity(y) && !float.IsNaN(y))
                        {
                            spiralPath.AddLine(lastPoint, new PointF(x, y));
                            lastPoint = new PointF(x, y);
                        }
                    }

                    // Draw the spiral with a gradient effect
                    if (spiralPath.PointCount > 0)
                    {
                        using (Pen spiralPen = new Pen(Color.FromArgb(alpha, 128, 0, 255), 4))
                        {
                            spiralPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                            spiralPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                            g.DrawPath(spiralPen, spiralPath);
                        }
                    }
                }

                // Add energy orb in the center with bounds checking
                int orbSize = Math.Max(1, (int)(50 * (1 - progress * 0.5f))); // Ensure size is never 0
                Rectangle orbBounds = new Rectangle(
                    currentX - orbSize/2,
                    currentY - orbSize/2,
                    Math.Max(1, orbSize),
                    Math.Max(1, orbSize)
                );

                using (System.Drawing.Drawing2D.GraphicsPath orbPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    orbPath.AddEllipse(orbBounds);
                    
                    // Create a gradient for the orb
                    using (System.Drawing.Drawing2D.PathGradientBrush orbBrush = 
                           new System.Drawing.Drawing2D.PathGradientBrush(orbPath))
                    {
                        orbBrush.CenterColor = Color.FromArgb(alpha, 255, 255, 255);
                        orbBrush.SurroundColors = new Color[] { Color.FromArgb(alpha, 128, 0, 255) };
                        g.FillPath(orbBrush, orbPath);
                    }
                }

                // Add energy particles with bounds checking
                if (alpha > 50)
                {
                    float particleRadius = Math.Max(1, 30 * (1 - progress)); // Ensure radius is never 0
                    for (int i = 0; i < 12; i++)
                    {
                        double particleAngle = i * Math.PI * 2 / 12 + progress * 10;
                        int px = (int)(currentX + Math.Cos(particleAngle) * particleRadius);
                        int py = (int)(currentY + Math.Sin(particleAngle) * particleRadius);
                        
                        // Ensure particle coordinates are valid
                        if (!double.IsInfinity(px) && !double.IsNaN(px) &&
                            !double.IsInfinity(py) && !double.IsNaN(py))
                        {
                            using (Brush particleBrush = new SolidBrush(Color.FromArgb(alpha, 180, 100, 255)))
                            {
                                int particleSize = Math.Max(1, effectRng.Next(2, 6)); // Ensure size is never 0
                                g.FillEllipse(particleBrush, 
                                    px - particleSize/2, 
                                    py - particleSize/2, 
                                    particleSize, 
                                    particleSize);
                            }
                        }
                    }
                }

                // Add outer glow ring with bounds checking
                float glowRadius = Math.Max(1, 60 * (1 - progress * 0.7f)); // Ensure radius is never 0
                Rectangle glowBounds = new Rectangle(
                    (int)(currentX - glowRadius),
                    (int)(currentY - glowRadius),
                    (int)(glowRadius * 2),
                    (int)(glowRadius * 2)
                );

                if (glowBounds.Width > 0 && glowBounds.Height > 0)
                {
                    using (Pen glowPen = new Pen(Color.FromArgb(alpha / 2, 180, 100, 255), 2))
                    {
                        g.DrawEllipse(glowPen, glowBounds);
                    }
                }
            }
        }

        // Draw critical hit flash effect
        private void DrawCriticalFlash(Graphics g, int x, int y)
        {
            // This method is now unused, but keeping it for reference or potential future use.
            // The sword wave effect replaces this.
        }

        // New method to handle potion usage
        private void UsePotion(ClassFighter player, bool isPlayer1)
        {
            if (isPlayer1)
            {
                if (!player1PotionUsed)
                {
                    // Change heal amount to a random value between 35 and 50
                    int healAmount = effectRng.Next(35, 51);
                    player.Heal(healAmount); // Assuming ClassFighter has a Heal method
                    player1PotionUsed = true;
                    lstBattleLog.Items.Add($"{player.Name} used a HP potion, restoring {healAmount} HP!");
                    // Add visual cue for healing
                    player1Dialogue = "Healing Galing!";
                    player1DialogueEndTime = DateTime.Now + dialogueDuration;
                    UpdateHealthLabels(player1, player2); // Update UI
                }
            }
            else // Player 2
            {
                if (!player2PotionUsed)
                {
                    // Change heal amount to a random value between 35 and 50
                    int healAmount = effectRng.Next(35, 51);
                    player.Heal(healAmount); // Assuming ClassFighter has a Heal method
                    player2PotionUsed = true;
                    lstBattleLog.Items.Add($"{player.Name} used a HP potion, restoring {healAmount} HP!");
                    // Add visual cue for healing
                    player2Dialogue = "Healing Bading!";
                    player2DialogueEndTime = DateTime.Now + dialogueDuration;
                     UpdateHealthLabels(player1, player2); // Update UI
                }
            }
        }
    }
}
