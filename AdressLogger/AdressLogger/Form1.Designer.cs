
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
            this.buttonLogArm = new System.Windows.Forms.Button();
            this.timerLogging = new System.Windows.Forms.Timer(this.components);
            this.labelVel = new System.Windows.Forms.Label();
            this.nUD_Time = new System.Windows.Forms.NumericUpDown();
            this.labelState = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
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
            // buttonLogArm
            // 
            this.buttonLogArm.Location = new System.Drawing.Point(352, 38);
            this.buttonLogArm.Name = "buttonLogArm";
            this.buttonLogArm.Size = new System.Drawing.Size(111, 45);
            this.buttonLogArm.TabIndex = 1;
            this.buttonLogArm.Text = "Arm logging";
            this.buttonLogArm.UseVisualStyleBackColor = true;
            this.buttonLogArm.Click += new System.EventHandler(this.buttonLog_Click);
            // 
            // timerLogging
            // 
            this.timerLogging.Interval = 10000;
            this.timerLogging.Tick += new System.EventHandler(this.timerLogging_Tick);
            // 
            // labelVel
            // 
            this.labelVel.AutoSize = true;
            this.labelVel.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVel.Location = new System.Drawing.Point(190, 9);
            this.labelVel.Name = "labelVel";
            this.labelVel.Size = new System.Drawing.Size(140, 22);
            this.labelVel.TabIndex = 0;
            this.labelVel.Text = "Velocity-lat:";
            // 
            // nUD_Time
            // 
            this.nUD_Time.Location = new System.Drawing.Point(487, 63);
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
            this.labelState.Location = new System.Drawing.Point(12, 144);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(177, 24);
            this.labelState.TabIndex = 0;
            this.labelState.Text = "State: Disconnected";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(352, 89);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(111, 45);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop logging";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 177);
            this.Controls.Add(this.nUD_Time);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonLogArm);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.labelVel);
            this.Controls.Add(this.labelPos);
            this.Location = new System.Drawing.Point(-600, 500);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Time)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPos;
        private System.Windows.Forms.Timer timerMemory;
        private System.Windows.Forms.Button buttonLogArm;
        private System.Windows.Forms.Timer timerLogging;
        private System.Windows.Forms.Label labelVel;
        private System.Windows.Forms.NumericUpDown nUD_Time;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Button buttonStop;
    }
}

