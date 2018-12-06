using System;
using System.Drawing;
using System.Drawing.Drawing2D;
/*
    Heap.cs와 HeapItem.cs 부분인 Heap구조 클래스는
    http://periar.tistory.com/entry/C-%EC%9A%B0%EC%84%A0%EC%88%9C%EC%9C%84-%ED%81%90
    에서 가져와 시각화 용도에 맞게(프로젝트 목표에 맞게) 일부 수정했습니다.
    해당 출처가 원 저작자의 출처인지는 알 수 없습니다.
*/
class Heap
{
    int _lastIndex = -1;
    int _capacity = 0;
    HeapItem[] _array = new HeapItem[0];

    public bool IsEmpty { get { return _lastIndex == -1; } }
    public int RemainItems { get { return _lastIndex + 1; } }

    public Heap()
    {

    }

    public void InsertItem(HeapItem item)
    {
        if (_lastIndex + 1 == _capacity)
        {
            _capacity += 2;
            Array.Resize(ref _array, _capacity);
        }

        _array[++_lastIndex] = item;

        int currentPosition = _lastIndex;
        int parentPosition = getParentIndex(currentPosition);

        while (currentPosition > 0 && (_array[currentPosition].Ranking < _array[parentPosition].Ranking))
        {
            itemSwap(currentPosition, parentPosition);

            currentPosition = parentPosition;
            parentPosition = getParentIndex(currentPosition);
        }
    }

    public HeapItem DeleteItem()
    {
        if (IsEmpty)
            return null;

        HeapItem root = _array[0];
        _array[0] = _array[_lastIndex];
        _array[_lastIndex] = null;
        _lastIndex--;

        int parent = 0;
        int leftChild = getLeftChild(parent);
        int rightChild = leftChild + 1;

        while (true)
        {
            int selectedChild = 0;

            if (leftChild > _lastIndex)
                break;

            if (rightChild > _lastIndex)
                selectedChild = leftChild;
            else
            {
                if (_array[leftChild].Ranking > _array[rightChild].Ranking)
                    selectedChild = rightChild;
                else
                    selectedChild = leftChild;
            }
            if (_array[selectedChild].Ranking < _array[parent].Ranking)
            {
                itemSwap(selectedChild, parent);
                parent = selectedChild;
            }
            else
                break;

            leftChild = getLeftChild(parent);
            rightChild = leftChild + 1;
        }

        if (_lastIndex < _capacity / 2)
        {
            _capacity /= 2;
            Array.Resize(ref _array, _capacity);
        }

        return root;
    }


    public void PrintHeap()
    {
        foreach (HeapItem item in _array)
        {
            if (item != null)
                Console.WriteLine(string.Format("{0}, {1}", item.Ranking, item.Value));
        }
        Console.WriteLine();
    }

    //힙 구조 시각화
    private int r_ = 25; // 시각화 원 반지름
    public void visualHeap(ref Bitmap visual)
    {
        Graphics g = Graphics.FromImage(visual);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.Clear(Color.White);

        int rx = visual.Width / 2, ry = 50, dx = visual.Width / 2, dy = 90;
        //간선 그리기
        if (_lastIndex >= 0)
        {
            visualHeap_draw_line(ref g, rx, ry, dx / 2, dy, 0);
        }

        //정점 그리기
        if (_lastIndex >= 0)
        {
            g.FillEllipse(new SolidBrush(Color.White), rx - r_, ry - r_, r_ * 2, r_ * 2);
            g.DrawEllipse(new Pen(Color.Black, 5), rx - r_, ry - r_, r_ * 2, r_ * 2);
            if ((int)_array[0].Value >= 100) //글자 길이별로 다르게 출력
                g.DrawString(_array[0].Value + "", new Font("나눔고딕", 18), new SolidBrush(Color.Black), rx - 25, ry - 15);
            else if ((int)_array[0].Value >= 10)
                g.DrawString(_array[0].Value + "", new Font("나눔고딕", 25), new SolidBrush(Color.Black), rx - 25, ry - 20);
            else
                g.DrawString(_array[0].Value + "", new Font("나눔고딕", 30), new SolidBrush(Color.Black), rx - 20, ry - 25);
            g.DrawString("(" + _array[0].Ranking + ")", new Font("나눔고딕", 20), new SolidBrush(Color.Black), rx - 20 - _array[0].Ranking.ToString().Length * 2, ry + 30);
            
            visualHeap_draw(ref g, rx, ry, dx / 2, dy, 0);
        }
    }
    private void visualHeap_draw_line(ref Graphics g, int x, int y, int dx, int dy, int i)
    {
        //left child
        int l = getLeftChild(i);
        if (l <= _lastIndex)
        {
            int l_x = x - dx, l_y = y + dy;
            g.DrawLine(new Pen(Color.Black, 5), x, y, l_x, l_y);
            visualHeap_draw_line(ref g, l_x, l_y, dx / 2, dy, l);
        }

        //right child
        int r = getRightChild(i);
        if (r <= _lastIndex)
        {
            int r_x = x + dx, r_y = y + dy;
            g.DrawLine(new Pen(Color.Black, 5), x, y, r_x, r_y);
            visualHeap_draw_line(ref g, r_x, r_y, dx / 2, dy, r);
        }
    }
    private void visualHeap_draw(ref Graphics g, int x, int y, int dx, int dy, int i)
    {
        //left child
        int l = getLeftChild(i);
        if(l <= _lastIndex)
        {
            int l_x = x - dx, l_y = y + dy;
            g.FillEllipse(new SolidBrush(Color.White), l_x - r_, l_y - r_, r_ * 2, r_ * 2);
            g.DrawEllipse(new Pen(Color.Black, 5), l_x - r_, l_y - r_, r_ * 2, r_ * 2);
            if ((int)_array[l].Value >= 100)
                g.DrawString(_array[l].Value + "", new Font("나눔고딕", 18), new SolidBrush(Color.Black), l_x - 25, l_y - 15);
            else if ((int)_array[l].Value >= 10)
                g.DrawString(_array[l].Value + "", new Font("나눔고딕", 25), new SolidBrush(Color.Black), l_x - 25, l_y - 20);
            else
                g.DrawString(_array[l].Value + "", new Font("나눔고딕", 30), new SolidBrush(Color.Black), l_x - 20, l_y - 25);
            g.DrawString("(" + _array[l].Ranking + ")", new Font("나눔고딕", 20), new SolidBrush(Color.Black), l_x - 20 - _array[l].Ranking.ToString().Length * 2, l_y + 30);

            visualHeap_draw(ref g, l_x, l_y, dx / 2, dy, l);
        }
        
        //right child
        int r = getRightChild(i);
        if (r <= _lastIndex)
        {
            int r_x = x + dx, r_y = y + dy;
            g.FillEllipse(new SolidBrush(Color.White), r_x - r_, r_y - r_, r_ * 2, r_ * 2);
            g.DrawEllipse(new Pen(Color.Black, 5), r_x - r_, r_y - r_, r_ * 2, r_ * 2);
            if ((int)_array[r].Value >= 100)
                g.DrawString(_array[r].Value + "", new Font("나눔고딕", 18), new SolidBrush(Color.Black), r_x - 25, r_y - 15);
            else if ((int)_array[r].Value >= 10)
                g.DrawString(_array[r].Value + "", new Font("나눔고딕", 25), new SolidBrush(Color.Black), r_x - 25, r_y - 20);
            else
                g.DrawString(_array[r].Value + "", new Font("나눔고딕", 30), new SolidBrush(Color.Black), r_x - 20, r_y - 25);
            g.DrawString("(" + _array[r].Ranking + ")", new Font("나눔고딕", 20), new SolidBrush(Color.Black), r_x - 20 - _array[r].Ranking.ToString().Length * 2, r_y + 30);

            visualHeap_draw(ref g, r_x, r_y, dx / 2, dy, r);
        }
    }

    private void itemSwap(int index0, int index1)
    {
        HeapItem item = _array[index0];
        _array[index0] = _array[index1];
        _array[index1] = item;
    }

    private int getLeftChild(int parent)
    {
        return 2 * parent + 1;
    }
    private int getRightChild(int parent)
    {
        return 2 * parent + 2;
    }
    private int getParentIndex(int childPosition)
    {
        return (childPosition - 1) / 2;
    }
}