﻿using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms
{
    partial class UpdatePending
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatePending));
            this.BtnDownload = new Syncfusion.WinForms.Controls.SfButton();
            this.LblNewReleaseInfo = new System.Windows.Forms.Label();
            this.LblVersion = new System.Windows.Forms.Label();
            this.LblInfo1 = new System.Windows.Forms.Label();
            this.LblUpdateQuestion = new System.Windows.Forms.Label();
            this.BtnIgnore = new Syncfusion.WinForms.Controls.SfButton();
            this.PbUpdate = new System.Windows.Forms.PictureBox();
            this.LblRelease = new System.Windows.Forms.Label();
            this.TbReleaseNotes = new System.Windows.Forms.RichTextBox();
            this.PnlReleaseNotes = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.PbUpdate)).BeginInit();
            this.PnlReleaseNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnDownload
            // 
            this.BtnDownload.AccessibleDescription = "Continues the update process. Either opens the release page, or downloads and lau" +
    "nches the updater, depending on your update settings.";
            this.BtnDownload.AccessibleName = "Continue";
            this.BtnDownload.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnDownload.Enabled = false;
            this.BtnDownload.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnDownload.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnDownload.Location = new System.Drawing.Point(165, 465);
            this.BtnDownload.Name = "BtnDownload";
            this.BtnDownload.Size = new System.Drawing.Size(455, 31);
            this.BtnDownload.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnDownload.Style.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnDownload.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnDownload.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnDownload.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnDownload.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnDownload.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnDownload.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnDownload.TabIndex = 0;
            this.BtnDownload.Text = global::HASS.Agent.Resources.Localization.Languages.UpdatePending_BtnDownload;
            this.BtnDownload.UseVisualStyleBackColor = false;
            this.BtnDownload.Click += new System.EventHandler(this.BtnDownload_Click);
            // 
            // LblNewReleaseInfo
            // 
            this.LblNewReleaseInfo.AccessibleDescription = "New release version description.";
            this.LblNewReleaseInfo.AccessibleName = "Version description";
            this.LblNewReleaseInfo.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblNewReleaseInfo.AutoSize = true;
            this.LblNewReleaseInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblNewReleaseInfo.Location = new System.Drawing.Point(22, 26);
            this.LblNewReleaseInfo.Name = "LblNewReleaseInfo";
            this.LblNewReleaseInfo.Size = new System.Drawing.Size(197, 19);
            this.LblNewReleaseInfo.TabIndex = 1;
            this.LblNewReleaseInfo.Text = Languages.UpdatePending_LblNewReleaseInfo;
            // 
            // LblVersion
            // 
            this.LblVersion.AccessibleDescription = "The new release version.";
            this.LblVersion.AccessibleName = "Version";
            this.LblVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblVersion.AutoSize = true;
            this.LblVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LblVersion.Location = new System.Drawing.Point(280, 24);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(16, 21);
            this.LblVersion.TabIndex = 2;
            this.LblVersion.Text = "-";
            // 
            // LblInfo1
            // 
            this.LblInfo1.AccessibleDescription = "Release notes description.";
            this.LblInfo1.AccessibleName = "Release notes description";
            this.LblInfo1.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblInfo1.AutoSize = true;
            this.LblInfo1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblInfo1.Location = new System.Drawing.Point(1, 82);
            this.LblInfo1.Name = "LblInfo1";
            this.LblInfo1.Size = new System.Drawing.Size(92, 19);
            this.LblInfo1.TabIndex = 3;
            this.LblInfo1.Text = Languages.UpdatePending_LblInfo1;
            // 
            // LblUpdateQuestion
            // 
            this.LblUpdateQuestion.AccessibleDescription = "Next step in the update process question.";
            this.LblUpdateQuestion.AccessibleName = "Question";
            this.LblUpdateQuestion.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblUpdateQuestion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblUpdateQuestion.Location = new System.Drawing.Point(165, 437);
            this.LblUpdateQuestion.Name = "LblUpdateQuestion";
            this.LblUpdateQuestion.Size = new System.Drawing.Size(455, 17);
            this.LblUpdateQuestion.TabIndex = 5;
            this.LblUpdateQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnIgnore
            // 
            this.BtnIgnore.AccessibleDescription = "Closes the window and ignores this version. Will show a new notification when the" +
    "re\'s a next version.";
            this.BtnIgnore.AccessibleName = "Ignore";
            this.BtnIgnore.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnIgnore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnIgnore.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnIgnore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnIgnore.Location = new System.Drawing.Point(2, 465);
            this.BtnIgnore.Name = "BtnIgnore";
            this.BtnIgnore.Size = new System.Drawing.Size(154, 31);
            this.BtnIgnore.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnIgnore.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnIgnore.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnIgnore.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnIgnore.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnIgnore.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnIgnore.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnIgnore.TabIndex = 1;
            this.BtnIgnore.Text = Languages.UpdatePending_BtnIgnore;
            this.BtnIgnore.UseVisualStyleBackColor = false;
            this.BtnIgnore.Click += new System.EventHandler(this.BtnIgnore_Click);
            // 
            // PbUpdate
            // 
            this.PbUpdate.AccessibleDescription = "Celebration picture.";
            this.PbUpdate.AccessibleName = "Celebrate";
            this.PbUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.PbUpdate.Image = global::HASS.Agent.Properties.Resources.update;
            this.PbUpdate.Location = new System.Drawing.Point(541, 12);
            this.PbUpdate.Name = "PbUpdate";
            this.PbUpdate.Size = new System.Drawing.Size(70, 70);
            this.PbUpdate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PbUpdate.TabIndex = 7;
            this.PbUpdate.TabStop = false;
            // 
            // LblRelease
            // 
            this.LblRelease.AccessibleDescription = "Opens the new release\'s github webpage.";
            this.LblRelease.AccessibleName = "Release page";
            this.LblRelease.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.LblRelease.AutoSize = true;
            this.LblRelease.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LblRelease.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.LblRelease.Location = new System.Drawing.Point(2, 435);
            this.LblRelease.Name = "LblRelease";
            this.LblRelease.Size = new System.Drawing.Size(85, 19);
            this.LblRelease.TabIndex = 8;
            this.LblRelease.Text = Languages.UpdatePending_LblRelease;
            this.LblRelease.Click += new System.EventHandler(this.LblRelease_Click);
            // 
            // TbReleaseNotes
            // 
            this.TbReleaseNotes.AccessibleDescription = "The release notes for the new version.";
            this.TbReleaseNotes.AccessibleName = "Release notes";
            this.TbReleaseNotes.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.TbReleaseNotes.AutoWordSelection = true;
            this.TbReleaseNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.TbReleaseNotes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TbReleaseNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbReleaseNotes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TbReleaseNotes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TbReleaseNotes.Location = new System.Drawing.Point(0, 0);
            this.TbReleaseNotes.Name = "TbReleaseNotes";
            this.TbReleaseNotes.ReadOnly = true;
            this.TbReleaseNotes.Size = new System.Drawing.Size(617, 321);
            this.TbReleaseNotes.TabIndex = 19;
            this.TbReleaseNotes.Text = "";
            this.TbReleaseNotes.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.TbReleaseNotes_LinkClicked);
            // 
            // PnlReleaseNotes
            // 
            this.PnlReleaseNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlReleaseNotes.Controls.Add(this.TbReleaseNotes);
            this.PnlReleaseNotes.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PnlReleaseNotes.Location = new System.Drawing.Point(1, 104);
            this.PnlReleaseNotes.Name = "PnlReleaseNotes";
            this.PnlReleaseNotes.Size = new System.Drawing.Size(619, 323);
            this.PnlReleaseNotes.TabIndex = 20;
            // 
            // UpdatePending
            // 
            this.AccessibleDescription = "Information about a pending update.";
            this.AccessibleName = "Update pending";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.CaptionBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.CaptionFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CaptionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(620, 497);
            this.Controls.Add(this.PnlReleaseNotes);
            this.Controls.Add(this.LblRelease);
            this.Controls.Add(this.PbUpdate);
            this.Controls.Add(this.BtnIgnore);
            this.Controls.Add(this.LblUpdateQuestion);
            this.Controls.Add(this.LblInfo1);
            this.Controls.Add(this.LblVersion);
            this.Controls.Add(this.LblNewReleaseInfo);
            this.Controls.Add(this.BtnDownload);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.Name = "UpdatePending";
            this.ShowMaximizeBox = false;
            this.ShowMinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Languages.UpdatePending_Title;
            this.Load += new System.EventHandler(this.UpdatePending_Load);
            this.ResizeEnd += new System.EventHandler(this.UpdatePending_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.PbUpdate)).EndInit();
            this.PnlReleaseNotes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Syncfusion.WinForms.Controls.SfButton BtnDownload;
        private System.Windows.Forms.Label LblNewReleaseInfo;
        private System.Windows.Forms.Label LblVersion;
        private System.Windows.Forms.Label LblInfo1;
        private System.Windows.Forms.Label LblUpdateQuestion;
        private Syncfusion.WinForms.Controls.SfButton BtnIgnore;
        private System.Windows.Forms.PictureBox PbUpdate;
        private System.Windows.Forms.Label LblRelease;
        private System.Windows.Forms.RichTextBox TbReleaseNotes;
        private System.Windows.Forms.Panel PnlReleaseNotes;
    }
}

