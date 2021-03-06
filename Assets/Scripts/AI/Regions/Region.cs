﻿using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechWars.AI.Regions
{
    public class Region
    {
        List<RegionStrip> leftList;
        List<RegionStrip> rightList; // right list contains 0

        int offset;

        public int Width { get { return leftList.Count + rightList.Count; } }
        
        int RelativeRight { get { return rightList.Count - 1; } }
        int RelativeLeft { get { return -leftList.Count; } }

        public int Left { get { return RelativeLeft + offset; } }
        public int Right { get { return RelativeRight + offset; } }

        public int Area { get; private set; }

        public IEnumerable<IVector2> AllTiles
        {
            get
            {
                for (int i = RelativeLeft, x = Left; x <= Right; i++, x++)
                {
                    var strip = GetStrip(i);
                    for (int j = 0; j < strip.Count; j++)
                    {
                        var stripPart = strip[j];
                        for (int y = stripPart.Start; y <= stripPart.End; y++)
                            yield return new IVector2(x, y);
                    }
                }
            }
        }
        
        public event System.Action RegionChanged;

        public Region()
        {
            leftList = new List<RegionStrip>();
            rightList = new List<RegionStrip>();
        }

        public void AddTile(IVector2 tile)
        {
            AddTile(tile.X, tile.Y);
        }

        public void AddTile(int x, int y)
        {
            if (Width == 0)
                offset = x;

            int relativeX = x - offset;
            List<RegionStrip> list;
            int idx = GetListAndIndex(relativeX, out list);

            while (list.Count <= idx)
                list.Add(new RegionStrip());

            list[idx].AddTile(y);
            Area++;

            if (RegionChanged != null) RegionChanged();
        }

        public void RemoveTile(IVector2 tile)
        {
            RemoveTile(tile.X, tile.Y);
        }

        public void RemoveTile(int x, int y)
        {
            if (Width == 0)
                throw new System.Exception("Region is empty.");

            int relativeX = x - offset;
            List<RegionStrip> list;
            int idx = GetListAndIndex(relativeX, out list);

            if (idx >= list.Count)
                throw new System.Exception("Tile at [{0}, {1}] is not inside region.");

            list[idx].RemoveTile(y);
            Area--;

            TrimList(list);
            Normalize();

            if (RegionChanged != null) RegionChanged();
        }

        public bool IsInside(IVector2 point)
        {
            return IsInside(point.X, point.Y);
        }

        public bool IsInside(int x, int y)
        {
            int relativeX = x - offset;
            List<RegionStrip> list;
            int idx = GetListAndIndex(relativeX, out list);

            if (idx >= list.Count) return false;
            return list[idx].IsInside(y);
        }

        void TrimList(List<RegionStrip> list)
        {
            while (list.Count > 0 && (list.Last() == null || list.Last().Empty))
                list.RemoveAt(list.Count - 1);
        }

        void Normalize()
        {
            if (Width == 0)
                offset = 0;
            else
            {
                if (rightList.Count == 0)
                {
                    var deltaOffset = -1;
                    foreach (var rs in leftList)
                    {
                        if (!rs.Empty) break;
                        deltaOffset--;
                    }
                    MoveOffset(deltaOffset);
                    Unnullify();
                }
                else if (leftList.Count == 0)
                {
                    var deltaOffset = 0;
                    foreach (var rs in rightList)
                    {
                        if (!rs.Empty) break;
                        deltaOffset++;
                    }
                    MoveOffset(deltaOffset);
                    Unnullify();
                }
            }
        }

        RegionStrip GetStrip(int relativeX)
        {
            List<RegionStrip> list;
            int idx = GetListAndIndex(relativeX, out list);
            return list[idx];
        }

        RegionStrip TakeStrip(int relativeX)
        {
            List<RegionStrip> list;
            int idx = GetListAndIndex(relativeX, out list);

            var strip = list[idx];
            if (idx == list.Count - 1)
                list.RemoveAt(idx);
            else list[idx] = null;
            return strip;
        }

        void PutStrip(int relativeX, RegionStrip strip)
        {
            List<RegionStrip> list;
            int idx = GetListAndIndex(relativeX, out list);

            while (list.Count < idx)
                list.Add(null);
            if (list.Count == idx)
                list.Add(strip);
            else list[idx] = strip;
        }

        public RegionStrip[] ToStripsTable()
        {
            var stripsTable = new RegionStrip[Width];
            for (int relativeX = RelativeLeft, i = 0; relativeX <= RelativeRight; relativeX++, i++)
                stripsTable[i] = GetStrip(relativeX);
            return stripsTable;
        }

        void MoveOffset(int deltaOffset)
        {
            if (deltaOffset == 0) return;

            offset += deltaOffset;
            var deltaStrips = -deltaOffset;

            int oldLeft = RelativeLeft;
            int oldRight = RelativeRight;
            bool putBack = false;
            if (deltaStrips > 0)
            {
                for (int i = oldRight; i >= oldLeft; i--)
                    MoveOneStrip(i, deltaStrips, ref putBack);
                TrimList(leftList);
            }
            else
            {
                for (int i = oldLeft; i <= oldRight; i++)
                    MoveOneStrip(i, deltaStrips, ref putBack);
                TrimList(rightList);
            }
            Unnullify();
        }

        void ChangeOffset(int newOffset)
        {
            MoveOffset(newOffset - offset);
        }

        void MoveOneStrip(int stripIdx, int delta, ref bool putBack)
        {
            var strip = TakeStrip(stripIdx);
            if (!putBack && !strip.Empty)
                putBack = true;
            if (putBack) PutStrip(stripIdx + delta, strip);
        } 

        void Unnullify()
        {
            for (int i = 0; i < leftList.Count; i++)
                if (leftList[i] == null)
                    leftList[i] = new RegionStrip();
            for (int i = 0; i < rightList.Count; i++)
                if (rightList[i] == null)
                    rightList[i] = new RegionStrip();
        }

        int GetListAndIndex(int relativeX, out List<RegionStrip> list)
        {
            int idx;
            if (relativeX < 0)
            {
                idx = -relativeX - 1;
                list = leftList;
            }
            else
            {
                idx = relativeX;
                list = rightList;
            }
            return idx;
        }

        public int CalculateVerticalStart()
        {
            return leftList.Concat(rightList).Where(rs => !rs.Empty).Min(s => s.Start);
        }

        public int CalculateVerticalEnd()
        {
            return leftList.Concat(rightList).Where(rs => !rs.Empty).Max(s => s.End);
        }

        public override string ToString()
        {
            var stripsTable = ToStripsTable();

            var sb = new StringBuilder()
                .AppendFormatLine("Width: {0}", Width)
                .AppendFormatLine("XOffset: {0}", offset)
                .AppendFormatLine("RelativeLeft: {0}", RelativeLeft)
                .AppendFormatLine("RelativeRight: {0}", RelativeRight)
                .AppendFormatLine("Left: {0}", Left)
                .AppendFormatLine("Right: {0}", Right)
                .AppendLine();
            
            if (!stripsTable.Empty())
            {
                for (int i = 0; i < Width; i++)
                    sb.AppendFormatLine("strips[{0}]: {1}", i, stripsTable[i].ToString());
                sb.AppendLine();

                var nonEmptyStrips = stripsTable.Where(rs => rs.Count > 0);
                if (nonEmptyStrips.Empty())
                    for (int i = 0; i < Width; i++)
                        sb.Append(".");
                else
                {
                    int minStart = nonEmptyStrips.Min(rs => rs.Start);
                    int maxEnd = nonEmptyStrips.Max(rs => rs.End);

                    for (int y = maxEnd; y >= minStart; y--)
                    {
                        for (int x = Left; x <= Right; x++)
                            sb.Append(IsInside(x, y) ? 'x' : '.');
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }
    }
}
