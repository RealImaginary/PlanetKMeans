namespace PlanetKMeans
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonSnapshot = new System.Windows.Forms.Button();
            this.buttonRandom = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonGo = new System.Windows.Forms.Button();
            this.buttonRandomKMeans = new System.Windows.Forms.Button();
            this.buttonStepKMeans = new System.Windows.Forms.Button();
            this.buttonStep = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonSnapshot);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRandom);
            this.splitContainer1.Panel1.Controls.Add(this.buttonClear);
            this.splitContainer1.Panel1.Controls.Add(this.buttonGo);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRandomKMeans);
            this.splitContainer1.Panel1.Controls.Add(this.buttonStepKMeans);
            this.splitContainer1.Panel1.Controls.Add(this.buttonStep);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Size = new System.Drawing.Size(706, 462);
            this.splitContainer1.SplitterDistance = 149;
            this.splitContainer1.TabIndex = 0;
            // 
            // buttonSnapshot
            // 
            this.buttonSnapshot.Location = new System.Drawing.Point(26, 172);
            this.buttonSnapshot.Name = "buttonSnapshot";
            this.buttonSnapshot.Size = new System.Drawing.Size(75, 23);
            this.buttonSnapshot.TabIndex = 14;
            this.buttonSnapshot.Text = "snapshot";
            this.buttonSnapshot.UseVisualStyleBackColor = true;
            // 
            // buttonRandom
            // 
            this.buttonRandom.Location = new System.Drawing.Point(26, 403);
            this.buttonRandom.Name = "buttonRandom";
            this.buttonRandom.Size = new System.Drawing.Size(75, 23);
            this.buttonRandom.TabIndex = 13;
            this.buttonRandom.Text = "random";
            this.buttonRandom.UseVisualStyleBackColor = true;
            this.buttonRandom.Click += new System.EventHandler(this.buttonRandom_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(26, 374);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 12;
            this.buttonClear.Text = "clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(26, 252);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 11;
            this.buttonGo.Text = "go";
            this.buttonGo.UseVisualStyleBackColor = true;
            // 
            // buttonRandomKMeans
            // 
            this.buttonRandomKMeans.Location = new System.Drawing.Point(26, 310);
            this.buttonRandomKMeans.Name = "buttonRandomKMeans";
            this.buttonRandomKMeans.Size = new System.Drawing.Size(75, 23);
            this.buttonRandomKMeans.TabIndex = 8;
            this.buttonRandomKMeans.Text = "randomKMeans";
            this.buttonRandomKMeans.UseVisualStyleBackColor = true;
            this.buttonRandomKMeans.Click += new System.EventHandler(this.buttonRandomKMeans_Click);
            // 
            // buttonStepKMeans
            // 
            this.buttonStepKMeans.Location = new System.Drawing.Point(26, 281);
            this.buttonStepKMeans.Name = "buttonStepKMeans";
            this.buttonStepKMeans.Size = new System.Drawing.Size(75, 23);
            this.buttonStepKMeans.TabIndex = 9;
            this.buttonStepKMeans.Text = "stepKMeans";
            this.buttonStepKMeans.UseVisualStyleBackColor = true;
            this.buttonStepKMeans.Click += new System.EventHandler(this.buttonStepKMeans_Click);
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(26, 223);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 10;
            this.buttonStep.Text = "step";
            this.buttonStep.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 125);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Interval = 33;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 462);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Planet KMeans";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSnapshot;
        private System.Windows.Forms.Button buttonRandom;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Button buttonRandomKMeans;
        private System.Windows.Forms.Button buttonStepKMeans;
        private System.Windows.Forms.Button buttonStep;
    }
}

