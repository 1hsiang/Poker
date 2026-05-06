using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPoker : Form
    {
        PictureBox[] pic = new PictureBox[5];

        int[] allPoker = new int[52];
        int[] playerPoker = new int[5];

        int totalMoney = 0;
        int currentBet = 0;
        bool gameStarted = false;

        Random rand = new Random();

        public frmPoker()
        {
            InitializeComponent();

            InitializePoker();
            InitializeBetSystem();
            BindEvents();
        }

        private void InitializePoker()
        {
            grpPoker.Controls.Clear();

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();

                pic[i].Name = "pic" + i;
                pic[i].Image = GetImage("back");
                pic[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pic[i].Width = 80;
                pic[i].Height = 110;
                pic[i].Top = 30;
                pic[i].Left = 25 + ((pic[i].Width + 25) * i);

                pic[i].Enabled = false;
                pic[i].Tag = "front";
                pic[i].BorderStyle = BorderStyle.None;
                pic[i].Visible = true;

                grpPoker.Controls.Add(pic[i]);
            }
        }

        private void InitializeBetSystem()
        {
            cmbRole.Items.Clear();
            cmbRole.Items.Add("富二代");
            cmbRole.Items.Add("凡人");
            cmbRole.Items.Add("窮人");
            cmbRole.SelectedIndex = 1;

            cmbBet.Items.Clear();
            cmbBet.Items.Add("100");
            cmbBet.Items.Add("500");
            cmbBet.Items.Add("1000");
            cmbBet.Items.Add("5000");
            cmbBet.Text = "500";

            btnDealCard.Enabled = false;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;

            lblResult.Text = "請選擇出生背景並下注";

            this.AcceptButton = btnBet;
        }

        private void BindEvents()
        {
            cmbRole.SelectedIndexChanged -= cmbRole_SelectedIndexChanged;
            cmbRole.SelectedIndexChanged += cmbRole_SelectedIndexChanged;

            btnBet.Click -= btnBet_Click;
            btnBet.Click += btnBet_Click;

            btnDealCard.Click -= btnDealCard_Click;
            btnDealCard.Click += btnDealCard_Click;

            btnChangeCard.Click -= btnChangeCard_Click;
            btnChangeCard.Click += btnChangeCard_Click;

            btnCheck.Click -= btnCheck_Click;
            btnCheck.Click += btnCheck_Click;

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Click -= Pic_Click;
                pic[i].Click += Pic_Click;
            }
        }

        private void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameStarted) return;

            switch (cmbRole.Text)
            {
                case "富二代":
                    totalMoney = 1000000;
                    lblResult.Text = "出生背景：富二代，目前資金：1000000";
                    break;

                case "凡人":
                    totalMoney = 100000;
                    lblResult.Text = "出生背景：凡人，目前資金：100000";
                    break;

                case "窮人":
                    totalMoney = 10000;
                    lblResult.Text = "出生背景：窮人，目前資金：10000";
                    break;
            }
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            int bet;

            if (!int.TryParse(cmbBet.Text, out bet))
            {
                lblResult.Text = "請輸入正確的押注金額";
                return;
            }

            if (bet <= 0)
            {
                lblResult.Text = "押注金額必須大於 0";
                return;
            }

            if (bet > totalMoney)
            {
                lblResult.Text = "資金不足，請降低押注金額";
                return;
            }

            currentBet = bet;
            totalMoney -= bet;

            gameStarted = true;
            cmbRole.Enabled = false;

            btnDealCard.Enabled = true;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;

            lblResult.Text = "下注成功：" + bet +
                             "，目前資金：" + totalMoney +
                             "，請按「發牌」";
        }

        private async void btnDealCard_Click(object sender, EventArgs e)
        {
            if (currentBet <= 0)
            {
                lblResult.Text = "請先下注";
                return;
            }

            lblResult.Text = "發牌中...";

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = GetImage("back");
                pic[i].Enabled = false;
                pic[i].Tag = "front";
                pic[i].BorderStyle = BorderStyle.None;
            }

            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }

            Shuffle();

            await Task.Delay(300);

            for (int i = 0; i < playerPoker.Length; i++)
            {
                playerPoker[i] = allPoker[i];
            }

            ShowCards();

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = true;
                pic[i].Tag = "front";
                pic[i].BorderStyle = BorderStyle.None;
            }

            btnDealCard.Enabled = false;
            btnChangeCard.Enabled = true;
            btnCheck.Enabled = false;

            lblResult.Text = "已發牌，請點選想換掉的牌，再按「換牌」";
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox clickedPic = sender as PictureBox;

            int index = Array.IndexOf(pic, clickedPic);
            if (index < 0) return;

            int cardNum = playerPoker[index] + 1;

            if (clickedPic.Tag.ToString() == "front")
            {
                clickedPic.Tag = "back";
                clickedPic.Image = GetImage("back");
                clickedPic.BorderStyle = BorderStyle.Fixed3D;
                lblResult.Text = "已選擇第 " + (index + 1) + " 張牌要換";
            }
            else
            {
                clickedPic.Tag = "front";
                clickedPic.Image = GetImage(cardNum);
                clickedPic.BorderStyle = BorderStyle.None;
                lblResult.Text = "取消選擇第 " + (index + 1) + " 張牌";
            }
        }

        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int startIndex = 5;
            int changeCount = 0;

            for (int i = 0; i < playerPoker.Length; i++)
            {
                if (pic[i].Tag.ToString() == "back")
                {
                    playerPoker[i] = allPoker[startIndex];
                    startIndex++;
                    changeCount++;
                }
            }

            ShowCards();

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = false;
                pic[i].Tag = "front";
                pic[i].BorderStyle = BorderStyle.None;
            }

            btnChangeCard.Enabled = false;
            btnCheck.Enabled = true;

            lblResult.Text = "換牌完成，共換了 " + changeCount + " 張，請按「判斷牌型」";
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            string result = CheckHand();
            int multiplier = GetMultiplier(result);
            int winMoney = currentBet * multiplier;

            totalMoney += winMoney;

            lblResult.Text = "牌型：" + result +
                             "，賠率：" + multiplier +
                             "，中獎金額：" + winMoney +
                             "，目前資金：" + totalMoney;

            currentBet = 0;

            btnDealCard.Enabled = false;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;

            if (totalMoney <= 0)
            {
                lblResult.Text += "。你破產了，請重新選擇出生背景。";
                gameStarted = false;
                cmbRole.Enabled = true;
            }
            else
            {
                lblResult.Text += "。請重新下注開始下一局。";
            }
        }

        private void Shuffle()
        {
            for (int i = 0; i < allPoker.Length; i++)
            {
                int r = rand.Next(allPoker.Length);

                int temp = allPoker[i];
                allPoker[i] = allPoker[r];
                allPoker[r] = temp;
            }
        }

        private void ShowCards()
        {
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pic[i].Image = GetImage(playerPoker[i] + 1);
            }
        }

        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        private Image GetImage(int num)
        {
            return GetImage("pic" + num);
        }

        private string CheckHand()
        {
            int[] pokerColor = new int[5];
            int[] pokerPoint = new int[5];

            for (int i = 0; i < playerPoker.Length; i++)
            {
                pokerColor[i] = playerPoker[i] % 4;
                pokerPoint[i] = playerPoker[i] / 4;
            }

            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            for (int i = 0; i < 5; i++)
            {
                colorCount[pokerColor[i]]++;
                pointCount[pokerPoint[i]]++;
            }

            Array.Sort(pointCount);
            Array.Reverse(pointCount);

            bool isFlush = colorCount.Any(c => c == 5);

            int[] sortedPoint = pokerPoint.OrderBy(x => x).ToArray();

            bool isNormalStraight = true;

            for (int i = 0; i < sortedPoint.Length - 1; i++)
            {
                if (sortedPoint[i + 1] - sortedPoint[i] != 1)
                {
                    isNormalStraight = false;
                    break;
                }
            }

            bool isRoyal = pokerPoint.Contains(0) &&
                           pokerPoint.Contains(9) &&
                           pokerPoint.Contains(10) &&
                           pokerPoint.Contains(11) &&
                           pokerPoint.Contains(12);

            bool isStraight = isNormalStraight || isRoyal;

            if (isRoyal && isFlush)
                return "皇家同花順";
            if (isStraight && isFlush)
                return "同花順";
            if (pointCount[0] == 4)
                return "四條";
            if (pointCount[0] == 3 && pointCount[1] == 2)
                return "葫蘆";
            if (isFlush)
                return "同花";
            if (isStraight)
                return "順子";
            if (pointCount[0] == 3)
                return "三條";
            if (pointCount[0] == 2 && pointCount[1] == 2)
                return "兩對";
            if (pointCount[0] == 2)
                return "一對";

            return "無";
        }

        private int GetMultiplier(string type)
        {
            switch (type)
            {
                case "皇家同花順": return 250;
                case "同花順": return 50;
                case "四條": return 25;
                case "葫蘆": return 9;
                case "同花": return 6;
                case "順子": return 4;
                case "三條": return 3;
                case "兩對": return 2;
                case "一對": return 1;
                default: return 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnBet_Click(sender, e);
        }
    }
}