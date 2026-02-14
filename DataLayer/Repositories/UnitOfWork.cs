using DataLayer.Context;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private bool disposed = false;
        private IPostRepository _postRepository;
        private IAttachmentRepository _attachmentRepository;

        public UnitOfWork(AppDbContext context) { _context = context; }

        #region Repositories

        public IPostRepository PostRepository
        {
            get
            {
                if (_postRepository == null)
                {
                    _postRepository = new PostRepository(_context);
                }
                return _postRepository;
            }
        }

        public IAttachmentRepository AttachmentRepository
        {
            get
            {
                if (_attachmentRepository == null)
                {
                    _attachmentRepository = new AttachmentRepository(_context);
                }
                return _attachmentRepository;
            }
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            //Dispose(true);
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
