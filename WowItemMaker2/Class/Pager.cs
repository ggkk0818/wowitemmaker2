using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowItemMaker2
{
    public class Pager
    {
        private int _currentPage;
        private int _totalCount;
        private int _pageSize;

        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            set { _totalCount = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int TotalPage
        {
            get 
            {
                int r = 0;
                if (_pageSize > 0 && _totalCount > 0)
                {
                    r = _totalCount / _pageSize + (_totalCount % _pageSize > 0 ? 1 : 0);
                }
                return r;
            }
        }

        public int Start
        {
            get { return _pageSize * (_currentPage > 0 ? (_currentPage - 1) : 0); }
        }

        public int Limit
        {
            get { return _pageSize; }
        }

        public bool CanFirst
        {
            get { return _currentPage > 1; }
        }

        public bool CanPrev
        {
            get { return _currentPage > 1; }
        }

        public bool CanNext
        {
            get { return _currentPage < TotalPage; }
        }

        public bool CanLast
        {
            get { return _currentPage < TotalPage; }
        }

        public Pager()
        {
            this._pageSize = Configuration.getPageSize();
        }
    }
}
