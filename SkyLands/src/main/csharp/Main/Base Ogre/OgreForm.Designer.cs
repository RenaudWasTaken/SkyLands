﻿using Awesomium.Core;

namespace Game.BaseApp {
    partial class OgreForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.webView = new Awesomium.Windows.Forms.WebControl(this.components);
            this.SuspendLayout();
            // 
            // webView
            // 
            this.webView.Location = new System.Drawing.Point(439, 672);
            this.webView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.webView.Size = new System.Drawing.Size(404, 44);
            this.webView.TabIndex = 0;
            // 
            // OgreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1449, 741);
            this.Controls.Add(this.webView);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "OgreForm";
            this.Text = "OgreForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Awesomium.Windows.Forms.WebControl webView;



    }
}