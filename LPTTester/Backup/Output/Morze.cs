using System;
using System.Collections.Generic;
using System.Text;

namespace Output
{
    public class Command
    {
        public bool enable;
        public int latency;
        public Command(bool Enable, int Latency)
        {
            enable = Enable;
            latency = Latency;
        }
    }
    public class Morze
    {
        private int dot;
        public Morze(int Dot)
        {
            dot = Dot;
        }
        public List<Command> Encode(string text)
        {
            text.Replace("\n\r", "\n");
            text.Replace("\r\n", "\n");
            text = text.ToUpper();
            List<Command> c = new List<Command>();
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0)
                {
                    if (text[i] != ' ' && text[i] != '\t' && text[i] != '\n' && text[i - 1] != ' ' && text[i - 1] != '\t' && text[i - 1] != '\n')
                    {
                        EncodeMorzeChar(c, MorzeChar.char_pause);
                    }
                }
                EncodeChar(c, text[i]);
            }
            if (text.Length > 0)
            {
                EncodeMorzeChar(c, MorzeChar.word_pause);
            }
            List<MorzeChar> finish = new List<MorzeChar>();
            finish.Add(MorzeChar.dot);
            finish.Add(MorzeChar.element_pause);
            finish.Add(MorzeChar.dot);
            finish.Add(MorzeChar.element_pause);
            finish.Add(MorzeChar.dash);
            finish.Add(MorzeChar.element_pause);
            finish.Add(MorzeChar.dot);
            finish.Add(MorzeChar.element_pause);
            finish.Add(MorzeChar.dash);
            finish.Add(MorzeChar.element_pause);
            for (int i = 0; i < finish.Count; i++)
            {
                EncodeMorzeChar(c, finish[i]);
            }

            return c;
        }
        public enum MorzeChar
        {
            dot,
            dash,
            element_pause,
            char_pause,
            word_pause
        }
        private void EncodeMorzeChar(List<Command> c, MorzeChar cr)
        {
            switch (cr)
            {
                case MorzeChar.char_pause:
                    c.Add(new Command(false, dot * 3));
                    break;
                case MorzeChar.element_pause:
                    c.Add(new Command(false, dot));
                    break;
                case MorzeChar.dot:
                    c.Add(new Command(true, dot));
                    break;
                case MorzeChar.dash:
                    c.Add(new Command(true, dot * 3));
                    break;
                case MorzeChar.word_pause:
                    c.Add(new Command(false, dot * 7));
                    break;
            }
        }
        private static string GetCodesOnChar(char cr)
        {   
            switch (cr)
            {
                case '!':
                    return "--..--";
                case '-':
                    return "-....-";
                case '/':
                    return "-..-.";
                case '0':
                    return "-----";
                case '1':
                    return ".----";
                case '2':
                    return "..---";
                case '3':
                    return "...--";
                case '4':
                    return "....-";
                case '5':
                    return ".....";
                case '6':
                    return "-....";
                case '7':
                    return "--...";
                case '8':
                    return "---..";
                case '9':
                    return "----.";
                case ';':
                    return "-.-.-";
                case '?':
                    return "..--..";
                case '@':
                    return ".--.-.";
                case 'A':
                    return ".-";
                case 'А':
                    return ".-";
                case '\'':
                    return ".----.";
                case 'B':
                    return "-...";
                case 'Б':
                    return "-...";
                case 'W':
                    return ".--";
                case 'В':
                    return ".--";
                case 'G':
                    return "--.";
                case 'Г':
                    return "--.";
                case 'D' :
                    return "-..";
                case 'Д':
                    return "-..";
                case ':':
                    return "---...";
                case 'E':
                    return ".";
                case 'Е':
                    return ".";
                case 'V':
                    return "...-";
                case 'Ж':
                    return "...-";
                case 'Z':
                    return "--..";
                case 'З':
                    return "--..";
                case ',':
                    return ".-.-.-";
                case '|':
                    return "-...-";
                case 'I':
                    return "..";
                case 'И':
                    return "..";
                case 'J':
                    return ".---";
                case 'Й':
                    return ".---";
                case 'K':
                    return "-.-";
                case 'К':
                    return "-.-";
                case '\"':
                    return ".-..-.";
                case 'L':
                    return ".-..";
                case 'Л':
                    return ".-..";
                case 'M':
                    return "--";
                case 'М':
                    return "--";
                case 'N':
                    return "-.";
                case 'Н':
                    return "-.";
                case 'O':
                    return "---";
                case 'О':
                    return "---";
                case 'P':
                    return ".--.";
                case 'П':
                    return ".--.";
                case 'R':
                    return ".-.";
                case 'Р':
                    return ".-.";
                case 'S':
                    return "...";
                case 'С':
                    return "...";
                case '(':
                    return "-.--.-";
                case ')':
                    return "-.--.-";
                case 'T':
                    return "-";
                case 'Т':
                    return "-";
                case '.':
                    return "......";
                case 'U':
                    return "..-";
                case 'У':
                    return "..-";
                case 'F':
                    return "..-.";
                case 'Ф':
                    return "..-.";
                case 'H':
                    return "....";
                case 'Х':
                    return "....";
                case 'C':
                    return "-.-.";
                case 'Ц':
                    return "-.-.";
                case 'Ч':
                    return "---.";
                case 'Ш':
                    return "----";
                case 'Q':
                    return "--.-";
                case 'Щ':
                    return "--.-";
                case 'Ъ':
                    return "--.--";
                case 'Y':
                    return "-.--";
                case 'Ы':
                    return "-.--";
                case 'X':
                    return "-..-";
                case 'Ь':
                    return "-..-";
                case 'Э':
                    return "..-..";
                case 'Ю':
                    return "..--";
                case 'Я':
                    return ".-.-";
            }
            return "";
        }
        private static List<MorzeChar> GetElementsOnChar(char cr)
        {
            List<MorzeChar> list = new List<MorzeChar>();
            string code = GetCodesOnChar(cr);
            for (int i = 0; i < code.Length; i++)
            {
                if(i > 0)
                {
                    list.Add(MorzeChar.element_pause);
                }
                switch (code[i])
                {
                    case '-':
                        list.Add(MorzeChar.dash);
                        break;
                    case '.':
                        list.Add(MorzeChar.dot);
                        break;
                }
            }
            return list;
        }
        private void EncodeChar(List<Command> c, char cr)
        {
            if (cr == ' ' || cr == '\t' || cr == '\n')
            {
                EncodeMorzeChar(c, MorzeChar.word_pause);
            }
            else
            {
                List<MorzeChar> mz = GetElementsOnChar(cr);
                for (int i = 0; i < mz.Count;i++ )
                {
                    EncodeMorzeChar(c, mz[i]);
                }
            }
        }
    }
}
