
namespace AdressLogger
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelPos = new System.Windows.Forms.Label();
            this.timerMemory = new System.Windows.Forms.Timer(this.components);
            this.buttonLogStuff = new System.Windows.Forms.Button();
            this.timerLogging = new System.Windows.Forms.Timer(this.components);
            this.labelStates = new System.Windows.Forms.Label();
            this.nUD_Time = new System.Windows.Forms.NumericUpDown();
            this.labelState = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Time)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPos
            // 
            this.labelPos.AutoSize = true;
            this.labelPos.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPos.Location = new System.Drawing.Point(12, 9);
            this.labelPos.Name = "labelPos";
            this.labelPos.Size = new System.Drawing.Size(100, 88);
            this.labelPos.TabIndex = 0;
            this.labelPos.Text = "Position:\r\nnan\r\nnan\r\nnan";
            // 
            // timerMemory
            // 
            this.timerMemory.Enabled = true;
            this.timerMemory.Interval = 10;
            this.timerMemory.Tick += new System.EventHandler(this.timerMemory_Tick);
            // 
            // buttonLogStuff
            // 
            this.buttonLogStuff.Location = new System.Drawing.Point(12, 250);
            this.buttonLogStuff.Name = "buttonLogStuff";
            this.buttonLogStuff.Size = new System.Drawing.Size(100, 36);
            this.buttonLogStuff.TabIndex = 1;
            this.buttonLogStuff.Text = "Arm logging";
            this.buttonLogStuff.UseVisualStyleBackColor = true;
            this.buttonLogStuff.Click += new System.EventHandler(this.buttonLogStuff_Click);
            // 
            // timerLogging
            // 
            this.timerLogging.Interval = 10000;
            this.timerLogging.Tick += new System.EventHandler(this.timerLogging_Tick);
            // 
            // labelStates
            // 
            this.labelStates.AutoSize = true;
            this.labelStates.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStates.Location = new System.Drawing.Point(12, 115);
            this.labelStates.Name = "labelStates";
            this.labelStates.Size = new System.Drawing.Size(200, 110);
            this.labelStates.TabIndex = 0;
            this.labelStates.Text = "States:\r\nPause \r\nLoads \r\nInv  TF   CS   WinA\r\n0x00 0x00 0x00 0x00";
            // 
            // nUD_Time
            // 
            this.nUD_Time.Location = new System.Drawing.Point(118, 260);
            this.nUD_Time.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nUD_Time.Name = "nUD_Time";
            this.nUD_Time.Size = new System.Drawing.Size(57, 20);
            this.nUD_Time.TabIndex = 2;
            this.nUD_Time.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelState.Location = new System.Drawing.Point(12, 289);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(177, 24);
            this.labelState.TabIndex = 0;
            this.labelState.Text = "State: Disconnected";
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(162, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(60, 88);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "Vel:\r\nnan\r\nHead:\r\nnan";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 323);
            this.Controls.Add(this.nUD_Time);
            this.Controls.Add(this.buttonLogStuff);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.labelStates);
            this.Controls.Add(this.labelPos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(300, 500);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Tallneck At Home:";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Time)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPos;
        private System.Windows.Forms.Timer timerMemory;
        private System.Windows.Forms.Button buttonLogStuff;
        private System.Windows.Forms.Timer timerLogging;
        private System.Windows.Forms.Label labelStates;
        private System.Windows.Forms.NumericUpDown nUD_Time;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Label labelInfo;
    }
}

