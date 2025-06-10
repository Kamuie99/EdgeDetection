using OpenCvSharp;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace EdgeDetection
{
    public partial class Form1 : Form
    {
        private string[] imagePaths = new string[4]; // 이미지 경로 저장

        public Form1()
        {
            InitializeComponent();
        }

        // 이미지 불러오기 (Bitmap)
        private void btnLoadImages_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp",
                Title = "4개의 꼭짓점 이미지를 선택하세요"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileNames.Length != 4)
                {
                    MessageBox.Show("반드시 4장의 이미지를 선택해야 합니다.");
                    return;
                }

                // 이미지 표시 및 저장
                for (int i = 0; i < 4; i++)
                {
                    imagePaths[i] = ofd.FileNames[i];

                    // PictureBox에 이미지 표시할 때 Image Lock 방지용 Bitmap 생성
                    using (var tempImg = Image.FromFile(imagePaths[i]))
                    {
                        PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                        picBox.Image?.Dispose(); // 기존 이미지 리소스 해제
                        picBox.Image = new Bitmap(tempImg);
                    }

                    // 이미지 저장 경로 설정
                    string saveDir = Path.Combine(Application.StartupPath, "SavedImages");
                    Directory.CreateDirectory(saveDir);
                    string fileName = $"corner_{i + 1}.bmp";
                    string savePath = Path.Combine(saveDir, fileName);

                    // 이미지 저장 (덮어쓰기)
                    File.Copy(imagePaths[i], savePath, true);
                }

                MessageBox.Show("이미지 4장이 성공적으로 저장되었습니다.");
            }
        }

        // 엣지 검출 함수
        private void btnEdgeDetect_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(Application.StartupPath, "SavedImages");

            for (int i = 0; i < 4; i++)
            {
                string imagePath = Path.Combine(folderPath, $"corner_{i + 1}.bmp");

                if (!File.Exists(imagePath))
                {
                    MessageBox.Show($"이미지 파일이 존재하지 않습니다: corner_{i + 1}.bmp");
                    return;
                }

                // 1. Bitmap으로 이미지 불러오기
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    // 2. Bitmap → Mat 변환
                    using (Mat mat = BitmapToMat(bitmap))
                    {
                        // 3. 그레이스케일 변환
                        using (Mat gray = new Mat())
                        {
                            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

                            // 4. 블러링으로 노이즈 제거
                            using (Mat blurred = new Mat())
                            {
                                Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 1.5);

                                // 5. Canny 엣지 검출
                                using (Mat edges = new Mat())
                                {
                                    Cv2.Canny(blurred, edges, 50, 150);

                                    // 6. Mat → Bitmap 변환
                                    Bitmap edgeBitmap = MatToBitmap(edges);

                                    // 7. PictureBox에 표시
                                    PictureBox picBox = Controls.Find($"pictureBox{i + 1}", true)[0] as PictureBox;
                                    picBox.Image?.Dispose(); // 기존 이미지 메모리 해제
                                    picBox.Image = edgeBitmap;
                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show("엣지 검출 완료!");
        }

        // Bitmap → Mat 변환
        private Mat BitmapToMat(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageData = ms.ToArray();
                return Mat.FromImageData(imageData, ImreadModes.Color);
            }
        }

        // Mat → Bitmap 변환
        private Bitmap MatToBitmap(Mat mat)
        {
            byte[] imageData = mat.ToBytes(".png");
            using (var ms = new MemoryStream(imageData))
            {
                Bitmap bmp = new Bitmap(ms);
                return new Bitmap(bmp);
            }
        }



    }
}
