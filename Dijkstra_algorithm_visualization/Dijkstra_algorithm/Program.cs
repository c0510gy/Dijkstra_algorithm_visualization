using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dijkstra_algorithm
{
    class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
        private static extern IntPtr GetConsoleHandle();
        static IntPtr handler = GetConsoleHandle();

        static Random random = new Random();

        static void Main(string[] args)
        {
            int width = 2000, height = 1500; //그래프가 출력될 공간 크기
            
            int V = 100, E = 1000, K = 1;
            int[,] w = new int[E, 3];
            int[,] Vs = new int[V, 2]; //각 정점의 좌표
            for(int j = 0; j < V; j++) //각 정점의 좌표 랜덤 설정
            {
                Vs[j, 0] = random.Next(30, width - 15 + 1);
                Vs[j, 1] = random.Next(30, height - 15 + 1);
            }

            bool[,] chk = new bool[V, V]; //간선 중복 방지
            for(int j = 0; j < E; j++) //간선 랜덤 설정
            {
                int v1 = random.Next(1, V + 1);
                int v2 = random.Next(1, V + 1);
                if(v1 == v2 || chk[v1 - 1, v2 - 1])
                {
                    j--;
                    continue;
                }
                chk[v1 - 1, v2 - 1] = true;

                w[j, 0] = v1;
                w[j, 1] = v2;
                int dis = (Vs[v1 - 1, 0] - Vs[v2 - 1, 0]) * (Vs[v1 - 1, 0] - Vs[v2 - 1, 0])
                    + (Vs[v1 - 1, 1] - Vs[v2 - 1, 1]) * (Vs[v1 - 1, 1] - Vs[v2 - 1, 1]);
                w[j, 2] = (int)Math.Sqrt((double)dis);
            }

            Dijkstra dijkstra = new Dijkstra(V, E, K);
            for (int j = 0; j < E; j++)
            {
                dijkstra.addWeight(w[j, 0], w[j, 1], w[j, 2]);
            }

            Bitmap visual = new Bitmap(width + 2000, height + 2000);
            Graphics g = Graphics.FromImage(visual);
            
            dijkstra.visualize(ref visual, ref g, ref Vs, width, height, -1, -1, "시작 정점을 제외한 모든 정점의 값을 무한대로 설정 및 시작 정점을 힙(min-heap)에 삽입(PUSH)");

            //콘솔에 출력
            using (var graphics = Graphics.FromHwnd(handler))
            using (var image = (Image)(new Bitmap(visual)))
            {
                graphics.Clear(Color.Black);
                graphics.DrawImage(image, 100, 50, (int)((width + 2000) * 0.2), (int)((height + 2000) * 0.2));
            }
            Console.ReadLine();
            

            while (!dijkstra.next(ref visual, ref g, ref Vs, width, height))
            {
                //콘솔에 출력
                using (var graphics = Graphics.FromHwnd(handler))
                using (var image = (Image)(new Bitmap(visual)))
                {
                    graphics.Clear(Color.Black);
                    graphics.DrawImage(image, 100, 50, (int)((width + 2000) * 0.2), (int)((height + 2000) * 0.2));
                }

                Thread.Sleep(100);
            }

            dijkstra.visualize(ref visual, ref g, ref Vs, width, height, -1, -1, "다익스트라 알고리즘을 이용한 최소경로 계산 완료", true);

            using (var graphics = Graphics.FromHwnd(handler))
            using (var image = (Image)(new Bitmap(visual)))
            {
                graphics.Clear(Color.Black);
                graphics.DrawImage(image, 100, 50, width + 1000, height + 500);
            }

            //dijkstra.run();
            for (int v = 1; v <= V; v++)
            {
                if (dijkstra.C[v] == -1)
                    Console.WriteLine("INF");
                else
                    Console.WriteLine(dijkstra.C[v]);
            }
            
            Console.ReadLine();
        }
    }
}
