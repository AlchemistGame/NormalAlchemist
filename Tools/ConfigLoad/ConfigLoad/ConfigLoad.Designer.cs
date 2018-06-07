namespace ConfigLoad
{
    partial class ConfigLoad
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectFolder = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.message = new System.Windows.Forms.Label();
            this.OpenFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SelectFolder
            // 
            this.SelectFolder.Location = new System.Drawing.Point(290, 285);
            this.SelectFolder.Name = "SelectFolder";
            this.SelectFolder.Size = new System.Drawing.Size(75, 23);
            this.SelectFolder.TabIndex = 0;
            this.SelectFolder.Text = "选择文件夹";
            this.SelectFolder.UseVisualStyleBackColor = true;
            this.SelectFolder.Click += new System.EventHandler(this.SelectFolder_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(290, 314);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "开始序列化";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.Location = new System.Drawing.Point(12, 9);
            this.message.MaximumSize = new System.Drawing.Size(200, 0);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(77, 12);
            this.message.TabIndex = 2;
            this.message.Text = "请选择文件夹";
            // 
            // OpenFiles
            // 
            this.OpenFiles.Location = new System.Drawing.Point(209, 285);
            this.OpenFiles.Name = "OpenFiles";
            this.OpenFiles.Size = new System.Drawing.Size(75, 23);
            this.OpenFiles.TabIndex = 3;
            this.OpenFiles.Text = "打开文件夹";
            this.OpenFiles.UseVisualStyleBackColor = true;
            this.OpenFiles.Click += new System.EventHandler(this.OpenFiles_Click);
            // 
            // ConfigLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 349);
            this.Controls.Add(this.OpenFiles);
            this.Controls.Add(this.message);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.SelectFolder);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigLoad";
            this.Text = "ConfigLoad";
            this.Load += new System.EventHandler(this.ConfigLoad_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectFolder;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Label message;
        private System.Windows.Forms.Button OpenFiles;
    }
}

