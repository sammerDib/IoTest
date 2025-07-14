namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    partial class ManualForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualForm));
            this.button1 = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.ManualLogPanel = new System.Windows.Forms.Panel();
            this.labelInputPath = new System.Windows.Forms.Label();
            this.labelOutputPath = new System.Windows.Forms.Label();
            this.textBoxInputPath = new System.Windows.Forms.TextBox();
            this.textBoxOutputPath = new System.Windows.Forms.TextBox();
            this.LabelLotID = new System.Windows.Forms.Label();
            this.textBoxLotID = new System.Windows.Forms.TextBox();
            this.checkBoxSaveData = new System.Windows.Forms.CheckBox();
            this.textBoxPeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._PhasesTiff = new System.Windows.Forms.RadioButton();
            this._PhasesBin = new System.Windows.Forms.RadioButton();
            //this._NormalsHbf = new System.Windows.Forms.RadioButton();
            this._NormalsTiff = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(726, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "Launch Mesure";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Stop
            // 
            this.Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Stop.Location = new System.Drawing.Point(726, 52);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(154, 32);
            this.Stop.TabIndex = 2;
            this.Stop.Text = "Stop Measure";
            this.Stop.UseMnemonic = false;
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.DbgTest_Click);
            // 
            // ManualLogPanel
            // 
            this.ManualLogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ManualLogPanel.Location = new System.Drawing.Point(12, 127);
            this.ManualLogPanel.Name = "ManualLogPanel";
            this.ManualLogPanel.Size = new System.Drawing.Size(868, 253);
            this.ManualLogPanel.TabIndex = 3;
            // 
            // labelInputPath
            // 
            this.labelInputPath.AutoSize = true;
            this.labelInputPath.Location = new System.Drawing.Point(22, 12);
            this.labelInputPath.Name = "labelInputPath";
            this.labelInputPath.Size = new System.Drawing.Size(56, 13);
            this.labelInputPath.TabIndex = 4;
            this.labelInputPath.Text = "Input Path";
            // 
            // labelOutputPath
            // 
            this.labelOutputPath.AutoSize = true;
            this.labelOutputPath.Location = new System.Drawing.Point(22, 37);
            this.labelOutputPath.Name = "labelOutputPath";
            this.labelOutputPath.Size = new System.Drawing.Size(64, 13);
            this.labelOutputPath.TabIndex = 5;
            this.labelOutputPath.Text = "Output Path";
            // 
            // textBoxInputPath
            // 
            this.textBoxInputPath.Location = new System.Drawing.Point(115, 9);
            this.textBoxInputPath.Name = "textBoxInputPath";
            this.textBoxInputPath.Size = new System.Drawing.Size(443, 20);
            this.textBoxInputPath.TabIndex = 6;
            // 
            // textBoxOutputPath
            // 
            this.textBoxOutputPath.Location = new System.Drawing.Point(115, 35);
            this.textBoxOutputPath.Name = "textBoxOutputPath";
            this.textBoxOutputPath.Size = new System.Drawing.Size(443, 20);
            this.textBoxOutputPath.TabIndex = 7;
            // 
            // LabelLotID
            // 
            this.LabelLotID.AutoSize = true;
            this.LabelLotID.Location = new System.Drawing.Point(22, 66);
            this.LabelLotID.Name = "LabelLotID";
            this.LabelLotID.Size = new System.Drawing.Size(36, 13);
            this.LabelLotID.TabIndex = 8;
            this.LabelLotID.Text = "Lot ID";
            // 
            // textBoxLotID
            // 
            this.textBoxLotID.Location = new System.Drawing.Point(115, 61);
            this.textBoxLotID.Name = "textBoxLotID";
            this.textBoxLotID.Size = new System.Drawing.Size(443, 20);
            this.textBoxLotID.TabIndex = 9;
            // 
            // checkBoxSaveData
            // 
            this.checkBoxSaveData.AutoSize = true;
            this.checkBoxSaveData.Location = new System.Drawing.Point(568, 90);
            this.checkBoxSaveData.Name = "checkBoxSaveData";
            this.checkBoxSaveData.Size = new System.Drawing.Size(87, 17);
            this.checkBoxSaveData.TabIndex = 11;
            this.checkBoxSaveData.Text = "Record Data";
            this.checkBoxSaveData.UseVisualStyleBackColor = true;
            this.checkBoxSaveData.Visible = false;
            this.checkBoxSaveData.CheckedChanged += new System.EventHandler(this.checkBoxSaveData_CheckedChanged);
            // 
            // textBoxPeriod
            // 
            this.textBoxPeriod.Location = new System.Drawing.Point(115, 87);
            this.textBoxPeriod.Name = "textBoxPeriod";
            this.textBoxPeriod.Size = new System.Drawing.Size(77, 20);
            this.textBoxPeriod.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Period";
            // 
            // _PhasesTiff
            // 
            this._PhasesTiff.AutoSize = true;
            //this._PhasesTiff.Checked = true;
            this._PhasesTiff.Location = new System.Drawing.Point(601, 10);
            this._PhasesTiff.Name = "_PhasesTiff";
            this._PhasesTiff.Size = new System.Drawing.Size(77, 17);
            this._PhasesTiff.TabIndex = 14;
            this._PhasesTiff.TabStop = true;
            this._PhasesTiff.Text = "Phases, tiff";
            this._PhasesTiff.UseVisualStyleBackColor = true;
            // 
            // _PhasesBin
            // 
            this._PhasesBin.AutoSize = true;
            this._PhasesBin.Location = new System.Drawing.Point(601, 33);
            this._PhasesBin.Name = "_PhasesBin";
            this._PhasesBin.Size = new System.Drawing.Size(80, 17);
            this._PhasesBin.TabIndex = 15;
            this._PhasesBin.Text = "Phases, bin";
            this._PhasesBin.UseVisualStyleBackColor = true;

            /*this._NormalsTiff.AutoSize = true;
            this._PhasesTiff.Location = new System.Drawing.Point(601, 33);
            this._PhasesTiff.Name = "_NormalsTiff";
            this._PhasesTiff.Size = new System.Drawing.Size(80, 17);
            this._PhasesTiff.TabIndex = 15;
            //this._PhasesTiff.TabStop = true;
            this._PhasesTiff.Text = "Normals, tiff";
            this._PhasesTiff.UseVisualStyleBackColor = true;*/

            // 
            // _NormalsHbf
            // 
            /*this._NormalsHbf.AutoSize = true;
            this._NormalsHbf.Location = new System.Drawing.Point(601, 56);
            this._NormalsHbf.Name = "_NormalsHbf";
            this._NormalsHbf.Size = new System.Drawing.Size(84, 17);
            this._NormalsHbf.TabIndex = 16;
            this._NormalsHbf.Text = "Normals, hbf";
            this._NormalsHbf.UseVisualStyleBackColor = true;*/
            // 
            // _NormalsTiff
            // 
            this._NormalsTiff.AutoSize = true;
            this._NormalsTiff.Checked = true;
            this._NormalsTiff.Location = new System.Drawing.Point(601, 56);
            this._NormalsTiff.Name = "_NormalsTiff";
            this._NormalsTiff.Size = new System.Drawing.Size(84, 17);
            this._NormalsTiff.TabIndex = 16;
            this._NormalsTiff.TabStop = true;
            this._NormalsTiff.Text = "Normals, tiff";
            this._NormalsTiff.UseVisualStyleBackColor = true;

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "px";
            // 
            // ManualForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 392);
            this.Controls.Add(this.label2);
            //this.Controls.Add(this._NormalsHbf);
            this.Controls.Add(this._NormalsTiff);
            this.Controls.Add(this._PhasesBin);
            this.Controls.Add(this._PhasesTiff);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPeriod);
            this.Controls.Add(this.checkBoxSaveData);
            this.Controls.Add(this.textBoxLotID);
            this.Controls.Add(this.LabelLotID);
            this.Controls.Add(this.textBoxOutputPath);
            this.Controls.Add(this.textBoxInputPath);
            this.Controls.Add(this.labelOutputPath);
            this.Controls.Add(this.labelInputPath);
            this.Controls.Add(this.ManualLogPanel);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ManualForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NanoTopography";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Panel ManualLogPanel;
        private System.Windows.Forms.Label labelInputPath;
        private System.Windows.Forms.TextBox textBoxInputPath; 
        private System.Windows.Forms.Label labelOutputPath;
        private System.Windows.Forms.TextBox textBoxOutputPath;
        private System.Windows.Forms.Label LabelLotID;
        private System.Windows.Forms.TextBox textBoxLotID;
        private System.Windows.Forms.CheckBox checkBoxSaveData;
        private System.Windows.Forms.TextBox textBoxPeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton _PhasesTiff;
        private System.Windows.Forms.RadioButton _PhasesBin;
        //private System.Windows.Forms.RadioButton _NormalsHbf;
        private System.Windows.Forms.RadioButton _NormalsTiff;
        private System.Windows.Forms.Label label2;
    }
}

