using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//довести исходное сообщение до такого размера(в битах), чтобы оно нацело делилось на размер блока(sizeOfBlock = 128 бит);
//разделить исходное сообщение на блоки;
//довести длину ключа до длины половины блока;
//перевести ключ в бинарный формат(в нули и единицы);
//провести над каждым блоком прямое преобразование сетью Фейстеля в течении 16-ти раундов.После каждого раунда необходимо выполнять циклический сдвиг ключа на заданное количество символов;
//соединить все блоки вместе; таким образом получим сообщение, зашифрованное алгоритмом DES.
namespace DES
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const int sizeOfBlock = 64; //в DES размер блока 64 бит, но поскольку в unicode символ в два раза длинее, то увеличим блок тоже в два раза
        private const int sizeOfChar = 16; //размер одного символа 

        private const int shiftKey = 10; //сдвиг ключа 

        private const int quantityOfRounds = 16; //количество раундов

        string[] Blocks; //сами блоки в двоичном формате

        /// <summary>
        /// довести строку до такого размера, чтобы она делилась на sizeOfBlock.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string StringToRightLength(string input)
        {
            while (((input.Length * sizeOfChar) % sizeOfBlock) != 0)
                input += " ";
            return input;
        }
        /// <summary>
        /// разбить строку в обычном (символьном) формате на блоки.
        /// </summary>
        /// <param name="input"></param>
        private void CutStringIntoBlocks(string input)
        {
            Blocks = new string[(input.Length * sizeOfChar) / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
                Blocks[i] = StringToBinaryFormat(Blocks[i]);
            }
        }
        /// <summary>
        /// разбить строку в двоичном формате на блоки.
        /// </summary>
        /// <param name="input"></param>
        private void CutBinaryStringIntoBlocks(string input)
        {
            Blocks = new string[input.Length / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
        }
        /// <summary>
        /// перевод строки в двоичный формат.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string StringToBinaryFormat(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                string char_binary = Convert.ToString(input[i], 2);

                while (char_binary.Length < sizeOfChar)
                    char_binary = "0" + char_binary;

                output += char_binary;
            }

            return output;
        }
        /// <summary>
        /// довести длину ключа до нужной длины.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lengthKey"></param>
        /// <returns></returns>
        private string CorrectKeyWord(string input, int lengthKey)
        {
            if (input.Length > lengthKey)
                input = input.Substring(0, lengthKey);
            else
                while (input.Length < lengthKey)
                    input = "0" + input;

            return input;
        }
        /// <summary>
        /// Один раунд шифрования алгоритмом DES.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string EncodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }
        /// <summary>
        /// Один раунд расшифровки алгоритмом DES.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string DecodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (XOR(f(L, key), R) + L);
        }
        /// <summary>
        /// XOR двух строк с двоичными данными.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private string XOR(string s1, string s2)
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }
        /// <summary>
        /// Шифрующая функция f
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private string f(string s1, string s2)
        {
            return XOR(s1, s2);
        }
        /// <summary>
        /// Вычисление ключа для следующего раунда шифрования DES
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string KeyToNextRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }
        /// <summary>
        /// Вычисление ключа для следующего раунда расшифровки DES
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string KeyToPrevRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }

            return key;
        }
        /// <summary>
        /// перевод строки с двоичными данными в символьный формат
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string StringFromBinaryToNormalFormat(string input)
        {
            string output = "";

            while (input.Length > 0)
            {
                string char_binary = input.Substring(0, sizeOfChar);
                input = input.Remove(0, sizeOfChar);

                int a = 0;
                int degree = char_binary.Length - 1;

                foreach (char c in char_binary)
                    a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

                output += ((char)a).ToString();
            }

            return output;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                string s = "";

                string key = textBox1.Text;

                StreamReader sr = new StreamReader("in.txt");

                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }

                sr.Close();

                s = StringToRightLength(s);

                CutStringIntoBlocks(s);

                key = CorrectKeyWord(key, s.Length / (2 * Blocks.Length));
                textBox1.Text = key;
                key = StringToBinaryFormat(key);

                for (int j = 0; j < quantityOfRounds; j++)
                {
                    for (int i = 0; i < Blocks.Length; i++)
                        Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

                    key = KeyToNextRound(key);
                }

                key = KeyToPrevRound(key);

                textBox2.Text = StringFromBinaryToNormalFormat(key);

                string result = "";

                for (int i = 0; i < Blocks.Length; i++)
                    result += Blocks[i];

                StreamWriter sw = new StreamWriter("out1.txt");
                sw.WriteLine(Utf8Encoder.GetString(Utf8Encoder.GetBytes(StringFromBinaryToNormalFormat(result)))); //переводим строку с двоичными данными в символьный формат
                sw.Close();

                Process.Start("out1.txt");
            }
            else
                MessageBox.Show("Введите ключевое слово!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0)
            {
                string s = "";

                string key = StringToBinaryFormat(textBox2.Text);

                StreamReader sr = new StreamReader("out1.txt");

                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }

                sr.Close();

                s = StringToBinaryFormat(s);

                CutBinaryStringIntoBlocks(s);

                for (int j = 0; j < quantityOfRounds; j++)
                {
                    for (int i = 0; i < Blocks.Length; i++)
                        Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

                    key = KeyToPrevRound(key);
                }

                key = KeyToNextRound(key);

                textBox1.Text = StringFromBinaryToNormalFormat(key);

                string result = "";

                for (int i = 0; i < Blocks.Length; i++)
                    result += Blocks[i];

                StreamWriter sw = new StreamWriter("out2.txt");
                sw.WriteLine(StringFromBinaryToNormalFormat(result));
                sw.Close();

                Process.Start("out2.txt");
            }
            else
                MessageBox.Show("Введите ключевое слово!");
        }
        private static readonly Encoding Utf8Encoder = Encoding.GetEncoding("UTF-8", new EncoderReplacementFallback(string.Empty),
new DecoderExceptionFallback());
    }
}
