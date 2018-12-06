using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

class Dijkstra
{
    class cost
    {
        public int v;
        public int w;
        public cost(int v, int w)
        {
            this.v = v;
            this.w = w;
        }
    }

    public int V, E, K;
    public int[] C; // C[v] := v까지 최단 값
    public int[] preV; // 최단 값을 가지게 되는 이전 경로 정점

    private Heap pC; // 최소인 정점 {cost, vertex}
    private List<cost>[] W; // W[u] = {v, w} : u -> v : w

    public Dijkstra(int V, int E, int K)
    {
        this.V = V;
        this.E = E;
        this.K = K;

        W = new List<cost>[this.V + 1];
        for (int j = 1; j <= this.V; j++)
            W[j] = new List<cost>();
        C = new int[this.V + 1];
        preV = new int[this.V + 1];
        pC = new Heap();

        for (int j = 1; j <= V; j++)
        {
            C[j] = -1; //-1 == INF
            if (j == K)
                C[j] = 0; //시작 정점
        }

        pC.InsertItem(new HeapItem(C[K], K));
    }

    //간선 가중치 추가 u->v : w
    public void addWeight(int u, int v, int w)
    {
        W[u].Add(new cost(v, w));
    }

    //다익스트라 알고리즘 작동
    public void run()
    {
        while (!pC.IsEmpty)
        {
            HeapItem top_item = pC.DeleteItem();
            int v = (int)top_item.Value;

            int cnt = W[v].Count;
            for (int i = 0; i < cnt; i++)
            {
                int u = W[v][i].v;
                if (C[u] == -1 || C[u] > C[v] + W[v][i].w)
                {
                    C[u] = C[v] + W[v][i].w;
                    pC.InsertItem(new HeapItem(C[u], u));
                    preV[u] = v;
                    //pC.PrintHeap();
                }
            }
        }
    }
    
    //시각화를 위한 부분 : RETURN pC.IsEmpty
    public bool next(ref Bitmap visual, ref Graphics g, ref int[,] Vs, int width, int height)
    {
        if (pC.IsEmpty)
        {
            return pC.IsEmpty;
        }

        HeapItem top_item = pC.DeleteItem();
        int v = (int)top_item.Value;

        //시각화
        visualize(ref visual, ref g, ref Vs, width, height, v, -1, "힙(min-heap)에서 팝(POP)한 정점 " + v + "을 선택");
        
        int cnt = W[v].Count;
        for (int i = 0; i < cnt; i++)
        {
            int u = W[v][i].v;

            visualize(ref visual, ref g, ref Vs, width, height, v, i, "선택한 정점 " + v + "와 연결된 정점 " + u + "탐색");

            if (C[u] == -1 || C[u] > C[v] + W[v][i].w)
            {
                C[u] = C[v] + W[v][i].w;
                pC.InsertItem(new HeapItem(C[u], u));
                preV[u] = v;

                //시각화
                visualize(ref visual, ref g, ref Vs, width, height, v, i, "정점 " + u + "로 가는 최소 경로 갱신 및 힙(min-heap)에 삽입(PUSH)");
            }
        }

        return false;
    }

    private int frames = 0;
    public void visualize(ref Bitmap visual, ref Graphics g, ref int[,] Vs, int width, int height, int selected_vertex, int selected_edge, string info_str, bool show_com = false)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.Clear(Color.White);

        int r = 25; //시각화 원 반지름
        // ### 간선 그리기 ###
        for (int j = 1; j <= V; j++)
        {
            int v1 = j;

            int cnt = W[j].Count;
            for (int i = 0; i < cnt; i++)
            {
                int v2 = W[j][i].v;
                Pen p = new Pen(Color.FromArgb(100, 0, 0, 0), 5);
                if(preV[v2] == v1) //선택된 경로
                {
                    p = new Pen(Color.Orange, 5);
                }else if (show_com) //완성된 경로만 보이기
                {
                    continue;
                }
                if(selected_vertex == j && selected_edge == i)
                {
                    p = new Pen(Color.BlueViolet, 10);
                }
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5); //큰 화살표
                p.StartCap = LineCap.Round;
                p.CustomEndCap = bigArrow;

                double m = (double)(Vs[v1 - 1, 1] - Vs[v2 - 1, 1]) / (double)(Vs[v1 - 1, 0] - Vs[v2 - 1, 0]);
                double seta = Math.Atan(m);
                int e = 1;
                if (Vs[v1 - 1, 0] > Vs[v2 - 1, 0] && Vs[v1 - 1, 1] < Vs[v2 - 1, 1]
                    || Vs[v1 - 1, 0] > Vs[v2 - 1, 0] && Vs[v1 - 1, 1] > Vs[v2 - 1, 1])
                {
                    e = -1; //arctan 범위별 조정
                }
                g.DrawLine(p, Vs[v1 - 1, 0], Vs[v1 - 1, 1], Vs[v2 - 1, 0] - (int)(Math.Cos(seta) * r) * e, Vs[v2 - 1, 1] - (int)(Math.Sin(seta) * r) * e);
            }
        }

        // ### 정점 그리기 ###
        for (int j = 0; j < V; j++)
        {
            SolidBrush brush = new SolidBrush(Color.White);
            if (j == K - 1)
                brush = new SolidBrush(Color.Red);
            g.FillEllipse(brush, Vs[j, 0] - r, Vs[j, 1] - r, r * 2, r * 2);
            if(selected_vertex - 1 == j)
                g.DrawEllipse(new Pen(Color.BlueViolet, 10), Vs[j, 0] - r, Vs[j, 1] - r, r * 2, r * 2);
            else
                g.DrawEllipse(new Pen(Color.Black, 5), Vs[j, 0] - r, Vs[j, 1] - r, r * 2, r * 2);

            if(j + 1 >= 100)
            {
                g.DrawString((j + 1) + "", new Font("나눔고딕", 18), new SolidBrush(Color.Black), Vs[j, 0] - 25, Vs[j, 1] - 15);
            }
            else if (j + 1 >= 10)
            {
                g.DrawString((j + 1) + "", new Font("나눔고딕", 25), new SolidBrush(Color.Black), Vs[j, 0] - 25, Vs[j, 1] - 20);
                //g.DrawString((j + 1) + "", new Font("나눔고딕", 9), new SolidBrush(Color.Black), Vs[j, 0] - 9, Vs[j, 1] - 8);
            }
            else
            {
                g.DrawString((j + 1) + "", new Font("나눔고딕", 30), new SolidBrush(Color.Black), Vs[j, 0] - 20, Vs[j, 1] - 25);
                //g.DrawString((j + 1) + "", new Font("나눔고딕", 12), new SolidBrush(Color.Black), Vs[j, 0] - 7, Vs[j, 1] - 10);
            }
        }


        Font font = new Font("나눔고딕", 30);

        // ### Vertext Cost Table 그리기 ###
        int dw = 150, dh = 60;
        g.DrawString(" Vertex", font, new SolidBrush(Color.Black), width + dw, dh + 5);
        g.DrawString("  Cost", font, new SolidBrush(Color.Black), width + dw * 2, dh + 5);
        g.DrawLine(new Pen(Color.Black, 3), width + dw * 2, dh, width + dw * 2, dh * (V / 2 + 2));
        g.DrawLine(new Pen(Color.Black, 3), width + dw, dh, width + dw, dh * (V / 2 + 2));
        g.DrawLine(new Pen(Color.Black, 3), width + dw * 3, dh, width + dw * 3, dh * (V / 2 + 2));
        g.DrawLine(new Pen(Color.Black, 3), width + dw, dh, width + dw * 3, dh);
        for (int v = 1; v <= V / 2; v++)
        {
            g.DrawString("     " + v, font, new SolidBrush(Color.Black), width + dw, dh * (v + 1) + 5);
            if (C[v] == -1)
                g.DrawString(" ∞", font, new SolidBrush(Color.Black), width + dw * 2, dh * (v + 1) + 5);
            else
                g.DrawString(" " + C[v], font, new SolidBrush(Color.Black), width + dw * 2, dh * (v + 1) + 5);
            g.DrawLine(new Pen(Color.Black, 3), width + dw, dh * (v + 1), width + dw * 3, dh * (v + 1));
        }
        g.DrawLine(new Pen(Color.Black, 3), width + dw, dh * (V / 2 + 2), width + dw * 3, dh * (V / 2 + 2));

        //V값이 너무 크기 때문에 두 부분으로 나누어 출력
        g.DrawString(" Vertex", font, new SolidBrush(Color.Black), dw * 2 + width + dw, dh + 5);
        g.DrawString("  Cost", font, new SolidBrush(Color.Black), dw * 2 + width + dw * 2, dh + 5);
        g.DrawLine(new Pen(Color.Black, 3), dw * 2 + width + dw * 2, dh, dw * 2 + width + dw * 2, dh * (V / 2 + 2));
        g.DrawLine(new Pen(Color.Black, 3), dw * 2 + width + dw, dh, dw * 2 + width + dw, dh * (V / 2 + 2));
        g.DrawLine(new Pen(Color.Black, 3), dw * 2 + width + dw * 3, dh, dw * 2 + width + dw * 3, dh * (V / 2 + 2));
        g.DrawLine(new Pen(Color.Black, 3), dw * 2 + width + dw, dh, dw * 2 + width + dw * 3, dh);
        for (int v = 1; v <= V / 2; v++)
        {
            g.DrawString("     " + (v + V / 2), font, new SolidBrush(Color.Black), dw * 2 + width + dw, dh * (v + 1) + 5);
            if (C[v + V / 2] == -1)
                g.DrawString(" ∞", font, new SolidBrush(Color.Black), dw * 2 + width + dw * 2, dh * (v + 1) + 5);
            else
                g.DrawString(" " + C[v + V / 2], font, new SolidBrush(Color.Black), dw * 2 + width + dw * 2, dh * (v + 1) + 5);
            g.DrawLine(new Pen(Color.Black, 3), dw * 2 + width + dw, dh * (v + 1), dw * 2 + width + dw * 3, dh * (v + 1));
        }
        g.DrawLine(new Pen(Color.Black, 3), dw * 2 + width + dw, dh * (V / 2 + 2), dw * 2 + width + dw * 3, dh * (V / 2 + 2));

        // ### HEAP 구조 그리기 ###
        Bitmap visual_heap = new Bitmap(2000, 1000);
        pC.visualHeap(ref visual_heap);
        g.DrawImage(visual_heap, 50, height + 200);

        font = new Font("나눔고딕", 60);
        g.DrawString(info_str, font, new SolidBrush(Color.Orange), 50, height + 1800);

        // 프레임 저장
        visual.Save(@"datas\" + frames + ".png", System.Drawing.Imaging.ImageFormat.Png); frames++;
    }
}