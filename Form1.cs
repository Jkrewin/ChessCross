using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChessCross
{
    public partial class Form1 : Form
    {
        public Form1() => InitializeComponent();

        //размер игрового поля
        const int RecX = 8;
        const int RecY = 8;
        readonly Button[,] Board = new Button [RecX, RecY]; //игровое поле
        readonly ChipsClass[,] Chips = new ChipsClass[RecX, RecY]; //фишки
        readonly List<PlayerClass> Players = new List<PlayerClass>(); //игроки

        short _SelectPlayer = -1;

        /// <summary>
        /// выбирает игрока
        /// </summary>
        short SelectPlayer 
        {
            get => _SelectPlayer;
            set
            {
                if (Players.Count == 0) return;
                if (Players.Count == value)
                {
                    _SelectPlayer = 0;
                }
                else
                {
                    _SelectPlayer = value;
                }
                Label1.Text = "Игрок:   " + Players[_SelectPlayer].name;
                Label1.BackColor = Players[_SelectPlayer].color;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            short TabIndex = 1;
            // далее процесс копирования по шаблону PatternButton изменение этого контрола влияет на всех
            for (int x = 0; x < RecX; x++)
            {
                for (int y = 0; y < RecY; y++)
                {
                    Chips[x, y] = new ChipsClass();
                    Button b = new Button
                    {
                        BackColor = PatternButton.BackColor,
                        Location = new Point(100 + (PatternButton.Size.Width * x), 100 + (PatternButton.Size.Height * y)),
                        Margin = PatternButton.Margin,
                        Name = x + ";" + y,
                        Size = PatternButton.Size,
                        TabIndex = TabIndex++,      // это нужно для премещения с помощи tab 
                        UseVisualStyleBackColor = this.PatternButton.UseVisualStyleBackColor,
                        Visible = this.PatternButton.Visible
                    };
                    this.Controls.Add(b);                    //важно добавить в список контролов
                    b.Show();                                // и активировать 
                    b.Click += new EventHandler(SelectRec); // для всех тут будет обрабатываться нажатие
                    Board[x, y] = b;
                }
            }

            //добовляем игроков 
            PlayerClass p1 = new PlayerClass() { name = "A", color = Color.Blue };
            Players.Add(p1);
            StartPoint(StartPointEnum.НижнияПраваяТочка, p1);

            PlayerClass p2 = new PlayerClass() { name = "B", color = Color.Red };
            Players.Add(p2);
            StartPoint(StartPointEnum.ВерхняяЛеваяТочка, p2);

            SelectPlayer++;
            Displ.Start();
        }

        /// <summary>
        /// Распределяет фишки по полю
        /// </summary>
        void Push_Chips(int x, int y) {
            int move = 0;

            Point[] p = new Point[5];
            p[1] = new Point(x - 1, y);
            p[2] = new Point(x, y - 1);
            p[3] = new Point(x + 1, y);
            p[4] = new Point(x, y + 1);

            while (Chips[x, y].quantity != 0)
            {
                move++;
                if (p[move].X > RecX-1 | p[move].Y > RecY-1 | p[move].Y < 0 | p[move].X < 0) { } else {
                    SetChips(Chips[p[move].X, p[move].Y], Chips[x, y].player);
                    Chips[x, y].quantity--;
                }
                if (move == 4) move = 0;
            }
        }

        /// <summary>
        /// Устанавливает стартовую точку игрока
        /// </summary>
        void StartPoint(StartPointEnum en, PlayerClass pl)
        {
            switch (en)
            {
                case StartPointEnum.ВерхняяЛеваяТочка:
                    Chips[0, 1] = new ChipsClass { quantity = 3, player = pl };
                    Chips[1, 0] = new ChipsClass { quantity = 3, player = pl };
                    break;
                case StartPointEnum.ВерхняяПраваяТочка:
                    Chips[RecX - 1, 1] = new ChipsClass { quantity = 3, player = pl };
                    Chips[RecX - 1, 0] = new ChipsClass();
                    Chips[RecX - 2, 0].quantity = 3;
                    Chips[RecX - 2, 0].player = pl;
                    break;
                case StartPointEnum.НижнияЛеваяТочка:
                    Chips[0, RecY - 2] = new ChipsClass { quantity = 3, player = pl };
                    Chips[1, RecY - 1] = new ChipsClass { quantity = 3, player = pl };
                    break;
                case StartPointEnum.НижнияПраваяТочка:
                    Chips[RecX - 1, RecY - 2] = new ChipsClass { quantity = 3, player = pl };
                    Chips[RecX - 1, RecY - 1] = new ChipsClass();
                    Chips[RecX - 2, RecY - 1].quantity = 3;
                    Chips[RecX - 2, RecY - 1].player = pl;
                    break;
            }
        }

        /// <summary>
        /// обработка нажатия
        /// </summary>
        void SelectRec(object sender, EventArgs e) {
            var cButton = sender as Button;

            short x = short.Parse(cButton.Name.Split(';')[0]);
            short y = short.Parse(cButton.Name.Split(';')[1]);

            if (Players[SelectPlayer] == Chips[x, y].player)
            {
                SetChips(Chips[x, y], Players[SelectPlayer]);
                SelectPlayer++;
            }
            else { Label1.Text = "Сейчас ход игрока  " + Players[SelectPlayer].name; }
        }

        /// <summary>
        /// Устанавливает фишки
        /// </summary>
        void  SetChips(ChipsClass a, PlayerClass s) {
            if (a == null)
            {
                a = new ChipsClass { player = s };
                a.quantity++;
            }
            else {
                a.player = s;
                a.quantity++;
            }
        }      

        /// <summary>
        /// определяет стартовое положение игрока
        /// </summary>
        enum StartPointEnum
        {
            ВерхняяЛеваяТочка,
            ВерхняяПраваяТочка,
            НижнияЛеваяТочка,
            НижнияПраваяТочка
        }

        /// <summary>
        /// Фишки
        /// </summary>
        class ChipsClass
        {
            /// <summary>
            /// количество фишк сейчас
            /// </summary>
            public short quantity;
            /// <summary>
            /// какой игрок вледеет фишками
            /// </summary>
            public PlayerClass player;
        }

        /// <summary>
        /// игрок
        /// </summary>
        class PlayerClass
        {
            /// <summary>
            /// имя игрока
            /// </summary>
            public string name;
            /// <summary>
            /// цвет фишек
            /// </summary>
            public Color color;
            /// <summary>
            /// расчет победителя по фишкам
            /// </summary>
            public int coun;
        }

        /// <summary>
        /// тамер игры
        /// </summary>       
        private void Displ_Tick(object sender, EventArgs e)
        {
            Players.ForEach(x => x.coun = 0); // очистка 

            for (int x = 0; x < RecX; x++)
            {
                for (int y = 0; y < RecY; y++)
                {
                    if (Chips[x, y].player != null) {
                        if (Chips[x, y].quantity > 3) { Push_Chips(x, y); }
                        if (Chips[x, y].quantity == 0)
                        {
                            Chips[x, y] = new ChipsClass();
                            Board[x, y].Text = "";
                        }
                        else {
                            Chips[x, y].player.coun = Chips[x, y].player.coun + Chips[x, y].quantity;
                            Board[x, y].ForeColor = Chips[x, y].player.color;
                            Board[x, y].Text = Chips[x, y].quantity.ToString();
                        }
                    }
                }
            }

            foreach (var tv in Players)
            {
                if (tv.coun == 0)
                {
                    Displ.Stop();
                    for (int x = 0; x < RecX; x++)
                    {
                        for (int y = 0; y < RecY; y++)
                        {                            
                            Board[x, y].Enabled =false ;
                        }
                    }
                    MessageBox.Show("Игрок " + tv.name + " проиграл!");                    
                    break;
                }               
            }
            
        }
    }
}
