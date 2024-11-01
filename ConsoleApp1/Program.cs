// See https://aka.ms/new-console-template for more information
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.WriteLine("Hello, World!");

char Choice(params char[] datas)
{
    while (true)
    {
        var key = Console.ReadKey();
        var aa = key.KeyChar;
        if(Array.Exists(datas, x => x ==aa))
        {
            return aa;
        }
        else
        {
            var pos = Console.GetCursorPosition();
            Console.SetCursorPosition(pos.Left - 1, pos.Top);
            Console.Beep();
        }
    }
}

var ccc = Enumerable.Range('1', '9')
    .Select(x => (char)x);

var ch = Choice('1', '2');

