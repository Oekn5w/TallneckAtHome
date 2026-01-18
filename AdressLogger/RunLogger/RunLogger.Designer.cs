
namespace RunLogger
{
    partial class RunLogger
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
            this.btnLogCtl = new System.Windows.Forms.Button();
            this.timerLogging = new System.Windows.Forms.Timer(this.components);
            this.lblState = new System.Windows.Forms.Label();
            this.tbLogFolder = new System.Windows.Forms.TextBox();
            this.btnFolderSelect = new System.Windows.Forms.Button();
            this.cbGame = new System.Windows.Forms.ComboBox();
            this.folderDialogue = new System.Windows.Forms.FolderBrowserDialog();
            this.lblWriting = new System.Windows.Forms.Label();
            this.lblBuffer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLogCtl
            // 
            this.btnLogCtl.Location = new System.Drawing.Point(95, 39);
            this.btnLogCtl.Name = "btnLogCtl";
            this.btnLogCtl.Size = new System.Drawing.Size(232, 28);
            this.btnLogCtl.TabIndex = 0;
            this.btnLogCtl.Text = "Start logging";
            this.btnLogCtl.UseVisualStyleBackColor = true;
            this.btnLogCtl.Click += new System.EventHandler(this.btnLogCtl_Click);
            // 
            // timerLogging
            // 
            this.timerLogging.Enabled = true;
            this.timerLogging.Interval = 500;
            this.timerLogging.Tick += new System.EventHandler(this.timerLogging_Tick);
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.Location = new System.Drawing.Point(12, 73);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(154, 20);
            this.lblState.TabIndex = 1;
            this.lblState.Text = "State: Disconnected";
            // 
            // tbLogFolder
            // 
            this.tbLogFolder.Location = new System.Drawing.Point(16, 12);
            this.tbLogFolder.Name = "tbLogFolder";
            this.tbLogFolder.Size = new System.Drawing.Size(184, 20);
            this.tbLogFolder.TabIndex = 2;
            this.tbLogFolder.Text = "H:\\SaH_logs";
            this.tbLogFolder.TextChanged += new System.EventHandler(this.tbLogFolder_TextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(206, 10);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(121, 23);
            this.btnFolderSelect.TabIndex = 3;
            this.btnFolderSelect.Text = "SelectLogFolder";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.btnFolderSelect_Click);
            // 
            // cbGame
            // 
            this.cbGame.FormattingEnabled = true;
            this.cbGame.Items.AddRange(new object[] {
            "HZD",
            "HFW"});
            this.cbGame.Location = new System.Drawing.Point(16, 44);
            this.cbGame.Name = "cbGame";
            this.cbGame.Size = new System.Drawing.Size(73, 21);
            this.cbGame.TabIndex = 4;
            this.cbGame.SelectedIndexChanged += new System.EventHandler(this.cbGame_SelectedIndexChanged);
            // 
            // folderDialogue
            // 
            this.folderDialogue.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // lblWriting
            // 
            this.lblWriting.AutoSize = true;
            this.lblWriting.Location = new System.Drawing.Point(13, 96);
            this.lblWriting.Name = "lblWriting";
            this.lblWriting.Size = new System.Drawing.Size(50, 13);
            this.lblWriting.TabIndex = 5;
            this.lblWriting.Text = "WriteInfo";
            // 
            // lblBuffer
            // 
            this.lblBuffer.AutoSize = true;
            this.lblBuffer.Location = new System.Drawing.Point(12, 109);
            this.lblBuffer.Name = "lblBuffer";
            this.lblBuffer.Size = new System.Drawing.Size(53, 13);
            this.lblBuffer.TabIndex = 6;
            this.lblBuffer.Text = "BufferInfo";
            // 
            // RunLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 132);
            this.Controls.Add(this.lblBuffer);
            this.Controls.Add(this.lblWriting);
            this.Controls.Add(this.cbGame);
            this.Controls.Add(this.btnFolderSelect);
            this.Controls.Add(this.tbLogFolder);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.btnLogCtl);
            this.Name = "RunLogger";
            this.Text = "StalkerAtHome";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RunLogger_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogCtl;
        private System.Windows.Forms.Timer timerLogging;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.TextBox tbLogFolder;
        private System.Windows.Forms.Button btnFolderSelect;
        private System.Windows.Forms.ComboBox cbGame;
        private System.Windows.Forms.FolderBrowserDialog folderDialogue;
        private System.Windows.Forms.Label lblWriting;
        private System.Windows.Forms.Label lblBuffer;
    }
}

