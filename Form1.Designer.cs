﻿namespace EdgeDetection
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoadImages = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btnEdgeDetect = new System.Windows.Forms.Button();
            this.btnSetROI = new System.Windows.Forms.Button();
            this.btnSaveRoiOnly = new System.Windows.Forms.Button();
            this.btnDetectChamfer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadImages
            // 
            this.btnLoadImages.Location = new System.Drawing.Point(684, 41);
            this.btnLoadImages.Name = "btnLoadImages";
            this.btnLoadImages.Size = new System.Drawing.Size(131, 23);
            this.btnLoadImages.TabIndex = 0;
            this.btnLoadImages.Text = "이미지 불러오기";
            this.btnLoadImages.UseVisualStyleBackColor = true;
            this.btnLoadImages.Click += new System.EventHandler(this.btnLoadImages_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(350, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 300);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(44, 41);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(300, 300);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(44, 347);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(300, 300);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 3;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(350, 347);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(300, 300);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            // 
            // btnEdgeDetect
            // 
            this.btnEdgeDetect.Location = new System.Drawing.Point(684, 70);
            this.btnEdgeDetect.Name = "btnEdgeDetect";
            this.btnEdgeDetect.Size = new System.Drawing.Size(131, 23);
            this.btnEdgeDetect.TabIndex = 5;
            this.btnEdgeDetect.Text = "캐니 엣지 검출";
            this.btnEdgeDetect.UseVisualStyleBackColor = true;
            this.btnEdgeDetect.Click += new System.EventHandler(this.btnEdgeDetect_Click);
            // 
            // btnSetROI
            // 
            this.btnSetROI.Location = new System.Drawing.Point(684, 99);
            this.btnSetROI.Name = "btnSetROI";
            this.btnSetROI.Size = new System.Drawing.Size(131, 23);
            this.btnSetROI.TabIndex = 7;
            this.btnSetROI.Text = "ROI 영역 지정";
            this.btnSetROI.UseVisualStyleBackColor = true;
            this.btnSetROI.Click += new System.EventHandler(this.btnSetROI_Click);
            // 
            // btnSaveRoiOnly
            // 
            this.btnSaveRoiOnly.Location = new System.Drawing.Point(684, 128);
            this.btnSaveRoiOnly.Name = "btnSaveRoiOnly";
            this.btnSaveRoiOnly.Size = new System.Drawing.Size(131, 23);
            this.btnSaveRoiOnly.TabIndex = 8;
            this.btnSaveRoiOnly.Text = "ROI 영역 저장";
            this.btnSaveRoiOnly.UseVisualStyleBackColor = true;
            this.btnSaveRoiOnly.Click += new System.EventHandler(this.btnSaveRoiOnly_Click);
            // 
            // btnDetectChamfer
            // 
            this.btnDetectChamfer.Location = new System.Drawing.Point(684, 157);
            this.btnDetectChamfer.Name = "btnDetectChamfer";
            this.btnDetectChamfer.Size = new System.Drawing.Size(131, 23);
            this.btnDetectChamfer.TabIndex = 9;
            this.btnDetectChamfer.Text = "C0.7 검출";
            this.btnDetectChamfer.UseVisualStyleBackColor = true;
            this.btnDetectChamfer.Click += new System.EventHandler(this.btnDetectChamfer_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 673);
            this.Controls.Add(this.btnDetectChamfer);
            this.Controls.Add(this.btnSaveRoiOnly);
            this.Controls.Add(this.btnSetROI);
            this.Controls.Add(this.btnEdgeDetect);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnLoadImages);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadImages;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button btnEdgeDetect;
        private System.Windows.Forms.Button btnSetROI;
        private System.Windows.Forms.Button btnSaveRoiOnly;
        private System.Windows.Forms.Button btnDetectChamfer;
    }
}

