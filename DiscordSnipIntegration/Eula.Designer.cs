/**
    Copyright (C) 2016 Adonis S. Deliannis

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace DiscordSnipIntegration
{
    partial class Eula
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose ( );
            }
            base.Dispose ( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ( )
        {
            this.eulaTxt = new System.Windows.Forms.TextBox();
            this.accept = new System.Windows.Forms.Button();
            this.decline = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // eulaTxt
            // 
            this.eulaTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eulaTxt.Location = new System.Drawing.Point(12, 12);
            this.eulaTxt.Multiline = true;
            this.eulaTxt.Name = "eulaTxt";
            this.eulaTxt.ReadOnly = true;
            this.eulaTxt.Size = new System.Drawing.Size(760, 508);
            this.eulaTxt.TabIndex = 0;
            // 
            // accept
            // 
            this.accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.accept.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.accept.Location = new System.Drawing.Point(697, 526);
            this.accept.Name = "accept";
            this.accept.Size = new System.Drawing.Size(75, 23);
            this.accept.TabIndex = 1;
            this.accept.Text = "I Accept";
            this.accept.UseVisualStyleBackColor = true;
            this.accept.Click += new System.EventHandler(this.accept_Click);
            // 
            // decline
            // 
            this.decline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.decline.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.decline.Location = new System.Drawing.Point(616, 526);
            this.decline.Name = "decline";
            this.decline.Size = new System.Drawing.Size(75, 23);
            this.decline.TabIndex = 2;
            this.decline.Text = "I Decline";
            this.decline.UseVisualStyleBackColor = true;
            this.decline.Click += new System.EventHandler(this.decline_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Image = global::DiscordSnipIntegration.DSI.gplv3;
            this.pictureBox1.Location = new System.Drawing.Point(12, 526);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(88, 33);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // Eula
            // 
            this.AcceptButton = this.accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.decline;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.decline);
            this.Controls.Add(this.accept);
            this.Controls.Add(this.eulaTxt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Eula";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Eula";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox eulaTxt;
        private System.Windows.Forms.Button accept;
        private System.Windows.Forms.Button decline;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}