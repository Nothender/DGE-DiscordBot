using Discord;
using Discord.Commands;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGameEngine.Rendering
{
    //The StringFrameBuffer will not be updated after V0.3.0 + the StringFrameBuffer has rendering issues -> now using the EmbedFrameBuffer is recommended
    public class StringFrameBuffer
    {

        private int width;
        private int height;
        public event EventHandler Resize;
        public string clearColor;
        public string[,] buffer;

        public StringFrameBuffer(int width, int height, string clear_color)
        {
            this.width = width;
            this.height = height;
            this.clearColor = clear_color;
            buffer = new string[height, width];
            Clear();
        }

        public void SetSize(int width, int height)
        {
            buffer = new string[height, width];
            Clear();
            Resize?.Invoke(this, new EventArgs());
        }

        public void Clear()
        {
            for(int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    buffer[i, j] = clearColor;
                }
            }
        }

        public void Draw(int x, int y, string px)
        {
            buffer[y, x] = px;
        }

        public string BufferToString()
        {
            string res = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    res += buffer[i, j];
                }
                res += '\n';
            }
            return res;
        }

        public string BufferToStringAndClear()
        {
            string res = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    res += buffer[i, j];
                    buffer[i, j] = clearColor;
                }
                res += '\n';
            }
            return res;
        }

        public string[] BufferToStringArray()
        {
            List<string> res = new List<string>();
            res.Add("");
            int pointer = 0;
            for (int i = 0; i < height; i++)
            {
                if (res[pointer].Length + width * buffer[i, 0].Length > 1500)
                {
                    res.Add("");
                    pointer++;
                }
                for (int j = 0; j < width; j++)
                {
                    res[pointer] += buffer[i, j];
                }
                res[pointer] += '\n';
            }
            return res.ToArray();
        }

        public string[] BufferToStringArrayAndClear()
        {
            List<string> res = new List<string>();
            res.Add("");
            int pointer = 0;
            for (int i = 0; i < height; i++)
            {
                if (res[pointer].Length + width * buffer[i, 0].Length > 1500)
                {
                    res.Add("");
                    pointer++;
                }
                for (int j = 0; j < width; j++)
                {
                    res[pointer] += buffer[i, j];
                    buffer[i, j] = clearColor;
                }
                res[pointer] += '\n';
            }
            return res.ToArray();
        }

        public async void Display(SocketCommandContext context)
        {
            foreach (string str in BufferToStringArray())
                await context.Channel.SendMessageAsync(str);
        }

        public async void DisplayAndClear(SocketCommandContext context)
        {
            foreach (string str in BufferToStringArrayAndClear())
                await context.Channel.SendMessageAsync(str);

        }

    }
}
