namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    partial class LogPanel
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoglistView = new System.Windows.Forms.ListView();
            this.ColDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // LoglistView
            // 
            this.LoglistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColDate,
            this.ColType,
            this.ColDescription});
            this.LoglistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoglistView.FullRowSelect = true;
            this.LoglistView.Location = new System.Drawing.Point(0, 0);
            this.LoglistView.Name = "LoglistView";
            this.LoglistView.Size = new System.Drawing.Size(720, 250);
            this.LoglistView.TabIndex = 0;
            this.LoglistView.TileSize = new System.Drawing.Size(168, 30);
            this.LoglistView.UseCompatibleStateImageBehavior = false;
            this.LoglistView.View = System.Windows.Forms.View.Details;
            this.LoglistView.SizeChanged += new System.EventHandler(this.LoglistView_SizeChanged);
            // 
            // ColDate
            // 
            this.ColDate.Text = "Date";
            this.ColDate.Width = 168;
            // 
            // ColType
            // 
            this.ColType.Text = "Type";
            this.ColType.Width = 75;
            // 
            // ColDescription
            // 
            this.ColDescription.Text = "Description";
            this.ColDescription.Width = 467;
            // 
            // LogPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LoglistView);
            this.Name = "LogPanel";
            this.Size = new System.Drawing.Size(720, 250);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LoglistView;
        private System.Windows.Forms.ColumnHeader ColDate;
        private System.Windows.Forms.ColumnHeader ColType;
        private System.Windows.Forms.ColumnHeader ColDescription;
    }
}
