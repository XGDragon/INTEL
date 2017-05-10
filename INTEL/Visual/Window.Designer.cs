namespace INTEL.Visual
{
    partial class Window
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
            this.NetworkLabel = new System.Windows.Forms.Label();
            this.SpeciesLabel = new System.Windows.Forms.Label();
            this.SpeciesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.GenomePanel = new System.Windows.Forms.Panel();
            this.GenomeCounter = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.GenomeCounter)).BeginInit();
            this.SuspendLayout();
            // 
            // NetworkLabel
            // 
            this.NetworkLabel.AutoSize = true;
            this.NetworkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NetworkLabel.Location = new System.Drawing.Point(9, 6);
            this.NetworkLabel.Name = "NetworkLabel";
            this.NetworkLabel.Size = new System.Drawing.Size(204, 16);
            this.NetworkLabel.TabIndex = 3;
            this.NetworkLabel.Text = "Most Fit Genome Representation";
            // 
            // SpeciesLabel
            // 
            this.SpeciesLabel.AutoSize = true;
            this.SpeciesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpeciesLabel.Location = new System.Drawing.Point(315, 6);
            this.SpeciesLabel.Name = "SpeciesLabel";
            this.SpeciesLabel.Size = new System.Drawing.Size(127, 16);
            this.SpeciesLabel.TabIndex = 4;
            this.SpeciesLabel.Text = "Species Distribution";
            // 
            // SpeciesPanel
            // 
            this.SpeciesPanel.Location = new System.Drawing.Point(318, 29);
            this.SpeciesPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SpeciesPanel.Name = "SpeciesPanel";
            this.SpeciesPanel.Size = new System.Drawing.Size(450, 299);
            this.SpeciesPanel.TabIndex = 5;
            // 
            // GenomePanel
            // 
            this.GenomePanel.Location = new System.Drawing.Point(12, 28);
            this.GenomePanel.Name = "GenomePanel";
            this.GenomePanel.Size = new System.Drawing.Size(300, 300);
            this.GenomePanel.TabIndex = 6;
            // 
            // GenomeCounter
            // 
            this.GenomeCounter.BackColor = System.Drawing.SystemColors.Info;
            this.GenomeCounter.Location = new System.Drawing.Point(253, 6);
            this.GenomeCounter.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GenomeCounter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GenomeCounter.Name = "GenomeCounter";
            this.GenomeCounter.Size = new System.Drawing.Size(59, 20);
            this.GenomeCounter.TabIndex = 7;
            this.GenomeCounter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GenomeCounter.ValueChanged += new System.EventHandler(this.GenomeCounter_ValueChanged);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 337);
            this.Controls.Add(this.GenomeCounter);
            this.Controls.Add(this.GenomePanel);
            this.Controls.Add(this.SpeciesPanel);
            this.Controls.Add(this.SpeciesLabel);
            this.Controls.Add(this.NetworkLabel);
            this.Name = "Window";
            this.Text = "Visual";
            ((System.ComponentModel.ISupportInitialize)(this.GenomeCounter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label NetworkLabel;
        private System.Windows.Forms.Label SpeciesLabel;
        private System.Windows.Forms.FlowLayoutPanel SpeciesPanel;
        private System.Windows.Forms.Panel GenomePanel;
        private System.Windows.Forms.NumericUpDown GenomeCounter;
    }
}

