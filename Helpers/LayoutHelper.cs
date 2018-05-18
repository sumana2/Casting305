using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.Hosting;

namespace WebApplication1.Helpers
{
    public class LayoutHelper : IDisposable
    {
        public List<Rectangle> Positions { get; set; }

        public List<Image> Elements { get; set; }

        public List<string> Paths { get; set; }

        public int Spacing { get; set; }

        public int ContainerWidth { get; set; }

        public int ColumnCount { get; set; }

        public LayoutHelper(List<string> paths, int spacing, int columnCount)
        {
            Elements = new List<Image>();
            Positions = new List<Rectangle>();
            Spacing = spacing;
            ContainerWidth = 1280;
            ColumnCount = columnCount;
            Paths = new List<string>();

            for (int i = 0, k = 0; i < paths.Count && k < columnCount; i++)
            {
                var img = Image.FromFile(HostingEnvironment.MapPath(paths[i]));
                if (img.Width < ContainerWidth)
                {
                    Elements.Add(img);
                    Paths.Add(paths[i]);
                    k++;
                }
            }
        }

        public void ComputeFixedColumns()
        {
            int i, n, i_column, col = 0, src_width, src_height, dst_width, dst_height, colMin;
            decimal aspect;

            var columnWidth = (int)Math.Round((decimal)(ContainerWidth - (ColumnCount - 1) * Spacing) / ColumnCount);

            var cols = new List<int>();
            for (i = 0; i < ColumnCount; i++)
            {
                cols.Add(0);
            }

            // distribute images to columns as evenly as possible
            for (i = 0, n = Elements.Count; i < n; i++)
            {
                src_width = Elements[i].Width;
                src_height = Elements[i].Height;
                aspect = (decimal)src_width / (decimal)src_height;
                dst_width = columnWidth;
                dst_height = (int)Math.Round((decimal)dst_width / aspect);

                // pick the column that is least-filled
                colMin = cols.Min();
                for (i_column = 0; i_column < ColumnCount; i_column++)
                {
                    if (cols[i_column] <= colMin)
                    {
                        colMin = cols[i_column];
                        col = i_column;
                    }
                }

                // update the column heights
                var y = cols[col];
                var adj = (y != 0) ? Spacing : 0;
                y += adj;
                cols[col] += dst_height + adj;

                var r = new Rectangle((col * columnWidth + col * Spacing), y, dst_width, dst_height);
                r.X *= (int)((float)914400 / 96);
                r.Y *= (int)((float)914400 / 96);
                r.Height *= (int)((float)914400 / 96);
                r.Width *= (int)((float)914400 / 96);

                Positions.Add(r);
            }
        }

        public void ComputeFixedPartition()
        {
            int i, j, k, n, height, elementCount;
            int idealHeight = (ContainerWidth / ColumnCount);
            string align = "center";

            // calculate aspect ratio of all photos
            decimal aspect;
            List<decimal> aspects = new List<decimal>();
            List<decimal> aspects100 = new List<decimal>();

            for (i = 0, n = Elements.Count; i < n; i++)
            {
                aspect = (decimal)Elements[i].Width / (decimal)Elements[i].Height;
                aspects.Add(aspect);
                aspects100.Add(Math.Round(aspect * 100));
            }

            // calculate total width of all photos
            decimal summedWidth = 0;
            for (i = 0, n = aspects.Count; i < n; i++)
            {
                summedWidth += aspects[i] * idealHeight;
            }

            // calculate rows needed
            int rowsNeeded = (int)Math.Round(summedWidth / ContainerWidth);

            // adjust photo sizes
            if (rowsNeeded < 1)
            {
                // (2a) Fallback to just standard size
                int xSum = 0, ySum, width;
                elementCount = Elements.Count;

                decimal padLeft = 0;
                if (align == "center")
                {
                    decimal spaceNeeded = (elementCount - 1) * Spacing;
                    for (i = 0; i < elementCount; i++)
                    {
                        spaceNeeded += Math.Round(idealHeight * aspects[i]) - (Spacing * (elementCount - 1) / elementCount);
                    }
                    padLeft = Math.Floor((ContainerWidth - spaceNeeded) / 2);
                }

                for (i = 0; i < elementCount; i++)
                {
                    width = (int)Math.Round(idealHeight * aspects[i]) - (Spacing * (elementCount - 1) / elementCount);
                    Positions.Add(new Rectangle((int)(padLeft + xSum), 0, width, idealHeight));

                    xSum += width;
                    if (i != n - 1)
                    {
                        xSum += Spacing;
                    }
                }

                ySum = idealHeight;
            }
            else {
                // (2b) Distribute photos over rows using the aspect ratio as weight
                decimal[][] partitions = LinearPartition(aspects100, rowsNeeded);
                int index = 0;
                int ySum = 0, xSum, width;

		        for (i = 0, n = partitions.Length; i<n; i++)
                {
			        int element_index = index;
                    decimal summedRatios = 0;

			        for (j = 0, k = partitions[i].Length; j<k; j++)
                    {
				        summedRatios += aspects[element_index + j];
				        index++;
			        }

                    xSum = 0;
			        height = (int)Math.Round(ContainerWidth / summedRatios);
			        elementCount = partitions[i].Length;
			        for (j = 0; j<elementCount; j++)
                    {
				        width = (int)Math.Round((ContainerWidth - (elementCount - 1) * Spacing) / summedRatios* aspects[element_index + j]);
                        Positions.Add(new Rectangle(xSum, ySum, width, height));

                        xSum += width;
				        if (j != elementCount - 1)
                        {
					        xSum += Spacing;
				        }
			        }

			        ySum += height;

			        if (i != n - 1)
                    {
				        ySum += Spacing;
			        }
		        }
	        }

            for (i = 0; i < Positions.Count; i++)
            {
                var r = Positions[i];
                r.X *= (int)((float)914400 / 96);
                r.Y *= (int)((float)914400 / 96);
                r.Height *= (int)((float)914400 / 96);
                r.Width *= (int)((float)914400 / 96);
                Positions[i] = r;
            }
        }

        public decimal[][] LinearPartition(List<decimal> seq, int k)
        {
            int[][] table, solution;
            int i, j, n, x, y, _i, _j, _k, _l, _ref, _ref1;
            int _m, _nn;

            n = seq.Count;

            if (k <= 0)
            {
                return new decimal[0][];
            }

            if (k > n)
            {
                return seq.Select(s => new decimal[(int)s]).ToArray();
            }

            List<int[]> _results = new List<int[]>();
            for (y = _i = 0; 0 <= n ? _i < n : _i > n; y = 0 <= n ? ++_i : --_i)
            {
                List<int> _results1 = new List<int>();
                for (x = _j = 0; 0 <= k ? _j < k : _j > k; x = 0 <= k ? ++_j : --_j)
                {
                    _results1.Add(0);
                }

                _results.Add(_results1.ToArray());
            }
            table = _results.ToArray();

            _results = new List<int[]>();
            for (y = _i = 0, _ref = n - 1; 0 <= _ref ? _i < _ref : _i > _ref; y = 0 <= _ref ? ++_i : --_i)
            {
                List<int> _results1 = new List<int>();
                for (x = _j = 0, _ref1 = k - 1; 0 <= _ref1 ? _j < _ref1 : _j > _ref1; x = 0 <= _ref1 ? ++_j : --_j)
                {
                    _results1.Add(0);
                }

                _results.Add(_results1.ToArray());
            }
            solution = _results.ToArray();

            for (i = _i = 0; 0 <= n? _i<n : _i > n; i = 0 <= n? ++_i : --_i) {
		        table[i][0] = (int)seq[i] + (i > 0 ? table[i - 1][0] : 0);
	        }
	        for (j = _j = 0; 0 <= k? _j<k : _j > k; j = 0 <= k? ++_j : --_j) {
		        table[0][j] = (int)seq[0];
	        }
	        for (i = _k = 1; 1 <= n? _k<n : _k > n; i = 1 <= n? ++_k : --_k) {
		        for (j = _l = 1; 1 <= k? _l<k : _l > k; j = 1 <= k? ++_l : --_l) {

			        List<int[]> g = new List<int[]>();
			        for (x = _m = 0; 0 <= i? _m<i : _m > i; x = 0 <= i? ++_m : --_m) {
                        g.Add(new int[] { Math.Max(table[x][j - 1], table[i][0] - table[x][0]), x });
			        }

                    int[][] m = g.ToArray();
			        int minValue = 0, minIndex = 0;
                    for (_m = 0, _nn = m.Length; _m < _nn; _m++)
                    {
                        if (_m == 0 || m[_m][0] < minValue)
                        {
                            minValue = m[_m][0];
                            minIndex = _m;
                        }
                    }

                    int[] d = m[minIndex];
                    table[i][j] = d[0];
                    solution[i - 1][j - 1] = d[1];
                }
	        }

            n = n - 1;
            k = k - 2;
            List<decimal[]> ans = new List<decimal[]>();
            while (k >= 0)
            {
                List<decimal> d_results1 = new List<decimal>();
                for (i = _m = _ref = solution[n - 1][k] + 1, _ref1 = n + 1; _ref <= _ref1 ? _m < _ref1 : _m > _ref1; i = _ref <= _ref1 ? ++_m : --_m)
                {
                    d_results1.Add(seq[i]);
                }

                ans.Add(d_results1.ToArray());

                n = solution[n - 1][k];
                k = k - 1;
                if (n == 0) break;
            }

            List<decimal> d_results2 = new List<decimal>();
            for (i = _m = 0, _ref = n + 1; 0 <= _ref ? _m < _ref : _m > _ref; i = 0 <= _ref ? ++_m : --_m)
            {
                d_results2.Add(seq[i]);
            }

            ans.Add(d_results2.ToArray());
            ans.Reverse();

            return ans.ToArray();
        }

        public void Dispose()
        {
            foreach (var e in Elements)
            {
                e.Dispose();
            }
        }
    }
}